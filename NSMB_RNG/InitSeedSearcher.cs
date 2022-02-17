namespace NSMB_RNG
{
    internal class InitSeedSearcher
    {
        public int minTimer0, maxTimer0;
        public int minVCount, maxVCount;
        public uint minVFrame, maxVFrame;
        public int minSeconds, maxSeconds;

        private SeedInitParams seedParams;
        uint desiredSeed;

        public InitSeedSearcher(SeedInitParams seedInitParams, uint desiredSeed)
        {
            seedParams = seedInitParams;
            this.desiredSeed = desiredSeed;

            minTimer0 = 0;
            maxTimer0 = 0x5FF; // Idk if that's right. Idk what timer0 is.
            minVCount = 0;
            maxVCount = 263; // 262?
            minVFrame = 1; // Lowest I've seen is 4.
            maxVFrame = 8; // Highest I've seen is 5.
            minSeconds = 0;
            maxSeconds = 0;
        }

        public SeedInitParams FindSeed()
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
                        for (int secs = minSeconds; secs <= maxSeconds; secs++)
                        {
                            seedParams.Second = secs;
                            if (seedParams.GetSeed() == desiredSeed)
                                return seedParams;
                        }
                    }
                }

                // Progress reporting
                Console.WriteLine("Searched timer0: " + timer0);
            }

            return null;
        }
    }
}
