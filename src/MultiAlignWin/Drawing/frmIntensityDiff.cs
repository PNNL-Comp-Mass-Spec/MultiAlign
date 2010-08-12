using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data ; 
using Cephes ;

using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmIntensityDiff.
	/// </summary>
	public class frmIntensityDiff : System.Windows.Forms.Form
	{
		#region "Components"
		//private PNNLControls.ExpandPanel m_expandPanel;
		//private PNNLControls.ctlScatterChart ctlScatterChart1;
		//private PNNLControls.ctlScatterChart ctlScatterChart2;
		//private PNNLControls.ExpandPanel expandPanel1;
		//private PNNLControls.ctlLineChart ctlLineChart1;
		//private PNNLControls.ctlScatterChart ctlScatterChart3;
		private PNNLControls.ExpandPanel mexpandPanel_options;
        private System.Windows.Forms.Splitter splitter1; 

		#endregion

		private string [] marr_dataset_names ; 
		private double [] marr_intensities ; 
		private int mint_num_rows ; 
		private int mint_num_columns ; 
		//private bool mbln_logarithmic ;
		private MultiAlignAnalysis mobjAnalysis ; 

		private PNNLControls.clsColorIterator miter_color = new PNNLControls.clsColorIterator() ; 
		private int mint_pt_size = 3 ;
        private bool mbln_hollow = false;
        //private DataGrid mobj_source_datagrid = null ;
        private Panel mpanel_plots;
        private Button mbtnCancel;
        private Button mbtnOK;
        private DataGrid mdataGrid_variability;
        private CheckedListBox mcheckedListBox_datasets;
        private Label label2;
        private CheckBox mcheckBox_Normalized;
        private ComboBox mcomboBox_baseline;
        private GroupBox mgroupBox_intensityScale;
        private RadioButton mradioButton_normal;
        private RadioButton mradioButton_log;
        private GroupBox mgroupBox_variabilityMetric;
        private RadioButton mradioButton_correl;
        private RadioButton mradioButton_stdev;
        private Label mlabel_baselineID;
        private PNNLControls.ctlLineChart mctl_histograms;
        private PNNLControls.ctlScatterChart mctl_scatter;
        private Splitter msplitter_plots;
        private IContainer components;

		public frmIntensityDiff()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public frmIntensityDiff(MultiAlignAnalysis analysis)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			mcheckedListBox_datasets.SelectedIndexChanged +=new EventHandler(mcheckedListBox_datasets_SelectedIndexChanged);
			mcomboBox_baseline.SelectedIndexChanged +=new EventHandler(mcomboBox_baseline_SelectedIndexChanged);
			mradioButton_log.CheckedChanged +=new EventHandler(mradioButton_log_CheckedChanged);
			mobjAnalysis = analysis ; 
			mint_num_rows = mobjAnalysis.UMCData.mobjClusterData.NumClusters ;
			mint_num_columns = mobjAnalysis.UMCData.NumDatasets ; 
			marr_intensities = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensity ; 
			marr_dataset_names = mobjAnalysis.UMCData.DatasetName ;
			mdataGrid_variability.MouseUp +=new MouseEventHandler(mdataGrid_variability_MouseUp);
			if (mobjAnalysis.UMCData.mobjClusterData.IsDataNormalized)
				mcheckBox_Normalized.Enabled = false ;
			FillData() ; 
			PlotData(0) ; 
		}


		private void FillData() 
		{
			for (int num_columns = 0 ; num_columns < mint_num_columns ; num_columns++)
			{
				mcomboBox_baseline.Items.Add(marr_dataset_names[num_columns]); 
				mcheckedListBox_datasets.Items.Add(marr_dataset_names[num_columns], CheckState.Checked); 
			}
		}

		private void PlotData(int baseline_column)
		{
			mctl_scatter.SeriesCollection.Clear() ; 
			mctl_histograms.SeriesCollection.Clear() ; 

			mctl_scatter.XAxisLabel = "Log Intensity (baseline: " + marr_dataset_names[baseline_column] + ")"; 
			mctl_scatter.YAxisLabel = "Log Intensity (Alignee)"; 

			mctl_histograms.XAxisLabel = "Log Intensity (Alignee/baseline:" + marr_dataset_names[baseline_column] + ")"; 
			mctl_histograms.YAxisLabel = "Number of Alignee Features"; 


			// for each selected dataset, fill the chart.. 
			float [] baseline_intensities = new float[0] ; 
			float [] dataset_intensities = new float[0] ; 
			float [] ratios = new float[0] ; 

			for (int index = 0 ; index < mcheckedListBox_datasets.CheckedIndices.Count ; index++)
			{
				int checked_index = mcheckedListBox_datasets.CheckedIndices[index] ; 
				if (checked_index == baseline_column)
					continue ; 
				GetNonZeroCommonPoints(baseline_column, checked_index, ref baseline_intensities, ref dataset_intensities) ;
				FillScatterChartData(baseline_column, checked_index, baseline_intensities, dataset_intensities) ; 
				CalculateRatios(ref ratios, baseline_intensities, dataset_intensities) ; 
				FillHistogramData(baseline_column, checked_index, ratios) ; 
				// get metric
			}
			FillVariabilityMetricsGrid() ;
		}

		private void FillVariabilityMetricsGrid()
		{
			int num_selected = mcheckedListBox_datasets.CheckedIndices.Count ; 

			bool bln_select_stdev = mradioButton_stdev.Checked ; 
			float [] variability_metrics = new float[num_selected*num_selected] ; 

			float [] baseline_intensities = new float[0] ; 
			float [] dataset_intensities = new float[0] ; 
			float [] ratios = new float[0] ; 

			for (int baseline_index = 0 ; baseline_index < mcheckedListBox_datasets.CheckedIndices.Count ; baseline_index++)
			{
				int baseline_column = mcheckedListBox_datasets.CheckedIndices[baseline_index] ; 
				if (bln_select_stdev)
				{
					variability_metrics[baseline_index* num_selected + baseline_index] = 0 ; 
				}
				else
				{
					variability_metrics[baseline_index* num_selected + baseline_index] = 1 ; 
				}

				for (int index = baseline_index + 1 ; index < mcheckedListBox_datasets.CheckedIndices.Count ; index++)
				{
					int checked_index = mcheckedListBox_datasets.CheckedIndices[index] ; 
					GetNonZeroCommonPoints(baseline_column, checked_index, ref baseline_intensities, ref dataset_intensities) ;
					CalculateRatios(ref ratios, baseline_intensities, dataset_intensities) ; 
					if (bln_select_stdev)
					{
						float stdev = clsMathUtilities.StandardDeviation(ratios) ; 
						variability_metrics[index * num_selected + baseline_index] = stdev ; 
						variability_metrics[baseline_index * num_selected + index] = stdev ; 
					}
					else
					{
						double r = clsMathUtilities.r(baseline_intensities, dataset_intensities) ; 
						variability_metrics[index * num_selected + baseline_index] = Convert.ToSingle(r) ; 
						variability_metrics[baseline_index * num_selected + index] = Convert.ToSingle(r) ; 
					}
				}
			}
			AddMetricToGrid(variability_metrics) ; 
		}

		private void AddMetricToGrid(float [] variability_metrics)
		{
			int num_selected = mcheckedListBox_datasets.CheckedIndices.Count ; 
			// now add stuff to the grid. 
			int selected_baseline = mcomboBox_baseline.SelectedIndex ; 
			if (selected_baseline < 0)
				return ; 

			DataTable table = new DataTable("Baseline: " + marr_dataset_names[selected_baseline]) ; 

			try
			{
				table.Columns.Add("baseline") ; 
				for (int baseline_index = 0 ; baseline_index < mcheckedListBox_datasets.CheckedIndices.Count ; baseline_index++)
				{
					int baseline_column = mcheckedListBox_datasets.CheckedIndices[baseline_index] ; 
					table.Columns.Add(marr_dataset_names[baseline_column]) ; 
				}
				for (int baseline_index = 0 ; baseline_index < num_selected ; baseline_index++)
				{
					DataRow row = table.NewRow() ;
					int baseline_column = mcheckedListBox_datasets.CheckedIndices[baseline_index] ; 
					row[0] = marr_dataset_names[baseline_column] ; 

					for (int dataset_index = 0 ; dataset_index < num_selected ; dataset_index++)
					{
						int dataset_column = mcheckedListBox_datasets.CheckedIndices[dataset_index] ; 
						float score = variability_metrics[baseline_index * num_selected + dataset_index] ;
						row[1+dataset_index] = Convert.ToString(score) ; 
					}
					table.Rows.Add(row) ; 
				}
				mdataGrid_variability.DataSource = table ; 
			}
			catch (Exception ex)
			{
                System.Diagnostics.Trace.WriteLine(ex.Message);
			}

		}

		private void FillScatterChartData(int baseline_column, int dataset_column, float [] baseline_intensities, 
			float [] dataset_intensities)
		{
			Color clr = miter_color.GetColor(dataset_column) ; 
			PNNLControls.clsShape shape = new PNNLControls.DiamondShape(mint_pt_size, mbln_hollow) ;  ; 
			PNNLControls.clsPlotParams plt_params = new PNNLControls.clsPlotParams(shape, clr) ; 
			plt_params.Name = marr_dataset_names[dataset_column] ; 
			mctl_scatter.ViewPortHistory.Clear(); 
			mctl_scatter.AutoViewPortOnAddition = true ; 
			mctl_scatter.AddSeries(new PNNLControls.clsSeries(ref baseline_intensities, ref dataset_intensities, plt_params)) ; 
		}

		private void CalculateRatios(ref double[] ratios, double [] baseline_intensities, double [] dataset_intensities)
		{
			int num_ratios = baseline_intensities.Length ; 
			bool logarithmic = mradioButton_log.Checked ; 
			ratios = new double[num_ratios] ; 

			for (int ratio_num = 0 ; ratio_num < num_ratios ; ratio_num++)
			{
				double ratio = 0 ; 
				if (logarithmic)
				{
					ratio = dataset_intensities[ratio_num] - baseline_intensities[ratio_num] ; 
				}
				else
				{
					ratio = dataset_intensities[ratio_num] / baseline_intensities[ratio_num] ; 
				}
				ratios[ratio_num] = ratio ; 
			}
		}

		private void CalculateRatios(ref float[] ratios, float [] baseline_intensities, float [] dataset_intensities)
		{
			int num_ratios = baseline_intensities.Length ; 
			bool logarithmic = mradioButton_log.Checked ; 
			ratios = new float[num_ratios] ; 

			for (int ratio_num = 0 ; ratio_num < num_ratios ; ratio_num++)
			{
				float ratio = 0 ; 
				if (logarithmic)
				{
					ratio = dataset_intensities[ratio_num] - baseline_intensities[ratio_num] ; 
				}
				else
				{
					ratio = dataset_intensities[ratio_num] / baseline_intensities[ratio_num] ; 
				}
				ratios[ratio_num] = ratio ; 
			}
		}

		private void FillHistogramData(int baseline_column, int dataset_column, float [] ratios )
		{
			// ratios are ready, create a histogram. 
			clsHistogram histo = new clsHistogram(ratios) ; 
			// now plot them.
			Color clr = miter_color.GetColor(dataset_column) ; 
			PNNLControls.clsShape shape = new PNNLControls.DiamondShape(mint_pt_size, mbln_hollow) ;  ; 
			PNNLControls.clsPlotParams plt_params = new PNNLControls.clsPlotParams(shape, clr) ; 
			plt_params.Name = marr_dataset_names[dataset_column] ; 
			mctl_histograms.ViewPortHistory.Clear(); 
			mctl_histograms.AutoViewPortOnAddition = true ; 
			mctl_histograms.SeriesCollection.Add(new PNNLControls.clsSeries(ref histo.marr_values, ref histo.marr_frequency, plt_params)) ; 
		}

		private void GetNonZeroCommonPoints(int baseline_column, int dataset_column, ref float [] baseline_intensities, 
			ref float [] dataset_intensities)
		{
			try
			{
				// two pass process. In the first round figure out how many points are needed. 
				// in the second copy that over. 
				int num_to_copy = 0 ; 
				int num_copied = 0 ; 
				for (int cluster_num = 0 ; cluster_num < mint_num_rows ; cluster_num++)
				{
					int baseline_index = cluster_num * mint_num_columns + baseline_column ; 
					int dataset_index = cluster_num * mint_num_columns + dataset_column ; 
					double baseline_intensity = marr_intensities[baseline_index] ;
					double dataset_intensity = marr_intensities[dataset_index] ;
					if (!double.IsNaN(baseline_intensity) && !double.IsNaN(dataset_intensity) && baseline_intensity != 0 && dataset_intensity != 0)
					{
						num_to_copy++ ; 
					}
				}

				baseline_intensities = new float[num_to_copy] ; 
				dataset_intensities = new float[num_to_copy] ; 

				bool logarithmic = mradioButton_log.Checked ; 

				for (int cluster_num = 0 ; cluster_num < mint_num_rows ; cluster_num++)
				{
					int baseline_index = cluster_num * mint_num_columns + baseline_column ; 
					int dataset_index = cluster_num * mint_num_columns + dataset_column ; 
					double baseline_intensity = marr_intensities[baseline_index] ;
					double dataset_intensity = marr_intensities[dataset_index] ;

					if (!double.IsNaN(baseline_intensity) && !double.IsNaN(dataset_intensity) 
						&& baseline_intensity != 0 && dataset_intensity != 0)
					{
						if (logarithmic)
						{
							baseline_intensity = Math.Log10(baseline_intensity) ;
							dataset_intensity = Math.Log10(dataset_intensity) ;
						}

						baseline_intensities[num_copied] = Convert.ToSingle(baseline_intensity) ; 
						dataset_intensities[num_copied] = Convert.ToSingle(dataset_intensity) ; 
						num_copied++ ; 
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + " " + ex.StackTrace) ; 
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
                mobjAnalysis = null;
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            PNNLControls.PenProvider penProvider5 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider6 = new PNNLControls.PenProvider();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIntensityDiff));
            PNNLControls.PenProvider penProvider1 = new PNNLControls.PenProvider();
            PNNLControls.PenProvider penProvider2 = new PNNLControls.PenProvider();
            this.mexpandPanel_options = new PNNLControls.ExpandPanel(299);
            this.mdataGrid_variability = new System.Windows.Forms.DataGrid();
            this.mgroupBox_intensityScale = new System.Windows.Forms.GroupBox();
            this.mradioButton_normal = new System.Windows.Forms.RadioButton();
            this.mradioButton_log = new System.Windows.Forms.RadioButton();
            this.mbtnCancel = new System.Windows.Forms.Button();
            this.mcheckBox_Normalized = new System.Windows.Forms.CheckBox();
            this.mlabel_baselineID = new System.Windows.Forms.Label();
            this.mcheckedListBox_datasets = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mgroupBox_variabilityMetric = new System.Windows.Forms.GroupBox();
            this.mradioButton_correl = new System.Windows.Forms.RadioButton();
            this.mradioButton_stdev = new System.Windows.Forms.RadioButton();
            this.mcomboBox_baseline = new System.Windows.Forms.ComboBox();
            this.mbtnOK = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.mpanel_plots = new System.Windows.Forms.Panel();
            this.mctl_histograms = new PNNLControls.ctlLineChart();
            this.mctl_scatter = new PNNLControls.ctlScatterChart();
            this.msplitter_plots = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.mexpandPanel_options)).BeginInit();
            this.mexpandPanel_options.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mdataGrid_variability)).BeginInit();
            this.mgroupBox_intensityScale.SuspendLayout();
            this.mgroupBox_variabilityMetric.SuspendLayout();
            this.mpanel_plots.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mctl_histograms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mctl_scatter)).BeginInit();
            this.SuspendLayout();
            // 
            // mexpandPanel_options
            // 
            this.mexpandPanel_options.Controls.Add(this.mdataGrid_variability);
            this.mexpandPanel_options.Controls.Add(this.mgroupBox_intensityScale);
            this.mexpandPanel_options.Controls.Add(this.mbtnCancel);
            this.mexpandPanel_options.Controls.Add(this.mcheckBox_Normalized);
            this.mexpandPanel_options.Controls.Add(this.mlabel_baselineID);
            this.mexpandPanel_options.Controls.Add(this.mcheckedListBox_datasets);
            this.mexpandPanel_options.Controls.Add(this.label2);
            this.mexpandPanel_options.Controls.Add(this.mgroupBox_variabilityMetric);
            this.mexpandPanel_options.Controls.Add(this.mcomboBox_baseline);
            this.mexpandPanel_options.Controls.Add(this.mbtnOK);
            this.mexpandPanel_options.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mexpandPanel_options.HeaderRightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mexpandPanel_options.HeaderTextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mexpandPanel_options.Location = new System.Drawing.Point(0, 717);
            this.mexpandPanel_options.Name = "mexpandPanel_options";
            this.mexpandPanel_options.Size = new System.Drawing.Size(1296, 319);
            this.mexpandPanel_options.TabIndex = 0;
            // 
            // mdataGrid_variability
            // 
            this.mdataGrid_variability.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mdataGrid_variability.DataMember = "";
            this.mdataGrid_variability.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.mdataGrid_variability.Location = new System.Drawing.Point(619, 23);
            this.mdataGrid_variability.Name = "mdataGrid_variability";
            this.mdataGrid_variability.Size = new System.Drawing.Size(673, 248);
            this.mdataGrid_variability.TabIndex = 0;
            // 
            // mgroupBox_intensityScale
            // 
            this.mgroupBox_intensityScale.Controls.Add(this.mradioButton_normal);
            this.mgroupBox_intensityScale.Controls.Add(this.mradioButton_log);
            this.mgroupBox_intensityScale.Location = new System.Drawing.Point(4, 27);
            this.mgroupBox_intensityScale.Name = "mgroupBox_intensityScale";
            this.mgroupBox_intensityScale.Size = new System.Drawing.Size(120, 58);
            this.mgroupBox_intensityScale.TabIndex = 7;
            this.mgroupBox_intensityScale.TabStop = false;
            this.mgroupBox_intensityScale.Text = "Intensity Scale";
            // 
            // mradioButton_normal
            // 
            this.mradioButton_normal.Location = new System.Drawing.Point(16, 35);
            this.mradioButton_normal.Name = "mradioButton_normal";
            this.mradioButton_normal.Size = new System.Drawing.Size(88, 16);
            this.mradioButton_normal.TabIndex = 1;
            this.mradioButton_normal.Text = "normal";
            // 
            // mradioButton_log
            // 
            this.mradioButton_log.Checked = true;
            this.mradioButton_log.Location = new System.Drawing.Point(16, 16);
            this.mradioButton_log.Name = "mradioButton_log";
            this.mradioButton_log.Size = new System.Drawing.Size(88, 16);
            this.mradioButton_log.TabIndex = 0;
            this.mradioButton_log.TabStop = true;
            this.mradioButton_log.Text = "logarithmic";
            // 
            // mbtnCancel
            // 
            this.mbtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbtnCancel.Location = new System.Drawing.Point(1208, 284);
            this.mbtnCancel.Name = "mbtnCancel";
            this.mbtnCancel.Size = new System.Drawing.Size(84, 31);
            this.mbtnCancel.TabIndex = 1;
            this.mbtnCancel.Text = "Cancel";
            this.mbtnCancel.UseVisualStyleBackColor = true;
            // 
            // mcheckBox_Normalized
            // 
            this.mcheckBox_Normalized.Location = new System.Drawing.Point(4, 91);
            this.mcheckBox_Normalized.Name = "mcheckBox_Normalized";
            this.mcheckBox_Normalized.Size = new System.Drawing.Size(120, 24);
            this.mcheckBox_Normalized.TabIndex = 9;
            this.mcheckBox_Normalized.Text = "Show Normalized Data";
            this.mcheckBox_Normalized.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mcheckBox_Normalized.CheckedChanged += new System.EventHandler(this.mcheckBox_Normalized_CheckedChanged);
            // 
            // mlabel_baselineID
            // 
            this.mlabel_baselineID.Location = new System.Drawing.Point(1, 118);
            this.mlabel_baselineID.Name = "mlabel_baselineID";
            this.mlabel_baselineID.Size = new System.Drawing.Size(107, 24);
            this.mlabel_baselineID.TabIndex = 3;
            this.mlabel_baselineID.Text = "Select Baseline:";
            this.mlabel_baselineID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mcheckedListBox_datasets
            // 
            this.mcheckedListBox_datasets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mcheckedListBox_datasets.Location = new System.Drawing.Point(4, 192);
            this.mcheckedListBox_datasets.Name = "mcheckedListBox_datasets";
            this.mcheckedListBox_datasets.Size = new System.Drawing.Size(601, 79);
            this.mcheckedListBox_datasets.TabIndex = 3;
            this.mcheckedListBox_datasets.ThreeDCheckBoxes = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(1, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select Datasets:";
            // 
            // mgroupBox_variabilityMetric
            // 
            this.mgroupBox_variabilityMetric.Controls.Add(this.mradioButton_correl);
            this.mgroupBox_variabilityMetric.Controls.Add(this.mradioButton_stdev);
            this.mgroupBox_variabilityMetric.Location = new System.Drawing.Point(130, 27);
            this.mgroupBox_variabilityMetric.Name = "mgroupBox_variabilityMetric";
            this.mgroupBox_variabilityMetric.Size = new System.Drawing.Size(141, 58);
            this.mgroupBox_variabilityMetric.TabIndex = 8;
            this.mgroupBox_variabilityMetric.TabStop = false;
            this.mgroupBox_variabilityMetric.Text = "Variability Metric";
            // 
            // mradioButton_correl
            // 
            this.mradioButton_correl.Location = new System.Drawing.Point(16, 35);
            this.mradioButton_correl.Name = "mradioButton_correl";
            this.mradioButton_correl.Size = new System.Drawing.Size(88, 16);
            this.mradioButton_correl.TabIndex = 1;
            this.mradioButton_correl.Text = "Correlation Coefficient";
            // 
            // mradioButton_stdev
            // 
            this.mradioButton_stdev.Location = new System.Drawing.Point(16, 16);
            this.mradioButton_stdev.Name = "mradioButton_stdev";
            this.mradioButton_stdev.Size = new System.Drawing.Size(120, 16);
            this.mradioButton_stdev.TabIndex = 0;
            this.mradioButton_stdev.Text = "Standard Deviation";
            // 
            // mcomboBox_baseline
            // 
            this.mcomboBox_baseline.Location = new System.Drawing.Point(4, 145);
            this.mcomboBox_baseline.Name = "mcomboBox_baseline";
            this.mcomboBox_baseline.Size = new System.Drawing.Size(601, 21);
            this.mcomboBox_baseline.TabIndex = 1;
            this.mcomboBox_baseline.Text = "Select Baseline:";
            // 
            // mbtnOK
            // 
            this.mbtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbtnOK.Location = new System.Drawing.Point(1118, 284);
            this.mbtnOK.Name = "mbtnOK";
            this.mbtnOK.Size = new System.Drawing.Size(84, 31);
            this.mbtnOK.TabIndex = 0;
            this.mbtnOK.Text = "OK";
            this.mbtnOK.UseVisualStyleBackColor = true;
            this.mbtnOK.Click += new System.EventHandler(this.mbtnOK_Click);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 712);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1296, 5);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // mpanel_plots
            // 
            this.mpanel_plots.Controls.Add(this.mctl_histograms);
            this.mpanel_plots.Controls.Add(this.msplitter_plots);
            this.mpanel_plots.Controls.Add(this.mctl_scatter);
            this.mpanel_plots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_plots.Location = new System.Drawing.Point(0, 0);
            this.mpanel_plots.Name = "mpanel_plots";
            this.mpanel_plots.Size = new System.Drawing.Size(1296, 712);
            this.mpanel_plots.TabIndex = 3;
            // 
            // mctl_histograms
            // 
            this.mctl_histograms.AutoViewPortXBase = 0F;
            this.mctl_histograms.AutoViewPortYBase = 0F;
            this.mctl_histograms.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.mctl_histograms.AxisAndLabelMaxFontSize = 15;
            this.mctl_histograms.AxisAndLabelMinFontSize = 10;
            this.mctl_histograms.ChartBackgroundColor = System.Drawing.Color.White;
            this.mctl_histograms.ChartLayout.LegendFraction = 0.2F;
            this.mctl_histograms.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Floating;
            this.mctl_histograms.ChartLayout.MaxLegendHeight = 150;
            this.mctl_histograms.ChartLayout.MaxLegendWidth = 250;
            this.mctl_histograms.ChartLayout.MaxTitleHeight = 50;
            this.mctl_histograms.ChartLayout.MinLegendHeight = 50;
            this.mctl_histograms.ChartLayout.MinLegendWidth = 75;
            this.mctl_histograms.ChartLayout.MinTitleHeight = 15;
            this.mctl_histograms.ChartLayout.TitleFraction = 0.1F;
            this.mctl_histograms.DefaultZoomHandler.Active = true;
            this.mctl_histograms.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.mctl_histograms.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            this.mctl_histograms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mctl_histograms.DrawPeakLabels = true;
            this.mctl_histograms.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            penProvider5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider5.Width = 1F;
            this.mctl_histograms.GridLinePen = penProvider5;
            this.mctl_histograms.HilightColor = System.Drawing.Color.Magenta;
            this.mctl_histograms.LabelOffset = 8F;
            this.mctl_histograms.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider6.Color = System.Drawing.Color.Black;
            penProvider6.Width = 1F;
            this.mctl_histograms.Legend.BorderPen = penProvider6;
            this.mctl_histograms.Legend.Bounds = new System.Drawing.Rectangle(10, 10, 130, 200);
            this.mctl_histograms.Legend.ColumnWidth = 125;
            this.mctl_histograms.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.mctl_histograms.Legend.MaxFontSize = 10F;
            this.mctl_histograms.Legend.MinFontSize = 6F;
            this.mctl_histograms.Location = new System.Drawing.Point(684, 0);
            this.mctl_histograms.Margins.BottomMarginFraction = 0.2F;
            this.mctl_histograms.Margins.BottomMarginMax = 72;
            this.mctl_histograms.Margins.BottomMarginMin = 30;
            this.mctl_histograms.Margins.DefaultMarginFraction = 0.05F;
            this.mctl_histograms.Margins.DefaultMarginMax = 15;
            this.mctl_histograms.Margins.DefaultMarginMin = 5;
            this.mctl_histograms.Margins.LeftMarginFraction = 0.3F;
            this.mctl_histograms.Margins.LeftMarginMax = 150;
            this.mctl_histograms.Margins.LeftMarginMin = 72;
            this.mctl_histograms.Name = "mctl_histograms";
            this.mctl_histograms.NumXBins = 20;
            this.mctl_histograms.Size = new System.Drawing.Size(612, 712);
            this.mctl_histograms.TabIndex = 0;
            this.mctl_histograms.Title = "Intensity Ratio Histogram";
            this.mctl_histograms.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.mctl_histograms.TitleMaxFontSize = 12F;
            this.mctl_histograms.TitleMinFontSize = 6F;
            this.mctl_histograms.VerticalExpansion = 1F;
            this.mctl_histograms.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("mctl_histograms.ViewPort")));
            this.mctl_histograms.XAxisLabel = "log(alignee/baseline)";
            this.mctl_histograms.YAxisLabel = "frequency";
            // 
            // mctl_scatter
            // 
            this.mctl_scatter.AutoViewPortXBase = 0F;
            this.mctl_scatter.AutoViewPortYBase = 0F;
            this.mctl_scatter.AxisAndLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.mctl_scatter.AxisAndLabelMaxFontSize = 15;
            this.mctl_scatter.AxisAndLabelMinFontSize = 10;
            this.mctl_scatter.ChartBackgroundColor = System.Drawing.Color.White;
            this.mctl_scatter.ChartLayout.LegendFraction = 0.2F;
            this.mctl_scatter.ChartLayout.LegendLocation = PNNLControls.ChartLegendLocation.Floating;
            this.mctl_scatter.ChartLayout.MaxLegendHeight = 150;
            this.mctl_scatter.ChartLayout.MaxLegendWidth = 250;
            this.mctl_scatter.ChartLayout.MaxTitleHeight = 50;
            this.mctl_scatter.ChartLayout.MinLegendHeight = 50;
            this.mctl_scatter.ChartLayout.MinLegendWidth = 75;
            this.mctl_scatter.ChartLayout.MinTitleHeight = 15;
            this.mctl_scatter.ChartLayout.TitleFraction = 0.1F;
            this.mctl_scatter.DefaultZoomHandler.Active = true;
            this.mctl_scatter.DefaultZoomHandler.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.mctl_scatter.DefaultZoomHandler.LineColor = System.Drawing.Color.Black;
            this.mctl_scatter.Dock = System.Windows.Forms.DockStyle.Left;
            this.mctl_scatter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            penProvider1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            penProvider1.Width = 1F;
            this.mctl_scatter.GridLinePen = penProvider1;
            this.mctl_scatter.HilightColor = System.Drawing.Color.Magenta;
            this.mctl_scatter.Legend.BackColor = System.Drawing.Color.Transparent;
            penProvider2.Color = System.Drawing.Color.Black;
            penProvider2.Width = 1F;
            this.mctl_scatter.Legend.BorderPen = penProvider2;
            this.mctl_scatter.Legend.Bounds = new System.Drawing.Rectangle(10, 10, 130, 200);
            this.mctl_scatter.Legend.ColumnWidth = 125;
            this.mctl_scatter.Legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.mctl_scatter.Legend.MaxFontSize = 10F;
            this.mctl_scatter.Legend.MinFontSize = 6F;
            this.mctl_scatter.Location = new System.Drawing.Point(0, 0);
            this.mctl_scatter.Margins.BottomMarginFraction = 0.2F;
            this.mctl_scatter.Margins.BottomMarginMax = 70;
            this.mctl_scatter.Margins.BottomMarginMin = 30;
            this.mctl_scatter.Margins.DefaultMarginFraction = 0.05F;
            this.mctl_scatter.Margins.DefaultMarginMax = 15;
            this.mctl_scatter.Margins.DefaultMarginMin = 5;
            this.mctl_scatter.Margins.LeftMarginFraction = 0.3F;
            this.mctl_scatter.Margins.LeftMarginMax = 150;
            this.mctl_scatter.Margins.LeftMarginMin = 72;
            this.mctl_scatter.Name = "mctl_scatter";
            this.mctl_scatter.Padding = new System.Windows.Forms.Padding(2);
            this.mctl_scatter.Size = new System.Drawing.Size(681, 712);
            this.mctl_scatter.TabIndex = 2;
            this.mctl_scatter.Title = "Cluster Intensity";
            this.mctl_scatter.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.mctl_scatter.TitleMaxFontSize = 12F;
            this.mctl_scatter.TitleMinFontSize = 5F;
            this.mctl_scatter.VerticalExpansion = 1F;
            this.mctl_scatter.ViewPort = ((System.Drawing.RectangleF)(resources.GetObject("mctl_scatter.ViewPort")));
            this.mctl_scatter.XAxisLabel = "log(baseline)";
            this.mctl_scatter.YAxisLabel = "log(alignee)";
            // 
            // msplitter_plots
            // 
            this.msplitter_plots.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.msplitter_plots.Location = new System.Drawing.Point(681, 0);
            this.msplitter_plots.Name = "msplitter_plots";
            this.msplitter_plots.Size = new System.Drawing.Size(3, 712);
            this.msplitter_plots.TabIndex = 3;
            this.msplitter_plots.TabStop = false;
            // 
            // frmIntensityDiff
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1296, 1036);
            this.Controls.Add(this.mpanel_plots);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.mexpandPanel_options);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmIntensityDiff";
            this.Text = "Scatter Plots/Histograms of Abundance";
            ((System.ComponentModel.ISupportInitialize)(this.mexpandPanel_options)).EndInit();
            this.mexpandPanel_options.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mdataGrid_variability)).EndInit();
            this.mgroupBox_intensityScale.ResumeLayout(false);
            this.mgroupBox_variabilityMetric.ResumeLayout(false);
            this.mpanel_plots.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mctl_histograms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mctl_scatter)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void mcheckedListBox_datasets_SelectedIndexChanged(object sender, EventArgs e)
		{
			int current_baseline = mcomboBox_baseline.SelectedIndex ; 
			if (current_baseline >= 0)
			{
				PlotData(current_baseline) ; 
			}
		}

		private void mcomboBox_baseline_SelectedIndexChanged(object sender, EventArgs e)
		{
			int current_baseline = mcomboBox_baseline.SelectedIndex ; 
			if (current_baseline >= 0)
			{
				PlotData(current_baseline) ; 
			}
		}

		private void mradioButton_log_CheckedChanged(object sender, EventArgs e)
		{
			int current_baseline = mcomboBox_baseline.SelectedIndex ; 
			if (current_baseline >= 0)
			{
				PlotData(current_baseline) ; 
			}
		}

		private void mdataGrid_variability_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				MenuItem mnu_normalize = new MenuItem("Normalize");
				mnu_normalize.Click +=new EventHandler(mnu_normalize_Click);
				// Create the popup menu object
				ContextMenu popup = new ContextMenu();

				// Define the list of menu commands
				popup.MenuItems.AddRange(new MenuItem[]{mnu_normalize});
				Point screenPoint = mdataGrid_variability.PointToScreen(new Point(e.X, e.Y)) ;
				// Convert from Screen coordinates to Form coordinates 
				Point formPoint = PointToClient( screenPoint ); 
				popup.Show(this, formPoint);
//				popup.Show(this, screenPoint);
			}
		}

		private void mnu_normalize_Click(object sender, System.EventArgs e)
		{
			clsNormalize objNormalizer = new clsNormalize() ; 
			objNormalizer.NormalizeData(mobjAnalysis) ; 
			mcheckBox_Normalized.Enabled = true ; 
		}

		private void mcheckBox_Normalized_CheckedChanged(object sender, System.EventArgs e)
		{
			if (mcheckBox_Normalized.Checked)
				marr_intensities = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensityNormalized ; 
			else
				marr_intensities = mobjAnalysis.UMCData.mobjClusterData.marrClusterIntensity ; 

			int current_baseline = mcomboBox_baseline.SelectedIndex ; 
			if (current_baseline >= 0)
			{
				PlotData(current_baseline) ; 
			}
		
		}

        private void mbtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
	}
}
