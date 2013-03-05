using System;
using System.Collections.Generic;

namespace NTwitter
{
    /// <summary>Interface for an http client - e.g. allows for OAuth to be used instead.
    /// The default version is {@link URLConnectionHttpClient}.
    /// <p>If creating your own version, please provide support for throwing the
    /// right subclass of TwitterException</p></summary>
    public interface IHttpClient
    {
        /// <summary>Whether this client can authenticate to the server</summary>
        /// <returns>true, if authentication is spported, otherwise false</returns>
        bool CanAuthenticate();

        /// <summary>Send an HTTP GET request and return the response body. Note that this
        /// will change all line breaks into system line breaks!
        /// </summary>
        /// <exception cref="TwitterException">for a variety of reasons</exception>
        /// <exception cref="TwitterException.E404">for resource-does-not-exist errors</exception>
        String GetPage(Uri uri, IDictionary<String, String> vars, bool authenticate);

        /// <summary>Send an HTTP POST request and return the response body.</summary>
        /// <param name="uri">The uri to post to.</param>
        /// <param name="vars">The form variables to send. These are URL encoded before sending.</param>
        /// <param name="authenticate">If true, send user authentication</param>
        /// <returns>The response from the server.</returns>
        /// <exception cref="TwitterException">for a variety of reasons</exception>
        /// <exception cref="TwitterException.E404">for resource-does-not-exist errors</exception>
        String Post(Uri uri, IDictionary<String, String> vars, bool authenticate);
    }
}