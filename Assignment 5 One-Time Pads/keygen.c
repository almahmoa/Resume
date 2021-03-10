#include <stdio.h>
#include <stdlib.h>
#include <time.h>

/*
* This program creates a key file of specified length. The characters in the file
* generated will be any of the 27 allowed characters, generated using the standard
* Unix randomization methods.
*/
int main(int argc, char* argv[]) {
    //if argv > 2 return stderr prompt
    // also if argv is not a int value
    int key_length = atoi(argv[1]);
    int i;
    char c;
    srand(time(0));
    for (int i = 0; i < key_length; i++)
    {
        int i = rand() % ('Z' - 'A' + 1) + 'A';
        c = i;
        fprintf(stdout, "%c", c);
    }
    fprintf(stdout, "\n");
    fflush(stdout);
    return 0;
}