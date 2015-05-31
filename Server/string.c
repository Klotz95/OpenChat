#include <stdio.h>
#include <stdlib.h>
#include <string.h>
int main()
{
  char BeispielUser[] = "{CN}{US:Nico Kotlenga}{CM:Hallo Welt}";
  char *currentSeperated;
  char trennzeichen[] = "{}";

  currentSeperated = strtok(BeispielUser,trennzeichen);

  while(currentSeperated != NULL)
  {
    //get the current one
    int length = strlen(currentSeperated);
    char current[length];
    strncpy(current,currentSeperated,20);
    currentSeperated = strtok(NULL,trennzeichen);
    printf("%s \n",current);
    //now look which information is in the current bracket
    if(strncmp(current,"US",2) == 0)
    {
      //UserName
      printf("Found UserName... \n");
      char UserName[strlen(current) - 3];
      for(int k = 3; k< strlen(current);k++)
      {
        UserName[k-3] = current[k];
      }
      printf("%s \n",UserName);
    }
    else if(strncmp(current,"CM",2) == 0)
    {
      //Comment
      printf("Found comment...\n");
      char Comment[strlen(current)-4];
      for(int k = 3; k < strlen(current); k++)
      {
        Comment[k-3] = current[k];
      }
      printf("%s \n",Comment);
    }
  }
}
