using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenChat
{
    public partial class ServerIp : Form
    {
        bool opened;
        string IP;
        public ServerIp()
        {
            InitializeComponent();
            opened = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IP = textBox1.Text;
            opened = false;
            this.Close();
        }
        public bool getWindowStatus()
        {
            return opened;
        }
        public string GetIP()
        {
            return IP;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IP = "";
            opened = false;
            this.Close();
        }
    }
}
