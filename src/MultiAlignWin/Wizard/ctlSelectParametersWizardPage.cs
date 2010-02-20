using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MultiAlignWin
{
	public class ctlSelectParametersWizardPage: Wizard.UI.InternalWizardPage
    {
        #region Windows Form Members

        private Button mbtnPeakPickingParameters;
        private Button mbtnAlignmentParameters;
        private Button peakMatchingButton;
        private IContainer components = null;
        private Button buttonLoadParametersFromFile;
        private ComboBox mcomboBox_baseline;
        private GroupBox mgroupBox_parameters;
        private GroupBox mgroupBox_alignment;
        private RadioButton mradio_alignToMTDB;
        private RadioButton mradio_alignToFile;
    				
		public delegate void OptionsButtonClicked() ;
        public delegate void DelegateMassTagDatabase(string newMTDB);
        public delegate void DelegatePeakMatchToDatabase(bool peakMatch);
        
        public event OptionsButtonClicked PeakPickingParameters ;
        public event OptionsButtonClicked AlignmentParameters;
        public event OptionsButtonClicked ClusteringParameters;
        public event OptionsButtonClicked PeakMatchingParameters;
		public event OptionsButtonClicked SelectMassTagDatabase ;
		public event OptionsButtonClicked LoadParametersFromFile;
        /// <summary>
        /// Fired when the user wants to set the scoring parameters;
        /// </summary>
        public event OptionsButtonClicked ScoringParameters;
        #endregion

        /// <summary>
        /// Flag indicating whether to align to database or not.
        /// </summary>
        private bool mbool_alignToDatabase;       
        private Button mbutton_loadMassTagDatabasePeaks;
        private Label mlabel_peakMatchingDatabase;
        private Label labelLoadParam;
        private Button smartParametersButton;
        private RadioButton mradioButton_useStandardPeakMatching;
        private RadioButton mradioButton_useSMART;
        private Label mlabel_databaseSelected;
        private GroupBox peptideIdentificationGroupBox;
        private Button mbutton_clustering;

        /// <summary>
        /// Flag indicating whether or not to use SMART.
        /// </summary>
        private bool mbool_useSMART;
       
        /// <summary>
        /// Default constructor for a parameter wizard page.
        /// </summary>
        public ctlSelectParametersWizardPage()
        {
            InitializeComponent();
            mbool_alignToDatabase   = false;
            mbool_useSMART          = false;
        }

        #region Windows Designer
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
            this.mbtnPeakPickingParameters = new System.Windows.Forms.Button();
            this.mbtnAlignmentParameters = new System.Windows.Forms.Button();
            this.peakMatchingButton = new System.Windows.Forms.Button();
            this.buttonLoadParametersFromFile = new System.Windows.Forms.Button();
            this.mcomboBox_baseline = new System.Windows.Forms.ComboBox();
            this.mgroupBox_parameters = new System.Windows.Forms.GroupBox();
            this.mbutton_clustering = new System.Windows.Forms.Button();
            this.peptideIdentificationGroupBox = new System.Windows.Forms.GroupBox();
            this.mradioButton_useSMART = new System.Windows.Forms.RadioButton();
            this.mradioButton_useStandardPeakMatching = new System.Windows.Forms.RadioButton();
            this.smartParametersButton = new System.Windows.Forms.Button();
            this.labelLoadParam = new System.Windows.Forms.Label();
            this.mbutton_loadMassTagDatabasePeaks = new System.Windows.Forms.Button();
            this.mlabel_peakMatchingDatabase = new System.Windows.Forms.Label();
            this.mgroupBox_alignment = new System.Windows.Forms.GroupBox();
            this.mlabel_databaseSelected = new System.Windows.Forms.Label();
            this.mradio_alignToMTDB = new System.Windows.Forms.RadioButton();
            this.mradio_alignToFile = new System.Windows.Forms.RadioButton();
            this.mgroupBox_parameters.SuspendLayout();
            this.peptideIdentificationGroupBox.SuspendLayout();
            this.mgroupBox_alignment.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.BackColor = System.Drawing.Color.White;
            this.Banner.Location = new System.Drawing.Point(139, 5);
            this.Banner.Size = new System.Drawing.Size(684, 64);
            this.Banner.Subtitle = "Select alignment, clustering, and peak picking parameters and baseline for alignm" +
                "ent.";
            this.Banner.Title = "Step 3.  Select Parameters and Baseline";
            // 
            // mbtnPeakPickingParameters
            // 
            this.mbtnPeakPickingParameters.BackColor = System.Drawing.Color.White;
            this.mbtnPeakPickingParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbtnPeakPickingParameters.Image = global::MultiAlignWin.Properties.Resources.featuresGlyph;
            this.mbtnPeakPickingParameters.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbtnPeakPickingParameters.Location = new System.Drawing.Point(19, 73);
            this.mbtnPeakPickingParameters.Name = "mbtnPeakPickingParameters";
            this.mbtnPeakPickingParameters.Size = new System.Drawing.Size(183, 31);
            this.mbtnPeakPickingParameters.TabIndex = 2;
            this.mbtnPeakPickingParameters.Text = "LCMS Feature Finding ";
            this.mbtnPeakPickingParameters.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbtnPeakPickingParameters.UseVisualStyleBackColor = false;
            this.mbtnPeakPickingParameters.Click += new System.EventHandler(this.mbtnPeakPickingParameters_Click);
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
            this.mbtnAlignmentParameters.Click += new System.EventHandler(this.mbtnAlignmentParameters_Click);
            // 
            // peakMatchingButton
            // 
            this.peakMatchingButton.BackColor = System.Drawing.Color.White;
            this.peakMatchingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peakMatchingButton.Image = global::MultiAlignWin.Properties.Resources.clusteringGlyph;
            this.peakMatchingButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.peakMatchingButton.Location = new System.Drawing.Point(192, 19);
            this.peakMatchingButton.Name = "peakMatchingButton";
            this.peakMatchingButton.Size = new System.Drawing.Size(221, 31);
            this.peakMatchingButton.TabIndex = 2;
            this.peakMatchingButton.Text = "Peak Matching";
            this.peakMatchingButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.peakMatchingButton.UseVisualStyleBackColor = false;
            this.peakMatchingButton.Click += new System.EventHandler(this.mbtnClusteringParameters_Click);
            // 
            // buttonLoadParametersFromFile
            // 
            this.buttonLoadParametersFromFile.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLoadParametersFromFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLoadParametersFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoadParametersFromFile.Location = new System.Drawing.Point(178, 27);
            this.buttonLoadParametersFromFile.Name = "buttonLoadParametersFromFile";
            this.buttonLoadParametersFromFile.Size = new System.Drawing.Size(24, 24);
            this.buttonLoadParametersFromFile.TabIndex = 10;
            this.buttonLoadParametersFromFile.Text = "...";
            this.buttonLoadParametersFromFile.UseVisualStyleBackColor = false;
            this.buttonLoadParametersFromFile.Click += new System.EventHandler(this.buttonLoadParametersFromFile_Click);
            // 
            // mcomboBox_baseline
            // 
            this.mcomboBox_baseline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcomboBox_baseline.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mcomboBox_baseline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mcomboBox_baseline.Location = new System.Drawing.Point(43, 44);
            this.mcomboBox_baseline.Name = "mcomboBox_baseline";
            this.mcomboBox_baseline.Size = new System.Drawing.Size(606, 21);
            this.mcomboBox_baseline.TabIndex = 13;
            this.mcomboBox_baseline.Text = "Select Baseline Dataset for Alignment";
            // 
            // mgroupBox_parameters
            // 
            this.mgroupBox_parameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_parameters.Controls.Add(this.mbutton_clustering);
            this.mgroupBox_parameters.Controls.Add(this.peptideIdentificationGroupBox);
            this.mgroupBox_parameters.Controls.Add(this.buttonLoadParametersFromFile);
            this.mgroupBox_parameters.Controls.Add(this.labelLoadParam);
            this.mgroupBox_parameters.Controls.Add(this.mbutton_loadMassTagDatabasePeaks);
            this.mgroupBox_parameters.Controls.Add(this.mbtnPeakPickingParameters);
            this.mgroupBox_parameters.Controls.Add(this.mlabel_peakMatchingDatabase);
            this.mgroupBox_parameters.Controls.Add(this.mbtnAlignmentParameters);
            this.mgroupBox_parameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.mgroupBox_parameters.Location = new System.Drawing.Point(154, 75);
            this.mgroupBox_parameters.Name = "mgroupBox_parameters";
            this.mgroupBox_parameters.Size = new System.Drawing.Size(666, 349);
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
            this.mbutton_clustering.Click += new System.EventHandler(this.mbutton_clustering_Click);
            // 
            // peptideIdentificationGroupBox
            // 
            this.peptideIdentificationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.peptideIdentificationGroupBox.Controls.Add(this.peakMatchingButton);
            this.peptideIdentificationGroupBox.Controls.Add(this.mradioButton_useSMART);
            this.peptideIdentificationGroupBox.Controls.Add(this.mradioButton_useStandardPeakMatching);
            this.peptideIdentificationGroupBox.Controls.Add(this.smartParametersButton);
            this.peptideIdentificationGroupBox.Enabled = false;
            this.peptideIdentificationGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peptideIdentificationGroupBox.Location = new System.Drawing.Point(19, 238);
            this.peptideIdentificationGroupBox.Name = "peptideIdentificationGroupBox";
            this.peptideIdentificationGroupBox.Size = new System.Drawing.Size(641, 99);
            this.peptideIdentificationGroupBox.TabIndex = 26;
            this.peptideIdentificationGroupBox.TabStop = false;
            this.peptideIdentificationGroupBox.Text = "Peptide Identification";
            // 
            // mradioButton_useSMART
            // 
            this.mradioButton_useSMART.AutoSize = true;
            this.mradioButton_useSMART.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mradioButton_useSMART.Location = new System.Drawing.Point(7, 63);
            this.mradioButton_useSMART.Name = "mradioButton_useSMART";
            this.mradioButton_useSMART.Size = new System.Drawing.Size(85, 17);
            this.mradioButton_useSMART.TabIndex = 24;
            this.mradioButton_useSMART.Text = "Use SMART";
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
            this.mradioButton_useStandardPeakMatching.CheckedChanged += new System.EventHandler(this.mradioButton_useStandardPeakMatching_CheckedChanged);
            // 
            // smartParametersButton
            // 
            this.smartParametersButton.BackColor = System.Drawing.Color.White;
            this.smartParametersButton.Enabled = false;
            this.smartParametersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.smartParametersButton.Image = global::MultiAlignWin.Properties.Resources.smart;
            this.smartParametersButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.smartParametersButton.Location = new System.Drawing.Point(192, 56);
            this.smartParametersButton.Name = "smartParametersButton";
            this.smartParametersButton.Size = new System.Drawing.Size(219, 30);
            this.smartParametersButton.TabIndex = 23;
            this.smartParametersButton.Text = "SMART";
            this.smartParametersButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.mbutton_loadMassTagDatabasePeaks.Click += new System.EventHandler(this.mbutton_loadMassTagDatabasePeaks_Click);
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
            this.mlabel_peakMatchingDatabase.Size = new System.Drawing.Size(441, 20);
            this.mlabel_peakMatchingDatabase.TabIndex = 22;
            this.mlabel_peakMatchingDatabase.Text = "No Database Selected";
            this.mlabel_peakMatchingDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mgroupBox_alignment
            // 
            this.mgroupBox_alignment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_alignment.Controls.Add(this.mlabel_databaseSelected);
            this.mgroupBox_alignment.Controls.Add(this.mradio_alignToMTDB);
            this.mgroupBox_alignment.Controls.Add(this.mradio_alignToFile);
            this.mgroupBox_alignment.Controls.Add(this.mcomboBox_baseline);
            this.mgroupBox_alignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.mgroupBox_alignment.Location = new System.Drawing.Point(154, 430);
            this.mgroupBox_alignment.Name = "mgroupBox_alignment";
            this.mgroupBox_alignment.Size = new System.Drawing.Size(666, 159);
            this.mgroupBox_alignment.TabIndex = 17;
            this.mgroupBox_alignment.TabStop = false;
            this.mgroupBox_alignment.Text = "Select Baseline";
            // 
            // mlabel_databaseSelected
            // 
            this.mlabel_databaseSelected.AutoSize = true;
            this.mlabel_databaseSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_databaseSelected.ForeColor = System.Drawing.Color.Red;
            this.mlabel_databaseSelected.Location = new System.Drawing.Point(40, 91);
            this.mlabel_databaseSelected.Name = "mlabel_databaseSelected";
            this.mlabel_databaseSelected.Size = new System.Drawing.Size(144, 13);
            this.mlabel_databaseSelected.TabIndex = 16;
            this.mlabel_databaseSelected.Text = "No database is selected";
            this.mlabel_databaseSelected.Visible = false;
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
            // ctlSelectParametersWizardPage
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.mgroupBox_parameters);
            this.Controls.Add(this.mgroupBox_alignment);
            this.MinimumSize = new System.Drawing.Size(704, 407);
            this.Name = "ctlSelectParametersWizardPage";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(828, 624);
            this.Controls.SetChildIndex(this.mgroupBox_alignment, 0);
            this.Controls.SetChildIndex(this.mgroupBox_parameters, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.mgroupBox_parameters.ResumeLayout(false);
            this.peptideIdentificationGroupBox.ResumeLayout(false);
            this.peptideIdentificationGroupBox.PerformLayout();
            this.mgroupBox_alignment.ResumeLayout(false);
            this.mgroupBox_alignment.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the file aliases used in the dataset alignment combo box.
        /// </summary>
        public string [] FileAliases
		{
			set
			{
				mcomboBox_baseline.Items.Clear() ; 
				string []aliases = value ; 
				int numFiles = aliases.Length ;
				if (numFiles == 1)
				{
					mcomboBox_baseline.Items.Add("Select Mass Tag DataBase") ;
					AlignToDatabase(true);
				}
				else
				{
					for (int fileNum = 0 ; fileNum < numFiles ; fileNum++)
					{
						mcomboBox_baseline.Items.Add(aliases[fileNum]) ; 
					}
				}
			}
			get
			{
				string []aliases = new string[mcomboBox_baseline.Items.Count] ;
				for (int i = 0 ; i < mcomboBox_baseline.Items.Count ; i++)
				{
					aliases[i] = mcomboBox_baseline.Items[i].ToString() ;
				}
				return aliases ;
			}
		}
        /// <summary>
        /// Gets or sets the dataset names
        /// </summary>
		public string [] DataSetNames
		{
			set
			{
				mcomboBox_baseline.Items.Clear() ; 
				string []names = value ; 
				int numFiles = names.Length ;
				if (numFiles == 1)
				{
					mcomboBox_baseline.Items.Add("Select Mass Tag Database") ;
                    AlignToDatabase(true);                    
                    mradio_alignToFile.Checked = false;
                    mradio_alignToMTDB.Checked = true;
                    mradio_alignToFile.Enabled = false;
                    mlabel_databaseSelected.Visible = true;
				}
				else
				{
                    mcomboBox_baseline.Text = "Select Baseline Dataset for Alignment";
                    mradio_alignToFile.Enabled = true;
					for (int fileNum = 0 ; fileNum < numFiles ; fileNum++)
					{
						mcomboBox_baseline.Items.Add(names[fileNum]) ; 
					}
                    mlabel_databaseSelected.Visible = false;
				}
			}
			get
			{
				string []names = new string[mcomboBox_baseline.Items.Count] ;
				for (int i = 0 ; i < mcomboBox_baseline.Items.Count ; i++)
				{
					names[i] = mcomboBox_baseline.Items[i].ToString() ;
				}
				return names ;
			}
		}
        /// <summary>
        /// Sets the mass tag database name.
        /// </summary>
		public string MassTagDBName
		{
			set
			{
                mlabel_peakMatchingDatabase.Text = value;
                if (value != null && string.IsNullOrEmpty(value) == false)
                {
                    mradio_alignToMTDB.Enabled      = true;
                    mlabel_databaseSelected.Visible = false;                    
                    peptideIdentificationGroupBox.Enabled = true;

                }
			}
		}
        /// <summary>
        /// Gets or sets whether to enable a baseline selection capability.
        /// </summary>
		public bool EnableBaselineSelection
		{
			get
			{
				return mcomboBox_baseline.Enabled;
			}
			set
			{
				mcomboBox_baseline.Enabled = value;
			}
		}
        /// <summary>
        /// Gets or sets whether to use a mass tag database as the baseline for alignment.
        /// </summary>
		public bool UseMassTagDBAsBaseline
		{
			get
			{
                return mbool_alignToDatabase; 
			}
			set
			{
                mbool_alignToDatabase = value;
			}
		}
        /// <summary>
        /// Gets or sets whether to use SMART.
        /// </summary>
        public bool UseSMART
        {
            get
            {
                return mbool_useSMART;
            }
            set
            {
                mbool_useSMART          = value;
                smartParametersButton.Enabled = mbool_useSMART;
                peakMatchingButton.Enabled = (value == false);
            }
        }
        /// <summary>
        /// Gets the selected index to use as the baseline for alignment.
        /// </summary>
		public int SelectedFileIndex
		{
			get
			{
				return mcomboBox_baseline.SelectedIndex ; 
			}
		}
        /// <summary>
        /// Gets the string name of the baseline dataset name for alignment.
        /// </summary>
		public string SelectedBaseline
		{
			get
			{
				return mcomboBox_baseline.SelectedItem.ToString() ; 
			}
        }
        #endregion

        #region Parameter Setting Button Event  Handlers
        private void mbtnPeakPickingParameters_Click(object sender, System.EventArgs e)
		{
			if (PeakPickingParameters != null)
				PeakPickingParameters() ;		
		}
		private void mbtnAlignmentParameters_Click(object sender, System.EventArgs e)
		{
			if (AlignmentParameters != null)
				AlignmentParameters() ;			
		}
		private void mbtnClusteringParameters_Click(object sender, System.EventArgs e)
		{
            if (PeakMatchingParameters != null)
                PeakMatchingParameters();
        }
        private void mbutton_clustering_Click(object sender, EventArgs e)
        {
            if (ClusteringParameters != null)
                ClusteringParameters();
        }
		private void mbtnLoadMassTagDatabase_Click(object sender, System.EventArgs e)
		{
			if (SelectMassTagDatabase != null)
				SelectMassTagDatabase() ;            
		}
		private void cmbBoxBaseline_Select(object sender, System.EventArgs e)
		{
			if (mcomboBox_baseline.SelectedItem.ToString().IndexOf("Mass Tag") != -1 &&
				SelectMassTagDatabase != null)
				SelectMassTagDatabase() ;
		}
        private void mbtnLoadPara_Click(object sender, EventArgs e)
        {
            if (LoadParametersFromFile != null)
                LoadParametersFromFile();
        }
        private void mbutton_loadMassTagDatabasePeaks_Click(object sender, EventArgs e)
        {
            if (SelectMassTagDatabase != null)
                SelectMassTagDatabase();
        }
		private void buttonLoadParametersFromFile_Click(object sender, System.EventArgs e)
		{
            if (LoadParametersFromFile != null)
                LoadParametersFromFile();
        }
        /// <summary>
        /// Displays the scoring parameter window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_scoring_Click(object sender, EventArgs e)
        {
            //TODO: Fix this stupid way of setting parameters.
            if (ScoringParameters != null)
                ScoringParameters();
        }
        private void mradioButton_useStandardPeakMatching_CheckedChanged(object sender, EventArgs e)
        {
            UseSMART = false;   
        }
        private void mradioButton_useSMART_CheckedChanged(object sender, EventArgs e)
        {
            UseSMART = true;
        }
        #endregion

        #region Alignment Selection
        /// <summary>
        /// Determines if the analysis should align to a mass tag database or not.
        /// </summary>
        /// <param name="databaseAlignment"></param>
        public void AlignToDatabase(bool databaseAlignment)
        {
            mbool_alignToDatabase                    = databaseAlignment;
            mcomboBox_baseline.Enabled               = (databaseAlignment == false);            
        }
        /// <summary>
        /// Handles when the user clicks to align to a dataset file and not a mass tag database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mradio_alignToFile_CheckedChanged(object sender, EventArgs e)
        {
            AlignToDatabase(false);
        }
        /// <summary>
        /// Handles when a user clicks to align to a mass tag database and not a dataset file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mradio_alignToMTDB_CheckedChanged(object sender, EventArgs e)
        {
            AlignToDatabase(true);
        }
        #endregion

    }
}

