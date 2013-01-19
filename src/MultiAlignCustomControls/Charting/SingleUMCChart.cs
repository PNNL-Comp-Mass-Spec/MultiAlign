using System;
using System.Collections.Generic;
using System.Drawing;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCustomControls.Charting
{

    

    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class SingleUMCChart : ctlScatterChart
    {
        #region Members
        private int mint_pt_size = 3;
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 				           
        private DatasetInformation  m_info;                
        private UMCLight m_mainFeature;
        #endregion

        #region Constructors
        public SingleUMCChart()
	    {			
			InitializeComponent();
                        
            m_mainFeature           = null;
            m_info                  = null;            

            AddPostProcessor(new ChartPostRenderingProcessor(RenderDifferences), PostProcessPriority.MidHigh);
        }
        #endregion

        private void RenderDifferences(ctlChartBase chart, PostRenderEventArgs args)
        {
            if (m_mainFeature == null)
                return;

            Process(m_mainFeature, chart, args);
        }
        private void Process(UMCLight feature, ctlChartBase chart, PostRenderEventArgs args)
        {
            if (feature == null) return;

            double minMass, maxMass, minNet, maxNet, minDrift, maxDrift; 
            feature.MSFeatures.FindRanges(  out minMass, 
                                                out maxMass, 
                                                out minNet, 
                                                out maxNet, 
                                                out minDrift,
                                                out maxDrift);

            double massDifference   = Feature.ComputeMassPPMDifference(minMass, maxMass);
            massDifference          = Math.Abs(massDifference);
            double netDifference    = Math.Abs(maxNet - minNet);
            double driftDifference  = Math.Abs(maxDrift - minDrift);
            
            Graphics graphics   = args.Graphics;                                                

            float netLineRight  = this.mobj_axis_plotter.XScreenPixel(Convert.ToSingle(maxNet));
            float netLineLeft   = this.mobj_axis_plotter.XScreenPixel(Convert.ToSingle(minNet));
            float netString     = netLineLeft + ((netLineRight - netLineLeft) / 4);

            float massLineTop   = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(maxMass));
            float massLineBot   = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(minMass));
            float massString    = massLineBot + ((massLineTop - massLineBot) / 2);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                using (Pen pen = new Pen(brush))
                {
                    // Draw the spread of the cluster.
                    graphics.DrawLine(pen, netLineRight  + 10 , massLineTop, netLineRight + 10, massLineBot);
                    graphics.DrawLine(pen, netLineLeft, massLineTop - 10, netLineRight, massLineTop - 10);
                    graphics.DrawString(string.Format("{0:.0} PPM", massDifference), Font, brush, netLineRight   + 20, massString);

                    pen.Color = Color.Red;
                    
                    // Draw the line between the aligned and regular scan
                    float scanAligned   = this.mobj_axis_plotter.XScreenPixel(Convert.ToSingle(feature.ScanAligned));
                    float scan          = this.mobj_axis_plotter.XScreenPixel(Convert.ToSingle(feature.Scan));
                    float mass          = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(feature.MassMonoisotopicAligned));
                    float minValue      = Math.Min(scan, scanAligned);                    
                    float scanDiff      = minValue + Math.Abs(scanAligned - scan) / 2;

                    //graphics.DrawLine(pen, scan, mass, scanAligned, mass);
                    //graphics.DrawString(string.Format("{0:.0} centroid scan difference", Math.Abs(feature.ScanAligned - feature.Scan)), Font, brush, scanDiff, mass - 20);

                    // Then draw the MS/MS values

                    float offset    = Convert.ToSingle(MaxChartAreaXPixel) * .1F;
                    float width     = Convert.ToSingle(MaxChartAreaXPixel);
                    float direction = offset;

                    foreach (MSFeatureLight msFeature in feature.MSFeatures)
                    {
                        foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                        {
                            float msMassScan = mobj_axis_plotter.YScreenPixel(Convert.ToSingle(msFeature.MassMonoisotopicAligned)) ;
                            float msScan     = mobj_axis_plotter.XScreenPixel(Convert.ToSingle(msFeature.Scan)) ;

                            // Draw the precursor strings.
                            string precursorString = string.Format("({0}, @{1:.00} m/z)", spectrum.Scan, spectrum.PrecursorMZ);
                            SizeF sizeOfPrecursor =  graphics.MeasureString(precursorString, Font);

                            float xPositionPrecursorString = msScan + offset + 2;
                            if (xPositionPrecursorString + sizeOfPrecursor.Width >= width)
                            {
                                xPositionPrecursorString = width - sizeOfPrecursor.Width - 2;
                            }
                            
                            graphics.DrawLine(pen, msScan, msMassScan, xPositionPrecursorString - 2, msMassScan + direction);
                            graphics.DrawString(precursorString, Font, brush, xPositionPrecursorString , msMassScan + direction );

                            // Alternate the direction of the MSMS precursor label
                            direction *= -1;
                        }
                    }
                }

                
            }
        }

        #region Data Addition Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {
            m_info      = null;
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
        }
        public void UpdateCharts(bool shouldAutoViewport)
        {
            SeriesCollection.Clear();
            PlotData();
            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }
        public UMCLight MainFeature
        {
            get
            {
                return m_mainFeature;
            }
            set
            {                
                m_mainFeature = value;                
            }
        }
        #endregion
        
        #region Feature Rendering
        
        /// <summary>
        /// Adds cluster data to the chart.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="isAlternate"></param>
        private void PlotData()
        {
            if (m_mainFeature == null)
                return;

            clsColorIterator colors = new clsColorIterator();            
            if (m_mainFeature.MSFeatures.Count > 0)
            {                
                Dictionary<int, List<XYZData>> charges   = m_mainFeature.CreateChargeSICForMonoMass();

                foreach (int charge in charges.Keys)
                {
                    Color color = colors.GetColor(charge);

                    List<float> massList = new List<float>();
                    List<float> scanList = new List<float>();
                    
                   
                    float[] masses  = null;
                    float[] scans   = null;

                    
                    if (massList.Count > 0)
                    {
                        // Add the series for MS/MS features
                        masses = new float[massList.Count];
                        scans  = new float[scanList.Count];
                        massList.CopyTo(masses);
                        scanList.CopyTo(scans);

                        clsShape shape              = new BubbleShape(mint_pt_size * 2, true);
                        clsPlotParams plotParams    = new clsPlotParams(shape, Color.Red);
                        clsSeries series            = new clsSeries(ref scans, ref masses, plotParams);
                        base.AddSeries(series);
                        massList.Clear();
                        scanList.Clear();
                    }

                    // Now add the rest of the features into the plot
                    charges[charge].ForEach(x => scanList.Add(Convert.ToSingle(x.X)));
                    charges[charge].ForEach(x => massList.Add(Convert.ToSingle(x.Z)));

                    masses  = new float[massList.Count];
                    scans   = new float[scanList.Count];
                    massList.CopyTo(masses);
                    scanList.CopyTo(scans);

                    clsShape msFeatureShape         = new BubbleShape(mint_pt_size, false);
                    clsPlotParams msFeatureParams   = new clsPlotParams(msFeatureShape, color);
                    msFeatureParams.Name            = string.Format("charge state {0}", charge);
                    clsSeries msFeaturesseries      = new clsSeries(ref scans, ref masses, msFeatureParams);
                    base.AddSeries(msFeaturesseries);                    
                }
            }

            //float[] clusterMass = new float[1];
            //float[] clusterScan = new float[1];
            //Color clusterColor = Color.FromArgb(200, 255, 0, 0);
            

            //// Original Scan
            //clusterMass[0] = Convert.ToSingle(m_mainFeature.MassMonoisotopicAligned);
            //clusterScan[0] = Convert.ToSingle(m_mainFeature.Scan);
            
            //clsPlotParams plotParamsCluster = new clsPlotParams(new TriangleShape(mint_pt_size, false), clusterColor);
            //plotParamsCluster.Name  = "Feature Centroid";
            //clsSeries clusterSeries = new clsSeries(ref clusterScan, ref clusterMass, plotParamsCluster);
            //base.AddSeries(clusterSeries);

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
            // SingleUMCChart
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
            this.Name = "SingleUMCChart";
            this.Size = new System.Drawing.Size(408, 382);
            this.Title = "Cluster Chart";
            this.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.TitleMaxFontSize = 15F;
            this.TitleVisible = false;
            this.XAxisLabel = "MS Scan";
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

