using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace OpenChat
{

    public partial class MainMenu : Form
    {
        //Attribute
        string Name;
        string Comment;
        byte[] Picture;
        User[] currentOnline;
        Message[] ChatHistory;
        Organization orga;
        Thread refreshThread;
        string CurrentSeletctedChatID = "";
        public MainMenu(string Name,string Comment,byte[] picture)
        {
            InitializeComponent();
            this.Name = Name;
            this.Comment = Comment;
            this.Picture = picture;

            //now integrate the Logic 
            orga = new Organization(Name, Comment, Picture);
            refreshThread = new Thread(RefreshContent);
            refreshThread.Start();
        }
        
        
        private void MainMenu_Load(object sender, EventArgs e)
        {
            
            
            //set minimum size
            this.MinimumSize = new Size(1200,750);
            //Panel 1
            panel1.Location = new Point(0, 0);
            panel1.Height = this.ClientSize.Height -150;
            panel1.Width = 300;

            //Panel 2
            panel2.Location = new Point(0, this.ClientSize.Height - 150);
            panel2.Height = 150;
            panel2.Width = this.ClientSize.Width;

            //Panel 3
            panel3.Location = new Point(panel1.Width, 0);
            panel3.Width = this.ClientSize.Width - panel1.Width;
            panel3.Height = Convert.ToInt32(this.ClientSize.Height - panel2.Height);
            
            //Panel 4
            panel4.Visible = false;

            //send Button
            button2.Text = "send";
            button2.Size = new Size(100, 50);
            button2.BackColor = Color.White;
            button2.Location = new Point(panel2.Width - 10 - button2.Width,(panel2.Height/2) - button2.Height/2);
           
            //text box of Panel 2

            textBox1.Location = new Point(panel1.Width, 10);
            textBox1.Width = panel2.Width - 30 - button2.Width - panel1.Width;
            textBox1.Height = panel2.Height - 20;
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;

            //Setting Symbol of Panel 2
            settingSymbol.Location = new Point(10, panel2.Height - settingSymbol.Height - 10);


            //setUp of Panel 3
            panel3.AutoScroll = true;
            panel3.VerticalScroll.Visible = false;
           
            //setUp Panel1
            panel1.AutoScroll = true;
            panel1.VerticalScroll.Visible = false;


        }

        private void MainMenu_SizeChanged(object sender, EventArgs e)
        {
            //Change the Panel Sizes
            //Panel1
            panel1.Height = this.ClientSize.Height -150;
           
            //Panel 2
            panel2.Location = new Point(0, this.ClientSize.Height - panel2.Height);
            panel2.Width = this.ClientSize.Width;

            //Panel 3
            panel3.Location = new Point(panel1.Width, 0);
            panel3.Width = this.ClientSize.Width - panel1.Width;
            panel3.Height = this.ClientSize.Height - panel2.Height;

            //send Button
            button2.Location = new Point(panel2.Width - 10 - button2.Width, (panel2.Height / 2) - button2.Height / 2);
            
            //textbox of Panel2
            textBox1.Location = new Point(panel1.Width, 10);
            textBox1.Width = panel2.Width - 30 - button2.Width -panel1.Width;

            //button of Panel 1
            settingSymbol.Location = new Point(10, panel2.Height - settingSymbol.Height - 10);

            //bubble arangement
            ResizeBubbles();

            //current Online User arrangement
            ResizeCurrentOnlineUser();



            
        }

        private void settingSymbol_Click(object sender, EventArgs e)
        {
            if (panel4.Visible == false)
            {
                panel4.Visible = true;
                panel4.Location = new Point(0, panel2.Location.Y - 20);
            }
            else
            {
                panel4.Visible = false;
            }
        }
        private void drawMessages(string[] messages)
        {
            //check all Messages and create a picture for it
            int heightdistance = 10;
            for (int i = 0; i < messages.Length; i++)
            {
                string current = messages[i];
                bool fromMe = false;
                string ChatID = Convert.ToString(current[0]) + Convert.ToString(current[1]);
                current = current.Remove(0, 2);
                if (ChatID == "M:")
                {
                    fromMe = true;
                }
                PictureBox CurrentBubble = new PictureBox();
                int lineCount = Convert.ToInt32(current.Length / 33);
                lineCount++;
                CurrentBubble.Size = new Size(400, 30 *lineCount);
                Bitmap Drawarea = new Bitmap(CurrentBubble.Width,CurrentBubble.Height);
                Graphics g = Graphics.FromImage(Drawarea);
                g.FillRectangle(Brushes.LightGreen,new Rectangle(new Point(0,0),new Size(CurrentBubble.Width,CurrentBubble.Height)));
                //draw the font
                Point cursorPosition = new Point(5, 5);
                //seperate the string in 33 Letters
                string[] currentSeperated = new string[0];
                string currentValue = "";
                for (int k = 0; k < current.Length; k++)
                {
                    currentValue += current[k];
                    if (currentValue.Length == 33)
                    {
                        string[] backup = currentSeperated;
                        currentSeperated = new String[backup.Length + 1];
                        for (int j = 0; j < backup.Length; j++)
                        {
                            currentSeperated[j] = backup[j];
                        }
                        if (currentValue[0] == ' ')
                        {
                            currentValue.Remove(0, 1);
                        }
                        currentSeperated[backup.Length] = currentValue;
                        currentValue = "";
                    }
                }
                if (currentValue.Length != 0)
                {
                    //save the rest
                    string[] backup = currentSeperated;
                    currentSeperated = new string[backup.Length + 1];
                    for (int k = 0; k < backup.Length; k++)
                    {
                        currentSeperated[k] = backup[k];
                    }
                    currentSeperated[backup.Length] = currentValue;
                }
                //draw
                for (int k = 0; k < currentSeperated.Length; k++)
                {
                    g.DrawString(currentSeperated[k], DefaultFont, Brushes.White, cursorPosition);
                    cursorPosition.Y += 25;
                }
                CurrentBubble.Image = Drawarea;
                panel3.Controls.Add(CurrentBubble);
                
                //arange the bubble
                if (fromMe)
                {
                    CurrentBubble.Location = new Point(panel3.Width - CurrentBubble.Width - 60, heightdistance);
                }
                else
                {
                    CurrentBubble.Location = new Point(20, heightdistance);
                }
                heightdistance += CurrentBubble.Height + 20;
                ResizeBubbles();
            }
        }
        private void ResizeBubbles()
        {
            
            Control.ControlCollection bubbles = panel3.Controls;
  
            
            //Curosor
            int cursor = 10;
            
            foreach (PictureBox c in bubbles)
            { 
                //stretch
                c.SizeMode = PictureBoxSizeMode.StretchImage;
                
                
                //check for left or right
                if (!(c.Location.X - 20 == 0))
                {
                    c.Location = new Point(panel3.Width - c.Width - 60, cursor);
                }
                else
                {
                    c.Location = new Point(20, cursor);

                }
                cursor += 20 + c.Height;

            }
             
        }
        private void panel3_MouseEnter(object sender, EventArgs e)
        {
            panel3.Focus();
        }
        private void ResizeCurrentOnlineUser()
        {
            Control.ControlCollection UserPanels = panel1.Controls;
            int height = 0;
            foreach (PictureBox a in UserPanels)
            {
                a.BorderStyle = BorderStyle.FixedSingle;
                a.Location = new Point(0, height);
                height += a.Height + 20;

            }
        }
        private void PaintCurrentOnlineUser()
        {
            for (int i = 0; i < currentOnline.Length; i++)
            {
                User current = currentOnline[i];
                string Name = current.GetUserName();
                string comment = current.GetComment();
                byte[] ProfilePic = current.getProfilePic();

                //create a Bitmap for painting
                Bitmap Drawarea = new Bitmap(300, 120);
                Graphics g = Graphics.FromImage(Drawarea);
                PictureBox UserPanel = new PictureBox();
                //draw the Background
                g.FillRectangle(Brushes.WhiteSmoke, new Rectangle(new Point(0, 0), Drawarea.Size));
                //draw the Profile Pic
                if (ProfilePic.Length == 0)
                {
                    //standart picture
                    Image standart = Image.FromFile("Z:/OpenChat/Projektmappe Client/OpenChatUI/OpenChat/OpenChat/profile-no-image.jpg");
                    g.DrawImage(standart, 10, 10, 100, 100);
                }
                else
                {
                    MemoryStream ms = new MemoryStream(ProfilePic);
                    Image UserProfile = Image.FromStream(ms);
                    g.DrawImage(UserProfile, 10, 10, 100, 100);
                }
                //draw the Name and the comment
                g.DrawString(Name, DefaultFont, Brushes.Black, new PointF(120, 25));
                g.DrawString(comment, DefaultFont, Brushes.Black, new Point(120, 50));
                UserPanel.Size = Drawarea.Size;
                UserPanel.Image = Drawarea;
                panel1.Controls.Add(UserPanel);
                //resize
                ResizeCurrentOnlineUser();
            }
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            panel1.Focus();
        }
        UserInformation UI;
        private void button3_Click(object sender, EventArgs e)
        {
            UI = new UserInformation(Name, Comment, Picture);
            panel4.Visible = false;
            UI.Show();
            Thread t1 = new Thread(UserInformationChanged);
            t1.Start();
        }
        private void UserInformationChanged()
        {
            while (UI.getWindowStatus())
            {
                Thread.Sleep(100);
            }
            UI.getInformation(ref Name, ref Comment, ref Picture);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        ServerIp si;
        private void button1_Click(object sender, EventArgs e)
        {
            si = new ServerIp();
            panel4.Hide();
            Thread t1 = new Thread(checkforSi);
            t1.Start();
            si.Show();
        }
        private void checkforSi()
        {
            while (si.getWindowStatus())
            {
                Thread.Sleep(100);
            }
            string IP = si.GetIP();
            MessageBox.Show(IP);
        }
        private void RefreshContent()
        {
            //refresh Online-List and ChatHistory
            while (true)
            {
                User[] check = orga.getCurrentUser();
                Message[] checkMessage = orga.getChatHistory();
                if (ChatHistory != checkMessage)
                {
                    ChatHistory = checkMessage;
                }
                if (currentOnline != check)
                {
                    currentOnline = check;
                    panel1.Controls.Clear();
                    PaintCurrentOnlineUser();
                }
            }
        }

      
    }
}
