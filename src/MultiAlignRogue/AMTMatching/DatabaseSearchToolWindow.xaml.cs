using System.Windows;

namespace MultiAlignRogue.AMTMatching
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

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}