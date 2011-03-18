using System;
using System.Data;
using System.Drawing;
using System.Data.OleDb;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Collections.Generic;


using PNNLControls;
using PNNLProteomics.IO;
using PNNLProteomics.Data;
using MultiAlignEngine.MassTags;
using MultiAlignWin.Network;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmDBName.
	/// </summary>
	public class frmDBName : System.Windows.Forms.Form
	{
		internal class clsMTDBentry
		{
			public string dBaseName ;
			public string dBaseDesc ;
			public string server ;

			public clsMTDBentry()
			{
				dBaseName = null ;
				dBaseDesc = null ;
				server = null ;
			}
		}

        /// <summary>
        /// Path to the database on pogo that has the stored procedure for finding all of
        /// the mass tag databases.
        /// </summary>
        private const string CONST_MASS_TAG_DATABASE_PATH = "PRISM_IFC";
        /// <summary>
        /// Path of the server that has the database for finding all the other paths
        /// of the mass tag databases.
        /// </summary>
        private const string CONST_MASS_TAG_SERVER_PATH = "pogo";
        /// <summary>
        /// The most mass tag databases to be held in the recent folder.
        /// </summary>
        private const int CONST_MASS_TAG_DATABASE_RECENT_MAX = 10;
        /// <summary>
        /// Default connection timeout test time.
        /// </summary>
		private const double CONST_TIMEOUT_CONNECTION_TEST = 20.0;
	
        #region Windows Forms
        private ToolTip ttipPMT;
        private IContainer components;
        private Button mbtn_ok;
		private Button mbtn_cancel;
		private ListBox mlstBoxDBNames;
		private CheckBox chkBoxUnused;
		private Label mlblDBdescription;
		private Label mlblServer;
		private Label mlblTotalDBs;		
		private Label mlblSeverTag;
        private Button mbtnDefaults;
		private RadioButton radioRemoteDatabase;
		private RadioButton radioLocalDatabase;
		private TextBox txtDataBasePath;
		private Label lblDatabasePath;
		private Button btnBrowse;
        private GroupBox groupRemoteDatabase;
		private Button mbtnShowMThits;
		private Label mlblMatchMTs;
		private Label label3;
        private Label label6;
		private TextBox mtxtPeptideProphet;
		private Label label5;
		private TextBox mtxtMinXCorr;
		private TextBox mtxtMinDiscriminant;
        private Label label4;
        private GroupBox groupLocalDatabase;
        #endregion  

        #region Members 
        /// <summary>
        /// Flag indicating whether a connection has been made to DMS.
        /// </summary>
        private bool                    mbool_connectionToDMSExists;
        /// <summary>
        /// DMS Server information for loading data from a database.
        /// </summary>
        private clsDMSServerInformation mobj_massTagServerInformation;
        /// <summary>
        /// Mass Tag Database information.
        /// </summary>
        private ArrayList   marrMTDBInfo    = new ArrayList();
        /// <summary>
        /// Flag whether to include unused databases.
        /// </summary>
        private bool        inclUnused      = false;
        /// <summary>
        /// Mass tag database options.
        /// </summary>
		clsMassTagDatabaseOptions defaults  = new clsMassTagDatabaseOptions();
        private LinkLabel       mlink_showPNNLProperties;
        private Label           mlabel_networkConnectionTest;
        private ProgressBar     mprogressBar_connectionTest;
        private LinkLabel       mlinkLabel_testConnection;
        /// <summary>
        /// Network connection tester.
        /// </summary>
        private clsDMSConnectionTester mobj_connectionTester;
        private ListBox mlistbox_recentdatabases;
        private Label label2;
        private Label label7;
        private GroupBox mgroupBox_pmtQualityScore;
        
        private NumericUpDown mnum_pmtQualityScore;
        private Label mlabel_pmtQualityScoreDescription;
        private TextBox mtextBox_experimentNameFilter;
        private Label label1;
        private TextBox mtextBox_experimentExclusionFilter;
        private Label label8;
        private ToolTip tipExclusionFilter;
        private ToolTip tipExperimentFilter;
        private ComboBox mcomboBox_experimentNames;
        private Label label10;
        private NumericUpDown mnum_minNumberMSMSObservations;
        private Label label9;
        /// <summary>
        /// A dictionary to a dictionary that holds pmt quality score information.
        /// </summary>
        private Dictionary<string, Dictionary<int, string>> mdict_pmtQualityScoreDescriptions;    
       

        #endregion

        /// <summary>
        /// Default constructor for a database form.
        /// </summary>
		public frmDBName()
		{			
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			radioRemoteDatabase.Enabled = false;
			radioRemoteDatabase.Checked = false;
            groupRemoteDatabase.Enabled = false;
            mtextBox_experimentExclusionFilter.Enabled  = false;
            mtextBox_experimentNameFilter.Enabled       = false;
			radioLocalDatabase.Enabled  = true;
			radioLocalDatabase.Checked  = true;
			
			mlblDBdescription.Text      = "None selected";
			mlblDBdescription.Enabled   = false;
			mlblServer.Text             = "";
			mlblMatchMTs.Text           = "";
			mlblMatchMTs.Enabled        = false;
			mbtnShowMThits.Enabled      = false;
			mlblSeverTag.Enabled        = false;

            mobj_massTagServerInformation               = new clsDMSServerInformation();
            mobj_massTagServerInformation.ServerName    = CONST_MASS_TAG_SERVER_PATH;
            mobj_massTagServerInformation.Username      = "mtuser";
            mobj_massTagServerInformation.Password      = "mt4fun";
            mobj_massTagServerInformation.DatabaseName  = CONST_MASS_TAG_DATABASE_PATH;

            mobj_connectionTester                    = new clsDMSConnectionTester(false, mobj_massTagServerInformation);
            mobj_connectionTester.ConnectionPercent += new DelegateConnectionToDMSMadePercent(mobj_connectionTester_ConnectionPercent);
            mobj_connectionTester.ConnectionStatus  += new DelegateConnectionToDMS(mobj_connectionTester_ConnectionStatus);
            mobj_connectionTester.TestConnection();
            
			txtDataBasePath.LostFocus       += new EventHandler(txtDataBasePath_LostFocus);
            this.Load += new EventHandler(frmDBName_Load);

            mdict_pmtQualityScoreDescriptions = new Dictionary<string, Dictionary<int, string>>();
            
            LoadRecentDatabaseNames();
            chkBoxUnused.Checked = Properties.Settings.Default.UserDBFormIncludeUnusedDBs;
            
            mlabel_pmtQualityScoreDescription.Text = "No database selected.";
		}
        /// <summary>
        /// Constructor that takes wheteher a previous connection to DMS has been made, indicating that we are inside the PNNL network.
        /// </summary>
        /// <param name="insidePNNLNetwork"></param>
		public frmDBName(bool insidePNNLNetwork)
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            mobj_massTagServerInformation = new clsDMSServerInformation();
            mobj_massTagServerInformation.ServerName = CONST_MASS_TAG_SERVER_PATH;
            mobj_massTagServerInformation.Username = "mtuser";
            mobj_massTagServerInformation.Password = "mt4fun";
            mobj_massTagServerInformation.DatabaseName = CONST_MASS_TAG_DATABASE_PATH;
            mobj_massTagServerInformation.ConnectionExists = insidePNNLNetwork;
            
            
            mobj_connectionTester                    = new clsDMSConnectionTester(insidePNNLNetwork, mobj_massTagServerInformation);
            mobj_connectionTester.ServerInformation.ConnectionExists = insidePNNLNetwork;

            mobj_connectionTester.ConnectionPercent += new DelegateConnectionToDMSMadePercent(mobj_connectionTester_ConnectionPercent);
            mobj_connectionTester.ConnectionStatus  += new DelegateConnectionToDMS(mobj_connectionTester_ConnectionStatus);


            mdict_pmtQualityScoreDescriptions = new Dictionary<string, Dictionary<int, string>>();
            
			/// 
			/// Make sure we have a network connection to DMS
			/// 			
			mbool_connectionToDMSExists = insidePNNLNetwork;
			if (mbool_connectionToDMSExists == true)
			{
				LoadMassTagDBNames();
			}
			else
			{
				radioRemoteDatabase.Enabled = false;
				radioRemoteDatabase.Checked = false;
                groupRemoteDatabase.Enabled = false;
                mtextBox_experimentExclusionFilter.Enabled = false;
                mtextBox_experimentNameFilter.Enabled = false;
				radioLocalDatabase.Checked  = true;
			}
            
			mlblDBdescription.Text      = "None selected" ;
			mlblDBdescription.Enabled   = false ;
			mlblServer.Text             = "" ;
			mlblMatchMTs.Text           = "" ;
			mlblMatchMTs.Enabled        = false ;
			mbtnShowMThits.Enabled      = false ;
			mlblSeverTag.Enabled        = false ;
		
			txtDataBasePath.LostFocus   += new EventHandler(txtDataBasePath_LostFocus);
            Load +=new EventHandler(frmDBName_Load);

            LoadRecentDatabaseNames();
            chkBoxUnused.Checked = Properties.Settings.Default.UserDBFormIncludeUnusedDBs;


            mlabel_pmtQualityScoreDescription.Text = "No database selected.";
        }

        /// <summary>
        /// Loads the most recent used databases to the form.
        /// </summary>
        private void LoadRecentDatabaseNames()
        {
            StringCollection collection = Properties.Settings.Default.RecentMassTagDatabases;
            
            mlistbox_recentdatabases.BeginUpdate();
            mlistbox_recentdatabases.Items.Clear();
            if (collection != null)
            {
                foreach (string item in collection)
                {
                    mlistbox_recentdatabases.Items.Insert(0, item);
                }                
            }
            mlistbox_recentdatabases.EndUpdate();
        }
        /// <summary>
        /// Add the mass tag database to the recent list.
        /// </summary>
        /// <param name="databaseName"></param>
        private void AddMassTagDatabaseToRecents(string databaseName)
        {

            StringCollection collection = Properties.Settings.Default.RecentMassTagDatabases;
            if (collection == null)
            {
                collection = new StringCollection();
            }

            
            /// 
            /// Add the database to the recent list and then update the settings 
            /// if we have too many entries by removing from the front of the list.
            /// 
            if (collection.Contains(databaseName) == false)
            {
                collection.Add(databaseName);
            }
            else
            {
                collection.Remove(databaseName);
                collection.Add(databaseName);
            }
            
            if (collection.Count > CONST_MASS_TAG_DATABASE_RECENT_MAX)
            {
                collection.RemoveAt(0);
            }

            /// 
            /// Save the settings now.
            /// 
            Properties.Settings.Default.RecentMassTagDatabases = collection;
            Properties.Settings.Default.Save();

            LoadRecentDatabaseNames();
                 
        }

        #region Network Connection Test Handlers
        /// <summary>
        /// Updates the network connection testing progress bar.
        /// </summary>
        /// <param name="percentage"></param>
        private void UpdateNetworkTestConnectionPercentage(double percentage)
        {
            mprogressBar_connectionTest.Style = ProgressBarStyle.Blocks;
            mprogressBar_connectionTest.Value = Math.Max(mprogressBar_connectionTest.Minimum,
                                                    Math.Min(mprogressBar_connectionTest.Maximum,
                                                    Convert.ToInt32(percentage)));
        }
        /// <summary>
        /// Enables or disables items based off of the ability to connect to DMS.
        /// </summary>
        /// <param name="canLoadFromDMS"></param>
        private void UpdateDMSSourceLoadingItems(bool canLoadFromDMS)
        {
            if (canLoadFromDMS == true)
            {
                mlabel_networkConnectionTest.Text = "Connection to DMS found!";
                mlabel_networkConnectionTest.ForeColor = Color.Black;
                if (mbool_connectionToDMSExists == false)
                {
                    mbool_connectionToDMSExists = true;
                    LoadMassTagDBNames();

                    radioRemoteDatabase.Enabled = true;

                    mtextBox_experimentExclusionFilter.Enabled = true;
                    mtextBox_experimentNameFilter.Enabled = true;
                }
            }
            else
            {
                radioRemoteDatabase.Enabled = false;
                groupRemoteDatabase.Enabled = false;

                mtextBox_experimentExclusionFilter.Enabled  = false;
                mtextBox_experimentNameFilter.Enabled       = false;

                mlabel_networkConnectionTest.Text = "Could not find connection to DMS!";
                mlabel_networkConnectionTest.ForeColor = Color.Red;
            }
        }
        /// <summary>
        /// Hids the network connection test controls.
        /// </summary>
        private void HideNetworkControls()
        {
            mprogressBar_connectionTest.Visible = false;
            mlink_showPNNLProperties.Enabled = true;
        }
        /// <summary>
        /// Makes network testing controls visible.
        /// </summary>
        private void ShowNetworkControls()
        {
            mlabel_networkConnectionTest.Text = "Testing Connection To DMS...";
            mlabel_networkConnectionTest.ForeColor = Color.Black;

            mprogressBar_connectionTest.Visible = true;
            mlink_showPNNLProperties.Enabled = false;
        }
        /// <summary>
        /// Determines if the status was a success or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status"></param>
        void mobj_connectionTester_ConnectionStatus(object sender, bool status)
        {
            if (InvokeRequired == true)
            {
                Invoke(new MethodInvoker(HideNetworkControls));
                Invoke(new DelegateUpdateLoadingState(UpdateDMSSourceLoadingItems), status);
            }
            else
            {
                HideNetworkControls();
                UpdateDMSSourceLoadingItems(status);
            }
        }
        /// <summary>
        /// Determines how much of the timeout is left to display back to the user who may be waiting for a DMS connection attempt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="percentWaited"></param>
        void mobj_connectionTester_ConnectionPercent(object sender, double percentWaited)
        {
            if (InvokeRequired == true)
            {
                Invoke(new DelegateUpdateLoadingPercentLoaded(UpdateNetworkTestConnectionPercentage), percentWaited);
            }
            else
            {
                UpdateNetworkTestConnectionPercentage(percentWaited);
            }

        }
        #endregion

        #region Windows Designer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDBName));
            this.mbtn_ok = new System.Windows.Forms.Button();
            this.mbtn_cancel = new System.Windows.Forms.Button();
            this.mlstBoxDBNames = new System.Windows.Forms.ListBox();
            this.chkBoxUnused = new System.Windows.Forms.CheckBox();
            this.groupRemoteDatabase = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.mcomboBox_experimentNames = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mlistbox_recentdatabases = new System.Windows.Forms.ListBox();
            this.mlblDBdescription = new System.Windows.Forms.Label();
            this.mlblTotalDBs = new System.Windows.Forms.Label();
            this.mlblServer = new System.Windows.Forms.Label();
            this.mlblSeverTag = new System.Windows.Forms.Label();
            this.ttipPMT = new System.Windows.Forms.ToolTip(this.components);
            this.btnBrowse = new System.Windows.Forms.Button();
            this.mbtnShowMThits = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mtxtPeptideProphet = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mtxtMinXCorr = new System.Windows.Forms.TextBox();
            this.mtxtMinDiscriminant = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.mtextBox_experimentNameFilter = new System.Windows.Forms.TextBox();
            this.mbtnDefaults = new System.Windows.Forms.Button();
            this.radioRemoteDatabase = new System.Windows.Forms.RadioButton();
            this.txtDataBasePath = new System.Windows.Forms.TextBox();
            this.lblDatabasePath = new System.Windows.Forms.Label();
            this.radioLocalDatabase = new System.Windows.Forms.RadioButton();
            this.mlblMatchMTs = new System.Windows.Forms.Label();
            this.groupLocalDatabase = new System.Windows.Forms.GroupBox();
            this.mlink_showPNNLProperties = new System.Windows.Forms.LinkLabel();
            this.mlabel_networkConnectionTest = new System.Windows.Forms.Label();
            this.mprogressBar_connectionTest = new System.Windows.Forms.ProgressBar();
            this.mlinkLabel_testConnection = new System.Windows.Forms.LinkLabel();
            this.mgroupBox_pmtQualityScore = new System.Windows.Forms.GroupBox();
            this.mlabel_pmtQualityScoreDescription = new System.Windows.Forms.Label();
            
            this.mnum_pmtQualityScore = new System.Windows.Forms.NumericUpDown();
            this.mtextBox_experimentExclusionFilter = new System.Windows.Forms.TextBox();
            this.tipExclusionFilter = new System.Windows.Forms.ToolTip(this.components);
            this.tipExperimentFilter = new System.Windows.Forms.ToolTip(this.components);
            this.mnum_minNumberMSMSObservations = new System.Windows.Forms.NumericUpDown();
            this.groupRemoteDatabase.SuspendLayout();
            this.groupLocalDatabase.SuspendLayout();
            this.mgroupBox_pmtQualityScore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pmtQualityScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minNumberMSMSObservations)).BeginInit();
            this.SuspendLayout();
            // 
            // mbtn_ok
            // 
            this.mbtn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbtn_ok.BackColor = System.Drawing.SystemColors.Control;
            this.mbtn_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtn_ok.Location = new System.Drawing.Point(504, 806);
            this.mbtn_ok.Name = "mbtn_ok";
            this.mbtn_ok.Size = new System.Drawing.Size(96, 24);
            this.mbtn_ok.TabIndex = 8;
            this.mbtn_ok.Text = "OK";
            this.mbtn_ok.UseVisualStyleBackColor = false;
            this.mbtn_ok.Click += new System.EventHandler(this.mbtn_ok_Click);
            // 
            // mbtn_cancel
            // 
            this.mbtn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbtn_cancel.BackColor = System.Drawing.SystemColors.Control;
            this.mbtn_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtn_cancel.Location = new System.Drawing.Point(606, 806);
            this.mbtn_cancel.Name = "mbtn_cancel";
            this.mbtn_cancel.Size = new System.Drawing.Size(96, 24);
            this.mbtn_cancel.TabIndex = 9;
            this.mbtn_cancel.Text = "Cancel";
            this.mbtn_cancel.UseVisualStyleBackColor = false;
            this.mbtn_cancel.Click += new System.EventHandler(this.mbtn_cancel_Click);
            // 
            // mlstBoxDBNames
            // 
            this.mlstBoxDBNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlstBoxDBNames.Location = new System.Drawing.Point(8, 35);
            this.mlstBoxDBNames.Name = "mlstBoxDBNames";
            this.mlstBoxDBNames.Size = new System.Drawing.Size(338, 264);
            this.mlstBoxDBNames.TabIndex = 0;
            this.mlstBoxDBNames.SelectedIndexChanged += new System.EventHandler(this.mlstBoxDBNames_SelectedIndexChanged);
            // 
            // chkBoxUnused
            // 
            this.chkBoxUnused.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkBoxUnused.Location = new System.Drawing.Point(6, 391);
            this.chkBoxUnused.Name = "chkBoxUnused";
            this.chkBoxUnused.Size = new System.Drawing.Size(128, 16);
            this.chkBoxUnused.TabIndex = 1;
            this.chkBoxUnused.Text = "Include unused DBs";
            this.chkBoxUnused.CheckedChanged += new System.EventHandler(this.chkBoxUnused_CheckedChanged);
            // 
            // groupRemoteDatabase
            // 
            this.groupRemoteDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupRemoteDatabase.Controls.Add(this.label10);
            this.groupRemoteDatabase.Controls.Add(this.mcomboBox_experimentNames);
            this.groupRemoteDatabase.Controls.Add(this.label7);
            this.groupRemoteDatabase.Controls.Add(this.label2);
            this.groupRemoteDatabase.Controls.Add(this.mlistbox_recentdatabases);
            this.groupRemoteDatabase.Controls.Add(this.mlblDBdescription);
            this.groupRemoteDatabase.Controls.Add(this.mlblTotalDBs);
            this.groupRemoteDatabase.Controls.Add(this.mlblServer);
            this.groupRemoteDatabase.Controls.Add(this.mlblSeverTag);
            this.groupRemoteDatabase.Controls.Add(this.mlstBoxDBNames);
            this.groupRemoteDatabase.Controls.Add(this.chkBoxUnused);
            this.groupRemoteDatabase.Enabled = false;
            this.groupRemoteDatabase.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupRemoteDatabase.Location = new System.Drawing.Point(31, 96);
            this.groupRemoteDatabase.Name = "groupRemoteDatabase";
            this.groupRemoteDatabase.Size = new System.Drawing.Size(671, 417);
            this.groupRemoteDatabase.TabIndex = 18;
            this.groupRemoteDatabase.TabStop = false;
            this.groupRemoteDatabase.Text = "Select Database";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 346);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 20);
            this.label10.TabIndex = 71;
            this.label10.Text = "Experiments:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mcomboBox_experimentNames
            // 
            this.mcomboBox_experimentNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mcomboBox_experimentNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcomboBox_experimentNames.FormattingEnabled = true;
            this.mcomboBox_experimentNames.Location = new System.Drawing.Point(84, 345);
            this.mcomboBox_experimentNames.Name = "mcomboBox_experimentNames";
            this.mcomboBox_experimentNames.Size = new System.Drawing.Size(336, 21);
            this.mcomboBox_experimentNames.TabIndex = 70;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Found databases";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(346, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Recently used databases";
            // 
            // mlistbox_recentdatabases
            // 
            this.mlistbox_recentdatabases.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistbox_recentdatabases.Location = new System.Drawing.Point(348, 35);
            this.mlistbox_recentdatabases.Name = "mlistbox_recentdatabases";
            this.mlistbox_recentdatabases.Size = new System.Drawing.Size(317, 264);
            this.mlistbox_recentdatabases.TabIndex = 21;
            this.mlistbox_recentdatabases.SelectedIndexChanged += new System.EventHandler(this.mlistbox_recentdatabases_SelectedIndexChanged);
            // 
            // mlblDBdescription
            // 
            this.mlblDBdescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlblDBdescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlblDBdescription.Location = new System.Drawing.Point(5, 302);
            this.mlblDBdescription.Name = "mlblDBdescription";
            this.mlblDBdescription.Size = new System.Drawing.Size(654, 30);
            this.mlblDBdescription.TabIndex = 19;
            this.mlblDBdescription.Text = "DB description";
            this.mlblDBdescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mlblTotalDBs
            // 
            this.mlblTotalDBs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlblTotalDBs.Location = new System.Drawing.Point(137, 392);
            this.mlblTotalDBs.Name = "mlblTotalDBs";
            this.mlblTotalDBs.Size = new System.Drawing.Size(523, 16);
            this.mlblTotalDBs.TabIndex = 20;
            // 
            // mlblServer
            // 
            this.mlblServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlblServer.Location = new System.Drawing.Point(61, 365);
            this.mlblServer.Name = "mlblServer";
            this.mlblServer.Size = new System.Drawing.Size(196, 23);
            this.mlblServer.TabIndex = 19;
            this.mlblServer.Text = "server";
            this.mlblServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mlblSeverTag
            // 
            this.mlblSeverTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlblSeverTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlblSeverTag.Location = new System.Drawing.Point(5, 366);
            this.mlblSeverTag.Name = "mlblSeverTag";
            this.mlblSeverTag.Size = new System.Drawing.Size(49, 20);
            this.mlblSeverTag.TabIndex = 18;
            this.mlblSeverTag.Text = "Server:";
            this.mlblSeverTag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.BackColor = System.Drawing.SystemColors.Control;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBrowse.Location = new System.Drawing.Point(635, 16);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnBrowse.TabIndex = 24;
            this.btnBrowse.Text = "...";
            this.ttipPMT.SetToolTip(this.btnBrowse, "Get the matching Mass Tag count for the choice of scores above.");
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // mbtnShowMThits
            // 
            this.mbtnShowMThits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbtnShowMThits.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnShowMThits.Enabled = false;
            this.mbtnShowMThits.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnShowMThits.Location = new System.Drawing.Point(28, 739);
            this.mbtnShowMThits.Name = "mbtnShowMThits";
            this.mbtnShowMThits.Size = new System.Drawing.Size(136, 46);
            this.mbtnShowMThits.TabIndex = 29;
            this.mbtnShowMThits.Text = "Show Mass Tag Count (without Min MS/MS  Filter)";
            this.ttipPMT.SetToolTip(this.mbtnShowMThits, "Get the matching Mass Tag count for the choice of scores above.");
            this.mbtnShowMThits.UseVisualStyleBackColor = false;
            this.mbtnShowMThits.Click += new System.EventHandler(this.mbtnShowMThits_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(25, 590);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 20);
            this.label3.TabIndex = 28;
            this.label3.Text = "PMT Quality Score:      >=";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label3, "The minimum PMT_Quality_Score to allow; 0 to allow all");
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(25, 664);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 29);
            this.label6.TabIndex = 32;
            this.label6.Text = "Peptide Prophet Probability:      >=";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label6, "The minimum High_Peptide_Prophet_Probability value to allow; 0 to allow");
            // 
            // mtxtPeptideProphet
            // 
            this.mtxtPeptideProphet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mtxtPeptideProphet.Location = new System.Drawing.Point(198, 667);
            this.mtxtPeptideProphet.Name = "mtxtPeptideProphet";
            this.mtxtPeptideProphet.Size = new System.Drawing.Size(111, 20);
            this.mtxtPeptideProphet.TabIndex = 27;
            this.mtxtPeptideProphet.Text = ".95";
            this.mtxtPeptideProphet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ttipPMT.SetToolTip(this.mtxtPeptideProphet, "The minimum High_Peptide_Prophet_Probability value to allow; 0 to allow");
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(25, 640);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 21);
            this.label5.TabIndex = 31;
            this.label5.Text = "Min. Discriminant:         >=";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label5, "The minimum High_Discriminant_Score to allow; 0 to allow all");
            // 
            // mtxtMinXCorr
            // 
            this.mtxtMinXCorr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mtxtMinXCorr.Location = new System.Drawing.Point(198, 615);
            this.mtxtMinXCorr.Name = "mtxtMinXCorr";
            this.mtxtMinXCorr.Size = new System.Drawing.Size(111, 20);
            this.mtxtMinXCorr.TabIndex = 25;
            this.mtxtMinXCorr.Text = "2";
            this.mtxtMinXCorr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ttipPMT.SetToolTip(this.mtxtMinXCorr, "The minimum value required for High_Normalized_Score; 0 to allow all");
            // 
            // mtxtMinDiscriminant
            // 
            this.mtxtMinDiscriminant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mtxtMinDiscriminant.Location = new System.Drawing.Point(198, 641);
            this.mtxtMinDiscriminant.Name = "mtxtMinDiscriminant";
            this.mtxtMinDiscriminant.Size = new System.Drawing.Size(111, 20);
            this.mtxtMinDiscriminant.TabIndex = 26;
            this.mtxtMinDiscriminant.Text = "0";
            this.mtxtMinDiscriminant.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ttipPMT.SetToolTip(this.mtxtMinDiscriminant, "The minimum High_Discriminant_Score to allow; 0 to allow all");
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(25, 615);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 20);
            this.label4.TabIndex = 30;
            this.label4.Text = "Min. XCorr:                   >=";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label4, "The minimum value required for High_Normalized_Score; 0 to allow all");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 693);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 20);
            this.label1.TabIndex = 65;
            this.label1.Text = "MTDB Experiment Filter";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label1, "The minimum value required for High_Normalized_Score; 0 to allow all");
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(320, 694);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(175, 20);
            this.label8.TabIndex = 67;
            this.label8.Text = "MTDB Experiment Exclusion Filter";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label8, "The minimum value required for High_Normalized_Score; 0 to allow all");
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(25, 713);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 23);
            this.label9.TabIndex = 69;
            this.label9.Text = "Min. # of MS/MS Obs.             >=";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ttipPMT.SetToolTip(this.label9, "The minimum value required for High_Normalized_Score; 0 to allow all");
            // 
            // mtextBox_experimentNameFilter
            // 
            this.mtextBox_experimentNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mtextBox_experimentNameFilter.Location = new System.Drawing.Point(198, 693);
            this.mtextBox_experimentNameFilter.Name = "mtextBox_experimentNameFilter";
            this.mtextBox_experimentNameFilter.Size = new System.Drawing.Size(111, 20);
            this.mtextBox_experimentNameFilter.TabIndex = 64;
            this.mtextBox_experimentNameFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipExperimentFilter.SetToolTip(this.mtextBox_experimentNameFilter, "Experiment names to include when loading a subset of the MTDB. ");
            // 
            // mbtnDefaults
            // 
            this.mbtnDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbtnDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnDefaults.Location = new System.Drawing.Point(28, 806);
            this.mbtnDefaults.Name = "mbtnDefaults";
            this.mbtnDefaults.Size = new System.Drawing.Size(136, 24);
            this.mbtnDefaults.TabIndex = 7;
            this.mbtnDefaults.Text = "Score Defaults";
            this.mbtnDefaults.UseVisualStyleBackColor = false;
            this.mbtnDefaults.Click += new System.EventHandler(this.mbtnDefaults_Click);
            // 
            // radioRemoteDatabase
            // 
            this.radioRemoteDatabase.Location = new System.Drawing.Point(13, 26);
            this.radioRemoteDatabase.Name = "radioRemoteDatabase";
            this.radioRemoteDatabase.Size = new System.Drawing.Size(128, 16);
            this.radioRemoteDatabase.TabIndex = 21;
            this.radioRemoteDatabase.Text = "Remote Database";
            this.radioRemoteDatabase.CheckedChanged += new System.EventHandler(this.radioRemoteDatabase_CheckedChanged);
            // 
            // txtDataBasePath
            // 
            this.txtDataBasePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataBasePath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDataBasePath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtDataBasePath.Location = new System.Drawing.Point(54, 16);
            this.txtDataBasePath.Name = "txtDataBasePath";
            this.txtDataBasePath.Size = new System.Drawing.Size(575, 20);
            this.txtDataBasePath.TabIndex = 23;
            // 
            // lblDatabasePath
            // 
            this.lblDatabasePath.Location = new System.Drawing.Point(8, 16);
            this.lblDatabasePath.Name = "lblDatabasePath";
            this.lblDatabasePath.Size = new System.Drawing.Size(40, 20);
            this.lblDatabasePath.TabIndex = 20;
            this.lblDatabasePath.Text = "Path:";
            // 
            // radioLocalDatabase
            // 
            this.radioLocalDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioLocalDatabase.Checked = true;
            this.radioLocalDatabase.Location = new System.Drawing.Point(12, 518);
            this.radioLocalDatabase.Name = "radioLocalDatabase";
            this.radioLocalDatabase.Size = new System.Drawing.Size(64, 15);
            this.radioLocalDatabase.TabIndex = 23;
            this.radioLocalDatabase.TabStop = true;
            this.radioLocalDatabase.Text = "Local Database";
            this.radioLocalDatabase.CheckedChanged += new System.EventHandler(this.radioLocalDatabase_CheckedChanged);
            // 
            // mlblMatchMTs
            // 
            this.mlblMatchMTs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlblMatchMTs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mlblMatchMTs.Location = new System.Drawing.Point(186, 762);
            this.mlblMatchMTs.Name = "mlblMatchMTs";
            this.mlblMatchMTs.Size = new System.Drawing.Size(113, 23);
            this.mlblMatchMTs.TabIndex = 33;
            this.mlblMatchMTs.Text = "number";
            this.mlblMatchMTs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupLocalDatabase
            // 
            this.groupLocalDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLocalDatabase.Controls.Add(this.btnBrowse);
            this.groupLocalDatabase.Controls.Add(this.txtDataBasePath);
            this.groupLocalDatabase.Controls.Add(this.lblDatabasePath);
            this.groupLocalDatabase.Location = new System.Drawing.Point(31, 539);
            this.groupLocalDatabase.Name = "groupLocalDatabase";
            this.groupLocalDatabase.Size = new System.Drawing.Size(671, 45);
            this.groupLocalDatabase.TabIndex = 22;
            this.groupLocalDatabase.TabStop = false;
            this.groupLocalDatabase.Text = "Select Database";
            // 
            // mlink_showPNNLProperties
            // 
            this.mlink_showPNNLProperties.AutoSize = true;
            this.mlink_showPNNLProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlink_showPNNLProperties.Location = new System.Drawing.Point(28, 45);
            this.mlink_showPNNLProperties.Name = "mlink_showPNNLProperties";
            this.mlink_showPNNLProperties.Size = new System.Drawing.Size(158, 13);
            this.mlink_showPNNLProperties.TabIndex = 57;
            this.mlink_showPNNLProperties.TabStop = true;
            this.mlink_showPNNLProperties.Text = "Inside PNNL Network Only";
            this.mlink_showPNNLProperties.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mlink_showPNNLProperties_LinkClicked);
            // 
            // mlabel_networkConnectionTest
            // 
            this.mlabel_networkConnectionTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mlabel_networkConnectionTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_networkConnectionTest.Location = new System.Drawing.Point(10, 72);
            this.mlabel_networkConnectionTest.Name = "mlabel_networkConnectionTest";
            this.mlabel_networkConnectionTest.Size = new System.Drawing.Size(174, 18);
            this.mlabel_networkConnectionTest.TabIndex = 54;
            this.mlabel_networkConnectionTest.Text = "Testing Connection To DMS...";
            this.mlabel_networkConnectionTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mprogressBar_connectionTest
            // 
            this.mprogressBar_connectionTest.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.mprogressBar_connectionTest.ForeColor = System.Drawing.Color.Lime;
            this.mprogressBar_connectionTest.Location = new System.Drawing.Point(186, 72);
            this.mprogressBar_connectionTest.Name = "mprogressBar_connectionTest";
            this.mprogressBar_connectionTest.Size = new System.Drawing.Size(113, 18);
            this.mprogressBar_connectionTest.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.mprogressBar_connectionTest.TabIndex = 55;
            this.mprogressBar_connectionTest.Visible = false;
            // 
            // mlinkLabel_testConnection
            // 
            this.mlinkLabel_testConnection.AutoSize = true;
            this.mlinkLabel_testConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlinkLabel_testConnection.Location = new System.Drawing.Point(196, 45);
            this.mlinkLabel_testConnection.Name = "mlinkLabel_testConnection";
            this.mlinkLabel_testConnection.Size = new System.Drawing.Size(103, 13);
            this.mlinkLabel_testConnection.TabIndex = 58;
            this.mlinkLabel_testConnection.TabStop = true;
            this.mlinkLabel_testConnection.Text = "(Re-test connection)";
            // 
            // mgroupBox_pmtQualityScore
            // 
            this.mgroupBox_pmtQualityScore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_pmtQualityScore.Controls.Add(this.mlabel_pmtQualityScoreDescription);
            this.mgroupBox_pmtQualityScore.Location = new System.Drawing.Point(323, 590);
            this.mgroupBox_pmtQualityScore.Name = "mgroupBox_pmtQualityScore";
            this.mgroupBox_pmtQualityScore.Size = new System.Drawing.Size(379, 101);
            this.mgroupBox_pmtQualityScore.TabIndex = 61;
            this.mgroupBox_pmtQualityScore.TabStop = false;
            this.mgroupBox_pmtQualityScore.Text = "PMT Quality Score Description";
            // 
            // mlabel_pmtQualityScoreDescription
            // 
            this.mlabel_pmtQualityScoreDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_pmtQualityScoreDescription.Location = new System.Drawing.Point(9, 18);
            this.mlabel_pmtQualityScoreDescription.Name = "mlabel_pmtQualityScoreDescription";
            this.mlabel_pmtQualityScoreDescription.Size = new System.Drawing.Size(360, 80);
            this.mlabel_pmtQualityScoreDescription.TabIndex = 0;            
            // 
            // mnum_pmtQualityScore
            // 
            this.mnum_pmtQualityScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mnum_pmtQualityScore.Location = new System.Drawing.Point(198, 590);
            this.mnum_pmtQualityScore.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mnum_pmtQualityScore.Name = "mnum_pmtQualityScore";
            this.mnum_pmtQualityScore.Size = new System.Drawing.Size(111, 20);
            this.mnum_pmtQualityScore.TabIndex = 63;
            this.mnum_pmtQualityScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_pmtQualityScore.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mnum_pmtQualityScore.ValueChanged += new System.EventHandler(this.mnum_pmtQualityScore_ValueChanged);
            // 
            // mtextBox_experimentExclusionFilter
            // 
            this.mtextBox_experimentExclusionFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mtextBox_experimentExclusionFilter.Location = new System.Drawing.Point(501, 695);
            this.mtextBox_experimentExclusionFilter.Name = "mtextBox_experimentExclusionFilter";
            this.mtextBox_experimentExclusionFilter.Size = new System.Drawing.Size(120, 20);
            this.mtextBox_experimentExclusionFilter.TabIndex = 66;
            this.mtextBox_experimentExclusionFilter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipExclusionFilter.SetToolTip(this.mtextBox_experimentExclusionFilter, "Experiment names to exclude when loading a subset of the MTDB. ");
            // 
            // mnum_minNumberMSMSObservations
            // 
            this.mnum_minNumberMSMSObservations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mnum_minNumberMSMSObservations.Location = new System.Drawing.Point(198, 716);
            this.mnum_minNumberMSMSObservations.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mnum_minNumberMSMSObservations.Name = "mnum_minNumberMSMSObservations";
            this.mnum_minNumberMSMSObservations.Size = new System.Drawing.Size(111, 20);
            this.mnum_minNumberMSMSObservations.TabIndex = 68;
            this.mnum_minNumberMSMSObservations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frmDBName
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(716, 844);
            this.Controls.Add(this.mtxtPeptideProphet);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.mnum_minNumberMSMSObservations);
            this.Controls.Add(this.mtextBox_experimentExclusionFilter);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.mtextBox_experimentNameFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mnum_pmtQualityScore);
            this.Controls.Add(this.mtxtMinXCorr);
            this.Controls.Add(this.mgroupBox_pmtQualityScore);            
            
            this.Controls.Add(this.mlinkLabel_testConnection);
            this.Controls.Add(this.mlink_showPNNLProperties);
            this.Controls.Add(this.mlabel_networkConnectionTest);
            this.Controls.Add(this.mprogressBar_connectionTest);
            this.Controls.Add(this.mbtnShowMThits);
            this.Controls.Add(this.mlblMatchMTs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.mtxtMinDiscriminant);
            this.Controls.Add(this.radioRemoteDatabase);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.radioLocalDatabase);
            this.Controls.Add(this.groupLocalDatabase);
            this.Controls.Add(this.mbtnDefaults);
            this.Controls.Add(this.groupRemoteDatabase);
            this.Controls.Add(this.mbtn_cancel);
            this.Controls.Add(this.mbtn_ok);            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(410, 661);
            this.Name = "frmDBName";
            this.Text = "Select Mass Tag Database";
            this.Load += new System.EventHandler(this.frmDBName_Load);
            this.groupRemoteDatabase.ResumeLayout(false);
            this.groupRemoteDatabase.PerformLayout();
            this.groupLocalDatabase.ResumeLayout(false);
            this.groupLocalDatabase.PerformLayout();
            this.mgroupBox_pmtQualityScore.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pmtQualityScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minNumberMSMSObservations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
        #endregion

        #region Mass Tag Database Loading
        /// <summary>
        /// Loads the mass tag database names.
        /// </summary>
        private void LoadMassTagDBNames()
		{
			try
			{
				string cString = String.Format("database={0};server={1};user id={2};Password={3}", 
                                                CONST_MASS_TAG_DATABASE_PATH,
					                            CONST_MASS_TAG_SERVER_PATH,
                                                mobj_massTagServerInformation.Username,
                                                mobj_massTagServerInformation.Password);

				string sqlCmmnd = "GetAllMassTagDatabases" ;

				//string cString = "Persist Security Info=False;Integrated Security=SSPI;database=DMS5;server=gigasax";
				SqlConnection myConnection = new SqlConnection(cString);
				myConnection.Open();

				SqlParameter incUnused   = new SqlParameter();
				// Specify the name of the argument
				incUnused.ParameterName  = "@IncludeUnused";
				// Specify the SQL data type of the argument
				incUnused.SqlDbType      = SqlDbType.TinyInt;
				// Specify the value passed as argument
				if (inclUnused)
					incUnused.Value = 1 ;
				else
					incUnused.Value = 0 ;

				SqlCommand myCommand = new SqlCommand(sqlCmmnd,myConnection);
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(incUnused);
				myCommand.CommandType = CommandType.StoredProcedure;
				SqlDataReader myReader = myCommand.ExecuteReader();
				try 
				{
					while (myReader.Read()) 
					{
						clsMTDBentry mtdbentry = new clsMTDBentry() ;

						mtdbentry.dBaseName = myReader.GetString(0) ;
						mtdbentry.dBaseDesc = myReader.GetString(1) ;
						mtdbentry.server = myReader.GetString(7) ;
						marrMTDBInfo.Add(mtdbentry) ;
						mlstBoxDBNames.Items.Add(mtdbentry.dBaseName) ;
					}
					mlblTotalDBs.Text = marrMTDBInfo.Count.ToString() + " Databases available." ;
					mlblDBdescription.Text = "" ;
					mlblServer.Text = "" ;
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message +  "POGO get information error") ;  
				}
				finally 
				{
					// always call Close when done reading.
					myReader.Close();
					// always call Close when done reading.
					myConnection.Close();
				}
			}
			catch (Exception ex)
			{
                System.Diagnostics.Trace.WriteLine(ex.Message);
			}
		}
        /// <summary>
        /// Gets the mass tag match count.
        /// </summary>
        /// <returns></returns>
		private int GetMassTagMatchCount()
		{			
			int count = 0 ;
			if (defaults.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL)
			{
				string cString = String.Format("database={0};server={1};user id={2};Password={3}", 
					                        mobj_massTagServerInformation.DatabaseName,
                                            mobj_massTagServerInformation.ServerName,
                                            mobj_massTagServerInformation.Username,
                                            mobj_massTagServerInformation.Password);

				string sqlCmmnd = "GetMassTagMatchCount" ;

				SqlConnection myConnection = new SqlConnection(cString);
				try
				{
					myConnection.Open();
				}
				catch (SqlException ex)
				{
					Console.WriteLine(ex.Message +  "DB get information error") ;  
				}
				SqlCommand myCommand = new SqlCommand(sqlCmmnd,myConnection);
				myCommand.CommandType = CommandType.StoredProcedure;
				
				SqlParameter MinHighNormalizedScore = new SqlParameter();
				// Specify the name of the argument
				MinHighNormalizedScore.ParameterName = "@MinimumHighNormalizedScore";
				// Specify the SQL data type of the argument
				MinHighNormalizedScore.SqlDbType = SqlDbType.Float;
				// Specify the value passed as argument
				MinHighNormalizedScore.Value = MinXCorr ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinHighNormalizedScore);

				SqlParameter MinimumPMTQualityScore = new SqlParameter();
				// Specify the name of the argument
				MinimumPMTQualityScore.ParameterName = "@MinimumPMTQualityScore";
				// Specify the SQL data type of the argument
				MinimumPMTQualityScore.SqlDbType = SqlDbType.Decimal;
				// Specify the value passed as argument
				MinimumPMTQualityScore.Value = MinPMTScore ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinimumPMTQualityScore);

				SqlParameter MinimumHighDiscriminantScore = new SqlParameter();
				// Specify the name of the argument
				MinimumHighDiscriminantScore.ParameterName = "@MinimumHighDiscriminantScore";
				// Specify the SQL data type of the argument
				MinimumHighDiscriminantScore.SqlDbType = SqlDbType.Real;
				// Specify the value passed as argument
				MinimumHighDiscriminantScore.Value = MinDiscriminant ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinimumHighDiscriminantScore);

                SqlParameter experimentFilter   = new SqlParameter();
                experimentFilter.ParameterName  = "@ExperimentFilter";
                experimentFilter.SqlDbType      = SqlDbType.VarChar;
                experimentFilter.Value          = ExperimentFilter;
                myCommand.Parameters.Add(experimentFilter);

                SqlParameter experimentExclusionFilter = new SqlParameter();
                experimentExclusionFilter.ParameterName = "@ExperimentExclusionFilter";
                experimentExclusionFilter.SqlDbType = SqlDbType.VarChar;
                experimentExclusionFilter.Value = ExperimentExclusionFilter;
                myCommand.Parameters.Add(experimentExclusionFilter);

				SqlParameter MinimumPeptideProphetProbability = new SqlParameter();
				// Specify the name of the argument
				MinimumPeptideProphetProbability.ParameterName = "@MinimumPeptideProphetProbability";
				// Specify the SQL data type of the argument
				MinimumPeptideProphetProbability.SqlDbType = SqlDbType.Real;
				// Specify the value passed as argument
				MinimumPeptideProphetProbability.Value = MinPeptideProphet ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinimumPeptideProphetProbability);
				
				SqlDataReader myReader = null ;
				try 
				{
                    
					myReader = myCommand.ExecuteReader();	                    
					while (myReader.Read()) 
					{
                        count = (int)myReader.GetValue(0);
					}					
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message +  "DB get information error") ;  
				}
				finally 
				{
					// always call Close when done reading.
					myReader.Close();
					// always call Close when done reading.
					myConnection.Close();
				}
			}
			else
			{
                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}", txtDataBasePath.Text);
				OleDbConnection myConnection = new OleDbConnection(connectionString);
				string commandName = "GetMassTagMatchCount" ;

				try
				{
					myConnection.Open();
				}
				catch (OleDbException ex)
				{                   
                    Console.WriteLine(ex.Message + "DB get information error");                    
					return 0;
				}

				OleDbCommand myCommand = new OleDbCommand(commandName,myConnection);
				myCommand.CommandType = CommandType.StoredProcedure;
				
				OleDbParameter MinHighNormalizedScore = new OleDbParameter();
				// Specify the name of the argument
				MinHighNormalizedScore.ParameterName = "@MinimumHighNormalizedScore";
				// Specify the OleDb data type of the argument
				MinHighNormalizedScore.OleDbType = OleDbType.Single;
				// Specify the value passed as argument
				MinHighNormalizedScore.Value = MinXCorr ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinHighNormalizedScore);

				OleDbParameter MinimumPMTQualityScore = new OleDbParameter();
				// Specify the name of the argument
				MinimumPMTQualityScore.ParameterName = "@MinimumPMTQualityScore";
				// Specify the OleDb data type of the argument
				MinimumPMTQualityScore.OleDbType = OleDbType.Decimal;
				// Specify the value passed as argument
				MinimumPMTQualityScore.Value = MinPMTScore ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinimumPMTQualityScore);

				OleDbParameter MinimumHighDiscriminantScore = new OleDbParameter();
				// Specify the name of the argument
				MinimumHighDiscriminantScore.ParameterName = "@MinimumHighDiscriminantScore";
				// Specify the OleDb data type of the argument
				MinimumHighDiscriminantScore.OleDbType = OleDbType.Single;
				// Specify the value passed as argument
				MinimumHighDiscriminantScore.Value = MinDiscriminant ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinimumHighDiscriminantScore);

				OleDbParameter MinimumPeptideProphetProbability = new OleDbParameter();
				// Specify the name of the argument
				MinimumPeptideProphetProbability.ParameterName = "@MinimumPeptideProphetProbability";
				// Specify the OleDb data type of the argument
				MinimumPeptideProphetProbability.OleDbType = OleDbType.Single;
				// Specify the value passed as argument
				MinimumPeptideProphetProbability.Value = MinPeptideProphet ;
				// Once the argument is ready, add it to the list of arguments
				myCommand.Parameters.Add(MinimumPeptideProphetProbability);
				
				OleDbDataReader myReader = null ;
				try 
				{
					myReader = myCommand.ExecuteReader();					
					while (myReader.Read()) 
					{
						count++; 
					}
					//mlblMatchMTs.Text = count.ToString() ;
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message +  "DB get information error") ;  
				}
				finally 
				{
					// always call Close when done reading.
					if (myReader !=null)
						myReader.Close();
				
					// always call Close when done reading.
					if (myConnection != null)
						myConnection.Close();
				}
			}
			return count;
        }
        #endregion

        private void SaveOptions()
        {
            Properties.Settings.Default.UserDBFormIncludeUnusedDBs  = chkBoxUnused.Checked;
            Properties.Settings.Default.UserDBFormPMTQuality        = Convert.ToDouble(MinPMTScore);
            Properties.Settings.Default.UserDBFormMinXCorr = Convert.ToDouble(MinXCorr);
            Properties.Settings.Default.UserDBFormMinDiscriminant = Convert.ToDouble(MinDiscriminant);
            Properties.Settings.Default.UserDBFormPeptideProphetProbability = Convert.ToDouble(MinPeptideProphet);
            Properties.Settings.Default.UserDBFormLocalDatabasePath = txtDataBasePath.Text;
            Properties.Settings.Default.UserDBFormExperimentExclusionFilter = mtextBox_experimentExclusionFilter.Text;
            Properties.Settings.Default.UserDBFormExperimentFilter = mtextBox_experimentNameFilter.Text;
            Properties.Settings.Default.UserDBFormMinObservationCountFilter = MinimumNumberMSMSObservations;
            Properties.Settings.Default.Save();
        }

        #region Event Handlers 
        private void mbtn_ok_Click(object sender, System.EventArgs e)
		{
			if (radioRemoteDatabase.Checked == true)
			{
				if (mlstBoxDBNames.SelectedIndex != -1)
				{

                    string value = Convert.ToString(mlstBoxDBNames.SelectedItem);
                    AddMassTagDatabaseToRecents(value);

					DialogResult = DialogResult.OK ;
                    SaveOptions();
					Hide() ;                     
				}
				else
				{
					MessageBox.Show("Select a MassTag DataBase first.","Select MT DB") ;
					
					mbtnShowMThits.Enabled = false;
					mlblDBdescription.Enabled = false;						
					mlblDBdescription.Text = "None selected";
				}
			}
			else
			{
				if (System.IO.File.Exists(txtDataBasePath.Text) == false)
				{
					MessageBox.Show("The database file does not exist.", "Select MT DB");

					mbtnShowMThits.Enabled = false;
					mlblDBdescription.Enabled = false;						
					mlblDBdescription.Text = "None selected";
				}
				else
				{
                    DialogResult = DialogResult.OK;
                    SaveOptions();
					Hide();
				}
			}            
		}
		private void mbtn_cancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel ; 
			this.Hide() ; 		
		}
		private void chkBoxUnused_CheckedChanged(object sender, System.EventArgs e)
		{
			inclUnused = chkBoxUnused.Checked ;
			marrMTDBInfo.Clear() ;
			mlstBoxDBNames.Items.Clear() ;
			if (mobj_connectionTester.HasDMSConnection == true)
			{
				LoadMassTagDBNames() ;
			}
			mlblDBdescription.Text      = "None selected" ;
			mlblDBdescription.Enabled   = false ;
			mlblServer.Text             = "" ;
			mlblMatchMTs.Text           = "" ;
			mlblMatchMTs.Enabled        = false ;
			mlblSeverTag.Enabled        = false ;			
		}
		private void mlstBoxDBNames_SelectedIndexChanged(object sender, System.EventArgs e)		
        {
            /// 
            /// Select the database found.
            /// 
            SelectDatabaseFromIndex(mlstBoxDBNames.SelectedIndex);
        }
        /// <summary>
        /// Retrieves the PMT Quality score from the selected database.
        /// </summary>
        /// <param name="server">Server to look at for the database</param>
        /// <param name="database">Database to read from.</param>
        private void LoadPMTQualityScoreInformation(string server, string database)
        {
            try
            {
                string cString = String.Format("database={0};server={1};user id={2};Password={3}",
                                                database,
                                                server,
                                                mobj_massTagServerInformation.Username,
                                                mobj_massTagServerInformation.Password);

                string sqlCmmnd  = "SELECT PMT_Quality_Score_Value, Filter_Set_Name, ";
                sqlCmmnd        += "Filter_Set_Description, Filter_Set_ID, Experiment_Filter, ";
                sqlCmmnd        += "Instrument_Class_Filter FROM V_Filter_Set_Overview";

                //string cString = "Persist Security Info=False;Integrated Security=SSPI;database=DMS5;server=gigasax";
                SqlConnection myConnection  = new SqlConnection(cString);
                myConnection.Open();
                
                SqlCommand myCommand        = new SqlCommand(sqlCmmnd, myConnection);
                SqlDataReader myReader      = myCommand.ExecuteReader();

                Dictionary<int, string> scores = new Dictionary<int,string>();
                if (mdict_pmtQualityScoreDescriptions.ContainsKey(database) == true)
                {
                    mdict_pmtQualityScoreDescriptions[database].Clear();
                }

                try
                {
                    scores.Add(0, "");

                    while (myReader.Read())
                    {
                        
                        int     score       = Convert.ToInt32(myReader.GetString(0));
                        string  description = myReader.GetString(2);

                        /// 
                        /// Just to make sure that the PMT quality score description gets synched with the 
                        /// score of 0 that allows for all things to be considered
                        /// 
                        if (scores.ContainsKey(0) && score == 0)
                            scores[0] = description;
                        else
                            scores.Add(score, description);
                    }
                    
                }
                catch
                {
                    
                }
                finally
                {
                    // always call Close when done reading.
                    myReader.Close();
                    // always call Close when done reading.
                    myConnection.Close();

                    mdict_pmtQualityScoreDescriptions[database] = scores;
                }
            }
            catch
            {                
            }
        }
        /// <summary>
        /// Selects a database from the index provided.
        /// </summary>
        /// <param name="index">Index of the database name found.</param>
        /// <returns>True if success.  False if failed.</returns>
        private bool SelectDatabaseFromIndex(int index)
        {
			if (index != -1)
			{
				mlblSeverTag.Enabled        = true ;
				mlblServer.Text             = mobj_massTagServerInformation.ServerName ;
				mbtnShowMThits.Enabled      = true ;
				mlblMatchMTs.Text           = "" ;				
				mlblDBdescription.Enabled   = true ;

				mobj_massTagServerInformation.DatabaseName  = ((clsMTDBentry)marrMTDBInfo[index]).dBaseName ;
				mobj_massTagServerInformation.ServerName    = ((clsMTDBentry)marrMTDBInfo[index]).server ;
				mlblDBdescription.Text                      = ((clsMTDBentry)marrMTDBInfo[index]).dBaseDesc ;
                LoadExperimentDescriptions();

                /// 
                /// If the user has already selected this database, then we want to use the information we 
                /// already cached.
                /// 
                if (mdict_pmtQualityScoreDescriptions.ContainsKey(mobj_massTagServerInformation.DatabaseName) == false)
                {                 
                    LoadPMTQualityScoreInformation( mobj_massTagServerInformation.ServerName,
                                                        mobj_massTagServerInformation.DatabaseName);                    
                }
                DisplayPMTQualityScoreDescription();
                return true;
			}
            return false;
		}
		private void mbtnShowMThits_Click(object sender, System.EventArgs e)
		{
			mbtnShowMThits.Enabled = false ;
            mlblMatchMTs.BackColor = Color.Gray;
            mlblMatchMTs.Refresh();

			int count = GetMassTagMatchCount() ;
			mlblMatchMTs.Enabled = true ;
			mlblMatchMTs.Text = count.ToString() ;
			mbtnShowMThits.Enabled = true ;

            mlblMatchMTs.BackColor = Color.White;
            mlblMatchMTs.Refresh();
		}
		private void mbtnDefaults_Click(object sender, System.EventArgs e)
		{
			MinPMTScore                 = defaults.mdecimalMinPMTScore ;
			MinXCorr                    = defaults.mfltMinXCorr ;
			MinDiscriminant             = defaults.mdblMinDiscriminant ;
			MinPeptideProphet           = defaults.mdblPeptideProphetVal ;
            ExperimentExclusionFilter   = defaults.mstrExperimentExclusionFilter;
            ExperimentFilter            = defaults.mstrExperimentFilter;
		}
		private void ScoreChanged_event(object sender, System.EventArgs e)
		{
			mbtnShowMThits.Enabled = true ;
		}
		private void frmDBName_Load(object sender, System.EventArgs e)
		{
			mbtnShowMThits.Enabled = false ;
            

            if (mbool_connectionToDMSExists == false)
            {
                mobj_connectionTester.TestConnection();
            }

            if (radioRemoteDatabase.Checked)
                defaults.menm_databaseType = MultiAlignEngine.MassTags.MassTagDatabaseType.SQL;
            else
                defaults.menm_databaseType = MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS;

            LoadRecentDatabaseNames();
		}
		private void radioRemoteDatabase_CheckedChanged(object sender, System.EventArgs e)
		{
            defaults.menm_databaseType = MassTagDatabaseType.SQL;

            groupRemoteDatabase.Enabled = true;
            mtextBox_experimentExclusionFilter.Enabled = true;
            mtextBox_experimentNameFilter.Enabled = true;
			groupLocalDatabase.Enabled = false;


            mlabel_pmtQualityScoreDescription.Enabled   = true;            
            mnum_pmtQualityScore.Enabled                = true;

			//int index = marrMTDBInfo.Count - mlstBoxDBNames.SelectedIndex - 1;
			int index = mlstBoxDBNames.SelectedIndex ;

			if (index != -1)
			{
				mlblSeverTag.Enabled        = true ;
				mlblServer.Text             = mobj_massTagServerInformation.ServerName ;
				mbtnShowMThits.Enabled      = true ;
				mlblMatchMTs.Text           = "" ;
				mlblDBdescription.Enabled   = true ;

				mobj_massTagServerInformation.DatabaseName  = ((clsMTDBentry)marrMTDBInfo[index]).dBaseName ;
				mobj_massTagServerInformation.ServerName    = ((clsMTDBentry)marrMTDBInfo[index]).server ;
				mlblDBdescription.Text = ((clsMTDBentry)marrMTDBInfo[index]).dBaseDesc ;

                LoadExperimentDescriptions();
			}
		}

        /// <summary>
        /// Loads the database descriptions from the database name.
        /// </summary>
        private void LoadExperimentDescriptions()
        {
            string cString = String.Format("database={0};server={1};user id={2};Password={3}",
                                            mobj_massTagServerInformation.DatabaseName,
                                            mobj_massTagServerInformation.ServerName,
                                            mobj_massTagServerInformation.Username,
                                            mobj_massTagServerInformation.Password);

            /// 
            /// Kevin you wont like this but I'm ready for the refactor...
            /// 
            string sqlCmmnd = "SELECT DISTINCT Experiment from V_Analysis_Description_Updates";
            SqlConnection myConnection = new SqlConnection(cString);
            try
            {
                myConnection.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message + "DB get information error");
            }
            SqlCommand myCommand = new SqlCommand(sqlCmmnd, myConnection);
            myCommand.CommandType = CommandType.Text;            
            SqlDataReader myReader = null;

            mcomboBox_experimentNames.Items.Clear();            
            try
            {
                //TODO: Fix this!
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                   mcomboBox_experimentNames.Items.Add(myReader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "DB get information error");
            }
            finally
            {
                // always call Close when done reading.
                myReader.Close();
                // always call Close when done reading.
                myConnection.Close();
            }
        }

		private void radioLocalDatabase_CheckedChanged(object sender, System.EventArgs e)
		{
            defaults.menm_databaseType = MassTagDatabaseType.ACCESS;

			groupRemoteDatabase.Enabled = false;
            mtextBox_experimentExclusionFilter.Enabled = false;
            mtextBox_experimentNameFilter.Enabled = false;
			groupLocalDatabase.Enabled  = true;

            mlabel_pmtQualityScoreDescription.Enabled = false;
            

			if (System.IO.File.Exists(txtDataBasePath.Text) == false)
			{					
				mbtnShowMThits.Enabled = false;
				mlblDBdescription.Enabled = false;					
				mlblDBdescription.Text = "None selected";
			}
			else
			{
				
				mbtnShowMThits.Enabled = true;
				mlblDBdescription.Enabled = true;					
				mlblDBdescription.Text = txtDataBasePath.Text;
				
			}
        }
        /// <summary>
        /// Displays the PMT quality score found.
        /// </summary>
        private void DisplayPMTQualityScoreDescription()
        {
            Dictionary<int, string> descriptions = null;                        
            if (mdict_pmtQualityScoreDescriptions.ContainsKey(mobj_massTagServerInformation.DatabaseName) == false)
            {
                mlabel_pmtQualityScoreDescription.Text = "Cannot find description.";
                return;
            }

            descriptions = mdict_pmtQualityScoreDescriptions[mobj_massTagServerInformation.DatabaseName];

            /// 
            /// Here we want to limit the PMT Quality score to the minimum found in the database.
            /// This allows the user to select a valid score.
            /// 
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (int score in descriptions.Keys)
            {
                max = Math.Max(max, score);
                min = Math.Min(min, score);
            }

            /// 
            /// Fit the value to the range of valid scores.
            /// 
            int currentScore = Convert.ToInt32(mnum_pmtQualityScore.Value);
            if (currentScore > max || currentScore < min)
            {
                mnum_pmtQualityScore.Value  = Convert.ToDecimal(min);
                currentScore                = min;
            }
            mnum_pmtQualityScore.Minimum = Convert.ToDecimal(min);
            mnum_pmtQualityScore.Maximum = Convert.ToDecimal(max);

            /// 
            /// Display the score
            /// 
            
            if (descriptions.ContainsKey(currentScore) == false)
            {
                mlabel_pmtQualityScoreDescription.Text = "The quality score for this value is not defined.";
                return;
            }
            mlabel_pmtQualityScoreDescription.Text = descriptions[currentScore];
        }
		private void btnBrowse_Click(object sender, System.EventArgs e)
		{
			mbtnShowMThits.Enabled = false;
			mlblDBdescription.Enabled = false;				
			mlblDBdescription.Text = "None selected" ;

			OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Access Databases (*.mdb)|*.mdb|XAMT Tab Delimited (*.txt)|*.txt; ";
			dialog.FileName = "";						
			if (DialogResult.OK  == dialog.ShowDialog())
			{
				txtDataBasePath.Text   = dialog.FileName;
				mlblDBdescription.Text = dialog.FileName;
				mbtnShowMThits.Enabled    = true;
				mlblDBdescription.Enabled = true;				
			}			
		}				

		public  clsMassTagDatabaseOptions MassTagDatabaseOptions
		{
			get
			{
				MultiAlignEngine.MassTags.clsMassTagDatabaseOptions options = new MultiAlignEngine.MassTags.clsMassTagDatabaseOptions() ; 
				if (radioRemoteDatabase.Checked == true)
				{
					options.mstrDatabase        = mobj_massTagServerInformation.DatabaseName ; 
					options.mstrServer          = mobj_massTagServerInformation.ServerName ;
					options.menm_databaseType   = MultiAlignEngine.MassTags.MassTagDatabaseType.SQL;
				}
				else
				{
					options.mstrDatabase            = "";
					options.mstrServer              = "";
					options.mstr_databaseFilePath   = txtDataBasePath.Text;
					options.menm_databaseType       = MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS;
				}
                options.mstrExperimentExclusionFilter   = ExperimentExclusionFilter;
                options.mstrExperimentFilter            = ExperimentFilter;
                options.mintMinObservationCountFilter = MinimumNumberMSMSObservations;
				options.mdecimalMinPMTScore     = MinPMTScore ;
				options.mfltMinXCorr            = MinXCorr ; 
				options.mdblMinDiscriminant     = MinDiscriminant ; 
				options.mdblPeptideProphetVal   = MinPeptideProphet ; 
				return options ; 
			}
			set
			{
				if (value.mstrDatabase != null)
					mobj_massTagServerInformation.DatabaseName = value.mstrDatabase ; 
				else
                    mobj_massTagServerInformation.DatabaseName = ""; 


				if (value.mstr_databaseFilePath != null)
					txtDataBasePath.Text = value.mstr_databaseFilePath;
				else
					txtDataBasePath.Text = "";

				if (value.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.None ||
					value.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL)
				{
					radioLocalDatabase.Checked = false;
					radioRemoteDatabase.Checked = true;
					groupLocalDatabase.Enabled = false;
					groupRemoteDatabase.Enabled = true;
                    mtextBox_experimentNameFilter.Enabled       = true;
                    mtextBox_experimentExclusionFilter.Enabled  = true;
				}
				else
				{
					radioLocalDatabase.Checked  = true;
					radioRemoteDatabase.Checked = false;
					groupLocalDatabase.Enabled = true;
                    groupRemoteDatabase.Enabled = false;
                    mtextBox_experimentNameFilter.Enabled       = false;
                    mtextBox_experimentExclusionFilter.Enabled  = false;
				}
				
                mobj_massTagServerInformation.ServerName = value.mstrServer;
				MinPMTScore         = value.mdecimalMinPMTScore ; 
				MinXCorr            = value.mfltMinXCorr ; 
				MinDiscriminant     = value.mdblMinDiscriminant ; 
				MinPeptideProphet   = value.mdblPeptideProphetVal ;
                ExperimentFilter            = value.mstrExperimentFilter;
                ExperimentExclusionFilter   = value.mstrExperimentExclusionFilter;
                MinimumNumberMSMSObservations = value.mintMinObservationCountFilter;
			}
		}
        private void txtDataBasePath_LostFocus(object sender, EventArgs e)
        {
            /// 
            /// If we have a local database make sure the user 
            /// entered a valid path to a database file.
            /// 
            if (radioLocalDatabase.Enabled == true)
            {
                if (System.IO.File.Exists(txtDataBasePath.Text) == false)
                {
                    mbtnShowMThits.Enabled = false;
                    mlblDBdescription.Enabled = false;
                    mlblDBdescription.Text = "None selected";
                }
            }
        }        
        #endregion

        #region "Private properties"

        /// <summary>
        /// Gets or sets the minimum number of MS/MS observations a Mass Tag must have.
        /// </summary>
        private int MinimumNumberMSMSObservations
        {
            get
            {
                return Convert.ToInt32(mnum_minNumberMSMSObservations.Value);
            }
            set
            {
                mnum_minNumberMSMSObservations.Value = Convert.ToDecimal(value);
            }
        }
        /// <summary>
        /// Gets or sets the experiment exclusion filter.
        /// </summary>
        private string ExperimentExclusionFilter
        {
            get
            {
                return mtextBox_experimentExclusionFilter.Text;
            }
            set
            {
                mtextBox_experimentExclusionFilter.Text = value;
            }
        }
        /// <summary>
        /// Gets or sets the experiment filter.
        /// </summary>
        private string ExperimentFilter
        {
            get
            {
                return mtextBox_experimentNameFilter.Text;  
            }
            set
            {
                mtextBox_experimentNameFilter.Text = value;
            }
        }
        /// <summary>
        /// Gets or sets the minimum PMT Quality score.
        /// </summary>
        private decimal MinPMTScore
		{
			get
			{

                return mnum_pmtQualityScore.Value;				
			}
			set
			{
                mnum_pmtQualityScore.Value = value;
			}
		}
        /// <summary>
        /// Gets or sets the minimum xcorr value.
        /// </summary>
		private float MinXCorr
		{
			get
			{
				if (!mtxtMinXCorr.Text.Equals(""))
					return Convert.ToSingle(mtxtMinXCorr.Text) ; 
				else
				{
					mtxtMinXCorr.Text = defaults.mfltMinXCorr.ToString() ;
					return defaults.mfltMinXCorr ;
				}
			}
			set
			{
				mtxtMinXCorr.Text = Convert.ToString(value) ; 
			}
		}
		private double MinDiscriminant
		{
			get
			{
				if (!mtxtMinDiscriminant.Text.Equals(""))
					return Convert.ToDouble(mtxtMinDiscriminant.Text) ; 
				else
				{
					mtxtMinDiscriminant.Text = defaults.mdblMinDiscriminant.ToString() ;
					return defaults.mdblMinDiscriminant ;
				}
			}
			set
			{
				mtxtMinDiscriminant.Text = Convert.ToString(value) ; 
			}
		}
		private double MinPeptideProphet
		{
			get
			{
				if (!mtxtPeptideProphet.Text.Equals(""))
					return Convert.ToDouble(mtxtPeptideProphet.Text) ; 
				else
				{
					mtxtPeptideProphet.Text = defaults.mdblPeptideProphetVal.ToString() ;
					return defaults.mdblPeptideProphetVal ;
				}
			}
			set
			{
				mtxtPeptideProphet.Text = Convert.ToString(value) ; 
			}
		}
		#endregion

        private void mlink_showPNNLProperties_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmDialogBase form = new frmDialogBase();
            PropertyGrid grid = new PropertyGrid();
            clsDMSServerInformation info = new clsDMSServerInformation(mobj_connectionTester.ServerInformation);
            grid.Dock = DockStyle.Fill;
            grid.SelectedObject = info;
            form.Controls.Add(grid);
            grid.BringToFront();
            form.Text = "DMS Server Information";
            form.Icon = Properties.Resources.MultiAlign;

            if (form.ShowDialog() == DialogResult.OK)
            {
                mobj_connectionTester.ServerInformation = info;                
            }
        }
        private void mlistbox_recentdatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index = marrMTDBInfo.Count - mlstBoxDBNames.SelectedIndex - 1;
            int index = mlistbox_recentdatabases.SelectedIndex;
            if (index != -1)
            {
                bool selectable     = false;
                string databaseName = mlistbox_recentdatabases.SelectedItem as string;

                /// 
                /// Find the selected database of select databases.
                /// 
                if (databaseName != null)
                {

                    index  = mlstBoxDBNames.Items.IndexOf(databaseName);
                    selectable = SelectDatabaseFromIndex(index);
                    if (selectable == true)
                    {
                        mlstBoxDBNames.SelectedIndex = index;
                    }
                }

                /// 
                /// Make sure the database in the recent list exists, as it may be from the unused list now.
                /// 
                if (selectable == false)
                {
                    string errorMessage;
                    if (databaseName != null)
                    {
                       errorMessage = string.Format("Could not find the database {0} in the list of found databases.",
                                                        databaseName);
                    }else
                    {
                        errorMessage = "An invalid database was selected.";
                    }
                    MessageBox.Show(errorMessage, "Invalid database name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void mnum_pmtQualityScore_ValueChanged(object sender, EventArgs e)
        {
            DisplayPMTQualityScoreDescription();
        }              				
	}
}
