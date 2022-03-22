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

        Dictionary<string, string[]> systems;
        List<uint> knownMagics
        {
            get
            {
                if (systems.ContainsKey(cbxSystem.Text))
                    return strArrayToMagicList(systems[cbxSystem.Text]);
                else
                    return new List<uint>();
            }
        }
        List<int[]> knownMagicPatterns = new List<int[]>();

        TilesFor12.SeedFinder? seedFinder = null;
        bool SeedFinderReady => seedFinder != null && seedFinder.isReady;

        private DateTime dt => dtpDate.Value.Add(dtpTime.Value.TimeOfDay);

        bool isLoaded = false;
        public MainForm()
        {
            InitializeComponent();

            // load files
            settings = Settings.loadSettings();
            if (File.Exists("systems.json"))
            {
                using (FileStream fs = File.OpenRead("systems.json"))
                    systems = JsonSerializer.Deserialize<Dictionary<string, string[]>>(fs) ?? new Dictionary<string, string[]>();
            }
            else
                systems = new Dictionary<string, string[]>();
            foreach (string key in systems.Keys)
                cbxSystem.Items.Add(key);
            if (File.Exists("otherMagics.json"))
            {
                using (FileStream fs = File.OpenRead("otherMagics.json"))
                    systems.Add("other", JsonSerializer.Deserialize<string[]>(fs) ?? new string[0]);
            }
            else
                systems.Add("other", new string[0]);

            // initialize controls
            txtMAC.Text = settings.MAC.ToString("X").PadLeft(12, '0');
            chkMini.Checked = settings.wantMini;
            cbxSystem.SelectedItem = settings.systemName;
            dtpDate.Value = settings.dt.Date;
            dtpTime.Value = settings.dt;

            isLoaded = true;
        }

        private List<uint> strArrayToMagicList(string[] strArray)
        {
            List<uint> magicList = new List<uint>();
            foreach (string str in strArray)
                magicList.Add(Convert.ToUInt32(str, 16));
            return magicList;
        }

        private void updateMagicPatterns()
        {
            knownMagicPatterns = new List<int[]>();
            foreach (uint magic in knownMagics)
            {
                SeedInitParams sip = new SeedInitParams(settings.MAC, dt);
                new SystemSeedInitParams(magic).SetSeedParams(sip);
                knownMagicPatterns.Add(TilesFor12.getFirstRowPattern(sip.GetSeed()));
            }
        }

        private void setWorkStatus(string str)
        {
            Action a = () =>
            {
                int oldWidth = lblWorkStatus.Width;
                lblWorkStatus.Text = str;
                lblWorkStatus.Location = new Point(lblWorkStatus.Location.X + oldWidth - lblWorkStatus.Width, lblWorkStatus.Location.Y);

                progressBar.Visible = lblWorkStatus.Visible = !string.IsNullOrEmpty(str);
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

        private void dirtySettings(bool save = true)
        {
            if (isLoaded)
                settings.saveSettings();

            lblMatch.Visible = false;
            btnTimeFinder.Visible = false;
            txtSecondRow.Enabled = false;

            tileDisplay2.update("");
            tileDisplay3.update("");
            tileDisplay4.update("");
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
            dirtySettings();
        }

        private void chkMini_CheckedChanged(object sender, EventArgs e)
        {
            settings.wantMini = chkMini.Checked;
            dirtySettings();
        }
        private void txtFirst7_Enter(object sender, EventArgs e)
        {
            updateMagicPatterns();
        }

        private void txtFirst7_TextChanged(object sender, EventArgs e)
        {
            if (sender != txtSecondRow) txtSecondRow.Enabled = false;

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
                btnTimeFinder.Visible = true;
            }
            else
            {
                btnTimeFinder.Visible = false;
                if (matches.Count == 0)
                {
                    if (userPattern.Count < 7)
                        lblMatch.Text = "No matching known magics. Enter all 7 tiles.";
                    else
                    {
                        lblMatch.Text = "Enter 11 tiles from second row.";
                        txtSecondRow.Enabled = true;
                        txtFirst7.Enabled = false;
                        createSeedFinder(userPattern);
                    }
                    lblMatch.Visible = true;
                }
                else
                    lblMatch.Visible = false;
            }
        }

        private void txtSecondRow_TextChanged(object sender, EventArgs e)
        {
            // Display+get pattern
            List<int> userPattern = tileDisplay2.update(txtSecondRow.Text);

            // Do we need to re-initialize the seed finder?
            if (seedFinder != null && seedFinder.isComplete)
                txtFirst7_TextChanged(sender, e);

            // Find seeds and magic
            if (userPattern.Count == 11 && SeedFinderReady)
            {
                txtSecondRow.Enabled = false;
                performMagicSearch(userPattern.ToArray());
            }
            else
            {
                tileDisplay3.update("");
                tileDisplay4.update("");
            }
        }

        private void createSeedFinder(List<int> userPattern)
        {
            seedFinder = null; // This ensures it isn't seen as "ready" between now and initing the new one.

            Action seedFinderReady = () =>
            {
                Invoke(() => {
                    if (seedFinder == null || seedFinder.error)
                    {
                        setMatchText("Failed to load lookup data.");
                        setWorkStatus("");
                        txtSecondRow.Enabled = false;
                        seedFinder = null;
                    }
                    else
                    {
                        setMatchText("Lookup complete. Enter second row of tiles.");
                        setWorkStatus("");
                    }

                    txtFirst7.Enabled = true;
                    if (txtSecondRow.Enabled)
                        txtSecondRow_TextChanged(progressBar, new EventArgs());
                });
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

                // Find seeds
                setWorkStatus("Finding seeds...");
                List<uint> seeds = seedFinder.calculatePossibleSeeds(secondRow);
                if (seeds.Count == 0)
                    setMatchText("No seeds found. Verify that you entered the correct tiles.");
                else
                {
                    // Find magic
                    setWorkStatus("Finding magics...");
                    SeedInitParams sip = new SeedInitParams(settings.MAC, dt);
                    InitSeedSearcher iss = new InitSeedSearcher(sip, seeds);
                    List<SeedInitParams> foundParams = iss.FindSeeds();
                    if (foundParams.Count == 0)
                        setMatchText("No magic found. Verify that you entered the correct tiles, MAC address, date, and time.");
                    // Expected result: only 1 params found. Save the magic.
                    else if (foundParams.Count == 1)
                    {
                        settings.magic = SystemSeedInitParams.GetMagic(foundParams[0]);
                        settings.saveSettings();
                        // Save this magic so it can be used again later.
                        string[] newOthers = new string[systems["other"].Length + 1];
                        Array.Copy(systems["other"], newOthers, systems["other"].Length);
                        newOthers[newOthers.Length - 1] = settings.magic.ToString("X");
                        systems["other"] = newOthers;
                        using (FileStream fs = File.Open("otherMagics.json", FileMode.Create))
                            JsonSerializer.Serialize<string[]>(fs, systems["other"]);
                        // Display results
                        setMatchText("Found and saved magic. Expected tile pattern shown.");
                        Invoke(() => displayExpectedPattern());
                    }
                    // If there are more than one, we cannot know which is correct.
                    else if (foundParams.Count > 1)
                    {
                        setMatchText("Multiple magics. Choose system 'temp' and try another tile pattern.");
                        // Save magic and slightly-varied magics
                        string[] magics = new string[foundParams.Count * 9];
                        int mID = 0;
                        foreach (SeedInitParams p in foundParams)
                        {
                            for (int t0 = -1; t0 <= 1; t0++)
                                for (int vc = -1; vc <= 1; vc++)
                                {
                                    SeedInitParams newParams = new SeedInitParams(p);
                                    newParams.Timer0 += (ushort)t0;
                                    newParams.VCount += (ushort)vc;
                                    magics[mID] = SystemSeedInitParams.GetMagic(newParams).ToString("X");
                                    mID++;
                                }
                        }
                        systems["temp"] = magics;
                        if (!cbxSystem.Items.Contains("temp"))
                            Invoke(() => cbxSystem.Items.Add("temp"));
                    }
                }

                Invoke(() => txtSecondRow.Enabled = true);
                setWorkStatus("");
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
            dirtySettings();
        }
        private void dtpDateTime_ValueChanged(object sender, EventArgs e)
        {
            dirtySettings(false);
        }

        private void cbxSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.systemName = cbxSystem.Text;
            dirtySettings();
        }

    }
}
