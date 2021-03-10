README:
Compile the program as follows:
      compileall.sh

That shell will compile all the necessary components to run the program. Use keygen to generate a key of random captial letter of a set length,
using the following format: keygen [KEY_LENGTH] > [OUT_KEY_FILE]
where [KEY_LENGTH] is the entered integer value for the legnth of the key, and [OUT_KEY_FILE] is a file to output the key into.
Set both the enc_server and dec_server in the background with seperate ports. Using the command as follows: enc_server [RANDOM_PORT] &
where [RANDOM_PORT] is a random 5 length integer. Use enc_client, and dec_client calling the corresponding server.
Using the following command: enc_client [FILE_ONE] [KEY_FILE] [PORT_NUMBER] > [ENC_FILE]
enc_client will create an encrypted file of [FILE_ONE]. dec_client will convert an encrypted file back by calling the same key as the encryption.