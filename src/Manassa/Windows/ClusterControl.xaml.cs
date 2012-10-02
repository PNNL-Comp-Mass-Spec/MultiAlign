using System;
using MultiAlignCore.Data.Features;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCustomControls.Extensions;
using PNNLOmics.Data;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClusterControl()
        {
            InitializeComponent();
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
        }

        
        #region Viewport synching
        void m_driftChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
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
                UMCClusterLight cluster                 = (UMCClusterLight) e.NewValue;
                lock (thisSender.Providers.Synch)
                {
                    cluster.ReconstructUMCCluster(thisSender.Providers);
                }
                thisSender.UpdatePlotsWithClusterData(cluster);


                // Make sure that if a new feature is selected that we update the feature list.
                if (cluster.Features.Count > 0)
                {
                    thisSender.m_featureGrid.SelectedFeature = cluster.Features[0];
                }
                thisSender.m_adjustingFeaturePlots = false;
            }
        }
        #endregion

        /// <summary>
        /// Dependency Property
        /// </summary>
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
        private void UpdatePlotsWithClusterData(UMCClusterLight cluster)
        {
            // Clear the charta
            m_clusterChart.ClearData();
            m_driftChart.ClearData();
            m_errorScatterplot.ClearData();
            m_driftErrorScatterplot.ClearData();

            // Then start populating the charts with data.
            m_mainCluster                   = cluster;
            m_clusterChart.MainCluster      = cluster;
            m_driftChart.MainCluster        = cluster;
            m_clusterName.Content           = string.Format("Cluster: {0}", cluster.ID);
            m_featureGrid.Features          = cluster.Features;

            // Then find all matching mass mass tags 
            List<ClusterToMassTagMap> maps  = Providers.MassTagMatches.FindByClusterId(cluster.ID);
            List<int> tagIds                = maps.ConvertAll<int>(x => x.MassTagId);
            List<MassTagLight> tags         = Providers.MassTags.FindMassTags(tagIds);

            m_masstagGrid.MassTags = tags;
            m_clusterChart.AddMassTags(tags);
            m_driftChart.AddMassTags(tags);

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

            // Make sure that we don't get the specified cluster.                
            otherClusters.ForEach(x => x.ReconstructUMCCluster(Providers, false, false));

            m_clusterChart.AddAdditionalClusters(otherClusters);
            m_driftChart.AddAdditionalClusters(otherClusters);
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

            // Update the data grid with nearby clusters.
            m_clusterGrid.Clusters = otherClusters;

            m_errorScatterplot.MainCluster = cluster;
            m_errorScatterplot.AddAdditionalClusters(otherClusters);
            m_errorScatterplot.UpdateCharts(true);
            m_errorScatterplot.AdjustViewPortWithTolerances(ClusterTolerances, false);

            m_driftErrorScatterplot.MainCluster = cluster;
            m_driftErrorScatterplot.AddAdditionalClusters(otherClusters);
            m_driftErrorScatterplot.UpdateCharts(true);
            m_driftErrorScatterplot.AdjustViewPortWithTolerances(ClusterTolerances, true);
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
