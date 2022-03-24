using System;
using System.Windows.Forms;

namespace NSMB_RNG_GUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            MainForm mainForm = new MainForm();
            mainForm.Show();
            Application.Run();
        }
    }
}