using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSMB_RNG
{
    internal class TilesFor12
    {
        // The RNG advances 1937 times between entering 1-2 and randomizing the first visible tile.
        // There are 27 tiles per row of tiles in the area of interest.
        const int STEPS_BEFORE = 1937;
        const int TILES_PER_ROW = 27;
        const int TILES_PER_SCREEN_VERTICAL = 12;

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
        public List<uint> calculatePossibleSeeds()
        {
            Console.WriteLine("Instructions:");
            Console.WriteLine("1a) Create a save file where 1-2 is unlocked and Mario is already in world 1.");
            Console.WriteLine("1b) Set the system clock to some value, and write down that date and time. (The seconds cound should be zero.)");
            Console.WriteLine("1c) Open the game, and take note of how many seconds passed between setting the system clock and the time the red Nintendo logo appears.");
            Console.WriteLine("1b) Load the save file from step 1a before the cutscene begins.");
            Console.WriteLine("-- This is necessary in order to ensure that you can enter 1-2 with zero random numbers being generated between booting the game and entering the level.");
            Console.WriteLine("2a) Enter 1-2.");
            Console.WriteLine("2b) Pause the game before the camera begins scrolling down.");
            Console.WriteLine("2c) Visually identify which randomized tiles were generated in the first, top row of tiles. Refer to the tiles.png file for clarification and for the tile names used by the program.");
            Console.WriteLine("");



            // The first data we will need is the first 7 tiles.
            // The lookup table will be used to get a list of RNG values that match those 7 tiles.
            Console.Write("Enter the first 7 tiles in the first row: ");
            // TODO


        }
    }
}
