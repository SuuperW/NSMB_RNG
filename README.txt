There is a pre-built version (1.0) for Windows x64 on the Release branch. You can download it here: https://github.com/SuuperW/NSMB_RNG/raw/Release/NSMB_RNG_v1.0.zip

--- Purpose of NSMB_RNG ---
The purpose of NSMB_RNG is to allow speedrunners to play the any% category with the best possible RNG. This means the red ? blocks in World 8 always move left. If attempting mini route, there will be a 7% chance of RNG supporting that in World 5.

--- How the game's RNG works ---
The random number generator keeps a 32-bit integer representing its current state. Whenever the game wants a random number, this 32-bit integer is given as the input to a function that outputs another 32-bit integer. The output of this function is then used as the RNG's new state. Depending on what the random number is to be used for, a selection of those 32 bits are then used to calculate the random number that is actually used.
The state of the RNG when it is first started/initialized is called the RNG's "seed". The seed is randomized in a much more robust and random way than the RNG's normal function. (This "more random" method is not used for each random number presumably because it is much slower.) 

--- How the RNG seed is manipulated ---
The RNG seed is calculated using various inputs. The important ones for this application are: (A) a "magic" that varies across console types, and can also slightly vary with a single system, (B) the system's MAC address, (C) the system's current date and time (rounded down to the nearest second), and (D) the controller buttons that are currently held.
In order to control the RNG seed, we first need to determine the system's MAC address and its most common magic. We can then calculate the expected seed for any given date/time and button combination, and search the available possibilities until we find a desired seed.

--- How to use NSMB_RNG ---
In order to determine which RNG seed your system generated, you will have to:
 0) Create a save file with World 1-2 unlocked (so, any save file other than a blank new one). The step only needs to be done once; the rest need to be done every time you want to look at a new seed.
 1) Set the current date/time in your system's settings to some value. What it is doesn't matter, but write it down and always use the same date/time every time you do this. WARNING: At least on some systems, if the current time already matches the hour and minute you set, it will not actually set the time despite what the confirmation message says.
 2) Open NSMB some number of seconds after setting the system's time, and then do not have any controller buttons pressed as the game starts. Again, this number of seconds doesn't matter as long as it is the same every time.
 3) The RNG seed is calculated about half a second before the red Nintendo logo appears. Write down the date/time of the system at that point. Use a stopwatch, or something similar, to know how many seconds passed between setting the system time and the RNG seed being calculated. Note that when calculating the RNG seed, the game rounds DOWN to the nearest second. Also note that when you set the DS's time, the seconds are set to 0.000.
 4) Open the save file from step 0 before the cutscene begins.
 5) Go directly to World 1-2 and start the level.
 6) Pause the game before the camera begins scrolling down.
 7) The on-screen tiles are what NSMB_RNG will use to determine your seed and magic.

You will have to repeat the above steps 1-7 multiple times in the following steps:
 1) Give NSMB_RNG your system's MAC address. This can be found somewhere under internet settings.
 2) Load NSMB and look at the first few tiles several times. Find a sequence that is relatively common, and take a picture of that sequence.
 3) Determine your system's "magic":
  A) Select the option to find your magic, and select your system type. Follow the promts, which will ask you to input your tile sequence. (This will be compared with known magics, if there are any for your chosen system. If no match is found, NSMB_RNG will have to calculate your seed and then calculate your magic.)
 4) Choose the option to find a date/time that gives a good seed. Follow the promts. (It will ask if you want to do the mini route, for seconds, buttons held, whether to auto-increment seconds after exhausting the DS's date range, and thread count.)
 5) If a good date/time is found, NSMB_RNG will output the tile sequence that the calculated seed should give. If not, try again with another seconds and button combination. (There is approximately a 50% chance of any given second count and buttons combination containing a good date/time.)
 6) Use that date/time, load NSMB, and confirm that you have the expected tile pattern. You may need a few tries to get the desired seed.
 7) Choose the option in NSMB_RNG to find a number of double jumps in World 1-1. You can then quit to the main menu and start a run. (This step removes the need to memorize toad patterns in the intro cutscene for RNG manip in World 1-1.)

Once you have the right seed, you can quit to main menu and start a new file as many times as you want without losing good RNG, as long as you do not re-boot the game. Every time you do boot the game, repeat step 6. If you want to guarantee a shell in 1-2, repeat step 7 before each attempt.

NSMB_RNG will save a file named settings.bin which may contain the MAC address you gave and your magic. This file will be loaded when NSMB_RNG starts next time. You can copy/rename this file to keep info for multiple systems.
