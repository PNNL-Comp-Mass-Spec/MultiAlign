using System.Windows;
using MultiAlign.ViewModels;
using MultiAlign.Windows;

namespace MultiAlign
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow   = new MainWindow();
            MainViewModel model     = new MainViewModel();
            MainWindow.DataContext  = model;

            mainWindow.ShowDialog();
        }
    }
}
