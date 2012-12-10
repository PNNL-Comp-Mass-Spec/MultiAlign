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
using MultiAlignCore.Data.Features;
using PNNLOmics.Data.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
    /// </summary>
    public partial class ClusterGrid : UserControl
    {
        private List<UMCClusterLightMatched> m_clusters;

        public ClusterGrid()
        {
            InitializeComponent();
            Clusters = new List<UMCClusterLightMatched>();
        }



        public bool UsesDriftTime
        {
            get { return (bool)GetValue(UsesDriftTimeProperty); }
            set { SetValue(UsesDriftTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsesDriftTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsesDriftTimeProperty =
            DependencyProperty.Register("UsesDriftTime", typeof(bool), typeof(ClusterGrid), new UIPropertyMetadata(true));



        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public List<UMCClusterLightMatched> Clusters
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

        public void SelectByClusterID(int id)
        {
            UMCClusterLightMatched clusterFound = m_clusters.Find(delegate(UMCClusterLightMatched cluster)
            {
                return cluster.Cluster.ID == id;
            });

            if (clusterFound != null)
            {
                m_dataGrid.SelectedItem = clusterFound;
            }
        }

        public UMCClusterLightMatched SelectedCluster
        {
            get { return (UMCClusterLightMatched)GetValue(SelectedClusterProperty); }
            set { SetValue(SelectedClusterProperty, value); }
        }        
        public static readonly DependencyProperty SelectedClusterProperty =
            DependencyProperty.Register("SelectedCluster", typeof(UMCClusterLightMatched), typeof(ClusterGrid)); 

       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCluster != null)
            {
                SelectedCluster.Cluster.Features.Clear();
            }
            SelectedCluster = m_dataGrid.SelectedItem as UMCClusterLightMatched;
        }            
    }
}
