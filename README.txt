--- Purpose of NSMB_RNG ---
The purpose of NSMB_RNG is to allow speedrunners to play the any% category without worrying about getting unlucky. This can be done because the game's random number generator has a bug (or maybe it's a "feature", Idk) that makes it possible for the random numbers that are generated to repeat in short patterns. One of these patterns gives highly desireable random numbers.

--- How the game's RNG works ---
The random number generator keeps a 32-bit integer representing its current state. Whenever the game wants a random number, this 32-bit integer is given as the input to a function that outputs another 32-bit integer. The output of this function is then used as the RNG's new state. Depending on what the random number is to be used for, a selection of those 32 bits are then used to calculate the random number that is actually used.
The state of the RNG when it is first started/initialized is called the RNG's "seed". The seed is randomized in a much more robust and random way than the RNG's normal function. (This "more random" method is not used for each random number presumably because it is much slower.) 

--- How the RNG seed is manipulated ---
The RNG seed is calculated using various inputs. The important ones for this application are: (A) a "magic" that varies across console types, and can also slightly vary with a single system, (B) the system's MAC address, (C) the system's current date and time (rounded down to the nearest second), and (D) the controller buttons that are currently held.
In order to control the RNG seed, we first need to determine the system's MAC address and its most common magic. We can then calculate the expected seed for any given date/time and button combination, and search the available possibilities until we find the desired seed.

--- How to use NSMB_RNG ---
In order to determine which RNG seed your system generated, you will have to:
 0) Create a save file with World 1-2 unlocked. The step can be done only once; the rest need to be done every time you want to look at a new seed.
 1) Set the current date/time to some value. What it is doesn't matter, but write it down and always use the same date/time every time you do this. NOTE: At least on some systems, if the current time already matches the hour and minute you set, it will not actually set the time despite what the confirmation message says.
 2) Open NSMB some number of seconds after setting the system's time, and then do not have any controller buttons pressed as the game starts. Again, this number of seconds doesn't matter as long as it is the same every time.
 3) The RNG seed is calculated about half a second before the red Nintendo logo appears. Write down the date/time of the system at that point. Use a stopwatch, or something similar, to know how many seconds passed between setting the system time and the RNG seed being calculated. Note that when calculating the RNG seed, the game rounds DOWN to the nearest second. Also note that when you set the DS's time, the seconds are set to 0.000.
 4) Open the save file from step 0 before the cutscene begins.
 5) Go directly to World 1-2 and start the level.
 6) Pause the game before the camera begins scrolling down.
 7) The on-screen tiles are what NSMB_RNG will use to determine all seeds that lead to that sequence of tiles. When inputting the tile sequence, refer to the tiles.png file for which letters correspond to which tiles.

You will have to repeat the above steps 1-7 multiple times in the following steps:
 1) Give NSMB_RNG your system's MAC address. This can be found somewhere under internet settings.
 2) Load NSMB and look at the first few tiles several times. Find a sequence that is relatively common, and take a picture of that sequence.
 3) Determine your system's "magic":
  A) Select the option to choose a magic. If NSMB_RNG already has magics for your system, select your system. You will be given a list of magics with their corresponding tile patterns. If any of the tile patterns match what you got, select that magic.
  B) If NSMB_RNG does not have magics for your system, or if they do not match the tile sequence you have:
   I) Follow the promts, which will ask you to input your tile sequence. NSMB_RNG will use the tile sequence to determine which seeds you might have.
   II) Once the seeds have been found, NSMB_RNG will attempt to calculate a magic.
   III) Optionally, confirm this magic by choosing the option to calculate the sequence of tiles that you should see, and comparing with one of the other possible tile sequences (or with another date/time).
 4) Choose the option to find a date/time that gives a good seed. Follow the promts to choose how many seconds you want time date/time to have. NSMB_RNG will test all dates/times with that number of seconds, and only move on to the next second if no good value is found. Optionally, you can choose to end the search instead of looking at the next second, and then re-try but with a certain combination of buttons held. This step will probably take along time to complete, as the average number of seeds that will need to be calculated is about 100 million. The number of available date/times with a specific seconds count is about 52.6 million. NSMB_RNG will produce output after it checks each year, as a means of progress reporting.
 5) Once the correct date/time is found, NSMB_RNG will output the tile sequence that the calculated seed should give, along with a few other sequences that you might see.
 6) Use that date/time, load NSMB, and look at the tiles in World 1-2. It will most likely be one of the sequences NSMB_RNG gave, but you may need a few tries to get the desired seed. Once you have confirmed via the tile sequence that you have the correct seed, you can quit to the main menu and start a run.
 7) Optional: Choose the opiton in NSMB_RNG to find a number of double jumps in World 1-1. Re-load World 1-2 and follow the prompt to input the tile sequence seen in World 1-2. If you do not control your double jumps in 1-1 there is a 1/8 chance that the red ? block in 1-2 will not give a shell.

Once you have the right seed, you can quit to main menu and start a new file as many times as you want without losing good RNG, as long as you do not re-boot the game. Every time you do boot the game, repeat step 6. If you want to guarantee a shell in 1-2, repeat step 7 before each attempt.

NSMB_RNG will save a file named settings.bin which may contain the MAC address you gave, chosen magic, and previously found date/times for good seeds. This file will be loaded when NSMB_RNG starts next time. You can copy/rename this file to keep info for multiple systems.
