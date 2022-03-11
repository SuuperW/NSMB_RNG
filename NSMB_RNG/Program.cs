using System;
using System.Collections.Generic;
using System.IO;

using NSMB_RNG;

const string PATH_SETTINGS = "settings.bin";

ulong MAC = 0;
uint magic = 0;

const string MAIN_MENU = "--- Main menu ---\n" +
    "0) Quit\n" +
    "1) Input MAC address\n" +
    "2) Choose or calculate a magic\n" +
    "3) Find a date/time for good seed\n" +
    "4) Calculate tile pattern\n" +
    "Select an option: ";

bool loadSettings()
{
    if (File.Exists(PATH_SETTINGS))
    {
        using (FileStream fs = File.OpenRead(PATH_SETTINGS))
        {
            byte[] buffer = new byte[8];
            int version = fs.ReadByte();
            if (version == 1)
            {
                // MAC
                int count = fs.Read(buffer, 0, 6);
                if (count == 6)
                    MAC = BitConverter.ToUInt64(buffer);
                // magic
                count = fs.Read(buffer, 0, 4);
                if (count == 4)
                    magic = BitConverter.ToUInt32(buffer);
                else
                    throw new Exception("bad settings file");
            }
        }
    }

    return MAC != 0;
}
void saveSettings()
{
    using (FileStream fs = File.Open(PATH_SETTINGS, FileMode.Create))
    {
        fs.WriteByte(1); // version
        fs.Write(BitConverter.GetBytes(MAC), 0, 6);
        fs.Write(BitConverter.GetBytes(magic), 0, 4);
    }
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

DateTime getDateTimeFromUser()
{
    while (true)
    {
        Console.Write("Enter the date/time that RNG was initialized, in your local format: ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dt))
            Console.WriteLine("Bad date/time format.");
        else if (dt.Year < 2000 || dt.Year > 2099)
            Console.WriteLine("Year must be a valid DS year. (2000-2099)");
        else
        {
            // Show the long date/time to the user for verification.
            Console.WriteLine("You entered: " + dt.ToLongDateString() + " " + dt.ToLongTimeString());
            Console.Write("Is this correct? [y/n]: ");
            if (UI.AskYesNo())
                return dt;
        }
    }
}

List<SeedInitParams> getSeedInitParams(DateTime dt, List<uint> seeds)
{
    SeedInitParams sip = new SeedInitParams(MAC, dt);
    InitSeedSearcher iss = new InitSeedSearcher(sip, seeds);
    return iss.FindSeeds();
}

void menuSetMac()
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

void calculateMagic()
{
    DateTime dt = getDateTimeFromUser();
    List<uint>? seeds = TilesFor12.calculatePossibleSeeds();
    if (seeds != null)
    {
        Console.WriteLine("Looking for magics. This may take several seconds...");
        List<SeedInitParams> seedParams = getSeedInitParams(dt, seeds);
        // Handle case where no params were found
        while (seedParams.Count == 0)
        {
            Console.WriteLine("Failed to find a magic that results in one of the found seeds.");
            Console.WriteLine("This may be because you mis-typed the tiles, or because the date/time you entered was incorrect.");
            Console.Write("Do you want to try another date/time? [y/n]: ");
            if (UI.AskYesNo())
            {
                dt = getDateTimeFromUser();
                seedParams = getSeedInitParams(dt, seeds);
            }
            else
                break;
        }
        // Expected result: only 1 params found. Save the magic.
        if (seedParams.Count == 1)
        {
            magic = SystemSeedInitParams.GetMagic(seedParams[0]);
            saveSettings();
            Console.WriteLine("One magic found and saved: " + magic.ToString("x"));
            Console.WriteLine("It might be a good idea to confirm this magic by using option 4: Calculate tile pattern.\n");
        }
        // If there are more than one, we cannot know which is correct.
        else if (seedParams.Count > 1)
        {
            Console.WriteLine("More than one possible magic was found. (This is rare.) You will have to get another tile pattern.");
            Console.Write("Found magics: " + SystemSeedInitParams.GetMagic(seedParams[0]).ToString("x"));
            for (int i = 1; i < seedParams.Count; i++)
                Console.Write(", " + SystemSeedInitParams.GetMagic(seedParams[i]).ToString("x"));
            Console.WriteLine();
        }
    }

}

void menuCalculateTilePattern()
{
    DateTime dt = getDateTimeFromUser();
    SeedInitParams sip = new SeedInitParams(MAC, dt);
    new SystemSeedInitParams(magic).SetSeedParams(sip);
    uint seed = sip.GetSeed();

    TilesFor12.printTilesFromSeed(seed);
    Console.WriteLine("Note: Because magics can vary slightly between boots, you are not guaranteed to get this pattern every time.\n");
}

int main()
{
    Console.WriteLine("Welcome to NSMB_RNG.");
    Console.WriteLine("Please refer to README.txt for instructions on how to use this program.");
    Console.WriteLine();

    if (loadSettings())
    {
        if (magic != 0)
            Console.WriteLine("MAC and magic loaded from file.\n");
        else
            Console.WriteLine("MAC address loaded from file.\n");
    }

    int menuOption = -1;
    while (menuOption != 0)
    {
        menuOption = getUserMenuSelection(MAIN_MENU, 4);
        // Input MAC address
        if (menuOption == 1)
            menuSetMac();
        // Choose/find magic
        else if (menuOption == 2)
        {
            if (MAC == 0)
            {
                Console.WriteLine("You must set a MAC address first.\n");
                continue;
            }

            // choosing system is not yet implemnted

            calculateMagic();
        }

        else if (menuOption == 4)
        {
            if (MAC == 0 || magic == 0)
            {
                Console.WriteLine("You must set both a MAC address and a magic before using this option.\n");
                continue;
            }
            else
                menuCalculateTilePattern();
        }
    }

    return 0;
}

main();
