README:
Compile the program as follows:
      gcc --std=gnu99 -o smallsh main.c

You can run the p3testscript by typing in the command prompt: ./p3testscript 2>&1
This shell has three built-in functions: cd, status, and exit.
The command cd will work with work with any absolute or relative path. Typing just cd will take you to the native file of the complier.
The command status will return 1 or 0 based on the success of the previous input. It will return the ternimation signal, if that was used.
The command exit will close the shell, and clean out any lingering process.
This shell also accepts redirections. An input file redirected via stdin will be opened for reading only. Similarly, an output file 
redirected via stdout should be opened for writing only. If the redirection is to a NULL argument, it will be prompted with /dev/null.
Otherwise, if the argument does not exist, you will be prompted so, and returned the status 1.