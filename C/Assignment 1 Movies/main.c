#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>

/* struct for movie information */
struct movie
{
    char* title;
    int year;
    char* language[6];
    double rating;
    struct movie* next;
};

/* Parse the current line which is comma delimited and create a
*  movie struct with the data in this line
*/
struct movie* createMovie(char* currLine)
{
    struct movie* currMovie = malloc(sizeof(struct movie));

    // For use with strtok_r
    char* temp;
    char* ptr;
    char* savePtr1;
    char* savePtr2;

    // The first token is the title
    char* token = strtok_r(currLine, ",", &savePtr1);
    currMovie->title = calloc(strlen(token) + 1, sizeof(char));
    strcpy(currMovie->title, token);

    // The next token is the year
    token = strtok_r(NULL, ",", &savePtr1);
    temp = calloc(strlen(token) + 1, sizeof(char));
    strcpy(temp, token);
    currMovie->year = atoi(temp);

    // The next token is the language, broken into more tokens for the array
    token = strtok_r(NULL, ",", &savePtr1);
    temp = calloc(strlen(token) + 1, sizeof(char));
    strcpy(temp, token);
    token = strtok_r(temp, "[];", &savePtr2);
    int i = 0;
    while (token != NULL)
    {
        currMovie->language[i] = calloc(strlen(token) + 1, sizeof(char));
        strcpy(currMovie->language[i], token);
        i++;
        token = strtok_r(NULL, "[];", &savePtr2);
    }

    // The last token is the rating
    token = strtok_r(NULL, ",", &savePtr1);
    temp = calloc(strlen(token) + 1, sizeof(char));
    strcpy(temp, token);
    currMovie->rating = strtod(temp, &ptr);

    // Set the next node to NULL in the newly created student entry
    currMovie->next = NULL;

    return currMovie;
}

/*
* Return a linked list of movies by parsing data from
* each line of the specified file.
*/
struct movie* processFile(char* filePath)
{
    // Open the specified file for reading only
    FILE* movieFile = fopen(filePath, "r");
    fscanf(movieFile, "%*[^\n]\n");
    char* currLine = NULL;
    size_t len = 0;
    ssize_t nread;
    char* token;
    int num = 0;

    // The head of the linked list
    struct movie* head = NULL;
    // The tail of the linked list
    struct movie* tail = NULL;
    // The temp linked list for storing nodes
    struct movie* temp = NULL;

    // Read the file line by line
    while ((nread = getline(&currLine, &len, movieFile)) != -1)
    {
        // Get a new movie node corresponding to the current line
        struct movie* newNode = createMovie(currLine);

        temp = newNode;
        temp->next = NULL;
        //If linked list is empty
        if (head == NULL)
        {
            head = temp;
            tail = head;
        }
        else
        {
            while (tail->next != NULL) {
                tail = tail->next;
            }
            tail->next = temp;
        }
        num++;
    }
    printf("Processed file %s and parsed data for %d movies\n", filePath, num);
    free(currLine);
    fclose(movieFile);
    return head;
}

/*
* Print the linked list of movies based on the year inputed
*/
void printMovieYearList(struct movie* list, int numInput)
{
    int checker = 0;

    while (list != NULL)
    {
        if (list->year == numInput)
        {
            printf("%s\n", list->title);
            checker = 1;
        }
        list = list->next;
    }

    if (checker == 0)
    {
        printf("No data about movies released in the year %d\n", numInput);
    }

    return;
}

/*
* Run through the list multiple times, comparing and printing the highest rating per year
*/
void movieRatingRecursion(struct movie* list, int year)
{
    char* title;
    double rating = 0.0;

    while (list != NULL)
    {
        if (list->year == year)
        {
            if (list->rating > rating)
            {
                title = calloc(strlen(list->title) + 1, sizeof(char));
                strcpy(title, list->title);
                rating = list->rating;
            }
        }
        list = list->next;
    }
    if (rating != 0.0)
    {
        printf("%d %g %s\n", year, rating, title);
    }

    return;
}

/*
* compare nodes of the linked list for the highest rated movies per year
*/
void printMovieRatingList(struct movie* list)
{
    int year = 1990;
    while (year <= 2021)
    {
        movieRatingRecursion(list, year);
        year++;
    }
    return;
}

// compare two undercased strings
int strcmp(char const* a, char const* b)
{
    int d = tolower((unsigned char)*a) - tolower((unsigned char)*b);
    return d;
}

/*
* Print the linked list of movies based on language available
*/
void printMovieLanguageList(struct movie* list, char* strInput)
{
    int checker = 0;

    while (list != NULL)
    {
        int i = 0;
        while (list->language[i] != NULL)
        {
            if (strcmp(list->language[i], strInput) == 0)
            {
                printf("%d %s\n", list->year, list->title);
                checker = 1;
                break;
            }
            i++;
        }
        list = list->next;
    }

    if (checker == 0)
    {
        printf("No data about movies released in %s\n", strInput);
    }

    return;
}

/*
* Interactive Functionality: outputs and read inputs for users
*/
void interactiveFunctionality(struct movie* list)
{
    int numInput = 0;
    char strInput[20];
    int runInterface = 1;
    while (runInterface == 1)
    {
        printf("\n1. Show movies released in the specified year\n2. Show highest rated movie for each year\n3. Show the title and year of release of all movies in a specific language\n4. Exit from the program\n");

        printf("\nEnter a choice from 1 to 4: ");
        scanf("%d", &numInput);
        if (numInput < 1 || numInput > 4)
        {
            printf("\nYou entered an incorrect choice. Try again.\n");
        }
        else
        {
            switch (numInput)
            {
            case 1:
                printf("Enter the year for which you want to see movies: ");
                scanf("%d", &numInput);
                printMovieYearList(list, numInput);
                break;
            case 2:
                printMovieRatingList(list);
                break;
            case 3:
                printf("Enter the language for which you want to see movies: ");
                scanf("%s", strInput);
                printMovieLanguageList(list, strInput);
                break;
            case 4:
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
*       gcc --std=gnu99 -o movies main.c
*       ./movies movies_sample_1.csv
*/

int main(int argc, char* argv[])
{
    if (argc < 2)
    {
        printf("You must provide the name of the file to process\n");
        printf("Example usage: ./movies movies_sample_1.csv\n");
        return EXIT_FAILURE;
    }
    struct movie* list = processFile(argv[1]);
    interactiveFunctionality(list);
    return EXIT_SUCCESS;
}