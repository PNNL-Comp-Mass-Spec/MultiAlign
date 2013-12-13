using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Extensions;
using MultiAlignCustomControls.Extensions;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data;
using MultiAlignCore.Extensions;

namespace MultiAlignCustomControls.Charting
{    
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class SingleClusterChart : FeatureScatterPlot
    {
        #region Members
        private int mint_pt_size = 3;
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ;
        private List<UMCClusterLightMatched> m_additionalClusters;
        private List<MassTagLight> m_massTags;
        private UMCClusterLightMatched m_mainCluster;
        /// <summary>
        /// Shapes for other clusters
        /// </summary>
        private List<clsShape> m_shapes;
        /// <summary>
        /// Shapes for the main cluster
        /// </summary>
        private List<clsShape> m_mainShapes;
        /// <summary>
        /// Shapes for cluster centroids
        /// </summary>
        private List<clsShape> m_clusterShapes;
        #endregion

        #region Constructors
        public SingleClusterChart()
	    {			
			InitializeComponent();

            IsClipboardCopyEnabled  = true;
            m_additionalClusters    = new List<UMCClusterLightMatched>();
            m_massTags              = new List<MassTagLight>();
            m_mainCluster           = null;
            AlternateColor          = Color.Black;
            IsDriftTimeXAxis        = false;
            XAxisShortHand          = "NET";
            DrawFeatures            = true;
            m_shapes                = ShapeIterator.CreateShapeList(mint_pt_size, true);
            m_mainShapes            = ShapeIterator.CreateShapeList(mint_pt_size, false);
            m_clusterShapes         = ShapeIterator.CreateShapeList(mint_pt_size + 2, false);
                        
            AddPostProcessor(new ChartPostRenderingProcessor(RenderDifferences), PostProcessPriority.MidHigh);
        }
                        
        private void RenderDifferences(ctlChartBase chart, PostRenderEventArgs args)
        {
            if (m_mainCluster == null)
                return;

            DisplayDifferences(m_mainCluster.Cluster.Features, chart, args);
        }
        private void DisplayDifferences(List<UMCLight> features, ctlChartBase chart, PostRenderEventArgs args)
        {
            double minMass, maxMass, minNet, maxNet, minDrift, maxDrift;

            m_mainCluster.Cluster.Features.FindRanges(out minMass,
                                                out maxMass,
                                                out minNet,
                                                out maxNet,
                                                out minDrift,
                                                out maxDrift);

            double massDifference = Feature.ComputeMassPPMDifference(minMass, maxMass);
            massDifference = Math.Abs(massDifference);
            double netDifference = Math.Abs(maxNet - minNet);
            double driftDifference = Math.Abs(maxDrift - minDrift);

            string xAddString = XAxisShortHand;
            Graphics graphics = args.Graphics;
            if (IsDriftTimeXAxis)
            {
                minNet = minDrift;
                maxNet = maxDrift;
                netDifference = driftDifference;
            }


            float netLineRight = this.mobj_axis_plotter.XScreenPixel(Convert.ToSingle(maxNet));
            float netLineLeft = this.mobj_axis_plotter.XScreenPixel(Convert.ToSingle(minNet));
            float netString = netLineLeft + ((netLineRight - netLineLeft) / 4);

            float massLineTop = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(maxMass));
            float massLineBot = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(minMass));
            float massString = massLineBot + ((massLineTop - massLineBot) / 2);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                using (Pen pen = new Pen(brush))
                {
                    graphics.DrawLine(pen, netLineRight + 10, massLineTop, netLineRight + 10, massLineBot);
                    graphics.DrawLine(pen, netLineLeft, massLineTop - 10, netLineRight, massLineTop - 10);
                    graphics.DrawString(string.Format("{0:.0} PPM", massDifference), Font, brush, netLineRight + 20, massString);
                    graphics.DrawString(string.Format("{0:.000} {1}", netDifference, xAddString), Font, brush, netString, massLineTop - 40);


                } 
            }


                clsColorIterator colors = new clsColorIterator(); 
                float offset            = Convert.ToSingle(MaxChartAreaXPixel) * .1F;
                float width             = Convert.ToSingle(MaxChartAreaXPixel);
                float direction         = offset* -1;
                float directionOffset   = -1;

                // Renders the peptide strings...
                if (m_mainCluster != null)
                {
                    Font newFont = new Font(Font, FontStyle.Bold);
                    /// Quickest way to access the feature and the peptides it has...for a given cluster
                    foreach (UMCLight feature in m_mainCluster.Cluster.Features)
                    {
                        float msMassScan    = mobj_axis_plotter.YScreenPixel(Convert.ToSingle(feature.MassMonoisotopicAligned));
                        float idOffset      = 0;
                                                
                        foreach (MSFeatureLight msFeature in feature.MSFeatures)
                        {
                            Color color        = colors.GetColor(msFeature.ChargeState);
                            using (Brush brush = new SolidBrush(color))
                            {
                                using (Pen pen = new Pen(brush, 3.0F))
                                {

                                foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                                {
                                    foreach (Peptide p in spectrum.Peptides)
                                    {
                                        float msScan     = mobj_axis_plotter.XScreenPixel(Convert.ToSingle(feature.RetentionTime));

                                        // Draw the precursor strings.
                                        string precursorString = string.Format("{0}", p.Sequence);
                                        SizeF sizeOfPrecursor = graphics.MeasureString(precursorString, Font);

                                        float xPositionPrecursorString = msScan + offset + 2;
                                        if (xPositionPrecursorString + sizeOfPrecursor.Width >= width)
                                        {
                                            xPositionPrecursorString = width - sizeOfPrecursor.Width - 2;
                                        }

                                        graphics.DrawLine(pen, msScan, msMassScan,    xPositionPrecursorString - 2,        msMassScan + direction);
                                        graphics.DrawString(precursorString, newFont, brush, xPositionPrecursorString, msMassScan + direction + idOffset);

                                        idOffset += (sizeOfPrecursor.Height + 2) * directionOffset;
                                    }
                                }
                                }
                            }
                        }                             
                        // Alternate the direction of the MSMS precursor label
                        direction       *= -1;
                        directionOffset *= -1;
                    }
                }                                                                                 
        }

        #endregion

        #region Properties
        public Color AlternateColor
        {
            get;
            set;
        }
        public bool IsDriftTimeXAxis
        {
            get;
            set;
        }
        #endregion

        #region Copying to System Clipboard
        public override void  CopyDataToClipboard()
        {
 	        string clusterData = "";
            string featureData = "";


            clusterData += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                                                            m_mainCluster.Cluster.ID,                                                             
                                                            m_mainCluster.Cluster.MassMonoisotopicAligned, 
                                                            m_mainCluster.Cluster.RetentionTime,
                                                            m_mainCluster.Cluster.DriftTime,
                                                            m_mainCluster.Cluster.ChargeState
                                                            );

            foreach (UMCClusterLightMatched matched in m_additionalClusters)
            {
                
                UMCClusterLight cluster = matched.Cluster;
                clusterData += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n", cluster.ID, 
                    cluster.MassMonoisotopicAligned,
                    cluster.RetentionTime,
                    cluster.DriftTime,
                    cluster.ChargeState);

                foreach (UMCLight feature in cluster.Features)
                {
                    featureData += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",
                        feature.ClusterID,
                        feature.GroupID,
                        feature.ID,
                        feature.MassMonoisotopicAligned,
                        feature.RetentionTime,
                        feature.DriftTime,
                        feature.ChargeState);
                }
            }

            foreach (UMCLight feature in m_mainCluster.Cluster.Features)
            {
                featureData += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",
                    feature.ClusterID,
                    feature.GroupID,
                    feature.ID,
                    feature.MassMonoisotopicAligned, 
                    feature.RetentionTime,
                    feature.DriftTime,
                        feature.ChargeState);
            }

            clusterData = "Cluster Id\tMono Mass\tNET\tDrift Time\tCharge\n" + clusterData;
            clusterData += "Cluster Id\tDataset Id\tFeature Id\tMono Mass\tNET\tDrift Time\tcharge\n" + featureData;
            ApplicationClipboard.SetData(clusterData);
        }
        #endregion

        #region Data Addition Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {            
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
            m_additionalClusters.Clear();
            m_massTags.Clear();
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddAdditionalClusters(List<UMCClusterLightMatched> clusters)
        {
            m_additionalClusters.AddRange(clusters);            
        }
        public void AddAdditionalClusters(UMCClusterLightMatched cluster)
        {                        
            m_additionalClusters.Add(cluster);
            
        }
        public void AddMassTags(List<MassTagLight> tags)
        {            
            m_massTags.AddRange(tags);            
        }
        public void AddMassTags(MassTagLight tag)
        {
            m_massTags.Add(tag);
        }

        public void UpdateCharts(bool shouldAutoViewport)
        {
            SeriesCollection.Clear();
            AddClusterDataToChart(m_additionalClusters, true);
            AddClusterDataToChart(m_mainCluster, false);
            AddMassTagsToChart(m_massTags);

            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }
        public UMCClusterLightMatched MainCluster
        {
            get
            {
                return m_mainCluster;
            }
            set
            {                
                m_mainCluster = value;
                if (value != null)
                {
                    m_massTags.Clear();
                    foreach (ClusterToMassTagMap map in value.ClusterMatches)
                    {
                        m_massTags.Add(map.MassTag.MassTag);
                    }
                }
            }
        }
        #endregion
        
        #region Cluster Rendering
        /// <summary>
        /// Adds the mass tags to the data chart.
        /// </summary>
        /// <param name="tags"></param>
        private void AddMassTagsToChart(List<MassTagLight> tags)
        {
            if (tags.Count < 1)
                return;

            clsColorIterator colors     = new clsColorIterator();            
            List<float> massList        = new List<float>();
            List<float> scanList        = new List<float>();
            Color color                 = Color.Orange;
            clsShape shape              = new DiamondShape(mint_pt_size + 4, false); 
            clsPlotParams plotParams    = new clsPlotParams(shape, color);

            tags.ForEach(x => massList.Add(Convert.ToSingle(x.MassMonoisotopic)));
            
            if (IsDriftTimeXAxis)
            {
                tags.ForEach(x => scanList.Add(Convert.ToSingle(x.DriftTime)));
            }
            else
            {
                tags.ForEach(x => scanList.Add(Convert.ToSingle(x.NETAverage)));
            }            
            float[] masses = new float[massList.Count];
            float[] scans = new float[scanList.Count];

            massList.CopyTo(masses);
            scanList.CopyTo(scans);
            clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
            base.AddSeries(series);            
        }
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void AddClusterDataToChart(List<UMCClusterLightMatched> clusters, bool isAlternate)
        {
            foreach (UMCClusterLightMatched cluster in clusters)
            {
                AddClusterDataToChart(cluster, isAlternate);
            }
        }
        public bool DrawFeatures
        {
            get;
            set;
        }
        /// <summary>
        /// Adds cluster data to the chart.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="isAlternate"></param>
        private void AddClusterDataToChart(UMCClusterLightMatched matchedCluster, bool isAlternate)
        {
            UMCClusterLight cluster = matchedCluster.Cluster;
            clsColorIterator colors = new clsColorIterator();                        
            List<float> massList    = new List<float>();
            List<float> scanList    = new List<float>();
            Color color             = colors.GetColor(0);
                
            int charge          = Math.Min(cluster.ChargeState, m_shapes.Count);
            charge              = Math.Max(0, charge - 1);
            clsShape shape      = null;

            if (isAlternate)
            {
                color = AlternateColor;
                shape = m_shapes[charge];
            }
            else
            {
                shape = m_mainShapes[charge];
            }
    
            if (cluster.Features.Count > 0 && DrawFeatures)
            {
                clsPlotParams plotParams = new clsPlotParams(shape, color);
                cluster.Features.ForEach(x => massList.Add(Convert.ToSingle(x.MassMonoisotopicAligned)));
                if (IsDriftTimeXAxis)
                {
                    cluster.Features.ForEach(x => scanList.Add(Convert.ToSingle(x.DriftTime)));
                }
                else
                {
                    cluster.Features.ForEach(x => scanList.Add(Convert.ToSingle(x.RetentionTime)));
                }
           
                float[] masses = new float[massList.Count];
                float[] scans  = new float[scanList.Count];
                massList.CopyTo(masses);
                scanList.CopyTo(scans);
                
                plotParams.Name  = string.Format("Cluster {0} Features", cluster.ID);
                clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
                base.AddSeries(series);                
            }

            float[] clusterMass = new float[1];
            float[] clusterScan = new float[1];
            Color clusterColor = clusterColor = Color.FromArgb(200, Color.Red);
            bool isHollow       = isAlternate;

            string name = string.Format("Main Cluster {0} Charge {1}", cluster.ID, cluster.ChargeState); 
            if (isAlternate)
            {
                name = string.Format("Nearby Cluster {0} Charge {1}", cluster.ID, cluster.ChargeState);
                clusterColor = Color.FromArgb(200, Color.Lime);
            }            

            clusterMass[0] = Convert.ToSingle(cluster.MassMonoisotopic);
            if (IsDriftTimeXAxis)
            {
                clusterScan[0] = Convert.ToSingle(cluster.DriftTime);
            }
            else
            {
                clusterScan[0] = Convert.ToSingle(cluster.RetentionTime);
            }

            

            clsShape clusterShape           = m_clusterShapes[charge];
            clsPlotParams plotParamsCluster = new clsPlotParams(clusterShape, clusterColor);
            plotParamsCluster.Name          = name;
            clsSeries clusterSeries         = new clsSeries(ref clusterScan, ref clusterMass, plotParamsCluster);
            base.AddSeries(clusterSeries);

        }
        #endregion

       

        #region Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            PNNLControls.PenProvider penProvider1 = new PNNLControls.PenProvider();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // SingleClusterChart
            // 
            this.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ChartLayout.LegendFraction = 0.2F;
            this.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Right;
            this.ChartLayout.MaxLegendHeight = 150;
            this.ChartLayout.MaxLegendWidth = 250;
            this.ChartLayout.MaxTitleHeight = 50;
            this.ChartLayout.MinLegendHeight = 50;
            this.ChartLayout.MinLegendWidth = 75;
            this.ChartLayout.MinTitleHeight = 15;
            this.ChartLayout.TitleFraction = 0.1F;
            this.DefaultZoomHandler.Active = true;
            this.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.HasLegend = false;
            this.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider1.Color = System.Drawing.Color.Black;
            penProvider1.Width = 1F;
            this.Legend.BorderPen = penProvider1;
            this.Legend.Bounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.Legend.ColumnWidth = 125;
            this.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Legend.MaxFontSize = 11F;
            this.Legend.MinFontSize = 6F;
            this.LegendVisible = false;
            this.Margins.BottomMarginFraction = 0.1F;
            this.Margins.BottomMarginMax = 72;
            this.Margins.BottomMarginMin = 30;
            this.Margins.DefaultMarginFraction = 0.05F;
            this.Margins.DefaultMarginMax = 15;
            this.Margins.DefaultMarginMin = 5;
            this.Margins.LeftMarginFraction = 0.2F;
            this.Margins.LeftMarginMax = 150;
            this.Margins.LeftMarginMin = 72;
            this.Name = "SingleClusterChart";
            this.Size = new System.Drawing.Size(408, 382);
            this.Title = "Cluster Chart";
            this.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.TitleMaxFontSize = 15F;
            this.TitleVisible = false;
            this.XAxisLabel = "NET";
            this.YAxisLabel = "Monoisotopic Mass";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
        
    }
}

