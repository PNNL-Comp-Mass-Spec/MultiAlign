using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.Features;

namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class AbundanceProfileChart : ctlScatterChart
    {
        #region Members
        private System.ComponentModel.IContainer components = null;		
		private int mint_pt_size = 5 ; 
        private UMCClusterLightMatched m_mainCluster;
        /// <summary>
        /// Flag indicating whether to use log transformed data or not.
        /// </summary>
        private bool m_useLogTransform;
        #endregion

        #region Constructors
        public AbundanceProfileChart()
	    {			
			InitializeComponent();       
            m_mainCluster = null;
            
            MenuItem logTransform   = new MenuItem();
            logTransform.Text       = "Use Log_2 Transform";
            logTransform.Checked    = true;
            logTransform.Click      += new EventHandler(logTransform_Click);            
            base.ContextMenu.MenuItems.Add(logTransform); 
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
           
        }

        void logTransform_Click(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item == null)
            {
                return;
            }

            item.Checked = (item.Checked == false);
            ShouldDisplayLogTransformed = item.Checked;
        }
        public bool ShouldDisplayLogTransformed
        {
            get
            {
                return m_useLogTransform;
            }
            set
            {
                m_useLogTransform = value;
                UpdateCharts(true);
            }
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public UMCClusterLightMatched MainCluster 
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
        public void UpdateCharts(bool shouldAutoViewport)
        {
            SeriesCollection.Clear();
            AddDataToChart(m_mainCluster);

            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void AddDataToChart(UMCClusterLightMatched cluster)
        {
            if (cluster == null)
                return;
            if (cluster.Cluster == null)
                return;
            if (cluster.Cluster.Features.Count < 1)
                return;

            clsColorIterator iterator = new clsColorIterator();

            Dictionary<int, List<UMCLight>> datasetMap = new Dictionary<int, List<UMCLight>>();
            List<UMCLight> features = new List<UMCLight>();
            cluster.Cluster.Features.ForEach(x => features.Add(x));
            features.Sort(delegate(UMCLight x, UMCLight y)
            {
                return x.GroupID.CompareTo(y.GroupID);
            });

            foreach (UMCLight feature in features)
            {
                if (!datasetMap.ContainsKey(feature.GroupID))
                {
                    datasetMap.Add(feature.GroupID, new List<UMCLight>());
                }
                datasetMap[feature.GroupID].Add(feature);
            }

            
            bool shouldTransform = ShouldDisplayLogTransformed;
            foreach (int key in datasetMap.Keys)
            {

                List<float> datasetList = new List<float>();
                List<float> abundanceList = new List<float>();


                Color color                 = iterator.GetColor(key);
                clsShape shape              = new BubbleShape(mint_pt_size, false);
                clsPlotParams plotParams    = new clsPlotParams(shape, color);


                float x = Convert.ToSingle(key);
                float y = 0;
                foreach (UMCLight feature in datasetMap[key])
                {

                    if (shouldTransform)
                    {
                        y += Convert.ToSingle(Math.Log(Convert.ToDouble(feature.AbundanceSum), 2));
                    }
                    else
                    {
                        y += Convert.ToSingle(feature.AbundanceSum);
                    }

                }
                datasetList.Add(x);
                abundanceList.Add(y);

                float[] xs = new float[datasetList.Count];
                float[] ys = new float[abundanceList.Count];

                datasetList.CopyTo(xs);
                abundanceList.CopyTo(ys);

                clsSeries series = new clsSeries(ref xs, ref ys, plotParams);
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
            // AbundanceProfileChart
            // 
            this.AxisAndLabelMinFontSize = 10;
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
            this.Name = "AbundanceProfileChart";
            this.Size = new System.Drawing.Size(408, 382);
            this.Title = "Abundance Profile";
            this.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.TitleMaxFontSize = 15F;
            this.TitleVisible = false;
            this.XAxisLabel = "Dataset ID";
            this.YAxisLabel = "Intensity";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
    }
}

