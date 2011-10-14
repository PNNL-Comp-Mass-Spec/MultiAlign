using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MultiAlignEngine.Features;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmPeakPicking.
	/// </summary>
	public class frmFeatureFindingParameters : System.Windows.Forms.Form
    {
        #region Members
        private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelAveMass;
		private System.Windows.Forms.Label labelLogAbund;
		private System.Windows.Forms.Label labelMonoMass;
		private System.Windows.Forms.Label labelScan;
		private System.Windows.Forms.Label labelNET;
		private System.Windows.Forms.Label labelFit;
		private System.Windows.Forms.TextBox textBoxMonoMass;
		private System.Windows.Forms.TextBox textBoxAveMass;
		private System.Windows.Forms.TextBox textBoxLogAbund;
		private System.Windows.Forms.TextBox textBoxScan;
		private System.Windows.Forms.TextBox textBoxNET;
		private System.Windows.Forms.TextBox textBoxFit;
		private System.Windows.Forms.Label labelMonoMassCt;
		private System.Windows.Forms.Label labelAveMassCt;
		private System.Windows.Forms.TextBox textBoxMonoMassCt;
		private System.Windows.Forms.TextBox textBoxAveMassCt;
		private System.Windows.Forms.Label labelMMppm;
		private System.Windows.Forms.Label labelAveMppm;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxUseNET;
		private System.Windows.Forms.Label labelMaxDist;
		private System.Windows.Forms.TextBox textBoxMaxDist;
		private System.Windows.Forms.Label labelMinUMCLen;
		private System.Windows.Forms.TextBox textBoxMinUMCLen;
		private System.Windows.Forms.Button mbtnDefaults;
        private TabControl tabControl1;
        private TabPage mtabPage_advanced;
        private TabPage mtabPage_filters;
        private CheckBox mheckBox_isotopicIntensityThreshold;
        private NumericUpDown mnum_isotopicIntensityFilter;
        private CheckBox mcheckBox_isotopicFitFilter;
        private CheckBox mcheckBox_scaleInverted;
        private NumericUpDown mnum_isotopicPeakFitFilter;
        private PictureBox mpicture_filterScoreGlyph;
        private Label mlabel_invertedFilterIndicatorLow;
        private Label label1;
        private Label label2;
        private TabPage mtabPage_generalOptions;
        private Label mlabel_invertedFilterIndicatorHigh;
        private GroupBox mgroupBox_abundanceReporting;
        private RadioButton mradioButton_usePeakMax;
        private RadioButton mradioButton_usePeakArea;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        #endregion

        public frmFeatureFindingParameters()
		{
			InitializeComponent();
		}


        #region Windows Form Designer generated code
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFeatureFindingParameters));
            this.labelMonoMass = new System.Windows.Forms.Label();
            this.labelAveMass = new System.Windows.Forms.Label();
            this.labelLogAbund = new System.Windows.Forms.Label();
            this.labelScan = new System.Windows.Forms.Label();
            this.labelNET = new System.Windows.Forms.Label();
            this.labelFit = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelMonoMassCt = new System.Windows.Forms.Label();
            this.labelMMppm = new System.Windows.Forms.Label();
            this.textBoxMonoMass = new System.Windows.Forms.TextBox();
            this.textBoxAveMass = new System.Windows.Forms.TextBox();
            this.textBoxLogAbund = new System.Windows.Forms.TextBox();
            this.textBoxScan = new System.Windows.Forms.TextBox();
            this.textBoxNET = new System.Windows.Forms.TextBox();
            this.textBoxFit = new System.Windows.Forms.TextBox();
            this.labelAveMassCt = new System.Windows.Forms.Label();
            this.textBoxMonoMassCt = new System.Windows.Forms.TextBox();
            this.textBoxAveMassCt = new System.Windows.Forms.TextBox();
            this.labelAveMppm = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxUseNET = new System.Windows.Forms.CheckBox();
            this.labelMaxDist = new System.Windows.Forms.Label();
            this.textBoxMaxDist = new System.Windows.Forms.TextBox();
            this.labelMinUMCLen = new System.Windows.Forms.Label();
            this.textBoxMinUMCLen = new System.Windows.Forms.TextBox();
            this.mbtnDefaults = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.mtabPage_generalOptions = new System.Windows.Forms.TabPage();
            this.mgroupBox_abundanceReporting = new System.Windows.Forms.GroupBox();
            this.mradioButton_usePeakMax = new System.Windows.Forms.RadioButton();
            this.mradioButton_usePeakArea = new System.Windows.Forms.RadioButton();
            this.mtabPage_filters = new System.Windows.Forms.TabPage();
            this.mlabel_invertedFilterIndicatorHigh = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mlabel_invertedFilterIndicatorLow = new System.Windows.Forms.Label();
            this.mpicture_filterScoreGlyph = new System.Windows.Forms.PictureBox();
            this.mnum_isotopicPeakFitFilter = new System.Windows.Forms.NumericUpDown();
            this.mcheckBox_scaleInverted = new System.Windows.Forms.CheckBox();
            this.mcheckBox_isotopicFitFilter = new System.Windows.Forms.CheckBox();
            this.mnum_isotopicIntensityFilter = new System.Windows.Forms.NumericUpDown();
            this.mheckBox_isotopicIntensityThreshold = new System.Windows.Forms.CheckBox();
            this.mtabPage_advanced = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.mtabPage_generalOptions.SuspendLayout();
            this.mgroupBox_abundanceReporting.SuspendLayout();
            this.mtabPage_filters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_filterScoreGlyph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_isotopicPeakFitFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_isotopicIntensityFilter)).BeginInit();
            this.mtabPage_advanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMonoMass
            // 
            this.labelMonoMass.Location = new System.Drawing.Point(9, 37);
            this.labelMonoMass.Name = "labelMonoMass";
            this.labelMonoMass.Size = new System.Drawing.Size(128, 23);
            this.labelMonoMass.TabIndex = 0;
            this.labelMonoMass.Text = "Monoisotopic Mass :";
            // 
            // labelAveMass
            // 
            this.labelAveMass.Location = new System.Drawing.Point(9, 61);
            this.labelAveMass.Name = "labelAveMass";
            this.labelAveMass.Size = new System.Drawing.Size(100, 23);
            this.labelAveMass.TabIndex = 1;
            this.labelAveMass.Text = "Average Mass :";
            // 
            // labelLogAbund
            // 
            this.labelLogAbund.Location = new System.Drawing.Point(9, 85);
            this.labelLogAbund.Name = "labelLogAbund";
            this.labelLogAbund.Size = new System.Drawing.Size(100, 23);
            this.labelLogAbund.TabIndex = 2;
            this.labelLogAbund.Text = "Log (Abundance) :";
            // 
            // labelScan
            // 
            this.labelScan.Location = new System.Drawing.Point(9, 109);
            this.labelScan.Name = "labelScan";
            this.labelScan.Size = new System.Drawing.Size(100, 23);
            this.labelScan.TabIndex = 3;
            this.labelScan.Text = "Scan :";
            // 
            // labelNET
            // 
            this.labelNET.Location = new System.Drawing.Point(9, 133);
            this.labelNET.Name = "labelNET";
            this.labelNET.Size = new System.Drawing.Size(100, 23);
            this.labelNET.TabIndex = 4;
            this.labelNET.Text = "NET :";
            // 
            // labelFit
            // 
            this.labelFit.Location = new System.Drawing.Point(9, 157);
            this.labelFit.Name = "labelFit";
            this.labelFit.Size = new System.Drawing.Size(100, 23);
            this.labelFit.TabIndex = 5;
            this.labelFit.Text = "Fit :";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(113, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "Weight Factor";
            // 
            // labelMonoMassCt
            // 
            this.labelMonoMassCt.Location = new System.Drawing.Point(193, 37);
            this.labelMonoMassCt.Name = "labelMonoMassCt";
            this.labelMonoMassCt.Size = new System.Drawing.Size(171, 23);
            this.labelMonoMassCt.TabIndex = 7;
            this.labelMonoMassCt.Text = "Mono. Mass Window Constraint";
            // 
            // labelMMppm
            // 
            this.labelMMppm.Location = new System.Drawing.Point(432, 37);
            this.labelMMppm.Name = "labelMMppm";
            this.labelMMppm.Size = new System.Drawing.Size(32, 23);
            this.labelMMppm.TabIndex = 8;
            this.labelMMppm.Text = "ppm";
            // 
            // textBoxMonoMass
            // 
            this.textBoxMonoMass.Location = new System.Drawing.Point(121, 37);
            this.textBoxMonoMass.Name = "textBoxMonoMass";
            this.textBoxMonoMass.Size = new System.Drawing.Size(56, 20);
            this.textBoxMonoMass.TabIndex = 0;
            this.textBoxMonoMass.Text = "0.01";
            this.textBoxMonoMass.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxAveMass
            // 
            this.textBoxAveMass.Location = new System.Drawing.Point(121, 61);
            this.textBoxAveMass.Name = "textBoxAveMass";
            this.textBoxAveMass.Size = new System.Drawing.Size(56, 20);
            this.textBoxAveMass.TabIndex = 1;
            this.textBoxAveMass.Text = "0.01";
            this.textBoxAveMass.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxLogAbund
            // 
            this.textBoxLogAbund.Location = new System.Drawing.Point(121, 85);
            this.textBoxLogAbund.Name = "textBoxLogAbund";
            this.textBoxLogAbund.Size = new System.Drawing.Size(56, 20);
            this.textBoxLogAbund.TabIndex = 2;
            this.textBoxLogAbund.Text = "0.1";
            this.textBoxLogAbund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxScan
            // 
            this.textBoxScan.Location = new System.Drawing.Point(121, 109);
            this.textBoxScan.Name = "textBoxScan";
            this.textBoxScan.Size = new System.Drawing.Size(56, 20);
            this.textBoxScan.TabIndex = 3;
            this.textBoxScan.Text = "0.01";
            this.textBoxScan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxNET
            // 
            this.textBoxNET.Location = new System.Drawing.Point(121, 133);
            this.textBoxNET.Name = "textBoxNET";
            this.textBoxNET.Size = new System.Drawing.Size(56, 20);
            this.textBoxNET.TabIndex = 4;
            this.textBoxNET.Text = "0.1";
            this.textBoxNET.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxFit
            // 
            this.textBoxFit.Location = new System.Drawing.Point(121, 157);
            this.textBoxFit.Name = "textBoxFit";
            this.textBoxFit.Size = new System.Drawing.Size(56, 20);
            this.textBoxFit.TabIndex = 6;
            this.textBoxFit.Text = "0.1";
            this.textBoxFit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelAveMassCt
            // 
            this.labelAveMassCt.Location = new System.Drawing.Point(193, 61);
            this.labelAveMassCt.Name = "labelAveMassCt";
            this.labelAveMassCt.Size = new System.Drawing.Size(187, 23);
            this.labelAveMassCt.TabIndex = 15;
            this.labelAveMassCt.Text = "Average Mass Window Constraint";
            // 
            // textBoxMonoMassCt
            // 
            this.textBoxMonoMassCt.Location = new System.Drawing.Point(370, 35);
            this.textBoxMonoMassCt.Name = "textBoxMonoMassCt";
            this.textBoxMonoMassCt.Size = new System.Drawing.Size(56, 20);
            this.textBoxMonoMassCt.TabIndex = 9;
            this.textBoxMonoMassCt.Text = "6.0";
            this.textBoxMonoMassCt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxAveMassCt
            // 
            this.textBoxAveMassCt.Location = new System.Drawing.Point(370, 61);
            this.textBoxAveMassCt.Name = "textBoxAveMassCt";
            this.textBoxAveMassCt.Size = new System.Drawing.Size(56, 20);
            this.textBoxAveMassCt.TabIndex = 10;
            this.textBoxAveMassCt.Text = "6.0";
            this.textBoxAveMassCt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelAveMppm
            // 
            this.labelAveMppm.Location = new System.Drawing.Point(432, 61);
            this.labelAveMppm.Name = "labelAveMppm";
            this.labelAveMppm.Size = new System.Drawing.Size(32, 23);
            this.labelAveMppm.TabIndex = 18;
            this.labelAveMppm.Text = "ppm";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(196, 305);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 19;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(300, 305);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 20;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // checkBoxUseNET
            // 
            this.checkBoxUseNET.Location = new System.Drawing.Point(193, 133);
            this.checkBoxUseNET.Name = "checkBoxUseNET";
            this.checkBoxUseNET.Size = new System.Drawing.Size(72, 24);
            this.checkBoxUseNET.TabIndex = 5;
            this.checkBoxUseNET.Text = "Use NET";
            this.checkBoxUseNET.CheckedChanged += new System.EventHandler(this.UseNETCheckChanged_event);
            // 
            // labelMaxDist
            // 
            this.labelMaxDist.Location = new System.Drawing.Point(9, 181);
            this.labelMaxDist.Name = "labelMaxDist";
            this.labelMaxDist.Size = new System.Drawing.Size(100, 23);
            this.labelMaxDist.TabIndex = 22;
            this.labelMaxDist.Text = "Max Distance :";
            // 
            // textBoxMaxDist
            // 
            this.textBoxMaxDist.Location = new System.Drawing.Point(121, 181);
            this.textBoxMaxDist.Name = "textBoxMaxDist";
            this.textBoxMaxDist.Size = new System.Drawing.Size(56, 20);
            this.textBoxMaxDist.TabIndex = 7;
            this.textBoxMaxDist.Text = "0.1";
            this.textBoxMaxDist.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelMinUMCLen
            // 
            this.labelMinUMCLen.Location = new System.Drawing.Point(9, 205);
            this.labelMinUMCLen.Name = "labelMinUMCLen";
            this.labelMinUMCLen.Size = new System.Drawing.Size(100, 23);
            this.labelMinUMCLen.TabIndex = 24;
            this.labelMinUMCLen.Text = "Min UMC Length :";
            // 
            // textBoxMinUMCLen
            // 
            this.textBoxMinUMCLen.Location = new System.Drawing.Point(121, 205);
            this.textBoxMinUMCLen.Name = "textBoxMinUMCLen";
            this.textBoxMinUMCLen.Size = new System.Drawing.Size(56, 20);
            this.textBoxMinUMCLen.TabIndex = 8;
            this.textBoxMinUMCLen.Text = "3";
            this.textBoxMinUMCLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // mbtnDefaults
            // 
            this.mbtnDefaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbtnDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnDefaults.Location = new System.Drawing.Point(92, 305);
            this.mbtnDefaults.Name = "mbtnDefaults";
            this.mbtnDefaults.Size = new System.Drawing.Size(80, 24);
            this.mbtnDefaults.TabIndex = 26;
            this.mbtnDefaults.Text = "Use Defaults";
            this.mbtnDefaults.UseVisualStyleBackColor = false;
            this.mbtnDefaults.Click += new System.EventHandler(this.mbtnDefaults_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.mtabPage_generalOptions);
            this.tabControl1.Controls.Add(this.mtabPage_filters);
            this.tabControl1.Controls.Add(this.mtabPage_advanced);
            this.tabControl1.Location = new System.Drawing.Point(1, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(477, 296);
            this.tabControl1.TabIndex = 27;
            // 
            // mtabPage_generalOptions
            // 
            this.mtabPage_generalOptions.Controls.Add(this.mgroupBox_abundanceReporting);
            this.mtabPage_generalOptions.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_generalOptions.Name = "mtabPage_generalOptions";
            this.mtabPage_generalOptions.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_generalOptions.Size = new System.Drawing.Size(469, 270);
            this.mtabPage_generalOptions.TabIndex = 2;
            this.mtabPage_generalOptions.Text = "UMC Finding Options";
            this.mtabPage_generalOptions.UseVisualStyleBackColor = true;
            // 
            // mgroupBox_abundanceReporting
            // 
            this.mgroupBox_abundanceReporting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_abundanceReporting.Controls.Add(this.mradioButton_usePeakMax);
            this.mgroupBox_abundanceReporting.Controls.Add(this.mradioButton_usePeakArea);
            this.mgroupBox_abundanceReporting.Location = new System.Drawing.Point(7, 12);
            this.mgroupBox_abundanceReporting.Name = "mgroupBox_abundanceReporting";
            this.mgroupBox_abundanceReporting.Size = new System.Drawing.Size(456, 81);
            this.mgroupBox_abundanceReporting.TabIndex = 0;
            this.mgroupBox_abundanceReporting.TabStop = false;
            this.mgroupBox_abundanceReporting.Text = "UMC Abundance Reporting";
            // 
            // mradioButton_usePeakMax
            // 
            this.mradioButton_usePeakMax.AutoSize = true;
            this.mradioButton_usePeakMax.Checked = true;
            this.mradioButton_usePeakMax.Location = new System.Drawing.Point(19, 48);
            this.mradioButton_usePeakMax.Name = "mradioButton_usePeakMax";
            this.mradioButton_usePeakMax.Size = new System.Drawing.Size(256, 17);
            this.mradioButton_usePeakMax.TabIndex = 1;
            this.mradioButton_usePeakMax.TabStop = true;
            this.mradioButton_usePeakMax.Text = "Use Elution Profile Max Isotopic Peak [PeakMax]";
            this.mradioButton_usePeakMax.UseVisualStyleBackColor = true;
            // 
            // mradioButton_usePeakArea
            // 
            this.mradioButton_usePeakArea.AutoSize = true;
            this.mradioButton_usePeakArea.Location = new System.Drawing.Point(19, 25);
            this.mradioButton_usePeakArea.Name = "mradioButton_usePeakArea";
            this.mradioButton_usePeakArea.Size = new System.Drawing.Size(192, 17);
            this.mradioButton_usePeakArea.TabIndex = 0;
            this.mradioButton_usePeakArea.Text = "Use Elution Profile Area [PeakArea]";
            this.mradioButton_usePeakArea.UseVisualStyleBackColor = true;
            // 
            // mtabPage_filters
            // 
            this.mtabPage_filters.Controls.Add(this.mlabel_invertedFilterIndicatorHigh);
            this.mtabPage_filters.Controls.Add(this.label2);
            this.mtabPage_filters.Controls.Add(this.label1);
            this.mtabPage_filters.Controls.Add(this.mlabel_invertedFilterIndicatorLow);
            this.mtabPage_filters.Controls.Add(this.mpicture_filterScoreGlyph);
            this.mtabPage_filters.Controls.Add(this.mnum_isotopicPeakFitFilter);
            this.mtabPage_filters.Controls.Add(this.mcheckBox_scaleInverted);
            this.mtabPage_filters.Controls.Add(this.mcheckBox_isotopicFitFilter);
            this.mtabPage_filters.Controls.Add(this.mnum_isotopicIntensityFilter);
            this.mtabPage_filters.Controls.Add(this.mheckBox_isotopicIntensityThreshold);
            this.mtabPage_filters.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_filters.Name = "mtabPage_filters";
            this.mtabPage_filters.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_filters.Size = new System.Drawing.Size(469, 270);
            this.mtabPage_filters.TabIndex = 1;
            this.mtabPage_filters.Text = "Isotopic Filters";
            this.mtabPage_filters.UseVisualStyleBackColor = true;
            // 
            // mlabel_invertedFilterIndicatorHigh
            // 
            this.mlabel_invertedFilterIndicatorHigh.AutoSize = true;
            this.mlabel_invertedFilterIndicatorHigh.Location = new System.Drawing.Point(335, 82);
            this.mlabel_invertedFilterIndicatorHigh.Name = "mlabel_invertedFilterIndicatorHigh";
            this.mlabel_invertedFilterIndicatorHigh.Size = new System.Drawing.Size(26, 13);
            this.mlabel_invertedFilterIndicatorHigh.TabIndex = 12;
            this.mlabel_invertedFilterIndicatorHigh.Text = "Bad";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(335, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "High";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(176, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Low";
            // 
            // mlabel_invertedFilterIndicatorLow
            // 
            this.mlabel_invertedFilterIndicatorLow.AutoSize = true;
            this.mlabel_invertedFilterIndicatorLow.Location = new System.Drawing.Point(176, 82);
            this.mlabel_invertedFilterIndicatorLow.Name = "mlabel_invertedFilterIndicatorLow";
            this.mlabel_invertedFilterIndicatorLow.Size = new System.Drawing.Size(33, 13);
            this.mlabel_invertedFilterIndicatorLow.TabIndex = 9;
            this.mlabel_invertedFilterIndicatorLow.Text = "Good";
            // 
            // mpicture_filterScoreGlyph
            // 
            this.mpicture_filterScoreGlyph.Location = new System.Drawing.Point(178, 98);
            this.mpicture_filterScoreGlyph.Name = "mpicture_filterScoreGlyph";
            this.mpicture_filterScoreGlyph.Size = new System.Drawing.Size(185, 27);
            this.mpicture_filterScoreGlyph.TabIndex = 8;
            this.mpicture_filterScoreGlyph.TabStop = false;
            // 
            // mnum_isotopicPeakFitFilter
            // 
            this.mnum_isotopicPeakFitFilter.DecimalPlaces = 2;
            this.mnum_isotopicPeakFitFilter.Enabled = false;
            this.mnum_isotopicPeakFitFilter.Location = new System.Drawing.Point(179, 52);
            this.mnum_isotopicPeakFitFilter.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.mnum_isotopicPeakFitFilter.Name = "mnum_isotopicPeakFitFilter";
            this.mnum_isotopicPeakFitFilter.Size = new System.Drawing.Size(75, 20);
            this.mnum_isotopicPeakFitFilter.TabIndex = 3;
            this.mnum_isotopicPeakFitFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mcheckBox_scaleInverted
            // 
            this.mcheckBox_scaleInverted.AutoSize = true;
            this.mcheckBox_scaleInverted.Location = new System.Drawing.Point(269, 55);
            this.mcheckBox_scaleInverted.Name = "mcheckBox_scaleInverted";
            this.mcheckBox_scaleInverted.Size = new System.Drawing.Size(95, 17);
            this.mcheckBox_scaleInverted.TabIndex = 4;
            this.mcheckBox_scaleInverted.Text = "Scale Inverted";
            this.mcheckBox_scaleInverted.UseVisualStyleBackColor = true;
            this.mcheckBox_scaleInverted.CheckedChanged += new System.EventHandler(this.mcheckBox_scaleInverted_CheckedChanged);
            // 
            // mcheckBox_isotopicFitFilter
            // 
            this.mcheckBox_isotopicFitFilter.AutoSize = true;
            this.mcheckBox_isotopicFitFilter.Location = new System.Drawing.Point(7, 52);
            this.mcheckBox_isotopicFitFilter.Name = "mcheckBox_isotopicFitFilter";
            this.mcheckBox_isotopicFitFilter.Size = new System.Drawing.Size(130, 17);
            this.mcheckBox_isotopicFitFilter.TabIndex = 2;
            this.mcheckBox_isotopicFitFilter.Text = "Filter Isotopic Peak Fit";
            this.mcheckBox_isotopicFitFilter.UseVisualStyleBackColor = true;
            this.mcheckBox_isotopicFitFilter.CheckedChanged += new System.EventHandler(this.mcheckBox_isotopicFitFilter_CheckedChanged);
            // 
            // mnum_isotopicIntensityFilter
            // 
            this.mnum_isotopicIntensityFilter.Enabled = false;
            this.mnum_isotopicIntensityFilter.Location = new System.Drawing.Point(179, 17);
            this.mnum_isotopicIntensityFilter.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.mnum_isotopicIntensityFilter.Name = "mnum_isotopicIntensityFilter";
            this.mnum_isotopicIntensityFilter.Size = new System.Drawing.Size(75, 20);
            this.mnum_isotopicIntensityFilter.TabIndex = 1;
            this.mnum_isotopicIntensityFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mheckBox_isotopicIntensityThreshold
            // 
            this.mheckBox_isotopicIntensityThreshold.AutoSize = true;
            this.mheckBox_isotopicIntensityThreshold.Location = new System.Drawing.Point(7, 17);
            this.mheckBox_isotopicIntensityThreshold.Name = "mheckBox_isotopicIntensityThreshold";
            this.mheckBox_isotopicIntensityThreshold.Size = new System.Drawing.Size(166, 17);
            this.mheckBox_isotopicIntensityThreshold.TabIndex = 0;
            this.mheckBox_isotopicIntensityThreshold.Text = "Filter Isotopic Peak Intensities";
            this.mheckBox_isotopicIntensityThreshold.UseVisualStyleBackColor = true;
            this.mheckBox_isotopicIntensityThreshold.CheckedChanged += new System.EventHandler(this.mheckBox_isotopicIntensityThreshold_CheckedChanged);
            // 
            // mtabPage_advanced
            // 
            this.mtabPage_advanced.Controls.Add(this.label7);
            this.mtabPage_advanced.Controls.Add(this.textBoxMinUMCLen);
            this.mtabPage_advanced.Controls.Add(this.labelAveMass);
            this.mtabPage_advanced.Controls.Add(this.labelMinUMCLen);
            this.mtabPage_advanced.Controls.Add(this.labelLogAbund);
            this.mtabPage_advanced.Controls.Add(this.textBoxMaxDist);
            this.mtabPage_advanced.Controls.Add(this.labelScan);
            this.mtabPage_advanced.Controls.Add(this.labelMaxDist);
            this.mtabPage_advanced.Controls.Add(this.labelNET);
            this.mtabPage_advanced.Controls.Add(this.checkBoxUseNET);
            this.mtabPage_advanced.Controls.Add(this.labelFit);
            this.mtabPage_advanced.Controls.Add(this.labelMonoMassCt);
            this.mtabPage_advanced.Controls.Add(this.labelMMppm);
            this.mtabPage_advanced.Controls.Add(this.labelAveMppm);
            this.mtabPage_advanced.Controls.Add(this.textBoxMonoMass);
            this.mtabPage_advanced.Controls.Add(this.textBoxAveMassCt);
            this.mtabPage_advanced.Controls.Add(this.textBoxAveMass);
            this.mtabPage_advanced.Controls.Add(this.textBoxMonoMassCt);
            this.mtabPage_advanced.Controls.Add(this.textBoxLogAbund);
            this.mtabPage_advanced.Controls.Add(this.labelAveMassCt);
            this.mtabPage_advanced.Controls.Add(this.textBoxScan);
            this.mtabPage_advanced.Controls.Add(this.textBoxFit);
            this.mtabPage_advanced.Controls.Add(this.textBoxNET);
            this.mtabPage_advanced.Controls.Add(this.labelMonoMass);
            this.mtabPage_advanced.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_advanced.Name = "mtabPage_advanced";
            this.mtabPage_advanced.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_advanced.Size = new System.Drawing.Size(469, 270);
            this.mtabPage_advanced.TabIndex = 0;
            this.mtabPage_advanced.Text = "Advanced Options";
            this.mtabPage_advanced.UseVisualStyleBackColor = true;
            // 
            // frmFeatureFindingParameters
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(490, 342);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.mbtnDefaults);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFeatureFindingParameters";
            this.Text = "Feature Finding Parameters";
            this.tabControl1.ResumeLayout(false);
            this.mtabPage_generalOptions.ResumeLayout(false);
            this.mgroupBox_abundanceReporting.ResumeLayout(false);
            this.mgroupBox_abundanceReporting.PerformLayout();
            this.mtabPage_filters.ResumeLayout(false);
            this.mtabPage_filters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_filterScoreGlyph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_isotopicPeakFitFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_isotopicIntensityFilter)).EndInit();
            this.mtabPage_advanced.ResumeLayout(false);
            this.mtabPage_advanced.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        #region Settings Methods
        private void SaveOptions()
        {
            Properties.Settings.Default.UserPeakPickingMonoMass                 = MonoMassWeight;
            Properties.Settings.Default.UserPeakPickingAverageMass              = AveMassWeight;
            Properties.Settings.Default.UserPeakPickingLogAbundance             = LogAbundanceWeight;
            Properties.Settings.Default.UserPeakPickingScan                     = ScanWeight;
            Properties.Settings.Default.UserPeakPickingNet                      = NETWeight;
            Properties.Settings.Default.UserPeakPickingUseNet                   = UseNET;
            Properties.Settings.Default.UserPeakPickingFit                      = FitWeight;
            Properties.Settings.Default.UserPeakPickingMaxDist                  = MaxDistance;
            Properties.Settings.Default.UserPeakPickingMinDist                  = MinUMCLength;
            Properties.Settings.Default.UserPeakPickingMonoMassConstraintPPM    = ConstraintMonoMass;
            Properties.Settings.Default.UserPeakPickingAverageMassConstraintPPM = ConstraintAveMass;

            Properties.Settings.Default.UserPeakPickingUseFitFilter             = this.UseIsotopicFitFilter;
            Properties.Settings.Default.UserPeakPickingUseIntensity             = this.UseIsotopicIntensityFilter;
            Properties.Settings.Default.UserPeakPickingIntensityFilter          = this.IsotopicItensityFilter;
            Properties.Settings.Default.UserPeakPickingFitFilter                = this.IsotopicFitFilter;
            Properties.Settings.Default.UserPeakPickingInvertFitFilter          = this.IsIsotopicFitFilterInverted;
            Properties.Settings.Default.UserPeakPickingUMCReportingTypePeakArea = (this.UMCAbundanceReportingType == enmAbundanceReportingType.PeakArea);
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Properties
        public float MonoMassWeight
		{
			get
			{
				return Convert.ToSingle(textBoxMonoMass.Text) ; 
			}
			set
			{
				textBoxMonoMass.Text = Convert.ToString(value) ;
			}
		}

		public float AveMassWeight
		{
			get
			{
				return Convert.ToSingle(textBoxAveMass.Text) ; 
			}
			set
			{
				textBoxAveMass.Text = Convert.ToString(value) ;
			}
		}

		public float LogAbundanceWeight
		{
			get
			{
				return Convert.ToSingle(textBoxLogAbund.Text) ; 
			}
			set
			{
				textBoxLogAbund.Text = Convert.ToString(value) ;
			}
		}

		public float ScanWeight
		{
			get
			{
				return Convert.ToSingle(textBoxScan.Text) ; 
			}
			set
			{
				textBoxScan.Text = Convert.ToString(value) ;
			}
		}

		public float FitWeight
		{
			get
			{
				return Convert.ToSingle(textBoxFit.Text) ; 
			}
			set
			{
				textBoxFit.Text = Convert.ToString(value) ;
			}
		}

		public float NETWeight
		{
			get
			{
				return Convert.ToSingle(textBoxNET.Text) ; 
			}
			set
			{
				textBoxNET.Text = Convert.ToString(value) ;
			}
		}


		public float ConstraintMonoMass
		{
			get
			{
				return Convert.ToSingle(textBoxMonoMassCt.Text) ; 
			}
			set
			{
				textBoxMonoMassCt.Text = Convert.ToString(value) ;
			}
		}
		public float ConstraintAveMass
		{
			get
			{
				return Convert.ToSingle(textBoxAveMassCt.Text) ; 
			}
			set
			{
				textBoxAveMassCt.Text = Convert.ToString(value) ;
			}
		}


		public double MaxDistance
		{
			get
			{
				return Convert.ToDouble(textBoxMaxDist.Text) ; 
			}
			set
			{
				textBoxMaxDist.Text = Convert.ToString(value) ;
			}
		}
		public bool UseNET
		{
			get
			{
				return checkBoxUseNET.Checked ; 
			}
			set
			{
				checkBoxUseNET.Checked = value ;
			}
		}

		public int MinUMCLength
		{
			get
			{
				return Convert.ToInt16(textBoxMinUMCLen.Text) ;
			}
			set
			{
				textBoxMinUMCLen.Text = Convert.ToString(value) ;
			}
		}

        /// <summary>
        /// Gets or sets the intensity filter for isotopic peaks.
        /// </summary>
        public int IsotopicItensityFilter
        {
            get
            {
                return Convert.ToInt32(mnum_isotopicIntensityFilter.Value);
            }
            set
            {
                mnum_isotopicIntensityFilter.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the isotopic fit filter inverted flag.
        /// </summary>
        public bool IsIsotopicFitFilterInverted
        {
            get
            {
                return mcheckBox_scaleInverted.Checked;
            }
            set
            {
                mcheckBox_scaleInverted.Checked = value;
                SetFilterGlyph();
            }
        }
        /// <summary>
        /// Gets or sets the isotopic fit filter value.
        /// </summary>
        public double IsotopicFitFilter
        {
            get
            {
                return Convert.ToDouble(mnum_isotopicPeakFitFilter.Value);
            }
            set
            {
                mnum_isotopicPeakFitFilter.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets whether to use the isotopic fit filter value.
        /// </summary>
        public bool UseIsotopicFitFilter
        {
            get
            {
                return mcheckBox_isotopicFitFilter.Checked;
            }
            set
            {
                mcheckBox_isotopicFitFilter.Checked = value;
            }
        }
        /// <summary>
        /// Gets or sets whether to use the Isotopic Intensity Filter.
        /// </summary>
        public bool UseIsotopicIntensityFilter
        {
            get
            {
                return mheckBox_isotopicIntensityThreshold.Checked;
            }
            set
            {
                mheckBox_isotopicIntensityThreshold.Checked = value;
            }
        }
        /// <summary>
        /// Gets or sets whether the reporting type for UMC's is done through elution profile area or elution profile peak max.
        /// </summary>
        public enmAbundanceReportingType UMCAbundanceReportingType
        {
            get
            {                
                enmAbundanceReportingType reporting = enmAbundanceReportingType.PeakArea;
                if (mradioButton_usePeakArea.Checked == false)
                    reporting = enmAbundanceReportingType.PeakMax;

                return reporting;
            }
            set
            {
                if (value == enmAbundanceReportingType.PeakMax)
                {
                    mradioButton_usePeakMax.Checked = true;
                    mradioButton_usePeakArea.Checked = false;
                }
                else
                {
                    mradioButton_usePeakMax.Checked = false;
                    mradioButton_usePeakArea.Checked = true;
                }
            }
        }
        /// <summary>
        /// Gets or sets the UMC Finding object that contains all settings contained herein.
        /// </summary>
        public MultiAlignCore.Algorithms.FeatureFinding.UMCFeatureFinderOptions UMCFindingOptions
		{
			get
			{
                MultiAlignCore.Algorithms.FeatureFinding.UMCFeatureFinderOptions options = new MultiAlignCore.Algorithms.FeatureFinding.UMCFeatureFinderOptions();
				options.MonoMassWeight      = MonoMassWeight ;
				options.AveMassWeight       = AveMassWeight ;
				options.LogAbundanceWeight  = LogAbundanceWeight ;
				options.ScanWeight          = ScanWeight ;
				options.FitWeight           = FitWeight ;
				options.NETWeight           = NETWeight ;
				options.ConstraintMonoMass  = ConstraintMonoMass ;
				options.ConstraintAveMass   = ConstraintAveMass ;
				options.MaxDistance         = MaxDistance ;
				options.UseNET              = UseNET ;
				options.MinUMCLength        = MinUMCLength ;

                options.IsIsotopicFitFilterInverted = IsIsotopicFitFilterInverted;
                options.IsotopicFitFilter           = IsotopicFitFilter;
                options.UseIsotopicFitFilter        = UseIsotopicFitFilter;
                options.IsotopicIntensityFilter     = IsotopicItensityFilter;
                options.UseIsotopicIntensityFilter  = UseIsotopicIntensityFilter;
                options.UMCAbundanceReportingType   = UMCAbundanceReportingType;

				return options;
			}
			set
			{
                MultiAlignCore.Algorithms.FeatureFinding.UMCFeatureFinderOptions options = value;
				MonoMassWeight      = options.MonoMassWeight ;
				AveMassWeight       = options.AveMassWeight ;
				LogAbundanceWeight  = options.LogAbundanceWeight ;
				ScanWeight          = options.ScanWeight ;
				FitWeight           = options.FitWeight ;
				NETWeight           = options.NETWeight ;
				ConstraintMonoMass  = options.ConstraintMonoMass ;
				ConstraintAveMass   = options.ConstraintAveMass ;
				MaxDistance         = options.MaxDistance ;
				UseNET              = options.UseNET ;
				MinUMCLength        = options.MinUMCLength ;
                UMCAbundanceReportingType = options.UMCAbundanceReportingType;

                IsIsotopicFitFilterInverted = options.IsIsotopicFitFilterInverted;                    
                IsotopicFitFilter           = options.IsotopicFitFilter;
                UseIsotopicFitFilter        = options.UseIsotopicFitFilter;
                IsotopicItensityFilter      = options.IsotopicIntensityFilter;
                UseIsotopicIntensityFilter  = options.UseIsotopicIntensityFilter;
			}
		}
		#endregion

        #region Event Handlers
        private void UseNETCheckChanged_event(object sender, System.EventArgs e)
        {
            if (checkBoxUseNET.Checked)
            {
                labelScan.Enabled = false;
                textBoxScan.Enabled = false;
            }
            else
            {
                labelScan.Enabled = true;
                textBoxScan.Enabled = true;
            }
        }
        private void mbutton_ok_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SaveOptions();
            this.Hide();
        }

        private void mbutton_cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void mbtnDefaults_Click(object sender, System.EventArgs e)
        {
            MultiAlignCore.Algorithms.FeatureFinding.UMCFeatureFinderOptions defaults =
                new MultiAlignCore.Algorithms.FeatureFinding.UMCFeatureFinderOptions();
            UMCFindingOptions = defaults;
        }
        private void mcheckBox_isotopicFitFilter_CheckedChanged(object sender, EventArgs e)
        {
            mnum_isotopicPeakFitFilter.Enabled = mcheckBox_isotopicFitFilter.Checked;
        }
        private void mheckBox_isotopicIntensityThreshold_CheckedChanged(object sender, EventArgs e)
        {
            mnum_isotopicIntensityFilter.Enabled = mheckBox_isotopicIntensityThreshold.Checked;
        }

        private void mcheckBox_scaleInverted_CheckedChanged(object sender, EventArgs e)
        {
            SetFilterGlyph();
        }
        #endregion

        private void SetFilterGlyph()
        {
            if (mcheckBox_scaleInverted.Checked == true)
            {
                mlabel_invertedFilterIndicatorLow.Text = "Bad";
                mlabel_invertedFilterIndicatorHigh.Text = "Good";
                mpicture_filterScoreGlyph.Image = global::MultiAlignWin.Properties.Resources.intensityFilterHighGood;
            }
            else
            {
                mlabel_invertedFilterIndicatorLow.Text = "Good";
                mlabel_invertedFilterIndicatorHigh.Text = "Bad";
                mpicture_filterScoreGlyph.Image = global::MultiAlignWin.Properties.Resources.intensityFilterHighBad;            
            }
        }  
    }
}
