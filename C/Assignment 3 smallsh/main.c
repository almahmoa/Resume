#include <ctype.h>
#include <errno.h>
#include <fcntl.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <time.h>
#include <unistd.h>

/*
*   Process the file provided as an argument to the program to
*   create a linked list of movie structs and print out the list.
*   Compile the program as follows:
*       gcc --std=gnu99 -o smallsh main.c
*/

/*
* handle signal arguemnts
*/
int statusCmd(char** args);
int STATUS = 0;
int SIGSTATUS = 0;
int PIDINDEX = 0;
int CHILDPID[1024];
int FGM = 0; //FOREGROUND MODE
void handleSignal(int sig)
{
    SIGSTATUS = sig;
    char** args;
    fflush(stdout);
    switch (sig)
    {
    case 2: //SIGINT
        printf("terminated by signal 2\n"); // DOESN"T WORK
        fflush(stdout);
        //wait(NULL);
        kill(CHILDPID[PIDINDEX], SIGKILL);
        break;
    case 20: //SIGTSTP
        if (FGM == 0)
        {
            FGM = 1;
            printf("\nEntering foreground-only mode (& is now ignored)\n");
            fflush(stdout);
            printf("> ");
            fflush(stdout);
        }
        else {
            FGM = 0;
            printf("\nExiting foreground-only mode\n");
            fflush(stdout);
            printf("> ");
            fflush(stdout);
        }
        break;
    case 15: //SIGTERM
        PIDINDEX--;
        if (CHILDPID[PIDINDEX] != 0)
        {
            wait(NULL);
            printf("background pid %d is done: ", CHILDPID[PIDINDEX]);
            fflush(stdout);
            statusCmd(args);
            if (PIDINDEX > 0)
            {
                PIDINDEX--;
            }
        }
        exit(0);
        break;
    default: //SIGCHLD 17
        wait(NULL);
        PIDINDEX--;
        if (CHILDPID[PIDINDEX] != 0)
        {
            printf("background pid %d is done: ", CHILDPID[PIDINDEX]);
            fflush(stdout);
            if (PIDINDEX > 0)
            {
                PIDINDEX--;
            }
            if (PIDINDEX == 0)
            {
                CHILDPID[0] = 0;
            }
            STATUS = 0;
            statusCmd(args);
            printf("> ");
            fflush(stdout);
        }
        break;
    }
}

/*
* navigate directories
*/
int cdCmd(char** args)
{
    char* path;
    if (args[1] == NULL) {
        //go back to the HOME environment variable
        chdir(getenv("PWD"));
    }
    else {
        //check for relative and absolute path
        path = args[1];
        int csignstatus = chdir(args[1]);
        if (csignstatus != 0)
        {
            printf("chdir() to %s failed\n", args[1]);
            fflush(stdout);
            return 1;
        }
    }
    return 0;
}

/*
* check the return status of the previous argument
*/
int statusCmd(char** args)
{
    if (SIGSTATUS == 0 || SIGSTATUS == 17)
    {
        printf("exit value %d\n", STATUS);
    }
    else
    {
        printf("terminated by signal %d\n", SIGSTATUS);
    }
    return 0;
}

/*
* leave the shell
*/
int exitCmd(char** args)
{
    //When this command is run, your shell must kill any other processes or jobs that your shell has started before it terminates itself.
    raise(SIGTERM);
    exit(0);
}

/*
* run the arguments through the process
*/
int BACKGROUND = 0;
int KILLCMD = 0;
int processes(char** args, int in, int out)
{
    SIGSTATUS = 0;
    int devNull;
    int status;
    pid_t spawnPid = fork();
    switch (spawnPid)
    {
    case -1:
        printf("fork failed\n");
        fflush(stdout);
        exit(1);
        break;
    case 0:
        if (BACKGROUND == 1 && FGM != 1)
        {
            //child process ignore the SIGINT signal
            signal(SIGINT, SIG_IGN);
        }

        if (in) // in redirection
        {
            int fdin = open(args[in], O_RDONLY);
            if (fdin == -1 && args[in] != NULL)
            {
                printf("cannot open %s for input\n", args[in]);
                fflush(stdout);
                break;
            }
            else if (args[in] == NULL) //if redirection is pointed to null
            {
                devNull = open("/dev/null", O_WRONLY);
                dup2(devNull, 0);
                close(devNull);
            }
            else
            {
                dup2(fdin, 0);
                close(fdin);
            }
            args[in] = NULL;
        }

        if (out) //out redirection
        {
            int fdout = open(args[out], O_WRONLY | O_CREAT | O_TRUNC, 0644);
            if (fdout == -1 && args[out] != NULL)
            {
                printf("cannot open %s for input\n", args[out]);
                fflush(stdout);
                break;
            }
            else if (args[out] == NULL) //if redirection is pointed to null
            {
                devNull = open("/dev/null", O_WRONLY);
                dup2(devNull, 0);
                close(devNull);
            }
            else
            {
                dup2(fdout, 1);
                close(fdout);
            }
            args[out] = NULL;
        }
        execvp(args[0], args);
        printf("%s: no such file or directory\n", *args);
        fflush(stdout);
        break;
    default:
        signal(SIGINT, SIG_IGN);
        CHILDPID[PIDINDEX] = spawnPid;
        PIDINDEX++;

        if (BACKGROUND == 1 && FGM != 1)
        {
            // parent process does not wait for child to terminate
            printf("background pid is %d\n", spawnPid);
            fflush(stdout);
            spawnPid = waitpid(spawnPid, &status, WNOHANG);
        }
        else
        {
            // parent process waits for child to terminate
            signal(SIGCHLD, SIG_DFL);
            PIDINDEX--;
            if (KILLCMD == 1)
            {
                PIDINDEX = 0; // clean pid index
            }
            signal(SIGINT, handleSignal);
            spawnPid = waitpid(spawnPid, &status, 0);
        }
        break;
    }
    //this returns exit status 0 for exiting normally, 1 otherwise
    return WEXITSTATUS(status);
}

char* builtinCmd[] = {
  "cd", "status", "exit"
};

int (*builtinFunction[]) (char**) = {
  &cdCmd, &statusCmd, &exitCmd
};

/*
* check if the first token is a built in argument, pass along if otherwise
*/
int execute(char** args, int in, int out)
{
    int i;
    int builtinLength = sizeof builtinCmd / sizeof * builtinCmd;
    // checks if the command was empty or a comment.
    if (args[0] == NULL || strcmp(args[0], "#") == 0) {
        return 0;
    }
    // check if first argument is a builtin function
    for (i = 0; i < builtinLength; i++)
    {
        if (strcmp(args[0], builtinCmd[i]) == 0)
        {
            return (*builtinFunction[i])(args);
        }
    }
    return processes(args, in, out);
}


/*
* Go through the line, converting them into token for arguments
*/
#define BUFSIZE 64
#define DELIM " \n"
int processLine(char* line)
{
    int bufsize = BUFSIZE, i = 0;
    char** tokens = malloc(bufsize * sizeof(char*));
    char* token, ** ttokens, * stoken; // temp tokens, and second token
    int in = 0;
    int out = 0;
    int length;
    char* signs = "$";
    int expandCount = 0;
    char processID[10];
    sprintf(processID, "%d", getpid());

    if (!tokens) {
        printf("allocation error\n");
        fflush(stdout);
        exit(1);
    }

    token = strtok(line, DELIM);
    while (token != NULL)
    {
        expandCount = 0;
        char ttoken[1024] = "";
        //CHECK ALL CHAR FOR TOKEN - for expansion
        length = (int)strlen(token);
        for (int j = 0; j < length + 1; j++)
        {
            if (token[j] == signs[0])
            {
                expandCount++;
            }
            else
            {
                if (expandCount == 1)
                {
                    sprintf(ttoken, "%s%c", ttoken, signs[0]);
                    expandCount--;
                }
                if (strcmp(ttoken, "") == 0)
                {
                    sprintf(ttoken, "%c", token[j]);
                }
                else
                {
                    sprintf(ttoken, "%s%c", ttoken, token[j]);
                }
            }
            if (expandCount == 2)
            {
                sprintf(ttoken, "%s%s", ttoken, processID);
                expandCount = 0;
            }
        }

        stoken = calloc(strlen(ttoken) + 1, sizeof(char));
        strcpy(stoken, ttoken);
        tokens[i] = stoken;

        //set redirection
        if (strcmp(token, "<") == 0)
        {
            in = i;
            i--;
        }
        else if (strcmp(token, ">") == 0)
        {
            out = i;
            i--;
        }
        i++;

        //if i exceesigns the max size of tokens
        if (i >= bufsize)
        {
            bufsize += BUFSIZE;
            ttokens = tokens;
            tokens = realloc(tokens, bufsize * sizeof(char*));
            if (!tokens)
            {
                free(ttokens);
                printf("allocation error\n");
                fflush(stdout);
                exit(1);
            }
        }
        token = strtok(NULL, DELIM);
    }
    //If the command is to be executed in the background, the last word must be &. 
    if (strcmp(tokens[i - 1], "&") == 0)
    {
        BACKGROUND = 1;
        tokens[i - 1] = NULL;
    }
    else //If the & character appears anywhere else, just treat it as normal text.
    {
        BACKGROUND = 0;
        tokens[i] = NULL;
    }

    //check if first token is a kill command
    if (strcmp(tokens[0], "pkill") == 0 || strcmp(tokens[0], "kill") == 0 ||
        strcmp(tokens[0], "killall") == 0)
    {
        KILLCMD = 1;
    }
    else
    {
        KILLCMD = 0;
    }

    return execute(tokens, in, out);
}

/*
* read each line of the argument
*/
char* readLine(void)
{
    char* line = NULL;
    ssize_t bufsize = 0;
    if (getline(&line, &bufsize, stdin) == -1) {
        if (feof(stdin)) {
            exit(0);  // We recieved an EOF
        }
        else {
            perror("ERROR: getline\n");
            exit(1);
        }
    }
    return line;
}

int main(int argc, char** argv) {
    signal(SIGTSTP, handleSignal);
    //shell loop
    while (1)
    {
        struct sigaction SIGTERM_action = { 0 };
        // Fill out the SIGTERM_action struct
        // Register handle_SIGTERM as the signal handler
        SIGTERM_action.sa_handler = handleSignal;
        // Block all catchable signals while handle_SIGTERM is running
        sigfillset(&SIGTERM_action.sa_mask);
        // No flags set
        SIGTERM_action.sa_flags = 0;
        sigaction(SIGTERM, &SIGTERM_action, NULL);

        signal(SIGCHLD, handleSignal);
        signal(SIGINT, SIG_IGN);
        char* line;
        int status;

        printf("> ");
        line = readLine();
        status = processLine(line);

        free(line);
        if (status >= 1)
        {
            STATUS = 1;
        }
        else
        {
            STATUS = 0;
        }

        int i = -1;
        do {
            waitpid(-1, NULL, WNOHANG);
            i++;
        } while (CHILDPID[i] != 0);
    }
}