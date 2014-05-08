using System.Windows;

namespace MultiAlign.Windows.Viewers.Databases
{
    /// <summary>
    ///     Interaction logic for DatabaseSearchToolWindow.xaml
    /// </summary>
    public partial class DatabaseSearchToolWindow : Window
    {
        public DatabaseSearchToolWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}