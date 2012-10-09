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

namespace MultiAlignCustomControls.Charting
{    
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class SingleMassTagChart : FeatureScatterPlot
    {
        #region Members
        private int mint_pt_size = 2;
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ;           
        private MassTagToCluster m_mainTag;
        /// <summary>
        /// List of other tags to render.
        /// </summary>
        private List<MassTagToCluster> m_otherTags;
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
        public SingleMassTagChart()
	    {			
			InitializeComponent();

            m_otherTags             = new List<MassTagToCluster>();
            m_mainTag               = null;
            AlternateColor          = Color.Black;
            IsDriftTimeXAxis        = false;
            XAxisShortHand          = "NET";
            DrawFeatures            = true;
            m_shapes                = ShapeIterator.CreateShapeList(mint_pt_size, true);
            m_mainShapes            = ShapeIterator.CreateShapeList(mint_pt_size, false);
            m_clusterShapes         = ShapeIterator.CreateShapeList(mint_pt_size + 2, false);
                        
        }
        #endregion
               
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

        #region Data Addition/Deletion Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {            
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
            m_otherTags = new List<MassTagToCluster>();
            m_mainTag   = null;
        }        
        /// <summary>
        /// Gets or sets the tag to render.
        /// </summary>
        public MassTagToCluster MassTag
        {
            get
            {
                return m_mainTag;
            }
            set
            {                
                m_mainTag = value;                
            }
        }
        /// <summary>
        /// Add other tags to render.
        /// </summary>
        /// <param name="otherTags"></param>
        public void AddAdditionalMassTags(List<MassTagToCluster> otherTags)
        {
            m_otherTags.AddRange(otherTags);
        }
        #endregion
        
        #region Cluster Rendering
        /// <summary>
        /// Updates the charts with new data.
        /// </summary>
        /// <param name="shouldAutoViewport"></param>
        public void UpdateCharts(bool shouldAutoViewport)
        {
            if (m_mainTag == null)
                return;

            SeriesCollection.Clear();

            // Add other data into the mix.
            foreach (MassTagToCluster massTag in m_otherTags)
            {
                //AddClusterDataToChart(massTag.Matches, true);
                AddMassTagToChart(massTag, true);
            }

            AddClusterDataToChart(m_mainTag.Matches, false);
            AddMassTagToChart(m_mainTag, false);

            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }
        /// <summary>
        /// Adds the mass tags to the data chart.
        /// </summary>
        /// <param name="tags"></param>
        private void AddMassTagToChart(MassTagToCluster matchedTag, bool isAlternative)
        {
            MassTagLight tag = matchedTag.MassTag;


            clsColorIterator colors     = new clsColorIterator();            
            List<float> massList        = new List<float>();
            List<float> scanList        = new List<float>();
            Color color                 = Color.Orange;
            int size                    = mint_pt_size*3;
            if (isAlternative)
            {
                color = Color.Black;
                size += 2;
            }
            clsShape shape              = new DiamondShape(size, isAlternative); 
            clsPlotParams plotParams    = new clsPlotParams(shape, color);

             massList.Add(Convert.ToSingle(tag.MassMonoisotopic));            
            if (IsDriftTimeXAxis)
            {
                scanList.Add(Convert.ToSingle(tag.DriftTime));
            }
            else
            {
                scanList.Add(Convert.ToSingle(tag.NETAverage));
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
        /// <summary>
        /// Draws the features along with the main centroids/mass tags.
        /// </summary>
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

                clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
                base.AddSeries(series);                
            }

            float[] clusterMass = new float[1];
            float[] clusterScan = new float[1];
            Color clusterColor  = Color.Red;
            bool isHollow = isAlternate;
            if (isAlternate)
            {
                clusterColor = Color.Lime;
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

