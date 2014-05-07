using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PNNLOmics.Data.Features;

namespace MultiAlign.Windows.Viewers.Features
{
    /// <summary>
    /// Interaction logic for 
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
