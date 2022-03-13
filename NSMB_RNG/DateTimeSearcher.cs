using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NSMB_RNG
{
    internal class DateTimeSearcher
    {
        HashSet<uint> desiredSeeds = new HashSet<uint>() {
            0xaa99ad81, 0x2aa12d89, 0xa2a1a589, 0xaaa3ad8b, 0xaa21ad09, 0xcaa1cd89, 0xaca1af89, 0x11281410,
            0x4433471b, 0xc43ac722, 0x3c3b3f23, 0x43bb46a3, 0x443d4725, 0x643b6723, 0x463b4923, 0xaac1ada9,
            0xddcce0b4, 0xd5d4d8bc, 0x5dd460bc, 0xddd6e0be, 0xdd54e03c, 0xfdd500bc, 0xdfd4e2bc, 0x445b4743,
            0x77667a4e, 0xf76dfa55, 0x6f6e7256, 0x76ee79d6, 0x77707a58, 0x976e9a56, 0x796e7c56, 0xddf4e0dc,
            0x110013e8, 0x910793ef, 0x09080bf0, 0x130815f0, 0x310833f0, 0x10881370, 0x110a13f2, 0x778e7a76
        };

        private int seconds;
        private uint buttonsHeld;
        private ulong mac;
        private uint magic;

        public DateTimeSearcher(int seconds, uint buttonsHeld, ulong mac, uint magic)
        {
            this.seconds = seconds;
            this.buttonsHeld = buttonsHeld;
            this.mac = mac;
            this.magic = magic;
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
