using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MultiAlign.Data;
using MultiAlign.Windows.Viewers.Datasets;
using MultiAlignCore.Data;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlign.Windows.Wizard
{
    /// <summary>
    /// Interaction logic for DatasetSelection.xaml
    /// </summary>
    public partial class DatasetSelection : UserControl
    {
        public DatasetSelection()
        {
            InitializeComponent();            
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }        
    }
}
