using System.Windows;
using MultiAlign.ViewModels;
using MultiAlign.Windows;

namespace MultiAlign
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            var model = new MainViewModel();
            MainWindow.DataContext = model;

            mainWindow.ShowDialog();
        }
    }
}