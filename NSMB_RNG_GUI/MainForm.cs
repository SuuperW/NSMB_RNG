using System;
using System.Drawing;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
    public partial class MainForm : Form
    {
        Settings settings;

        public MainForm()
        {
            InitializeComponent();

            settings = Settings.loadSettings();
            txtMAC.Text = settings.MAC.ToString("X").PadLeft(12, '0');
            chkMini.Checked = settings.wantMini;
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
    }
}
