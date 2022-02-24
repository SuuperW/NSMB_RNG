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
        // TODO: Names. Consider changing A to Submarine, H to Bean/Bell/Liver, B to Turtle/Porkchop

        private uint LCRNG_NSMB(uint v)
        {
            ulong a = ((ulong)0x0019660D * v + 0x3C6EF35F);
            return (uint)(a + (a >> 32));
        }
        // LCRNG_NSMB can be re-written as:
        // uint m = 0x0019660D
        // uint a = 0x3C6EF35F
        // ulong r = v*m + a
        // ulong b = r + (r >> 32)
        // return (u32)b
        // Consider that m * 0x33333333 equals 0x51468fffaeb97. Let us call this value T. Note that (u32)(T + (T >> 32)) equals 0xffffffff.
        // Let V be any input to nsmbStep such that the 33rd bit of r would be flipped by adding T.
        // Then for all V, nsmbStep(V + 0x33333333) is equal to nsmbStep(V).
        // Note that this only means that (r & 0xffffffff) must be greater than 0x51468, so most possible inputs to nsmbStep satisfy this condition.
        // Let us call the set of all possible inputs that do NOT satisfy this condition E.
        // So, for most values x, nsmbStep(x) equals nsmbStep(x + 0x33333333). In the case that x is in set E, nsmbStep(x) will be equal to nsmbStep(x + 0x33333333) + 1.
        // Also, there are no two possible outputs of nsmbStep with a difference of less than 5, except in cases where one of the inputs is in set E. (In which case, the difference is 4 or 1.)
        // Using these facts, reverseStep can search only up to 0x33333333, and find any remaining values by adding multiples of 0x33333333.
        private List<uint> reverseStep(uint v)
        {
            const uint m = 0x0019660D;
            const ulong twoP32 = 0x100000000;

            const uint bigStep = (uint)(twoP32 / m);
            const uint bigStepOffset = (uint)(twoP32 - (bigStep * m));

            uint tryMe = (v - LCRNG_NSMB(0)) / m;


            while (tryMe < 0x33333333)
            {
                uint result = LCRNG_NSMB(tryMe);
                // We added one so that we can easily check if r is within 1 of v.
                uint diff = result + 1 - v; // uint arithmetic may underflow

                if (diff <= 2)
                    break;
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

            if (tryMe >= 0x33333333)
                return new List<uint>();
            else
            {
                List<uint> allResults = new List<uint>(5);
                uint tryMeToo = tryMe;
                while (tryMeToo >= tryMe) // loop until tryMeToo overflows
                {
                    if (LCRNG_NSMB(tryMeToo) == v)
                        allResults.Add(tryMeToo);

                    tryMeToo += 0x33333333;
                }
                return allResults;
            }
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
                tiles[i] = new byte[11];

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
            for (int i = 0; i < TILES_PER_ROW * (TILES_PER_SCREEN_VERTICAL - 4); i++)
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
            List<uint> lookupResults = lookUpRNGByTiles(inputTiles);
            if (lookupResults.Count == 0) // should never happen
            {
                Console.WriteLine("There are no RNG values that lead to the given set of tiles. Maybe you mis-typed one?");
                return null;
            }
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
            int distinctValues = removeNonmatchingValues(lookupResults, currentValues, inputTiles);
            if (distinctValues == 0) // should never happen
            {
                Console.WriteLine("There are no RNG values that lead to the given set of tiles. Maybe you mis-typed one?");
                return null;
            }

            // Step 3c: Determine if the list is small enough (meaning, only 1 distinct value).
            if (distinctValues != 1)
            {
                // If not, ask for second-to-last row and basically repeat step 3b.
                for (int index = 0; index < currentValues.Count; index++)
                {
                    for (int i = 0; i < TILES_PER_ROW * (TILES_PER_SCREEN_VERTICAL - 4); i++)
                        currentValues[i] = LCRNG_NSMB(currentValues[i]);
                }
                inputTiles = getAllTiles("second-to-last");
                removeNonmatchingValues(lookupResults, currentValues, inputTiles);
                distinctValues = removeNonmatchingValues(lookupResults, currentValues, inputTiles);
                if (distinctValues == 0) // should never happen
                {
                    Console.WriteLine("There are no RNG values that lead to the given set of tiles. Maybe you mis-typed one?");
                    return null;
                }
                // Then check again, and maybe ask for last row.
                if (distinctValues != 1)
                {
                    inputTiles = getAllTiles("last");
                    removeNonmatchingValues(lookupResults, currentValues, inputTiles);
                    distinctValues = removeNonmatchingValues(lookupResults, currentValues, inputTiles);
                    if (distinctValues == 0) // should never happen
                    {
                        Console.WriteLine("There are no RNG values that lead to the given set of tiles. Maybe you mis-typed one?");
                        return null;
                    }
                }
            }

            // Step 4a: If the list is large, warn the user and offer to quit.
            if (lookupResults.Count > 10) // 10 is probably not large enough to bother warning about... but really I don't expect 10 to be possible
            {
                Console.WriteLine("There are " + lookupResults.Count + " potential 'intermediate' RNG values. The process of going back to initial RNG values may take a while.");
                Console.Write("You can quit and try for a different seed if you want. Quit? [y/n]: ");
                string? quit = Console.ReadLine();
                if (!string.IsNullOrEmpty(quit) && quit[0] == 'y')
                    return null;
            }
            // Calculate all possible initial RNG values.
            List<uint> initials = new List<uint>();
            foreach (uint v in lookupResults)
                initials.AddRange(reverseStep(v));

            // Step 4b: Save the list of initial RNG values.
            FileStream fs = File.Open("initialValues.bin", FileMode.Create); // creates new or truncates
            foreach (uint v in initials)
                fs.Write(BitConverter.GetBytes(v));
            fs.Close();
            Console.WriteLine("Success! " + initials.Count + " values written to initialValues.bin.");

            return initials;
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
            else if (rawInput.Length == count)
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
        private List<uint> lookUpRNGByTiles(int[] firstSeven)
        {
            string path = "path/to/my/files/";
            string folder = firstSeven[0].ToString() + firstSeven[1].ToString() + firstSeven[2].ToString() + "/";
            string file = firstSeven[3].ToString() + firstSeven[4].ToString() + firstSeven[5].ToString() + firstSeven[6].ToString();
            FileStream fs = File.OpenRead(path + folder + file);
            byte[] data = new byte[1024 * 210]; // all files are under 210KB
            int bytesRead = 0;
            // The Read method should read all bytes first time, but is not guaranteed to do so.
            int count;
            while ((count = fs.Read(data, bytesRead, data.Length - bytesRead)) != 0)
            {
                bytesRead += count;
                if (bytesRead == data.Length)
                    throw new Exception("There shouldn't be any files that big.");
            }
            fs.Close();

            List<uint> values = new List<uint>(bytesRead / sizeof(uint));
            for (int i = 0; i < bytesRead; i += sizeof(uint))
                values.Add(BitConverter.ToUInt32(data, i));
            return values;
        }
        /// <summary>
        /// Returns the number of distinct values remaining in currentValues.
        /// </summary>
        private int removeNonmatchingValues(List<uint> lookupResults, List<uint> currentValues, int[] tiles)
        {
            int currentIndex = 0;
            while (currentIndex < lookupResults.Count)
            {
                // Check if this value matches the input for 11 tiles.
                uint v = currentValues[currentIndex];
                bool match = true;
                for (int i = 0; i < tiles.Length; i++)
                {
                    v = LCRNG_NSMB(v);
                    uint tID = tileIDwithAfterStep(v);
                    if (tID != tiles[i])
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
                for (int i = tiles.Length; i < TILES_PER_ROW; i++)
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
            return distinctValues.Count;
        }

        public void printTilesFromSeed(uint seed)
        {
            byte[][] tiles = calculateTileRows(seed);
            for (int y = 0; y < tiles.Length + 1; y++)
            {
                if (y == 2)
                    Console.WriteLine("-----------");
                else
                {
                    int tilesToOutput = y == 0 ? 7 : 11;
                    int yIndex = y - (y > 2 ? 1 : 0);
                    for (int x = 0; x < tilesToOutput; x++)
                        Console.Write(tileLetters[tiles[yIndex][x]] + " ");
                    Console.WriteLine();
                }
            }
        }
    }
}
