namespace NSMB_RNG
{
    internal class TilesFor12
    {
        // The RNG advances 1937 times between entering 1-2 and randomizing the first visible tile.
        // There are 27 tiles per row of tiles in the area of interest.
        const int STEPS_BEFORE = 1937;
        const int TILES_PER_ROW = 27;
        const int TILES_PER_SCREEN_VERTICAL = 12;
        char[] tileLetters = new char[] { 'H', 'E', 'P', 'C', 'B', 'A' };
        // TODO: Names. Consider changing A to Submarine, H to Bean/Bell/Liver, B to Turtle

        private uint LCRNG_NSMB(uint v)
        {
            ulong a = ((ulong)0x0019660D * v + 0x3C6EF35F);
            return (uint)(a + (a >> 32));
        }
        private List<uint> reverseStep(uint v)
        {
            const uint m = 0x0019660D;
            const ulong twoP32 = 0x100000000;

            const uint bigStep = (uint)(twoP32 / m);
            const uint bigStepOffset = (uint)(twoP32 - (bigStep * m));

            uint tryMe = (v - LCRNG_NSMB(0)) / m;
            uint lastTry = 0;

            List<uint> allResults = new List<uint>(5);

            while (tryMe >= lastTry) // loop until we pass uint.MaxValue
            {
                uint result = LCRNG_NSMB(tryMe);
                uint diff = result - v; // uint arithmetic may underflow

                if (diff == 0)
                {
                    allResults.Add(tryMe);
                    // Go to next value known to be before the next match
                    uint bigStepsCount = (uint)((twoP32 - bigStep * m) / bigStepOffset);
                    tryMe += bigStep * bigStepsCount;
                }
                // Underflow, result was less than v: Get back above v.
                else if (diff >= 0x80000000)
                    tryMe += 1;
                // Result was greater than v: Go to next value known to be before or at the next match.
                else
                {
                    uint bigStepsCount = (diff + bigStepOffset - 1) / bigStepOffset;
                    tryMe += bigStep * bigStepsCount;
                }
            }

            return allResults;
        }

        private uint tileIDwithBeforeStep(uint v)
        {
            v = LCRNG_NSMB(v);
            return ((v >> 8) & 0x7) % 6;
        }
        private uint tileIDwithAfterStep(uint v) { return ((v >> 8) & 0x7) % 6; }

        public byte[][] calculateTileRows(uint vBeforEntering12)
        {
            // Do pre-randomized-tiles rng steps
            uint v = vBeforEntering12;
            for (int i = 0; i < STEPS_BEFORE; i++)
                v = LCRNG_NSMB(v);

            // Create arrays
            byte[][] tiles = new byte[4][];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = new byte[10];

            // Populate first two rows
            for (int row = 0; row < 2; row++)
            {
                for (int i = 0; i < tiles[row].Length; i++)
                {
                    v = LCRNG_NSMB(v);
                    uint tID = tileIDwithAfterStep(v);
                    tiles[row][i] = (byte)tID;
                }
                for (int i = tiles[row].Length; i < TILES_PER_ROW; i++)
                    v = LCRNG_NSMB(v);
            }

            // Advance to second-to-last row
            for (int i = 0; i < TILES_PER_ROW * TILES_PER_SCREEN_VERTICAL - 4; i++)
                v = LCRNG_NSMB(v);

            // Populate last two rows
            for (int row = 2; row < tiles.Length; row++)
            {
                for (int i = 0; i < tiles[row].Length; i++)
                {
                    v = LCRNG_NSMB(v);
                    uint tID = tileIDwithAfterStep(v);
                    tiles[row][i] = (byte)tID;
                }
                for (int i = tiles[0].Length; i < TILES_PER_ROW; i++)
                    v = LCRNG_NSMB(v);
            }

            return tiles;
        }

        /// <summary>
        /// This method will prompt the user to input various information in order to perform the calculation.
        /// </summary>
        public List<uint>? calculatePossibleSeeds()
        {
            Console.WriteLine("Instructions:");
            Console.WriteLine("1a) Create a save file where 1-2 is unlocked.");
            Console.WriteLine("1b) Set the system clock to some value, and write down that date and time. (The seconds cound should be zero.)");
            Console.WriteLine("1c) Open the game, and take note of how many seconds passed between setting the system clock and the time the red Nintendo logo appears.");
            Console.WriteLine("1d) Load the save file from step 1a before the cutscene begins.");
            Console.WriteLine("-- This is necessary in order to ensure that you can enter 1-2 with zero random numbers being generated between booting the game and entering the level.");
            Console.WriteLine("2a) Enter 1-2. This must be the first level (including special levels such as toad house) that the game loads.");
            Console.WriteLine("2b) Pause the game before the camera begins scrolling down.");
            Console.WriteLine("2c) Visually identify the first 7 randomized tiles in the first, top row of tiles. Refer to the tiles.png file for clarification and for the tile names used by the program.");
            Console.WriteLine("-- If the visible tiles are all the same, go back to step 1b and choose a different time. This is extremely unlikley to happen, but if it does the program will not work.");
            // TODO: Verify that the above warning is necessary. Check that 1-cycle has only dead ends leading to it, and that no small-cycles give all identical tiles.
            Console.WriteLine("-- Double or triple-check that you enter the tiles correctly! The process of finding RNG seeds can take a long time, and will not work at all if any of the entered values are incorrect.");
            Console.WriteLine("3a) The program will use a lookup table to identify all 'intermediate' RNG values that lead to this sequence of tiles.");
            Console.WriteLine("-- The 'intermediate' RNG values are tracked for optimization reasons. Actual initial RNG seeds will be calculated later on.");
            Console.WriteLine("3b) Once the data is retrieved and processed, the program will ask for all tiles in the second row of tiles.");
            Console.WriteLine("3c) If the number of possible 'intermediate' RNG values is still too large, it will ask for the second-to-last on-screen row, and then the last on-screen row.");
            Console.WriteLine("4a) The list of 'intermediate' RNG values will be used to calculate a list of initial RNG values. This list will be ~5 times larger.");
            Console.WriteLine("4b) This list will be saved to a file for use by another part of this program. You may with to save a copy of this file if you intend to go through this process a second time.");
            Console.WriteLine("----------------------------\n");

            // Step 2c
            int[] inputTiles = getFirstSevenTiles();

            // Step 3a
            List<uint> lookupResults = new List<uint>(); // TODO
            // processing data
            List<uint> currentValues = new List<uint>();
            foreach (uint v in lookupResults)
            {
                uint r = v;
                for (int i = 0; i < STEPS_BEFORE - 1; i++)
                    r = LCRNG_NSMB(r);
                currentValues.Add(r);
            }
            // Verify that the found values are good and move to second row.
            for (int index = 0; index < currentValues.Count; index++)
            {
                uint v = currentValues[index];
                for (int i = 0; i < 7; i++)
                {
                    v = LCRNG_NSMB(v);
                    uint tID = tileIDwithAfterStep(v);
                    if (tID != inputTiles[i])
                    {
                        Console.WriteLine("It appears that the lookup table is corrupted.");
                        return null;
                    }
                }
                for (int i = 7; i < TILES_PER_ROW; i++)
                    v = LCRNG_NSMB(v);
                currentValues[index] = v;
            }

            // Step 3b
            inputTiles = getAllTiles("second");
            int currentIndex = 0;
            while (currentIndex < lookupResults.Count)
            {
                // Check if this value matches the input for 11 tiles.
                uint v = currentValues[currentIndex];
                bool match = true;
                for (int i = 0; i < inputTiles.Length; i++)
                {
                    v = LCRNG_NSMB(v);
                    uint tID = tileIDwithAfterStep(v);
                    if (tID != inputTiles[i])
                    {
                        match = false;
                        break;
                    }
                }
                // If not match, remove this entry from the list of possible 'intermediate' RNG values.
                if (!match)
                {
                    lookupResults.RemoveAt(currentIndex);
                    currentValues.RemoveAt(currentIndex);
                    continue;
                }
                // Move to the next row
                for (int i = inputTiles.Length; i < TILES_PER_ROW; i++)
                    v = LCRNG_NSMB(v);
                // Update
                currentValues[currentIndex] = v;
                currentIndex++;
            }

            // Find values that have converged
            HashSet<uint> distinctValues = new HashSet<uint>();
            for (int i = 0; i < currentValues.Count; i++)
            {
                if (distinctValues.Contains(currentValues[i]))
                    currentValues[i] = 0; // 0 is not a possible result of LCRNG_NSMB
                else
                    distinctValues.Add(currentValues[i]);
            }

            // Step 3c: Determine if the list is small enough (meaning, only 1 distinct value).
            // If not, ask for second-to-last row and basically repeat step 3b.
            // Then check again, and maybe ask for last row.

            // Step 4a: If the list is still more than one distinct value, warn the user and offer to quit.
            // Calculate all possible initial RNG values.

            // Step 4b: Save the list of initial RNG values.
        }
        private int[] getFirstSevenTiles()
        {
            while (true)
            {
                Console.Write("Enter the first 7 tiles in the first row: ");
                int[]? tiles = tryGetTiles(7);

                if (tiles != null)
                    return tiles;
                else
                    Console.WriteLine("Input was not in the expected format. Make sure you are entering exactly 7 letters, that each letter is a valid tile letter as indicated in tiles.png, and that you input only letters and (optionally) spaces.");
            }
        }
        private int[] getAllTiles(string rowStr)
        {
            while (true)
            {
                Console.Write("Enter all 11 visible tiles in the " + rowStr + " row: ");
                int[]? tiles = tryGetTiles(11);

                if (tiles != null)
                    return tiles;
                else
                    Console.WriteLine("Input was not in the expected format. Make sure you are entering exactly 11 letters, that each letter is a valid tile letter as indicated in tiles.png, and that you input only letters and (optionally) spaces.");
            }
        }
        private int[]? tryGetTiles(int count)
        {
            string? rawInput = Console.ReadLine();
            int[] tilesIntArray = new int[0]; // assigned because not-smart-enough code analyzer gave a warning
            bool inputIsValid = true;
            if (string.IsNullOrEmpty(rawInput))
                inputIsValid = false;
            else if (rawInput.Length == 7)
            {
                rawInput = rawInput.ToUpper();
                tilesIntArray = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    tilesIntArray[i] = Array.IndexOf(tileLetters, rawInput[i]);
                    if (tilesIntArray[i] == -1)
                        inputIsValid = false;
                }
            }
            else
            {
                string[] tilesStrArray = rawInput.ToUpper().Split(' ');
                if (tilesStrArray.Length != 7)
                    inputIsValid = false;
                else
                {
                    tilesIntArray = new int[7];
                    for (int i = 0; i < 7; i++)
                    {
                        tilesIntArray[i] = Array.IndexOf(tileLetters, tilesStrArray[i][0]);
                        if (tilesIntArray[i] == -1)
                            inputIsValid = false;
                    }
                }
            }

            if (inputIsValid)
                return tilesIntArray;
            else
                return null;
        }
    }
}
