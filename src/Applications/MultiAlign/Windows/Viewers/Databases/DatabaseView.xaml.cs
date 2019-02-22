using System.Windows.Controls;

namespace MultiAlign.Windows.Viewers.Databases
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        public DatabaseView()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}