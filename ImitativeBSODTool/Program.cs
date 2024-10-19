using System;
using System.IO;
using System.Windows.Forms;

namespace ImitativeBSODTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Directory.GetCurrentDirectory() + "\\" != AppDomain.CurrentDomain.BaseDirectory)
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
