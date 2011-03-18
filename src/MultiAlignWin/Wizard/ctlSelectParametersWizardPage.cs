using System;
using System.ComponentModel;
using System.Windows.Forms;
using MultiAlignWin.Forms.Parameters;
using PNNLProteomics.Data;


namespace MultiAlignWin
{
    public class ctlSelectParametersWizardPage : UserControl,  MultiAlignWin.Forms.Wizard.IWizardControl<PNNLProteomics.Data.MultiAlignAnalysis>
    {
        #region Members
        private Button      m_peakPickingParametersButton;
        private Button      mbtnAlignmentParameters;
        private Button      peakMatchingButton;
        private Button      buttonLoadParametersFromFile;
        private Button      mbutton_loadMassTagDatabasePeaks;
        private Button      smartParametersButton;
        private Button      mbutton_clustering;
        private IContainer  components = null;
        private ComboBox    mcomboBox_baseline;
        private GroupBox    mgroupBox_parameters;
        private GroupBox    mgroupBox_alignment;
        private GroupBox    m_peptideIdentificationGroupBox;
        private RadioButton mradio_alignToMTDB;
        private RadioButton mradio_alignToFile;
        private RadioButton mradioButton_useStandardPeakMatching;
        private RadioButton mradioButton_useSMART;   
        private Label       mlabel_peakMatchingDatabase;
        private Label       labelLoadParam;
        private Label       m_databaseSelected;
        private bool m_updatingRadioButtons = false;

        /// <summary>
        /// Analysis object that we are currently building.
        /// </summary>
        private MultiAlignAnalysis m_analysis;
        private Button mbtnSavePara;
        private Label label1;
        /// <summary>
        /// Holds information on how to connect to DMS.
        /// </summary>
        private clsDMSServerInformation m_serverInformation;
        #endregion

        /// <summary>
        /// Default constructor for a parameter wizard page.
        /// </summary>
        public ctlSelectParametersWizardPage()
        {
            InitializeComponent();

            buttonLoadParametersFromFile.Click      += new EventHandler(buttonLoadParametersFromFile_Click);
            mbutton_loadMassTagDatabasePeaks.Click  += new EventHandler(mbutton_loadMassTagDatabasePeaks_Click);
        }


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

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_peakPickingParametersButton = new System.Windows.Forms.Button();
            this.mbtnAlignmentParameters = new System.Windows.Forms.Button();
            this.peakMatchingButton = new System.Windows.Forms.Button();
            this.buttonLoadParametersFromFile = new System.Windows.Forms.Button();
            this.mcomboBox_baseline = new System.Windows.Forms.ComboBox();
            this.mgroupBox_parameters = new System.Windows.Forms.GroupBox();
            this.mbutton_clustering = new System.Windows.Forms.Button();
            this.m_peptideIdentificationGroupBox = new System.Windows.Forms.GroupBox();
            this.mradioButton_useSMART = new System.Windows.Forms.RadioButton();
            this.mradioButton_useStandardPeakMatching = new System.Windows.Forms.RadioButton();
            this.smartParametersButton = new System.Windows.Forms.Button();
            this.labelLoadParam = new System.Windows.Forms.Label();
            this.mbutton_loadMassTagDatabasePeaks = new System.Windows.Forms.Button();
            this.mlabel_peakMatchingDatabase = new System.Windows.Forms.Label();
            this.mgroupBox_alignment = new System.Windows.Forms.GroupBox();
            this.m_databaseSelected = new System.Windows.Forms.Label();
            this.mradio_alignToMTDB = new System.Windows.Forms.RadioButton();
            this.mradio_alignToFile = new System.Windows.Forms.RadioButton();
            this.mbtnSavePara = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mgroupBox_parameters.SuspendLayout();
            this.m_peptideIdentificationGroupBox.SuspendLayout();
            this.mgroupBox_alignment.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_peakPickingParametersButton
            // 
            this.m_peakPickingParametersButton.BackColor = System.Drawing.Color.White;
            this.m_peakPickingParametersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_peakPickingParametersButton.Image = global::MultiAlignWin.Properties.Resources.featuresGlyph;
            this.m_peakPickingParametersButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_peakPickingParametersButton.Location = new System.Drawing.Point(19, 73);
            this.m_peakPickingParametersButton.Name = "m_peakPickingParametersButton";
            this.m_peakPickingParametersButton.Size = new System.Drawing.Size(183, 31);
            this.m_peakPickingParametersButton.TabIndex = 2;
            this.m_peakPickingParametersButton.Text = "LCMS Feature Finding ";
            this.m_peakPickingParametersButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_peakPickingParametersButton.UseVisualStyleBackColor = false;
            this.m_peakPickingParametersButton.Click += new System.EventHandler(this.FeatureFinding_Click);
            // 
            // mbtnAlignmentParameters
            // 
            this.mbtnAlignmentParameters.BackColor = System.Drawing.Color.White;
            this.mbtnAlignmentParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbtnAlignmentParameters.Image = global::MultiAlignWin.Properties.Resources.alignmentGlyph;
            this.mbtnAlignmentParameters.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbtnAlignmentParameters.Location = new System.Drawing.Point(19, 110);
            this.mbtnAlignmentParameters.Name = "mbtnAlignmentParameters";
            this.mbtnAlignmentParameters.Size = new System.Drawing.Size(183, 31);
            this.mbtnAlignmentParameters.TabIndex = 2;
            this.mbtnAlignmentParameters.Text = "Alignment ";
            this.mbtnAlignmentParameters.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbtnAlignmentParameters.UseVisualStyleBackColor = false;
            this.mbtnAlignmentParameters.Click += new System.EventHandler(this.Alignment_Click);
            // 
            // peakMatchingButton
            // 
            this.peakMatchingButton.BackColor = System.Drawing.Color.White;
            this.peakMatchingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peakMatchingButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.peakMatchingButton.Location = new System.Drawing.Point(192, 19);
            this.peakMatchingButton.Name = "peakMatchingButton";
            this.peakMatchingButton.Size = new System.Drawing.Size(95, 31);
            this.peakMatchingButton.TabIndex = 2;
            this.peakMatchingButton.Text = "Peak Matching";
            this.peakMatchingButton.UseVisualStyleBackColor = false;
            this.peakMatchingButton.Click += new System.EventHandler(this.PeakMatching_Click);
            // 
            // buttonLoadParametersFromFile
            // 
            this.buttonLoadParametersFromFile.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLoadParametersFromFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLoadParametersFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoadParametersFromFile.Location = new System.Drawing.Point(178, 22);
            this.buttonLoadParametersFromFile.Name = "buttonLoadParametersFromFile";
            this.buttonLoadParametersFromFile.Size = new System.Drawing.Size(40, 24);
            this.buttonLoadParametersFromFile.TabIndex = 10;
            this.buttonLoadParametersFromFile.Text = "...";
            this.buttonLoadParametersFromFile.UseVisualStyleBackColor = false;
            // 
            // mcomboBox_baseline
            // 
            this.mcomboBox_baseline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_baseline.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_baseline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcomboBox_baseline.Location = new System.Drawing.Point(43, 44);
            this.mcomboBox_baseline.Name = "mcomboBox_baseline";
            this.mcomboBox_baseline.Size = new System.Drawing.Size(752, 21);
            this.mcomboBox_baseline.TabIndex = 13;
            this.mcomboBox_baseline.Text = "Select Baseline Dataset for Alignment";
            this.mcomboBox_baseline.SelectedIndexChanged += new System.EventHandler(this.mcomboBox_baseline_SelectedIndexChanged);
            // 
            // mgroupBox_parameters
            // 
            this.mgroupBox_parameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_parameters.Controls.Add(this.mbtnSavePara);
            this.mgroupBox_parameters.Controls.Add(this.label1);
            this.mgroupBox_parameters.Controls.Add(this.mbutton_clustering);
            this.mgroupBox_parameters.Controls.Add(this.m_peptideIdentificationGroupBox);
            this.mgroupBox_parameters.Controls.Add(this.buttonLoadParametersFromFile);
            this.mgroupBox_parameters.Controls.Add(this.labelLoadParam);
            this.mgroupBox_parameters.Controls.Add(this.mbutton_loadMassTagDatabasePeaks);
            this.mgroupBox_parameters.Controls.Add(this.m_peakPickingParametersButton);
            this.mgroupBox_parameters.Controls.Add(this.mlabel_peakMatchingDatabase);
            this.mgroupBox_parameters.Controls.Add(this.mbtnAlignmentParameters);
            this.mgroupBox_parameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.mgroupBox_parameters.Location = new System.Drawing.Point(8, 11);
            this.mgroupBox_parameters.Name = "mgroupBox_parameters";
            this.mgroupBox_parameters.Size = new System.Drawing.Size(812, 349);
            this.mgroupBox_parameters.TabIndex = 16;
            this.mgroupBox_parameters.TabStop = false;
            this.mgroupBox_parameters.Text = "Set Parameters";
            // 
            // mbutton_clustering
            // 
            this.mbutton_clustering.BackColor = System.Drawing.Color.White;
            this.mbutton_clustering.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clustering.Image = global::MultiAlignWin.Properties.Resources.clusteringGlyph;
            this.mbutton_clustering.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_clustering.Location = new System.Drawing.Point(19, 184);
            this.mbutton_clustering.Name = "mbutton_clustering";
            this.mbutton_clustering.Size = new System.Drawing.Size(183, 31);
            this.mbutton_clustering.TabIndex = 27;
            this.mbutton_clustering.Text = "Clustering ";
            this.mbutton_clustering.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_clustering.UseVisualStyleBackColor = false;
            this.mbutton_clustering.Click += new System.EventHandler(this.FeatureClustering_Click);
            // 
            // m_peptideIdentificationGroupBox
            // 
            this.m_peptideIdentificationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_peptideIdentificationGroupBox.Controls.Add(this.peakMatchingButton);
            this.m_peptideIdentificationGroupBox.Controls.Add(this.mradioButton_useSMART);
            this.m_peptideIdentificationGroupBox.Controls.Add(this.mradioButton_useStandardPeakMatching);
            this.m_peptideIdentificationGroupBox.Controls.Add(this.smartParametersButton);
            this.m_peptideIdentificationGroupBox.Enabled = false;
            this.m_peptideIdentificationGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_peptideIdentificationGroupBox.Location = new System.Drawing.Point(19, 238);
            this.m_peptideIdentificationGroupBox.Name = "m_peptideIdentificationGroupBox";
            this.m_peptideIdentificationGroupBox.Size = new System.Drawing.Size(787, 99);
            this.m_peptideIdentificationGroupBox.TabIndex = 26;
            this.m_peptideIdentificationGroupBox.TabStop = false;
            this.m_peptideIdentificationGroupBox.Text = "Peptide Identification";
            // 
            // mradioButton_useSMART
            // 
            this.mradioButton_useSMART.AutoSize = true;
            this.mradioButton_useSMART.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradioButton_useSMART.Location = new System.Drawing.Point(7, 63);
            this.mradioButton_useSMART.Name = "mradioButton_useSMART";
            this.mradioButton_useSMART.Size = new System.Drawing.Size(75, 17);
            this.mradioButton_useSMART.TabIndex = 24;
            this.mradioButton_useSMART.Text = "Use STAC";
            this.mradioButton_useSMART.UseVisualStyleBackColor = true;
            this.mradioButton_useSMART.CheckedChanged += new System.EventHandler(this.mradioButton_useSMART_CheckedChanged);
            // 
            // mradioButton_useStandardPeakMatching
            // 
            this.mradioButton_useStandardPeakMatching.AutoSize = true;
            this.mradioButton_useStandardPeakMatching.Checked = true;
            this.mradioButton_useStandardPeakMatching.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradioButton_useStandardPeakMatching.Location = new System.Drawing.Point(7, 26);
            this.mradioButton_useStandardPeakMatching.Name = "mradioButton_useStandardPeakMatching";
            this.mradioButton_useStandardPeakMatching.Size = new System.Drawing.Size(165, 17);
            this.mradioButton_useStandardPeakMatching.TabIndex = 25;
            this.mradioButton_useStandardPeakMatching.TabStop = true;
            this.mradioButton_useStandardPeakMatching.Text = "Use Standard Peak Matching";
            this.mradioButton_useStandardPeakMatching.UseVisualStyleBackColor = true;
            // 
            // smartParametersButton
            // 
            this.smartParametersButton.BackColor = System.Drawing.Color.White;
            this.smartParametersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.smartParametersButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.smartParametersButton.Location = new System.Drawing.Point(192, 56);
            this.smartParametersButton.Name = "smartParametersButton";
            this.smartParametersButton.Size = new System.Drawing.Size(95, 30);
            this.smartParametersButton.TabIndex = 23;
            this.smartParametersButton.Text = "STAC";
            this.smartParametersButton.UseVisualStyleBackColor = false;
            this.smartParametersButton.Click += new System.EventHandler(this.mbutton_scoring_Click);
            // 
            // labelLoadParam
            // 
            this.labelLoadParam.Enabled = false;
            this.labelLoadParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLoadParam.Location = new System.Drawing.Point(23, 28);
            this.labelLoadParam.Name = "labelLoadParam";
            this.labelLoadParam.Size = new System.Drawing.Size(152, 23);
            this.labelLoadParam.TabIndex = 8;
            this.labelLoadParam.Text = "Load Parameters from a File:";
            // 
            // mbutton_loadMassTagDatabasePeaks
            // 
            this.mbutton_loadMassTagDatabasePeaks.BackColor = System.Drawing.Color.White;
            this.mbutton_loadMassTagDatabasePeaks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_loadMassTagDatabasePeaks.Image = global::MultiAlignWin.Properties.Resources.databaseGlyph;
            this.mbutton_loadMassTagDatabasePeaks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_loadMassTagDatabasePeaks.Location = new System.Drawing.Point(19, 147);
            this.mbutton_loadMassTagDatabasePeaks.Name = "mbutton_loadMassTagDatabasePeaks";
            this.mbutton_loadMassTagDatabasePeaks.Size = new System.Drawing.Size(183, 31);
            this.mbutton_loadMassTagDatabasePeaks.TabIndex = 21;
            this.mbutton_loadMassTagDatabasePeaks.Text = "Mass Tag Database";
            this.mbutton_loadMassTagDatabasePeaks.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_loadMassTagDatabasePeaks.UseVisualStyleBackColor = false;
            // 
            // mlabel_peakMatchingDatabase
            // 
            this.mlabel_peakMatchingDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_peakMatchingDatabase.AutoEllipsis = true;
            this.mlabel_peakMatchingDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_peakMatchingDatabase.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.mlabel_peakMatchingDatabase.Location = new System.Drawing.Point(208, 151);
            this.mlabel_peakMatchingDatabase.Name = "mlabel_peakMatchingDatabase";
            this.mlabel_peakMatchingDatabase.Size = new System.Drawing.Size(587, 20);
            this.mlabel_peakMatchingDatabase.TabIndex = 22;
            this.mlabel_peakMatchingDatabase.Text = "No Database Selected";
            this.mlabel_peakMatchingDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mgroupBox_alignment
            // 
            this.mgroupBox_alignment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_alignment.Controls.Add(this.m_databaseSelected);
            this.mgroupBox_alignment.Controls.Add(this.mradio_alignToMTDB);
            this.mgroupBox_alignment.Controls.Add(this.mradio_alignToFile);
            this.mgroupBox_alignment.Controls.Add(this.mcomboBox_baseline);
            this.mgroupBox_alignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.mgroupBox_alignment.Location = new System.Drawing.Point(8, 366);
            this.mgroupBox_alignment.Name = "mgroupBox_alignment";
            this.mgroupBox_alignment.Size = new System.Drawing.Size(812, 159);
            this.mgroupBox_alignment.TabIndex = 17;
            this.mgroupBox_alignment.TabStop = false;
            this.mgroupBox_alignment.Text = "Select Baseline";
            // 
            // m_databaseSelected
            // 
            this.m_databaseSelected.AutoSize = true;
            this.m_databaseSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_databaseSelected.ForeColor = System.Drawing.Color.Red;
            this.m_databaseSelected.Location = new System.Drawing.Point(40, 91);
            this.m_databaseSelected.Name = "m_databaseSelected";
            this.m_databaseSelected.Size = new System.Drawing.Size(144, 13);
            this.m_databaseSelected.TabIndex = 16;
            this.m_databaseSelected.Text = "No database is selected";
            this.m_databaseSelected.Visible = false;
            // 
            // mradio_alignToMTDB
            // 
            this.mradio_alignToMTDB.AutoSize = true;
            this.mradio_alignToMTDB.Enabled = false;
            this.mradio_alignToMTDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradio_alignToMTDB.Location = new System.Drawing.Point(19, 71);
            this.mradio_alignToMTDB.Name = "mradio_alignToMTDB";
            this.mradio_alignToMTDB.Size = new System.Drawing.Size(199, 17);
            this.mradio_alignToMTDB.TabIndex = 15;
            this.mradio_alignToMTDB.TabStop = true;
            this.mradio_alignToMTDB.Text = "Align to Mass Tag Database (MTDB)";
            this.mradio_alignToMTDB.UseVisualStyleBackColor = true;
            this.mradio_alignToMTDB.CheckedChanged += new System.EventHandler(this.mradio_alignToMTDB_CheckedChanged);
            // 
            // mradio_alignToFile
            // 
            this.mradio_alignToFile.AutoSize = true;
            this.mradio_alignToFile.Checked = true;
            this.mradio_alignToFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradio_alignToFile.Location = new System.Drawing.Point(19, 21);
            this.mradio_alignToFile.Name = "mradio_alignToFile";
            this.mradio_alignToFile.Size = new System.Drawing.Size(107, 17);
            this.mradio_alignToFile.TabIndex = 14;
            this.mradio_alignToFile.TabStop = true;
            this.mradio_alignToFile.Text = "Align to a dataset";
            this.mradio_alignToFile.UseVisualStyleBackColor = true;
            this.mradio_alignToFile.CheckedChanged += new System.EventHandler(this.mradio_alignToFile_CheckedChanged);
            // 
            // mbtnSavePara
            // 
            this.mbtnSavePara.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnSavePara.Enabled = false;
            this.mbtnSavePara.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnSavePara.Location = new System.Drawing.Point(386, 23);
            this.mbtnSavePara.Name = "mbtnSavePara";
            this.mbtnSavePara.Size = new System.Drawing.Size(37, 23);
            this.mbtnSavePara.TabIndex = 29;
            this.mbtnSavePara.Text = "...";
            this.mbtnSavePara.UseVisualStyleBackColor = false;
            this.mbtnSavePara.Click += new System.EventHandler(this.mbtnSavePara_Click);
            // 
            // label1
            // 
            this.label1.Enabled = false;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(234, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 23);
            this.label1.TabIndex = 28;
            this.label1.Text = "Save Parameters to a File";
            // 
            // ctlSelectParametersWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.mgroupBox_parameters);
            this.Controls.Add(this.mgroupBox_alignment);
            this.MinimumSize = new System.Drawing.Size(704, 407);
            this.Name = "ctlSelectParametersWizardPage";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(828, 545);
            this.mgroupBox_parameters.ResumeLayout(false);
            this.m_peptideIdentificationGroupBox.ResumeLayout(false);
            this.m_peptideIdentificationGroupBox.PerformLayout();
            this.mgroupBox_alignment.ResumeLayout(false);
            this.mgroupBox_alignment.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        #region Properties
        /// <summary>
        /// Gets or sets the server information needed to communicate to DMS.
        /// </summary>
        public clsDMSServerInformation ServerInformation
        {
            get
            {
                return m_serverInformation;
            }
            set
            {
                m_serverInformation = value;
            }
        }     
        #endregion

        #region Parameter Setting Button Event  Handlers
        private void FeatureFinding_Click(object sender, System.EventArgs e)
        {
            frmFeatureFindingParameters optionsForm = new frmFeatureFindingParameters();
            optionsForm.UMCFindingOptions   = m_analysis.UMCFindingOptions;            
            optionsForm.StartPosition       = FormStartPosition.CenterParent;
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                m_analysis.UMCFindingOptions = optionsForm.UMCFindingOptions;
            }
            optionsForm.Dispose();
		}
		private void Alignment_Click(object sender, System.EventArgs e)
        {
            frmMSAlignmentParameters optionsForm = new frmMSAlignmentParameters();
            optionsForm.AlignmentOptions    = m_analysis.DefaultAlignmentOptions;            
            optionsForm.StartPosition       = FormStartPosition.CenterParent;
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                m_analysis.DefaultAlignmentOptions = optionsForm.AlignmentOptions;
            }
            optionsForm.Dispose();
		}
        private void FeatureClustering_Click(object sender, EventArgs e)
        {
            frmClusterParameters optionsForm = new frmClusterParameters();
            optionsForm.ClusterOptions  = m_analysis.ClusterOptions;
            optionsForm.StartPosition   = FormStartPosition.CenterParent;
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                m_analysis.ClusterOptions = optionsForm.ClusterOptions;
            }
            optionsForm.Dispose();
        }
        private void PeakMatching_Click(object sender, System.EventArgs e)
        {
            frmPeakMatchingParameters optionsForm = new frmPeakMatchingParameters();
            optionsForm.PeakMatchingOptions = m_analysis.PeakMatchingOptions;
            optionsForm.StartPosition = FormStartPosition.CenterParent;
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                m_analysis.PeakMatchingOptions = optionsForm.PeakMatchingOptions;
            }
            optionsForm.Dispose();
        }
        /// <summary>
        /// Loads parameters stored in a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonLoadParametersFromFile_Click(object sender, EventArgs e)
        {
            //TODO: Load parameters from file here.
        }
        /// <summary>
        /// Loads the mass tag database form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mbutton_loadMassTagDatabasePeaks_Click(object sender, EventArgs e)
		{
			frmDBName dbForm                = new frmDBName(m_serverInformation.ConnectionExists);
            dbForm.MassTagDatabaseOptions   = m_analysis.MassTagDBOptions;
            dbForm.StartPosition            = FormStartPosition.CenterParent;

			if (dbForm.ShowDialog() == DialogResult.OK)
			{
				m_analysis.MassTagDBOptions      = dbForm.MassTagDatabaseOptions;                
                mlabel_peakMatchingDatabase.Text = m_analysis.MassTagDBOptions.mstrDatabase;
                UpdateAlignmentScenarios();
			}
            dbForm.Dispose();
		}
        /// <summary>
        /// Displays the scoring parameter window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_scoring_Click(object sender, EventArgs e)
        {
            formPeptideIDScoring scoreForm  = new formPeptideIDScoring();                        
            scoreForm.Options               = m_analysis.SMARTOptions;
            if (scoreForm.ShowDialog() == DialogResult.OK)
            {
                m_analysis.SMARTOptions = scoreForm.Options;
            }   
        }
        #endregion


        private void UpdateAlignmentScenarios()
        {
            m_updatingRadioButtons = true;

            // We can only align to a DB.
            if (m_analysis.Datasets.Count < 2)
            {
                mradio_alignToFile.Enabled  = false;
                mradio_alignToMTDB.Enabled  = true;
                mradio_alignToMTDB.Checked  = true;
                mgroupBox_alignment.Enabled = true;
            }

            bool isEmpty = string.IsNullOrEmpty(m_analysis.MassTagDBOptions.mstrDatabase);            
            // Make sure that we can align to a MTDB.
            if (!isEmpty)
            {
                if (m_analysis.UseMassTagDBAsBaseline)
                {
                    mradio_alignToMTDB.Checked = true;
                }
                mradio_alignToMTDB.Enabled              = true;
                m_databaseSelected.Visible              = false;
                m_peptideIdentificationGroupBox.Enabled = true;                
            }
            else
            {
                mradio_alignToMTDB.Enabled = false;
                mradio_alignToMTDB.Checked = false;
                m_databaseSelected.Visible = true;
                m_peptideIdentificationGroupBox.Enabled = false;
            }

            m_updatingRadioButtons = false;
        }

        #region Alignment Selection
        /// <summary>
        /// Determines if the analysis should align to a mass tag database or not.
        /// </summary>
        /// <param name="databaseAlignment"></param>
        public void AlignToDatabase(bool databaseAlignment)
        {
            m_analysis.UseMassTagDBAsBaseline = databaseAlignment;
            UpdateAlignmentScenarios();            
        }
        /// <summary>
        /// Handles when the user clicks to align to a dataset file and not a mass tag database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mradio_alignToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_updatingRadioButtons)
            {
                AlignToDatabase(false);
            }
        }
        /// <summary>
        /// Handles when a user clicks to align to a mass tag database and not a dataset file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mradio_alignToMTDB_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_updatingRadioButtons)
            {
                AlignToDatabase(true);
            }
        }
        #endregion

        
        #region IWizardControl<MultiAlignAnalysis> Members
        /// <summary>
        /// Gets the title of the wizard page.
        /// </summary>
        public string Title
        {
            get
            {
                return "Select Parameters";
            }
        }
        public MultiAlignAnalysis Data
        {
            get
            {
                return m_analysis;
            }
            set
            {
                m_analysis = value;
                if (m_analysis != null)
                {
                    mcomboBox_baseline.Items.Clear();
                    if (m_analysis.Datasets.Count < 2)
                    {
                        mcomboBox_baseline.Enabled = false;
                        mcomboBox_baseline.Items.Add("Select Mass Tag Database (MTDB)");
                        AlignToDatabase(true);
                    }
                    else
                    {
                        mcomboBox_baseline.Enabled = true;
                        foreach (DatasetInformation info in m_analysis.Datasets)
                        {
                            mcomboBox_baseline.Items.Add(info.DatasetName);
                        }

                        if (m_analysis.BaselineDatasetName != null && mcomboBox_baseline.Items.Contains(m_analysis.BaselineDatasetName))
                        {
                            mcomboBox_baseline.SelectedValue = m_analysis.BaselineDatasetName;
                        }
                    }
                    UpdateAlignmentScenarios();
                }
            }
        }
        /// <summary>
        /// Determines if the page is completely filled out.
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            bool isComplete = true;

            if (!m_analysis.UseMassTagDBAsBaseline && string.IsNullOrEmpty(m_analysis.BaselineDatasetName))
            {
                MessageBox.Show("Select a basline first.",
                                "Baseline ?", 
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isComplete = false;
            }
            else if (m_analysis.UseMassTagDBAsBaseline)
            {
                bool specifiedMTDB      = !string.IsNullOrWhiteSpace(m_analysis.MassTagDBOptions.mstrDatabase); 
                specifiedMTDB           = specifiedMTDB && (m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL);

                bool specifiedMTDBFile  = !string.IsNullOrWhiteSpace(m_analysis.MassTagDBOptions.mstr_databaseFilePath); 
                specifiedMTDBFile       = specifiedMTDBFile && (m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS);

                if (!specifiedMTDB && !specifiedMTDBFile)
                {
                    MessageBox.Show("Mass Tag DB not loaded.",
                                    "Load Mass Tag DB",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    isComplete = false;
                }
            }
            return isComplete;
        }
        public void SetAsActivePage()
        {
            bool usePredefinedFeaturesOnly = true;
            foreach (DatasetInformation info in m_analysis.Datasets)
            {
                bool containsFeatures = info.DatasetName.Contains("_LCMSFeatures");
                if (!containsFeatures)
                {
                    usePredefinedFeaturesOnly = false;
                    break;
                }
            }

            m_peakPickingParametersButton.Enabled = (usePredefinedFeaturesOnly == false);
        }
        #endregion
        
        private void mcomboBox_baseline_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_analysis.BaselineDatasetName = mcomboBox_baseline.SelectedItem.ToString();
        }
        private void mradioButton_useSMART_CheckedChanged(object sender, EventArgs e)
        {
            m_analysis.UseSTAC = mradioButton_useSMART.Checked;            
        }

        private void mbtnSavePara_Click(object sender, EventArgs e)
        {
            
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" ;
			saveFileDialog1.FilterIndex = 1 ;
			saveFileDialog1.RestoreDirectory = true ;
			saveFileDialog1.InitialDirectory = "c:\\";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}

