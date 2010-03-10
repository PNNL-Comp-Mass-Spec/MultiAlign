using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using MultiAlignEngine.Alignment;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmMSAlignmentOptions.
	/// </summary>
	public class frmMSAlignmentParameters : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl mtabControlOptions;
		private System.Windows.Forms.TabPage mtabPageNetOptions;
		private System.Windows.Forms.TabPage mtabPageMassOptions;
		private System.Windows.Forms.TabPage mtabPageTolerances;
		private System.Windows.Forms.TabPage mtabPageCalibrationType;
		private System.Windows.Forms.Button mbutton_ok;
		private System.Windows.Forms.Button mbutton_cancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox mtxtNumSections;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox mtxtContractionFactor;
		private System.Windows.Forms.TextBox mtxtMaxDistortion;
		private System.Windows.Forms.CheckBox mcheckBox_ignore_promiscous;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox mtextBox_mass_window;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox mtextBox_num_mass_delta_bins;
		private System.Windows.Forms.TextBox mtextBox_mass_jump;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox mtextBox_NET_tolerance;
		private System.Windows.Forms.TextBox mtextBox_mass_tolerance;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton mradioButton_mz;
		private System.Windows.Forms.RadioButton mradioButton_net;
		private System.Windows.Forms.CheckBox mcheckBox_recalibrate;
		private System.Windows.Forms.TextBox mtextBox_max_promiscuity;
		private System.Windows.Forms.TextBox mtextBox_mass_num_xslices;
		private System.Windows.Forms.RadioButton mradioButton_hybrid;
		private System.Windows.Forms.TextBox mtextBox_outlier_zscore;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox mcheckBox_use_lsq;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox mtextBox_num_knots;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox mtextBox_lsq_outlier_zscore;
		private System.Windows.Forms.Button mbtnDefaults;
        private TabPage mtabPage_alignmentSplit;
        private CheckBox mcheckBox_splitAlignment;
        private Label mlabel_splitAlignment;
        private NumericUpDown mnum_splitAlignment;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private Label label15;
        private Label label14;
        private NumericUpDown mnum_NETBinSize;
        private NumericUpDown mnum_massBinSize;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMSAlignmentParameters()
		{
			InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMSAlignmentParameters));
            this.mtabControlOptions = new System.Windows.Forms.TabControl();
            this.mtabPageNetOptions = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mtxtMaxDistortion = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mtxtContractionFactor = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mtxtNumSections = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mtextBox_max_promiscuity = new System.Windows.Forms.TextBox();
            this.mcheckBox_ignore_promiscous = new System.Windows.Forms.CheckBox();
            this.mtabPageMassOptions = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mtextBox_lsq_outlier_zscore = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.mtextBox_num_knots = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.mcheckBox_use_lsq = new System.Windows.Forms.CheckBox();
            this.mtextBox_outlier_zscore = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mtextBox_mass_jump = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.mtextBox_mass_num_xslices = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.mtextBox_mass_window = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mtextBox_num_mass_delta_bins = new System.Windows.Forms.TextBox();
            this.mtabPageTolerances = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.mnum_NETBinSize = new System.Windows.Forms.NumericUpDown();
            this.mnum_massBinSize = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.mtextBox_mass_tolerance = new System.Windows.Forms.TextBox();
            this.mtextBox_NET_tolerance = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.mcheckBox_recalibrate = new System.Windows.Forms.CheckBox();
            this.mtabPageCalibrationType = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mradioButton_hybrid = new System.Windows.Forms.RadioButton();
            this.mradioButton_net = new System.Windows.Forms.RadioButton();
            this.mradioButton_mz = new System.Windows.Forms.RadioButton();
            this.mtabPage_alignmentSplit = new System.Windows.Forms.TabPage();
            this.mlabel_splitAlignment = new System.Windows.Forms.Label();
            this.mnum_splitAlignment = new System.Windows.Forms.NumericUpDown();
            this.mcheckBox_splitAlignment = new System.Windows.Forms.CheckBox();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.mbtnDefaults = new System.Windows.Forms.Button();
            this.mtabControlOptions.SuspendLayout();
            this.mtabPageNetOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mtabPageMassOptions.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.mtabPageTolerances.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_NETBinSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_massBinSize)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.mtabPageCalibrationType.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mtabPage_alignmentSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_splitAlignment)).BeginInit();
            this.SuspendLayout();
            // 
            // mtabControlOptions
            // 
            this.mtabControlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtabControlOptions.Controls.Add(this.mtabPageNetOptions);
            this.mtabControlOptions.Controls.Add(this.mtabPageMassOptions);
            this.mtabControlOptions.Controls.Add(this.mtabPageTolerances);
            this.mtabControlOptions.Controls.Add(this.mtabPageCalibrationType);
            this.mtabControlOptions.Controls.Add(this.mtabPage_alignmentSplit);
            this.mtabControlOptions.Location = new System.Drawing.Point(6, 7);
            this.mtabControlOptions.Name = "mtabControlOptions";
            this.mtabControlOptions.SelectedIndex = 0;
            this.mtabControlOptions.Size = new System.Drawing.Size(541, 160);
            this.mtabControlOptions.TabIndex = 0;
            // 
            // mtabPageNetOptions
            // 
            this.mtabPageNetOptions.BackColor = System.Drawing.Color.White;
            this.mtabPageNetOptions.Controls.Add(this.groupBox1);
            this.mtabPageNetOptions.Location = new System.Drawing.Point(4, 22);
            this.mtabPageNetOptions.Name = "mtabPageNetOptions";
            this.mtabPageNetOptions.Size = new System.Drawing.Size(533, 134);
            this.mtabPageNetOptions.TabIndex = 0;
            this.mtabPageNetOptions.Text = "NET Options";
            this.mtabPageNetOptions.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.mtxtMaxDistortion);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.mtxtContractionFactor);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.mtxtNumSections);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.mtextBox_max_promiscuity);
            this.groupBox1.Controls.Add(this.mcheckBox_ignore_promiscous);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(533, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "NET Warp Options";
            // 
            // mtxtMaxDistortion
            // 
            this.mtxtMaxDistortion.Location = new System.Drawing.Point(117, 47);
            this.mtxtMaxDistortion.Name = "mtxtMaxDistortion";
            this.mtxtMaxDistortion.Size = new System.Drawing.Size(43, 20);
            this.mtxtMaxDistortion.TabIndex = 5;
            this.mtxtMaxDistortion.Text = "10";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(15, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Max Distortion:";
            // 
            // mtxtContractionFactor
            // 
            this.mtxtContractionFactor.Location = new System.Drawing.Point(304, 18);
            this.mtxtContractionFactor.Name = "mtxtContractionFactor";
            this.mtxtContractionFactor.Size = new System.Drawing.Size(32, 20);
            this.mtxtContractionFactor.TabIndex = 3;
            this.mtxtContractionFactor.Text = "2";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(181, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Contraction Factor:";
            // 
            // mtxtNumSections
            // 
            this.mtxtNumSections.Location = new System.Drawing.Point(117, 16);
            this.mtxtNumSections.Name = "mtxtNumSections";
            this.mtxtNumSections.Size = new System.Drawing.Size(43, 20);
            this.mtxtNumSections.TabIndex = 1;
            this.mtxtNumSections.Text = "100";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "# of sections:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(183, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Max Promiscuity:";
            // 
            // mtextBox_max_promiscuity
            // 
            this.mtextBox_max_promiscuity.Location = new System.Drawing.Point(303, 45);
            this.mtextBox_max_promiscuity.Name = "mtextBox_max_promiscuity";
            this.mtextBox_max_promiscuity.Size = new System.Drawing.Size(32, 20);
            this.mtextBox_max_promiscuity.TabIndex = 7;
            this.mtextBox_max_promiscuity.Text = "3";
            // 
            // mcheckBox_ignore_promiscous
            // 
            this.mcheckBox_ignore_promiscous.Location = new System.Drawing.Point(17, 80);
            this.mcheckBox_ignore_promiscous.Name = "mcheckBox_ignore_promiscous";
            this.mcheckBox_ignore_promiscous.Size = new System.Drawing.Size(125, 24);
            this.mcheckBox_ignore_promiscous.TabIndex = 9;
            this.mcheckBox_ignore_promiscous.Text = "Ignore Promiscous";
            // 
            // mtabPageMassOptions
            // 
            this.mtabPageMassOptions.BackColor = System.Drawing.Color.White;
            this.mtabPageMassOptions.Controls.Add(this.groupBox3);
            this.mtabPageMassOptions.Controls.Add(this.mcheckBox_use_lsq);
            this.mtabPageMassOptions.Controls.Add(this.mtextBox_outlier_zscore);
            this.mtabPageMassOptions.Controls.Add(this.label9);
            this.mtabPageMassOptions.Controls.Add(this.mtextBox_mass_jump);
            this.mtabPageMassOptions.Controls.Add(this.label7);
            this.mtabPageMassOptions.Controls.Add(this.label8);
            this.mtabPageMassOptions.Controls.Add(this.mtextBox_mass_num_xslices);
            this.mtabPageMassOptions.Controls.Add(this.label6);
            this.mtabPageMassOptions.Controls.Add(this.mtextBox_mass_window);
            this.mtabPageMassOptions.Controls.Add(this.label5);
            this.mtabPageMassOptions.Controls.Add(this.mtextBox_num_mass_delta_bins);
            this.mtabPageMassOptions.Location = new System.Drawing.Point(4, 22);
            this.mtabPageMassOptions.Name = "mtabPageMassOptions";
            this.mtabPageMassOptions.Size = new System.Drawing.Size(533, 134);
            this.mtabPageMassOptions.TabIndex = 1;
            this.mtabPageMassOptions.Text = "Mass Options";
            this.mtabPageMassOptions.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.mtextBox_lsq_outlier_zscore);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.mtextBox_num_knots);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Location = new System.Drawing.Point(11, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(509, 48);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "LSQ Options";
            // 
            // mtextBox_lsq_outlier_zscore
            // 
            this.mtextBox_lsq_outlier_zscore.Location = new System.Drawing.Point(246, 13);
            this.mtextBox_lsq_outlier_zscore.Name = "mtextBox_lsq_outlier_zscore";
            this.mtextBox_lsq_outlier_zscore.Size = new System.Drawing.Size(37, 20);
            this.mtextBox_lsq_outlier_zscore.TabIndex = 13;
            this.mtextBox_lsq_outlier_zscore.Text = "2.5";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(157, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 16);
            this.label11.TabIndex = 12;
            this.label11.Text = "Outlier ZScore:";
            // 
            // mtextBox_num_knots
            // 
            this.mtextBox_num_knots.Location = new System.Drawing.Point(109, 16);
            this.mtextBox_num_knots.Name = "mtextBox_num_knots";
            this.mtextBox_num_knots.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_num_knots.TabIndex = 11;
            this.mtextBox_num_knots.Text = "12";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(6, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 16);
            this.label13.TabIndex = 10;
            this.label13.Text = "# of knots:";
            // 
            // mcheckBox_use_lsq
            // 
            this.mcheckBox_use_lsq.Location = new System.Drawing.Point(312, 43);
            this.mcheckBox_use_lsq.Name = "mcheckBox_use_lsq";
            this.mcheckBox_use_lsq.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mcheckBox_use_lsq.Size = new System.Drawing.Size(72, 16);
            this.mcheckBox_use_lsq.TabIndex = 10;
            this.mcheckBox_use_lsq.Text = "  LSQ Fit  ";
            // 
            // mtextBox_outlier_zscore
            // 
            this.mtextBox_outlier_zscore.Location = new System.Drawing.Point(256, 40);
            this.mtextBox_outlier_zscore.Name = "mtextBox_outlier_zscore";
            this.mtextBox_outlier_zscore.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_outlier_zscore.TabIndex = 9;
            this.mtextBox_outlier_zscore.Text = "3";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(168, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 16);
            this.label9.TabIndex = 8;
            this.label9.Text = "Outlier ZScore:";
            // 
            // mtextBox_mass_jump
            // 
            this.mtextBox_mass_jump.Location = new System.Drawing.Point(368, 16);
            this.mtextBox_mass_jump.Name = "mtextBox_mass_jump";
            this.mtextBox_mass_jump.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_mass_jump.TabIndex = 7;
            this.mtextBox_mass_jump.Text = "20";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(306, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "Max Jump:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 16);
            this.label8.TabIndex = 4;
            this.label8.Text = "# of Mass Delta Bins:";
            // 
            // mtextBox_mass_num_xslices
            // 
            this.mtextBox_mass_num_xslices.Location = new System.Drawing.Point(256, 16);
            this.mtextBox_mass_num_xslices.Name = "mtextBox_mass_num_xslices";
            this.mtextBox_mass_num_xslices.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_mass_num_xslices.TabIndex = 3;
            this.mtextBox_mass_num_xslices.Text = "12";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(168, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "# of x slices:";
            // 
            // mtextBox_mass_window
            // 
            this.mtextBox_mass_window.Location = new System.Drawing.Point(120, 16);
            this.mtextBox_mass_window.Name = "mtextBox_mass_window";
            this.mtextBox_mass_window.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_mass_window.TabIndex = 1;
            this.mtextBox_mass_window.Text = "6.0";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Mass Window (ppm):";
            // 
            // mtextBox_num_mass_delta_bins
            // 
            this.mtextBox_num_mass_delta_bins.Location = new System.Drawing.Point(120, 40);
            this.mtextBox_num_mass_delta_bins.Name = "mtextBox_num_mass_delta_bins";
            this.mtextBox_num_mass_delta_bins.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_num_mass_delta_bins.TabIndex = 5;
            this.mtextBox_num_mass_delta_bins.Text = "50";
            // 
            // mtabPageTolerances
            // 
            this.mtabPageTolerances.BackColor = System.Drawing.Color.White;
            this.mtabPageTolerances.Controls.Add(this.groupBox5);
            this.mtabPageTolerances.Controls.Add(this.groupBox4);
            this.mtabPageTolerances.Controls.Add(this.mcheckBox_recalibrate);
            this.mtabPageTolerances.Location = new System.Drawing.Point(4, 22);
            this.mtabPageTolerances.Name = "mtabPageTolerances";
            this.mtabPageTolerances.Size = new System.Drawing.Size(533, 134);
            this.mtabPageTolerances.TabIndex = 2;
            this.mtabPageTolerances.Text = "Tolerances and Histograms";
            this.mtabPageTolerances.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.mnum_NETBinSize);
            this.groupBox5.Controls.Add(this.mnum_massBinSize);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Location = new System.Drawing.Point(247, 13);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(265, 90);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Histogram Binning Options";
            // 
            // mnum_NETBinSize
            // 
            this.mnum_NETBinSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_NETBinSize.DecimalPlaces = 5;
            this.mnum_NETBinSize.Location = new System.Drawing.Point(146, 39);
            this.mnum_NETBinSize.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mnum_NETBinSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            327680});
            this.mnum_NETBinSize.Name = "mnum_NETBinSize";
            this.mnum_NETBinSize.Size = new System.Drawing.Size(93, 20);
            this.mnum_NETBinSize.TabIndex = 3;
            this.mnum_NETBinSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // mnum_massBinSize
            // 
            this.mnum_massBinSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_massBinSize.DecimalPlaces = 2;
            this.mnum_massBinSize.Location = new System.Drawing.Point(146, 16);
            this.mnum_massBinSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mnum_massBinSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mnum_massBinSize.Name = "mnum_massBinSize";
            this.mnum_massBinSize.Size = new System.Drawing.Size(93, 20);
            this.mnum_massBinSize.TabIndex = 2;
            this.mnum_massBinSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(13, 45);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "NET Bin Size";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Mass";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.mtextBox_mass_tolerance);
            this.groupBox4.Controls.Add(this.mtextBox_NET_tolerance);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(3, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 91);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tolerances";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(6, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 16);
            this.label12.TabIndex = 8;
            this.label12.Text = "Mass Tolerance (ppm):";
            // 
            // mtextBox_mass_tolerance
            // 
            this.mtextBox_mass_tolerance.Location = new System.Drawing.Point(137, 13);
            this.mtextBox_mass_tolerance.Name = "mtextBox_mass_tolerance";
            this.mtextBox_mass_tolerance.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_mass_tolerance.TabIndex = 9;
            this.mtextBox_mass_tolerance.Text = "6.0";
            // 
            // mtextBox_NET_tolerance
            // 
            this.mtextBox_NET_tolerance.Location = new System.Drawing.Point(137, 38);
            this.mtextBox_NET_tolerance.Name = "mtextBox_NET_tolerance";
            this.mtextBox_NET_tolerance.Size = new System.Drawing.Size(38, 20);
            this.mtextBox_NET_tolerance.TabIndex = 13;
            this.mtextBox_NET_tolerance.Text = "0.03";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(6, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 16);
            this.label10.TabIndex = 12;
            this.label10.Text = "NET Tolerance:";
            // 
            // mcheckBox_recalibrate
            // 
            this.mcheckBox_recalibrate.Checked = true;
            this.mcheckBox_recalibrate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcheckBox_recalibrate.Location = new System.Drawing.Point(11, 110);
            this.mcheckBox_recalibrate.Name = "mcheckBox_recalibrate";
            this.mcheckBox_recalibrate.Size = new System.Drawing.Size(162, 16);
            this.mcheckBox_recalibrate.TabIndex = 14;
            this.mcheckBox_recalibrate.Text = "Recalibrate Masses";
            // 
            // mtabPageCalibrationType
            // 
            this.mtabPageCalibrationType.BackColor = System.Drawing.Color.White;
            this.mtabPageCalibrationType.Controls.Add(this.groupBox2);
            this.mtabPageCalibrationType.Location = new System.Drawing.Point(4, 22);
            this.mtabPageCalibrationType.Name = "mtabPageCalibrationType";
            this.mtabPageCalibrationType.Size = new System.Drawing.Size(533, 134);
            this.mtabPageCalibrationType.TabIndex = 3;
            this.mtabPageCalibrationType.Text = "Calibration Type";
            this.mtabPageCalibrationType.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.mradioButton_hybrid);
            this.groupBox2.Controls.Add(this.mradioButton_net);
            this.groupBox2.Controls.Add(this.mradioButton_mz);
            this.groupBox2.Location = new System.Drawing.Point(2, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(528, 131);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Recalibration Type";
            // 
            // mradioButton_hybrid
            // 
            this.mradioButton_hybrid.Checked = true;
            this.mradioButton_hybrid.Location = new System.Drawing.Point(24, 74);
            this.mradioButton_hybrid.Name = "mradioButton_hybrid";
            this.mradioButton_hybrid.Size = new System.Drawing.Size(195, 24);
            this.mradioButton_hybrid.TabIndex = 2;
            this.mradioButton_hybrid.TabStop = true;
            this.mradioButton_hybrid.Text = "Hybrid Recalibration";
            // 
            // mradioButton_net
            // 
            this.mradioButton_net.Location = new System.Drawing.Point(24, 44);
            this.mradioButton_net.Name = "mradioButton_net";
            this.mradioButton_net.Size = new System.Drawing.Size(290, 24);
            this.mradioButton_net.TabIndex = 1;
            this.mradioButton_net.Text = "Recalibrate Mass using net relationship";
            // 
            // mradioButton_mz
            // 
            this.mradioButton_mz.Location = new System.Drawing.Point(24, 19);
            this.mradioButton_mz.Name = "mradioButton_mz";
            this.mradioButton_mz.Size = new System.Drawing.Size(184, 24);
            this.mradioButton_mz.TabIndex = 0;
            this.mradioButton_mz.Text = "Recalibrate m/z coefficients";
            // 
            // mtabPage_alignmentSplit
            // 
            this.mtabPage_alignmentSplit.Controls.Add(this.mlabel_splitAlignment);
            this.mtabPage_alignmentSplit.Controls.Add(this.mnum_splitAlignment);
            this.mtabPage_alignmentSplit.Controls.Add(this.mcheckBox_splitAlignment);
            this.mtabPage_alignmentSplit.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_alignmentSplit.Name = "mtabPage_alignmentSplit";
            this.mtabPage_alignmentSplit.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_alignmentSplit.Size = new System.Drawing.Size(533, 134);
            this.mtabPage_alignmentSplit.TabIndex = 4;
            this.mtabPage_alignmentSplit.Text = "Split Alignment (Advanced)";
            this.mtabPage_alignmentSplit.UseVisualStyleBackColor = true;
            // 
            // mlabel_splitAlignment
            // 
            this.mlabel_splitAlignment.AutoSize = true;
            this.mlabel_splitAlignment.Enabled = false;
            this.mlabel_splitAlignment.Location = new System.Drawing.Point(40, 40);
            this.mlabel_splitAlignment.Name = "mlabel_splitAlignment";
            this.mlabel_splitAlignment.Size = new System.Drawing.Size(72, 13);
            this.mlabel_splitAlignment.TabIndex = 2;
            this.mlabel_splitAlignment.Text = "m/z boundary";
            // 
            // mnum_splitAlignment
            // 
            this.mnum_splitAlignment.DecimalPlaces = 1;
            this.mnum_splitAlignment.Enabled = false;
            this.mnum_splitAlignment.Location = new System.Drawing.Point(121, 38);
            this.mnum_splitAlignment.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.mnum_splitAlignment.Name = "mnum_splitAlignment";
            this.mnum_splitAlignment.Size = new System.Drawing.Size(78, 20);
            this.mnum_splitAlignment.TabIndex = 1;
            this.mnum_splitAlignment.ValueChanged += new System.EventHandler(this.mnum_splitAlignment_ValueChanged);
            // 
            // mcheckBox_splitAlignment
            // 
            this.mcheckBox_splitAlignment.AutoSize = true;
            this.mcheckBox_splitAlignment.Location = new System.Drawing.Point(20, 15);
            this.mcheckBox_splitAlignment.Name = "mcheckBox_splitAlignment";
            this.mcheckBox_splitAlignment.Size = new System.Drawing.Size(179, 17);
            this.mcheckBox_splitAlignment.TabIndex = 0;
            this.mcheckBox_splitAlignment.Text = "Split Alignment at M/Z Boundary";
            this.mcheckBox_splitAlignment.UseVisualStyleBackColor = true;
            this.mcheckBox_splitAlignment.CheckedChanged += new System.EventHandler(this.mcheckBox_splitAlignment_CheckedChanged);
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_ok.Location = new System.Drawing.Point(233, 172);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(88, 24);
            this.mbutton_ok.TabIndex = 1;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_cancel.Location = new System.Drawing.Point(345, 172);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(88, 24);
            this.mbutton_cancel.TabIndex = 2;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // mbtnDefaults
            // 
            this.mbtnDefaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbtnDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnDefaults.Location = new System.Drawing.Point(129, 172);
            this.mbtnDefaults.Name = "mbtnDefaults";
            this.mbtnDefaults.Size = new System.Drawing.Size(80, 24);
            this.mbtnDefaults.TabIndex = 9;
            this.mbtnDefaults.Text = "Use Defaults";
            this.mbtnDefaults.Click += new System.EventHandler(this.mbtnDefaults_Click);
            // 
            // frmMSAlignmentParameters
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(553, 202);
            this.Controls.Add(this.mbtnDefaults);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.mtabControlOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMSAlignmentParameters";
            this.Text = "Alignment Options";
            this.mtabControlOptions.ResumeLayout(false);
            this.mtabPageNetOptions.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.mtabPageMassOptions.ResumeLayout(false);
            this.mtabPageMassOptions.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.mtabPageTolerances.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_NETBinSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_massBinSize)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.mtabPageCalibrationType.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.mtabPage_alignmentSplit.ResumeLayout(false);
            this.mtabPage_alignmentSplit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_splitAlignment)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		public MultiAlignEngine.Alignment.clsAlignmentOptions SetDefaults()
		{
			MultiAlignEngine.Alignment.clsAlignmentOptions defaults = new MultiAlignEngine.Alignment.clsAlignmentOptions() ;
			string tabName = mtabControlOptions.SelectedTab.Text ;
			MultiAlignEngine.Alignment.clsAlignmentOptions options = new MultiAlignEngine.Alignment.clsAlignmentOptions() ;
			options = AlignmentOptions ;

			switch (tabName)
			{
				case ("NET Options") :
					options.NumTimeSections         = defaults.NumTimeSections ;
					options.ContractionFactor       = defaults.ContractionFactor ;
					options.MaxTimeJump             = defaults.MaxTimeJump ; //mshortMaxTimeDistortion
					options.MaxPromiscuity          = defaults.MaxPromiscuity ;
					options.UsePromiscuousPoints    = defaults.UsePromiscuousPoints ;
					break ;
				case ("Mass Options") :
					options.MassCalibrationUseLSQ           = defaults.MassCalibrationUseLSQ ;
					options.MassCalibrationWindow           = defaults.MassCalibrationWindow ;
					options.MassCalibrationNumXSlices       = defaults.MassCalibrationNumXSlices ;
					options.MassCalibrationNumMassDeltaBins = defaults.MassCalibrationNumMassDeltaBins ; //mshortMassCalibNumYSlices
					options.MassCalibrationMaxJump          = defaults.MassCalibrationMaxJump ;
					options.MassCalibrationMaxZScore        = defaults.MassCalibrationMaxZScore ;
					options.MassCalibrationLSQZScore        = defaults.MassCalibrationLSQZScore ;
					options.MassCalibrationLSQNumKnots      = defaults.MassCalibrationLSQNumKnots ;
					break ;
				case ("Tolerances") :
					options.MassTolerance           = defaults.MassTolerance ;
					options.NETTolerance            = defaults.NETTolerance;
					options.ApplyMassRecalibration  = defaults.ApplyMassRecalibration ;

					break ;
				case("Calibration Type") :
					options.RecalibrationType       = defaults.RecalibrationType ;
					break ;
				default:
					Console.WriteLine("Default case");
					break;
			}	
			return options ; 
		}

        /// <summary>
        /// Saves the options from the user.
        /// </summary>
        private void SaveOptions()
        {
            Properties.Settings.Default.UserAlignmentOptionsNumSections       = NumTimeSections;
            Properties.Settings.Default.UserAlignmentOptionsMaxDistortion     = MaxTimeJump;
            Properties.Settings.Default.UserAlignmentOptionsContractionFactor = ContractionFactor;
            Properties.Settings.Default.UserAlignmentOptionsMaxPromiscuity    = MaxPromiscuity;
            Properties.Settings.Default.UserAlignmentOptionsIgnorePromiscuity = UsePromiscuousPoints;

            Properties.Settings.Default.UserAlignmentOptionsMassWindowPPM = MassCalibrationWindow;
            Properties.Settings.Default.UserAlignmentOptionsNumMassDeltaBins = MassCalibrationNumMassDeltaBins;
            Properties.Settings.Default.UserAlignmentOptionsNumXSlices = MassCalibrationNumXSlices;
            Properties.Settings.Default.UserAlignmentOptionsOutlierZScore = MassCalibrationMaxZScore;
            Properties.Settings.Default.UserAlignmentOptionsMaxJump = MassCalibrationMaxJump;
            Properties.Settings.Default.UserAlignmentOptionsUseLSQFit = mcheckBox_use_lsq.Checked;
            Properties.Settings.Default.UserAlignmentOptionsLSQNumOptions = MassCalibrationLSQNumKnots;
            Properties.Settings.Default.UserAlignmentOptionsLSQOutlierZScore = MassCalibrationLSQZScore;

            Properties.Settings.Default.UserAlignmentOptionsMassTolerance = MassTolerance;
            Properties.Settings.Default.UserAlignmentOptionsNETTolerance = NETTolerance;
            Properties.Settings.Default.UserAlignmentOptionsUseRecalibrateMasses = ApplyMassRecalibration;

            Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeMzCoeff          = (MultiAlignEngine.Alignment.enmCalibrationType.MZ_CALIB == RecalibrationType);
            Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeNetRelationship  = (MultiAlignEngine.Alignment.enmCalibrationType.SCAN_CALIB == RecalibrationType);
            Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeHybrid           = (MultiAlignEngine.Alignment.enmCalibrationType.HYBRID_CALIB == RecalibrationType);
            Properties.Settings.Default.UserAlignmentOptionsMassBinSize                     = MassBinSize;
            Properties.Settings.Default.UserAlignmentOptionsNETBinSize                      = NETBinSize;

            Properties.Settings.Default.Save();
        }

        #region Event Handlers
        /// <summary>
        /// Selects these options for alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_ok_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK ;
            SaveOptions();
			this.Hide() ; 
		}
        /// <summary>
        ///  cancels using these options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mbutton_cancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel ; 
			this.Hide() ; 		
		}
        /// <summary>
        /// Sets the options to their default values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mbtnDefaults_Click(object sender, System.EventArgs e)
		{
			this.AlignmentOptions = SetDefaults() ;
		}
        /// <summary>
        /// Handles setting the alignment split option at a m/z boundary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcheckBox_splitAlignment_CheckedChanged(object sender, EventArgs e)
        {            
            mlabel_splitAlignment.Enabled = mcheckBox_splitAlignment.Checked;
            mnum_splitAlignment.Enabled = mcheckBox_splitAlignment.Checked;
        }
        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the mass bin size for the alignment error histograms.
        /// </summary>
        public double MassBinSize
        {
            get
            {
                return Convert.ToDouble(mnum_massBinSize.Value);
            }
            set
            {
                decimal d = Convert.ToDecimal(value);
                d         = Math.Min(Math.Max(d, mnum_massBinSize.Minimum), mnum_massBinSize.Maximum);
                mnum_massBinSize.Value = d;
            }
        }
        /// <summary>
        /// Gets or sets the NET bin size for the alignment error histograms.
        /// </summary>
        public double NETBinSize
        {
            get
            {
                return Convert.ToDouble(mnum_NETBinSize.Value);
            }
            set
            {
                try
                {
                    mnum_NETBinSize.Value = Convert.ToDecimal(value);
                }
                catch
                {
                }
            }
        }
        public int NumTimeSections
		{
			get
			{
				return Convert.ToInt32(mtxtNumSections.Text) ; 
			}
			set
			{
				mtxtNumSections.Text = Convert.ToString(value) ; 
			}
		}
		public short ContractionFactor
		{
			get
			{
				return Convert.ToInt16(mtxtContractionFactor.Text) ; 
			}
			set
			{
				mtxtContractionFactor.Text = Convert.ToString(value) ; 
			}
		}

		public short MaxTimeJump
		{
			get
			{
				return Convert.ToInt16(mtxtMaxDistortion.Text) ; 
			}
			set
			{
				mtxtMaxDistortion.Text = Convert.ToString(value) ; 
			}
		}

		public short MaxPromiscuity
		{
			get
			{
				return Convert.ToInt16(mtextBox_max_promiscuity.Text) ; 
			}
			set
			{
				mtextBox_max_promiscuity.Text = Convert.ToString(value) ; 
			}
		}

		public bool UsePromiscuousPoints
		{
			get
			{
				return !mcheckBox_ignore_promiscous.Checked ; 
			}
			set
			{
				mcheckBox_ignore_promiscous.Checked  = !value ; 
			}
		}

		public bool MassCalibrationUseLSQ
		{
			get
			{
				return mcheckBox_use_lsq.Checked ; 
			}
			set
			{
				mcheckBox_use_lsq.Checked = value ; 
			}
		}
		public double MassCalibrationWindow
		{
			get
			{
				return Convert.ToDouble(mtextBox_mass_window.Text) ; 
			}
			set
			{
				mtextBox_mass_window.Text = Convert.ToString(value) ; 
			}
		}

		public short MassCalibrationNumXSlices
		{
			get
			{
				return Convert.ToInt16(mtextBox_mass_num_xslices.Text) ; 
			}
			set
			{
				mtextBox_mass_num_xslices.Text = Convert.ToString(value) ; 
			}
		}

		public short MassCalibrationNumMassDeltaBins
		{
			get
			{
				return Convert.ToInt16(mtextBox_num_mass_delta_bins.Text) ; 
			}
			set
			{
				mtextBox_num_mass_delta_bins.Text = Convert.ToString(value) ; 
			}
		}

		public short MassCalibrationMaxJump
		{
			get
			{
				return Convert.ToInt16(mtextBox_mass_jump.Text) ; 
			}
			set
			{
				mtextBox_mass_jump.Text = Convert.ToString(value) ; 
			}
		}

		public double MassCalibrationMaxZScore
		{
			get
			{
				return Convert.ToDouble(mtextBox_outlier_zscore.Text) ; 
			}
			set
			{
				mtextBox_outlier_zscore.Text = Convert.ToString(value) ; 
			}
		}

		public double MassCalibrationLSQZScore
		{
			get
			{
				return Convert.ToDouble(mtextBox_lsq_outlier_zscore.Text) ; 
			}
			set
			{
				mtextBox_lsq_outlier_zscore.Text = Convert.ToString(value) ; 
			}
		}

		public short MassCalibrationLSQNumKnots
		{
			get
			{
				return Convert.ToInt16(mtextBox_num_knots.Text) ; 
			}
			set
			{
				mtextBox_num_knots.Text = Convert.ToString(value) ; 
			}
		}

		public double MassTolerance
		{
			get
			{
				return Convert.ToDouble(mtextBox_mass_tolerance.Text) ; 
			}
			set
			{
				mtextBox_mass_tolerance.Text = Convert.ToString(value) ; 
			}
		}

		public double NETTolerance
		{
			get
			{
				return Convert.ToDouble(mtextBox_NET_tolerance.Text) ; 
			}
			set
			{
				mtextBox_NET_tolerance.Text = Convert.ToString(value) ; 
			}
		}
		
		public bool ApplyMassRecalibration
		{
			get
			{
				return mcheckBox_recalibrate.Checked ; 
			}
			set
			{
				mcheckBox_recalibrate.Checked = value ; 
			}
		}

		public MultiAlignEngine.Alignment.enmCalibrationType RecalibrationType
		{
			get
			{
				if (mradioButton_mz.Checked)
					return MultiAlignEngine.Alignment.enmCalibrationType.MZ_CALIB ; 
				else if (mradioButton_net.Checked)
					return MultiAlignEngine.Alignment.enmCalibrationType.SCAN_CALIB ; 
				else
					return MultiAlignEngine.Alignment.enmCalibrationType.HYBRID_CALIB ;
			}
			set
			{
				if(value == MultiAlignEngine.Alignment.enmCalibrationType.MZ_CALIB)
					mradioButton_mz.Checked = true ; 
				else if (value == MultiAlignEngine.Alignment.enmCalibrationType.SCAN_CALIB)
					mradioButton_net.Checked = true  ; 
				else
					mradioButton_hybrid.Checked = true ; 
			}
		}
        /// <summary>
        /// Gets or sets the options set by the user.
        /// </summary>
		public MultiAlignEngine.Alignment.clsAlignmentOptions AlignmentOptions
		{
			get
			{
				MultiAlignEngine.Alignment.clsAlignmentOptions options = new MultiAlignEngine.Alignment.clsAlignmentOptions() ; 
				options.NumTimeSections                 = NumTimeSections;
				options.ContractionFactor               = ContractionFactor;
				options.MaxTimeJump                     = MaxTimeJump;
				options.MaxPromiscuity                  = MaxPromiscuity;
				options.UsePromiscuousPoints            = UsePromiscuousPoints;
				options.MassCalibrationUseLSQ           = MassCalibrationUseLSQ;
				options.MassCalibrationWindow           = MassCalibrationWindow;
				options.MassCalibrationNumXSlices       = MassCalibrationNumXSlices;
				options.MassCalibrationNumMassDeltaBins = MassCalibrationNumMassDeltaBins;
				options.MassCalibrationMaxJump          = MassCalibrationMaxJump;
				options.MassCalibrationMaxZScore        = MassCalibrationMaxZScore;
				options.MassCalibrationLSQZScore        = MassCalibrationLSQZScore;
				options.MassCalibrationLSQNumKnots      = MassCalibrationLSQNumKnots;
				options.MassTolerance                   = MassTolerance;
				options.NETTolerance                    = NETTolerance;
				options.ApplyMassRecalibration          = ApplyMassRecalibration;
				options.RecalibrationType               = RecalibrationType;
                options.SplitAlignmentInMZ              = SplitAlignment;
                options.MZBoundaries                    = MZSplitBoundary;
                options.NETBinSize                      = NETBinSize;
                options.MassBinSize                     = MassBinSize;
				return options; 
			}
			set
			{
				MultiAlignEngine.Alignment.clsAlignmentOptions  options = value; 
				NumTimeSections                  = options.NumTimeSections; 
				ContractionFactor                = options.ContractionFactor; 
				MaxTimeJump                      = options.MaxTimeJump; 
				MaxPromiscuity                   = options.MaxPromiscuity; 
				UsePromiscuousPoints             = options.UsePromiscuousPoints; 
				MassCalibrationUseLSQ            = options.MassCalibrationUseLSQ; 
				MassCalibrationWindow            = options.MassCalibrationWindow; 
				MassCalibrationNumXSlices        = options.MassCalibrationNumXSlices; 
				MassCalibrationNumMassDeltaBins  = options.MassCalibrationNumMassDeltaBins; 
				MassCalibrationMaxJump           = options.MassCalibrationMaxJump; 
				MassCalibrationMaxZScore         = options.MassCalibrationMaxZScore; 
				MassCalibrationLSQZScore         = options.MassCalibrationLSQZScore; 
				MassCalibrationLSQNumKnots       = options.MassCalibrationLSQNumKnots; 
				MassTolerance                    = options.MassTolerance; 
				NETTolerance                     = options.NETTolerance; 
				ApplyMassRecalibration           = options.ApplyMassRecalibration; 
				RecalibrationType                = options.RecalibrationType;
                MZSplitBoundary                  = options.MZBoundaries;
                SplitAlignment                   = options.SplitAlignmentInMZ;
                NETBinSize                       = options.NETBinSize;
                MassBinSize                      = options.MassBinSize;                
			}
 		}
        /// <summary>
        /// Gets or sets whether to split the alignment.
        /// </summary>
        public bool SplitAlignment
        {
            get
            {
                return mcheckBox_splitAlignment.Checked;
            }
            set
            {
                mcheckBox_splitAlignment.Checked = value;
            }            
        }
        /// <summary>
        /// Gets or sets the alignment m/z boundaries.
        /// </summary>
        public List<classAlignmentMZBoundary> MZSplitBoundary
        {
            get
            {
                double boundary = Convert.ToDouble(mnum_splitAlignment.Value);
                List<classAlignmentMZBoundary> boundaries = new List<classAlignmentMZBoundary>();
                boundaries.Add(new classAlignmentMZBoundary(0.0, boundary));
                boundaries.Add(new classAlignmentMZBoundary(boundary, double.MaxValue));
                return boundaries;
            }
            set
            {
                /// 
                /// Use the high boundary of the first boundary (since we only use one) to
                /// display to the user.
                /// 
                if (value == null)
                    return;

                mnum_splitAlignment.Value = Convert.ToDecimal(value[0].HighBoundary);
            }
        }
		#endregion 

        private void mnum_splitAlignment_ValueChanged(object sender, EventArgs e)
        {
            
        }
	}
}
