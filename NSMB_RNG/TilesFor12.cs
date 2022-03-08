using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NSMB_RNG
{
    internal static class TilesFor12
    {
        // The RNG advances 1937 times between entering 1-2 and randomizing the first visible tile.
        // There are 27 tiles per row of tiles in the area of interest.
        const int STEPS_BEFORE = 1937;
        const int TILES_PER_ROW = 27;
        const int TILES_PER_SCREEN_VERTICAL = 12;
        private static char[] tileLetters = new char[] { 'H', 'E', 'P', 'C', 'B', 'A' };
        // TODO: Names. Consider changing A to Submarine, H to Bean/Bell/Liver, B to Turtle/Porkchop

        private static uint LCRNG_NSMB(uint v)
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
        private static List<uint> reverseStep(uint v)
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

        private static uint tileIDwithBeforeStep(uint v) { return tileIDwithAfterStep(LCRNG_NSMB(v)); }
        private static uint tileIDwithAfterStep(uint v) { return ((v >> 8) & 0x7) % 6; }

        public static byte[][] calculateTileRows(uint vBeforEntering12)
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
        public static List<uint>? calculatePossibleSeeds()
        {
            Console.WriteLine("Instructions:");
            Console.WriteLine("1) Enter 1-2 as instructed in the README.txt file.");
            Console.WriteLine("2) Visually identify the first 7 randomized tiles in the first row of tiles. Refer to the tiles.png file for clarification, and for the tile names used by the program.");
            Console.WriteLine("3) The program will continue asking for tiles until only one there is only one possible RNG state. (There will be more than one possible seed that leads to that RNG state.)");
            Console.WriteLine("----------------------------\n");

            // Step 2
            int[] inputTiles = getFirstSevenTiles();
            Console.Write("Reading lookup table...");
            List<uint> lookupResults = lookUpRNGByTiles(inputTiles);
            if (lookupResults.Count == 0)
            {
                Console.WriteLine("Error: Could not retrieve data from lookup table.");
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
            Console.Write("done\n\n");

            // Step 3
            inputTiles = getAllTiles("second");
            Console.Write("Calculating...");
            int distinctValues = removeNonmatchingValues(lookupResults, currentValues, inputTiles);
            if (distinctValues == 0) // should never happen
            {
                Console.WriteLine("There are no RNG values that lead to the given set of tiles. Maybe you mis-typed one?");
                return null;
            }

            // Determine if the list is small enough (meaning, only 1 distinct value).
            if (distinctValues != 1)
            {
                // If not, ask for second-to-last row and basically repeat step 3.
                for (int index = 0; index < currentValues.Count; index++)
                {
                    for (int i = 0; i < TILES_PER_ROW * (TILES_PER_SCREEN_VERTICAL - 4); i++)
                        currentValues[index] = LCRNG_NSMB(currentValues[index]);
                }
                Console.WriteLine("done");

                inputTiles = getAllTiles("second-to-last");
                Console.Write("Calculating...");
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
                    Console.WriteLine("done");
                    inputTiles = getAllTiles("last");
                    Console.Write("Calculating...");
                    removeNonmatchingValues(lookupResults, currentValues, inputTiles);
                    distinctValues = removeNonmatchingValues(lookupResults, currentValues, inputTiles);
                    if (distinctValues == 0) // should never happen
                    {
                        Console.WriteLine("There are no RNG values that lead to the given set of tiles. Maybe you mis-typed one?");
                        return null;
                    }
                }
            }
            Console.WriteLine("done");

            // If the list is large, warn the user and offer to quit.
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

            Console.WriteLine("Found single RNG state, and " + initials.Count + " possible seeds.");
            return initials;
        }
        private static int[] getFirstSevenTiles()
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
        private static int[] getAllTiles(string rowStr)
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
        private static int[]? tryGetTiles(int count)
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
        /// <summary>
        /// Returns the number of distinct values remaining in currentValues.
        /// </summary>
        private static int removeNonmatchingValues(List<uint> lookupResults, List<uint> currentValues, int[] tiles)
        {
            int currentIndex = lookupResults.Count - 1;
            while (currentIndex >= 0)
            {
                // Check if this value matches the input for 11 tiles.
                uint v = currentValues[currentIndex];
                bool match = true;
                if (v != 0) // 0 means it was previously found to have converged with another value
                {
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
                }
                // If not match, remove this entry from the list of possible 'intermediate' RNG values.
                if (!match)
                {
                    lookupResults.RemoveAt(currentIndex);
                    currentValues.RemoveAt(currentIndex);
                }
                else
                    currentValues[currentIndex] = v;

                currentIndex--;
            }

            // Find values that have converged
            HashSet<uint> distinctValues = new HashSet<uint>();
            for (int i = 0; i < currentValues.Count; i++)
            {
                if (currentValues[i] != 0) // 0 means it was previously found to have converged with another value
                {
                    if (distinctValues.Contains(currentValues[i]))
                        currentValues[i] = 0; // choose 0 because it is not a possible result of LCRNG_NSMB
                    else
                        distinctValues.Add(currentValues[i]);
                }
            }
            return distinctValues.Count;
        }


        private static List<uint> lookUpRNGByTiles(int[] firstSeven)
        {
            string folder = firstSeven[0].ToString() + firstSeven[1].ToString() + firstSeven[2].ToString() + "/" +
                firstSeven[3].ToString() + firstSeven[4].ToString();
            string file = "/" + firstSeven[5].ToString() + firstSeven[6].ToString();
            string filePath = "lookup/" + folder + file;
            if (!File.Exists(filePath))
            {
                Console.Write("\nThe lookup file for this pattern was not found. Do you want to download it?\nDownloads are in chunks of up to 1.6MB. [y/n]: ");
                if (!UI.AskYesNo())
                    return new List<uint>();
                if (!DownloadChunk(folder).Result)
                    return new List<uint>();
            }
            FileStream fs = File.OpenRead(filePath);
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

            // The file contains a list of 4-byte values, but these are not exactly the values we want.
            // To get the values we want, we have to first multiply the value by 5, then add it to the previous value.
            // The "previous" value starts at 3, since almost all values that we want equal 3 when taken mod 5.
            // The remainder of the values we want are 2 mod 5:
            // 1) Most of these are equal to some value from the list minus c (= 0x33333333*4).
            //    To find these, we subtract c from values in the range c -> c + m (= 0x19660D, from LCRNG_NSMB),
            //    and then verify that it leads to the same RNG value.
            // 2) The remainder of these are ones where LCRNB_NSMB(x) % 5 is also equal to 2.
            //    These are encoded in the files as the last value in the file, if the high bit is set.
            List<uint> values = new List<uint>(bytesRead / sizeof(uint));
            uint lastV = 3;
            uint c = (uint)0x3333_3333 * 4;
            uint m = 0x19660D;
            for (int i = 0; i < bytesRead - sizeof(uint); i += sizeof(uint))
            {
                lastV += BitConverter.ToUInt32(data, i) * 5;
                values.Add(lastV);
                if (lastV > c && lastV < c + m)
                {
                    if (LCRNG_NSMB(lastV) == LCRNG_NSMB(lastV - c))
                        values.Add(lastV - c);
                }
            }
            // Check for the last value being included by (2) in comments above
            uint lastValueInFile = BitConverter.ToUInt32(data, data.Length - sizeof(uint));
            if ((lastValueInFile & 0x8000_0000) != 0)
                values.Add(lastValueInFile & 0x7fff_ffff);
            else
            {
                lastV += lastValueInFile * 5;
                values.Add(lastV);
                if (lastV > c && lastV < c + m)
                {
                    if (LCRNG_NSMB(lastV) == LCRNG_NSMB(lastV - c))
                        values.Add(lastV - c);
                }
            }
            return values;
        }
        private static bool ExtractFile(string source, string destination)
        {
            // If the directory doesn't exist, create it.
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            string zPath = "7za.exe";
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.RedirectStandardOutput = true;
                psi.FileName = zPath;
                psi.Arguments = "x " + source + " -o" + destination;
                Process? process = Process.Start(psi);
                if (process != null)
                    process.WaitForExit();
            }
            catch
            {
                return false;
            }

            return File.Exists(destination + "/00");
        }
        static HttpClient? client;
        private async static Task<bool> DownloadChunk(string file)
        {
            if (client == null)
                client = new HttpClient();
            string archivePath = "lookup/" + file + ".7z";

            // Download .7z
            Console.WriteLine("Downloading archive file...");
            Directory.CreateDirectory("lookup/" + file);
            Uri uri = new Uri("https://github.com/SuuperW/NSMB_RNG/raw/vLookup/" + archivePath);
            using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
            {
                if (!response.IsSuccessStatusCode)
                    return false;
                long contentLength = response.Content.Headers.ContentLength ?? long.MaxValue;
                using (FileStream fs = new FileStream(archivePath, FileMode.CreateNew))
                {
                    var t = response.Content.CopyToAsync(fs);
                    Thread.Sleep(100);
                    while (!t.IsCompleted)
                    {
                        Console.WriteLine((fs.Length / contentLength * 100).ToString() + "%");
                        Thread.Sleep(500);
                    }
                }
            }

            // Extract contents of .7z archive and delete archive
            Console.Write("Done downloading. Extracting archive file contents...");
            if (!ExtractFile(archivePath, "lookup/" + file))
                return false;
            File.Delete(archivePath);

            return true;
        }


        public static void printTilesFromSeed(uint seed)
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
