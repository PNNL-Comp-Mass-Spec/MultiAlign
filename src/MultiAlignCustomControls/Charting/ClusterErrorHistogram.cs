using System;
using System.Collections.Generic;
using System.Drawing;
using MultiAlignCore.Extensions;
using MultiAlignCustomControls.Drawing;
using MultiAlignEngine.MassTags;
using PNNLControls;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data.Features;

namespace MultiAlignCustomControls.Charting
{

    

    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class ClusterErrorHistogram : ctlLineChart
    {
        #region Members
        private int mint_pt_size = 3;
        private System.ComponentModel.IContainer components = null;
        private clsColorIterator miter_color = new clsColorIterator();
        private List<UMCClusterLightMatched> m_additionalClusters;
        private UMCClusterLightMatched m_mainCluster;	
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
        public ClusterErrorHistogram()
	    {			
			InitializeComponent();                        
            AlternateColor          = Color.Black; 

            m_additionalClusters    = new List<UMCClusterLightMatched>();            
            m_mainCluster           = null;                 
            
            m_shapes                = ShapeIterator.CreateShapeList(mint_pt_size, true);
            m_mainShapes            = ShapeIterator.CreateShapeList(mint_pt_size, false);
            m_clusterShapes         = ShapeIterator.CreateShapeList(mint_pt_size + 2, false);
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
            m_additionalClusters.Clear();
            m_mainCluster = null;
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddAdditionalClusters(List<UMCClusterLightMatched> clusters)
        {
            m_additionalClusters.AddRange(clusters);
        }
        public void AddAdditionalClusters(UMCClusterLightMatched cluster)
        {
            m_additionalClusters.Add(cluster);
        }        
        public void UpdateCharts(bool shouldAutoViewport)
        {
            SeriesCollection.Clear();
            AddClusterDataToChart();

            if (shouldAutoViewport)
            {
                AutoViewPort();
            }
        }

        public Color AlternateColor
        {
            get;
            set;
        }
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

        public int DifferenceType
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
        private void AddClusterDataToChart()
        {        
            clsColorIterator colors = new clsColorIterator();

            int count = 0;
            List<float> distances = new List<float>();
            List<float> counts    = new List<float>();
            List<UMCLight> features = new List<UMCLight>();

            foreach (UMCClusterLightMatched tempCluster in m_additionalClusters)
            {
                features.AddRange(tempCluster.Cluster.Features);
            }
            features.AddRange(m_mainCluster.Cluster.Features);

            UMCClusterLight cluster = m_mainCluster.Cluster;
            switch(DifferenceType)
            {
                case 0:
                    features.Sort(delegate(UMCLight x, UMCLight y)
                    {
                        return y.MassMonoisotopicAligned.CompareTo(x.MassMonoisotopicAligned);
                    });

                    foreach (UMCLight feature in features)
                    {
                        distances.Add(Convert.ToSingle(Feature.ComputeMassPPMDifference(cluster.MassMonoisotopic, feature.MassMonoisotopicAligned)));
                        counts.Add(count++);
                    }

                    break;
                case 1:                    
                    features.Sort(delegate(UMCLight x, UMCLight y)
                    {
                        return y.RetentionTime.CompareTo(x.RetentionTime);
                    });

                    foreach (UMCLight feature in features)
                    {
                        distances.Add(Convert.ToSingle(cluster.RetentionTime - feature.RetentionTime));
                        counts.Add(count++);
                    }
                    break;
                case 2:
                    
                    features.Sort(delegate(UMCLight x, UMCLight y)
                    {
                        return y.DriftTime.CompareTo(x.DriftTime);
                    });

                    foreach (UMCLight feature in features)
                    {
                        distances.Add(Convert.ToSingle(cluster.DriftTime - feature.DriftTime));
                        counts.Add(count++);
                    }
                    break;
            }
                                   
            // This is for the differences series
            clsShape shape              = new BubbleShape(2, false);
            clsPlotParams plotParams    = new clsPlotParams(shape, AlternateColor);
            float[] distanceArray       = new float[distances.Count];
            float[] countArray          = new float[counts.Count];
            distances.CopyTo(distanceArray);
            counts.CopyTo(countArray);

            clsSeries series = new clsSeries(ref distanceArray, ref countArray, plotParams);
            base.AddSeries(series);                            
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

