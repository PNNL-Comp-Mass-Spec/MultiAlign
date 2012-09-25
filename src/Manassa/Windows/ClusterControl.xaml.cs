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
using MultiAlignCore.IO.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using PNNLOmics.Data.MassTags;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterControl.xaml
    /// </summary>
    public partial class ClusterControl : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ClusterControl()
        {
            InitializeComponent();            
        }

        private static void ClusterSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (ClusterControl)sender;

            if (e.NewValue != null)
            {
                thisSender.m_clusterChart.ClearData();

                UMCClusterLight cluster                 = (UMCClusterLight) e.NewValue;                
                cluster.ReconstructUMCCluster(thisSender.Providers);
                thisSender.m_clusterChart.MainCluster   = cluster;

                List<ClusterToMassTagMap> maps  = thisSender.Providers.MassTagMatches.FindByClusterId(cluster.ID);
                List<int> tagIds                = maps.ConvertAll<int> (x => x.MassTagId);
                List<MassTagLight> tags         = thisSender.Providers.MassTags.FindMassTags(tagIds);
                thisSender.m_clusterChart.AddMassTags(tags);
            }
        }
        /// <summary>
        /// Gets or sets the feature data access providers for retrieving extra data for display.
        /// </summary>
        public FeatureDataAccessProviders Providers
        {
            get;
            set;
        }

        public UMCClusterLight Cluster
        {
            get 
            { 
                return (UMCClusterLight)GetValue(ClusterProperty);
            }
            set 
            { 
                SetValue(ClusterProperty, value);
            }
        }                

        // Using a DependencyProperty as the backing store for Cluster.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClusterProperty =            
            DependencyProperty.Register("Cluster",
                                        typeof(UMCClusterLight), 
                                        typeof(ClusterControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(ClusterSet)));        
    }
}
