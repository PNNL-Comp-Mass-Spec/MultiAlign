using System.Windows;

namespace MultiAlign.Windows.Viewers.Clusters
{
    /// <summary>
    /// Interaction logic for ClusterFilterWindow.xaml
    /// </summary>
    public partial class ClusterFilterWindow : Window
    {
        public ClusterFilterWindow()
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
