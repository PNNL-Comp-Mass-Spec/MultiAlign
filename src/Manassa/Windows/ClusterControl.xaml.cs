using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Extensions;
using PNNLControls;
using PNNLOmics.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterControl.xaml
    /// </summary>
    public partial class ClusterControl : UserControl
    {
        private System.Windows.Forms.SaveFileDialog  m_featureImageSaveDialog;
        private UMCClusterLight m_mainCluster;
        private bool m_adjustingFeaturePlots = false;
        private Dictionary<ctlChartBase, List<ChartSynchData>> m_chartSynchMap;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClusterControl()
        {
            InitializeComponent();

            m_chartSynchMap = new Dictionary<ctlChartBase, List<ChartSynchData>>();

            FeatureFindingTolerances                = new FeatureTolerances();
            FeatureFindingTolerances.Mass           = 10;
            FeatureFindingTolerances.DriftTime      = 3;
            FeatureFindingTolerances.RetentionTime  = 50;


            ClusterTolerances               = new FeatureTolerances();
            ClusterTolerances.Mass          = 10;
            ClusterTolerances.DriftTime     = 3;
            ClusterTolerances.RetentionTime = .03;

            m_featureImageSaveDialog        = new System.Windows.Forms.SaveFileDialog();
            Binding binding                 = new Binding("SelectedFeature");
            binding.Source                  = m_featureGrid;
            SetBinding(FeatureProperty, binding);

            m_sicChart.ViewPortChanged      += new PNNLControls.ViewPortChangedHandler(m_sicChart_ViewPortChanged);
            m_msFeaturePlot.ViewPortChanged += new PNNLControls.ViewPortChangedHandler(m_msFeaturePlot_ViewPortChanged);

            m_clusterChart.ViewPortChanged  += new PNNLControls.ViewPortChangedHandler(m_clusterChart_ViewPortChanged);
            m_driftChart.ViewPortChanged    += new PNNLControls.ViewPortChangedHandler(m_driftChart_ViewPortChanged);
            
            SynchViewports(m_driftErrorScatterplot, ChartSynchType.XAxis,       m_driftErrorHistogram);
            SynchViewports(m_driftErrorScatterplot, ChartSynchType.XAxis,       m_driftErrorDistances);
            SynchViewports(m_driftErrorScatterplot, ChartSynchType.YToXAxis,    m_massErrorHistogram);
            SynchViewports(m_driftErrorScatterplot, ChartSynchType.YAxis,       m_errorScatterplot);
            
            SynchViewports(m_driftErrorHistogram,   ChartSynchType.XAxis,       m_driftErrorScatterplot);
            SynchViewports(m_netErrorHistogram,     ChartSynchType.XAxis,       m_errorScatterplot);
            SynchViewports(m_massErrorHistogram,    ChartSynchType.XToYAxis,    m_errorScatterplot);
            
            SynchViewports(m_errorScatterplot,      ChartSynchType.XAxis,       m_netErrorHistogram); 
            SynchViewports(m_errorScatterplot,      ChartSynchType.XAxis,       m_netDistances);                        
            SynchViewports(m_errorScatterplot,      ChartSynchType.YAxis,       m_driftErrorScatterplot);
            SynchViewports(m_errorScatterplot,      ChartSynchType.YToXAxis,    m_massErrorHistogram);
            SynchViewports(m_errorScatterplot,      ChartSynchType.YToXAxis,    m_massDistances);

        }

        private void SynchViewports(ctlChartBase sender, ChartSynchType synchType, params ctlChartBase[] targets)
        {
            sender.ViewPortChanged += new ViewPortChangedHandler(sender_ViewPortChanged);
            List<ChartSynchData> synchs = new List<ChartSynchData>();
            foreach (ctlChartBase target in targets)
            {
                ChartSynchData data = new ChartSynchData();
                data.SynchType = synchType;
                data.Target = target;
                synchs.Add(data);
            }
            if (!m_chartSynchMap.ContainsKey(sender))
            {
                m_chartSynchMap.Add(sender, new List<ChartSynchData>());
            }
            m_chartSynchMap[sender].AddRange(synchs);
        }
        void sender_ViewPortChanged(ctlChartBase chart, ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;
            m_adjustingFeaturePlots = true;

            if (!m_chartSynchMap.ContainsKey(chart))
                return;

            foreach (ChartSynchData data in m_chartSynchMap[chart]) 
            {
                RectangleF senderView   = args.ViewPort;                
                ctlChartBase target     = data.Target; 
                RectangleF targetView   = target.ViewPort;            
                switch (data.SynchType)
                {
                    case ChartSynchType.XAxis:
                        target.ViewPort = new RectangleF(senderView.X,
                                                     targetView.Y,
                                                     senderView.Width,
                                                     targetView.Height);
                        break;
                    case ChartSynchType.YAxis:
                        target.ViewPort = new RectangleF(targetView.X,
                                                     senderView.Y,
                                                     targetView.Width,
                                                     senderView.Height);
                        break;
                    case ChartSynchType.YToXAxis:
                        target.ViewPort = new RectangleF(senderView.Y,
                                                     targetView.Y,
                                                     senderView.Height,
                                                     targetView.Height);
                        break;
                    case ChartSynchType.XToYAxis:
                        target.ViewPort = new RectangleF(targetView.X,
                                                     senderView.X,
                                                     targetView.Width,
                                                     senderView.Width);
                        break;
                    case ChartSynchType.Both:
                        target.ViewPort = new RectangleF(senderView.X,
                                                     senderView.Y,
                                                     senderView.Width,
                                                     senderView.Height);
                        break;
                    default:
                        break;
                }
            }
            m_adjustingFeaturePlots = false;
        }

        
        #region Viewport synching
        void m_driftChart_ViewPortChanged(ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            
            m_adjustingFeaturePlots     = true;
            RectangleF otherView        = args.ViewPort;
            RectangleF newViewport      = new RectangleF(   m_clusterChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_clusterChart.ViewPort.Width,                                                            
                                                            otherView.Height);
            m_clusterChart.ViewPort     = newViewport;
            m_adjustingFeaturePlots     = false;

            Viewport = m_clusterChart.ViewPort;
        }
        void m_clusterChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {

            if (m_adjustingFeaturePlots) return;
            
            m_adjustingFeaturePlots     = true;
            RectangleF otherView        = args.ViewPort;
            
            RectangleF newViewport      = new RectangleF(   m_driftChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_driftChart.ViewPort.Width,                                                            
                                                            otherView.Height);
            m_driftChart.ViewPort       = newViewport;
            m_adjustingFeaturePlots     = false;

            Viewport = m_clusterChart.ViewPort;
        }        
        void m_msFeaturePlot_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots     = true;            
            RectangleF otherView        = args.ViewPort;
            RectangleF newViewport      = new RectangleF(   otherView.X,
                                                            m_sicChart.ViewPort.Y, 
                                                            otherView.Width,
                                                            m_sicChart.ViewPort.Height);
            m_sicChart.ViewPort         = newViewport;
            m_adjustingFeaturePlots     = false;
        }
        void m_sicChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots     = true;
            RectangleF otherView        = args.ViewPort;
            RectangleF newViewport      = new RectangleF(   otherView.X, 
                                                            m_msFeaturePlot.ViewPort.Y,
                                                            otherView.Width, 
                                                            m_msFeaturePlot.ViewPort.Height);
            m_msFeaturePlot.ViewPort    = newViewport;
            m_adjustingFeaturePlots     = false;            
        }
        #endregion

        #region Dependency Property Setters
        /// <summary>
        /// Updates the single feature plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FeatureSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (ClusterControl)sender;

            if (e.NewValue != null)
            {
                thisSender.m_adjustingFeaturePlots = true;
                UMCLight feature                        = (UMCLight)e.NewValue;                
                thisSender.m_msFeaturePlot.MainFeature  = feature;

                thisSender.m_sicChart.SetFeature(feature); 
                thisSender.m_sicChart.AutoViewPort();
                thisSender.m_msFeaturePlot.UpdateCharts(true);

                thisSender.m_sicChart.AdjustViewPortWithTolerances(thisSender.FeatureFindingTolerances, false);
                thisSender.m_msFeaturePlot.AdjustViewPortWithTolerances(thisSender.FeatureFindingTolerances, false);

                thisSender.m_adjustingFeaturePlots = false;
            }
        }
        /// <summary>
        /// Grabs the data from the provider cache and shoves into UI where needed.  This is done
        /// here like this to prevent holding large amounts of data in memory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClusterSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (ClusterControl)sender;
            if (e.NewValue != null)
            {
                thisSender.m_adjustingFeaturePlots = true;

                // Grab the data from the cache
                UMCClusterLightMatched matchedCluster = (UMCClusterLightMatched)e.NewValue;
                lock (thisSender.Providers.Synch)
                {
                    matchedCluster.Cluster.ReconstructUMCCluster(thisSender.Providers);
                }
                thisSender.UpdatePlotsWithClusterData(matchedCluster);


                // Make sure that if a new feature is selected that we update the feature list.
                if (matchedCluster.Cluster.Features.Count > 0)
                {
                    thisSender.m_featureGrid.SelectedFeature = matchedCluster.Cluster.Features[0];
                }
                thisSender.m_adjustingFeaturePlots = false;
            }
        }
        #endregion

        /// <summary>
        /// Dependency Property
        /// </summary>
        public UMCClusterLightMatched Cluster
        {
            get 
            {
                return (UMCClusterLightMatched)GetValue(ClusterProperty);
            }
            set 
            { 
                SetValue(ClusterProperty, value);
            }
        }                

        // Using a DependencyProperty as the backing store for Cluster.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClusterProperty =            
            DependencyProperty.Register("Cluster",
                                        typeof(UMCClusterLightMatched), 
                                        typeof(ClusterControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(ClusterSet)));



        public RectangleF Viewport
        {
            get { return (RectangleF)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Viewport.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(RectangleF), typeof(ClusterControl));

        

        /// <summary>
        /// Gets or sets the UMC feature that was selected to view in the feature viewer
        /// </summary>
        public UMCLight UMCFeature
        {
            get
            {
                return (UMCLight)GetValue(FeatureProperty);
            }
            set
            {
                SetValue(FeatureProperty, value);
            }
        }
        
        public static readonly DependencyProperty FeatureProperty     =
            DependencyProperty.Register("UMCFeature",
                                        typeof(UMCLight),
                                        typeof(ClusterControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(FeatureSet)));

        #region Plot updating.
        /// <summary>
        /// Updates the plots with data stored in the cache.  
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="cluster"></param>
        private void UpdatePlotsWithClusterData(UMCClusterLightMatched matchedCluster)
        {
            // Clear the charta
            m_clusterChart.ClearData();
            m_driftChart.ClearData();
            m_errorScatterplot.ClearData();
            m_driftErrorScatterplot.ClearData();
            m_driftErrorHistogram.ClearData();
            m_massErrorHistogram.ClearData();
            m_netErrorHistogram.ClearData();
            m_massDistances.ClearData();
            m_netDistances.ClearData();
            m_driftErrorDistances.ClearData();

            UMCClusterLight cluster = matchedCluster.Cluster;

            // Then start populating the charts with data.
            m_mainCluster                   = cluster;
            m_clusterChart.MainCluster      = matchedCluster;
            m_driftChart.MainCluster        = matchedCluster;
            m_clusterName.Content           = string.Format("Cluster: {0}", cluster.ID);
            m_featureGrid.Features          = cluster.Features;

            // Load the matching data.
            List<ClusterToMassTagMap> tagMatches       = matchedCluster.ClusterMatches;            
            m_masstagGrid.MassTags = tagMatches;

            // Then we find all the nearby clusters
            double massPpm  = ClusterTolerances.Mass;
            double net      = ClusterTolerances.RetentionTime;
            double minMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, massPpm);
            double maxMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, -massPpm);
            double minNet   = cluster.RetentionTime - net;
            double maxNet   = cluster.RetentionTime + net;

            List<UMCClusterLight> otherClusters
                = Providers.ClusterCache.FindNearby(minMass, maxMass, minNet, maxNet);

            // Remove self from the list
            int index = otherClusters.FindIndex(delegate(UMCClusterLight x)
            {
                return x.ID == cluster.ID;
            });

            if (index > -1)
            {
                otherClusters.RemoveAt(index);
            }

            // Then find the matching clusters and map them back to previously matched (to mass tag data)
            List<UMCClusterLightMatched> otherClusterMatches = new List<UMCClusterLightMatched>();            
            otherClusters.ForEach(x => otherClusterMatches.Add(UMCClusterLightCacheManager.FindById(x.ID)));

            foreach (UMCClusterLightMatched matchedOtherCluster in otherClusterMatches)
            {
                matchedOtherCluster.Cluster.Features.Clear();
                matchedOtherCluster.Cluster.ReconstructUMCCluster(Providers, false, false);
            }            
            m_clusterGrid.Clusters = otherClusterMatches;


            m_clusterChart.AddAdditionalClusters(otherClusterMatches);
            m_driftChart.AddAdditionalClusters(otherClusterMatches);
            m_clusterChart.UpdateCharts(true);
            m_driftChart.UpdateCharts(true);
            m_clusterChart.AdjustViewPortWithTolerances(ClusterTolerances,  false);
            m_driftChart.AdjustViewPortWithTolerances(ClusterTolerances,    true);
            Viewport = m_clusterChart.ViewPort;

            // Map out the MS/MS spectra.
            List<MSFeatureMsMs> msmsFeatures = new List<MSFeatureMsMs>();
            foreach (UMCLight feature in cluster.Features)
            {
                foreach (MSFeatureLight msFeature in feature.MSFeatures)
                {
                    foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                    {
                        MSFeatureMsMs map   = new MSFeatureMsMs();
                        map.Spectra         = spectrum;                        
                        map.Feature         = msFeature;
                        msmsFeatures.Add(map);
                    }
                }
            }

            m_msmsGrid.SetMsMsFeatureSpectra(msmsFeatures);

            m_errorScatterplot.MainCluster = matchedCluster;
            m_errorScatterplot.AddAdditionalClusters(otherClusterMatches);
            m_errorScatterplot.UpdateCharts(true);
            m_errorScatterplot.AdjustViewPortWithTolerances(ClusterTolerances, false);

            m_driftErrorScatterplot.MainCluster = matchedCluster;
            m_driftErrorScatterplot.AddAdditionalClusters(otherClusterMatches);
            m_driftErrorScatterplot.UpdateCharts(true);
            m_driftErrorScatterplot.AdjustViewPortWithTolerances(ClusterTolerances, true);

            m_massErrorHistogram.MainCluster = matchedCluster;
            m_massErrorHistogram.AddAdditionalClusters(otherClusterMatches);
            m_massErrorHistogram.UpdateCharts(true);
            m_massErrorHistogram.AdjustViewPortWithTolerances(ClusterTolerances.Mass);

            m_netErrorHistogram.MainCluster = matchedCluster;
            m_netErrorHistogram.AddAdditionalClusters(otherClusterMatches);
            m_netErrorHistogram.UpdateCharts(true);
            m_netErrorHistogram.AdjustViewPortWithTolerances(ClusterTolerances.RetentionTime);

            m_driftErrorHistogram.MainCluster = matchedCluster;
            m_driftErrorHistogram.AddAdditionalClusters(otherClusterMatches);
            m_driftErrorHistogram.UpdateCharts(true);
            m_driftErrorHistogram.AdjustViewPortWithTolerances(ClusterTolerances.DriftTime);

            m_driftErrorDistances.MainCluster = matchedCluster;
            m_driftErrorDistances.AddAdditionalClusters(otherClusterMatches);
            m_driftErrorDistances.UpdateCharts(true);
            m_driftErrorDistances.AdjustViewPortWithTolerances(ClusterTolerances.DriftTime);

            m_massDistances.MainCluster = matchedCluster;
            m_massDistances.AddAdditionalClusters(otherClusterMatches);
            m_massDistances.UpdateCharts(true);
            m_massDistances.AdjustViewPortWithTolerances(ClusterTolerances.Mass);

            m_netDistances.MainCluster = matchedCluster;
            m_netDistances.AddAdditionalClusters(otherClusterMatches);
            m_netDistances.UpdateCharts(true);
            m_netDistances.AdjustViewPortWithTolerances(ClusterTolerances.RetentionTime);
        }
        #endregion

        #region Data auto-properties
        /// <summary>
        /// Cluster tolerances
        /// </summary>
        public FeatureTolerances ClusterTolerances
        {
            get;
            set;
        }
        /// <summary>
        /// Feature finding tolerances
        /// </summary>
        public FeatureTolerances FeatureFindingTolerances
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the feature data access providers for retrieving extra data for display.
        /// </summary>
        public FeatureDataAccessProviders Providers
        {
            get;
            set;
        }
        #endregion

        #region Control Event Handlers

        private void m_saveFeaturePlotButton_Click(object sender, RoutedEventArgs e)
        {

            m_featureImageSaveDialog.FileName = string.Format("cluster-{0}.png", m_mainCluster.ID);
            System.Windows.Forms.DialogResult result = m_featureImageSaveDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string extension = System.IO.Path.GetExtension(m_featureImageSaveDialog.FileName);
                string filename  = System.IO.Path.GetFileNameWithoutExtension(m_featureImageSaveDialog.FileName);
                string pathName  = System.IO.Path.GetDirectoryName(m_featureImageSaveDialog.FileName);

                try
                {
                    m_clusterChart.ToBitmap(640, 640).Save(System.IO.Path.Combine(pathName, string.Format("{0}-massnet.{1}", filename, extension)));
                    m_driftChart.ToBitmap(640, 640).Save(System.IO.Path.Combine(pathName, string.Format("{0}-massdrifttime.{1}", filename, extension)));
                }
                catch
                {
                    //TODO: Add a message to the user here.
                }
            }
        }
        #endregion
    }
}
