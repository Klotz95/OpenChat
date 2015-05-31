using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace OpenChat
{
    class ServerOrganization
    {
      //Attribute
      string[] newMessages;
      System.Collections.ArrayList content;
      Socket Server;
      string IPsAddress;
      Thread Listener;
      
      public ServerOrganization(ref User myself,string IPsAddress)
      {
          
        newMessages = new string[0];
        this.IPsAddress = IPsAddress;
        IPAddress serverIP = IPAddress.Parse(IPsAddress);
        IPEndPoint ie = new IPEndPoint(serverIP,2031);
        Server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        bool connected = false;
        try
        {
          //try to connect to Server
          Server.Connect(ie);
          connected = true;
        }
        catch
        {
          connected = false;
        }
        if(connected)
        {
          //get the ID
          ASCIIEncoding enc = new ASCIIEncoding();
          byte[] buffer = new byte[100];
          Server.Receive(buffer);
          string ID = enc.GetString(buffer);
          //Remove empty spaces
          ID = ID.Replace(" ","");
          ID = ID.Replace("\0", "");
          myself.SetUserID(ID);
          //Introduce myself
          string Intro = "{CN}";
          Intro += "{US:" + myself.GetUserName() + "}" + "{UI:" + myself.GetUserID() + "}";
          if(myself.GetComment().Length != 0)
          {
            Intro += "{CM:" + myself.GetComment() + "}";
          }
          if (myself.getProfilePic().Length != 0)
          {
              string pp = enc.GetString(myself.getProfilePic());
              pp = "{PP:" + pp + "}";
              Intro += pp;
          }
          //now send this informations
          byte[] sendableMessage = enc.GetBytes(Intro);
          Console.WriteLine("sendable Message Count: {0}", sendableMessage.Length);
          Server.Send(sendableMessage);
          //now the server is informed about me. Start Thread to receive messages of serverIP
          Listener = new Thread(Listen);
          Listener.Start();
        }
    }
    private void Listen()
    {
      ASCIIEncoding enc = new ASCIIEncoding();
      bool rest = false;
      string restMessage ="";
      while (true)
      {
          byte[] buffer = new byte[1000000];
          Server.Receive(buffer);
          string receivedMessage = enc.GetString(buffer);
          if (rest)
          {
              receivedMessage = restMessage + receivedMessage;
              rest = false;
          }
          //seperate the current Datas
          string currentMessage = "";
          string[] receivedMessageSeperated= new string[0];
          for(int i = 0; i < receivedMessage.Length; i++)
          {
            if(receivedMessage[i] == '\n')
            {
              string[] backup = receivedMessageSeperated;
              receivedMessageSeperated = new string[backup.Length + 1];
              for(int k = 0; k < backup.Length; k++)
              {
                receivedMessageSeperated[k] = backup[k];
              }
              receivedMessageSeperated[backup.Length] = currentMessage;
              currentMessage = "";
            }
            else
            {
                                                                                    currentMessage += Convert.ToString(receivedMessage[i]);
            }
          }
          //now check if there is a rest
          if(currentMessage != "")
          {
            //there is a rest...
              string[] backup = receivedMessageSeperated;
              receivedMessageSeperated = new string[backup.Length + 1];
              for (int k = 0; k < backup.Length; k++)
              {
                  receivedMessageSeperated[k] = backup[k];
              }
              receivedMessageSeperated[backup.Length] = currentMessage;
              currentMessage = "";
          }
          //save the received messages to the array
          string[] backupNewMessages = newMessages;
          newMessages = new string[backupNewMessages.Length + receivedMessageSeperated.Length];
          for(int i = 0; i < backupNewMessages.Length; i++)
          {
            newMessages[i] = backupNewMessages[i];
          }
          for(int i = backupNewMessages.Length; i < newMessages.Length; i++)
          {
            newMessages[i] = receivedMessageSeperated[i - backupNewMessages.Length];
          }
          //now the new message has been received and saved

      }
    }
    public void SendMessage(string Message)
    {
      ASCIIEncoding enc = new ASCIIEncoding();
      byte[] content = enc.GetBytes(Message);
      Server.Send(content);
    }
    public string[] GetNewMessages()
    {
        string[] returnValue = newMessages;
        newMessages = new string[0];
        return returnValue;
    }
    public void Shutdown()
    {
      SendMessage("<EOT>");
      Server.Shutdown(SocketShutdown.Both);
      Listener.Suspend();
      Server.Close();
    }
    }
}
