There is a pre-built version (1.5) for Windows x64 on the Release branch. You can download it here: https://github.com/SuuperW/NSMB_RNG/raw/Release/NSMB_RNG_v1.5.zip

--- Purpose of NSMB_RNG ---
The purpose of NSMB_RNG is to allow speedrunners to play the any% category with the best possible RNG. This means the red ? blocks in World 8 always move left. If attempting mini route, there will be a 7% chance of RNG supporting that in World 5, or 56% if you can manipulate RNG through 5-1.

--- How to use NSMB_RNG ---
You will have to have the game generate multiple randomized tile sequences. Any time you need to do that, follow these steps:
 1) Make sure you have an existing NSMB saved file. (Because you must load 1-2 without viewing the cutscene.)
 2) Ensure that you do not have a GBA game in the GBA slot.
 3) Choose any date+time and write it down. Set your DS's clock to that date+time. When the time is set, the seconds will be set to 0. WARNING: On some DSs, if the current time already matches the hour and minute you set, it will not actually set the time (seconds) despite what the confirmation message says.
 4) Open NSMB any chosen number of seconds after setting the DS's time, and then do not have any controller buttons pressed as the game starts.
 5) The RNG seed is calculated about half a second before the Nintendo logo appears. Write down the number of seconds that have passed between this time and when you set the date+time. Use a stopwatch, or something similar, to know how many seconds passed between setting the DS's time and the RNG seed being calculated. Note that when calculating the RNG seed, the game rounds DOWN to the nearest second.
 6) Open the save file from step 1 before the cutscene begins.
 7) Go directly to World 1-2 and start the level.
 8) Pause the game before the screen begins scrolling down.
 9) The on-screen tiles are what NSMB_RNG will use to determine your seed and magic.

Now, here are the steps to set up your game for optimal RNG:
 1) Get a tile sequence from 1-2 (as described above). You may want to do this multiple times and find a relatively common pattern, because this increases the chance you have the seconds count that you think you do. Also because any given date+time can result in a few different tile sequences, which may not be equally likely.
 2) Choose your DS type in the drop-down menu. (Note: It is not possible to manipulate RNG via date+time on WiiU VC.)
 3) Give NSMB_RNG your DS's MAC address. This can be found somewhere under internet settings.
--- Main / GUI version ---
 4) Enter the date and time that the RNG seed was calculated.
 5) Enter the first several tiles, according to the tiles.png file.
  A) If these match a known magic for your DS, the text beneath the tiles will say so. Ensure the full displayed tile sequence matches what you see in-game.
  B) If not, you will have to enter 7 tiles from the first row and 11 tiles from the second row and wait for NSMB_RNG to calculate a magic. (Most magics for US ROMs are known, most others are not.)
  C) If the app says it cannot find a magic, this means you probably entered something wrong. Try changing the number of seconds you entered by 1 up or down then try again, as this is a common error. (It may not even be your fault; the DS might have not set the seconds to EXACTLY zero when you set the time.)
 6) Once a magic has been found, click the "Time Finder" button.
 7) If you want to attempt the mini route, check the box.
 8) Click "Search". (You don't need to worry about any other options here.)
 9) NSMB_RNG will check all date+times that have the same seconds count as you used earlier, until it finds one that gives a good seed. If no such date+time exists, it will automatically add one to the seconds count and try again.
  A) Optionally, you can disable automatic seconds and/or select buttons to hold during RNG seed calculation. Use this if no date/time is found for the seconds you want, and you really want that exact seconds count.
 10) Once a good date+time is found, the double jumps window will appear. This will show you the tile sequence you should get with that date+time and tell you how many double jumps you should do in World 1-1 in order to get a shell in 1-2. 
  A) If you chose not to use mini route, it will tell which number of double jumps to NOT do; any other number will work. (Note: 0, 2, 3, and 8 will always work!)
  B) If you chose to use the mini route, it will tell you which numbers of double jumps will work. (There is no number that will always work in this case.)
  C) NOTE: The given number of double jumps are valid only if you paused before Mario fell one-third of the way down the screen.
 11) Use the given date+time to get a new tile sequence. Verify that it matches the displayed tile sequence. If it doesn't just try again - as mentioned before, any given date+time can result in a few different tile sequences so you'll need a bit of luck. (Or you might have missed the right start-up time.)
 12) Once you have the right sequence, you can use the pause menu to quit and return to the main menu, and start a run.
 13) You keep the good RNG as long as the game keeps running. You'll only need to reset the DS's date+time and verify the tile sequence (as in step 11) if/when you reboot the DS. However you may need a different number of double jumps for each attempt. You can return to 1-2 before each new attempt and enter the position of the P tile in the double jumps window to get a new count. (Or you can memorize toad patterns in the intro cutscene. Or just use 0/2/3/8 double jumps if using the non-mini option.)
--- CLI version ---
 3) Determine your DS's "magic":
  A) Select the option to find your magic, and select your DS type. Follow the promts, which will ask you to input your tile sequence. (This will be compared with known magics, if there are any for your chosen DS. If no match is found, NSMB_RNG will have to calculate your seed and then calculate your magic.)
 4) Choose the option to find a date+time that gives a good seed. Follow the promts. (It will ask if you want to do the mini route, for seconds, buttons held, whether to auto-increment seconds after exhausting the DS's date range, and thread count.)
 5) If a good date+time is found, NSMB_RNG will output the tile sequence that the calculated seed should give. If not, try again with another seconds and button combination.
 6) Use that date+time, load NSMB, and confirm that you have the expected tile pattern. You may need a few tries to get the desired seed.
 7) Choose the option in NSMB_RNG to find a number of double jumps in World 1-1. You can then quit to the main menu and start a run. (This step removes the need to memorize toad patterns in the intro cutscene for RNG manip in World 1-1.)
 8) You can now quit to the main menu and start a run. Repeat step 6 every time you boot the game. Repeat step 7 before every attempt.

NSMB_RNG will save a file named settings.bin, which will be loaded when NSMB_RNG starts next time. You can copy/rename this file to keep info for multiple DSs.

--- How the game's RNG works ---
The random number generator keeps a 32-bit integer representing its current state. Whenever the game wants a random number, this 32-bit integer is given as the input to a function that outputs another 32-bit integer. The output of this function is then used as the RNG's new state. Depending on what the random number is to be used for, a selection of those 32 bits are then used to calculate the random number that is actually used.
The state of the RNG when it is first started/initialized is called the RNG's "seed". The seed is randomized in a much more robust and random way than the RNG's normal function. (This "more random" method is not used for each random number presumably because it is much slower.) 

--- How the RNG seed is manipulated ---
The RNG seed is calculated using various inputs. The important ones for this application are: (A) a "magic" that varies across console types, and can also slightly vary with a single DS, (B) the DS's MAC address, (C) the DS's current date and time (rounded down to the nearest second), and (D) the controller buttons that are currently held.
In order to control the RNG seed, we first need to determine the DS's MAC address and its most common magic. We can then calculate the expected seed for any given date+time and button combination, and search the available possibilities until we find a desired seed.
