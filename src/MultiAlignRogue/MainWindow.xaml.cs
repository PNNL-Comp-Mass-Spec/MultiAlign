using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = this.DataContext as MainViewModel;
            this.DataContextChanged += (o, e) => this.viewModel = this.DataContext as MainViewModel;
        }

         
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = this.FileSelectionDataGrid.SelectedItems;
            if (this.viewModel != null)
            {
                this.viewModel.SelectedDatasets = new List<DatasetInformation>(selectedItems.Cast<DatasetInformation>());
            }
        }

    }
}
