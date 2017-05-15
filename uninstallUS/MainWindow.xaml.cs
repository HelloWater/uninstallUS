using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using IWshRuntimeLibrary;


namespace uninstallUS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer dtimer;

        public MainWindow()
        {
            InitializeComponent();

            this.ResizeMode = ResizeMode.NoResize;
        }

        private void Func_Uninstall()
        {
            try
            {
                //delete software Registkey
                RegistryKey machinelocalItem;
                RegistryKey softwareItem;
                machinelocalItem = Registry.LocalMachine;
                softwareItem = machinelocalItem.OpenSubKey("SOFTWARE", true);

                RegistryKey shawayItem = softwareItem.OpenSubKey("Shaway", true);
                if (shawayItem != null)
                {
                    string sPath = (string)shawayItem.GetValue("ProgramFile");
                    if (Directory.Exists(sPath))
                    {
                        Directory.Delete(sPath,true);
                    }
                    softwareItem.DeleteSubKeyTree("Shaway");
                }

                //delete uninstall registkey
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", true);
                RegistryKey software = key.OpenSubKey("Shaway");
                if (software != null)
                {
                    key.DeleteSubKeyTree("Shaway");
                }
            }
            catch(Exception)
            {
                
            }
        }

        private void CheckStartup()
        {
            string sStartup = Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);
            //Get All Startup Item  
            List<string> files = new List<string>(Directory.GetFiles(sStartup, "*.lnk"));
            foreach (var ink in files)
            {
                //if (System.IO.File.Exists(ink))
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ink);
                    string sDesPath = shortcut.TargetPath;
                    string sWorkingPath = shortcut.WorkingDirectory;

                    string sTail = sDesPath.Substring(sDesPath.Length - 8, 8);
                    if (sTail.Equals("demo.exe"))
                    {
                        System.IO.File.Delete(ink);
                    }
                }
            }
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            this._loading.Visibility = Visibility.Visible;

            if(dtimer == null)
            {
                dtimer = new System.Windows.Threading.DispatcherTimer();
                dtimer.Interval = TimeSpan.FromSeconds(5);
                dtimer.Tick += dtimer_Tick;

                dtimer.Start();
            }
        }

        void dtimer_Tick(object sender, EventArgs e)
        {
            Func_Uninstall();

            CheckStartup();

            dtimer.Stop();
            
            this._loading.Visibility = Visibility.Collapsed;
            finishIcon.Visibility = Visibility.Visible;
            m_title.Text = "Uninstall Finished !";
            m_BtnOk.IsEnabled = false;
            m_BtnCancel.Content = "退出";
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            this._loading.Visibility = Visibility.Collapsed;

            Close();
        }  
    }
}
