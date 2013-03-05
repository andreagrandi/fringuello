namespace Fringuello
{
    partial class MainFeed
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mnuMainFeed;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mnuMainFeed = new System.Windows.Forms.MainMenu();
            this.mnuLogout = new System.Windows.Forms.MenuItem();
            this.mnuNewStatus = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mnuMainFeed
            // 
            this.mnuMainFeed.MenuItems.Add(this.mnuLogout);
            this.mnuMainFeed.MenuItems.Add(this.mnuNewStatus);
            // 
            // mnuLogout
            // 
            this.mnuLogout.Text = "Logout";
            this.mnuLogout.Click += new System.EventHandler(this.mnuLogout_Click);
            // 
            // mnuNewStatus
            // 
            this.mnuNewStatus.Text = "New Status";
            this.mnuNewStatus.Click += new System.EventHandler(this.mnuNewStatus_Click);
            // 
            // MainFeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Menu = this.mnuMainFeed;
            this.Name = "MainFeed";
            this.Text = "Twitter Feed";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuLogout;
        private System.Windows.Forms.MenuItem mnuNewStatus;
    }
}