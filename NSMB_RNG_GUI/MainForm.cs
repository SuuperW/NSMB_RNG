using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text.Json;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
    public partial class MainForm : Form
    {
        Settings settings;

        Dictionary<string, List<uint>> systems;
        List<uint> knownMagics
        {
            get
            {
                if (systems.ContainsKey(cbxSystem.Text))
                    return systems[cbxSystem.Text];
                else
                    return new List<uint>();
            }
        }
        List<int[]> knownMagicPatterns = new List<int[]>();

        TilesFor12.SeedFinder? seedFinder = null;

        private DateTime dt => dtpDate.Value.AddHours(dtpTime.Value.Hour).AddMinutes(dtpTime.Value.Minute).AddSeconds(dtpTime.Value.Second);

        bool isLoaded = false;
        public MainForm()
        {
            InitializeComponent();

            // load files
            settings = Settings.loadSettings();
            if (File.Exists("systems.json"))
            {
                using (FileStream fs = File.OpenRead("systems.json"))
                {
                    Dictionary<string, string[]>? systemsStrArray = JsonSerializer.Deserialize<Dictionary<string, string[]>>(fs);
                    systems = new Dictionary<string, List<uint>>();
                    if (systemsStrArray != null)
                    {
                        foreach (var kvp in systemsStrArray)
                        {
                            systems.Add(kvp.Key, new List<uint>());
                            foreach (string str in kvp.Value)
                                systems[kvp.Key].Add(Convert.ToUInt32(str, 16));
                        }
                    }
                }
            }
            else
                systems = new Dictionary<string, List<uint>>();
            foreach (string key in systems.Keys)
                cbxSystem.Items.Add(key);

            // initialize controls
            txtMAC.Text = settings.MAC.ToString("X").PadLeft(12, '0');
            chkMini.Checked = settings.wantMini;
            cbxSystem.SelectedIndex = 0;
            dtpDate.Value = settings.dt.Date;
            dtpTime.Value = settings.dt;

            isLoaded = true;
        }

        private void updateMagicPatterns()
        {
            knownMagicPatterns = new List<int[]>();
            if (cbxSystem.SelectedIndex != 0) // 0 is "other"
            {
                List<uint> magics = systems[cbxSystem.Text];
                foreach (uint magic in magics)
                {
                    SeedInitParams sip = new SeedInitParams(settings.MAC, dt);
                    new SystemSeedInitParams(magic).SetSeedParams(sip);
                    knownMagicPatterns.Add(TilesFor12.getFirstRowPattern(sip.GetSeed()));
                }
            }
        }

        private void setWorkStatus(string str)
        {
            Action a = () =>
            {
                int oldWidth = lblWorkStatus.Width;
                lblWorkStatus.Text = str;
                lblWorkStatus.Location = new Point(lblWorkStatus.Location.X + oldWidth - lblWorkStatus.Width, lblWorkStatus.Location.Y);

                lblWorkStatus.Visible = !string.IsNullOrEmpty(str);
            };
            if (InvokeRequired)
                Invoke(a);
            else
                a();
        }
        private void setMatchText(string str)
        {
            Action a = () => { lblMatch.Text = str; };
            if (InvokeRequired)
                Invoke(a);
            else
                a();
        }

        private void txtMAC_TextChanged(object sender, EventArgs e)
        {
            pbxMAC.Visible = true;

            string userInput = txtMAC.Text;
            if (string.IsNullOrEmpty(userInput) || (userInput.Length != 12 && userInput.Length != 17))
            {
                pbxMAC.BackColor = Color.Red;
                return;
            }
            else if (userInput.Length == 17)
            {
                string[] MACParts = userInput.Split(userInput[2]);
                userInput = string.Join("", MACParts);
            }

            ulong newMAC;
            try { newMAC = Convert.ToUInt64(userInput, 16); }
            catch
            {
                pbxMAC.BackColor = Color.Red;
                return;
            }

            pbxMAC.BackColor = Color.Green;
            settings.MAC = newMAC;
            if (isLoaded)
                settings.saveSettings();
        }

        private void chkMini_CheckedChanged(object sender, EventArgs e)
        {
            settings.wantMini = chkMini.Checked;
            if (isLoaded)
                settings.saveSettings();
        }
        private void txtFirst7_Enter(object sender, EventArgs e)
        {
            updateMagicPatterns();
        }

        private void txtFirst7_TextChanged(object sender, EventArgs e)
        {
            txtSecondRow.Enabled = false;

            // Display+get pattern
            List<int> userPattern = tileDisplay1.update(txtFirst7.Text);

            // Compare with known magics
            List<int> matches = new List<int>();
            for (int patternIndex = 0; patternIndex < knownMagicPatterns.Count; patternIndex++)
            {
                int[] pattern = knownMagicPatterns[patternIndex];
                bool match = true;
                for (int i = 0; i < userPattern.Count; i++)
                {
                    if (pattern[i] != userPattern[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    matches.Add(patternIndex);
            }
            // Display matching results
            if (userPattern.Count > 0 && matches.Count == 1)
            {
                lblMatch.Text = "Magic found.";
                lblMatch.Visible = true;
                settings.magic = knownMagics[matches[0]];
                settings.saveSettings();
            }
            else if (matches.Count == 0)
            {
                if (userPattern.Count < 7)
                    lblMatch.Text = "No matching known magics. Enter all 7 tiles.";
                else
                {
                    lblMatch.Text = "Enter 11 tiles from second row.";
                    txtSecondRow.Enabled = true;
                    createSeedFinder(userPattern);
                }
                lblMatch.Visible = true;
            }
            else
                lblMatch.Visible = false;
        }
        private void txtSecondRow_TextChanged(object sender, EventArgs e)
        {
            // Display+get pattern
            List<int> userPattern = tileDisplay2.update(txtSecondRow.Text);

            // Find seeds and magic
            if (userPattern.Count == 11)
            {
                performMagicSearch(userPattern.ToArray());
            }
        }

        private void createSeedFinder(List<int> userPattern)
        {
            progressBar.Visible = true;

            Action seedFinderReady = () =>
            {
                if (seedFinder == null || seedFinder.error)
                {
                    setWorkStatus("Failed to load lookup data.");
                    seedFinder = null;
                }
                else
                    setWorkStatus("Lookup complete.");
                Invoke(() => progressBar.Visible = false);
            };
            setWorkStatus("Loading lookup data...");
            Thread t = new Thread(() => { 
                seedFinder = new TilesFor12.SeedFinder(userPattern.ToArray());
                // If it didn't have to download, then it ran syncronously.
                if (!seedFinder.isReady)
                {
                    seedFinder.DownloadProgress += (progress) =>
                    {
                        if (progress < 100)
                            setWorkStatus("Downloading lookup... " + Math.Round(progress).ToString() + "%");
                        else
                            setWorkStatus("Extracting files...");
                    };
                    seedFinder.Ready += seedFinderReady;
                }
                else
                    seedFinderReady();
            });
            t.Start();
        }

        private void performMagicSearch(int[] secondRow)
        {
            Thread t = new Thread(() =>
            {
                if (seedFinder == null)
                {
                    setMatchText("Error: Idk."); // it should never happen
                    return;
                }

                Invoke(() => progressBar.Visible = true);

                // Find seeds
                setMatchText("Finding seeds...");
                List<uint> seeds = seedFinder.calculatePossibleSeeds(secondRow);
                if (seeds.Count == 0)
                    setMatchText("No seeds found. Verify that you entered the correct tiles.");
                else
                {
                    // Find magic
                    setMatchText("Finding magics...");
                    SeedInitParams sip = new SeedInitParams(settings.MAC, dt);
                    InitSeedSearcher iss = new InitSeedSearcher(sip, seeds);
                    List<SeedInitParams> seedParams = iss.FindSeeds();
                    if (seedParams.Count == 0)
                        setMatchText("No magic found. Verify that you entered the correct tiles, MAC address, date, and time.");
                    // Expected result: only 1 params found. Save the magic.
                    else if (seedParams.Count == 1)
                    {
                        settings.magic = SystemSeedInitParams.GetMagic(seedParams[0]);
                        settings.saveSettings();
                        setMatchText("Found magic. Expected tile pattern shown.");
                        Invoke(() => displayExpectedPattern());
                    }
                    // If there are more than one, we cannot know which is correct.
                    else if (seedParams.Count > 1)
                    {
                        setMatchText("Multiple magics found there's no way to know which one is correct.");
                    }
                }

                Invoke(() => progressBar.Visible = false);
            });
            t.Start();
        }

        private void displayExpectedPattern()
        {
            SeedInitParams sip = new SeedInitParams(settings.MAC, dt);
            new SystemSeedInitParams(settings.magic).SetSeedParams(sip);
            byte[][] pattern = TilesFor12.calculateTileRows(sip.GetSeed());

            tileDisplay1.update(pattern[0]);
            tileDisplay2.update(pattern[1]);
            tileDisplay3.update(pattern[2]);
            tileDisplay4.update(pattern[3]);
        }

        private void dtpDateTime_Leave(object sender, EventArgs e)
        {
            settings.dt = dt;
            if (isLoaded)
                settings.saveSettings();
        }

    }
}
