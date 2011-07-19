using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLControls;

namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Renders UMC Clusters and UMC Features as a scatter plot.
    /// </summary>
	public class ctlClusterChart : ctlScatterChart
    {
        #region Members
        private System.ComponentModel.IContainer components = null;		
		private clsColorIterator miter_color = new  clsColorIterator() ; 
		private int mint_pt_size = 1 ; 
		/// <summary>
		/// Are the shapes of the points hollow
		/// </summary>
		private bool mbln_hollow = false ;
        private CheckBox mcheckBox_showAligned;
        private CheckBox mcheckBox_showNET;
        private ComboBox mcomboBox_chargeStates;
        private CheckBox mcheckBox_displayMZ;
        private Label label1;
        private const int MAX_CHARGE_STATE = 10;
        private List<clsUMC>        m_features;
        private List<clsCluster>    m_clusters;
        private DatasetInformation  m_info;
        #endregion

        #region Constructors
        public ctlClusterChart()
	    {			
			InitializeComponent();

            mcomboBox_chargeStates.Items.Add("All");
            for (int i = 1; i < MAX_CHARGE_STATE; i++)
                mcomboBox_chargeStates.Items.Add(i.ToString());
            mcomboBox_chargeStates.SelectedIndex = 0;
            
            mcomboBox_chargeStates.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_chargeStates_SelectedIndexChanged);

            m_clusters  = null;
            m_features  = null;
            m_info      = null;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="dataset"></param>
        public ctlClusterChart(DatasetInformation info, List<clsUMC> features) :
            this()
        {
            AddFeatures(info, features);
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        /// <param name="dataset"></param>
        public ctlClusterChart(List<clsCluster> clusters) :
            this()
        {
            AddClusters(clusters);
        }
        #endregion

        #region Data Addition Methods
        /// <summary>
        /// Clears the data currently on the plot.
        /// </summary>
        public void ClearData()
        {
            m_features  = null;
            m_clusters  = null;
            m_info      = null;
            ViewPortHistory.Clear();
            SeriesCollection.Clear();
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
		public void AddFeatures(DatasetInformation info, List<clsUMC> features)
		{			
                
            AutoViewPortOnAddition          = true;
            Title = "Features";
            if (info != null)
            {
                Title = info.DatasetName + " ";
            }
            mcheckBox_displayMZ.Visible     = true;
            mcheckBox_showAligned.Visible   = true;                     
            AddDatasetToOverlapChart(features, mcheckBox_showAligned.Checked, MAX_CHARGE_STATE);                
            AutoViewPortOnAddition          = false;		
        }
        /// <summary>
        /// Sets the analysis object and extracts data for display.
        /// </summary>
        public void AddClusters(List<clsCluster> clusters)
        {
            AutoViewPortOnAddition = true;
            Title = "Clusters";
            mcheckBox_displayMZ.Visible     = true;
            mcheckBox_showAligned.Visible   = true;

            AddClusterDataToChart(clusters, mcheckBox_showAligned.Checked, MAX_CHARGE_STATE);
            AutoViewPortOnAddition          = false;
        }
        #endregion

        #region Dataset Rendering.
        /// <summary>
        /// Renders data with charge state information.
        /// </summary>
        /// <param name="chargeState">Charge state to render.</param>
        /// <param name="analysis">Analysis object to use</param>
        /// <param name="datasetNum">Dataset index</param>
        /// <param name="aligned">whether the data has been aligned or not.</param>
        private void AddChargeStateToOverlapChart(List<clsUMC> features, int chargeState, bool showAligned)
        {            
            List<float> x = new List<float>();
            List<float> xa = new List<float>();
            List<float> y = new List<float>();
            List<float> ya = new List<float>();
            
            bool useNet = mcheckBox_showNET.Checked;
            bool useMZ = mcheckBox_displayMZ.Checked;
            foreach (clsUMC feature in features)
            {
                // Make sure charges match.
                if (feature.ChargeRepresentative == chargeState)
                {
                    // X value 
                    if (useNet)
                    {
                        x.Add((float)feature.Net);
                        xa.Add((float)feature.Net);
                    }
                    else
                    {
                        x.Add((float)feature.ScanAligned);
                        xa.Add((float)feature.Scan);
                    }

                    // Y value
                    if (useMZ)
                    {
                        y.Add((float)feature.MZForCharge);
                    }
                    else
                    {
                        y.Add((float)feature.Mass);
                        ya.Add((float)feature.MassCalibrated);
                    }
                }
            }

            
            Color clr                = miter_color.GetColor(chargeState - 1);
            clsShape shape           = new DiamondShape(mint_pt_size, mbln_hollow); 
            clsPlotParams plt_params = new clsPlotParams(shape, clr);

            plt_params.Name = "Charge State " + chargeState.ToString(); 
            ViewPortHistory.Clear();
            AutoViewPortOnAddition = true;

            float[] masses = new float[y.Count];
            float[] scans  = new float[x.Count];

            if (showAligned)
            {
                ya.CopyTo(masses, 0);
                xa.CopyTo(scans, 0);
            }
            else
            {
                y.CopyTo(masses, 0);
                x.CopyTo(scans, 0);
            }
                        
            clsSeries series = new clsSeries(ref scans, ref masses, plt_params);
            AddSeries(series);            
        }
        /// <summary>
        /// Adds a dataset to the cluster chart.
        /// </summary>
        /// <param name="datasetNum"></param>
        /// <param name="aligned"></param>
        private void AddDatasetToOverlapChart(List<clsUMC>   features,                                             
                                             bool           showAligned,
                                             int            numChargeStates)
        {
            for (int i = 1; i <= numChargeStates; i++)
            {
                AddChargeStateToOverlapChart(features, i, showAligned);
            }
        }        
        #endregion

        #region Cluster Rendering
        /// <summary>
        /// Adds all cluster data to the plot.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="specificCharge"></param>
        private void AddClusterDataToChart(List<clsCluster> clusters, bool showAligned, int specificCharge)
        {            
            if (mcheckBox_displayMZ.Checked == true)
                return;

            clsColorIterator colors = new clsColorIterator();
            float maxY = 500;
            float minY = 0;
            float maxX = 500;
            float minX = 0;

            int chargeMax   = specificCharge + 1;
            if (specificCharge < 1)
            {
                chargeMax = MAX_CHARGE_STATE;
            }

            for (int charge = specificCharge; charge < chargeMax; charge++)
            {
                List<float> massList = new List<float>();
                List<float> scanList = new List<float>();
                Color color              = colors.GetColor(charge);
                clsShape shape           = new BubbleShape(mint_pt_size, false);
                clsPlotParams plotParams = new clsPlotParams(shape, color);
                
                int clustersAdded = 0;
                foreach(clsCluster cluster in clusters)
                {
                    float x = 0;
                    float y = 0;
                                      
                    if (cluster.Charge == charge)
                    {
                        if (showAligned == true)
                        {
                            y = Convert.ToSingle(cluster.mdouble_mass_calibrated);
                        }
                        else
                        {
                            y = Convert.ToSingle(cluster.mdouble_mass);
                        }
                        massList.Add(y);

                        if (mcheckBox_showNET.Checked == true)
                        {                            
                            x = Convert.ToSingle(cluster.mdouble_net);                            
                        }
                        else
                        {
                            x = Convert.ToSingle(cluster.mint_scan);
                        }
                        scanList.Add(x);

                        minX = Math.Min(x, minX);
                        maxX = Math.Max(x, maxX);

                        minY = Math.Min(y, minY);
                        maxY = Math.Max(y, maxY);
                        clustersAdded++;
                    }
                }
                if (clustersAdded > 0)
                {
                    float[] masses = new float[massList.Count];
                    float[] scans = new float[scanList.Count];

                    massList.CopyTo(masses);
                    scanList.CopyTo(scans);
                    plotParams.Name = "Clusters with charge " + charge.ToString();
                    clsSeries series = new clsSeries(ref scans, ref masses, plotParams);
                    base.AddSeries(series);
                }
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
            this.mcheckBox_showAligned = new System.Windows.Forms.CheckBox();
            this.mcheckBox_showNET = new System.Windows.Forms.CheckBox();
            this.mcomboBox_chargeStates = new System.Windows.Forms.ComboBox();
            this.mcheckBox_displayMZ = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // mcheckBox_showAligned
            // 
            this.mcheckBox_showAligned.AutoSize = true;
            this.mcheckBox_showAligned.Location = new System.Drawing.Point(83, 3);
            this.mcheckBox_showAligned.Name = "mcheckBox_showAligned";
            this.mcheckBox_showAligned.Size = new System.Drawing.Size(91, 17);
            this.mcheckBox_showAligned.TabIndex = 0;
            this.mcheckBox_showAligned.Text = "Show Aligned";
            this.mcheckBox_showAligned.UseVisualStyleBackColor = true;
            this.mcheckBox_showAligned.CheckedChanged += new System.EventHandler(this.mcheckBox_showAligned_CheckedChanged);
            // 
            // mcheckBox_showNET
            // 
            this.mcheckBox_showNET.AutoSize = true;
            this.mcheckBox_showNET.Location = new System.Drawing.Point(3, 3);
            this.mcheckBox_showNET.Name = "mcheckBox_showNET";
            this.mcheckBox_showNET.Size = new System.Drawing.Size(78, 17);
            this.mcheckBox_showNET.TabIndex = 1;
            this.mcheckBox_showNET.Text = "Show NET";
            this.mcheckBox_showNET.UseVisualStyleBackColor = true;
            this.mcheckBox_showNET.CheckedChanged += new System.EventHandler(this.mcheckBox_showNET_CheckedChanged);
            // 
            // mcomboBox_chargeStates
            // 
            this.mcomboBox_chargeStates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_chargeStates.FormattingEnabled = true;
            this.mcomboBox_chargeStates.Location = new System.Drawing.Point(352, 3);
            this.mcomboBox_chargeStates.Name = "mcomboBox_chargeStates";
            this.mcomboBox_chargeStates.Size = new System.Drawing.Size(53, 21);
            this.mcomboBox_chargeStates.TabIndex = 2;
            // 
            // mcheckBox_displayMZ
            // 
            this.mcheckBox_displayMZ.AutoSize = true;
            this.mcheckBox_displayMZ.Location = new System.Drawing.Point(174, 3);
            this.mcheckBox_displayMZ.Name = "mcheckBox_displayMZ";
            this.mcheckBox_displayMZ.Size = new System.Drawing.Size(87, 17);
            this.mcheckBox_displayMZ.TabIndex = 3;
            this.mcheckBox_displayMZ.Text = "Display M/Z ";
            this.mcheckBox_displayMZ.UseVisualStyleBackColor = true;
            this.mcheckBox_displayMZ.CheckedChanged += new System.EventHandler(this.mcheckBox_displayMZ_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(277, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Charge State";
            // 
            // ctlClusterChart
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
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mcheckBox_displayMZ);
            this.Controls.Add(this.mcomboBox_chargeStates);
            this.Controls.Add(this.mcheckBox_showNET);
            this.Controls.Add(this.mcheckBox_showAligned);
            this.DefaultZoomHandler.Active = true;
            this.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider1.Color = System.Drawing.Color.Black;
            penProvider1.Width = 1F;
            this.Legend.BorderPen = penProvider1;
            this.Legend.Bounds = new System.Drawing.Rectangle(327, 80, 76, 271);
            this.Legend.ColumnWidth = 125;
            this.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Legend.MaxFontSize = 11F;
            this.Legend.MinFontSize = 6F;
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
            this.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.TitleMaxFontSize = 20F;
            this.XAxisLabel = "Scan #";
            this.YAxisLabel = "Monoisotopic Mass";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private void mcomboBox_chargeStates_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        private void mcheckBox_displayMZ_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        public void UpdateDisplay()
        {
            AutoViewPortOnAddition = false;
            this.SeriesCollection.Clear();

            if (mcheckBox_showNET.Checked)
            {
                XAxisLabel = "NET";
                if (mcheckBox_showAligned.Checked)
                {
                    XAxisLabel = "Aligned NET";
                }
            }
            else
            {
                XAxisLabel = "Scan #";
                if (mcheckBox_showAligned.Checked)
                {
                    XAxisLabel = "Aligned Scan #";
                }
            }

            if (mcheckBox_displayMZ.Checked)
            {
                YAxisLabel = "M/Z";
            }
            else
            {
                YAxisLabel = "Monoisotopic Mass";
            }

            //TODO: Something here.
        }
        #endregion
    }
}

