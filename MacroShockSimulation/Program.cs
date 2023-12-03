using System;
using System.Windows.Forms;

namespace MacroShockSimulation
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MacroShockSimulator());
            return 0;
        }
    }
}