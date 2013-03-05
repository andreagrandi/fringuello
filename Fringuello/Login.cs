using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Fringuello
{
    public partial class Login : Form
    {
        private string userName = "";
        private string password = "";

        public Login()
        {
            InitializeComponent();
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            string previousNode = "";

            try
            {
                if (System.IO.File.Exists(getCurrentPath() + "configuration.xml"))
                {
                    XmlTextReader xmlReader = new XmlTextReader(getCurrentPath() + "configuration.xml");
                    xmlReader.MoveToContent();

                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                previousNode = xmlReader.Name;

                                switch (previousNode)
                                {
                                    case "user":
                                        userName = xmlReader.GetAttribute("username");
                                        password = xmlReader.GetAttribute("password");
                                        break;
                                }
                                break;
                        }
                    }
                    xmlReader.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if (userName != "")
                txtUsername.Text = userName;
            if (password != "")
                txtPassword.Text = password;
        }

        private void mnuLogin_Click(object sender, EventArgs e)
        {
            SaveConfigurationTest();
            MainFeed mainFrm = new MainFeed(txtUsername.Text, txtPassword.Text);
            mainFrm.Show();
        }

        private void SaveConfigurationTest()
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(getCurrentPath() + "configuration.xml", Encoding.ASCII);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("configuration");

            xmlWriter.WriteStartElement("user");
            xmlWriter.WriteAttributeString("username", txtUsername.Text);
            xmlWriter.WriteAttributeString("password", txtPassword.Text);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }
        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string getCurrentPath() 
        {
            string full_path = System.Reflection.Assembly.GetCallingAssembly().GetName().CodeBase;
            string directory_path = full_path.Substring(0, full_path.LastIndexOf("\\"));
            return directory_path + "\\";
        }
    }
}