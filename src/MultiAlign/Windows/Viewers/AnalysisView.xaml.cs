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
            
            //Binding binding  = new Binding("Viewport");
            //binding.Source   = m_clusterControl;
            //SetBinding(ViewportProperty, binding);
        }
                
        /// <summary>
        /// Gets or sets the Analysis
        /// </summary>
        public void Analysis(MultiAlignAnalysis analysis)
        {                                                                          
                   
            //m_clusterPlot.SetClusters(value.Clusters);                                                                 
            //m_clusterControl.Providers  = m_analysis.DataProviders;
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

        //private static void SynchronizeViewport(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var thisSender = (AnalysisView)sender;

        //    if (e.NewValue != null)
        //    {
        //        RectangleF viewport = (RectangleF) e.NewValue;
        //        thisSender.m_clusterPlot.UpdateHighlightArea(viewport);                
        //    }
        //}
    }
}
