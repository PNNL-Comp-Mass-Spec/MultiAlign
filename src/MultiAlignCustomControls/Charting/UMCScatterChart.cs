using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLControls;
using System.Drawing;
using MultiAlignCore.Extensions;

namespace MultiAlignCustomControls.Charting
{
    public class UMCScatterChart : FeatureScatterPlot
    {
        private System.ComponentModel.IContainer components = null;
        private List<UMCLight> m_features; 
        private clsColorIterator miter_color = new clsColorIterator();
        private int mint_pt_size = 2; 		

        public UMCScatterChart()
        {
            InitializeComponent();


            m_features = new List<UMCLight>();
        }


        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
            m_features.Clear();
        }

        public void AddFeatures(IEnumerable<UMCLight> features)
        {
            m_features.AddRange(features);
            AddClusterDataToChart(m_features);
            AutoViewPort();
            Refresh();
        }
        private void AddClusterDataToChart(List<UMCLight> clusters)
        {
                
            float maxY = 500;
            float minY = 0;
            float maxX = 500;
            float minX = 0;

            clsColorIterator colors = new clsColorIterator();
            Dictionary<int, List<UMCLight>> chargeMap = clusters.MapCharges();

            foreach(int charge in chargeMap.Keys)
            {

                Color color                 = colors.GetColor(charge);
                clsShape shape              = new BubbleShape(mint_pt_size, false);
                clsPlotParams plotParams    = new clsPlotParams(shape, color);
                List<float> massList        = new List<float>();
                List<float> scanList        = new List<float>();

                foreach (UMCLight cluster in clusters)
                {
                    float x = 0;
                    float y = 0;


                    y = Convert.ToSingle(cluster.MassMonoisotopicAligned);
                    x = Convert.ToSingle(cluster.RetentionTime);

                    massList.Add(y);
                    scanList.Add(x);

                    minX = Math.Min(x, minX);
                    maxX = Math.Max(x, maxX);

                    minY = Math.Min(y, minY);
                    maxY = Math.Max(y, maxY);
                }

                float[] masses = new float[massList.Count];
                float[] scans  = new float[scanList.Count];

                massList.CopyTo(masses);
                scanList.CopyTo(scans);
                clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
                base.AddSeries(series);
            }    
        }

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
