using System;
using System.Windows.Forms;

namespace TilesetEditor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (s, e) => MessageBox.Show("UI exception: " + e.Exception.Message);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show("Unhandled: " + (e.ExceptionObject?.ToString() ?? "null"));

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
