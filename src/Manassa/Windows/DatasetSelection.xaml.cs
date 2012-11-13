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

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for DatasetSelection.xaml
    /// </summary>
    public partial class DatasetSelection : UserControl
    {
        public DatasetSelection()
        {
            InitializeComponent();
            TestClass x = new TestClass()
            {
                DatasetName="Test",
                ID=0
            };
            MainDatasetGrid.Items.Add(x);
            MainDatasetGrid.Items.Add(x);
            MainDatasetGrid.Items.Add(x);
            MainDatasetGrid.Items.Add(x);
        }

        private void AddInputFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseForInputFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InputSingleFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseSingleFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseForFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveSelectedButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    /// <summary>
    /// Gets the test class stuff
    /// </summary>
    public class TestClass
    {
        public string DatasetName
        {
            get;
            set;
        }
        public int ID
        {
            get;
            set;
        }
    }
}
