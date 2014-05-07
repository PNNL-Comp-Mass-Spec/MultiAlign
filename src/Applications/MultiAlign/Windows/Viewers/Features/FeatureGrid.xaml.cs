using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PNNLOmics.Data.Features;

namespace MultiAlign.Windows.Viewers.Features
{
    /// <summary>
    /// Interaction logic for 
    /// </summary>
    public partial class FeatureGrid : UserControl
    {
        private List<UMCLight> m_features;

        public FeatureGrid()
        {
            InitializeComponent();
            Features = new List<UMCLight>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public List<UMCLight> Features
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


        public UMCLight SelectedFeature
        {
            get { return (UMCLight)GetValue(SelectedClusterProperty); }
            set { SetValue(SelectedClusterProperty, value); }
        }        
        public static readonly DependencyProperty SelectedClusterProperty =
            DependencyProperty.Register("SelectedFeature", typeof(UMCLight), typeof(FeatureGrid)); 
       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFeature = m_dataGrid.SelectedItem as UMCLight;
        }    
    }
}
