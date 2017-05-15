using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace uninstallUS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        ///  检查是否是管理员身份 
        /// </summary>
        private void CheckAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,  
                // so instead we launch the app as administrator in a new process.  
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator  
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process  
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    //ex.WriteLog();
                }

                // Shut down the current process  
                Environment.Exit(0);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CheckAdministrator();
            //如果不是管理员，程序会直接退出，并使用管理员身份重新运行。  
            StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
        }  
    }
}
