using System;
using System.Drawing;
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

        public MainForm()
        {
            InitializeComponent();

            settings = Settings.loadSettings();
            txtMAC.Text = settings.MAC.ToString("X").PadLeft(12, '0');
            chkMini.Checked = settings.wantMini;
            cbxSystem.SelectedIndex = 0;

            pbxFirst = new PictureBox[] { pbxTile11, pbxTile12, pbxTile13, pbxTile14, pbxTile15, pbxTile16, pbxTile17, pbxTile1End };
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
            settings.saveSettings();
        }

        private void chkMini_CheckedChanged(object sender, EventArgs e)
        {
            settings.wantMini = chkMini.Checked;
            settings.saveSettings();
        }

        private void txtFirst7_TextChanged(object sender, EventArgs e)
        {
            string upper = txtFirst7.Text.ToUpper();
            for (int i = 0; i < upper.Length; i++)
            {
                int index = Array.IndexOf(TilesFor12.tileLetters, upper[i]);
                if (index == -1 || i >= 7)
                {
                    pbxFirst[i].BackgroundImage = null;
                    pbxFirst[i].Visible = true;
                    for (int j = i + 1; j < pbxFirst.Length; j++)
                        pbxFirst[j].Visible = false;
                    break;
                }
                else
                {
                    pbxFirst[i].BackgroundImage = tiles[index];
                    pbxFirst[i].Visible = true;
                }
            }

            lblFirstTooLong.Visible = upper.Length >= pbxFirst.Length;
            for (int i = upper.Length; i < pbxFirst.Length; i++)
                pbxFirst[i].Visible = false;
        }
    }
}
