using System;
using System.Collections.Generic;
using System.Drawing;
using MultiAlignCore.Extensions;
using MultiAlignCustomControls.Drawing;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCustomControls.Charting
{

    

    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class ClusterErrorScatterPlot : FeatureScatterPlot
    {
        #region Members
        private int mint_pt_size = 3;
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 				                   
        private List<UMCClusterLight> m_additionalClusters;
        private List<MassTagLight> m_massTags;
        private UMCClusterLight m_mainCluster;
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
        public ClusterErrorScatterPlot()
	    {			
			InitializeComponent();

            m_additionalClusters    = new List<UMCClusterLight>();
            m_massTags              = new List<MassTagLight>();
            m_mainCluster           = null;
            AlternateColor          = Color.Black;
            IsDriftTimeXAxis        = false;

            m_shapes                = ShapeIterator.CreateShapeList(mint_pt_size, true);
            m_mainShapes            = ShapeIterator.CreateShapeList(mint_pt_size, false);
            m_clusterShapes         = ShapeIterator.CreateShapeList(mint_pt_size + 2, false);
            

            AddPostProcessor(new ChartPostRenderingProcessor(RenderDifferences), PostProcessPriority.MidHigh);            
        }
        #endregion

        protected override void DrawZoomValues(ctlChartBase chart, RectangleF rect, RectangleF bounds, Graphics graphics)
        {
            double ppmDiff = Math.Abs(bounds.Top - bounds.Bottom);
            double netDiff = Convert.ToDouble(bounds.Width);

            base.DrawZoomValues(chart,
                            rect,
                            bounds,
                            netDiff,
                            ppmDiff,
                            graphics);
        }

        /// <summary>
        /// Draws the differences (bounds) of a cluster on the chart.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="chart"></param>
        /// <param name="args"></param>
        private void DisplayDifferences(List<UMCLight> features, ctlChartBase chart, PostRenderEventArgs args)
        {
            double minMass, maxMass, minNet, maxNet, minDrift, maxDrift;

            m_mainCluster.Features.FindRanges(out minMass,
                                                out maxMass,
                                                out minNet,
                                                out maxNet,
                                                out minDrift,
                                                out maxDrift);

            double massDifference = Feature.ComputeMassPPMDifference(minMass, maxMass);
            massDifference = Math.Abs(massDifference);
            double netDifference = Math.Abs(maxNet - minNet);
            double driftDifference = Math.Abs(maxDrift - minDrift);

            string xAddString = "NET";
            Graphics graphics = args.Graphics;
            if (IsDriftTimeXAxis)
            {
                minNet = minDrift;
                maxNet = maxDrift;
                netDifference = driftDifference;
                xAddString = "dt";
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
        }
        
       private void RenderDifferences(ctlChartBase chart, PostRenderEventArgs args)
        {
            if (m_mainCluster == null)
                return;

            DisplayDifferences(m_mainCluster.Features, chart, args);
        }

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
        public void AddAdditionalClusters(List<UMCClusterLight> clusters)
        {
            m_additionalClusters.AddRange(clusters);            
        }
        public void AddAdditionalClusters(UMCClusterLight cluster)
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
        public UMCClusterLight MainCluster
        {
            get
            {
                return m_mainCluster;
            }
            set
            {                
                m_mainCluster = value;
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
            Color color                 = Color.RoyalBlue;
            clsShape shape              = new DiamondShape(4, false); 
            clsPlotParams plotParams    = new clsPlotParams(shape, color);

            tags.ForEach(x => massList.Add(Convert.ToSingle(x.MassMonoisotopic)));
            
            if (IsDriftTimeXAxis)
            {
                tags.ForEach(x => massList.Add(Convert.ToSingle(x.DriftTime)));
            }
            else
            {
                tags.ForEach(x => massList.Add(Convert.ToSingle(x.NETAverage)));
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
        private void AddClusterDataToChart(List<UMCClusterLight> clusters, bool isAlternate)
        {
            foreach (UMCClusterLight cluster in clusters)
            {
                AddClusterDataToChart(cluster, isAlternate);
            }
        }
        /// <summary>
        /// Adds cluster data to the chart.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="isAlternate"></param>
        private void AddClusterDataToChart(UMCClusterLight cluster, bool isAlternate)
        {             
            clsColorIterator colors = new clsColorIterator();            
            if (cluster.Features.Count > 0)
            {
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

                double clusterX = m_mainCluster.RetentionTime; 
                double clusterY = m_mainCluster.MassMonoisotopic;                
                if (IsDriftTimeXAxis)
                    clusterX = m_mainCluster.DriftTime;

                clsPlotParams plotParams = new clsPlotParams(shape, color);
                cluster.Features.ForEach(x => massList.Add(Convert.ToSingle(Feature.ComputeMassPPMDifference(clusterY, x.MassMonoisotopicAligned))));
                if (IsDriftTimeXAxis)
                {
                    cluster.Features.ForEach(x => scanList.Add(Convert.ToSingle(clusterX - x.DriftTime)));
                }
                else
                {
                    cluster.Features.ForEach(x => scanList.Add(Convert.ToSingle(clusterX - x.RetentionTime)));
                }
           
                float[] masses = new float[massList.Count];
                float[] scans  = new float[scanList.Count];
                massList.CopyTo(masses);
                scanList.CopyTo(scans);

                clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
                base.AddSeries(series);                
            }            
        }
        #endregion

        #region Mass Tag Display
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        private void AddMassTagDatabasePointsToChart(clsMassTagDB database) 
        {

            int i = 0;
            int numberOfTags = database.GetMassTagCount(); 

            float[] masses = new float[numberOfTags];
            float[] scans = new float[numberOfTags];
            float[] charges = new float[numberOfTags];

            List<float> massList = new List<float>();
            List<float> scanList = new List<float>();

            clsShape shape = new CrossShape(mint_pt_size + 3, true);
            clsPlotParams plotParams = new clsPlotParams(shape, Color.FromArgb(64, Color.DarkOrange));

            
            while (i < numberOfTags)
            {
                clsMassTag tag = database.GetMassTagFromIndex(i++);
                massList.Add(Convert.ToSingle(tag.mdblMonoMass));
                scanList.Add(Convert.ToSingle(tag.mdblAvgGANET));                
            }
            massList.CopyTo(masses);
            scanList.CopyTo(scans);
            plotParams.Name = "Mass Tags";
            clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
            AddSeries(series);
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

