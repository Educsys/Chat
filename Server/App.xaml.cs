using Server.Modules;
using Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public WindowsInteractionService WIS;
        public App()
        {
            WIS = new WindowsInteractionService();

            WIS.RegisterVMandWindow<MainWindowViewModel, MainWindow>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WIS.ShowModalWindow(new MainWindowViewModel());

            Shutdown();
        }
    }
}
