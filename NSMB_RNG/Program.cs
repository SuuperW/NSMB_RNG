using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NSMB_RNG;

const string PATH_SETTINGS = "settings.bin";

ulong MAC = 0;
uint magic = 0;

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
            // We accept anything other than a no as a yes.
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !input.StartsWith('n'))
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

IEnumerable<string> tableFileNames(int digitCount)
{
    int[] tileIDs = new int[digitCount];
    while (true)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < digitCount; i++)
            sb.Append(tileIDs[i].ToString());
        yield return sb.ToString();

        int index = tileIDs.Length - 1;
        tileIDs[index]++;
        while (tileIDs[index] > 5)
        {
            tileIDs[index] = 0;
            index--;
            if (index < 0)
                yield break;
            tileIDs[index]++;
        }
    }
}

int main()
{
    string oldTable = "C:/tmp/data/init12/";
    string newTable = "C:/tmp/data/table3/";
    foreach (string dn in tableFileNames(3))
    {
        Directory.CreateDirectory(newTable + dn);
        string subDirName = "";
        foreach (string fn in tableFileNames(4))
        {
            FileStream ifs = File.OpenRead(oldTable + dn + "/" + fn);
            string sdn = fn.Substring(0, 2);
            string sfn = fn.Substring(2, 2);
            if (sdn != subDirName)
            {
                Directory.CreateDirectory(newTable + dn + "/" + sdn);
                subDirName = sdn;
            }
            FileStream ofs = File.Open(newTable + dn + "/" + sdn + "/" + sfn, FileMode.Create);

            byte[] buffer = new byte[4];
            int rCount = ifs.Read(buffer, 0, 4);
            uint last = 0;
            while (rCount != 0)
            {
                uint v = BitConverter.ToUInt32(buffer);
                uint m5 = v % 5;
                // If mod 5 is 2, we won't include it in the new table.
                // This value will be found using the new table because v + 0x33333333*4 must also exist in this file.
                // When reading the new table, any value greater than 0x33333333*4 will count as also being that value minus 0x33...
                // That new value (which newValue % 5 == 2) will then be given to reverseStep, which will determine if it should be there.
                if (m5 == 3)
                {
                    v = v / 5;
                    uint diff = v - last;
                    last = v;
                    // Since we divided by 5, we have two spare bits to indicate the size in bytes of this diff.
                    // This could mean we only use 2 or 3 bytes for each value, but we are going to use 7z to compress the data.
                    // 7z actually produces smaller files when each value is 4 bytes than when each value is variable size (or constant 3 bytes)
                    ofs.Write(BitConverter.GetBytes(diff), 0, 4);
                }

                rCount = ifs.Read(buffer, 0, 4);
            }

            ifs.Close();
            ofs.Close();
        }
    }

    return 0;
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
        // Choose/find magic
        else if (menuOption == 2)
        {
            if (MAC == 0)
            {
                Console.WriteLine("You must set a MAC address first.");
                continue;
            }

            DateTime dt = getDateTimeFromUser();

            // choosing system is not yet implemnted

            // Find a magic
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
                    // We accept anything other than a no as a yes.
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input) || !input.StartsWith('n'))
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
                    Console.WriteLine("One magic found and saved: " + magic.ToString("x") + "\n");
                }
                // If there are more than one, we cannot know which is correct.
                else if (seedParams.Count > 1)
                {
                    Console.WriteLine("More than one possible magic was found. There is no way to know without more information which is correct.");
                    Console.WriteLine("Since this is a rare occurence, NSMB_RNG will not attempt to find the correct one. Set another date/time or use another tile sequence with the same date/time and try again.\n");
                }
            }
        }
    }

    return 0;
}

main();