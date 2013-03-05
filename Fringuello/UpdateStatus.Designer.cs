namespace Fringuello
{
    partial class UpdateStatus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mnuStatus;

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
            this.mnuStatus = new System.Windows.Forms.MainMenu();
            this.mnuCancel = new System.Windows.Forms.MenuItem();
            this.mnuUpdate = new System.Windows.Forms.MenuItem();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLenght = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mnuStatus
            // 
            this.mnuStatus.MenuItems.Add(this.mnuCancel);
            this.mnuStatus.MenuItems.Add(this.mnuUpdate);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Text = "Cancel";
            this.mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
            // 
            // mnuUpdate
            // 
            this.mnuUpdate.Text = "Update";
            this.mnuUpdate.Click += new System.EventHandler(this.mnuUpdate_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(4, 4);
            this.txtStatus.MaxLength = 140;
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(233, 109);
            this.txtStatus.TabIndex = 0;
            this.txtStatus.TextChanged += new System.EventHandler(this.txtStatus_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "Message lenght:";
            // 
            // lblLenght
            // 
            this.lblLenght.Location = new System.Drawing.Point(98, 120);
            this.lblLenght.Name = "lblLenght";
            this.lblLenght.Size = new System.Drawing.Size(41, 20);
            this.lblLenght.Text = "0";
            this.lblLenght.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // UpdateStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lblLenght);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStatus);
            this.Menu = this.mnuStatus;
            this.Name = "UpdateStatus";
            this.Text = "Twitter Status";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuCancel;
        private System.Windows.Forms.MenuItem mnuUpdate;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLenght;
    }
}