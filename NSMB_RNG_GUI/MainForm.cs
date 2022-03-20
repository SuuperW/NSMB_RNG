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
        Bitmap[] tiles = new Bitmap[] { Properties.Resources.tileB, Properties.Resources.tileE, Properties.Resources.tileI,
            Properties.Resources.tileC, Properties.Resources.tileP, Properties.Resources.tileS };

        Dictionary<string, List<uint>> systems;
        List<int[]> knownMagicPatterns = new List<int[]>();

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
            // Display+get pattern
            string upper = txtFirst7.Text.ToUpper();
            bool inputIsValid = true;
            int[] userPattern = new int[7];
            for (int i = 0; i < upper.Length; i++)
            {
                int index = Array.IndexOf(TilesFor12.tileLetters, upper[i]);
                if (index == -1 || i >= 7)
                {
                    pbxFirst[i].BackgroundImage = null;
                    pbxFirst[i].Visible = true;
                    for (int j = i + 1; j < pbxFirst.Length; j++)
                        pbxFirst[j].Visible = false;
                    inputIsValid = false;
                    break;
                }
                else
                {
                    pbxFirst[i].BackgroundImage = tiles[index];
                    pbxFirst[i].Visible = true;
                    userPattern[i] = index;
                }
            }

            lblFirstTooLong.Visible = upper.Length >= pbxFirst.Length;
            for (int i = upper.Length; i < pbxFirst.Length; i++)
                pbxFirst[i].Visible = false;

            // Compare with known magics
            if (inputIsValid)
            {
                List<int> matches = new List<int>();
                for (int patternIndex = 0; patternIndex < knownMagicPatterns.Count; patternIndex++)
                {
                    int[] pattern = knownMagicPatterns[patternIndex];
                    bool match = true;
                    for (int i = 0; i < upper.Length && i < userPattern.Length; i++)
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
                if (matches.Count == 1)
                {
                    lblMatch.Text = "Magic found.";
                    lblMatch.Visible = true;
                }
                else if (matches.Count == 0)
                {
                    lblMatch.Text = "No matching known magics. Enter all tiles.";
                    lblMatch.Visible = true;
                }
                else
                    lblMatch.Visible = false;
            }
            else
            {
                lblMatch.Visible = false;
            }
        }

        private void dtpDateTime_Leave(object sender, EventArgs e)
        {
            settings.dt = dt;
            if (isLoaded)
                settings.saveSettings();
        }

    }
}
