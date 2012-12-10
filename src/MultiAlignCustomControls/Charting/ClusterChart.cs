using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data.Features;
using MultiAlignCustomControls.Extensions;

namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class ctlClusterChart : FeatureScatterPlot
    {
        #region Members

        private RectangleF m_viewportBox = new RectangleF(-1, -1, 1, 1);
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 
		private int mint_pt_size = 2 ; 		
        private List<UMCClusterLight> m_clusters;
        #endregion

        #region Constructors
        public ctlClusterChart()
	    {			
			InitializeComponent();

            m_clusters  = new List<UMCClusterLight>();
            AddPostProcessor(this.DrawViewportBox, PostProcessPriority.Mid);
        }

        protected override void DrawZoomValues(ctlChartBase chart, RectangleF rect, RectangleF bounds, Graphics graphics)
        {

            double ppmDiff = Math.Abs(Feature.ComputeMassPPMDifference(bounds.Top, bounds.Bottom));
            double netDiff = Convert.ToDouble(bounds.Width);

            string yString = string.Format("{0:.00} {1}", ppmDiff, YAxisShortHand);
            if (ppmDiff > 1000)
            {
                yString = string.Format("{0:.00} da", Math.Abs(bounds.Top - bounds.Bottom));
            }
            string xString = string.Format("{0:.00} {1}", netDiff, XAxisShortHand);

            base.DrawZoomValues(chart,
                            rect,
                            bounds,
                            xString,
                            yString,
                            graphics);            
        }
        
        void DrawViewportBox(ctlChartBase chart, PostRenderEventArgs args)
        {
            if (m_viewportBox.X < 0 && m_viewportBox.Y < 0)
            {
                return;
            }

            using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.IndianRed)))
            {
                using (Pen pen = new Pen(brush, 5))
                {
                    
                    float x         = base.mobj_axis_plotter.XScreenPixel(m_viewportBox.X);
                    float y         = base.mobj_axis_plotter.YScreenPixel(m_viewportBox.Y);
                    float right     = base.mobj_axis_plotter.XScreenPixel(m_viewportBox.Right);
                    float bottom    = base.mobj_axis_plotter.YScreenPixel(m_viewportBox.Bottom);
                    
                    float width     = Math.Abs(right  - x);
                    float height    = Math.Abs(y - bottom);
                        
                    if (width < 1)
                    {
                        width = 10;
                    }
                    if (height < 1)
                    {
                        height = 10;
                    }
                    
                    args.Graphics.DrawRectangle(pen,
                                                x,
                                                y - height,
                                                width,
                                                height);
                }
            }
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="dataset"></param>
        public ctlClusterChart(List<UMCClusterLight> clusters) :
            this()
        {
            AddClusters(clusters);
        }
        #endregion

        #region Data Addition Methods
        public void UpdateHighlightArea(RectangleF viewport)
        {
            m_viewportBox = viewport;
            Refresh();
        }
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {            
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
            m_clusters.Clear();
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddClusters(List<UMCClusterLight> clusters)
        {
            m_clusters.AddRange(clusters);
            SeriesCollection.Clear();
            AddClusterDataToChart(m_clusters);
            AutoViewPort();
            Refresh();            
        }
        public void AddClusters(UMCClusterLight cluster)
        {
            m_clusters.Add(cluster);
            AddClusterDataToChart(m_clusters);
            AutoViewPort();
            Refresh();
        }
        public void SetClusters(List<UMCClusterLight> clusters)
        {
            m_clusters.Clear();
            m_clusters.AddRange(clusters);
            SeriesCollection.Clear();
            AddClusterDataToChart(m_clusters);
            AutoViewPort(); 
            Refresh();
        }
        public void SetClusters(UMCClusterLight cluster)
        {
            m_clusters.Clear();
            m_clusters.Add(cluster);
            SeriesCollection.Clear();            
            AddClusterDataToChart(m_clusters);
            AutoViewPort();
            Refresh();
        }
        #endregion
        
        #region Cluster Rendering
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void AddClusterDataToChart(List<UMCClusterLight> clusters)
        {            

            clsColorIterator colors = new clsColorIterator();
            float maxY = 500;
            float minY = 0;
            float maxX = 500;
            float minX = 0;
            
            List<float> massList = new List<float>();
            List<float> scanList = new List<float>();
            Color color              = colors.GetColor(0);
            clsShape shape           = new BubbleShape(mint_pt_size, false);
            clsPlotParams plotParams = new clsPlotParams(shape, color);
            
            int clustersAdded = 0;
            foreach(UMCClusterLight cluster in clusters)
            {
                float x = 0;
                float y = 0;
                                      
                                            
                y = Convert.ToSingle(cluster.MassMonoisotopic);                                                                          
                x = Convert.ToSingle(cluster.RetentionTime);                            
                        
                massList.Add(y);
                scanList.Add(x);

                minX = Math.Min(x, minX);
                maxX = Math.Max(x, maxX);

                minY = Math.Min(y, minY);
                maxY = Math.Max(y, maxY);
                clustersAdded++;                    
            }

            if (clustersAdded > 0)
            {
                float[] masses = new float[massList.Count];
                float[] scans = new float[scanList.Count];

                massList.CopyTo(masses);
                scanList.CopyTo(scans);
                clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
                base.AddSeries(series);
            }            
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
            // ctlClusterChart
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
            this.Name = "ctlClusterChart";
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

