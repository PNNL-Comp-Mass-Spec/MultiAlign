using System;
using System.Data ; 
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PNNLControls;
using MultiAlignEngine.Features;
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
			int num_datasets = mobjAnalysis.UMCData.NumDatasets ; 			
			for (int dataset_num = 0 ; dataset_num < num_datasets ; dataset_num++)
			{
				AddDatasetToOverlapChart(mobjAnalysis, dataset_num, mblnDatasetsAligned) ; 
			}
		}
		public clsMultiAlignAnalysis Analysis
		{
			set
			{
				mobjAnalysis = value ; 
				AddAnalysisData() ; 
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
        /// <summary>
        /// Adds the dataset mass and scan values to the overlay chart.
        /// </summary>
        /// <param name="chargeState"></param>
        /// <param name="analysis"></param>
        /// <param name="datasetNum"></param>
        /// <param name="aligned"></param>
        private void AddDatasetToOverlapChart(clsMultiAlignAnalysis analysis, int datasetNum, bool aligned)
        {
            float[] masses = new float[1];
            float[] scans = new float[1];
            float[] charges = new float[1];

            analysis.UMCData.GetMassesAndScans(  datasetNum,
                                                                    aligned,
                                                                    ref masses,
                                                                    ref scans);

            Color clr                = miter_color.GetColor(datasetNum);
            clsShape shape           = new DiamondShape(mint_pt_size, mbln_hollow); ;
            clsPlotParams plt_params = new clsPlotParams(shape, clr);

            plt_params.Name = analysis.UMCData.DatasetName[datasetNum];
            ViewPortHistory.Clear();
            AutoViewPortOnAddition = true;

            clsSeries series = new clsSeries(ref scans, ref masses, plt_params);
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
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
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
            this.DefaultZoomHandler.Active = true;
            this.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            this.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider1.Color = System.Drawing.Color.Black;
            penProvider1.Width = 1F;
            this.Legend.BorderPen = penProvider1;
            this.Legend.Bounds = new System.Drawing.Rectangle(384, 46, 91, 172);
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
            this.Size = new System.Drawing.Size(480, 248);
            this.Title = "Cluster Chart";
            this.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.TitleMaxFontSize = 20F;
            this.XAxisLabel = "Scan #";
            this.YAxisLabel = "Monoisotopic Mass";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
	}
}

