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
using MultiAlignCore.Data;
using System.Collections.ObjectModel;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ListBoxTemplate.xaml
    /// </summary>
    public partial class DatasetListBox : UserControl
    {
        public DatasetListBox()
        {
            InitializeComponent();
        }

        public ObservableCollection<DatasetInformation> Datasets
        {
            get;
            set;
        }
    }
}
