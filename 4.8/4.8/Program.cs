using System;
using System.Windows.Forms;

namespace Bai4_8_Async_ProgressBar
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi chạy ứng dụng từ FormMain (MDI Parent)
            Application.Run(new FormMain());
        }
    }
}