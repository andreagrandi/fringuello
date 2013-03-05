using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NTwitter;
using CompactNTwitter;

namespace Fringuello
{
    public partial class UpdateStatus : Form
    {
        private string _username = "";
        private string _password = "";

        public UpdateStatus(string username, string password)
        {
            InitializeComponent();
            _username = username;
            _password = password;
        }

        private void txtStatus_TextChanged(object sender, EventArgs e)
        {
            lblLenght.Text = txtStatus.Text.Length.ToString();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuUpdate_Click(object sender, EventArgs e)
        {
            FringuelloUtil fu = new FringuelloUtil();
            fu.UpdateStatus(_username, _password, txtStatus.Text);
            MessageBox.Show("Status updated");
            this.Close();
        }
    }
}