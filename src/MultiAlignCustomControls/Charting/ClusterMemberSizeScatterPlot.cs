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
	public class ClusterMemberSizeScatterPlot : ctlScatterChart
    {
        #region Members
        private int mint_pt_size = 2;
        private System.ComponentModel.IContainer components = null;				
        private List<UMCClusterLight> m_clusters;        
        #endregion

        #region Constructors
        public ClusterMemberSizeScatterPlot()
	    {			
			InitializeComponent();
            m_clusters = new List<UMCClusterLight>();                        
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
            m_clusters.Clear();
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddClusters(List<UMCClusterLight> clusters)
        {
            m_clusters.AddRange(clusters);            
        }
        public void UpdateCharts(bool shouldAutoViewport)
        {
            SeriesCollection.Clear();
            AddClusterDataToChart(m_clusters, true);
            
            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }
        #endregion

        public bool DisplayRatio
        {
            get;
            set;
        }
        
        #region Cluster Rendering        
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void AddClusterDataToChart(List<UMCClusterLight> clusters, bool isAlternate)
        {                      
            List<float> ratioList   = new List<float>();
            List<float> indexList   = new List<float>();

            int i = 0;
            foreach(UMCClusterLight cluster in clusters)
            {
                if (DisplayRatio)
                {
                    double ratio = Convert.ToDouble(cluster.MemberCount) / Convert.ToDouble(cluster.DatasetMemberCount);
                    ratioList.Add(Convert.ToSingle(ratio));
                }
                else
                {
                    ratioList.Add(Convert.ToSingle(cluster.MemberCount));
                }
                indexList.Add(Convert.ToSingle(cluster.DatasetMemberCount));
            }
                            
            clsShape shape              = new BubbleShape(mint_pt_size, false);
            clsPlotParams plotParams    = new clsPlotParams(shape, Color.Black);

            float[] x = new float[indexList.Count]; 
            float[] y = new float[ratioList.Count];
            
            ratioList.CopyTo(y);
            indexList.CopyTo(x);

            clsSeries series = new clsSeries(ref x, ref y, plotParams);
            base.AddSeries(series);                            
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

