using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Globalization;

namespace NTwitter
{
    /// <summary>A Twitter status post. <see cref="Tweet.ToString"/>returns the status text.
    /// <p>Notes: This is a finalised data object. It has no methods but exposes its
    /// fields. If you want to change your status, use <see cref="Twitter.UpdateStatus"/>
    /// and <see cref="Twitter.DestroyStatus(long)"/></p></summary>
    public sealed class Status : Tweet
    {
        /// <summary>
        /// regex for @you mentions
        /// </summary>
        static readonly Regex AT_YOU_SIR = new Regex("@(\\w+)", RegexOptions.Compiled);

        /// <exception cref="TwitterException"></exception>
        internal static Collection<Status> getStatuses(String json)
        {
            if (String.IsNullOrEmpty(json.Trim()))
                return new Collection<Status>();
            try
            {
                Collection<Status> users = new Collection<Status>();
                JsonArray arr = new JsonArray(json);
                for (int i = 0; i < arr.Count; i++)
                {
                    JsonObject obj = arr[i];
                    Status u = new Status(obj, null);
                    users.Add(u);
                }
                return users;
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Search results use a slightly different protocol! In particular w.r.t. user ids and info.
        /// </summary>
        /// <param name="searchResults"></param>
        /// <returns>Search results as Status objects - but with dummy users!</returns>
        internal static Collection<Status> GetStatusesFromSearch(JsonObject searchResults)
        {
            try
            {
                Collection<Status> users = new Collection<Status>();
                JsonArray arr = searchResults.getJSONArray("results");
                for (int i = 0; i < arr.Count; i++)
                {
                    JsonObject obj = arr[i];
                    String userScreenName = obj.getString("from_user");
                    User user = new User(userScreenName);
                    Status s = new Status(obj, user);
                    users.Add(s);
                }
                return users;
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <param name="user">Set when parsing the json returned for a User</param>
        /// <param name="jsonobj">The JSON object the Status is deserialized from</param>
        /// <exception cref="TwitterException"></exception>
        internal Status(JsonObject jsonobj, User user)
        {
            try
            {
                this.Id = jsonobj.getLong("id");
                this.Text = jsonobj.getString("text");
                String c = jsonobj.getString("created_at");
                this.CreatedAt = ParseTweetDateTime(c);
                Source = jsonobj.getString("source");
                if (user != null)
                {
                    this.User = user;
                }
                else
                {
                    JsonObject jsonUser = jsonobj.optJSONObject("user");
                    this.User = new User(jsonUser, this);
                }
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <returns>list of \@mentioned people  (there is no guarantee that 
        /// these mentions are for correct Twitter screen-names). May be empty,
        /// never null. Screen-names are always lowercased.</returns>
        public Collection<String> GetMentions()
        {
            MatchCollection matches = AT_YOU_SIR.Matches(Text);
            Collection<String> list = new Collection<String>();
            foreach (Match m in matches)
            {
                // skip email addresses (and other poorly formatted things)
                if (m.Index != 0 && Text[m.Index - 1] != ' ')
                    continue;
                string mention = m.Groups[1].Value;
                // enforce lower case
                list.Add(mention.ToLower(CultureInfo.InvariantCulture));
            }
            return list;
        }

        /// <summary>e.g. "web" vs. "im"</summary>
        public string Source { get; private set; }
    }
}
