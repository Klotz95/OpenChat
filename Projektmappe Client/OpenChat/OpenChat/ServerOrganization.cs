using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.Threading;
namespace OpenChat
{
    class ServerOrganization
    {
      //Attribute
      string[] newMessages;
      Arraylist content;
      Socket Server;
      string IP-Address:
      Thread Listener;
      //Konstruktor
      public ServerOrganization(ref string USERID, string IP-Address,User myself)
      {
        //Create the Socket
        IPAddress serverIP = IPAddress.Parse(IP-Address);
        IPEndPoint ie = new IPEndPoint(serverIP,3000);
        Server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        //now try to connect
        try
        {
          Server.Connect(ie);
        }
        catch
        {
          return;
        }
        //server is now reachable. Now receive our ID
        byte[]buffer = new byte[100];
        Server.Receive(buffer);
        ASCIIEncoding enc = new ASCIIEncoding();
        USERID = enc.GetString(buffer);
        USERID = USERID.Replace(" ","");
        //introduce myself
        //get all informations to create a sendable information
        string sendableMessage = "{CN}";
        byte[] ProfilePic = myself.getProfilePic();
        string UserName = myself.GetUserName();
        string comment = myself.GetComment();

        if(USERID != null || USERID != "")
        {
          //now create the sendableMessage
          if(ProfilePic.Length != 0)
          {
            sendableMessage += "{PP:"+ ProfilePic.Length.ToString() + "}";
          }
          sendableMessage += "{US:" + UserName + "}";
          if(comment != "")
          {
            sendableMessage += "{CM:" + comment + "}";
          }
          sendableMessage += "{UI:" + USERID +"}";
          //now the message is ready to send
          byte[] readyToSend = enc.GetByte(sendableMessage);
          Send(readyToSend);
          //now creating the thread to get the incomming messages
          Listener = new Thread(listen);
          Listener.Start();
        }


      }
      private void listen()
      {
        while(true)
        {
        //now receive the content of the server
        byte[] lengthinByte = new byte[32];
        Server.Receive(lengthinByte);
        int length = BitConverter.ToInt32(lengthinByte,0)
        byte[] content = new byte[length];
        Server.Receive(content,length);
        ASCIIEncoding enc = new ASCIIEncoding();
        string receivedMessage = enc.GetString(content);
        string currentSeperated[] = receivedMessage.Split("{");
        bool incommingContent = false;
        int lengthOfPic;
        for(int i = 0; i < currentSeperated.Length; i++)
        {
          if(currentSeperated[i].Contains("PP:"))
          {
            string current = currentSeperated[i].Remove(0,3);
            string LengthInString = "";
            for(int k = 0; current[k] != "}" || k < 20; k++)
            {
              LengthInString += current[k];
            }
            //remove empty spaces
            LengthInString = LengthInString.Replace(" ","");
            //now try to convert
            try
            {
              lengthOfPic = Convert.ToInt32(LengthInString)
              incommingContent = true;
            }
            catch
            {

            }
            //now put in that string the place of the array


          }
          else if(currentSeperated[i].Contains("C:"))
          {
            string current = currentSeperated[i].Remove(0,2);
            string LengthInString = "";
            for(int k = 0; current[k] != "}" || k < 20; k++)
            {
              LengthInString += current[k];
            }
            //remove empty spaces
            LengthInString = LengthInString.Replace(" ","");
            //now try to convert
            try
            {
              lengthOfPic = Convert.ToInt32(LengthInString)
              incommingContent = true;
            }
            catch
            {

            }
          }
        }
        }
      }
      private void Send(byte[] c)
      {

      }
      public void SendContent(byte[]content)
      {

      }
      public void SendMessage(string message)
      {

      }
      public string[] GetNewMessages()
      {
        string[] returnValue = newMessages;
        newMessages = new string[0];
        return returnValue;
      }
      public Arraylist GetNewContent()
      {
        Arraylist returnValue = content;
        content.Clear();
        return returnValue ;
      }

    }
}
