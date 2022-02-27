using System;
using System.Collections.Generic;
using System.IO;

using NSMB_RNG;

const string PATH_SETTINGS = "settings.bin";

ulong MAC = 0;

const string MAIN_MENU = "--- Main menu ---\n" +
    "0) Quit\n" +
    "1) Input MAC address\n" +
    "2) Choose or calculate a magic\n" +
    "3) Find a date/time for good seed\n" +
    "Select an option: ";

bool loadSettings()
{
    if (File.Exists(PATH_SETTINGS))
    {
        using (FileStream fs = File.OpenRead(PATH_SETTINGS))
        {
            byte[] buffer = new byte[8];
            int version = fs.ReadByte();
            if (version == 0)
            {
                int count = fs.Read(buffer, 0, 6);
                if (count == 6)
                    MAC = BitConverter.ToUInt64(buffer);
            }
        }
    }

    return MAC != 0;
}
void saveSettings()
{
    using (FileStream fs = File.Open(PATH_SETTINGS, FileMode.Create))
    {
        fs.WriteByte(0); // version
        fs.Write(BitConverter.GetBytes(MAC), 0, 6);
    }
}

void findSeedParams()
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
    SeedInitParams sip = new SeedInitParams(MAC, new DateTime(2000, 1, 1, 0, 0, 16));
    InitSeedSearcher iss = new InitSeedSearcher(sip, values);
    iss.secondsRange = 2;
    List<SeedInitParams> seedParams = iss.FindSeeds();
    for (int i = 0; i < seedParams.Count; i++)
    {
        Console.WriteLine(SystemSeedInitParams.GetMagic(seedParams[i]).ToString("x"));
    }
    Console.WriteLine("done");
}

int getUserMenuSelection(string menu, int maxOption)
{
    while (true)
    {
        Console.Write(menu);
        string? userInput = Console.ReadLine();
        if (!int.TryParse(userInput, out int menuOption))
            Console.WriteLine("ERROR: Input was not an integer.");
        else if (menuOption > maxOption || menuOption < 0)
            Console.WriteLine("ERROR: Input is not a valid selection.");
        else
            return menuOption;
    }
}

int main()
{
    Console.WriteLine("Welcome to NSMB_RNG.");
    Console.WriteLine("Please refer to README.txt for instructions on how to use this program.");
    Console.WriteLine();

    if (loadSettings())
        Console.WriteLine("MAC address loaded from file.\n");

    int menuOption = -1;
    while (menuOption != 0)
    {
        menuOption = getUserMenuSelection(MAIN_MENU, 3);
        // Input MAC address
        if (menuOption == 1)
        {
            ulong newMAC = ulong.MaxValue;
            while (newMAC == ulong.MaxValue)
            {
                Console.Write("Enter your system's MAC address, with our without separators: ");
                string? userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput) || (userInput.Length != 12 && userInput.Length != 17))
                {
                    Console.WriteLine("ERROR: Unexpected number of characters. (should be either 12 or 17)");
                    continue;
                }
                else if (userInput.Length == 17)
                {
                    string[] MACParts = userInput.Split(userInput[2]);
                    userInput = string.Join("", MACParts);
                }

                try { newMAC = Convert.ToUInt64(userInput, 16); }
                catch { Console.WriteLine("ERROR: Could not read MAC address."); }
            }
            MAC = newMAC;
            saveSettings();
            Console.WriteLine("MAC address set and saved to file.\n");
        }
    }

    return 0;
}

main();