using System.Collections.Generic;
using System.Windows.Controls;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.MetaData;
using System.IO;

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

                    m_clusterGrid.Clusters      = value.Clusters;                   
                    m_clusterPlot.SetClusters(value.Clusters);

                    m_clusterControl.Providers  = m_analysis.DataProviders;
                    Dictionary<int, int> map    = value.Clusters.CreateChargeMap<UMCClusterLight>();

                    m_msmsViewer.Analysis       = value;
                    m_msmsViewer.ExtractMsMsData(m_analysis.DataProviders);

                    // Make the dataset plots.
                    string directoryPath        = Path.GetDirectoryName(value.MetaData.AnalysisPath);
                    string plotPath             = Path.Combine(directoryPath, "plots");
                    if (Directory.Exists(plotPath))
                    {
                        Manassa.Data.DatasetPlotLoader loader = new Data.DatasetPlotLoader();                      
                        loader.LoadDatasetPlots(plotPath, value.MetaData.Datasets);
                    }

                    // Sort the datasets for the view...
                    value.MetaData.Datasets.Sort(delegate(DatasetInformation x, DatasetInformation y)
                    {
                        if (x.IsBaseline)
                            return -1;

                        return x.DatasetName.CompareTo(y.DatasetName);
                    });

                    m_datasetsName.Datasets = value.MetaData.Datasets;

                    // Setup the histogram data.
                    m_chargeStates.ConstructHistogram(map);
                    m_chargeStates.AutoViewPort();
                }
            }
        }
    }
}
