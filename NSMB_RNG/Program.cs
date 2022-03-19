using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

using NSMB_RNG;

const string PATH_SETTINGS = "settings.bin";

ulong MAC = 0;
uint magic = 0;
bool wantMini = false;

const string MAIN_MENU = "--- Main menu ---\n" +
    "0) Quit\n" +
    "1) Input MAC address\n" +
    "2) Choose or calculate a magic\n" +
    "3) Find a date/time for good seed\n" +
    "4) Calculate tile pattern\n" +
    "5) Get desired number of double jumps for 1-1\n" +
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

// Returns a list of magics associated with this system, if there are any.
List<uint> chooseSystem()
{
    // read file
    FileStream fs = File.OpenRead("systems.json");
    Dictionary<string, string[]>? systems = JsonSerializer.Deserialize<Dictionary<string, string[]>>(fs);
    fs.Close();
    if (systems == null)
    {
        Console.WriteLine("Failed to read systems.json.");
        return new List<uint>();
    }

    // output list
    Console.Write("Enter your system type. Valid options are: ");
    StringBuilder sb = new StringBuilder();
    foreach (string systemName in systems.Keys)
        sb.Append(systemName).Append(", ");
    sb.Append("other");
    Console.WriteLine(sb.ToString());
    Console.WriteLine("Note: 3DS-like systems and virtual console are not supported, as their RNG initialization procedures are unknown.");

    // case-insensitive, and 'other' option
    Dictionary<string, string[]> systemsCaseInsensitive = new Dictionary<string, string[]>();
    foreach (var kvp in systems)
        systemsCaseInsensitive.Add(kvp.Key.ToLower(), kvp.Value);
    systemsCaseInsensitive.Add("other", new string[] { });

    // system selection
    string[] magicsArray;
    while (true)
    {
        Console.Write("System type: ");
        string? chosenSystem = Console.ReadLine();
        if (chosenSystem == null || !systemsCaseInsensitive.ContainsKey(chosenSystem.ToLower()))
            Console.WriteLine("Invalid system type.");
        else
        {
            magicsArray = systemsCaseInsensitive[chosenSystem.ToLower()];
            break;
        }
    }
    if (magicsArray.Length == 0)
        return new List<uint>();

    // create patterns
    List<uint> magics = new List<uint>();
    foreach (string str in magicsArray)
    {
        uint m = Convert.ToUInt32(str, 16);
        magics.Add(m);
    }
    return magics;
}
void getMagic(List<uint> knownMagics)
{
    DateTime dt = UI.GetDateTimeFromUser();

    // First 7 tiles, which might match a known magic.
    Console.WriteLine("\nEnter 1-2 as instructed in the README.txt file, and visually identify the first 7 randomized tiles in the first row of tiles.");
    Console.WriteLine("Refer to the tiles.png file for clarification, and for the tile names used by the program.");
    Console.WriteLine("----------------------------\n");
    int[] inputTiles = TilesFor12.getFirstSevenTiles();
    // Check if a known magic gives this tile pattern
    uint matchedMagic = findMatchingMagic(knownMagics, inputTiles, dt);
    if (matchedMagic != 0)
    {
        magic = matchedMagic;
        saveSettings();
        Console.WriteLine("Tile pattern matched with known magic. Magic saved.\n");
        return;
    }
    else if (knownMagics.Count != 0)
        Console.WriteLine("Tile pattern does not match any known magics. Magic must be calculated.");

    // No match with a known magic. Calculate the seed, then calculate magic based on seed.
    List<uint>? seeds = TilesFor12.calculatePossibleSeeds(inputTiles);
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
                dt = UI.GetDateTimeFromUser();
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
uint findMatchingMagic(List<uint> knownMagics, int[] first7Tiles, DateTime dt)
{
    foreach (uint m in knownMagics)
    {
        // create pattern
        SeedInitParams sip = new SeedInitParams(MAC, dt);
        new SystemSeedInitParams(m).SetSeedParams(sip);
        uint seed = sip.GetSeed();
        int[] pattern = TilesFor12.getFirstRowPattern(seed);

        // compare
        bool match = true;
        for (int i = 0; i < 7; i++)
        {
            if (pattern[i] != first7Tiles[i])
            {
                match = false;
                break;
            }
        }
        if (match)
            return m;
    }

    // No match
    return 0;
}

void menuFindGoodDateTime()
{
    // mini?
    Console.WriteLine("Do you want to attempt mini route? [y/n]: ");
    wantMini = UI.AskYesNo();

    // choose seconds
    int seconds = UI.GetUserMenuSelection("Enter the number of seconds you want to have between setting the date/time and loading the game: ", 999);

    // choose buttons
    StringBuilder buttonNames = new StringBuilder();
    foreach (var kvp in SeedInitParams.buttons)
        buttonNames.Append(kvp.Key).Append(' ');
    buttonNames.Length--;

    string? buttonsSelection = null;
    List<string> buttonsList = new List<string>();
    while (buttonsSelection == null)
    {
        Console.WriteLine("Enter the buttons you will hold during game start-up, separated by spaces, or leave blank for no buttons.");
        Console.WriteLine("Button names: " + buttonNames.ToString());
        Console.Write("Buttons to hold: ");
        while (buttonsSelection == null)
            buttonsSelection = Console.ReadLine();
        
        foreach (string s in buttonsSelection.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            string properName = s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
            if (SeedInitParams.buttons.ContainsKey(properName))
                buttonsList.Add(properName);
            else
            {
                buttonsList.Clear();
                buttonsSelection = null;
                break;
            }
        }
    }

    uint buttonsHeld = 0;
    foreach (string s in buttonsList)
        buttonsHeld |= SeedInitParams.buttons[s];

    // choose auto-second-increment option
    Console.Write("Do you want to automatically increment the seconds try again, in the event that no good date/time is found? [y/n]: ");
    bool autoIncrementSeconds = UI.AskYesNo();

    // thread count
    int threadCount = UI.GetUserMenuSelection("Number of threads to use (default " + Environment.ProcessorCount + "): ", 100);
    if (threadCount == 0)
        threadCount = Environment.ProcessorCount;

    // the big loop
    while (true)
    {
        DateTimeSearcher dts = new DateTimeSearcher(seconds, buttonsHeld, MAC, magic, wantMini);
        Console.WriteLine("Searching with seconds = " + seconds.ToString());
        DateTime dt = dts.findGoodDateTime(threadCount, true);
        Console.WriteLine();

        // Did we find a match?
        if (dt.Year >= 2000)
        {
            Console.WriteLine("Found a good date/time!");
            Console.WriteLine(dt.ToLongDateString() + " " + dt.ToLongTimeString());
            Console.WriteLine("Expected tile pattern: ");
            SeedInitParams sip = new SeedInitParams(MAC, dt);
            new SystemSeedInitParams(magic).SetSeedParams(sip);
            sip.Buttons = buttonsHeld;
            TilesFor12.printTilesFromSeed(sip.GetSeed());
            break;
        }
        else
        {
            if (autoIncrementSeconds)
                seconds++;
            else
            {
                Console.WriteLine("Done searching all possible date/times with the given seconds count. No matches found.");
                Console.WriteLine("Try again with another seconds count, or with different buttons held.");
                break;
            }
        }
    }
}

void menuCalculateTilePattern()
{
    DateTime dt = UI.GetDateTimeFromUser();
    SeedInitParams sip = new SeedInitParams(MAC, dt);
    new SystemSeedInitParams(magic).SetSeedParams(sip);
    uint seed = sip.GetSeed();

    TilesFor12.printTilesFromSeed(seed);
    Console.WriteLine("Note: Because magics can vary slightly between boots, you are not guaranteed to get this pattern every time.\n");
}

void menuDoubleJumps()
{
    Console.WriteLine("This assumes that you already have a good seed!");

    // mini?
    Console.WriteLine("Do you want to attempt mini route? [y/n]: ");
    wantMini = UI.AskYesNo();

    Console.WriteLine("1) Go to 1-2 as instructed in the README.txt file. You MUST pause.");
    Console.WriteLine("2) Enter the position (1-8) of the first 'P' tile in the first row of tiles.");
    Console.WriteLine("3) Quit directly to the main menu. Do not go to the overworld.");
    int tilePosition = UI.GetUserMenuSelection("'S' tile position: ", 8);
    // This array has indexes 0-8. [0] is equal to [8], of course.
    int[] doubleJumpCountsNoMini = new int[] { 2, 4, 6, 2, 3, 3, 4, 1, 2 };
    int[] doubleJumpCountsMini1 = new int[]  { 1, 0, 2, 2, 0, 0, 1, 4, 1 };
    int[] doubleJumpCountsMini2 = new int[]  { 4, 3, 5, 7, 5, 3, 6, 7, 4 };
    if (wantMini)
    {
        Console.WriteLine("\nYou can do " + doubleJumpCountsMini1[tilePosition] + " or " + doubleJumpCountsMini2[tilePosition] + " double jumps.");
        Console.WriteLine("Adding 8, 16, etc, to either of those numbers will also work.\n");
    }
    else
    {
        int djCount = doubleJumpCountsNoMini[tilePosition];
        Console.WriteLine("\nYou can do any number of double jumps except " + djCount + " (or " + (djCount + 8) + ", " + (djCount + 16) + ", etc.).");
        Console.WriteLine("Note: 7 and 8 double jumps will always work, regardless of tile position.\n");
    }
}

int main()
{
    Console.WriteLine("Welcome to NSMB_RNG.");
    Console.WriteLine("Please refer to README.txt for instructions on how to use this program.");
    Console.WriteLine();

    // Check for files
    if (!File.Exists("systems.json"))
    {
        Console.WriteLine("WARNING: Could not find file systems.json. File with no data automatically generated.\n");
        File.WriteAllText("systems.json", "{}");
    }

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
        menuOption = UI.GetUserMenuSelection(MAIN_MENU, 5);
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

            List<uint> knownMagics = chooseSystem();
            getMagic(knownMagics);
        }
        // find a good date/time
        else if (menuOption == 3)
        {
            if (MAC == 0 || magic == 0)
            {
                Console.WriteLine("You must set both a MAC address and a magic before using this option.\n");
                continue;
            }
            else
                menuFindGoodDateTime();
        }
        // display a tile pattern
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
        // double jumps
        else if (menuOption == 5)
        {
            menuDoubleJumps();
        }
    }

    return 0;
}

main();
