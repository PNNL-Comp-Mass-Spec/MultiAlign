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
using System.Collections.ObjectModel;
using MultiAlignCore.Data;

using MultiAlignParameterFileEditor;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for SelectBaselinesAndOptions.xaml
    /// </summary>
    public partial class SelectBaselinesAndOptions : UserControl
    {
        private AnalysisConfig m_config;
        public SelectBaselinesAndOptions()
        {
            InitializeComponent();
        }

        private void SelectAnalysisOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Main parameterOptions = new Main();
            System.Windows.Forms.DialogResult result = parameterOptions.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                m_config.Analysis.Options = parameterOptions.Options;
            }
        }

        private void BrowseForFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectMtsMtdbButton_Click(object sender, RoutedEventArgs e)
        {

        }


        

        public ObservableCollection<DatasetInformation> Datasets
        {
            get { return (ObservableCollection<DatasetInformation>)GetValue(DatasetsProperty); }
            set { SetValue(DatasetsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Datasets.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatasetsProperty =
            DependencyProperty.Register("Datasets", typeof(ObservableCollection<DatasetInformation>), typeof(SelectBaselinesAndOptions));


        public void SetAnalysisConfig(AnalysisConfig config)
        {
            m_config = config;            
        }
    }
}
