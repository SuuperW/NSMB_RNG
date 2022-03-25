using System;
using System.Collections.Generic;

namespace NSMB_RNG
{
    public class InitSeedSearcher
    {
        public int minTimer0, maxTimer0;
        public int minVCount, maxVCount;
        public uint minVFrame, maxVFrame;
        public int secondsRange;

        private SeedInitParams seedParams;
        private HashSet<uint> desiredSeeds;

        public InitSeedSearcher(SeedInitParams seedInitParams, IEnumerable<uint> desiredSeeds)
        {
            seedParams = seedInitParams;
            this.desiredSeeds = new HashSet<uint>();
            foreach (uint s in desiredSeeds)
                this.desiredSeeds.Add(s);

            minTimer0 = 0x200;
            maxTimer0 = 0x5ff; // Idk if that's right. Idk what timer0 is.
            minVCount = 0;
            maxVCount = 263; // 262?
            minVFrame = 3; // Lowest I've seen is 4.
            maxVFrame = 9; // Highest I've seen is 8.

            secondsRange = 1;
        }

        public List<SeedInitParams> FindSeeds(bool reportProgress = false)
        {
            // We will iterate over the chosen range for timer0, vcount, vframe, and seconds.
            List<SeedInitParams> list = new List<SeedInitParams>();
            DateTime dt = seedParams.GetDateTime();
            for (int secs = 0; secs < secondsRange; secs++)
            {
                for (int timer0 = minTimer0; timer0 <= maxTimer0; timer0++)
                {
                    seedParams.Timer0 = (ushort)timer0;
                    for (ushort vCount = (ushort)minVCount; vCount <= maxVCount; vCount++)
                    {
                        seedParams.VCount = vCount;
                        for (uint vFrame = minVFrame; vFrame <= maxVFrame; vFrame++)
                        {
                            seedParams.VFrame = vFrame;
                            if (desiredSeeds.Contains(seedParams.GetSeed()))
                                list.Add(new SeedInitParams(seedParams));
                        }
                    }
                }
                // Increment seconds
                dt = dt.AddSeconds(1);
                seedParams.SetDateTime(dt);

                // Progress reporting
                if (reportProgress)
                    Console.WriteLine("Searched second " + secs);
            }

            return list;
        }
    }
}
