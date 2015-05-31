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
      bool MessageChanged;
      //Konstuktor
      public Organization(string UserName,string Comment, byte[] ProfilePic)
      {
        //connect to server and get my ID
        string ID = "";


        //now create mySelf
        myself = new User(UserName,Comment,ProfilePic,ID);
        serverHandling = new ServerOrganization(ref myself,"192.168.1.1");
        currentConnetced = new User[0];
        ChatHistory = new Message[0];
        MessageChanged = true;
        refreshThread = new Thread(refreshContent);
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
            MessageChanged = true;
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
          MessageChanged = true;
        }
      }
      public void SendMessageWithContent(string Content, string UserID, byte[] picture)
      {
        //Make a sendableMessage
        string sendableMessage = "{"+UserID+"}"+"{C}" + Content;
        serverHandling.SendMessage(sendableMessage);

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
          ChatHistory = new Message[backup.Length + 1];
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
      public bool getChangedMessage()
      {
        bool returnvalue = MessageChanged;
        MessageChanged = false;
        return returnvalue;
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

        serverHandling = new ServerOrganization(ref myself,IPAddress);
        if(myself == null)
        {
          return false;
        }
        else
        {

          return true;
        }
      }
      private void refreshContent()
      {
          while (true)
          {
              string[] messages = serverHandling.GetNewMessages();
              workWithNewContent(messages);
              Thread.Sleep(100);
          }

      }
      private void workWithNewContent(string[] messages)
      {
        for (int i = 0; i < messages.Length; i++)
        {
          //check the first bracket
          string current = messages[i];
          current = current.Replace("\0", "");
          string firstBracket = "";
          for(int k = 0;k< current.Length;k++)
          {
            if(current[k] == '}')
            {
              firstBracket += current[k];
              break;
            }
            else
            {
              firstBracket += current[k];
            }
          }
          //now check if there is a CN or DC inside of the message
          if(firstBracket == "{CN}")
          {
            //a client connected
            //get all important informations
            current = current.Replace(firstBracket,"");
            //seperate every bracket and get the informations
            byte[] UserPic = new byte[0];
            string comment ="";
            string UserName = "";
            string UserId = "";
            string[] currentSeperated = seperateBrackets(current);
            for(int k = 0; k< currentSeperated.Length ; k++)
            {
              string currentBracket = currentSeperated[k];
              if(currentBracket.Contains("{US:"))
              {
                //UserName
                currentBracket = currentBracket.Replace("{US:","");
                currentBracket = currentBracket.Replace("}","");
                UserName = currentBracket;
              }
              else if(currentBracket.Contains("{UI:"))
              {
                //User-ID
                currentBracket = currentBracket.Replace("{UI:","");
                currentBracket = currentBracket.Replace("}","");
                UserId = currentBracket;
              }
              else if(currentBracket.Contains("{CM:"))
              {
                //Comment
                currentBracket = currentBracket.Replace("{CM:","");
                currentBracket = currentBracket.Replace("}","");
                comment = currentBracket;
              }
              else if (currentBracket.Contains("{PP:"))
              {
                  //Profile Pic
                  currentBracket = currentBracket.Replace("PP:", "");
                  currentBracket = currentBracket.Replace("}", "");
                  ASCIIEncoding enc = new ASCIIEncoding();
                  UserPic = enc.GetBytes(currentBracket);
              }
            }
            //create the new user object
            User newConnected = new User(UserName,comment,UserPic,UserId);
            //add it to the array
            User[] backup = currentConnetced;
           currentConnetced = new User[backup.Length +1];
            for(int k = 0; k<backup.Length;k++)
            {
              currentConnetced[k] = backup[k];
            }
            currentConnetced[backup.Length] = newConnected;
            //finish//
          }
          else if(firstBracket == "{DC}")
          {
            //a client disconnected
            current = current.Replace(firstBracket,"");
            //serach the ID and delete all Messages of that User
            for(int k = 0; k < currentConnetced.Length; k++)
            {
              if(currentConnetced[k].GetUserID() == current)
              {
                currentConnetced[k] = null;
                break;
              }
            }
            //search for the Messages
            for(int k = 0; k < ChatHistory.Length ; k++)
            {
              if(ChatHistory[k].GetPartnerID() == current)
              {
                ChatHistory[k] = null;
              }
            }
            //now clean up the Arraylist: Userlist
            User[] newCurrentConnected = new User[0];
            for(int k = 0; k < currentConnetced.Length; k++)
            {
              if(currentConnetced[k] != null)
              {
                User[] newCurrentConnectedBackup = newCurrentConnected;
                newCurrentConnected = new User[newCurrentConnectedBackup.Length + 1];
                for(int j = 0; j<newCurrentConnectedBackup.Length;j++)
                {
                  newCurrentConnected[j] = newCurrentConnectedBackup[j];
                }
                newCurrentConnected[newCurrentConnectedBackup.Length] = currentConnetced[k];
              }
            }
            currentConnetced = newCurrentConnected;
            //now clean up the Arraylist: Message
            Message[] newChatHistory = new Message[0];
            for(int k = 0; k< ChatHistory.Length; k++)
            {
              if(ChatHistory[k] != null)
              {
                Message[] newChatHistoryBackup = newChatHistory;
                newChatHistory = new Message[newChatHistoryBackup.Length + 1];
                for(int j = 0; j < newChatHistoryBackup.Length; j++)
                {
                  newChatHistory[j] = newChatHistoryBackup[j];
                }
                newChatHistory[newChatHistoryBackup.Length] = ChatHistory[k];
              }
            }
            ChatHistory = newChatHistory;
            //all arrays are clear and the user has clomplety disconnected
          }
          else
          {
            //a message received
            //the first bracket contains the User_ID
              string message = "";
              try
              {
                  message = seperateBrackets(messages[i])[1];
              }
              catch
              {

              }
              if (message != "")
              {
                  message = message.Replace("{", "");
                  message = message.Replace("}", "");
                  firstBracket = firstBracket.Replace("{", "");
                  firstBracket = firstBracket.Replace("}", "");
                  //now search for the User-ID to add this message to the history
                  bool found = false;
                  for (int k = 0; k < ChatHistory.Length; k++)
                  {
                      if (ChatHistory[k].GetPartnerID() == firstBracket)
                      {
                          //add the message to the history
                          ChatHistory[k].AddToHistory(false, message);
                          found = true;
                          MessageChanged = true;
                          break;
                      }
                  }
                  if (!found)
                  {
                      //create a new Message
                      Message newMessage = new Message(firstBracket);
                      newMessage.AddToHistory(false, message);
                      //add it to the history
                      Message[] backup = ChatHistory;
                      ChatHistory = new Message[backup.Length + 1];
                      for (int k = 0; k < backup.Length; k++)
                      {
                          ChatHistory[k] = backup[k];
                      }
                      ChatHistory[backup.Length] = newMessage;
                      MessageChanged = true;
                  }
                  //new message is now stored
              }
          }

        }
      }
      private string[] seperateBrackets(string workString)
      {

        string[] seperated = new string[0];
        string currentBracket = "";
        for(int i = 0; i<workString.Length; i++)
        {
          if(workString[i] == '}')
          {
            //start a new bracket
            currentBracket += workString[i];
            string[] backup = seperated;
            seperated = new string[backup.Length + 1];
            for(int k = 0; k < backup.Length;k++)
            {
              seperated[k] = backup[k];
            }
            seperated[backup.Length] = currentBracket;
            currentBracket = "";
          }
          else
          {
            currentBracket += workString[i];
          }
        }
        return seperated;

      }
      public void ShutDownClient()
      {
          refreshThread.Suspend();
        serverHandling.Shutdown();
      }

    }
}
