using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using QuanLyThietBi.BLL;
using QuanLyThietBi.DAL;
namespace QuanLyThietBi.GUI
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture=CultureInfo.GetCultureInfo("vi-VN");
            Thread.CurrentThread.CurrentUICulture=CultureInfo.GetCultureInfo("vi-VN");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var service=new ThietBiService(new ThietBiRepository(), new LoaiThietBiRepository());
            var settings=new DatabaseSetupService(new ConnectionSettingsStore());
            Application.Run(new FrmThietBi(service, DatabaseBootstrapper.Initialize, settings));
        }
    }
}
