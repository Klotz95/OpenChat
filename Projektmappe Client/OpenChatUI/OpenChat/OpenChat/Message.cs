using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace OpenChat
{
    class Message
    {
      //Attribute
      string PartnerID;
      string[] ChatHistory;
      System.Collections.ArrayList Content;

      //Kosntruktor
      public Message(string PartnerID)
      {
        this.PartnerID = PartnerID;
        ChatHistory = new string[0];
        Content = new System.Collections.ArrayList();
      }

      public void AddToHistory(bool fromMe,string Content)
      {
        string current;
        //is this message from me or the Partner
        if(fromMe)
        {
          current = "M:"+Content;
        }
        else
        {
          current = "P:" + Content;
        }
        //save it in the array
        string[] backup = ChatHistory;
        ChatHistory = new string[backup.Length + 1];
        for(int i = 0;i < backup.Length;i++)
        {
          ChatHistory[i] = backup[i];
        }
        ChatHistory[backup.Length] = current;
      }
      public void AddContent(byte[] content, bool fromMe)
      {
        string current;
        //is this pic from me?
        if(fromMe)
        {
          current = "M:";
        }
        else
        {
          current = "P:";
        }
        //now look for the lenght of the Picturearray and put in the right Placeholder
        current += "{"+ Content.Count.ToString() +"}";
        //save it into the ChatHistory
        string[] backup = ChatHistory;
        ChatHistory = new string[backup.Length + 1];
        for(int i = 0; i< backup.Length; i++)
        {
          ChatHistory[i] = backup[i];
        }
        ChatHistory[backup.Length] = current;
        //save the content
        Content.Add(content);
      }
      public string[] getHistory()
      {
        return ChatHistory;
      }
      public System.Collections.ArrayList getContent()
      {
        return Content;
      }
      public string GetPartnerID()
      {
        return PartnerID;
      }
    }
}
