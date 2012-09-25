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

namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class SingleClusterChart : ctlScatterChart
    {
        #region Members
        private int mint_pt_size = 3;
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 				           
        private DatasetInformation  m_info;
        private List<UMCClusterLight> m_additionalClusters;
        private List<MassTagLight> m_massTags;
        private UMCClusterLight m_mainCluster;
        #endregion

        #region Constructors
        public SingleClusterChart()
	    {			
			InitializeComponent();

            m_additionalClusters    = new List<UMCClusterLight>();
            m_massTags              = new List<MassTagLight>();
            m_mainCluster           = null;
            m_info                  = null;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="dataset"></param>
        public SingleClusterChart(UMCClusterLight cluster, List<UMCClusterLight> clusters) :
            this()
        {
            AddAdditionalClusters(clusters);
        }
        #endregion

        #region Data Addition Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {
            m_info      = null;
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
            m_additionalClusters.Clear();
            m_massTags.Clear();
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddAdditionalClusters(List<UMCClusterLight> clusters)
        {
            m_additionalClusters.AddRange(clusters);
            UpdateCharts();
        }
        public void AddAdditionalClusters(UMCClusterLight cluster)
        {                        
            m_additionalClusters.Add(cluster);
            UpdateCharts();
        }
        public void AddMassTags(List<MassTagLight> tags)
        {            
            m_massTags.AddRange(tags);
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            SeriesCollection.Clear();
            AddMassTagsToChart(m_massTags);
            AddClusterDataToChart(m_additionalClusters, true);
            AddClusterDataToChart(m_mainCluster, false);
            AutoViewPort();
            
        }
        public void AddMassTags(MassTagLight tag)
        {
            SeriesCollection.Clear();
            m_massTags.Add(tag);
            AutoViewPort();            
        }
        public UMCClusterLight MainCluster
        {
            get
            {
                return m_mainCluster;
            }
            set
            {
                m_additionalClusters.Clear();                
                m_mainCluster = value;
                if (value != null)
                {                    
                    AddClusterDataToChart(m_mainCluster, false);
                    UpdateCharts();
                }
            }
        }
        #endregion
        
        #region Cluster Rendering

        private void AddMassTagsToChart(List<MassTagLight> tags)
        {
            clsColorIterator colors = new clsColorIterator();
            float maxY = 500;
            float minY = 0;
            float maxX = 500;
            float minX = 0;

            List<float> massList = new List<float>();
            List<float> scanList = new List<float>();
            Color color = Color.Gray;
            clsShape shape  = new CrossShape(3, false); 

            clsPlotParams plotParams = new clsPlotParams(shape, color);

            int clustersAdded = 0;
            foreach (MassTagLight tag in tags)
            {
                float x = 0;
                float y = 0;

                y = Convert.ToSingle(tag.MassMonoisotopic);
                x = Convert.ToSingle(tag.NETAverage);

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
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void AddClusterDataToChart(List<UMCClusterLight> clusters, bool isAlternate)
        {
            foreach (UMCClusterLight cluster in clusters)
            {
                AddClusterDataToChart(cluster, isAlternate);
            }
        }
        private void AddClusterDataToChart(UMCClusterLight cluster, bool isAlternate)
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
            if (isAlternate)
            {
                shape = new BubbleShape(mint_pt_size, true);
                color = Color.LightGray;
            }

            clsPlotParams plotParams = new clsPlotParams(shape, color);
                
            int clustersAdded = 0;
            foreach(UMCLight feature in cluster.Features)
            {
                float x = 0;
                float y = 0;

                y = Convert.ToSingle(feature.MassMonoisotopicAligned);
                x = Convert.ToSingle(feature.RetentionTime);                            
                        
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
            this.XAxisLabel = "Scan #";
            this.YAxisLabel = "Monoisotopic Mass";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        #region Display Event Handlers
        private void mcheckBox_showAligned_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        private void mcheckBox_showNET_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
            AutoViewPort();
        }
        private void mcheckBox_displayMZ_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
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

