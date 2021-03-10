#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <pthread.h> // must link with -pthread
#include <unistd.h>

/*
A program with a pipeline of 4 threads that interact with each other as producers and consumers.
- Input thread is the first thread in the pipeline. It gets input from the user and puts it in a buffer it shares with the next thread in the pipeline.
- Line separator thread is the second thread in the pipeline. It consumes items from the buffer it shares with the input thread. It changes all new line inputs to space characters in the string. It puts the new string in a buffer it shares with the next thread in the pipeline. Thus this thread implements both consumer and producer functionalities.
- Plus sign thread is the third thread in the pipeline. It consumes items from the buffer it shares with the line separator thread. It changes all ++ to ^ characters in the string. It puts the new string in a buffer it shares with the next thread in the pipeline. Thus this thread implements both consumer and producer functionalities.
- Output thread is the forth thread in the pipeline. It consumes items from the buffer it shares with the plus sign thread and prints the items.
Citation: Assignment 4 - example program
*/

// Special marker used to indicate end of the producer data
int END_MARKER = -1;

// Size of the buffers
#define SIZE 50

// Number of items that will be produced. This number is less than the size of the buffer. Hence, we can model the buffer as being unbounded.
int NUM_ITEMS = 1;

// Number of characters each line of array can hold
#define NUM_CHAR 1000

// Buffer 1, shared resource between input thread and . . .
char buffer_1[NUM_CHAR] = "";
// Number of items in the buffer
int count_1 = 0;
// Initialize the mutex for buffer 1
pthread_mutex_t mutex_1 = PTHREAD_MUTEX_INITIALIZER;
// Initialize the condition variable for buffer 1
pthread_cond_t full_1 = PTHREAD_COND_INITIALIZER;

// Buffer 2, shared resource between . . .
char buffer_2[NUM_CHAR] = "";
// Number of items in the buffer
int count_2 = 0;
// Initialize the mutex for buffer 2
pthread_mutex_t mutex_2 = PTHREAD_MUTEX_INITIALIZER;
// Initialize the condition variable for buffer 2
pthread_cond_t full_2 = PTHREAD_COND_INITIALIZER;

// Buffer 3, shared resource between square root thread and output thread
char buffer_3[NUM_CHAR] = "";
// Number of items in the buffer
int count_3 = 0;
// Initialize the mutex for buffer 3
pthread_mutex_t mutex_3 = PTHREAD_MUTEX_INITIALIZER;
// Initialize the condition variable for buffer 3
pthread_cond_t full_3 = PTHREAD_COND_INITIALIZER;
//output counter to push every 80 characters
int OUT_COUNT = 0;

int manual_input = 0;

char* stop_processing_line(char* line)
{
    char* new_line = malloc(NUM_CHAR * sizeof(char*));
    int length = (int)strlen(line);
    for (int i = 0; i < length + 1; i++)
    {
        //checks if the condition to end the program has been flagged
        if (line[i] == 'S' && line[i + 1] == 'T' && line[i + 2] == 'O' && line[i + 3] == 'P' && line[i + 4] == '\n' && (line[i - 1] == ' ' || line[i - 1] == '\0'))
        {
            END_MARKER = 0;
            return new_line;
        }
        sprintf(new_line, "%s%c", new_line, line[i]);
    }
    return new_line;
}

/*
 Put an item in buff_1
*/
void put_buff_1(char* line) {
    // Lock the mutex before putting the string in the buffer
    pthread_mutex_lock(&mutex_1);
    // Put the string in the buffer
    sprintf(buffer_1, "%s", line);
    count_1++;
    // Signal to the consumer that the buffer is no longer empty
    pthread_cond_signal(&full_1);
    // Unlock the mutex
    pthread_mutex_unlock(&mutex_1);
}

/*
 Get input from the user.
 read each line of the argument
*/
char* get_user_input()
{
    char* line = malloc(NUM_CHAR * sizeof(char*));
    char* currLine = NULL;
    size_t bufsize = 0;

    if (manual_input == 0) //input was from a redirection
    {
        while ((getline(&currLine, &bufsize, stdin)) != -1 && END_MARKER == -1)
        {
            stop_processing_line(currLine);
            sprintf(line, "%s%s", line, currLine);
        }
    }
    else //inputs from teh keyboard
    {
        if ((getline(&currLine, &bufsize, stdin)) != -1 && END_MARKER == -1)
        {
            stop_processing_line(currLine);
            sprintf(line, "%s%s", line, currLine);
        }
    }
    free(currLine);

    return line;
}

/*
 Function that the input thread will run.
 Get input from the user.
 Put the item in the buffer shared with the square_root thread.
*/
void* get_input(void* args)
{
    char* line = malloc(NUM_CHAR * sizeof(char*));
    for (int i = 0; i < NUM_ITEMS; i++)
    {
        // Get the user input
        line = get_user_input();
        put_buff_1(line);
    }
    return NULL;
}

/*
Get the next item from buffer 1
*/
char* get_buff_1() {
    // Lock the mutex before checking if the buffer has data
    pthread_mutex_lock(&mutex_1);
    while (count_1 == 0)
    {
        // Buffer is empty. Wait for the producer to signal that the buffer has data
        pthread_cond_wait(&full_1, &mutex_1);
    }

    char* line = malloc(NUM_CHAR * sizeof(char*));
    sprintf(line, "%s", buffer_1);
    count_1--;
    // Unlock the mutex
    pthread_mutex_unlock(&mutex_1);
    // Return the line
    return line;
}

/*
 Put an item in buff_2
*/
void put_buff_2(char* line) {
    // Lock the mutex before putting the item in the buffer
    pthread_mutex_lock(&mutex_2);
    // Put the item in the buffer
  //printf("put_buff_2: %s\n", buffer_2);
    sprintf(buffer_2, "%s", line);
    //printf("2 put_buff_2: %s\n", buffer_2);
    count_2++;
    // Signal to the consumer that the buffer is no longer empty
    pthread_cond_signal(&full_2);
    // Unlock the mutex
    pthread_mutex_unlock(&mutex_2);
}

/*
 line separator
*/
char* line_separator_input(char* line)
{
    int length = (int)strlen(line);
    for (int i = 0; i < length + 1; i++)
    {
        if (line[i] == '\n')
        {
            line[i] = ' ';
        }
    }
    return line;
}

/*
 Function that replaces every line separator in the input by a space.
*/
void* line_separator(void* args)
{
    char* line = malloc(NUM_CHAR * sizeof(char*));
    char* new_line = malloc(NUM_CHAR * sizeof(char*));
    for (int i = 0; i < NUM_ITEMS; i++)
    {
        line = get_buff_1();
        new_line = line_separator_input(line);
        put_buff_2(new_line);
    }
    return NULL;
}


/*
Get the next item from buffer 2
*/
char* get_buff_2() {
    // Lock the mutex before checking if the buffer has data
    pthread_mutex_lock(&mutex_2);
    while (count_2 == 0)
    {
        // Buffer is empty. Wait for the producer to signal that the buffer has data
        pthread_cond_wait(&full_2, &mutex_2);
    }

    char* line = malloc(NUM_CHAR * sizeof(char*));
    sprintf(line, "%s", buffer_2);
    count_2--;
    // Unlock the mutex
    pthread_mutex_unlock(&mutex_2);
    // Return the line
    return line;
}

/*
 Put an item in buff_3
*/
void put_buff_3(char* line) {
    // Lock the mutex before putting the item in the buffer
    pthread_mutex_lock(&mutex_3);
    // Put the item in the buffer
    sprintf(buffer_3, "%s%s", buffer_3, line);
    count_3++;
    // Signal to the consumer that the buffer is no longer empty
    pthread_cond_signal(&full_3);
    // Unlock the mutex
    pthread_mutex_unlock(&mutex_3);
}

char* plus_sign_input(char* line)
{
    char* new_line = malloc(NUM_CHAR * sizeof(char*));
    int length = (int)strlen(line);
    for (int i = 0; i < length + 1; i++)
    {
        if (line[i] == '+' && line[i + 1] == '+')
        {
            line[i] = '^';
            sprintf(new_line, "%s%c", new_line, line[i]);
            i++;
        }
        else
        {
            sprintf(new_line, "%s%c", new_line, line[i]);
        }
    }
    return new_line;
}

/*
 Function that replaces every pair of plus signs, i.e., "++", by a "^"
*/
void* plus_sign(void* args)
{
    char* line = malloc(NUM_CHAR * sizeof(char*));
    char* new_line = malloc(NUM_CHAR * sizeof(char*));
    for (int i = 0; i < NUM_ITEMS; i++)
    {
        line = get_buff_2();
        new_line = plus_sign_input(line);
        put_buff_3(new_line);
    }
    return NULL;
}

/*
Get the next item from buffer 3
*/
char* get_buff_3() {
    // Lock the mutex before checking if the buffer has data
    pthread_mutex_lock(&mutex_3);
    while (count_3 == 0)
    {
        // Buffer is empty. Wait for the producer to signal that the buffer has data
        pthread_cond_wait(&full_3, &mutex_3);
    }

    char* line = malloc(NUM_CHAR * sizeof(char*));
    sprintf(line, "%s", buffer_3);
    count_3--;
    // Unlock the mutex
    pthread_mutex_unlock(&mutex_3);
    // Return the line
    return line;
}

void write_output_characters(char* line)
{
    int length = (int)strlen(line);
    while (length > OUT_COUNT + 80)
    {
        for (int i = 0; i < 79 + 1; i++)
        {
            fprintf(stdout, "%c", line[OUT_COUNT + i]);
            fflush(stdout);
        }
        fprintf(stdout, "\n");
        fflush(stdout);
        OUT_COUNT = OUT_COUNT + 80;
    }
}

/*
  write this processed data to standard output as lines of exactly 80 characters
*/
void* write_output(void* args)
{
    char* line = malloc(NUM_CHAR * sizeof(char*));
    for (int i = 0; i < NUM_ITEMS; i++)
    {
        line = get_buff_3();
        write_output_characters(line);
        if (END_MARKER == 0)
            exit(0);
    }
    return NULL;
}

/*
*   Process the file provided as an argument to the program to
*   create a linked list of movie structs and print out the list.
*   Compile the program as follows:
*       gcc -std=gnu99 -pthread -o line_processor main.c
*       ./line_processor
*/
int main(int argc, char* argv[])
{
    //check if redirection was used
    if (isatty(0))
    {
        NUM_ITEMS = 50;
        manual_input = 1;
    }
    srand(time(0));
    pthread_t input_t, line_separator_t, plus_sign_t, output_t;
    // Create the threads
    pthread_create(&input_t, NULL, get_input, NULL);
    pthread_create(&line_separator_t, NULL, line_separator, NULL);
    pthread_create(&plus_sign_t, NULL, plus_sign, NULL);
    pthread_create(&output_t, NULL, write_output, NULL);
    // Wait for the threads to terminate
    pthread_join(input_t, NULL);
    pthread_join(line_separator_t, NULL);
    pthread_join(plus_sign_t, NULL);
    pthread_join(output_t, NULL);

    return EXIT_SUCCESS;
}