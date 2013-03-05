using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Globalization;

namespace NTwitter
{
    class URLConnectionHttpClient : IHttpClient
    {
        #region private field

        private const int c_DefaultTimeout = 10 * 1000;
        private string username;
        private string password;

        #endregion

        #region private methods

        /// <summary>Builds a list of parameter for a GET-Request</summary>
        private static string GetParamString(IDictionary<string, string> vars)
        {
            int i = 0;
            string paramlist = String.Empty;
            foreach (KeyValuePair<string, string> kv in vars)
            {
                if (kv.Value != null)
                {
                    paramlist += kv.Key + "=" + kv.Value;
                    if (i < vars.Count - 1)
                    {
                        paramlist += "&";
                    }
                }
                i++;
            }
            return paramlist;
        }

        private static string GetResponseText(HttpWebRequest request)
        {
            string responseText = null;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream respStream = response.GetResponseStream())
            {
                Uri respuri = response.ResponseUri;
                responseText = GetStreamText(respStream);
                HttpStatusCode statusCode = response.StatusCode;
                response.Close();

                // Check if call was successfull
                switch (statusCode)
                {
                    case HttpStatusCode.OK:
                        break;

                    case HttpStatusCode.Forbidden:
                        throw new TwitterException.E403(responseText + " " + respuri);

                    case HttpStatusCode.NotFound:
                        throw new TwitterException.E404(responseText + " " + respuri);

                    default:
                        int numericStatus = (int)statusCode;
                        if ((numericStatus >= 500) && (numericStatus <= 600))
                        {
                            throw new TwitterException.E50X(responseText + " " + respuri);
                        }
                        else
                        {
                            bool rateLimitExceeded = responseText.Contains("Rate limit exceeded");
                            if (rateLimitExceeded)
                            {
                                throw new TwitterException.RateLimit(responseText);
                            }
                            throw new TwitterException(responseText + " " + respuri);
                        }
                }
            }
            return responseText;
        }

        private static string GetStreamText(Stream respStream)
        {
            string responseText = null;
            using (StreamReader sr = new StreamReader(respStream))
            {
                responseText = sr.ReadToEnd();
            }
            return responseText;
        }

        #endregion

        #region constructor

        public URLConnectionHttpClient()
        {
        }

        public URLConnectionHttpClient(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        #endregion

        #region public methods

        public bool CanAuthenticate()
        {
            return ((!String.IsNullOrEmpty(username)) && !String.IsNullOrEmpty(password));
        }

        public string GetPage(Uri uri, IDictionary<string, string> vars, bool authenticate)
        {
            string httpuri = uri.ToString();
            if ((vars != null) && (vars.Count > 0))
            {
                string paramlist = GetParamString(vars);
                httpuri += "?" + paramlist;
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(httpuri));
            if (authenticate)
            {
                request.Credentials = new NetworkCredential(username, password);
            }
            request.UserAgent = "NTwitter/" + Twitter.Version;
            request.ReadWriteTimeout = c_DefaultTimeout;

            string responseText = null;
            try
            {
                responseText = GetResponseText(request);
            }
            catch (WebException ex)
            {
                throw new TwitterException(
                    String.Format(CultureInfo.InvariantCulture,"An error occured accesing page {0}", uri)
                    , ex);
            }
            return responseText;
        }

        public string Post(Uri uri, IDictionary<string, string> vars, bool authenticate)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            if (authenticate)
            {
                request.Credentials = new NetworkCredential(username, password);
            }
            request.UserAgent = "NTwitter/" + Twitter.Version;
            request.ReadWriteTimeout = c_DefaultTimeout;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ServicePoint.Expect100Continue = false;

            // Append params to post-request
            string paramlist = GetParamString(vars);
            request.ContentLength = paramlist.Length;

            using (Stream postStream = request.GetRequestStream())
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(paramlist);
                postStream.Write(bytes, 0, bytes.Length);
            }

            // Perform request
            string responseText = null;
            try
            {
                responseText = GetResponseText(request);
            }
            catch (WebException ex)
            {
                throw new TwitterException(
                    String.Format(CultureInfo.InvariantCulture, "An error occured accesing page {0}", uri)
                    ,ex);
            }
            return responseText;
        }

        #endregion
    }
}
