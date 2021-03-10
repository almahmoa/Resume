README:
Compile the program as follows:
      gcc --std=gnu99 -o movies_by_year main.c
      ./movies_by_year

The program will then prompt you a selection of question which will read an input between 1-2. If the user selects option 1,
the program will ask from another set of three questions asking for an input between 1-3, else if the user selects option 2,
the program will exit. Reaching the second set of questions, input 1 will make a diractory with the name "almahmoa.movies.random"
where random is a random number between 0-99999. The dictionary will be populated with text filed based on the years of the movies in the largest
file in the base directory. Input 2 will do the same for the smallest file, and input 3 will do the same with a valid string input entry.
After a sucessful run, the program will return to the first selection of questions. The program will crash if there is no files to process.