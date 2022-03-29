using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResCryption
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            EncryptionManager.Init();
            ErorManager.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                Application.Run(new MainScreen());
            }
            else
            {
                if (FileManager.GetFileExtension(args.First()) == FileManager.ResEncryptionExtension)
                {
                 
                    Application.Run(new Password(args.First()));
                }
                else
                {
                    ErorManager.Show("ERR_BAD_EXTENTION");
                }
                
            }
        }
    }
}
