using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenChat
{
    class User
    {
      //Attribute
      string UserName;
      string Comment;
      byte[] ProfilePic;
      string UserID;

      //Konstruktoren
      public User(string UserName,string Comment,byte[] ProfilePic,string UserID)
      {
        this.UserName = UserName;
        this.Comment = Comment;
        this.ProfilePic = ProfilePic;
        this.UserID = UserID;
      }

      public void setUserName(string UserName)
      {
        this.UserName = UserName;
      }
      public void setComment(string Comment)
      {
        this.Comment = Comment;
      }
      public void setProfilePic(byte[] ProfilePic)
      {
        this.ProfilePic = ProfilePic;
      }
      public string GetUserName()
      {
        return UserName;
      }
      public string GetComment()
      {
        return Comment;
      }
      public byte[] getProfilePic()
      {
        return ProfilePic;
      }
      public string GetUserID()
      {
        return UserID;
      }
      public void SetUserID(string ID)
      {
        UserID = ID;
      }

    }
}
