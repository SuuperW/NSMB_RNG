using System;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
    public partial class DoubleJumpsForm : Form
    {
        private byte[] tilePatternNoMini = new byte[] { 4, 3, 3, 3, 3, 3, 5, 3 };
        private byte[] tilePatternMini = new byte[] { 4, 5, 5, 3, 5, 5, 5, 5 };
        private int[] doubleJumpCountsNoMini = new int[] { 4, 6, 2, 3, 3, 4, 1, 2 };
        private int[] doubleJumpCountsMini1 = new int[] { 1, 4, 1, 0, 2, 2, 0, 0 };
        private int[] doubleJumpCountsMini2 = new int[] { 6, 7, 4, 3, 5, 7, 5, 3 };

        Settings settings;
        bool fromTimeFinder;

        public DoubleJumpsForm(Settings settings, bool patternFromSettings)
        {
            InitializeComponent();

            this.settings = settings;
            fromTimeFinder = patternFromSettings;

            // Set controls
            chkMini.Checked = settings.wantMini;
            if (patternFromSettings)
            {
                lblTime.Text = settings.dt.ToLongDateString() + " " + settings.dt.ToLongTimeString();
                SeedInitParams sip = new SeedInitParams(settings.MAC, settings.dt);
                new SystemSeedInitParams(settings.magic).SetSeedParams(sip);
                int[] pattern = TilesFor12.getFirstRowPattern(sip.GetSeed(), 8);
                numPTile.Value = Array.IndexOf(pattern, 4) + 1;
                // We came from the time finder form, so switching mini would not be compatible with the displayed date/time.
                chkMini.Enabled = false;
            }
            else
            {
                numPTile.Value = 1;
                lblTime.Visible = lblExpectedPattern.Visible = false;
            }

            // The initial value (and thus max) is set to 9 in the designer.
            // This is because we need to guarantee that setting the value above actually changes the value,
            // so that numPTile_ValueChanged will be called.
            numPTile.Maximum = 8;
        }

        private void DoubleJumpsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Exit the application if there isn't another open window.
            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (fromTimeFinder)
            {
                TimeFinder tfForm = new TimeFinder(settings);
                tfForm.Show();
            }
            else
            {
                MainForm mainForm = new MainForm();
                mainForm.Show();
            }
            this.Close();

        }

        private void numPTile_ValueChanged(object sender, EventArgs e)
        {
            // tiles
            int tilePosition = (int)numPTile.Value - 1; // make it 0-based index
            tileDisplay1.update(getRow(tilePosition));
            // There are 27 tiles per row, so the second row will be offset by -3 mod 8 = +5.
            tileDisplay2.update(getRow((tilePosition + 5) % 8));

            // double jump counts
            if (settings.wantMini)
                lblDJCount.Text = doubleJumpCountsMini1[tilePosition] + ", " + doubleJumpCountsMini2[tilePosition] + ", " +
                    (doubleJumpCountsMini1[tilePosition] + 8) + ", " + (doubleJumpCountsMini2[tilePosition] + 8);
            else
                lblDJCount.Text = "not " + doubleJumpCountsNoMini[tilePosition] + ", " + (doubleJumpCountsNoMini[tilePosition] + 8);
        }
        private byte[] getRow(int PTileLocation)
        {
            byte[] patternSource = settings.wantMini ? tilePatternMini : tilePatternNoMini;
            byte[] pattern = new byte[11];
            // First copy, start copying from PTileLocation
            int beginIndex = (8 - PTileLocation) % 8;
            int len = 8 - beginIndex;
            Array.Copy(patternSource, beginIndex, pattern, 0, len);
            // Remaining copies, start copying from index 0
            int dstIndex = len;
            while (dstIndex < 11)
            {
                len = (dstIndex + 8 > pattern.Length) ? (pattern.Length - dstIndex) : 8;
                Array.Copy(patternSource, 0, pattern, dstIndex, len);
                dstIndex += len;
            }

            return pattern;
        }

        private void chkMini_CheckedChanged(object sender, EventArgs e)
        {
            settings.wantMini = chkMini.Checked;
            // Update tile and double jump count display.
            numPTile_ValueChanged(sender, e);
            // If not using mini route, 7 and 8 double jumps will always work.
            lbl78.Visible = !chkMini.Checked;
            // This message is no longer true
            lblTime.Visible = lblExpectedPattern.Visible = false;
        }
    }
}
