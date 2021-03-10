#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>  // ssize_t
#include <sys/socket.h> // send(),recv()
#include <netdb.h>      // gethostbyname()

/**
* Client code
* 1. Create a socket and connect to the server specified in the command arugments.
* 2. Prompt the user for input and send that input as a message to the server.
* 3. Print the message received from the server and exit the program.
*/

// Error function used for reporting issues
void error(const char* msg) {
    perror(msg);
    exit(0);
}

// Set up the address struct
void setupAddressStruct(struct sockaddr_in* address,
    int portNumber) {

    // Clear out the address struct
    memset((char*)address, '\0', sizeof(*address));

    // The address should be network capable
    address->sin_family = AF_INET;
    // Store the port number
    address->sin_port = htons(portNumber);
    
}

//Code cite: https://stackoverflow.com/questions/35485438/partial-send-receive-tcp-sockets-c
int sendall(int s, char* buf, int* len)
{
    int total = 0;        // how many bytes we've sent
    int bytesleft = *len; // how many we have left to send
    int n, send_amount;

    while (total < *len) {
        // send in max chunks of 1000 
        if (bytesleft > 1000)
            send_amount = 1000;
        else
            send_amount = bytesleft;
        n = send(s, buf + total, send_amount, 0);
        if (n == -1) { break; }
        total += n;
        bytesleft -= n;
    }

    return n == -1 ? -1 : 0; // return -1 on failure, 0 on success
}

int recvall(int s, char* buffer, size_t size)
{
    size_t received = 0;
    while (received < size)
    {
        ssize_t r = recv(s, buffer + received, size - received, 0);
        if (r <= 0) break;
        received += r;
    }

    return 0;
}

int main(int argc, char* argv[]) {
    int socketFD, socketFD2, portNumber, charsWritten, keysWritten, charsRead;
    struct sockaddr_in serverAddress;
    char dataBuffer[200000], keyBuffer[200000], buffer[200000];
    FILE* dataFile;
    FILE* keyFile;
    // Check usage & args
    if (argc < 4) {
        fprintf(stderr, "USAGE: %s hostname port\n", argv[0]);
        exit(0);
    }

    // Create sockets
    socketFD = socket(AF_INET, SOCK_STREAM, 0);
    if (socketFD < 0) {
        error("CLIENT: ERROR opening socket");
    }

    socketFD2 = socket(AF_INET, SOCK_STREAM, 0);
    if (socketFD2 < 0) {
        error("CLIENT: ERROR opening socket");
    }

    // Set up the server address struct
    setupAddressStruct(&serverAddress, atoi(argv[3]));

    // Connect to server
    if (connect(socketFD, (struct sockaddr*)&serverAddress, sizeof(serverAddress)) < 0) {
        error("CLIENT: ERROR connecting");
    }

    if (connect(socketFD2, (struct sockaddr*)&serverAddress, sizeof(serverAddress)) < 0) {
        error("CLIENT: ERROR connecting");
    }

    // Clear out the dataBuffer/keyBuffer array
    memset(dataBuffer, '\0', sizeof(dataBuffer));
    memset(keyBuffer, '\0', sizeof(keyBuffer));
    memset(buffer, '\0', sizeof(buffer));

    //open the data file
    dataFile = fopen(argv[1], "r");
    if (dataFile == NULL && argv[1] != NULL)
    {
        fprintf(stderr, "cannot open file for input\n");
        close(socketFD);
        exit(1);
    }
    else if (dataFile)
    {
        fread(dataBuffer, 1, sizeof(dataBuffer), dataFile);
    }
    fclose(dataFile);

    //open the key file
    keyFile = fopen(argv[2], "r");
    if (keyFile == NULL && argv[2] != NULL)
    {
        fprintf(stderr, "Error: cannot open file for input\n");
        close(socketFD);
        exit(1);
    }
    else if (keyFile)
    {
        fread(keyBuffer, 1, sizeof(keyBuffer), keyFile);
    }
    fclose(keyFile);

    // check if file length is greater than key
    if (strlen(dataBuffer) > strlen(keyBuffer))
    {
        sprintf(buffer, "key_too_short\n");
    }
    else
    {
        // Write to the server
        // Send client name for confirmation
        sprintf(buffer, "dec_client%c%s%s", '\n', dataBuffer, keyBuffer);
    }
    int len = strlen(buffer);
    if (sendall(socketFD, buffer, &len) == -1) {
        fprintf(stderr, "client: sendall\n");
    }
    close(socketFD);

    // Get return message from server
    // Clear out the dataBuffer again for reuse
    memset(buffer, '\0', sizeof(buffer));
    charsRead = recvall(socketFD2, buffer, sizeof(buffer) - 1);
    if (charsRead < 0) 
    {
        error("CLIENT: ERROR reading from socket");
    }
    else
    {
        if (strcmp(buffer, "key_too_short") == 0)
        {
            fprintf(stderr, "Error: key '%s' is too short\n", argv[2]);
            close(socketFD2);
            exit(1);
        }
        if (strcmp(buffer, "wrong_client") == 0)
        {
            fprintf(stderr, "Error: could not contact dec_server on port %s\n", argv[3]);
            close(socketFD2);
            exit(1);
        }
        else if (strcmp(buffer, "bad_file") == 0)
        {
            fprintf(stderr, "dec_client error: input contains bad characters\n");
            close(socketFD2);
            exit(1);
        }
        else
        {
            fprintf(stdout, "%s", buffer);
            fflush(stdout);
        }
    }
    // Close the socket
    close(socketFD2);
    exit(0);
    return 0;
}