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
using System.ComponentModel;
using PNNLOmics.Data.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
    /// </summary>
    public partial class MsFeatureGrid : UserControl
    {
        private List<MSFeatureLight> m_features;

        public MsFeatureGrid()
        {
            InitializeComponent();
            Features = new List<MSFeatureLight>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public List<MSFeatureLight> Features
        {
            get
            {
                return m_features;
            }
            set
            {
                m_features = value;
                if (value != null)
                {
                    m_dataGrid.ItemsSource = value;
                }
            }
        }


        public MSFeatureLight SelectedFeature
        {
            get { return (MSFeatureLight)GetValue(SelectedClusterProperty); }
            set { SetValue(SelectedClusterProperty, value); }
        }        
        public static readonly DependencyProperty SelectedClusterProperty =
            DependencyProperty.Register("SelectedFeature", typeof(MSFeatureLight), typeof(MsFeatureGrid)); 
       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFeature = m_dataGrid.SelectedItem as MSFeatureLight;
        }    
    }
}
