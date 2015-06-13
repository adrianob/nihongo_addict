using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NihongoAddict
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        
        public static void Application_ThreadException
          (object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Abort;
            try
            {
                result = MessageBox.Show(e.Exception.Message, "Erro");                
            }
            finally
            {
                if (result == DialogResult.Abort)
                {
                    Application.Exit();
                }
            }
        }
    }
}
