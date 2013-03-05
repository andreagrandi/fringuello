using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;

namespace NTwitter
{
    /// <summary>A Twitter user. Fields are null if unset.</summary>
    public sealed class User : IEquatable<User>
    {
        /// <exception cref="TwitterException"></exception>
        internal static Collection<User> getUsers(string json)
        {
            if (String.IsNullOrEmpty(json.Trim()))
                return new Collection<User>();
            try
            {
                Collection<User> users = new Collection<User>();
                JsonArray arr = new JsonArray(json);
                for (int i = 0; i < arr.Count; i++)
                {
                    JsonObject obj = arr[i];
                    User u = new User(obj, null);
                    users.Add(u);
                }
                return users;
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        internal User(JsonObject obj, Status status)
        {
            try
            {
                this.Id = obj.getLong("id");
                this.Name = obj.getString("name");
                this.ScreenName = obj.getString("screen_name");
                this.Location = obj.getString("location");
                this.Description = obj.getString("description");
                String img = obj.getString("profile_image_url");
                this.ProfileImageUrl = img == null ? null : new Uri(img);
                String url = obj.getString("url");
                this.Website = url == null ? null : new Uri(url);
                this.ProtectedUser = obj.getbool("protected");
                if (status == null)
                {
                    JsonObject s = obj.optJSONObject("status");
                    this.Status = (s == null) ? null : new Status(s, this);
                }
                else
                {
                    this.Status = status;
                }
                String utcOffSet = obj.getString("utc_offset");
                this.TimezoneOffset = utcOffSet == null ? 0 : int.Parse(utcOffSet, CultureInfo.InvariantCulture);
                this.Timezone = obj.getString("time_zone");
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Create a dummy User object. All fields are set to null. This will be equals()
        /// to an actual User object, so it can be used to query collections.
        /// </summary>
        /// <param name="screenName"></param>
        /// <example>
        /// <code><pre>
        /// // Test whether jtwit is a friend
        /// twitter.getFriends().contains(new User("jtwit"));
        /// </pre></code>
        /// </example>
        public User(String screenName)
        {
            this.Id = -1;
            this.Name = null;
            this.ScreenName = screenName;
            this.Status = null;
            this.Location = null;
            this.Description = null;
            this.ProfileImageUrl = null;
            this.Website = null;
            this.ProtectedUser = false;
            this.TimezoneOffset = -1;
            this.Timezone = null;
        }

        /// <summary>Profile description of the user</summary>
        public string Description { get; private set; }

        /// <summary>User Id</summary>
        public long Id { get; private set; }

        /// <summary>Location of the user</summary>
        public string Location { get; private set; }

        /// <summary>The display name, e.g. "Daniel Winterstein"</summary>
        public string Name { get; private set; }

        /// <summary>The URL to the user's photo</summary>
        public Uri ProfileImageUrl { get; private set; }

        /// <summary>Signals, if the current user's profile is a protected one</summary>
        public bool ProtectedUser { get; private set; }

        /// <summary>The login name, e.g. "winterstein" This is
        /// the only thing used by equals() and hashcode()</summary>
        public string ScreenName { get; private set; }

        /// <summary>The current status of the user</summary>
        public Status Status { get; private set; }

        /// <summary>A link to the user's website</summary>
        public Uri Website { get; private set; }

        /// <summary>Number of seconds between a user's registered time zone and Greenwich
        /// Mean Time (GMT) - aka Coordinated Universal Time or UTC. Can be positive or negative.</summary>
        public int TimezoneOffset { get; private set; }

        /// <summary>The timezone the user lives in</summary>
        public string Timezone { get; private set; }

        /// <summary>Determines, if two instance of <see cref="User"/> are identical</summary>
        /// <param name="obj">The instance to compare with</param>
        /// <returns>true, if equal, otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            User ou = obj as User;
            if (ou == null)
                return false;
            return this.Equals(ou);
        }

        /// <summary>Determines, if two instance of <see cref="User"/> are identical</summary>
        /// <param name="other">The instance to compare with</param>
        /// <returns>true, if equal, otherwise false</returns>
        public bool Equals(User other)
        {
            return (this.ScreenName.Equals(other.ScreenName)) ? true : false;
        }

        /// <summary>Return the hash code for the current instance</summary>
        /// <returns>Hash Code</returns>
        /// <remarks>Equals the hash code of the <see cref="ScreenName"/></remarks>
        public override int GetHashCode()
        {
            return this.ScreenName.GetHashCode();
        }

        /// <summary>Returns the User's screenName (i.e. their Twitter login)</summary>
        public override string ToString()
        {
            return this.ScreenName;
        }
    }
}
