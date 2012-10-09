using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Data;
using PNNLControls;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Extensions;
using MultiAlignCore.Data.Features;

namespace MultiAlignCustomControls.Charting
{  
    /// <summary>
    /// Renders MS/MS Clusters 
    /// </summary>
	public class MsMsChart : ctlScatterChart
    {
        #region Members
        private int mint_pt_size = 2;
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 				                                       
        #endregion

        #region Constructors
        public MsMsChart()
	    {			
			InitializeComponent();
        }
        #endregion

        private void RenderDifferences(ctlChartBase chart, PostRenderEventArgs args)
        {
        }
        
        #region Data Addition Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
        }
        public void AddFeatures(List<MSFeatureMsMs> features)
        {
            PlotData(features);
        }
        public void UpdateCharts(bool shouldAutoViewport)
        {            
            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }
        #endregion
        
        #region Feature Rendering
        
        /// <summary>
        /// Adds cluster data to the chart.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="isAlternate"></param>
        private void PlotData(List<MSFeatureMsMs> features)
        {
            if (features.Count < 0)
                return;

            clsColorIterator colors = new clsColorIterator();            
            
            List<float> massList = new List<float>();
            List<float> scanList = new List<float>();
            Color color = colors.GetColor(0);

            foreach (MSFeatureMsMs feature in features)
            {                               
                massList.Add(Convert.ToSingle(feature.MassMonoisotopicAligned));
                scanList.Add(Convert.ToSingle(feature.MsMsScan));                
            }
            float[] masses = null;
            float[] scans  = null;
            if (massList.Count > 0)
            {
                // Add the series for MS/MS features
                masses  = new float[massList.Count];
                scans   = new float[scanList.Count];
                massList.CopyTo(masses);
                scanList.CopyTo(scans);

                int feature = features[0].FeatureGroupID;
                clsShape shape              = new BubbleShape(mint_pt_size, false);
                clsPlotParams plotParams    = new clsPlotParams(shape, miter_color.GetColor(feature));
                plotParams.Name             = feature.ToString();
                clsSeries series            = new clsSeries(ref scans, ref masses, plotParams);
                base.AddSeries(series);
                massList.Clear();
                scanList.Clear();
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
            this.XAxisLabel = "m/z";
            this.YAxisLabel = "Monoisotopic Mass";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        #region Display Event Handlers
       
        public void UpdateDisplay()
        {
            AutoViewPortOnAddition = false;
            this.SeriesCollection.Clear();            
            XAxisLabel = "NET";              
            YAxisLabel = "Monoisotopic Mass";            
        }
        #endregion
    }
}

