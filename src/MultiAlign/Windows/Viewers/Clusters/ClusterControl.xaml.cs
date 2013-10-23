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
using PNNLOmicsIO.IO;
using System.IO;
using System;
using System.Linq;

namespace MultiAlign.Windows.Viewers.Clusters
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
        private System.Windows.Forms.SaveFileDialog m_saveFileDialog;

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
            
            
            /*Binding binding                 = new Binding("SelectedFeature");
            binding.Source                  = m_featureGrid;
            SetBinding(FeatureProperty, binding);
            */

            m_sicChart.ViewPortChanged      += new PNNLControls.ViewPortChangedHandler(m_sicChart_ViewPortChanged);
            m_sicChart.ChartPointPressed    += new System.EventHandler<SelectedPointEventArgs>(m_sicChart_ChartPointPressed);     

            m_msFeaturePlot.ViewPortChanged += new PNNLControls.ViewPortChangedHandler(m_msFeaturePlot_ViewPortChanged);

            m_clusterChart.ViewPortChanged  += new PNNLControls.ViewPortChangedHandler(m_clusterChart_ViewPortChanged);
            m_driftChart.ViewPortChanged    += new PNNLControls.ViewPortChangedHandler(m_driftChart_ViewPortChanged);
                        
            m_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            m_saveFileDialog.Filter = "DTA (*.dta)|*.dta|MGF (*.mgf)|*.mgf";

        }

        void m_sicChart_ChartPointPressed(object sender, SelectedPointEventArgs e)
        {
            // Figure out the closest spectra to view.
            // TODO: Move this out of the way!
            if (e != null && e.SelectedPoint != null)
            {
                float x = e.SelectedPoint.X;

                if (this.UMCFeature != null)
                {
                    int point           = Convert.ToInt32(x);
                    MSFeatureLight closest   = null;
                    int closestDist     = int.MaxValue;
                    foreach (MSFeatureLight feature in UMCFeature.MSFeatures)
                    {
                        int diff = Math.Abs(feature.Scan - point);
                        if (closest == null || diff < closestDist)
                        {
                            closest     = feature; 
                            closestDist = diff;
                        }
                    }
                    if (closest != null)
                    {
                        this.LoadMsSpectrum(closest);
                    }
                }
            }
        }

        public bool UsesDriftTime
        {
            get { return (bool)GetValue(UsesDriftTimeProperty); }
            set { SetValue(UsesDriftTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsesDriftTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsesDriftTimeProperty =
            DependencyProperty.Register("UsesDriftTime", typeof(bool), typeof(ClusterControl), new UIPropertyMetadata(false));



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
                
                if (feature.MSFeatures.Count > 0)
                {
                    feature.MSFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        return x.Scan.CompareTo(y.Scan);
                    });
                    thisSender.LoadMsSpectrum(feature.MSFeatures[0]);
                   // thisSender.m_msFeatureGrid.Features = feature.MSFeatures;
                }
            }
        }

        private void LoadMsSpectrum(MSFeatureLight msFeature)
        {
            DatasetInformation info = Analysis.MetaData.FindDatasetInformation(msFeature.GroupID);            
            if (info != null && info.Raw != null && info.RawPath != null)
            {
                Dictionary<int, List<MSFeatureLight>> features = msFeature.ParentFeature.CreateChargeMap();
                
                int N = features[msFeature.ChargeState].Count;
                if (!features.ContainsKey(msFeature.ChargeState) || N < 1)
                    return;

                double sum = 0;
                double min = double.MaxValue;
                double max = double.MinValue;

                foreach (MSFeatureLight chargeFeature in features[msFeature.ChargeState])
                {
                    min = Math.Min(min, chargeFeature.Mz);
                    max = Math.Max(max, chargeFeature.Mz);
                    sum += chargeFeature.Mz;                    
                }

                double averageMz = sum / Convert.ToDouble(N);
                //double adder = .02; //Math.Abs(max - min) * .1;
                //double lowMz = msFeature.Mz - adder; // min - adder;
                //double highMz = msFeature.Mz + adder;//max + adder;
                min = max = msFeature.Mz;
                double adder    = .09; //  Math.Abs(max - min) * .1;
                double lowMz    = min - adder;
                double highMz   = max + adder;


                List<XYData> spectrum = ParentSpectraFinder.GetParentSpectrum(info.RawPath,
                                                                                msFeature.Scan,
                                                                                lowMz,
                                                                                highMz);
                if (spectrum != null)
                {
                    m_parentSpectra.Title = string.Format("scan {0} @ {1} m/z", msFeature.Scan, msFeature.Mz);
                    m_parentSpectra.SetSpectra(spectrum);                     
                    m_parentSpectra.AutoViewPort(); 
                    //RectangleF viewport = m_parentSpectra.ViewPort;
                    //m_parentSpectra.ViewPort = new RectangleF(  Convert.ToSingle(lowMz), 
                    //                                            viewport.Top, 
                    //                                            Convert.ToSingle(Math.Abs(highMz  - lowMz)), 
                    //                                            Math.Abs(viewport.Bottom - viewport.Top));
                }
            }
        }
        /// <summary>
        /// Hack for stephens presentation.
        /// </summary>
        static int clusterCount = 0;
        void WriteUMCCluster(UMCClusterLight cluster)
        {
            using(TextWriter writer = File.CreateText(@"m:\clusterData.txt"))
            {
                
                List<MSSpectra> spectra = new List<MSSpectra>();
                foreach (UMCLight feature in cluster.Features)
                {
                    foreach (MSFeatureLight msfeature in feature.MSFeatures)
                    {
                        foreach(MSSpectra spectrum in msfeature.MSnSpectra)
                        {
                            DatasetInformation info = Analysis.MetaData.FindDatasetInformation(msfeature.GroupID);
                            if (info != null && info.Raw != null && info.RawPath != null)
                            {
                                writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", cluster.ID,
                                                                    feature.GroupID,
                                                                    feature.ID,
                                                                    msfeature.ID,
                                                                    spectrum.ID,
                                                                    spectrum.Scan,
                                                                    info.RawPath);
                            
                                spectrum.Peaks = ParentSpectraFinder.GetDaughterSpectrum(info.RawPath, spectrum.Scan);
                                spectra.Add(spectrum);

                                using (TextWriter specWriter = File.CreateText(string.Format(@"m:\spec-{0}-{1}-{2}.txt", cluster.ID, spectrum.GroupID, spectrum.ID)))
                                {
                                    foreach (XYData datum in spectrum.Peaks)
                                    {
                                        specWriter.WriteLine("{0}\t{1}", datum.X, datum.Y);
                                    }
                                }
                            }                                                          
                        }
                    }
                }
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
                thisSender.WriteUMCCluster(matchedCluster.Cluster);
                thisSender.UpdatePlotsWithClusterData(matchedCluster);


                // Make sure that if a new feature is selected that we update the feature list.
                if (matchedCluster.Cluster.Features.Count > 0)
                {
                   // thisSender.m_featureGrid.SelectedFeature = matchedCluster.Cluster.Features[0];
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

            UMCClusterLight cluster = matchedCluster.Cluster;

            // Then start populating the charts with data.
            m_mainCluster                   = cluster;
            m_clusterChart.MainCluster      = matchedCluster;
            m_driftChart.MainCluster        = matchedCluster;
            //m_profileChart.MainCluster      = matchedCluster;
            //m_profileChart.UpdateCharts(true);
            
           //  m_featureGrid.Features          = cluster.Features;

            // Load the matching data.
            //List<ClusterToMassTagMap> tagMatches       = matchedCluster.ClusterMatches;            
            //m_masstagGrid.MassTags = tagMatches;

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
            otherClusters.ForEach(x => otherClusterMatches.Add(FeatureCacheManager<UMCClusterLightMatched>.FindById(x.ID)));

            foreach (UMCClusterLightMatched matchedOtherCluster in otherClusterMatches)
            {
                matchedOtherCluster.Cluster.Features.Clear();
                matchedOtherCluster.Cluster.ReconstructUMCCluster(Providers, false, false);
            }            
           // m_clusterGrid.Clusters = otherClusterMatches;




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
                        MSFeatureMsMs map           = new MSFeatureMsMs();
                        map.FeatureID               = msFeature.ID;
                        map.FeatureGroupID          = msFeature.GroupID;
                        map.Mz                      = msFeature.Mz;
                        map.PrecursorMZ             = spectrum.PrecursorMZ;                     
                        map.FeatureScan             = msFeature.Scan;
                        map.MsMsScan                = spectrum.Scan;
                        map.MassMonoisotopicAligned = msFeature.MassMonoisotopicAligned;
                        map.ChargeState             = msFeature.ChargeState;
                        map.Sequence                = "";
                        map.PeptideSequence         = "";
                        msmsFeatures.Add(map);
                    }
                }
            }
            m_msmsGrid.MsMsSpectra = new System.Collections.ObjectModel.ObservableCollection<MSFeatureMsMs>(msmsFeatures);

            float count = 0;
            List<KeyValuePair<float, float>> distances = new List<KeyValuePair<float, float>>();
            List<UMCLight> features = new List<UMCLight>();
            features.AddRange(matchedCluster.Cluster.Features);                       
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
        /// <summary>
        /// Gets or sets the analysis used.
        /// </summary>
        public MultiAlignAnalysis Analysis
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

        private void exportMsMsButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result =  m_saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path                         = m_saveFileDialog.FileName;
                IMsMsSpectraWriter writer           = MsMsFileWriterFactory.CreateSpectraWriter(System.IO.Path.GetExtension(path));                                                
                List<DatasetInformation> datasets   = Analysis.MetaData.Datasets.ToList();
                Cluster.Cluster.ExportMsMs(m_saveFileDialog.FileName, datasets, writer);           
            }
        }
    }
}
