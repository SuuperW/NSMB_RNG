There is a pre-built version (1.2) for Windows x64 on the Release branch. You can download it here: https://github.com/SuuperW/NSMB_RNG/raw/Release/NSMB_RNG_v1.2.zip

--- Purpose of NSMB_RNG ---
The purpose of NSMB_RNG is to allow speedrunners to play the any% category with the best possible RNG. This means the red ? blocks in World 8 always move left. If attempting mini route, there will be a 7% chance of RNG supporting that in World 5.

--- How to use NSMB_RNG ---
In order to determine which RNG seed your system generated, you will have to:
 A) Create a save file with World 1-2 unlocked (so, any save file other than a blank new one). The step only needs to be done once; the rest need to be done every time you want to look at a new seed.
 B) Ensure that you do not have a GBA game in the GBA slot.
 1) Choose any date/time and set your system's clock to that date/time. When the time is set, the seconds will be set to 0. Write down this date/time. WARNING: On some systems, if the current time already matches the hour and minute you set, it will not actually set the time (seconds) despite what the confirmation message says.
 2) Open NSMB any chosen number of seconds after setting the system's time, and then do not have any controller buttons pressed as the game starts.
 3) The RNG seed is calculated about half a second before the red Nintendo logo appears. Write down the date/time of the system at that point. Use a stopwatch, or something similar, to know how many seconds passed between setting the system time and the RNG seed being calculated. Note that when calculating the RNG seed, the game rounds DOWN to the nearest second.
 4) Open the save file from step A before the cutscene begins.
 5) Go directly to World 1-2 and start the level.
 6) Pause the game before the camera begins scrolling down.
 7) The on-screen tiles are what NSMB_RNG will use to determine your seed and magic.

You will have to repeat the above steps 1-7 multiple times in the following steps:
 1) Give NSMB_RNG your system's MAC address. This can be found somewhere under internet settings.
 2) Load NSMB and look at the first few tiles. Do this several times with the same date and time. Find a sequence that is relatively common, and take a picture of that sequence.
--- Main / GUI version ---
 4) Enter the date and time that the RNG seed was calculated.
 3) Choose your system type in the drop-down menu. (Note: WiiU VC is not supported and will not work. If you have any way of installing custom ROMs on a WiiU VC please contact me.)
 5) Enter the first 7 tiles, according to the tiles.png file.
  A) If these match a known magic for your system, the text beneath the tiles will say so.
  B) If not, you will have to also enter 11 tiles for the second row and wait for NSMB_RNG to calculate a magic.
 6) Click the "Time Finder" button.
 7) If you want to attempt the mini route, check the box.
 8) Click "Search".
 9) NSMB_RNG will check all date/times that have the same seconds count as you used earlier, until it finds one that gives a good seed. If no such date/time exists, it will automatically add one to the seconds count and try again.
  A) Optionally, you can disable automatic seconds and/or select buttons to hold during RNG seed calculation. Use this if no date/time is found for the seconds you want, and you really want that exact seconds count.
 10) The double jumps window will appear. This will tell you how many double jumps you should do in World 1-1 in order to get a shell in 1-2. 
  A) If you chose not to use mini route, it will tell which number of double jumps to NOT do; any other number will work. (Note: 7 and 8 will always work!)
  B) If you chose to use the mini route, it will tell you which numbers of double jumps will work. (There is no number that will always work in this case.)
  C) Note: These double jump numbers are only valid if the camera does not begin scrolling down in 1-2 before you pause and quit to the main menu. Otherwise, extra random numbers will be generated.
 11) You can now quit to the main menu and start a run. For future attempts, you can go straight to the double jumps window by clicking the button on the main window. Go back to 1-2 before each attempt to get new double jump counts.
--- CLI version ---
 3) Determine your system's "magic":
  A) Select the option to find your magic, and select your system type. Follow the promts, which will ask you to input your tile sequence. (This will be compared with known magics, if there are any for your chosen system. If no match is found, NSMB_RNG will have to calculate your seed and then calculate your magic.)
 4) Choose the option to find a date/time that gives a good seed. Follow the promts. (It will ask if you want to do the mini route, for seconds, buttons held, whether to auto-increment seconds after exhausting the DS's date range, and thread count.)
 5) If a good date/time is found, NSMB_RNG will output the tile sequence that the calculated seed should give. If not, try again with another seconds and button combination.
 6) Use that date/time, load NSMB, and confirm that you have the expected tile pattern. You may need a few tries to get the desired seed.
 7) Choose the option in NSMB_RNG to find a number of double jumps in World 1-1. You can then quit to the main menu and start a run. (This step removes the need to memorize toad patterns in the intro cutscene for RNG manip in World 1-1.)
 8) You can now quit to the main menu and start a run. Repeat step 6 every time you boot the game. Repeat step 7 before every attempt.

NSMB_RNG will save a file named settings.bin, which will be loaded when NSMB_RNG starts next time. You can copy/rename this file to keep info for multiple systems.

--- How the game's RNG works ---
The random number generator keeps a 32-bit integer representing its current state. Whenever the game wants a random number, this 32-bit integer is given as the input to a function that outputs another 32-bit integer. The output of this function is then used as the RNG's new state. Depending on what the random number is to be used for, a selection of those 32 bits are then used to calculate the random number that is actually used.
The state of the RNG when it is first started/initialized is called the RNG's "seed". The seed is randomized in a much more robust and random way than the RNG's normal function. (This "more random" method is not used for each random number presumably because it is much slower.) 

--- How the RNG seed is manipulated ---
The RNG seed is calculated using various inputs. The important ones for this application are: (A) a "magic" that varies across console types, and can also slightly vary with a single system, (B) the system's MAC address, (C) the system's current date and time (rounded down to the nearest second), and (D) the controller buttons that are currently held.
In order to control the RNG seed, we first need to determine the system's MAC address and its most common magic. We can then calculate the expected seed for any given date/time and button combination, and search the available possibilities until we find a desired seed.
