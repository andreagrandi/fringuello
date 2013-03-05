using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Fringuello
{
    public partial class TweetUC : UserControl
    {
        private String textPseudo;
        private String textTweet;
        private String textSignature;
        private Image imageAvatar;

        public String PseudoLabel
        {
            get
            {
                return textPseudo;
            }

            set
            {
                textPseudo = value;
                pseudo.Text = value;
            }

        }

        public String TweetLabel
        {
            get
            {
                return textTweet;
            }

            set
            {
                textTweet = value;
                tweet.Text = value;
            }

        }

        public String SignatureLabel
        {
            get
            {
                return textSignature;
            }

            set
            {
                textSignature = value;
                signature.Text = value;
            }

        }

        public Image AvatarImage
        {
            get
            {
                return imageAvatar;
            }

            set
            {
                imageAvatar = value;
                avatar.Image = value;
            }

        }

        public TweetUC()
        {
            InitializeComponent();
        }

        private void avatar_Click(object sender, EventArgs e)
        {

        }

        private void pseudo_ParentChanged(object sender, EventArgs e)
        {

        }

       
    }
}
