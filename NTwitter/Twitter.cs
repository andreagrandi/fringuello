using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.ObjectModel;

namespace NTwitter
{
    /// <summary>.NET wrapper for the Twitter API v1.2</summary>
    /// <remarks>
    /// <h4>Copyright and License</h4>
    /// This code is is released asopen-source under the LGPL license. See
    /// <a href="http://www.gnu.org/licenses/lgpl.html">
    /// http://www.gnu.org/licenses/lgpl.html</a>
    /// for license details.
    /// This code comes with no warranty or support.
    /// <h4>Notes:</h4>
    /// <ul><li>This wrapper takes care of all url-encoding/decoding.</li>
    /// <li>This wrapper will throw a runtime exception (TwitterException) if a
    /// methods fails, e.g. it cannot connect to Twitter.com or you make a bad
    /// request. I would like to improve error-handling, and welcome suggestions on
    /// cases where more informative exceptions would be helpful.</li></ul>
    /// </remarks>
    /// <example><code><pre>
    /// //Make a Twitter object
    /// Twitter twitter = new Twitter("my-name","my-password");
    /// //Print Winterstein's status
    /// Console.WriteLine(twitter.getStatus("winterstein"));
    /// //Set my status
    /// twitter.updateStatus("Messing about in .NET");
    /// </pre></code></example>
    public class Twitter
    {
        /// <summary>Holds the version of the supported Twitter API</summary>
        public const string Version = "1.2";

        #region private fields

        private string sourceApp = "ntwitterlib";

        private int? pageNumber;
        private long? sinceId;

        private readonly IHttpClient http;

        /// <summary>
        /// Can be null even if we have authentication when using OAuth
        /// </summary>
        private readonly string name;

        #endregion

        #region constructor

        /// <summary>
        /// Create a Twitter client without specifying a user.
        /// </summary>
        public Twitter()
            : this(null, new URLConnectionHttpClient())
        {
        }

        /// <summary>.NET wrapper for the Twitter API.</summary>
        /// <param name="name">the authenticating user's name, if known. Can be null.</param>
        /// <param name="client">An HttpClient instance for accesing the Twitter API (based on REST)</param>
        public Twitter(string name, IHttpClient client)
        {
            this.name = name;
            this.http = client;
        }

        /// <summary>
        /// .NET wrapper for the Twitter API.
        /// </summary>
        /// <param name="screenName">The name of the Twitter user. Only used by some methods.
        /// Can be null if you avoid methods requiring authentication.</param>
        /// <param name="password">The password of the Twitter user.
        /// Can be null if you avoid methods requiring authentication.</param>
        public Twitter(string screenName, string password)
            : this(screenName, new URLConnectionHttpClient(screenName, password))
        {
        }

        #endregion

        #region private methods

        /// <summary>
        /// Add in since and page, if set. This is called by methods that return
        /// lists of statuses or messages.
        /// </summary>
        private IDictionary<String, String> AddStandardishParameters(IDictionary<String, String> vars)
        {
            if (sinceId.HasValue)
                vars.Add("since_id", sinceId.Value.ToString(CultureInfo.InvariantCulture));
            if (pageNumber.HasValue)
            {
                vars.Add("page", pageNumber.ToString());
                // this is used once only
                pageNumber = null;
            }
            return vars;
        }

        /// <summary>
        /// If sinceDate is set, filter keeping only those messages that came after since date
        /// </summary>
        private Collection<Message> DateFilter(Collection<Message> list)
        {
            if (SinceDate == null)
                return list;

            Collection<Message> filtered = new Collection<Message>();
            foreach (Message message in list)
            {
                if (message.CreatedAt == null)
                    filtered.Add(message);
                else if (SinceDate <= message.CreatedAt)
                    filtered.Add(message);
            }
            return filtered;
        }

        /// <summary>
        /// If sinceDate is set, filter keeping only those messages that came after since date
        /// </summary>
        private Collection<Status> DateFilter(Collection<Status> list)
        {
            if (SinceDate == null)
                return list;

            Collection<Status> filtered = new Collection<Status>();
            foreach (Status message in list)
            {
                if (message.CreatedAt == null)
                    filtered.Add(message);
                else if (SinceDate <= message.CreatedAt)
                    filtered.Add(message);
            }
            return filtered;
        }

        private Collection<Message> GetMessages(Uri uri, IDictionary<String, String> var)
        {
            // Default: 1 page
            Collection<Message> msgs = null;
            if (MaxResults < 1)
            {
                msgs = Message.GetMessages(http.GetPage(uri, var, true));
                msgs = DateFilter(msgs);
            }
            else
            {
                // Fetch all pages until we run out or Twitter
                // complains in which case you'll get an exception
                pageNumber = 1;
                msgs = new Collection<Message>();
                while (msgs.Count <= MaxResults)
                {
                    string p = http.GetPage(uri, var, true);
                    Collection<Message> nextpage = Message.GetMessages(p);
                    nextpage = DateFilter(nextpage);
                    foreach (Message item in nextpage)
                    {
                        msgs.Add(item);
                    }
                    if (nextpage.Count < 20)
                        break;
                    pageNumber++;
                    var.Add("page", pageNumber.ToString());
                }
            }
            return msgs;
        }

        private Collection<Status> GetStatuses(Uri uri, IDictionary<String, String> var)
        {
            // Default: 1 page
            Collection<Status> msgs = null;
            if (MaxResults < 1)
            {
                msgs = Status.getStatuses(http.GetPage(uri, var, true));
                msgs = DateFilter(msgs);
            }
            else
            {
                // Fetch all pages until we run out
                // or Twitter complains in which case you'll get an exception
                pageNumber = 1;
                msgs = new Collection<Status>();
                while (msgs.Count <= MaxResults)
                {
                    Collection<Status> nextpage = Status.getStatuses(http.GetPage(uri, var, true));
                    nextpage = DateFilter(nextpage);
                    foreach (Status item in nextpage)
                    {
                        msgs.Add(item);
                    }
                    if (nextpage.Count < 20)
                        break;
                    pageNumber++;
                    var.Add("page", pageNumber.ToString());
                }
            }
            return msgs;
        }

        private Collection<long> GetUserIds(Uri uri)
        {
            string json = http.GetPage(uri, null, true);
            Collection<long> ids = new Collection<long>();
            try
            {
                JsonArray jarr = new JsonArray(json);
                for (int i = 0; i < jarr.Count; i++)
                {
                    ids.Add(jarr.getLong(i));
                }
            }
            catch (JsonException e)
            {
                throw new TwitterException("Could not parse id list" + e);
            }
            return ids;
        }

        private Collection<User> GetUsers(Uri uri)
        {
            return User.getUsers(http.GetPage(uri, null, true));
        }

        /// <summary>
        /// Wrapper for <see cref="IHttpClient.Post"/>.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        private string Post(Uri uri, IDictionary<String, String> vars, bool authenticate)
        {
            string page = http.Post(uri, vars, authenticate);
            return page;
        }

        private IDictionary<String, String> StandardishParameters()
        {
            return AddStandardishParameters(new Dictionary<string, string>());
        }

        #endregion

        #region public properties

        /// <summary>if greater than zero, requests will attempt to fetch as many
        /// pages as are needed! -1 by default, in which case most methods return the first 20
        /// statuses/messages.
        /// <p>If setting a high figure, you should usually also set a 
        /// sinceId or sinceDate to limit your Twitter usage. Otherwise
        /// you can easily exceed your rate limit.</p></summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Date based filter on statuses and messages. This is done client-side as
        /// Twitter have - for their own inscrutable reasons - pulled support for
        /// this feature.
        /// <p>If using this, you probably also want to increase <see cref="MaxResults"/>.
        /// Otherwise you get at most 20, and possibly less (since the filtering is done client side).</p>
        /// </summary>
        public DateTime SinceDate { get; set; }

        #endregion

        #region public methods

        /// <summary>
        /// Equivalent to <see cref="Follow"/>.
        /// <a href="http://apiwiki.twitter.com/Migrating-to-followers-terminology">Twitter API</a>
        /// <param name="userName">Required. The ID or screen name of the user to befriend.</param>
        /// </summary>
        /// <returns>The befriended user.</returns>
        /// <exception cref="TwitterException"></exception>
        [Obsolete("Please use \"follow\" instead, which is equivalent.", true)]
        public User Befriend(string userName)
        {
            return this.Follow(userName);
        }

        /// <summary>Equivalent to <see cref="StopFollowing"/></summary>
        /// <param name="userName">Required. The ID or screen name of the user to befriend.</param>
        [Obsolete("Please use \"stopFollowing\" instead, which is equivalent.", true)]
        public User BreakFriendship(string userName)
        {
            return StopFollowing(userName);
        }

        /// <summary>
        /// Destroys the status specified by the required ID parameter. The
        /// authenticating user must be the author of the specified status.
        /// Sends two HTTP requests to Twitter rather than one: Twitter appears
        /// not to make deletions visible until the user's status page is requested.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public void DestroyStatus(long id)
        {
            string page = Post(new Uri("http://twitter.com/statuses/destroy/" + id + ".json"), null, true);
            http.GetPage(new Uri("http://twitter.com/" + name), null, true);
            Debug.Assert(page != null);
        }

        /// <summary>
        /// Destroys the given status. Equivalent to {@link #destroyStatus(int)}. The
        /// authenticating user must be the author of the status post.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public void DestroyStatus(Status status)
        {
            DestroyStatus(status.Id);
        }

        /// <summary>
        /// Start following a user.
        /// <param name="userName">Required. The ID or screen name of the user to befriend.</param>
        /// </summary>
        /// <returns>The befriended user, or null if they were already being followed.</returns>
        /// <exception cref="TwitterException">If the user does not exist or has been suspended.</exception>
        public User Follow(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            string page;
            try
            {
                page = Post(new Uri("http://twitter.com/friendships/create/" + userName + ".json"),
                    null, true);
            }
            catch (TwitterException e)
            {
                if (e.Message.Contains("HTTP response code: 403"))
                {
                    // check if we've tried to follow someone we're already
                    // following
                    if (IsFollowing(userName))
                    {
                        return null;
                    }
                }
                throw;
            }
            try
            {
                return new User(new JsonObject(page), null);
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Returns a list of the direct messages sent to the authenticating user.
        /// <p>Note: the Twitter API makes this available in rss if that's of interest.</p>
        /// </summary>
        public Collection<Message> GetDirectMessages()
        {
            return GetMessages(new Uri("http://twitter.com/direct_messages.json"),
                    StandardishParameters());
        }

        /// <summary>
        /// Returns a list of the direct messages sent *by* the authenticating user.
        /// </summary>
        public Collection<Message> GetDirectMessagesSent()
        {
            return GetMessages(new Uri("http://twitter.com/direct_messages/sent.json"),
                    StandardishParameters());
        }

        /// <summary>
        /// Returns a list of the users currently featured on the site with their
        /// current statuses inline.
        /// <p>
        /// Note: This is no longer part of the Twitter API. Support is provided via
        /// other methods.</p>
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<User> GetFeatured()
        {
            Collection<User> users = new Collection<User>();
            Collection<Status> featured = GetPublicTimeline();
            foreach (Status status in featured)
            {
                users.Add(status.User);
            }
            return users;
        }

        /// <summary>
        /// Returns the Ids of the authenticating user's followers.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<long> GetFollowerIds()
        {
            return GetUserIds(new Uri("http://twitter.com/followers/ids.json"));
        }

        /// <summary>
        /// Returns the Ids of the specified user's followers.
        /// </summary>
        /// <param name="screenName">screen name of the user whose followers are to be fetched.</param>
        /// <exception cref="TwitterException"></exception>
        public Collection<long> GetFollowerIds(string screenName)
        {
            return GetUserIds(new Uri("http://twitter.com/followers/ids/" + screenName + ".json"));
        }

        /// <summary>
        /// Returns the authenticating user's (latest) followers, each with current
        /// status inline.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<User> GetFollowers()
        {
            return GetUsers(new Uri("http://twitter.com/statuses/followers.json"));
        }

        /// <summary>
        /// Returns the Ids of the authenticating user's friends.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<long> GetFriendIds()
        {
            return GetUserIds(new Uri("http://twitter.com/friends/ids.json"));
        }

        /// <summary>
        /// Returns the Ids of the specified user's friends.
        /// </summary>
        /// <param name="screenName">screen name of the user whose friends are to be fetched.</param>
        /// <exception cref="TwitterException"></exception>
        public Collection<long> GetFriendIds(string screenName)
        {
            return GetUserIds(new Uri("http://twitter.com/friends/ids/" + screenName + ".json"));
        }

        /// <summary>
        /// Returns the authenticating user's (latest 100) friends, each with current
        /// status inline. NB - friends are people who *you* follow.
        /// <p>
        /// Note that there seems to be a small delay from Twitter in updates to this list.</p>
        /// <seealso cref="GetFriendIds()"/>
        /// <seealso cref="IsFollowing"/>
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<User> GetFriends()
        {
            return GetUsers(new Uri("http://twitter.com/statuses/friends.json"));
        }

        /// <summary>
        /// Returns the (latest 100) given user's friends, each with current status inline.
        /// </summary>
        /// <param name="userName">The ID or screen name of the user for whom to request a list of friends.</param>
        /// <exception cref="TwitterException"></exception>
        public Collection<User> GetFriends(string userName)
        {
            return GetUsers(new Uri("http://twitter.com/statuses/friends/" + userName + ".json"));
        }

        /// <summary>
        /// Returns the 20 most recent statuses posted in the last 24 hours from the
        /// authenticating user and that user's friends.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<Status> GetFriendsTimeline()
        {
            return GetStatuses(new Uri("http://twitter.com/statuses/friends_timeline.json"), StandardishParameters());
        }

        /// <summary>
        /// Returns the 20 most recent statuses posted in the last 24 hours from the
        /// user (given by id) and that user's friends.
        /// </summary>
        /// <param name="id">Specifies the ID or screen name of the user for whom to return the friends_timeline.</param>
        public Collection<Status> GetFriendsTimeline(string id)
        {
            IDictionary<String, String> map = new Dictionary<string, string>();
            map.Add("id", id);
            AddStandardishParameters(map);
            return GetStatuses(new Uri("http://twitter.com/statuses/friends_timeline.json"), map);
        }

        /// <returns>Name of the authenticating user, or null if not set.</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Returns the 20 most recent statuses from non-protected users who have set
        /// a custom user icon. Does not require authentication.
        /// <p>
        /// Note: Twitter cache-and-refresh this every 60 seconds, so there is little
        /// point calling it more frequently than that.</p>
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<Status> GetPublicTimeline()
        {
            return GetStatuses(new Uri("http://twitter.com/statuses/public_timeline.json"), StandardishParameters());
        }

        /// <returns>
        /// The remaining number of API requests available to the authenticating user
        /// before the API limit is reached for the current hour.
        /// <i>If this is negative you should stop using Twitter with this login for a bit.</i>
        /// Note: Calls to rate_limit_status do not count against the rate limit.
        /// </returns>
        public int GetRateLimitStatus()
        {
            string json = http.GetPage(new Uri("http://twitter.com/account/rate_limit_status.json"), null, true);
            try
            {
                JsonObject obj = new JsonObject(json);
                int hits = obj.getInt("remaining_hits");
                return hits;
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Returns the 20 most recent replies/mentions (status updates with the userName)
        /// to the authenticating user. Replies are only available to the authenticating user;
        /// You can not request a list of replies to another user whether public or protected.
        /// <p>The Twitter API now refers to replies as <i>mentions</i>. We have kept the
        /// old terminology here.</p>
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<Status> GetReplies()
        {
            return GetStatuses(new Uri("http://twitter.com/statuses/replies.json"), StandardishParameters());
        }

        /// <returns>The current status of the user. null if unset (ie if they have never tweeted)</returns>
        /// <exception cref="TwitterException"></exception>
        public Status GetStatus()
        {
            IDictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("count", "1");
            string json = http.GetPage(new Uri("http://twitter.com/statuses/user_timeline.json"), vars, true);
            Collection<Status> statuses = Status.getStatuses(json);
            if (statuses.Count == 0)
                return null;
            return statuses[0];
        }

        /// <summary>
        /// Returns a single status, specified by the id parameter below. The
        /// status's author will be returned inline.
        /// </summary>
        /// <param name="id">The numerical ID of the status you're trying to retrieve.</param>
        /// <exception cref="TwitterException"></exception>
        public Status GetStatus(long id)
        {
            string json = http.GetPage(new Uri("http://twitter.com/statuses/show/" + id + ".json"), null, true);
            try
            {
                return new Status(new JsonObject(json), null);
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <returns>The current status of the given user, as a normal String.</returns>
        /// <exception cref="TwitterException"></exception>
        public Status GetStatus(string userName)
        {
            Debug.Assert(userName != null);
            IDictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("id", userName);
            vars.Add("count", "1");

            string json = http.GetPage(new Uri("http://twitter.com/statuses/user_timeline.json"), vars, false);
            return Status.getStatuses(json)[0];
        }

        /// <summary>
        /// Returns the 20 most recent statuses posted in the last 24 hours from the
        /// authenticating user.
        /// </summary>
        /// <exception cref="TwitterException"></exception>
        public Collection<Status> GetUserTimeline()
        {
            return GetStatuses(new Uri("http://twitter.com/statuses/user_timeline.json"), StandardishParameters());
        }

        /// <summary>
        /// Returns the most recent statuses posted in the last 24 hours from the
        /// given user.
        /// <p>This method will authenticate if it can (i.e. if the Twitter object has a
        /// userName and password). Authentication is needed to see the posts of a
        /// private user.</p>
        /// </summary>
        /// <param name="id">Can be null. Specifies the ID or screen name of the user for whom to return the user_timeline.</param>
        /// <exception cref="TwitterException"></exception>
        public Collection<Status> GetUserTimeline(string id)
        {
            IDictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("id", id);
            AddStandardishParameters(vars);
            // Should we authenticate?
            bool authenticate = http.CanAuthenticate();
            string json = http.GetPage(new Uri("http://twitter.com/statuses/user_timeline.json"), vars, authenticate);
            return Status.getStatuses(json);
        }

        /// <summary>
        /// Is the authenticating user <i>followed by</i> userB?
        /// </summary>
        /// <param name="userB">The screen name of a Twitter user.</param>
        /// <returns>Whether or not the user is followed by userB.</returns>
        public bool IsFollower(string userB)
        {
            return IsFollower(userB, name);
        }

        /// <returns>true if followerScreenName <i>is</i> following followedScreenName</returns>
        /// <exception cref="TwitterException.E403">if one of the users has protected their updates
        /// and you don't have access. This can be counter-intuitive (and annoying) at times.</exception>
        public bool IsFollower(string followerScreenName, string followedScreenName)
        {
            Debug.Assert(followerScreenName != null && followedScreenName != null);
            IDictionary<string, string> args = new Dictionary<string, string>();
            args.Add("user_a", followerScreenName);
            args.Add("user_b", followedScreenName);
            string page = http.GetPage(new Uri("http://twitter.com/friendships/exists.json"), args, true);
            return bool.Parse(page);
        }

        /// <summary>
        /// Does the authenticating user <i>follow</i> userB?
        /// </summary>
        /// <param name="userB">The screen name of a Twitter user.</param>
        /// <returns>Whether or not the user follows userB.</returns>
        public bool IsFollowing(string userB)
        {
            return IsFollower(name, userB);
        }

        /// <summary>
        /// Switches off notifications for updates from the specified user <i>who
        /// must already be a friend</i>.
        /// </summary>
        /// <param name="userName">Stop getting notifications from this user, who must already be one of your friends.</param>
        /// <returns>the specified user</returns>
        public User LeaveNotifications(string userName)
        {
            string page = http.GetPage(new Uri("http://twitter.com/notifications/leave/" + userName + ".json"), null, true);
            try
            {
                return new User(new JsonObject(page), null);
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Enables notifications for updates from the specified user <i>who must already be a friend</i>.
        /// </summary>
        /// <param name="userName">Get notifications from this user, who must already be one of your friends.</param>
        /// <returns>the specified user</returns>
        public User Notify(string userName)
        {
            string page = http.GetPage(new Uri("http://twitter.com/notifications/follow/" + userName + ".json"), null, true);
            try
            {
                return new User(new JsonObject(page), null);
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Perform a search of Twitter.
        /// <p>Warning: the User objects returned by a search (as part of the Status objects)
        /// are dummy-users. The only information that is set is the user's screen-name. If you need more info,
        /// call <see cref="Show"/> with the screen name.</p>
        /// </summary>
        /// <returns>search results (upto 100 per page)</returns>
        public Collection<Status> Search(string searchTerm)
        {
            // number of tweets per page, max 100
            int rpp = 100;
            IDictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("rpp", "" + rpp);
            vars.Add("q", searchTerm);
            AddStandardishParameters(vars);
            string json = http.GetPage(new Uri("http://search.twitter.com/search.json"), vars, true);
            try
            {
                JsonObject jo = new JsonObject(json);
                Collection<Status> stati = Status.GetStatusesFromSearch(jo);
                return DateFilter(stati);
            }
            catch (Exception e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Sends a new direct message to the specified user from the authenticating user.
        /// </summary>
        /// <param name="recipient">Required. The ID or screen name of the recipient user.</param>
        /// <param name="text">Required. The text of your direct message. Keep it under 140 characters!</param>
        /// <returns>the sent message</returns>
        /// <exception cref="TwitterException"></exception>
        public Message SendMessage(string recipient, string text)
        {
            Debug.Assert(recipient != null);
            if (text.Length > 140)
                throw new ArgumentException("Message is too long.");
            IDictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("user", recipient);
            vars.Add("text", text);
            string result = Post(new Uri("http://twitter.com/direct_messages/new.json"), vars, true);
            try
            {
                return new Message(new JsonObject(result));
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <param name="pageNumber">null (the default) returns the first page.
        /// Pages are indexed from 1. This is used once only</param>
        public void SetPageNumber(int pageNumber)
        {
            this.pageNumber = pageNumber;
        }

        /// <summary>
        /// Narrows the returned results to just those statuses created after the
        /// specified status id. This will be used until it is set to null. Default
        /// is null.
        /// <p>If using this, you probably also want to increase <see cref="MaxResults"/>.
        /// Otherwise you get at most 20, and possibly less (since the filtering is done client side).</p>
        /// </summary>
        public void SetSinceId(long statusId)
        {
            sinceId = statusId;
        }

        /// <summary>
        /// Set the source application. This will be mentioned on Twitter alongside
        /// status updates (with a small label saying source: myapp).
        /// 
        /// <i>In order for this to work, you must first register your app with
        /// Twitter and get a source name from them! Otherwise the source will appear
        /// as "web".</i>
        /// </summary>
        /// <param name="sourceApp">ntwitterlib by default. Set to null for no source.</param>
        public void SetSource(string sourceApp)
        {
            this.sourceApp = sourceApp;
        }

        /// <summary>
        /// Sets the authenticating user's status.
        /// <p>Identical to <see cref="UpdateStatus"/>, inherited from the Java-library</p>
        /// </summary>
        /// <param name="statusText">The text of your status update. Must not be more
        /// than 160 characters and should not be more than 140 characters to ensure optimal display.</param>
        /// <returns>The posted status when successful.</returns>
        /// <exception cref="TwitterException"></exception>
        public Status SetStatus(string statusText)
        {
            return UpdateStatus(statusText);
        }

        /// <summary>
        /// Returns information of a given user, specified by ID or screen name.
        /// </summary>
        /// <param name="id">The ID or screen name of a user.</param>
        /// <exception cref="TwitterException">if the user does not exist - or
        /// has been terminated (as happens to spam bots).</exception>
        public User Show(string id)
        {
            string json = http.GetPage(new Uri("http://twitter.com/users/show/" + id + ".json"), null, http.CanAuthenticate());
            User user;
            try
            {
                user = new User(new JsonObject(json), null);
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
            return user;
        }

        /// <summary>
        /// Split a long message up into shorter chunks suitable for use with
        /// <see cref="SetStatus"/> or <see cref="SendMessage"/>.
        /// </summary>
        /// <returns>longStatus broken into a list of max 140 char strings</returns>
        public static Collection<string> SplitMessage(string longStatus)
        {
            // Is it really long?
            if (longStatus.Length <= 140)
                return new Collection<string>() { longStatus };

            // Multiple tweets for a longer post
            Collection<string> sections = new Collection<String>();
            StringBuilder tweet = new StringBuilder(140);
            string[] words = Regex.Split(longStatus, "\\s+");
            foreach (string w in words)
            {
                // messages have a max length of 140
                // plus the last bit of a long tweet tends to be hidden on
                // twitter.com, so best to chop 'em short too
                if (tweet.Length + w.Length + 1 > 140)
                {
                    // Emit
                    tweet.Append("...");
                    sections.Add(tweet.ToString());
                    tweet = new StringBuilder(140);
                    tweet.Append(w);
                }
                else
                {
                    if (tweet.Length != 0)
                        tweet.Append(" ");
                    tweet.Append(w);
                }
            }
            // Final bit
            if (tweet.Length != 0)
                sections.Add(tweet.ToString());
            return sections;
        }

        /// <summary>
        /// Destroy: Discontinues friendship with the user specified in the ID parameter as the authenticating user.
        /// </summary>
        /// <param name="userName">The ID or screen name of the user with whom to discontinue friendship.</param>
        /// <returns>the un-friended user (if they were a friend), or null if the method fails because the specified user was not a friend.</returns>
        public User StopFollowing(string userName)
        {
            Debug.Assert(GetName() != null);
            try
            {
                string page = Post(new Uri("http://twitter.com/friendships/destroy/" + userName + ".json"), null, true);
                User user;
                try
                {
                    user = new User(new JsonObject(page), null);
                }
                catch (JsonException e)
                {
                    throw new TwitterException(e);
                }
                return user;
            }
            catch (TwitterException)
            {
                // were they a friend anyway?
                if (!IsFollower(GetName(), userName))
                {
                    return null;
                }
                // Something else went wrong
                throw;
            }
        }

        /// <summary>
        /// Updates the authenticating user's status.
        /// @return The posted status when successful.
        /// </summary>
        /// <param name="statusText">The text of your status update.
        /// Must not be more than 160 characters and should not be more
        /// than 140 characters to ensure optimal display.</param>
        /// <exception cref="TwitterException"></exception>
        public Status UpdateStatus(string statusText)
        {
            if (statusText.Length > 160)
                throw new ArgumentException("Status text must be 160 characters or less: " + statusText.Length);
            IDictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("status", statusText);
            if (sourceApp != null)
                vars.Add("source", sourceApp);
            string result = Post(new Uri("http://twitter.com/statuses/update.json"), vars, true);

            try
            {
                return new Status(new JsonObject(result), null);
            }
            catch (JsonException e)
            {
                throw new TwitterException(e);
            }
        }

        /// <summary>
        /// Does a user with the specified name or id exist?
        /// </summary>
        /// <param name="id">The screen name or user id of the suspected user.</param>
        /// <returns>False if the user doesn't exist or has been suspended, true otherwise.</returns>
        public bool UserExists(string id)
        {
            try
            {
                string json = http.GetPage(new Uri("http://twitter.com/users/show/" + id + ".json"), null, true);
            }
            catch (TwitterException.E404)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}