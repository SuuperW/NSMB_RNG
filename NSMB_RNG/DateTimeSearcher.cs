using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NSMB_RNG
{
    internal class DateTimeSearcher
    {
        HashSet<uint> seedsForNoMini = new HashSet<uint>() {
            0xaa99ad81, 0x2aa12d89, 0xa2a1a589, 0xaaa3ad8b, 0xaa21ad09, 0xcaa1cd89, 0xaca1af89, 0x11281410,
            0x4433471b, 0xc43ac722, 0x3c3b3f23, 0x43bb46a3, 0x443d4725, 0x643b6723, 0x463b4923, 0xaac1ada9,
            0xddcce0b4, 0xd5d4d8bc, 0x5dd460bc, 0xddd6e0be, 0xdd54e03c, 0xfdd500bc, 0xdfd4e2bc, 0x445b4743,
            0x77667a4e, 0xf76dfa55, 0x6f6e7256, 0x76ee79d6, 0x77707a58, 0x976e9a56, 0x796e7c56, 0xddf4e0dc,
            0x110013e8, 0x910793ef, 0x09080bf0, 0x130815f0, 0x310833f0, 0x10881370, 0x110a13f2, 0x778e7a76
        };
        HashSet<uint> seedsForMini = new HashSet<uint>() {
            0x449b4783, 0x2aa02d88, 0xa921ac09, 0x0aa20d8a, 0x92a19589, 0xb0a1b389, 0x4423470b, 0xaaa7ad8f,
            0xde34e11c, 0xc439c721, 0x42bb45a3, 0x2c3b2f23, 0x4a3b4d23, 0xa43ba723, 0xddbce0a4, 0x44414729,
            0x77ce7ab6, 0x5dd360bb, 0xe3d4e6bc, 0x3dd540bd, 0x77567a3e, 0xc5d4c8bc, 0xdc54df3c, 0xdddae0c2,
            0x11681450, 0xf76cfa54, 0x75ee78d6, 0x5f6e6256, 0x7d6e8056, 0x10f013d8, 0xd76eda56, 0x77747a5c,
            0xab01ade9, 0x910693ee, 0xf907fbef, 0x0f881270, 0x170819f0, 0xaa89ad71, 0x710873f0, 0x110e13f6
        };
        HashSet<uint> desiredSeeds;

        private int seconds;
        private uint buttonsHeld;
        private ulong mac;
        private uint magic;

        public DateTimeSearcher(int seconds, uint buttonsHeld, ulong mac, uint magic, bool wantMini)
        {
            this.seconds = seconds;
            this.buttonsHeld = buttonsHeld;
            this.mac = mac;
            this.magic = magic;
            if (wantMini)
                desiredSeeds = seedsForMini;
            else
                desiredSeeds = seedsForNoMini;
        }

        bool cancel = false;
        private DateTime worker(int startYear, int yearCount)
        {
            DateTime dt = new DateTime(startYear, 1, 1, 0, 0, 0).AddSeconds(seconds);
            SeedInitParams sip = new SeedInitParams(mac, dt);
            new SystemSeedInitParams(magic).SetSeedParams(sip);
            sip.Buttons = buttonsHeld;

            // loop through all minutes with the given seconds count
            while (!cancel && dt.Year < startYear + yearCount)
            {
                if (desiredSeeds.Contains(sip.GetSeed()))
                    return dt;
                dt = dt.AddMinutes(1);
                sip.SetDateTime(dt);
            }

            // No match
            return new DateTime(1, 1, 1);
        }

        public DateTime findGoodDateTime(int threads = 8)
        {
            // Start threads
            Task<DateTime>[] searchers = new Task<DateTime>[threads];
            double year = 2000;
            for (int i = 0; i < threads; i++)
            {
                int startYear = (int)year;
                year += 100 / threads;
                int endYear = (int)year;
                if (i == threads - 1) // just to make sure we avoid rounding errors
                    endYear = 2100;

                searchers[i] = Task.Run<DateTime>(() => worker(startYear, endYear - startYear));
            }
            Console.Write("Searching with seconds=" + seconds.ToString());

            // Wait for completion
            bool allCompleted = false;
            DateTime result = new DateTime(1, 1, 1);
            while (!allCompleted)
            {
                Thread.Sleep(1000);
                Console.Write('.');
                allCompleted = true;
                foreach (Task<DateTime> t in searchers)
                {
                    if (t.IsCompleted)
                    {
                        // If we don't have a result yet, see if this thread found a DateTime.
                        if (result.Year == 1)
                        {
                            DateTime dt = t.Result;
                            if (dt.Year >= 2000)
                            {
                                // Save it to be returned, and cancel remaining threads.
                                result = dt;
                                cancel = true;
                            }
                        }
                    }
                    else
                        allCompleted = false;
                }
            }
            Console.WriteLine();

            return result;
        }
    }
}
