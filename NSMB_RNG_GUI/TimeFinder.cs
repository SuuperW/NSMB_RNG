using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            UIEnable(false);
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            lblResults.Text = "Progress";
            lblResults.Visible = true;

            DateTimeSearcher dts = new DateTimeSearcher((int)numSeconds.Value, 0, settings.MAC, settings.magic, chkMini.Checked);
            dts.ProgressReport += (p) => Invoke(() => progressBar1.Value = (int)(p * 100));
            dts.Completed += (dt) =>
            {
                progressBar1.Visible = false;
                lblResults.Text = dt.ToLongDateString() + " " + dt.ToLongTimeString();
                UIEnable(true);
            };
            Task t = dts.findGoodDateTime((int)numThreads.Value);
        }
    }
}
