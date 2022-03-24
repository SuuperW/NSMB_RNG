using System;
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
            dts?.cancel();
            // Exit the application if there isn't another open window.
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
            numSeconds.Enabled = enabled && !chkAutoSeconds.Checked;
            chkAutoSeconds.Enabled = enabled;
            chkMini.Enabled = enabled;
            btnBack.Enabled = enabled;
            btnSearch.Enabled = enabled;
        }

        DateTimeSearcher? dts;
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Prepare UI
            UIEnable(false);
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            lblResults.Text = "Progress";
            lblResults.Visible = true;

            // Search for a date and time
            dts = new DateTimeSearcher((int)numSeconds.Value, 0, settings.MAC, settings.magic, chkMini.Checked);
            dts.ProgressReport += (p) => Invoke(() => progressBar1.Value = (int)(p * progressBar1.Maximum));
            dts.Completed += (dt) =>
            {
                dts = null;
                if (dt.Year == 1) // no result
                {
                    // Automatically increment seconds and try again?
                    if (chkAutoSeconds.Checked)
                    {
                        numSeconds.Value += 1;
                        btnSearch_Click(dt, new EventArgs());
                    }
                    else
                    {
                        progressBar1.Visible = false;
                        lblResults.Text = "No date/time with " + numSeconds.Value.ToString() + " gives a good seed.";
                        UIEnable(true);
                    }
                }
                else
                {
                    // Sends results to the double jumps form
                    Settings newSettings = new Settings();
                    newSettings.dt = dt; // (newSettings because we don't want to change this in settings.bin, if the user returns to the main form)
                    newSettings.MAC = settings.MAC;
                    newSettings.wantMini = settings.wantMini;
                    newSettings.magic = settings.magic;
                    DoubleJumpsForm djForm = new DoubleJumpsForm(newSettings, true);
                    djForm.Show();
                    this.Close();
                }
            };
            Task t = dts.findGoodDateTime((int)numThreads.Value);
        }
    }
}
