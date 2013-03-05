namespace Fringuello
{
    partial class TweetUC
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.avatar = new System.Windows.Forms.PictureBox();
            this.pseudo = new System.Windows.Forms.Label();
            this.tweet = new System.Windows.Forms.Label();
            this.signature = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // avatar
            // 
            this.avatar.Location = new System.Drawing.Point(4, 3);
            this.avatar.Name = "avatar";
            this.avatar.Size = new System.Drawing.Size(52, 49);
            this.avatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.avatar.Click += new System.EventHandler(this.avatar_Click);
            // 
            // pseudo
            // 
            this.pseudo.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
            this.pseudo.Location = new System.Drawing.Point(4, 58);
            this.pseudo.Name = "pseudo";
            this.pseudo.Size = new System.Drawing.Size(52, 27);
            this.pseudo.ParentChanged += new System.EventHandler(this.pseudo_ParentChanged);
            // 
            // tweet
            // 
            this.tweet.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.tweet.Location = new System.Drawing.Point(61, 3);
            this.tweet.Name = "tweet";
            this.tweet.Size = new System.Drawing.Size(160, 65);
            // 
            // signature
            // 
            this.signature.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
            this.signature.Location = new System.Drawing.Point(61, 71);
            this.signature.Name = "signature";
            this.signature.Size = new System.Drawing.Size(160, 14);
            // 
            // TweetUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.signature);
            this.Controls.Add(this.tweet);
            this.Controls.Add(this.pseudo);
            this.Controls.Add(this.avatar);
            this.Name = "TweetUC";
            this.Size = new System.Drawing.Size(227, 85);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox avatar;
        private System.Windows.Forms.Label pseudo;
        private System.Windows.Forms.Label tweet;
        private System.Windows.Forms.Label signature;
    }
}
