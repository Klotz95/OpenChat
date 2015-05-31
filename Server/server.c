#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <pthread.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <unistd.h>
#include <string.h>

int Clients[100];
struct sockaddr_in remotehost[100];
struct User
{
  char UserName[50];
  char UserID[50];
  char Comment[50];
  char ProfilePic[1000000];
};
struct User currentOnline[100];
void ClientHandling(int ClientValue)
{
  printf("CLient %d gets now his routine... \n",ClientValue);
  //send the ID
  sprintf(currentOnline[ClientValue].UserID,"%d",ClientValue);
  if(send(Clients[ClientValue],currentOnline[ClientValue].UserID,strlen(currentOnline[ClientValue].UserID),0) == -1)
  {
    printf("Cant send the ID... \n");
    return;
  }
  printf("ID send\n");
  //now get the information about the User
  char buffer[1000000];
  int checkCount;
  if((checkCount = recv(Clients[ClientValue],buffer,1000000,0)) == -1)
  {
    //there was a mistake
    printf("Can't get the User-Information...");
  }
  //now get the information
  printf("checkCount: %d \n",checkCount);
  char* currentSeperated;
  char trennzeichen[] = "{}";

  currentSeperated = strtok(buffer,trennzeichen);

  while(currentSeperated != NULL)
  {
    //get the current one
    int length = strlen(currentSeperated);
    char current[length];
    strcpy(current,currentSeperated);
    currentSeperated = strtok(NULL,trennzeichen);
    //now look which information is in the current bracket
    printf("current: %s \n",current);
    if(strncmp(current,"US",2) == 0)
    {
      //UserName
      char UserName[strlen(current) - 3];
      for(int k = 3; k< strlen(current);k++)
      {
        UserName[k-3] = current[k];
      }
      strcpy(currentOnline[ClientValue].UserName,UserName);
    }
    else if(strncmp(current,"CM",2) == 0)
    {
      //Comment
      char Comment[strlen(current)-4];
      for(int k = 3; k < strlen(current); k++)
      {
        Comment[k-3] = current[k];
      }
      strcpy(currentOnline[ClientValue].Comment,Comment);
    }
    else if(strncmp(current,"PP",2) == 0)
    {
      //Comment
      char PP[strlen(current)-3];
      for(int k = 3; k < strlen(current); k++)
      {
        PP[k-3] = current[k];
      }
      strcpy(currentOnline[ClientValue].ProfilePic,PP);
    }

  }
  //now send all informations about the current Online User
  for(int i = 0; i < 100; i++)
  {
    if(Clients[i] != NULL && i != ClientValue)
    {
      char MySelfInString[200] = {"{CN}{US:"};
      strcat(MySelfInString,currentOnline[i].UserName);
      strcat(MySelfInString,"}{CM:");
      strcat(MySelfInString,currentOnline[i].Comment);
      strcat(MySelfInString,"}{UI:");
      strcat(MySelfInString,currentOnline[i].UserID);
      strcat(MySelfInString,"}");
      if(strlen(currentOnline[i].ProfilePic) != 0)
      {
        strcat(MySelfInString,"{PP:");
        strcat(MySelfInString,currentOnline[i].ProfilePic);
        strcat(MySelfInString,"}");
      }
      strcat(MySelfInString,"\n");
      //now the message is ready to send
      if(send(Clients[ClientValue],MySelfInString,strlen(MySelfInString),0) == -1)
      {
        printf("Cant send the infomration about the current Online User \n");
      }
    }
  }
  //now inform everyone about the new User
  for(int i = 0; i < 100; i++)
  {
    if(Clients[i] != NULL && i != ClientValue)
    {
      //create myself string
      char MySelfInString[200] = {"{CN}{US:"};
      strcat(MySelfInString,currentOnline[ClientValue].UserName);
      strcat(MySelfInString,"}{CM:");
      strcat(MySelfInString,currentOnline[ClientValue].Comment);
      strcat(MySelfInString,"}{UI:");
      strcat(MySelfInString,currentOnline[ClientValue].UserID);
      if(strlen(currentOnline[ClientValue].ProfilePic) != 0)
      {
        strcat(MySelfInString,"{PP:");
        strcat(MySelfInString,currentOnline[ClientValue].ProfilePic);
        strcat(MySelfInString,"}");
      }
      strcat(MySelfInString,"}\n");
      if(send(Clients[i],MySelfInString,strlen(MySelfInString),0) == -1)
      {
        printf("Cant inform the Client %d about the new User... \n",i);
      }
    }
  }
  //now wait for a message of the Client
  bool stillConnected = true;
  while(stillConnected)
  {
    char NewReceivedMessage[1000000];
    printf("Waiting for message of Client %d \n",ClientValue);
    int count;
    if((count = recv(Clients[ClientValue],NewReceivedMessage,1000000,0)) == -1)
    {
      printf("Cant receive a messsage of Client %d",ClientValue);
    }
    else
    {

      char ReceivedMessage[count];
      strncpy(ReceivedMessage,NewReceivedMessage,count);
      printf("Send Message... \n");
      if(strncmp(ReceivedMessage,"<EOT>",5) == 0)
      {
        printf("Shutdown Socket %d \n",ClientValue);
        //inform everyone about the Disconnect
        char MyDCMessage[200] = {"{DC}"};
        strcat(MyDCMessage, currentOnline[ClientValue].UserID);
        strcat(MyDCMessage, "\n");
        for(int i = 0 ; i < 100; i++)
        {
          if(Clients[i] != NULL && i != ClientValue)
          {
            //send the message
            if(send(Clients[i],MyDCMessage,200,0) == -1)
            {
              printf("Cant send the message of the dc of Client %d to CLient %d",ClientValue,i);
            }
          }
        }
        stillConnected = false;
      }
      else
      {


        //get the Partner for the Receiver
        char* patnerID = strtok(NewReceivedMessage,"{}");
        char* plaintext = strtok(NULL,"");
        char PatnerIDinString[3];
        printf("Plaintext: %s \n",plaintext);
        strcpy(PatnerIDinString,patnerID);
        char sendableMessage[strlen(ReceivedMessage) + 10];
        strcpy(sendableMessage,"{");
        char MyIdInString[3];
        sprintf(MyIdInString,"%d",ClientValue);
        strcat(sendableMessage,MyIdInString);
        strcat(sendableMessage,"}{");
        strcat(sendableMessage,plaintext);
        strcat(sendableMessage,"}");
        strcat(sendableMessage,"\n");
        int PatnerValue = atoi(PatnerIDinString);
        //send the Message
        if(send(Clients[PatnerValue],sendableMessage,1000000,0) == -1)
        {
          printf("Cant send message from Client %d to Client %d \n",ClientValue,PatnerValue);
        }
        else
        {
          printf("Message send from Client %d to Client %d \n",ClientValue,PatnerValue);
        }
      }

    }
  }


}
void WaitForClient(void *ch)
{
  int mySocket = socket(AF_INET,SOCK_STREAM,0);
  if(mySocket == -1)
  {
    //mistake
    printf("Cant create a socket...\nShutdown programm...");
    return;
  }
  struct sockaddr_in mySelf;
  mySelf.sin_family = AF_INET;
  mySelf.sin_port = htons(2031);
  mySelf.sin_addr.s_addr = htonl(INADDR_ANY);
  if(bind(mySocket,(struct sockaddr *)&mySelf,sizeof(mySelf)) == -1)
  {
    //mistake
    printf("Can't bind socket...\nShutdown progamm...");
    return;

  }
  printf("Waiting for incomming connections...\n");
  //listen
  int CurrentValue = 2;
  while(CurrentValue < 100)
  {
    if(listen(mySocket,10) == -1)
    {
      printf("Cant listen to incomming request...\n" );
    }
    //accept the Clients
      socklen_t sin_size = sizeof(struct sockaddr_in);
      struct sockaddr_in currentSockIP;
      int currentSock = accept(mySocket,(struct sockaddr *) &currentSockIP,&sin_size);
      if(currentSock != -1)
      {
        //search a place for the new socket
        printf("new Client connected \n");
        for(int L  = 0; L < 100; L++)
        {
          if(Clients[L] == NULL)
          {
            Clients[L] = currentSock;
            remotehost[L] = currentSockIP;
            //create a client-Handling Thread
            pthread_t clientHandlingThread;
            pthread_create(&clientHandlingThread,NULL,ClientHandling,L);
            break;
          }
        }
      }



    }
  }


int main()
{
  //set the Clients to NULL
  for(int i = 0; i < 100; i++)
  {
    Clients[i] = NULL;
  }
  //make debugging values
  Clients[0] = 20;
  Clients[1] = 30;

  //create fake values
  strcpy(currentOnline[0].UserName,"Claudio C.");
  strcpy(currentOnline[0].Comment,"Hahha");
  strcpy(currentOnline[0].UserID,"0");
  strcpy(currentOnline[1].UserName,"Peter Fox");
  strcpy(currentOnline[1].Comment,"Alles neu");
  strcpy(currentOnline[1].UserID,"1");
  printf("Start Server...\n");
  //Start Wait for Client Thread
  pthread_t WaitingThread;
  pthread_create(&WaitingThread,NULL,WaitForClient,NULL);
  pthread_join(WaitingThread,NULL);
}
