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
using MultiAlignCore.Data.Features;
using Manassa.ViewModels;

namespace Manassa.Windows.Viewers.Clusters
{
    /// <summary>
    /// Interaction logic for ClusterTree.xaml
    /// </summary>
    public partial class ClusterTree : UserControl
    {
        UMCClusterTreeRootViewModel m_clusters;

        public ClusterTree()
        {
            InitializeComponent();

        }

        public void SetClusters(List<UMCClusterLightMatched> clusters)
        {
            m_clusters = new UMCClusterTreeRootViewModel(clusters);
            DataContext = m_clusters;
        }
    }
}
