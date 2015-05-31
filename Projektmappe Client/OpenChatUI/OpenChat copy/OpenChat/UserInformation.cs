using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace OpenChat
{
    public partial class UserInformation : Form
    {
        //Attribute 
        string Name;
        string comment;
        byte[] ProfilePic;
        bool opened;
        public UserInformation(string Name, string comment, byte[] ProfilePic)
        {
            InitializeComponent();
            this.Name = Name;
            this.comment = comment;
            this.ProfilePic = ProfilePic;
            opened = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog os = new OpenFileDialog();
            os.ShowDialog();
            try
            {
                pictureBox1.Image = Image.FromFile(os.FileName);
            }
            catch
            {
                MessageBox.Show("A not valid file was selected");
            }
        }

        private void UserInformation_Load(object sender, EventArgs e)
        {
            //get Informations and fill the UI-Elements
            textBox1.Text = Name;
            textBox2.Text = comment;
            byte[] ProfilePicInByte = ProfilePic;
            if (ProfilePicInByte.Length != 0)
            {
                MemoryStream ms = new MemoryStream(ProfilePicInByte);
                pictureBox1.Image = Image.FromStream(ms);
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //close this window
            opened = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                Name = textBox1.Text;
                comment = textBox2.Text;
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ProfilePic = ms.ToArray();
                this.Close();
                opened = false;
            }
            else
            {
                MessageBox.Show("you made a unvalid input", "warning");
            }
        }
        public bool getWindowStatus()
        {
            return opened;
        }
        public void getInformation(ref string Name, ref string comment,ref  byte[] ProfilePic)
        {
            Name = this.Name;
            comment = this.comment;
            ProfilePic = this.ProfilePic;
        }
    }
}
