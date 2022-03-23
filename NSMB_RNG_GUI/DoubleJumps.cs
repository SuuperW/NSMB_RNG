﻿using System;
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
            lblTime.Text = settings.dt.ToLongDateString() + " " + settings.dt.ToLongTimeString();
            if (patternFromSettings)
            {
                SeedInitParams sip = new SeedInitParams(settings.MAC, settings.dt);
                new SystemSeedInitParams(settings.magic).SetSeedParams(sip);
                int[] pattern = TilesFor12.getFirstRowPattern(sip.GetSeed());
                numPTile.Value = Array.IndexOf(pattern, 4) + 1;
            }
            else
                numPTile.Value = 1;

            // The initial value (and thus max) is set to 9 in the designer.
            // This is because we need to guarantee that setting the value above actually changes the value,
            // so that numPTile_ValueChanged will be called.
            numPTile.Maximum = 8;
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

        private void numPTile_ValueChanged(object sender, EventArgs e)
        {
            tileDisplay1.update(getRow((int)numPTile.Value));
            // There are 27 tiles per row, so the second row will be offset by -3 mod 8 = +5.
            tileDisplay2.update(getRow(((int)numPTile.Value + 5) % 8));
        }
        private byte[] getRow(int PTileLocation)
        {
            byte[] patternSource = chkMini.Checked ? tilePatternMini : tilePatternNoMini;
            byte[] pattern = new byte[11];
            // First copy, start copying from PTileLocation
            PTileLocation--; // make it 0-based index
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
    }
}
