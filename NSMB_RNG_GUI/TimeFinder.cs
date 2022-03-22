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
    public partial class TimeFinder : Form
    {
        private Settings settings;

        public TimeFinder(Settings settings)
        {
            InitializeComponent();

            this.settings = settings;

            numSeconds.Value = settings.dt.Second;
            chkMini.Checked = settings.wantMini;
            numThreads.Value = Environment.ProcessorCount;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }

        private void TimeFinder_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }

        private void chkAutoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            numSeconds.Enabled = !chkAutoSeconds.Checked;
        }

        private void UIEnable(bool enabled)
        {
            pnlButtons.Enabled = enabled;
            numThreads.Enabled = enabled;
            numSeconds.Enabled = enabled;
            chkAutoSeconds.Enabled = enabled;
            chkMini.Enabled = enabled;
            btnBack.Enabled = enabled;
            btnSearch.Enabled = enabled;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            UIEnable(false);
            DateTimeSearcher dts = new DateTimeSearcher((int)numSeconds.Value, 0, settings.MAC, settings.magic, chkMini.Checked);
            DateTime dt = await dts.findGoodDateTime((int)numThreads.Value);
            // DateTimeSearcher needs to be updated to not block the thread before the progress bar will work.
            progressBar1.Visible = true;
            lblResults.Text = "Progress";
            lblResults.Visible = true;
            // Once it's done:
            progressBar1.Visible = false;
            lblResults.Text = dt.ToLongDateString() + " " + dt.ToLongTimeString();
            UIEnable(true);
        }
    }
}
