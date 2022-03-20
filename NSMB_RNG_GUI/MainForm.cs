using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
    public partial class MainForm : Form
    {
        Settings settings;
        PictureBox[] pbxFirst;
        PictureBox[] pbxSecond;
        Bitmap[] tiles = new Bitmap[] { Properties.Resources.tileB, Properties.Resources.tileE, Properties.Resources.tileI,
            Properties.Resources.tileC, Properties.Resources.tileP, Properties.Resources.tileS };

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

            pbxFirst = new PictureBox[] { pbxTile11, pbxTile12, pbxTile13, pbxTile14, pbxTile15, pbxTile16, pbxTile17, pbxTile1End };
            pbxSecond = new PictureBox[] { pbxTile21, pbxTile22, pbxTile23, pbxTile24, pbxTile25, pbxTile26, pbxTile27, pbxTile28, pbxTile29, pbxTile210, pbxTile211, pbxTile2End };

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
            int oldWidth = lblWorkStatus.Width;
            lblWorkStatus.Text = str;
            lblWorkStatus.Location = new Point(lblWorkStatus.Location.X + oldWidth - lblWorkStatus.Width, lblWorkStatus.Location.Y);
            
            lblWorkStatus.Visible = !string.IsNullOrEmpty(str);
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
            List<int> userPattern = parseTiles(txtFirst7.Text, pbxFirst);

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
                    setWorkStatus("Loading lookup data...");
                }
                lblMatch.Visible = true;
            }
            else
                lblMatch.Visible = false;
        }
        private void txtSecondRow_TextChanged(object sender, EventArgs e)
        {
            // Display+get pattern
            List<int> userPattern = parseTiles(txtSecondRow.Text, pbxSecond);

            // Find seeds and magic
            if (userPattern.Count == 11)
            {
                if (seedFinder == null)
                {
                    lblMatch.Text = "Error: Idk."; // it should never happen
                    return;
                }

                // Find seeds
                List<uint> seeds = seedFinder.calculatePossibleSeeds(userPattern.ToArray());
                if (seeds.Count == 0)
                {
                    lblMatch.Text = "No seeds found. Verify that you entered the correct tiles.";
                    return;
                }

                // Find magic
                SeedInitParams sip = new SeedInitParams(settings.MAC, dt);
                InitSeedSearcher iss = new InitSeedSearcher(sip, seeds);
                List<SeedInitParams> seedParams = iss.FindSeeds();
                if (seedParams.Count == 0)
                    lblMatch.Text = "No magic found. Verify that you entered the correct tiles, MAC address, date, and time.";
                // Expected result: only 1 params found. Save the magic.
                else if (seedParams.Count == 1)
                {
                    settings.magic = SystemSeedInitParams.GetMagic(seedParams[0]);
                    settings.saveSettings();
                    lblMatch.Text = "Found magic.";
                }
                // If there are more than one, we cannot know which is correct.
                else if (seedParams.Count > 1)
                {
                    lblMatch.Text = "Multiple magics found there's no way to know which one is correct.";
                }
            }
        }
        private List<int> parseTiles(string input, PictureBox[] pbxArray)
        {
            string upper = input.ToUpper();
            bool inputIsValid = true;
            int maxLength = pbxArray.Length - 1;
            List<int> userPattern = new List<int>(maxLength);
            int pi = 0;
            for (int i = 0; i < upper.Length; i++)
            {
                if (upper[i] == ' ')
                    continue;

                int index = Array.IndexOf(TilesFor12.tileLetters, upper[i]);
                if (index == -1 || pi >= maxLength)
                {
                    pbxArray[pi].BackgroundImage = null;
                    pbxArray[pi].Visible = true;
                    pi++;
                    inputIsValid = false;
                    break;
                }
                else
                {
                    pbxArray[pi].BackgroundImage = tiles[index];
                    pbxArray[pi].Visible = true;
                    userPattern.Add(index);
                    pi++;
                }
            }

            lblFirstTooLong.Visible = pi >= pbxArray.Length;
            for (int i = pi; i < pbxArray.Length; i++)
                pbxArray[i].Visible = false;

            if (inputIsValid) return userPattern;
            else return new List<int>();
        }

        private async void createSeedFinder(List<int> userPattern)
        {
            seedFinder = null;
            seedFinder = await TilesFor12.SeedFinder.Create(userPattern.ToArray());
        }

        private void dtpDateTime_Leave(object sender, EventArgs e)
        {
            settings.dt = dt;
            if (isLoaded)
                settings.saveSettings();
        }

    }
}
