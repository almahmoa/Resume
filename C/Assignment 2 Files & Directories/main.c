#include <fcntl.h>
#include <dirent.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <time.h>
#include <unistd.h>

#define PREFIX "movies_"

/* Create a file (if one does not already exist) based on the year of the movie
* write the movie's name on a new line (if it is not the first entry)
*/
void generateFile(char* title, char* year, char* dirName)
{
    int fd;
    char filePath[50];
    sprintf(filePath, "./%s/%s.txt", dirName, year);
    FILE* fp = NULL;
    fd = open(filePath, O_CREAT | O_RDWR, 0640);
    if (fd == -1) {
        printf("open() failed on \"%s\"\n", filePath);
        perror("Error");
        exit(1);
    }
    fp = fopen(filePath, "r");
    fseek(fp, 0, SEEK_END);
    int  size = ftell(fp);
    //check if file is empty
    if (0 >= size) {
        write(fd, title, strlen(title));
    }
    else
    {
        lseek(fd, 0, SEEK_END);
        write(fd, "\n", strlen("\n"));
        write(fd, title, strlen(title));
    }
    fclose(fp);
}

/* Make Directory
* Parsing data of  movie from each line of the specified file.
*/
void processFile(char* filePath)
{
    /*  Omitted
    *   Variable set-up
    */

    printf("Now processing the chosen file named %s\n", filePath);

    //Make Directory
    /* Intializes random number generator */
    srand(time(0));
    char dirName[23] = "almahmoa.movies.";
    char ranNum[6];
    sprintf(ranNum, "%d", rand() % 100000);
    strcat(dirName, ranNum);
    mkdir(dirName, 0750);
    printf("Created directory with name %s\n", dirName);

    // Read the file line by line
    while ((nread = getline(&currLine, &len, movieFile)) != -1)
    {
        char* savePtr;
        // Get a new movie node corresponding to the current line
        char* token = strtok_r(currLine, ",", &savePtr);
        title = calloc(strlen(token) + 1, sizeof(char));
        strcpy(title, token);
        
        /* Omitted
        *  // The next token is the year
        *   . . .
        */

        //printf("%s %s\n", title, year);
        generateFile(title, year, dirName);
    }
    free(currLine);
    fclose(movieFile);
}

void menuOne();

void menuTwo()
{
    // Open the current directory
    DIR* currDir = opendir(".");
    struct dirent* aDir;
    char entryName[256];
    char largestEntry[256] = "";
    char smallestEntry[256] = "";
    int fd;
    int sizeCmpL = 0;
    int sizeCmpS = 0;

    // Go through all the entries
    while ((aDir = readdir(currDir)) != NULL)
    {
        if (strncmp(PREFIX, aDir->d_name, strlen(PREFIX)) == 0)
        {
            // Get meta-data for the current entry
            strcpy(entryName, aDir->d_name);
            const char* ext = strrchr(entryName, '.');
            if (!ext) {
                /* no extension */
            }
            else if (strcmp(ext + 1, "csv") == 0)
            {
                fd = open(entryName, O_RDONLY);
                if (fd == -1) {
                    printf("open() failed on \"%s\"\n", entryName);
                    perror("Error");
                    exit(1);
                }
                // We allocate a buffer to read from the file
                char* readBuffer = malloc(100000 * sizeof(char));
                lseek(fd, 0, SEEK_SET);
                int howMany = read(fd, readBuffer, 100000);
                if (sizeCmpL < howMany) //check if file is larger
                {
                    sizeCmpL = howMany;
                    strcpy(largestEntry, entryName);
                }
                if (sizeCmpS > howMany || sizeCmpS == 0) //check if file is smaller
                {
                    sizeCmpS = howMany;
                    strcpy(smallestEntry, entryName);
                }
            }
        }
    }
    // Close the directory
    closedir(currDir);

    int numInput = 0;
    char strInput[256];
    bool checker = false;
    bool runInterface = true;

    while (runInterface)
    {
        printf("\nWhich file you want to process?\nEnter 1 to pick the largest file\nEnter 2 to pick the smallest file\nEnter 3 to specify the name of a file\n");

        printf("\nEnter a choice from 1 to 3: ");
        scanf("%d", &numInput);
        if (numInput < 1 || numInput > 3)
        {
            printf("\nYou entered an incorrect choice. Try again.\n");
        }
        else
        {
            /*  Omitted
            *   Switch statment that check the numInput
            *   case 1: //largest file
            *   . . .
            *   case 2: //smallest file
            *   . . .
            *   case 3 // asked for an input for the complete file name
            */
                closedir(currDir);
                break;
            }
        }
    }
    menuOne();
}

void menuOne()
{
    int numInput = 0;
    bool runInterface = true;
    while (runInterface)
    {
        printf("\n1. Select file to process\n2. Exit the program\n");

        printf("\nEnter a choice 1 or 2: ");
        scanf("%d", &numInput);
        if (numInput < 1 || numInput > 2)
        {
            printf("\nYou entered an incorrect choice. Try again.\n");
        }
        else
        {
            switch (numInput)
            {
            case 1:
                menuTwo();
                break;
            case 2:
                exit(0);
                break;
            }
        }
    }
}

/*
*   Process the file provided as an argument to the program to
*   create a linked list of movie structs and print out the list.
*   Compile the program as follows:
*       gcc --std=gnu99 -o movies_by_year main.c
*       ./movies_by_year
*/
int main(int argc, char* argv[])
{
    if (argc < 1)
    {
        printf("You must provide the name of the file to process\n");
        printf("Example usage: ./movies_by_year\n");
        return EXIT_FAILURE;
    }
    menuOne();
    return EXIT_SUCCESS;
}
