using System;
using System.Collections.Generic;

namespace NTwitter
{
    /// <summary>
    /// A runtime exception for when Twitter requests don't work. All
    /// <see cref="Twitter"/> methods can throw this.
    /// <p>
    /// This contains several subclasses which should be thrown to mark
    /// different problems. Error handling is particulary important as
    /// Twitter tends to be a bit flaky.
    /// </p>
    /// </summary>
    [Serializable]
    public class TwitterException : ApplicationException
    {
        #region Inner Exceptions

        /// <summary>
        /// A code 50X error (e.g. 502) - indicating soemthing went wrong at 
        /// Twitter's end. Usually retrying in a minute will fix this.
        /// </summary>
        public sealed class E50X : TwitterException
        {
            /// <summary>Initializes a new instance of the E50X exception</summary>
            public E50X() : base("Server error: 50x") { }

            /// <summary>Initializes a new instance of the E50X exception</summary>
            /// <param name="message">Exception text</param>
            public E50X(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// A Forbidden exception
        /// </summary>
        public sealed class E403 : TwitterException
        {
            /// <summary>Initializes a new instance of the E50X exception</summary>
            public E403() : base ("Server error: 403") { }

            /// <summary>Initializes a new instance of the E403 exception</summary>
            /// <param name="message">Exception text</param>
            public E403(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Indicates a 404: resource does not exist error from Twitter.
        /// Note: Can be throw in relation to suspended users.
        /// </summary>
        public sealed class E404 : TwitterException
        {
            /// <summary>Initializes a new instance of the E404 exception</summary>
            public E404() : base ("Server error: 404") { }

            /// <summary>Initializes a new instance of the E404 exception</summary>
            /// <param name="message">Exception text</param>
            public E404(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Indicates a rate limit error (i.e. you've over-used Twitter)
        /// </summary>
        public sealed class RateLimit : TwitterException
        {
            /// <summary>Initializes a new instance of the RateLimit-exception</summary>
            public RateLimit() : base ("Maximum API-call limit reached.")
            {
            }

            /// <summary>Initializes a new instance of the RateLimit-exception</summary>
            /// <param name="message">Exception text</param>
            public RateLimit(string message)
                : base(message)
            {
            }
        }

        #endregion

        /// <summary>Initializes a new instance of the TwitterException</summary>
        public TwitterException()
        {
        }

        /// <summary>Initializes a new instance of the TwitterException</summary>
        /// <param name="message">Exception text</param>
        public TwitterException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the TwitterException</summary>
        /// <param name="innerException">The inner exception as reason of this message</param>
        public TwitterException(Exception innerException)
            : base(String.Empty, innerException)
        {
        }

        /// <summary>Initializes a new instance of the TwitterException</summary>
        /// <param name="message">Exception text</param>
        /// <param name="additionalInfo">Additional information related to the exception</param>
        public TwitterException(string message, string additionalInfo)
            : this(message)
        {
            this.AdditionalInfo = additionalInfo;
        }

        /// <summary>Initializes a new instance of the TwitterException</summary>
        /// <param name="message">Exception text</param>
        /// <param name="innerException">The inner exception as reason of this message</param>
        public TwitterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !WindowsCE
        /// <summary>Used for serialization of the exception</summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected TwitterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif

        /// <summary>Contains additional iformation regarding the exception</summary>
        public string AdditionalInfo { get; private set; }
    }
}
