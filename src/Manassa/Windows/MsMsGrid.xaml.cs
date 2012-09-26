using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PNNLOmics.Data;
using MultiAlignCore.Data.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
    /// </summary>
    public partial class MsMsGrid : UserControl
    {
        private List<MSFeatureMsMs> m_clusters;

        public MsMsGrid()
        {
            InitializeComponent();
            Clusters = new List<MSFeatureMsMs>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public List<MSFeatureMsMs> Clusters
        {
            get
            {
                return m_clusters;
            }
            set
            {
                m_clusters = value;
                if (value != null)
                {
                    m_dataGrid.ItemsSource = value;
                }
            }
        }

        public MSFeatureMsMs SelectedSpectra
        {
            get { return (MSFeatureMsMs)GetValue(SelectedClusterProperty); }
            set { SetValue(SelectedClusterProperty, value); }
        }        
        public static readonly DependencyProperty SelectedClusterProperty =
            DependencyProperty.Register("SelectedSpectra", typeof(MSFeatureMsMs), typeof(MsMsGrid)); 

       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSpectra = m_dataGrid.SelectedItem as MSFeatureMsMs;
        }
    }
}
