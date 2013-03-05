using System;

namespace NTwitter
{
    /// <summary>Occurs if an error occurs while deserializing
    /// an object from a JSON document</summary>
    [Serializable]
    public class JsonException : ApplicationException
    {
        /// <summary>Initializes a new instance of a JSONException</summary>
        public JsonException() { }

        /// <summary>Initializes a new instance of a JSONException</summary>
        /// <param name="message">Exception message</param>
        public JsonException(string message) : base(message) { }

        /// <summary>Initializes a new instance of a JSONException</summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception as reason of the current exception</param>
        public JsonException(string message, Exception inner) : base(message, inner) { }
#if !WindowsCE
        /// <summary>Used for serialization of the exception</summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected JsonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
