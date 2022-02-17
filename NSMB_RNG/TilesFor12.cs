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
        // There are 27 tiles per row of tiles in that area. (Until the last row in the area of interest, which is longer.)
        const int STEPS_BEFORE = 1937;
        const int TILES_PER_ROW = 27;

        private uint LCRNG_NSMB(uint v)
        {
            ulong a = ((ulong)0x0019660D * v + 0x3C6EF35F);
            return (uint)(a + (a >> 32));
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
            byte[][] tiles = new byte[3][];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = new byte[10];

            // Populate first row
            for (int i = 0; i < tiles[0].Length; i++)
            {
                v = LCRNG_NSMB(v);
                uint tID = tileIDwithAfterStep(v);
                tiles[0][i] = (byte)tID;
            }
            for (int i = tiles[0].Length; i < TILES_PER_ROW; i++)
                v = LCRNG_NSMB(v);

            // Advance to second-to-last row
            for (int i = 0; i < TILES_PER_ROW * 12; i++)
                v = LCRNG_NSMB(v);

            // Populate last two rows
            for (int row = 1; row < tiles.Length; row++)
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
    }
}
