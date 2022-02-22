using System.IO;

using NSMB_RNG;

uint LCRNG_NSMB(uint v)
{
    ulong a = ((ulong)0x0019660D * v + 0x3C6EF35F);
    return (uint)(a + (a >> 32));
}

int main()
{
    // Read initals.bin
    FileStream fs = File.OpenRead("initialValues.bin");
    byte[] data = new byte[1024 * 1];
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

    // Find the seed params!
    SeedInitParams sip = new SeedInitParams(0x40f407f7d421, new DateTime(2000, 1, 1, 0, 0, 16));
    InitSeedSearcher iss = new InitSeedSearcher(sip, values);
    iss.secondsRange = 2;
    List<SeedInitParams> seedParams = iss.FindSeeds();
    for (int i = 0; i < seedParams.Count; i++)
    {
        Console.WriteLine(SystemSeedInitParams.GetMagic(seedParams[i]).ToString("x"));
    }
    Console.WriteLine("done");

    return 0;
}

main();