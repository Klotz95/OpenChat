using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
namespace OpenChat
{
    public partial class StartUp : Form
    {
        public StartUp()
        {
            InitializeComponent();
        }
        //Attribute
        bool TextBox1Insert = false;
        bool TextBox2Insert = false;
        bool mystate = false;
        bool profilePic = false;

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog os = new OpenFileDialog();
            os.ShowDialog();
            try
            {
                pictureBox1.Image = Image.FromFile(os.FileName);
                profilePic = true;
            }
            catch
            {
                MessageBox.Show("You selected a not valid file");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox1.Text != "")
            {
                TextBox1Insert = true;
            }
            else
            {
                TextBox1Insert = false;
                this.Height = 535;
                mystate = false;
            }
            if (TextBox2Insert && TextBox1Insert)
            {
                if (!mystate)
                {

                    //draw the button and show it
                    mystate = true;
                    this.Height += 100;
                    button1.Visible = true;
                    button1.Width = this.Width;
                    button1.Height = 50;

                    button1.Location = new Point(0, this.Height - 100);
                    button1.Text = "Connect";
                    button1.ForeColor = Color.Green;
                    
                }
            }
        }

        

        

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                TextBox2Insert = true;
            }
            else
            {
                TextBox2Insert = false;
                this.Height = 535;
                mystate = false;
            }
            if (TextBox2Insert && TextBox1Insert)
            {
                if (!mystate)
                {

                    //draw the button and show it
                    mystate = true;
                    this.Height += 100;
                    button1.Visible = true;
                    button1.Width = this.Width;
                    button1.Height = 50;
                   
                    button1.Location = new Point(0, this.Height - 100);
                    button1.Text = "Connect";
                    button1.ForeColor = Color.Green;
                }
            }
        }

        private void StartUp_Load(object sender, EventArgs e)
        {
            button1.Visible = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;
            //get the User-Information
            string Name = textBox1.Text;
            string Comment = textBox2.Text;
            MemoryStream ms = new MemoryStream();
            //get the Image Format
            byte[] ProfilePic;
            if (profilePic)
            {
                ImageFormatConverter mf = new ImageFormatConverter();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                ProfilePic = ms.ToArray();
            }
            else
            {
                ProfilePic = new byte[0];
            }
            MainMenu mn = new MainMenu(Name, Comment, ProfilePic);
            mn.Show();
        }
    }
}
