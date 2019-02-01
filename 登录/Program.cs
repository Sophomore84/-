using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace 风帆电池
{
    static class Program
    {
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmLogin frmLogin = new frmLogin();
            
            frmNewMain f1 = new frmNewMain();
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(f1);

            }
            //Application.Run(new frmNewMain());


        }
    }
}
