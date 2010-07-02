using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PNNLControls;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin
{
	public class ctlClusterChart : ctlScatterChart
	{

		private System.ComponentModel.IContainer components = null;
		private clsMultiAlignAnalysis mobjAnalysis ; 

		private clsColorIterator miter_color = new  clsColorIterator() ; 
		private int mint_pt_size = 2 ; 
		/// <summary>
		/// Are the shapes of the points hollow
		/// </summary>
		private bool mbln_hollow = false ;
        private CheckBox mcheckBox_showAligned;
        private CheckBox mcheckBox_showNET;
        private ComboBox mcomboBox_chargeStates;
        private CheckBox mcheckBox_displayMZ;
        private Label label1;

        private int mint_dataset; 

		/// <summary>
		/// Show datasets aligned together
		/// </summary>
		private bool mblnDatasetsAligned = true ; 		
		public ctlClusterChart()
	    {			
			InitializeComponent();

            mcomboBox_chargeStates.Items.Add("All");
            for (int i = 1; i < 31; i++)
                mcomboBox_chargeStates.Items.Add(i.ToString());
            mcomboBox_chargeStates.SelectedIndex = 0;

            mint_dataset = -1;

            this.mcomboBox_chargeStates.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_chargeStates_SelectedIndexChanged);
        }
		public ctlClusterChart(clsMultiAlignAnalysis analysis)
		{
            InitializeComponent();


            mcomboBox_chargeStates.Items.Add("All");
            for (int i = 1; i < 31; i++)
                mcomboBox_chargeStates.Items.Add(i.ToString());
            mcomboBox_chargeStates.SelectedIndex = 0;

            this.mcomboBox_chargeStates.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_chargeStates_SelectedIndexChanged);

            mint_dataset = -1;
        	Analysis     = analysis ;
		}

        public ctlClusterChart(clsMultiAlignAnalysis analysis, int dataset)
        {
            InitializeComponent();


            mcomboBox_chargeStates.Items.Add("All");
            for (int i = 1; i < 31; i++)
                mcomboBox_chargeStates.Items.Add(i.ToString());
            mcomboBox_chargeStates.SelectedIndex = 0;

            this.mcomboBox_chargeStates.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_chargeStates_SelectedIndexChanged);

            mint_dataset = dataset;
            Analysis     = analysis;
        }

        private void AddAnalysisData()
		{
            if (mobjAnalysis == null)
                return;
            int num_datasets = mobjAnalysis.UMCData.NumDatasets;

            if (mobjAnalysis.UMCData.mobjClusterData != null)
            {
                AddClusterDataToChart(mobjAnalysis.UMCData.mobjClusterData);
            }


            int charge = mcomboBox_chargeStates.SelectedIndex;
            
			for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
			{
                if (charge == 0)
				    AddDatasetToOverlapChart(mobjAnalysis, dataset_num, mblnDatasetsAligned) ; 
                else
                    AddChargeStateToOverlapChart(charge, mobjAnalysis, dataset_num, mblnDatasetsAligned);
			}

		}
		public clsMultiAlignAnalysis Analysis
		{
			set
			{
                mobjAnalysis = value;

                ViewPortHistory.Clear();
                this.SeriesCollection.Clear();
                
                AutoViewPortOnAddition = true;

                if (mint_dataset < 0)
                {
                    AddAnalysisData();
                }
                else
                {
                    AddDatasetToOverlapChart(mobjAnalysis, mint_dataset, mcheckBox_showAligned.Checked, 5);
                }
			}
		}
        /// <summary>
        /// Renders data with charge state information.
        /// </summary>
        /// <param name="chargeState">Charge state to render.</param>
        /// <param name="analysis">Analysis object to use</param>
        /// <param name="datasetNum">Dataset index</param>
        /// <param name="aligned">whether the data has been aligned or not.</param>
        private void AddChargeStateToOverlapChart(int chargeState, clsMultiAlignAnalysis analysis, int datasetNum, bool aligned)
        {
            float[] masses  = new float[1];
            float[] scans   = new float[1];
            float[] charges = new float[1];

            int chargesFound = 0;
            if (mcheckBox_displayMZ.Checked == false)
            {
                analysis.UMCData.GetMassesAndScans(datasetNum,
                                                                       aligned,
                                                                       ref masses,
                                                                       ref scans,
                                                                       chargeState);
                YAxisLabel = "Monoisotopic Mass";
            }
            else
            {

                analysis.UMCData.GetMZAndScans(datasetNum,
                                                                       aligned,
                                                                       ref masses,
                                                                       ref scans,
                                                                       chargeState);
                YAxisLabel = "M/Z";
            }
            Color clr                = miter_color.GetColor(chargeState - 1);
            clsShape shape           = new DiamondShape(mint_pt_size, mbln_hollow); ;
            clsPlotParams plt_params = new clsPlotParams(shape, clr);

            plt_params.Name = "Charge State" + chargeState.ToString(); // //analysis.UMCData.DatasetName[datasetNum];
            ViewPortHistory.Clear();
            AutoViewPortOnAddition = true;
                        
            clsSeries series = new clsSeries(ref scans, ref masses, plt_params);
            AddSeries(series);            
        }
        /// <summary>
        /// Adds a dataset to the cluster chart.
        /// </summary>
        /// <param name="datasetNum"></param>
        /// <param name="aligned"></param>
        public void AddDatasetToOverlapChart(clsMultiAlignAnalysis analysis,
                                             int datasetNum,
                                             bool aligned,
                                             int numChargeStates)
        {
            for (int i = 1; i <= numChargeStates; i++)
            {
                AddChargeStateToOverlapChart(i, analysis, datasetNum, aligned);
            }
        }
        private void AddClusterDataToChart(clsClusterData clusters)
        {
            /// We dont have M/z data.
            if (mcheckBox_displayMZ.Checked == true)
                return;

            int i = 0;
            int numberOfClusters = clusters.NumClusters;

            float[] masses = new float[numberOfClusters];
            float[] scans = new float[numberOfClusters];

            List<float> massList = new List<float>();
            List<float> scanList = new List<float>();


            clsShape shape           = new DiamondShape(mint_pt_size + 3, true);
            clsPlotParams plotParams = new clsPlotParams(shape, Color.FromArgb(64, Color.DarkGray));

            while(i < numberOfClusters)
            {
                float x, y;
                clsCluster cluster = clusters.GetCluster(i);
                if (cluster.mshort_num_dataset_members > 1)
                {
                    if (mcheckBox_showAligned.Checked == true)
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

                        /// 
                        /// the clusters will not be aligned if they are not peak matched 
                        /// 
                        if (mcheckBox_showAligned.Checked == true && mobjAnalysis.AlignmentOptions[0].IsAlignmentBaselineAMasstagDB)
                        {
                            x = Convert.ToSingle(cluster.mdouble_aligned_net);
                        }
                        else
                        {
                            x = Convert.ToSingle(cluster.mdouble_net);
                        }
                    }
                    else
                    {
                        x = Convert.ToSingle(cluster.mint_scan);
                    }
                    scanList.Add(x);
                }
                i++;                
            }
            massList.CopyTo(masses);
            scanList.CopyTo(scans);
            plotParams.Name = "Clusters";
            clsSeries series = new clsSeries(ref scans, ref masses, plotParams);

            AddSeries(series);
        }
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
        /// <summary>
        /// Adds the dataset mass and scan values to the overlay chart.
        /// </summary>
        /// <param name="chargeState"></param>
        /// <param name="analysis"></param>
        /// <param name="datasetNum"></param>
        /// <param name="aligned"></param>
        private void AddDatasetToOverlapChart(clsMultiAlignAnalysis analysis, int datasetNum, bool aligned)
        {
            float[] masses       = new float[1];
            float[] scans        = new float[1];
            float[] charges      = new float[1];
            double[] driftTimes  = new double[1];

            if (mcheckBox_showNET.Checked == true)
            {
                if (mcheckBox_displayMZ.Checked == true)
                {
                    analysis.UMCData.GetMZAndNETs(datasetNum,
                                                            ref masses,
                                                            ref scans,
                                                            ref driftTimes);
                    YAxisLabel = "M/Z";
                }
                else
                {
                    analysis.UMCData.GetMassesAndNETs(datasetNum,
                                                            mcheckBox_showAligned.Checked,
                                                            ref masses,
                                                            ref scans,
                                                            ref driftTimes);
                    YAxisLabel = "Monoisotopic Mass";
                }
                    XAxisLabel = "NET";
            }
            else 
            {
                if (mcheckBox_displayMZ.Checked == true)
                {
                    analysis.UMCData.GetMZAndScans(datasetNum,
                                                                mcheckBox_showAligned.Checked,
                                                                ref masses,
                                                                ref scans,
                                                                ref driftTimes);
                    YAxisLabel = "M/Z";
                }else
                {
                    analysis.UMCData.GetMassesAndScans(datasetNum,
                                                                mcheckBox_showAligned.Checked,
                                                                ref masses,
                                                                ref scans);
                    YAxisLabel = "Monoisotopic Mass";

                }
                XAxisLabel = "Scans";
            }
            Color clr                = miter_color.GetColor(datasetNum);
            clsShape shape           = new DiamondShape(mint_pt_size, mbln_hollow); ;
            clsPlotParams plt_params = new clsPlotParams(shape, clr);

            plt_params.Name         = analysis.UMCData.DatasetName[datasetNum];

            clsSeries series        = new clsSeries(ref scans, ref masses, plt_params);
            AddSeries(series);
        }
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
                mobjAnalysis = null;                
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
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
            this.mcheckBox_showAligned.Location = new System.Drawing.Point(3, 3);
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
            this.mcheckBox_showNET.Location = new System.Drawing.Point(3, 26);
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
            this.mcomboBox_chargeStates.Location = new System.Drawing.Point(482, 11);
            this.mcomboBox_chargeStates.Name = "mcomboBox_chargeStates";
            this.mcomboBox_chargeStates.Size = new System.Drawing.Size(107, 21);
            this.mcomboBox_chargeStates.TabIndex = 2;
            // 
            // mcheckBox_displayMZ
            // 
            this.mcheckBox_displayMZ.AutoSize = true;
            this.mcheckBox_displayMZ.Location = new System.Drawing.Point(3, 49);
            this.mcheckBox_displayMZ.Name = "mcheckBox_displayMZ";
            this.mcheckBox_displayMZ.Size = new System.Drawing.Size(227, 17);
            this.mcheckBox_displayMZ.TabIndex = 3;
            this.mcheckBox_displayMZ.Text = "Display M/Z instead of Monoisotopic Mass";
            this.mcheckBox_displayMZ.UseVisualStyleBackColor = true;
            this.mcheckBox_displayMZ.CheckedChanged += new System.EventHandler(this.mcheckBox_displayMZ_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(407, 14);
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
            this.Legend.Bounds = new System.Drawing.Rectangle(474, 80, 113, 325);
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
            this.Size = new System.Drawing.Size(592, 442);
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

        private void mcheckBox_showAligned_CheckedChanged(object sender, EventArgs e)
        {
            AutoViewPortOnAddition = false;
            this.SeriesCollection.Clear();
            if (mint_dataset < 0)
            {
                AddAnalysisData();
            }
            else
            {
                AddDatasetToOverlapChart(mobjAnalysis, mint_dataset, mcheckBox_showAligned.Checked, 5);
            }
        }

        private void mcheckBox_showNET_CheckedChanged(object sender, EventArgs e)
        {
            ViewPortHistory.Clear();
            AutoViewPortOnAddition = true;
            this.SeriesCollection.Clear();
            if (mint_dataset < 0)
            {
                AddAnalysisData();
            }
            else
            {
                AddDatasetToOverlapChart(mobjAnalysis, mint_dataset, mcheckBox_showAligned.Checked, 5);
            }
        }

        protected override void PaintSeries(Graphics g, Bitmap bitmap, clsSeries series)
        {
            base.PaintSeries(g, bitmap, series);
        }

        private void mcomboBox_chargeStates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mobjAnalysis == null)
                return;

            AutoViewPortOnAddition = true;
            this.SeriesCollection.Clear();
            if (mint_dataset < 0)
            {
                AddAnalysisData();
            }
            else
            {
                AddDatasetToOverlapChart(mobjAnalysis, mint_dataset, mcheckBox_showAligned.Checked, 5);
            }
        }

        private void mcheckBox_displayMZ_CheckedChanged(object sender, EventArgs e)
        {
            ViewPortHistory.Clear();
            AutoViewPortOnAddition = true;
            this.SeriesCollection.Clear();
            if (mint_dataset < 0)
            {
                AddAnalysisData();
            }
            else
            {
                AddDatasetToOverlapChart(mobjAnalysis, mint_dataset, mcheckBox_showAligned.Checked, 5);
            }
        }

	}
}

