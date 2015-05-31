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

int main()
{
  int sockFD = socket(AF_INET,SOCK_STREAM,0);
  struct sockaddr_in serverIP;
  serverIP.sin_family = AF_INET;
  serverIP.sin_port =htons(2031);
  serverIP.sin_addr.s_addr = inet_addr("192.168.1.118");

  connect(sockFD,(struct sockaddr *)&serverIP,sizeof(serverIP));
}
