using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace QuickAccess
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
					//QuickAccess-{6EBAC5AC-BCF2-4263-A82C-F189930AEA30}
					//Mutex.

					using (Mutex mutex = new Mutex(false, "QuickAccess-{6EBAC5AC-BCF2-4263-A82C-F189930AEA30}"))
					{
						if (!mutex.WaitOne(0, true))
						{
							Application.EnableVisualStyles();
							MessageBox.Show("Another instances of QuickAccess is already running.", "Only one instance allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							Application.EnableVisualStyles();
							Application.SetCompatibleTextRenderingDefault(false);
							Application.Run(new Form1());
						}
					}

						//Application.EnableVisualStyles();
						//Application.SetCompatibleTextRenderingDefault(false);
						//Application.Run(new Form1());
        }
    }
}
