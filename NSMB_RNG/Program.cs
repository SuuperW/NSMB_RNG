using NSMB_RNG;

uint LCRNG_NSMB(uint v)
{
    ulong a = ((ulong)0x0019660D * v + 0x3C6EF35F);
    return (uint)(a + (a >> 32));
}

int main()
{
    // 0x31D44BA9
    // 2010 JAN 01
    DateTime dt = new DateTime(2010, 1, 1, 0, 0, 0);
    //DateTime dt = new DateTime(2012, 3, 7, 14, 25, 45); // 2012-MAR-07 14:25:45:000
    Console.WriteLine(dt.ToLongDateString());

    ulong macBizHawk = 0x0009BF0E4916;
    ulong macDeSmuME = 0x0009BF123456;

    SeedInitParams seedParams = new SeedInitParams(macBizHawk, dt);
    InitSeedSearcher searcher = new InitSeedSearcher(seedParams, 0x31D44BA9);
    searcher.minSeconds = dt.Second;
    searcher.maxSeconds = dt.Second;

    SeedInitParams result = searcher.FindSeed();
    if (result == null)
        Console.WriteLine("No match found.");
    else
    {
        Console.WriteLine("timer0: " + result.Timer0);
        Console.WriteLine("vcount: " + result.VCount);
        Console.WriteLine("vframe: " + result.VFrame);
    }

    return 0;
}

main();