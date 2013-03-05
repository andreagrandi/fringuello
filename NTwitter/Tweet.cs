using System;
using System.Globalization;

namespace NTwitter
{
    /// <summary>This gives common access to features that are common
    /// to both <see cref="Message"/>s and <see cref="Status"/>es.</summary>
    public abstract class Tweet
    {
        #region protected methods

        /// <summary>Parses a string containing date- and
        /// time-information from the Twitter platform</summary>
        /// <param name="dt">The string containg the DateTime-information</param>
        /// <returns>An <see cref="DateTime"/>-instance containg corresponding information</returns>
        /// <example>The input string has to be formatted as follows: ddd MMM dd HH:mm:ss zz00 yyyy</example>
        protected static DateTime ParseTweetDateTime(string dt)
        {
            CultureInfo ci = new CultureInfo("en-US");
            DateTime pdt = DateTime.ParseExact(dt, "ddd MMM dd HH:mm:ss zz00 yyyy", ci.DateTimeFormat);
            return pdt;
        }

        #endregion

        #region constructor

        /// <summary>Initialises a new instance of a tweet</summary>
        protected Tweet()
        {
        }

        /// <summary>Initialises a new instance of a tweet</summary>
        /// <param name="createdAt">Creation timestamp of the tweet</param>
        /// <param name="id">Unique id of th tweet</param>
        /// <param name="text">Text of the tweet</param>
        /// <param name="user">The user created the tweet</param>
        protected Tweet(DateTime createdAt, long id, string text, User user)
        {
            this.CreatedAt = createdAt;
            this.Id = id;
            this.Text = text;
            this.User = user;
        }

        #endregion

        #region public properties

        /// <summary>Creation timestamp for the tweet</summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>The Twitter id for this post. This is used by some API methods.</summary>
        public long Id { get; protected set; }

        /// <summary>The actual status text. This is also returned by <see cref="ToString"/></summary>
        public string Text { get; protected set; }

        /// <summary>The User who made the tweet</summary>
        public User User { get; protected set; }

        #endregion

        #region public methods

        /// <returns>The text of this tweet</returns>
        public override string ToString()
        {
            return this.Text;
        }

        #endregion
    }
}