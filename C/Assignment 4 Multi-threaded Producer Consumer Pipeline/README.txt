README:
Compile the program as follows:
      gcc -std=gnu99 -pthread -o line_processor main.c

Use: ./line_processor for input directly form the keyboard. The program will output every 80 characters. End the program typing STOP.
Use: ./line_processor < input.txt to convert the text inside the program to the terminal of the console.
use: ./line_processor < input.txt > output.txt to placed the converted text to the designated output file.
use: ./line_processor > output.txt to add converted text directly entered to the terminal to the outputfile.