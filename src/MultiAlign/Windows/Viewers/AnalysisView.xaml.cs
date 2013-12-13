using System;
using PNNLOmics.Algorithms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;
using MultiAlignCore.Algorithms.Features;
using System.Collections.ObjectModel;
using MultiAlign.ViewModels;
using MultiAlign.IO;
using MultiAlign.ViewModels.TreeView;
using System.Linq;

namespace MultiAlign.Windows.Viewers
{
    

    /// <summary>
    /// Interaction logic for AllClustersControl.xaml
    /// </summary>
    public partial class AnalysisView : UserControl
    {
                
        public AnalysisView()
        {
            InitializeComponent();            
        }
                
        /// <summary>
        /// Gets or sets the Analysis
        /// </summary>
        public void SetClusters(List<UMCClusterLightMatched> matches, bool autoViewport)
        {
            List<UMCClusterLight> clusters =    (from cluster in matches
                                                select cluster.Cluster).ToList();
                        
            m_clusterPlot.SetClusters(clusters, autoViewport);                                                             
            //m_clusterPlot.SetClusters(value.Clusters);                                                                 
            //m_massTagViewer.Providers   = m_analysis.DataProviders;

            //if (value.MassTagDatabase != null)
            //{
            //    m_massTagViewer.Database = value.MassTagDatabase;
            //} 
                                                                           
                        
            //// Setup the histogram data.
            //Dictionary<int, int> map    = value.Clusters.CreateChargeMap<UMCClusterLight>();
            //m_chargeStates.ConstructHistogram(map);
            //m_chargeStates.AutoViewPort();

            //Dictionary<int, int> datasetMap = value.Clusters.CreateClusterDatasetMemeberSizeHistogram();
            //m_datasetSizeHistogram.ConstructHistogram(datasetMap);
            //m_datasetSizeHistogram.AutoViewPort();

            //Dictionary<int, int> sizeMap = value.Clusters.CreateClusterSizeHistogram();
            //m_clusterSizeHistogram.ConstructHistogram(sizeMap);
            //m_clusterSizeHistogram.AutoViewPort();

            //Dictionary<int, int> massTagMap = clusters.Item2.CreateMassTagClusterSizeHistogram();
            //m_massTagHistogram.ConstructHistogram(massTagMap);
            //m_massTagHistogram.AutoViewPort();                            
        }        
    }
}
