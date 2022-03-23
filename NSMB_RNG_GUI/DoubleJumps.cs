using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
    public partial class DoubleJumpsForm : Form
    {
        private byte[] tilePatternNoMini = new byte[] { 4, 3, 3, 3, 3, 3, 5, 3 };
        private byte[] tilePatternMini = new byte[] { 4, 5, 5, 3, 5, 5, 5, 5 };
        int[] doubleJumpCountsNoMini = new int[] { 4, 6, 2, 3, 3, 4, 1, 2 };
        int[] doubleJumpCountsMini1 = new int[] { 1, 4, 1, 0, 2, 2, 0, 0 };
        int[] doubleJumpCountsMini2 = new int[] { 6, 7, 4, 3, 5, 7, 5, 3 };

        Settings settings;
        bool fromTimeFinder;

        public DoubleJumpsForm(Settings settings, bool patternFromSettings)
        {
            InitializeComponent();

            chkMini.Checked = settings.wantMini;
            lblTime.Text = settings.dt.ToLongDateString() + " " + settings.dt.ToLongTimeString();

            this.settings = settings;
            fromTimeFinder = patternFromSettings;

            if (patternFromSettings)
            {
                SeedInitParams sip = new SeedInitParams(settings.MAC, settings.dt);
                new SystemSeedInitParams(settings.magic).SetSeedParams(sip);
                byte[][] pattern = TilesFor12.calculateTileRows(sip.GetSeed());

                tileDisplay1.update(pattern[0]);
                tileDisplay2.update(pattern[1]);
                numSTile.Value = Array.IndexOf<byte>(pattern[0], 4) + 1;
            }
            else
            {
                // todo
            }
        }

        private void DoubleJumpsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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
    }
}
