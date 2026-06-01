using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DoAnCN_Net
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Ghi log ngay khi app khởi động
                string logPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "startup.log");
                System.IO.File.WriteAllText(logPath,
                    "App started at: " + DateTime.Now + "\n" +
                    "BaseDir: " + AppDomain.CurrentDomain.BaseDirectory);
            }
            catch { }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "sqllocaldb",
                    Arguments = "start MSSQLLocalDB",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                System.Threading.Thread.Sleep(2000);
            }
            catch { }
        }
    }
}