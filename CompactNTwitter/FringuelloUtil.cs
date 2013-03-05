using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace CompactNTwitter
{
    public class FringuelloUtil
    {
        public String UpdateStatus(String username, String password, String tweet)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            const int bufSizeMax = 65536; // max read buffer size conserves memory
            const int bufSizeMin = 8192;  // min size prevents numerous small reads
            StringBuilder sb = new StringBuilder();

            // Encode credentials
            string user = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
            string data = "status=" + tweet;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://twitter.com/statuses/update.xml");
            request.Credentials = new NetworkCredential(username, password);
            request.Method = "POST";
            //request.ServicePoint.Expect100Continue = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            request.AllowWriteStreamBuffering = true;

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();

                // Execute the request and obtain the response stream
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                // Content-Length header is not trustable, but makes a good hint.
                // Responses longer than int size will throw an exception here!
                int length = (int)response.ContentLength;

                // Use Content-Length if between bufSizeMax and bufSizeMin
                int bufSize = bufSizeMin;
                if (length > bufSize)
                    bufSize = length > bufSizeMax ? bufSizeMax : length;

                // Allocate buffer and StringBuilder for reading response
                byte[] buf = new byte[bufSize];
                sb = new StringBuilder(bufSize);

                // Read response stream until end
                while ((length = responseStream.Read(buf, 0, buf.Length)) != 0)
                    sb.Append(Encoding.UTF8.GetString(buf, 0, length));

                return sb.ToString();
            }
        }
    }
}
