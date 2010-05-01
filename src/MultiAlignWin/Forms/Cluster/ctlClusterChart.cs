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
		/// <summary>
		/// Show datasets aligned together
		/// </summary>
		private bool mblnDatasetsAligned = true ; 		
		public ctlClusterChart()
	    {			
			InitializeComponent();
		}
		public ctlClusterChart(clsMultiAlignAnalysis analysis)
		{
			InitializeComponent();
			Analysis = analysis ; 
		}

		private void AddAnalysisData()
		{
            int num_datasets = mobjAnalysis.UMCData.NumDatasets;

            if (mobjAnalysis.UMCData.mobjClusterData != null)
            {
                AddClusterDataToChart(mobjAnalysis.UMCData.mobjClusterData);
            }

            if (mobjAnalysis.PeakMatchedToMassTagDB == true)
            {
                try
                {
                    //AddMassTagDatabasePointsToChart(mobjAnalysis.MassTagDatabase);
                }
                catch
                {
                }
            }

			for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
			{
				AddDatasetToOverlapChart(mobjAnalysis, dataset_num, mblnDatasetsAligned) ; 
			}

		}
		public clsMultiAlignAnalysis Analysis
		{
			set
			{
                mobjAnalysis = value;

                ViewPortHistory.Clear();
                this.SeriesCollection.Clear();
                AddAnalysisData();
                AutoViewPortOnAddition = true;

                AddAnalysisData();
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

            int chargesFound = analysis.UMCData.GetMassesAndScans(  datasetNum, 
                                                                    aligned, 
                                                                    ref masses, 
                                                                    ref scans, 
                                                                    chargeState);

            Color clr                = miter_color.GetColor(chargeState - 1);
            clsShape shape           = new DiamondShape(mint_pt_size, mbln_hollow); ;
            clsPlotParams plt_params = new clsPlotParams(shape, clr);

            plt_params.Name = analysis.UMCData.DatasetName[datasetNum];
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
                clsCluster cluster = mobjAnalysis.UMCData.mobjClusterData.GetCluster(i++);
                if (cluster.mshort_num_dataset_members > 1)
                {
                    if (mcheckBox_showAligned.Checked == true)
                    {
                        massList.Add(Convert.ToSingle(cluster.mdouble_mass_calibrated));
                    }
                    else 
                    {
                        massList.Add(Convert.ToSingle(cluster.mdouble_mass));
                    }
                    if (mcheckBox_showNET.Checked == true)
                    {
                        if (mcheckBox_showAligned.Checked == true)
                        {
                            scanList.Add(Convert.ToSingle(cluster.mdouble_aligned_net));
                        }
                        else
                        {
                            scanList.Add(Convert.ToSingle(cluster.mdouble_net));
                        }
                    }
                    else
                    {
                        scanList.Add(Convert.ToSingle(cluster.mint_scan));
                    }
                }
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
            float[] masses  = new float[1];
            float[] scans   = new float[1];
            float[] charges = new float[1];

            if (mcheckBox_showNET.Checked == true)
            {                
                    analysis.UMCData.GetMassesAndNETs(      datasetNum,
                                                            mcheckBox_showAligned.Checked,
                                                            ref masses,
                                                            ref scans);
            }
            else 
            {
                analysis.UMCData.GetMassesAndScans(         datasetNum,                    
                                                            mcheckBox_showAligned.Checked,
                                                            ref masses,
                                                            ref scans);
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

        /// <summary>
        /// Displays cluster information at the given point.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void SeriesSelectedAtPoint(int series, int x, int y)
        {
            /*
             
            clsUMC umc = mobjAnalysis.UMCData.GetUMC(series,
                                        false,
                                        Convert.ToInt32(mobj_axis_plotter.XChartCoordinate(x)),
                                        Convert.ToSingle(mobj_axis_plotter.YChartCoordinate(y)));
            System.Diagnostics.Debug.WriteLine(string.Format("{0},{1}", mobj_axis_plotter.XChartCoordinate(x),
                mobj_axis_plotter.YChartCoordinate(y)));

            if (umc != null)
            {
                MultiAlignWin.Forms.formUMCData umcForm = new MultiAlignWin.Forms.formUMCData(umc);
                umcForm.Show();
            }
             
            */
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
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // mcheckBox_showAligned
            // 
            this.mcheckBox_showAligned.AutoSize = true;
            this.mcheckBox_showAligned.Location = new System.Drawing.Point(14, 15);
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
            this.mcheckBox_showNET.Location = new System.Drawing.Point(14, 38);
            this.mcheckBox_showNET.Name = "mcheckBox_showNET";
            this.mcheckBox_showNET.Size = new System.Drawing.Size(78, 17);
            this.mcheckBox_showNET.TabIndex = 1;
            this.mcheckBox_showNET.Text = "Show NET";
            this.mcheckBox_showNET.UseVisualStyleBackColor = true;
            this.mcheckBox_showNET.CheckedChanged += new System.EventHandler(this.mcheckBox_showNET_CheckedChanged);
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
            this.Legend.Bounds = new System.Drawing.Rectangle(451, 80, 107, 291);
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
            this.Size = new System.Drawing.Size(563, 404);
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
            AddAnalysisData();
        }

        private void mcheckBox_showNET_CheckedChanged(object sender, EventArgs e)
        {
            ViewPortHistory.Clear();
            AutoViewPortOnAddition = true;
            this.SeriesCollection.Clear();
            AddAnalysisData();
        }
	}
}

