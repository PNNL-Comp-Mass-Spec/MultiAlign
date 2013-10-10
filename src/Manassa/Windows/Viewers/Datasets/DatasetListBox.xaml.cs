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

namespace MultiAlign.Windows.Viewers.Datasets
{
    /// <summary>
    /// Interaction logic for ListBoxTemplate.xaml
    /// </summary>
    public partial class DatasetListBox : UserControl
    {
        public DatasetListBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        public ObservableCollection<DatasetInformation> Datasets
        {
            get { return (ObservableCollection<DatasetInformation>)GetValue(DatasetsProperty); }
            set { SetValue(DatasetsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Datasets.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatasetsProperty =
            DependencyProperty.Register("Datasets", typeof(ObservableCollection<DatasetInformation>), typeof(DatasetListBox));

        public DatasetInformation SelectedDataset
        {
            get { return (DatasetInformation)GetValue(SelectedDatasetProperty); }
            set { SetValue(SelectedDatasetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDataset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDatasetProperty =
            DependencyProperty.Register("SelectedDataset", typeof(DatasetInformation), typeof(DatasetListBox));
    }
}
