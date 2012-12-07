using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Extensions;
using PNNLControls;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.MassTags;

namespace Manassa.Windows
{    
    /// <summary>
    /// Interaction logic for ClusterControl.xaml
    /// </summary>
    public partial class MassTagControl : UserControl
    {
        private System.Windows.Forms.SaveFileDialog  m_featureImageSaveDialog;        
        private bool m_adjustingFeaturePlots = false;
        private Dictionary<ctlChartBase, List<ChartSynchData>> m_chartSynchMap;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MassTagControl()
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

            SynchViewports(m_massTagChart,  ChartSynchType.YAxis,   m_driftChart);
            SynchViewports(m_driftChart,    ChartSynchType.YAxis,   m_massTagChart);            
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
                UMCLight feature                        = (UMCLight)e.NewValue;                
                thisSender.m_msFeaturePlot.MainFeature  = feature;

                thisSender.m_sicChart.SetFeature(feature); 
                thisSender.m_sicChart.AutoViewPort();
                thisSender.m_msFeaturePlot.UpdateCharts(true);

                thisSender.m_sicChart.AdjustViewPortWithTolerances(thisSender.FeatureFindingTolerances, false);
                thisSender.m_msFeaturePlot.AdjustViewPortWithTolerances(thisSender.FeatureFindingTolerances, false);

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
            var thisSender = (MassTagControl)sender;
            if (e.NewValue != null)
            {
                MassTagToCluster tag = e.NewValue as MassTagToCluster;
                if (tag == null)
                    return;

                

                lock (thisSender.Providers.Synch)
                {
                    foreach (UMCClusterLightMatched cluster in tag.Matches)
                    {
                        cluster.Cluster.ReconstructUMCCluster(thisSender.Providers, false, false);
                    }
                }

                thisSender.UpdatePlotsWithClusterData(tag);
            }
        }
        #endregion

        /// <summary>
        /// Dependency Property
        /// </summary>
        public MassTagToCluster MassTag
        {
            get 
            {
                return (MassTagToCluster)GetValue(ClusterProperty);
            }
            set 
            { 
                SetValue(ClusterProperty, value);
            }
        }                

        // Using a DependencyProperty as the backing store for Cluster.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClusterProperty =            
            DependencyProperty.Register("MassTag",
                                        typeof(MassTagToCluster), 
                                        typeof(MassTagControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(ClusterSet)));



        public RectangleF Viewport
        {
            get { return (RectangleF)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Viewport.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(RectangleF), typeof(MassTagControl));

                        
        #region Plot updating.
        /// <summary>
        /// Updates the plots with data stored in the cache.  
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="cluster"></param>
        private void UpdatePlotsWithClusterData(MassTagToCluster matchedTag)
        {
            // Clear the charta
            m_massTagChart.ClearData();
            m_driftChart.ClearData();

            MassTagLight massTag    = matchedTag.MassTag;
            

             m_clusterGrid.Clusters  = matchedTag.Matches;     
                                    
            // Then we find all the nearby clusters
            double massPpm  = ClusterTolerances.Mass;
            double net      = ClusterTolerances.RetentionTime;
            double minMass  = Feature.ComputeDaDifferenceFromPPM(massTag.MassMonoisotopic, massPpm);
            double maxMass  = Feature.ComputeDaDifferenceFromPPM(massTag.MassMonoisotopic, -massPpm);
            double minNet   = massTag.NETAverage - net;
            double maxNet   = massTag.NETAverage + net;

            List<MassTagLight> otherTags = Providers.MassTags.FindNearby(minMass, maxMass, minNet, maxNet);

            // Remove self from the list
            int index = otherTags.FindIndex(delegate(MassTagLight x)
            {
                return (x.ID == massTag.ID) && (x.ConformationID == massTag.ConformationID);
            });
            if (index > -1)
            {
                otherTags.RemoveAt(index);
            }

            List<MassTagToCluster> matchedOtherTags = new List<MassTagToCluster>();
            otherTags.ForEach(x => matchedOtherTags.Add(FeatureCacheManager<MassTagToCluster>.FindById(x.BuildId())));

            m_massTagChart.MassTag = matchedTag;
            m_massTagChart.AddAdditionalMassTags(matchedOtherTags);
            m_massTagChart.UpdateCharts(true);
            m_massTagChart.AdjustViewPortWithTolerances(ClusterTolerances, false);

            /// Add the other matched tags to this list.
            m_masstagGrid.MassTags = matchedOtherTags;

            m_driftChart.MassTag = matchedTag;
            m_driftChart.AddAdditionalMassTags(matchedOtherTags);
            m_driftChart.UpdateCharts(true);
            m_driftChart.AdjustViewPortWithTolerances(ClusterTolerances,   true);
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

            //m_featureImageSaveDialog.FileName = string.Format("cluster-{0}.png", m_mainCluster.ID);
            System.Windows.Forms.DialogResult result = m_featureImageSaveDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string extension = System.IO.Path.GetExtension(m_featureImageSaveDialog.FileName);
                string filename  = System.IO.Path.GetFileNameWithoutExtension(m_featureImageSaveDialog.FileName);
                string pathName  = System.IO.Path.GetDirectoryName(m_featureImageSaveDialog.FileName);

                try
                {
                   // m_clusterChart.ToBitmap(640, 640).Save(System.IO.Path.Combine(pathName, string.Format("{0}-massnet.{1}", filename, extension)));
                   // m_driftChart.ToBitmap(640, 640).Save(System.IO.Path.Combine(pathName, string.Format("{0}-massdrifttime.{1}", filename, extension)));
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
