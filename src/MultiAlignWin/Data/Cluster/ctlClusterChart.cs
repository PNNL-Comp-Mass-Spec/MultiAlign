using System;
using System.Data ; 
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PNNLControls;

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
        /// Adds a dataset to the cluster chart.
        /// </summary>
        /// <param name="datasetNum"></param>
        /// <param name="aligned"></param>
		public void AddDatasetToOverlapChart(clsMultiAlignAnalysis analysis, 
                                             int datasetNum,
                                             bool aligned) 
		{
			float [] masses = new float[1]; 
			float [] scans = new float[1]; 

			analysis.UMCData.GetMassesAndScans(datasetNum, aligned, ref masses, ref scans) ; 

			Color clr                = miter_color.GetColor(datasetNum) ; 
			clsShape shape           = new DiamondShape(mint_pt_size, mbln_hollow) ;  ; 
			clsPlotParams plt_params = new clsPlotParams(shape, clr) ; 

			plt_params.Name          = analysis.UMCData.DatasetName[datasetNum] ; 
			ViewPortHistory.Clear(); 
			AutoViewPortOnAddition   = true; 

			clsSeries series = new clsSeries(ref scans, ref masses, plt_params); 
			SeriesCollection.Add(series); 			
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

