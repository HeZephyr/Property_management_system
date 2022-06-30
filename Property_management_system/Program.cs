using System;
using System.Windows.Forms;

namespace Property_management_system
{
    static class Program
    {
        // 应用程序主入口点
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
