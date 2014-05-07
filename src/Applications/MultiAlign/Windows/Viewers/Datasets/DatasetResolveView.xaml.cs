using System.Windows;

namespace MultiAlign.Windows.Viewers.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetResolveView.xaml
    /// </summary>
    public partial class DatasetResolveView : Window
    {
        public DatasetResolveView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
