using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;

namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class SpectraChart : ctlLineChart
    {
        #region Members
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 
		private int mint_pt_size = 2 ; 		
        private Dictionary<string, List<XYData>> m_features;

        clsColorIterator colors = new clsColorIterator();
        #endregion

        #region Constructors
        public SpectraChart()
	    {
			InitializeComponent();
            m_features = new Dictionary<string, List<XYData>>(); 
            DrawSticks = true;            
        }
        #endregion        
        
        #region Data Addition Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {
            m_features.Clear();
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void SetSpectra(List<XYData> feature)
        {            
            SeriesCollection.Clear();
            m_features.Clear();
            m_features.Add("", feature);
            
            PlotFeature(feature, Color.Blue, "");

        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddSpectra(List<XYData> feature, string name)
        {
            AddSpectra(feature, name, Color.Red);
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddSpectra(List<XYData> feature, string name, Color color)
        {
            if (m_features.ContainsKey(name))
                m_features[name] = feature;
            else
                m_features.Add(name, feature);
            
            PlotFeature(feature, color, name);
        }      
    
        #endregion
        
        #region Cluster Rendering
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void PlotFeature(List<XYData> feature, Color color, string name)
        {            

            

            /// Sort by m/z
            feature.Sort(delegate(XYData x, XYData y)
            {
                return x.X.CompareTo(y.X);
            });

            List<float> intensities     = new List<float>();
            List<float> scanList        = new List<float>();                
            clsShape shape              = new BubbleShape(mint_pt_size, false);
            clsPlotParams plotParams    = new clsPlotParams(shape, color);
            plotParams.Name             = name;

            foreach (XYData datum in feature)
            {
                    scanList.Add(Convert.ToSingle(datum.X));
                    intensities.Add(Convert.ToSingle(datum.Y));
            }                
            // Then plot.
            if (scanList.Count < 1)
                return;

            float[] intensity = new float[intensities.Count];
            float[] scans     = new float[scanList.Count];

            intensities.CopyTo(intensity);
            scanList.CopyTo(scans);

            clsSeries series = new clsSeries(ref scans, ref intensity, plotParams);
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
            // SICChart
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
            this.Name = "SICChart";
            this.Size = new System.Drawing.Size(408, 382);
            this.Title = "SIC Chart";
            this.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.TitleMaxFontSize = 15F;
            this.TitleVisible = false;
            this.XAxisLabel = "MS Scan";
            this.YAxisLabel = "Intensity";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
    }
}


