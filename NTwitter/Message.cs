using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTwitter
{
    ///<summary>A Twitter direct message. Fields are null if unset.
    ///<p>Historical note: this class used to cover \@you mentions
    ///as well, but these are better described by Statuses.</p></summary>	     
    public sealed class Message : Tweet
    {
        internal Message(JsonObject obj)
        {
            this.Id = obj.getLong("id");
            this.Text = obj.getString("text");
            String c = obj.getString("created_at");
            this.CreatedAt = ParseTweetDateTime(c);
            this.Sender = new User(obj.getJSONObject("sender"), null);
            // recipient - for messages you sent
            if (obj.has("recipient"))
            {
                this.Recipient = new User(obj.getJSONObject("recipient"), null);
            }
            else
            {
                this.Recipient = null;
            }
        }

        /// <returns>the recipient (for messages sent by the authenticating user)</returns>
        public User Recipient { get; private set; }

        /// <summary>This is equivalent to <see cref="User"/></summary>
        public User Sender { get; private set; }

        /// <exception cref="TwitterException"></exception>
        public static Collection<Message> GetMessages(String json)
        {
            if (String.IsNullOrEmpty(json.Trim()))
                return new Collection<Message>();
            try
            {
                Collection<Message> msgs = new Collection<Message>();
                JsonArray arr = new JsonArray(json);
                for (int i = 0; i < arr.Count; i++)
                {
                    JsonObject obj = arr[i];
                    Message u = new Message(obj);
                    msgs.Add(u);
                }
                return msgs;
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }
    }
}
