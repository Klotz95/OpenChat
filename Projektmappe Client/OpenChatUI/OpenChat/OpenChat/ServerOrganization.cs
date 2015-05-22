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
        IPEndPoint ie = new IPEndPoint(serverIP,2030);
        Server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        try
        {
          //try to connect to Server
          Server.Connect(ie);
        }
        catch
        {
          myself = null;
        }
        if(myself != null)
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
          //now send this informations
          byte[] sendableMessage = enc.GetBytes(Intro);
          Server.Send(sendableMessage);
          //now the server is informed about me. Start Thread to receive messages of serverIP
          Listener = new Thread(Listen);
          Listener.Start();
        }
    }
    private void Listen()
    {
      ASCIIEncoding enc = new ASCIIEncoding();
      while(true)
      {
        byte[] buffer = new byte[1000];
        Server.Receive(buffer);
        string current = enc.GetString(buffer);
        int indexOf0 = 0;
        for(int i = 0; i<current.Length;i++)
        {
          if(current[i] == '\0')
          {
            indexOf0 = i;
            break;
          }
        }
        if(indexOf0 != 0)
        {
          current = current.Remove(indexOf0);
        }
        else
        {
          bool more = true;
          while(more)
          {
            buffer = new byte[1000];
            Server.Receive(buffer);
            current += enc.GetString(buffer);
            //search for the end
            for(int i = 0 ; i<current.Length;i++)
            {
              if(current[i] == '\0')
              {
                indexOf0 = i;
                more = false;
                break;
              }
            }
          }
          current = current.Remove(indexOf0);
        }
        //now the message has been completly received
        //Add this message to the receivedMessage-Array
        string[] backup = newMessages;
        newMessages = new string[backup.Length + 1];
        for(int i = 0;i<backup.Length; i++)
        {
          newMessages[i] = backup[i];
        }
        newMessages[backup.Length] = current;
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
      Server.Close();
    }
    }
}
