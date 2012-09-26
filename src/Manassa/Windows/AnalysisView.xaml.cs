using System.Collections.Generic;
using System.Windows.Controls;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for AllClustersControl.xaml
    /// </summary>
    public partial class AllClustersControl : UserControl
    {
        private MultiAlignAnalysis m_analysis;
        
        public AllClustersControl()
        {
            InitializeComponent();

            m_analysis = null;
        }

        /// <summary>
        /// Gets or sets the Analysis
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get
            {
                return m_analysis;
            }
            set
            {
                m_analysis = value;
                if (value != null)
                {
                    m_datasetsName.Datasets     = value.MetaData.Datasets;
                    m_clusterGrid.Clusters      = value.Clusters;                   
                    m_clusterPlot.SetClusters(value.Clusters);
                    m_clusterControl.Providers  = m_analysis.DataProviders;
                    Dictionary<int, int> map    = value.Clusters.CreateChargeMap<UMCClusterLight>();

                    m_chargeStates.ConstructHistogram(map);
                    m_chargeStates.AutoViewPort();
                }
            }
        }
    }
}
