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
	public class SICChart : ctlLineChart
    {
        #region Members
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 
		private int mint_pt_size = 2 ; 		
        private UMCLight m_feature;
        #endregion

        #region Constructors
        public SICChart()
	    {			
			InitializeComponent();
            m_feature = null;

            AddPostProcessor(new ChartPostRenderingProcessor(RenderMSMS), PostProcessPriority.MidHigh);
        }
        #endregion

        #region MSMS Processing Display
        private void RenderMSMS(ctlChartBase chart, PostRenderEventArgs args)
        {
            if (m_feature == null)
                return;

            RenderMSMS(m_feature, chart, args);
        }
        /// <summary>
        /// Displays the MS/MS values onto the screen.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="chart"></param>
        /// <param name="args"></param>
        private void RenderMSMS(UMCLight feature, ctlChartBase chart, PostRenderEventArgs args)
        {            
            long minIntensity = long.MaxValue;
            feature.MSFeatures.ForEach(x => minIntensity = Math.Min(minIntensity, x.Abundance));

            Graphics graphics = args.Graphics;
            foreach (MSFeatureLight msFeature in feature.MSFeatures)
            {
                foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                {
                    float scan          = mobj_axis_plotter.XScreenPixel(Convert.ToSingle(spectrum.Scan));                                        
                    float intensity     = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(msFeature.Abundance));   
                    float pixelBottom   = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(minIntensity));                           
                    
                    using (Brush brush = new SolidBrush(Color.Red))
                    {
                        using (Pen pen = new Pen(brush, 8.0F))
                        {
                            graphics.DrawLine(pen, scan, pixelBottom, scan, intensity);
                        }
                    }
                }
            }           
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
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void SetFeature(UMCLight feature)
        {            
            SeriesCollection.Clear();
            m_feature = feature;
            PlotFeature();                                    
        }        
        #endregion
        
        #region Cluster Rendering
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void PlotFeature()
        {            
            if (m_feature == null)
                return;

            clsColorIterator colors     = new clsColorIterator();  
            Color color                 = colors.GetColor(0);
            
            Dictionary<int, List<XYZData>> charges = m_feature.CreateChargeSIC();

            foreach (int charge in charges.Keys)
            {                                      
                List<float> intensities     = new List<float>();
                List<float> scanList        = new List<float>();                
                clsShape shape              = new BubbleShape(mint_pt_size, false);
                clsPlotParams plotParams    = new clsPlotParams(shape, color);

                // Sort by scan.
                List<XYZData> data = charges[charge];
                data.Sort(delegate(XYZData x, XYZData y)
                {
                    return x.X.CompareTo(y.X);
                });

                // Extract the data 
                foreach (XYZData datum in data)
                {
                    scanList.Add(Convert.ToSingle(datum.X));
                    intensities.Add(Convert.ToSingle(datum.Y));
                }
                
                // Then plot.
                if (scanList.Count < 1)
                    continue;

                float[] intensity = new float[intensities.Count];
                float[] scans     = new float[scanList.Count];

                intensities.CopyTo(intensity);
                scanList.CopyTo(scans);

                clsSeries series = new clsSeries(ref scans, ref intensity, plotParams);
                base.AddSeries(series);                
            }                              
   
            // Plot the feature max
            float [] intensityOfFeature = new float[2];
            float [] scanOfFeature      = new float[2];
            
            intensityOfFeature[0]       = Convert.ToSingle(m_feature.Abundance);
            intensityOfFeature[1]       = Convert.ToSingle(m_feature.Abundance);
            scanOfFeature[0]            = Convert.ToSingle(m_feature.Scan);
            scanOfFeature[1]            = Convert.ToSingle(m_feature.ScanAligned);

            clsShape featureShape       = new TriangleShape(mint_pt_size, false);
            clsPlotParams featureParams = new clsPlotParams(featureShape, Color.Red);
            clsSeries featureSeries     = new clsSeries(ref scanOfFeature, ref intensityOfFeature, featureParams);
            base.AddSeries(featureSeries);

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


