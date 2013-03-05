using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using CompactNTwitter;
using NTwitter;

namespace Fringuello
{
    public partial class MainFeed : Form
    {
        NTwitter.Twitter twitter;
        private string _username = ""; 
        private string _password = "";
        int numberTweets = 0;
        List<TweetUC> listTweets;

        public MainFeed(string username, string password)
        {
            InitializeComponent();
            _username = username;
            _password = password;
            twitter = new NTwitter.Twitter(_username, _password);
            getFeed();
        }

        private void getFeed()
        {
            numberTweets = 0;
            List<Status> feed;
            feed = twitter.GetFriendsTimeline().ToList();
            listTweets = new List<TweetUC>();

            foreach (Status aStatus in feed)
            {
                numberTweets++;
                TweetUC tweetUC = new TweetUC();

                tweetUC.Location = new Point(0, tweetUC.Size.Height * (numberTweets - 1));
                tweetUC.TweetLabel = aStatus.Text;
                tweetUC.PseudoLabel = aStatus.User.Name;
                tweetUC.SignatureLabel = "Posted at " + aStatus.CreatedAt;
                tweetUC.AvatarImage = new Bitmap(new MemoryStream(GetBytesFromUrl(aStatus.User.ProfileImageUrl.ToString())));
                this.Controls.Add(tweetUC);
                listTweets.Add(tweetUC);
            }
        }

        private byte[] GetBytesFromUrl(string url)
        {
            byte[] b;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResp = myReq.GetResponse();

            Stream stream = myResp.GetResponseStream();
            using (BinaryReader br = new BinaryReader(stream))
            {
                b = br.ReadBytes(500000);
                br.Close();
            }
            myResp.Close();
            return b;
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuNewStatus_Click(object sender, EventArgs e)
        {
            UpdateStatus updateFrm = new UpdateStatus(_username, _password);
            updateFrm.Show();
        }
    }
}