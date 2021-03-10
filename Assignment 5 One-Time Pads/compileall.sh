#!/bin/bash
gcc -o enc_server enc_server.c
gcc -o enc_client enc_client.c
gcc -o dec_server dec_server.c
gcc -o dec_client dec_client.c
gcc -std=gnu99 -o keygen keygen.c