﻿namespace NSMB_RNG
{
    internal class InitSeedSearcher
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

            minTimer0 = 0;
            maxTimer0 = 0x5FF; // Idk if that's right. Idk what timer0 is.
            minVCount = 0;
            maxVCount = 263; // 262?
            minVFrame = 1; // Lowest I've seen is 4.
            maxVFrame = 8; // Highest I've seen is 5.

            secondsRange = 0;
        }

        public List<SeedInitParams> FindSeeds()
        {
            // We will iterate over the chosen range for timer0, vcount, vframe, and seconds.
            List<SeedInitParams> list = new List<SeedInitParams>();
            DateTime dt = seedParams.GetDateTime();
            for (int secs = 0; secs <= secondsRange; secs++)
            {
                for (ushort timer0 = (ushort)minTimer0; timer0 <= maxTimer0; timer0++)
                {
                    seedParams.Timer0 = timer0;
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
                dt.AddSeconds(1);
                seedParams.SetDateTime(dt);

                // Progress reporting
                Console.WriteLine("Searched second " + secs);
            }

            return list;
        }
    }
}
