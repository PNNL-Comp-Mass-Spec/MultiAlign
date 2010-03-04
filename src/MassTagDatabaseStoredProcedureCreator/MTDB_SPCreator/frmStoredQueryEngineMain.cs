using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

/// 
/// Data Type mappings
///		http://www.carlprothman.net/Technology/DataTypeMapping/tabid/97/Default.aspx
/// 

namespace MassTagDatabaseStoredQueryCreator
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmTestMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl mobj_toolTabs;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtVIFC;
		private System.Windows.Forms.TextBox txtMassTagProteinNameMap;
		private System.Windows.Forms.TextBox txtDatabasename;
		private System.Windows.Forms.Button btnBrowseGetMassTagProteinNameMap;
		private System.Windows.Forms.Button btnOpenDB;
		private System.Windows.Forms.TextBox txtMassTagsPlusPepProphetStatsWorker;
		private System.Windows.Forms.TextBox txtMassTagsPlusPepProphetStats;
		private System.Windows.Forms.Button btnBrowseMassTagsPlusPepProphetStats;
		private System.Windows.Forms.Button btnRunGetProphetStats;
		private System.Windows.Forms.TextBox txtStoredQueryDatabase;
		private System.Windows.Forms.Button btnRunProteinNamedMap;
		private System.Windows.Forms.Button btnRunBrowseDatabase;
		private System.Windows.Forms.DataGrid mobj_dataGrid;
		private System.Windows.Forms.TabPage tabCreateStoredQuery;
		private System.Windows.Forms.Button btnMakeStoredQueries;
		private System.Windows.Forms.Button btnFindVIFC;
		private System.Windows.Forms.Button btnBrowseMassTagPepProphetStats;
		private System.Windows.Forms.TabPage tabRunStoredQuery;
		private System.Windows.Forms.StatusBar status;
		private System.Windows.Forms.Button btnBrowseMassTagCount;
		private System.Windows.Forms.TextBox txtMassTagCount;
        private RadioButton access2007RadioButton;
        private RadioButton access2003RadioButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmTestMain()
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
				if (components != null) 
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
            this.mobj_toolTabs = new System.Windows.Forms.TabControl();
            this.tabCreateStoredQuery = new System.Windows.Forms.TabPage();
            this.btnMakeStoredQueries = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowseMassTagCount = new System.Windows.Forms.Button();
            this.txtMassTagCount = new System.Windows.Forms.TextBox();
            this.btnFindVIFC = new System.Windows.Forms.Button();
            this.txtVIFC = new System.Windows.Forms.TextBox();
            this.txtMassTagProteinNameMap = new System.Windows.Forms.TextBox();
            this.txtDatabasename = new System.Windows.Forms.TextBox();
            this.btnBrowseGetMassTagProteinNameMap = new System.Windows.Forms.Button();
            this.btnOpenDB = new System.Windows.Forms.Button();
            this.btnBrowseMassTagPepProphetStats = new System.Windows.Forms.Button();
            this.txtMassTagsPlusPepProphetStatsWorker = new System.Windows.Forms.TextBox();
            this.txtMassTagsPlusPepProphetStats = new System.Windows.Forms.TextBox();
            this.btnBrowseMassTagsPlusPepProphetStats = new System.Windows.Forms.Button();
            this.tabRunStoredQuery = new System.Windows.Forms.TabPage();
            this.btnRunGetProphetStats = new System.Windows.Forms.Button();
            this.txtStoredQueryDatabase = new System.Windows.Forms.TextBox();
            this.btnRunBrowseDatabase = new System.Windows.Forms.Button();
            this.mobj_dataGrid = new System.Windows.Forms.DataGrid();
            this.btnRunProteinNamedMap = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.StatusBar();
            this.access2003RadioButton = new System.Windows.Forms.RadioButton();
            this.access2007RadioButton = new System.Windows.Forms.RadioButton();
            this.mobj_toolTabs.SuspendLayout();
            this.tabCreateStoredQuery.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabRunStoredQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mobj_dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mobj_toolTabs
            // 
            this.mobj_toolTabs.Controls.Add(this.tabCreateStoredQuery);
            this.mobj_toolTabs.Controls.Add(this.tabRunStoredQuery);
            this.mobj_toolTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mobj_toolTabs.Location = new System.Drawing.Point(0, 0);
            this.mobj_toolTabs.Name = "mobj_toolTabs";
            this.mobj_toolTabs.SelectedIndex = 0;
            this.mobj_toolTabs.Size = new System.Drawing.Size(837, 441);
            this.mobj_toolTabs.TabIndex = 15;
            // 
            // tabCreateStoredQuery
            // 
            this.tabCreateStoredQuery.Controls.Add(this.groupBox1);
            this.tabCreateStoredQuery.Location = new System.Drawing.Point(4, 22);
            this.tabCreateStoredQuery.Name = "tabCreateStoredQuery";
            this.tabCreateStoredQuery.Size = new System.Drawing.Size(829, 415);
            this.tabCreateStoredQuery.TabIndex = 0;
            this.tabCreateStoredQuery.Text = "Stored Query Creation Tool";
            // 
            // btnMakeStoredQueries
            // 
            this.btnMakeStoredQueries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMakeStoredQueries.Location = new System.Drawing.Point(580, 385);
            this.btnMakeStoredQueries.Name = "btnMakeStoredQueries";
            this.btnMakeStoredQueries.Size = new System.Drawing.Size(241, 24);
            this.btnMakeStoredQueries.TabIndex = 16;
            this.btnMakeStoredQueries.Text = "Make Stored Queries";
            this.btnMakeStoredQueries.Click += new System.EventHandler(this.btnMakeStoredQueries_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.access2007RadioButton);
            this.groupBox1.Controls.Add(this.access2003RadioButton);
            this.groupBox1.Controls.Add(this.btnMakeStoredQueries);
            this.groupBox1.Controls.Add(this.btnBrowseMassTagCount);
            this.groupBox1.Controls.Add(this.txtMassTagCount);
            this.groupBox1.Controls.Add(this.txtVIFC);
            this.groupBox1.Controls.Add(this.btnFindVIFC);
            this.groupBox1.Controls.Add(this.txtMassTagProteinNameMap);
            this.groupBox1.Controls.Add(this.btnBrowseMassTagPepProphetStats);
            this.groupBox1.Controls.Add(this.txtDatabasename);
            this.groupBox1.Controls.Add(this.btnBrowseMassTagsPlusPepProphetStats);
            this.groupBox1.Controls.Add(this.txtMassTagsPlusPepProphetStatsWorker);
            this.groupBox1.Controls.Add(this.btnOpenDB);
            this.groupBox1.Controls.Add(this.txtMassTagsPlusPepProphetStats);
            this.groupBox1.Controls.Add(this.btnBrowseGetMassTagProteinNameMap);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(829, 415);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stored Query Creation Tool";
            // 
            // btnBrowseMassTagCount
            // 
            this.btnBrowseMassTagCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseMassTagCount.Location = new System.Drawing.Point(580, 202);
            this.btnBrowseMassTagCount.Name = "btnBrowseMassTagCount";
            this.btnBrowseMassTagCount.Size = new System.Drawing.Size(241, 24);
            this.btnBrowseMassTagCount.TabIndex = 22;
            this.btnBrowseMassTagCount.Text = "Browse GetMassTagsMatchCount";
            this.btnBrowseMassTagCount.Click += new System.EventHandler(this.btnBrowseMassTagCount_Click);
            // 
            // txtMassTagCount
            // 
            this.txtMassTagCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMassTagCount.Location = new System.Drawing.Point(8, 206);
            this.txtMassTagCount.Name = "txtMassTagCount";
            this.txtMassTagCount.Size = new System.Drawing.Size(566, 20);
            this.txtMassTagCount.TabIndex = 21;
            this.txtMassTagCount.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagsMatchCoun" +
                "t.txt";
            // 
            // btnFindVIFC
            // 
            this.btnFindVIFC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindVIFC.Location = new System.Drawing.Point(580, 114);
            this.btnFindVIFC.Name = "btnFindVIFC";
            this.btnFindVIFC.Size = new System.Drawing.Size(241, 24);
            this.btnFindVIFC.TabIndex = 20;
            this.btnFindVIFC.Text = "Browse VIFC";
            this.btnFindVIFC.Click += new System.EventHandler(this.btnFindVIFC_Click);
            // 
            // txtVIFC
            // 
            this.txtVIFC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVIFC.Location = new System.Drawing.Point(8, 114);
            this.txtVIFC.Name = "txtVIFC";
            this.txtVIFC.Size = new System.Drawing.Size(566, 20);
            this.txtVIFC.TabIndex = 19;
            this.txtVIFC.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\V_IFC_Mass_Tag_to_Pr" +
                "otein_Name_Map.txt";
            // 
            // txtMassTagProteinNameMap
            // 
            this.txtMassTagProteinNameMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMassTagProteinNameMap.Location = new System.Drawing.Point(8, 84);
            this.txtMassTagProteinNameMap.Name = "txtMassTagProteinNameMap";
            this.txtMassTagProteinNameMap.Size = new System.Drawing.Size(566, 20);
            this.txtMassTagProteinNameMap.TabIndex = 17;
            this.txtMassTagProteinNameMap.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagToProteinN" +
                "ameMap.txt";
            // 
            // txtDatabasename
            // 
            this.txtDatabasename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabasename.Location = new System.Drawing.Point(8, 19);
            this.txtDatabasename.Name = "txtDatabasename";
            this.txtDatabasename.Size = new System.Drawing.Size(566, 20);
            this.txtDatabasename.TabIndex = 16;
            this.txtDatabasename.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\MT_Macaque_P395.mdb";
            // 
            // btnBrowseGetMassTagProteinNameMap
            // 
            this.btnBrowseGetMassTagProteinNameMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseGetMassTagProteinNameMap.Location = new System.Drawing.Point(580, 82);
            this.btnBrowseGetMassTagProteinNameMap.Name = "btnBrowseGetMassTagProteinNameMap";
            this.btnBrowseGetMassTagProteinNameMap.Size = new System.Drawing.Size(241, 24);
            this.btnBrowseGetMassTagProteinNameMap.TabIndex = 18;
            this.btnBrowseGetMassTagProteinNameMap.Text = "Browse MassTagProteinNameMap";
            this.btnBrowseGetMassTagProteinNameMap.Click += new System.EventHandler(this.btnBrowseGetMassTagProteinNameMap_Click);
            // 
            // btnOpenDB
            // 
            this.btnOpenDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDB.Location = new System.Drawing.Point(580, 19);
            this.btnOpenDB.Name = "btnOpenDB";
            this.btnOpenDB.Size = new System.Drawing.Size(241, 24);
            this.btnOpenDB.TabIndex = 15;
            this.btnOpenDB.Text = "Browse Database";
            this.btnOpenDB.Click += new System.EventHandler(this.btnOpenDB_Click);
            // 
            // btnBrowseMassTagPepProphetStats
            // 
            this.btnBrowseMassTagPepProphetStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseMassTagPepProphetStats.Location = new System.Drawing.Point(580, 172);
            this.btnBrowseMassTagPepProphetStats.Name = "btnBrowseMassTagPepProphetStats";
            this.btnBrowseMassTagPepProphetStats.Size = new System.Drawing.Size(241, 24);
            this.btnBrowseMassTagPepProphetStats.TabIndex = 14;
            this.btnBrowseMassTagPepProphetStats.Text = "Browse MassTags+PepProphetStats worker";
            this.btnBrowseMassTagPepProphetStats.Click += new System.EventHandler(this.btnBrowseMassTagPepProphetStats_Click);
            // 
            // txtMassTagsPlusPepProphetStatsWorker
            // 
            this.txtMassTagsPlusPepProphetStatsWorker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMassTagsPlusPepProphetStatsWorker.Location = new System.Drawing.Point(8, 172);
            this.txtMassTagsPlusPepProphetStatsWorker.Name = "txtMassTagsPlusPepProphetStatsWorker";
            this.txtMassTagsPlusPepProphetStatsWorker.Size = new System.Drawing.Size(566, 20);
            this.txtMassTagsPlusPepProphetStatsWorker.TabIndex = 13;
            this.txtMassTagsPlusPepProphetStatsWorker.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagsPassingFi" +
                "ltersWork.txt";
            // 
            // txtMassTagsPlusPepProphetStats
            // 
            this.txtMassTagsPlusPepProphetStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMassTagsPlusPepProphetStats.Location = new System.Drawing.Point(8, 144);
            this.txtMassTagsPlusPepProphetStats.Name = "txtMassTagsPlusPepProphetStats";
            this.txtMassTagsPlusPepProphetStats.Size = new System.Drawing.Size(566, 20);
            this.txtMassTagsPlusPepProphetStats.TabIndex = 11;
            this.txtMassTagsPlusPepProphetStats.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagsPlusPepPr" +
                "ophetStats.txt";
            // 
            // btnBrowseMassTagsPlusPepProphetStats
            // 
            this.btnBrowseMassTagsPlusPepProphetStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseMassTagsPlusPepProphetStats.Location = new System.Drawing.Point(580, 144);
            this.btnBrowseMassTagsPlusPepProphetStats.Name = "btnBrowseMassTagsPlusPepProphetStats";
            this.btnBrowseMassTagsPlusPepProphetStats.Size = new System.Drawing.Size(241, 24);
            this.btnBrowseMassTagsPlusPepProphetStats.TabIndex = 12;
            this.btnBrowseMassTagsPlusPepProphetStats.Text = "Browse MassTags+PepProphetStats";
            this.btnBrowseMassTagsPlusPepProphetStats.Click += new System.EventHandler(this.btnBrowseMassTagsPlusPepProphetStats_Click);
            // 
            // tabRunStoredQuery
            // 
            this.tabRunStoredQuery.Controls.Add(this.btnRunGetProphetStats);
            this.tabRunStoredQuery.Controls.Add(this.txtStoredQueryDatabase);
            this.tabRunStoredQuery.Controls.Add(this.btnRunBrowseDatabase);
            this.tabRunStoredQuery.Controls.Add(this.mobj_dataGrid);
            this.tabRunStoredQuery.Controls.Add(this.btnRunProteinNamedMap);
            this.tabRunStoredQuery.Location = new System.Drawing.Point(4, 22);
            this.tabRunStoredQuery.Name = "tabRunStoredQuery";
            this.tabRunStoredQuery.Size = new System.Drawing.Size(829, 415);
            this.tabRunStoredQuery.TabIndex = 1;
            this.tabRunStoredQuery.Text = "Stored Query Reader";
            // 
            // btnRunGetProphetStats
            // 
            this.btnRunGetProphetStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunGetProphetStats.Location = new System.Drawing.Point(287, 34);
            this.btnRunGetProphetStats.Name = "btnRunGetProphetStats";
            this.btnRunGetProphetStats.Size = new System.Drawing.Size(264, 24);
            this.btnRunGetProphetStats.TabIndex = 19;
            this.btnRunGetProphetStats.Text = "Run Get Protein Prophet Stats";
            // 
            // txtStoredQueryDatabase
            // 
            this.txtStoredQueryDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStoredQueryDatabase.Location = new System.Drawing.Point(288, 8);
            this.txtStoredQueryDatabase.Name = "txtStoredQueryDatabase";
            this.txtStoredQueryDatabase.Size = new System.Drawing.Size(533, 20);
            this.txtStoredQueryDatabase.TabIndex = 18;
            this.txtStoredQueryDatabase.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\ExampleDBForBrian.md" +
                "b";
            // 
            // btnRunBrowseDatabase
            // 
            this.btnRunBrowseDatabase.Location = new System.Drawing.Point(8, 8);
            this.btnRunBrowseDatabase.Name = "btnRunBrowseDatabase";
            this.btnRunBrowseDatabase.Size = new System.Drawing.Size(264, 24);
            this.btnRunBrowseDatabase.TabIndex = 17;
            this.btnRunBrowseDatabase.Text = "Browse Database";
            this.btnRunBrowseDatabase.Click += new System.EventHandler(this.btnRunBrowseDatabase_Click);
            // 
            // mobj_dataGrid
            // 
            this.mobj_dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mobj_dataGrid.DataMember = "";
            this.mobj_dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.mobj_dataGrid.Location = new System.Drawing.Point(0, 64);
            this.mobj_dataGrid.Name = "mobj_dataGrid";
            this.mobj_dataGrid.Size = new System.Drawing.Size(829, 351);
            this.mobj_dataGrid.TabIndex = 15;
            // 
            // btnRunProteinNamedMap
            // 
            this.btnRunProteinNamedMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunProteinNamedMap.Location = new System.Drawing.Point(557, 34);
            this.btnRunProteinNamedMap.Name = "btnRunProteinNamedMap";
            this.btnRunProteinNamedMap.Size = new System.Drawing.Size(264, 24);
            this.btnRunProteinNamedMap.TabIndex = 14;
            this.btnRunProteinNamedMap.Text = "Run Get Protein Name Map Stored Query";
            this.btnRunProteinNamedMap.Click += new System.EventHandler(this.btnRunProteinNamedMap_Click);
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(0, 441);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(837, 16);
            this.status.TabIndex = 16;
            this.status.Text = "Ready.";
            // 
            // access2003RadioButton
            // 
            this.access2003RadioButton.AutoSize = true;
            this.access2003RadioButton.Checked = true;
            this.access2003RadioButton.Location = new System.Drawing.Point(13, 45);
            this.access2003RadioButton.Name = "access2003RadioButton";
            this.access2003RadioButton.Size = new System.Drawing.Size(133, 17);
            this.access2003RadioButton.TabIndex = 23;
            this.access2003RadioButton.TabStop = true;
            this.access2003RadioButton.Text = "Microsoft Access 2003";
            this.access2003RadioButton.UseVisualStyleBackColor = true;
            // 
            // access2007RadioButton
            // 
            this.access2007RadioButton.AutoSize = true;
            this.access2007RadioButton.Location = new System.Drawing.Point(152, 45);
            this.access2007RadioButton.Name = "access2007RadioButton";
            this.access2007RadioButton.Size = new System.Drawing.Size(133, 17);
            this.access2007RadioButton.TabIndex = 24;
            this.access2007RadioButton.TabStop = true;
            this.access2007RadioButton.Text = "Microsoft Access 2007";
            this.access2007RadioButton.UseVisualStyleBackColor = true;
            // 
            // frmTestMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(837, 457);
            this.Controls.Add(this.mobj_toolTabs);
            this.Controls.Add(this.status);
            this.Name = "frmTestMain";
            this.Text = "Stored Query Setup Engine";
            this.mobj_toolTabs.ResumeLayout(false);
            this.tabCreateStoredQuery.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabRunStoredQuery.ResumeLayout(false);
            this.tabRunStoredQuery.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mobj_dataGrid)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmTestMain());
		}


		private void btnOpenDB_Click(object sender, System.EventArgs e)
		{	
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtDatabasename.Text = dialog.FileName;
			}
		}


		private void btnBrowseGetMassTagProteinNameMap_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtMassTagProteinNameMap.Text = dialog.FileName;
			}
		}

		private void btnBrowseMassTagsPlusPepProphetStats_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtMassTagsPlusPepProphetStats.Text = dialog.FileName;
			}
		}

		private void btnRunProteinNamedMap_Click(object sender, System.EventArgs e)
		{	
			string filename = txtStoredQueryDatabase.Text;

            clsAccessMassTagDBStoredQueryReader query = new clsAccessMassTagDBStoredQueryReader(clsAccessStoredQueryCreator.GetConnectionString(AccessType, txtDatabasename.Text));
			DataSet data = query.GetMassTagToProteinNameMapData();
			mobj_dataGrid.DataSource = data;
		}

		private void btnFindVIFC_Click(object sender, System.EventArgs e)
		{		
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtVIFC.Text = dialog.FileName;
			}
		}

		private void btnBrowseMassTagPepProphetStats_Click(object sender, System.EventArgs e)
		{		
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtMassTagsPlusPepProphetStatsWorker.Text = dialog.FileName;
			}
		}

		private void btnMakeStoredQueries_Click(object sender, System.EventArgs e)
		{		
			status.Text = "";

			clsMassTagDBPeptideStoredQueryCreator creator = new clsMassTagDBPeptideStoredQueryCreator(AccessType, txtDatabasename.Text);

            try
            {
                creator.CreateGetMassTagToProteinNameMapFromFile(
                    txtMassTagProteinNameMap.Text,
                    txtVIFC.Text);

                creator.CreateGetMassTagsPlusPepProphetStatsFromFile(
                    txtMassTagsPlusPepProphetStats.Text,
                    txtMassTagsPlusPepProphetStatsWorker.Text);

                creator.CreateGetMassTagsMatchCountFromFile(txtMassTagCount.Text);
                status.Text = "Creation success.";
            }
            catch(Exception ex)
            {
                status.Text = "Could not update the database with the stored procedures.  Do you have the right database format selected?" + ex.Message;
            }
		}

		private void btnRunBrowseDatabase_Click(object sender, System.EventArgs e)
		{			
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtStoredQueryDatabase.Text = dialog.FileName;
			}
		}

		private void btnBrowseMassTagCount_Click(object sender, System.EventArgs e)
		{				
			OpenFileDialog dialog = new OpenFileDialog();
			if (DialogResult.OK == dialog.ShowDialog())
			{
				txtMassTagCount.Text = dialog.FileName;
			}
		}

        public AccessType AccessType
        {
            get
            {
                AccessType type = AccessType.Access2003;
                if (access2007RadioButton.Checked == true)
                    type = AccessType.Access2007;

                return type;
            }
            set
            {
                switch (value)
                {
                    case AccessType.Access2003 :
                        access2003RadioButton.Checked = true;
                        break;
                    case AccessType.Access2007:
                        access2007RadioButton.Checked = true;
                        break;
                }
            }
        }
	}
}
