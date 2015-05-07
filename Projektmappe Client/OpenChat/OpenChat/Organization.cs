using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
namespace OpenChat
{
    class Organization
    {
      //Attribute
      User myself;
      User[] currentConnetced;
      ServerOrganization serverHandling;
      Message[] ChatHistory;
      Thread refreshThread;
      //Konstuktor
      public Organization(string UserName,string Comment, byte[] ProfilePic)
      {
        //connect to server and get my ID
        string ID = "";
        bool check = true;
        serverHandling = new ServerOrganization(ref ID);
        //now create mySelf
        myself = new User(UserName,Comment,ProfilePic,ID);
        serverHandling.SendMyself(true);
        currentConnetced = new User[0];
        ChatHistory = new Message[0];
        refreshThread = new Thread(refreshContent)
        refreshThread.Start();
      }
      public void SendMessage(string content, string UserID)
      {
        //Make a sendableMessage
        string sendableMessage = "{" + UserID + "}" + content;
        serverHandling.SendMessage(sendableMessage);
        //search the message to save
        bool found = false;
        for(int i = 0; i<ChatHistory.Length;i++)
        {
          if(ChatHistory[i].GetPartnerID() == UserID)
          {
            ChatHistory[i].AddToHistory(true,content);
            found = true;
          }
        }
        if(!found)
        {
          //create a new message to save the history
          Message[] backup = ChatHistory;
          ChatHistory = new Message[backup.Length + 1];
          for(int i = 0; i< backup.Length; i++)
          {
            ChatHistory[i] = backup[i];
          }
          ChatHistory[backup.Length] = new Message(UserID);
          ChatHistory[backup.Length].AddToHistory(true,content);
        }
      }
      public void SendMessageWithContent(string Content, string UserID, byte[] picture)
      {
        //Make a sendableMessage
        string sendableMessage = "{C}"+"{"+UserID+"}" + Content;
        serverHandling.SendMessage(sendableMessage);
        serverHandling.SendContent(picture);
        //search the Message to save
        bool found = false;
        for(int i = 0; i< ChatHistory.Length; i++)
        {
          if(ChatHistory[i].GetPartnerID()  == UserID)
          {
            found = true;
            ChatHistory[i].AddToHistory(true,Content);
            ChatHistory[i].AddContent(picture,true);
          }
        }
        if(!found)
        {
          //save the current array
          Message[] backup = ChatHistory;
          ChatHistory = new backup[backup.Length + 1]
          for(int i  = 0 ; i<backup.Length;i++)
          {
            ChatHistory[i] = backup[i];
          }
          ChatHistory[backup.Length] = new Message(UserID);
          //now save the message
          ChatHistory[backup.Length].AddToHistory(true, Content);
          ChatHistory[backup.Length].AddContent(picture,true);
        }
      }
      public Message[] getChatHistory()
      {
        return ChatHistory;
      }
      public User[] getCurrentUser()
      {
        return currentConnetced;
      }
      public User getMyself()
      {
        return myself;
      }
      public void ChangeMyself(User myself)
      {
        this.myself = myself;
      }
      public bool ChangeIp(string IPAddress)
      {
        string ID = "";
        serverHandling = new ServerOrganization(ref ID,IPAddress);
        if(ID == "")
        {
          return false;
        }
        else
        {
          myself.SetUserID(ID);
          return true;
        }
      }
      private void refreshContent()
      {
        //Get the new Content
        Arraylist Content = serverHandling.GetContent();
        string[] messages = serverHandling.GetNewMessages();


      }
      private void workWithNewContent(Arraylist Content, string[] messages)
      {
        for(int i = 0; i< messages.Length; i++)
        {
          //check if the message is a announcement for a new user, a disconnetced user or a new message
          if((string Controlstring = messages[i].Remove(4)) == "{CN}" || Controlstring == "{DC}")
          {
            //a User announcement

          }
          else
          {
            //a new message
            //first of all get User-ID
            string UserID = "";
            string MessageContent;
            for(int k = 1; messages[k] != '}';i++)
            {
              UserID += messages[k];
            }
            //now look if there is a Content in that message
            string checkForContent = messages[i].Remove(UserID.Length+2)
            if(checkForContent[0] == '{')
            {
              //there is a Content
              //get the value for the content
              
            }
            else
            {
              //there is no more content
              //Save the message
              bool exist = false;
              for (int k = 0; k < ChatHistory.Length;k++)
              {
                if(ChatHistory[k].GetPartnerID() == UserID)
                {
                  //now save the message
                  ChatHistory[k].AddToHistory(false,messages[i].Replace("{" + UserID +"}",""));
                  exist = true;
                  break;
                }
              }
              if(!exist)
              {
                //Create a new Message
                Message[] backup = ChatHistory;
                ChatHistory = new Message[backup.Length +1];
                for(int k = 0; k<ChatHistory.Length;k++)
                {
                  ChatHistory[k] = backup[k];
                }
                ChatHistory[backup.Length] = new Message(UserID);
                ChatHistory[backup.Length].AddToHistory(false, messages[i].Replace("{"+ UserID + "}",""));
              }
            }
          }
        }
      }
      public void shutdownClient()
      {

      }

    }
}
