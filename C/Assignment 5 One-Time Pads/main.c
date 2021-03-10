#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

// Error function used for reporting issues
void error(const char* msg) {
    perror(msg);
    exit(1);
}

// Set up the address struct for the server socket
void setupAddressStruct(struct sockaddr_in* address,
    int portNumber) {

    // Clear out the address struct
    memset((char*)address, '\0', sizeof(*address));

    // The address should be network capable
    address->sin_family = AF_INET;
    // Store the port number
    address->sin_port = htons(portNumber);
    // Allow a client at any address to connect to this server
    address->sin_addr.s_addr = INADDR_ANY;
}

int main(int argc, char* argv[]) {
    int connectionSocket, charsRead, i, file_ok = 1;
    char buffer[256], t_buffer[256];
    char *clientName, *dataFile, *keyFile;
    struct sockaddr_in serverAddress, clientAddress;

    socklen_t sizeOfClientInfo = sizeof(clientAddress);

    // Check usage & args
    if (argc < 2) {
        fprintf(stderr, "USAGE: %s port\n", argv[0]);
        exit(1);
    }

    // Create the socket that will listen for connections
    int listenSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (listenSocket < 0) {
        error("ERROR opening socket");
    }

    // Set up the address struct for the server socket
    setupAddressStruct(&serverAddress, atoi(argv[1]));

    // Associate the socket to the port
    if (bind(listenSocket,
        (struct sockaddr*)&serverAddress,
        sizeof(serverAddress)) < 0) {
        error("ERROR on binding");
    }

    // Start listening for connetions. Allow up to 5 connections to queue up
    listen(listenSocket, 5);

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
            while (1)
            {
                // Accept the connection request which creates a connection socket
                connectionSocket = accept(listenSocket,
                    (struct sockaddr*)&clientAddress,
                    &sizeOfClientInfo);
                if (connectionSocket < 0) {
                    error("ERROR on accept");
                }

                file_ok = 1;
                // Get the message from the client and display it
                memset(buffer, '\0', 256);
                // Read the client's message from the socket
                charsRead = recv(connectionSocket, buffer, 255, 0);

                char* token = strtok(buffer, "\n");
                clientName = calloc(strlen(token) + 1, sizeof(char));
                strcpy(clientName, token);
                printf("clientName; %s", clientName);
                if (strcmp(clientName, "enc_client") != 0) {
                    if (strcmp(clientName, "key_too_short") == 0) 
                    {
                        //file is not enc_client -> need to make client output the error
                        strcpy(buffer, "key_too_short");
                    }
                    else
                    {
                        //file is not enc_client -> need to make client output the error
                        strcpy(buffer, "wrong_client");
                    }
                    file_ok = 0;
                }

                if (file_ok == 1)
                {
                    token = strtok(NULL, "\n");
                    dataFile = calloc(strlen(token) + 1, sizeof(char));
                    strcpy(dataFile, token);

                    i = 0;
                    while (i < strlen(dataFile) - 1)
                    {
                        if (isalpha(dataFile[i]) == 0 && dataFile[i] != ' ')
                        {
                            if (isupper(dataFile[i]) == 0)
                            {
                                //invalid character detected
                                strcpy(buffer, "bad_file");
                                file_ok = 0;
                                break;
                            }
                        }
                        i++;
                    }
                }

                if (file_ok == 1)
                {
                    // The next token is the year
                    token = strtok(NULL, "\n");
                    keyFile = calloc(strlen(token) + 1, sizeof(char));
                    strcpy(keyFile, token);
                    memset(buffer, '\0', sizeof(buffer));
                    i = 0;
                    while (i < strlen(dataFile))
                    {
                        if (dataFile[i] == 32)
                            dataFile[i] = 91;

                        t_buffer[i] = (dataFile[i] - 'A') + (keyFile[i] - 'A');
                        t_buffer[i] = (t_buffer[i] % 27) + 'A';
                        if (t_buffer[i] == 91)
                            t_buffer[i] = 32;

                        sprintf(buffer, "%s%c", buffer, t_buffer[i]);
                        i++;
                    }
                    sprintf(buffer, "%s%c", buffer, '\n');
                }

                // Send a Success back to the client
                charsRead = send(connectionSocket,
                    buffer, 256, 0);
                if (charsRead < 0) {
                    error("ERROR writing to socket");
                }
                // Close the connection socket for this client
                close(connectionSocket);
            }
            break;
        default:
            //spawnPid = waitpid(spawnPid, &status, WNOHANG);
            spawnPid = waitpid(spawnPid, &status, 0);
            break;
    }
    // Close the listening socket
    close(listenSocket);
    exit(0);
    return 0;
}
