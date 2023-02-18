using System;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
	public partial class DoubleJumpsForm : Form
	{
		private byte[] tilePatternNoMini = new byte[] { 4, 3, 3, 3, 3, 3, 5, 3 };
		private byte[] tilePatternMini = new byte[] { 4, 5, 5, 3, 5, 5, 5, 5 };
		private int[] doubleJumpCountsMiniLow = new int[] { 1, 0, 2, 2, 0, 1, 1, 4 };
		private int[] doubleJumpCountsMiniHigh = new int[] { 4, 3, 5, 7, 5, 4, 6, 7 };
		private int[] doubleJumpCountsNoMini = new int[] { 5, 7, 1, 5, 6, 6, 7, 4 };

		Settings settings;
		bool fromTimeFinder;

		public DoubleJumpsForm(Settings settings, bool patternFromSettings)
		{
			InitializeComponent();

			this.settings = settings;
			fromTimeFinder = patternFromSettings;

			// Set controls
			if (patternFromSettings)
			{
				lblTime.Text = settings.dt.ToLongDateString() + " " + settings.dt.ToLongTimeString();
				SeedInitParams sip = new SeedInitParams(settings.MAC, settings.dt);
				sip.Buttons = settings.buttonsHeld;
				new SystemSeedInitParams(settings.magic).SetSeedParams(sip);
				int[] pattern = TilesFor12.getFirstRowPattern(sip.GetSeed(), 8);
				numPTile.Value = Array.IndexOf(pattern, 4) + 1;
				// We came from the time finder form, so switching mini would not be compatible with the displayed date/time.
				chkMini.Enabled = false;
			}
			else
			{
				numPTile.Value = 1;
				btnWrongPattern.Visible = lblTime.Visible = lblExpectedPattern.Visible = false;
				// don't need that empty space where the non-visible tiles and button are
				this.Height -= 35;
			}
			chkMini.Checked = settings.wantMini;

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
				lblDJCount.Text = doubleJumpCountsMiniLow[tilePosition] + ", " + doubleJumpCountsMiniHigh[tilePosition] + ", " +
					(doubleJumpCountsMiniLow[tilePosition] + 8) + ", or " + (doubleJumpCountsMiniHigh[tilePosition] + 8);
			else
				lblDJCount.Text = "not " + doubleJumpCountsNoMini[tilePosition] + " or " + (doubleJumpCountsNoMini[tilePosition] + 8);
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

			MainForm.updateSettings(settings.wantMini, settings.buttonsHeld);
		}

		private void btnWrongPattern_Click(object sender, EventArgs e)
		{
			MessageBox.Show("It is normal to have to set the time and boot the game multiple times before getting the expected pattern!\n\n" +
				"If you can't get this displayed pattern, it might be because your system used a different magic, or you missed the target time.\n" +
				"Go back to the magic finder window, enter the date/time for this pattern, and type in the pattern that you actually got.\n" +
				"1) If it finds a magic, then your system was using a different magic than the one you found the first time. Either try again to get the desired magic and pattern, or calculate a new date/time based on this other magic.\n" +
				"2) If it does not find a magic, try changing the seconds by +/-1. If that works, then you missed the target time when starting the game.\n" +
				"3) If it still doesn't find a magic, ... you probably messed up somewhere else. If you're confident you did everything correctly, send me a video of you showing the MAC address on your system, setting the date/time, opening NSMB, and showing the tile pattern in 1-2, along with the settings.bin file.",
				"What to do if your tile pattern doesn't match");
		}
	}
}
