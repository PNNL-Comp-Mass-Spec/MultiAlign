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
            this.mobj_toolTabs.Size = new System.Drawing.Size(872, 525);
            this.mobj_toolTabs.TabIndex = 15;
            // 
            // tabCreateStoredQuery
            // 
            this.tabCreateStoredQuery.Controls.Add(this.btnMakeStoredQueries);
            this.tabCreateStoredQuery.Controls.Add(this.groupBox1);
            this.tabCreateStoredQuery.Location = new System.Drawing.Point(4, 22);
            this.tabCreateStoredQuery.Name = "tabCreateStoredQuery";
            this.tabCreateStoredQuery.Size = new System.Drawing.Size(864, 499);
            this.tabCreateStoredQuery.TabIndex = 0;
            this.tabCreateStoredQuery.Text = "Stored Query Creation Tool";
            // 
            // btnMakeStoredQueries
            // 
            this.btnMakeStoredQueries.Location = new System.Drawing.Point(8, 312);
            this.btnMakeStoredQueries.Name = "btnMakeStoredQueries";
            this.btnMakeStoredQueries.Size = new System.Drawing.Size(128, 24);
            this.btnMakeStoredQueries.TabIndex = 16;
            this.btnMakeStoredQueries.Text = "Make Stored Queries";
            this.btnMakeStoredQueries.Click += new System.EventHandler(this.btnMakeStoredQueries_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBrowseMassTagCount);
            this.groupBox1.Controls.Add(this.txtMassTagCount);
            this.groupBox1.Controls.Add(this.btnFindVIFC);
            this.groupBox1.Controls.Add(this.txtVIFC);
            this.groupBox1.Controls.Add(this.txtMassTagProteinNameMap);
            this.groupBox1.Controls.Add(this.txtDatabasename);
            this.groupBox1.Controls.Add(this.btnBrowseGetMassTagProteinNameMap);
            this.groupBox1.Controls.Add(this.btnOpenDB);
            this.groupBox1.Controls.Add(this.btnBrowseMassTagPepProphetStats);
            this.groupBox1.Controls.Add(this.txtMassTagsPlusPepProphetStatsWorker);
            this.groupBox1.Controls.Add(this.txtMassTagsPlusPepProphetStats);
            this.groupBox1.Controls.Add(this.btnBrowseMassTagsPlusPepProphetStats);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(864, 304);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stored Query Creation Tool";
            // 
            // btnBrowseMassTagCount
            // 
            this.btnBrowseMassTagCount.Location = new System.Drawing.Point(8, 216);
            this.btnBrowseMassTagCount.Name = "btnBrowseMassTagCount";
            this.btnBrowseMassTagCount.Size = new System.Drawing.Size(264, 24);
            this.btnBrowseMassTagCount.TabIndex = 22;
            this.btnBrowseMassTagCount.Text = "Browse GetMassTagsMatchCount";
            this.btnBrowseMassTagCount.Click += new System.EventHandler(this.btnBrowseMassTagCount_Click);
            // 
            // txtMassTagCount
            // 
            this.txtMassTagCount.Location = new System.Drawing.Point(288, 216);
            this.txtMassTagCount.Name = "txtMassTagCount";
            this.txtMassTagCount.Size = new System.Drawing.Size(560, 20);
            this.txtMassTagCount.TabIndex = 21;
            this.txtMassTagCount.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagsMatchCoun" +
                "t.txt";
            // 
            // btnFindVIFC
            // 
            this.btnFindVIFC.Location = new System.Drawing.Point(8, 88);
            this.btnFindVIFC.Name = "btnFindVIFC";
            this.btnFindVIFC.Size = new System.Drawing.Size(264, 24);
            this.btnFindVIFC.TabIndex = 20;
            this.btnFindVIFC.Text = "Browse VIFC";
            this.btnFindVIFC.Click += new System.EventHandler(this.btnFindVIFC_Click);
            // 
            // txtVIFC
            // 
            this.txtVIFC.Location = new System.Drawing.Point(288, 88);
            this.txtVIFC.Name = "txtVIFC";
            this.txtVIFC.Size = new System.Drawing.Size(560, 20);
            this.txtVIFC.TabIndex = 19;
            this.txtVIFC.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\V_IFC_Mass_Tag_to_Pr" +
                "otein_Name_Map.txt";
            // 
            // txtMassTagProteinNameMap
            // 
            this.txtMassTagProteinNameMap.Location = new System.Drawing.Point(288, 56);
            this.txtMassTagProteinNameMap.Name = "txtMassTagProteinNameMap";
            this.txtMassTagProteinNameMap.Size = new System.Drawing.Size(560, 20);
            this.txtMassTagProteinNameMap.TabIndex = 17;
            this.txtMassTagProteinNameMap.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagToProteinN" +
                "ameMap.txt";
            // 
            // txtDatabasename
            // 
            this.txtDatabasename.Location = new System.Drawing.Point(288, 16);
            this.txtDatabasename.Name = "txtDatabasename";
            this.txtDatabasename.Size = new System.Drawing.Size(560, 20);
            this.txtDatabasename.TabIndex = 16;
            this.txtDatabasename.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\MT_Macaque_P395.mdb";
            // 
            // btnBrowseGetMassTagProteinNameMap
            // 
            this.btnBrowseGetMassTagProteinNameMap.Location = new System.Drawing.Point(8, 56);
            this.btnBrowseGetMassTagProteinNameMap.Name = "btnBrowseGetMassTagProteinNameMap";
            this.btnBrowseGetMassTagProteinNameMap.Size = new System.Drawing.Size(264, 24);
            this.btnBrowseGetMassTagProteinNameMap.TabIndex = 18;
            this.btnBrowseGetMassTagProteinNameMap.Text = "Browse MassTagProteinNameMap";
            this.btnBrowseGetMassTagProteinNameMap.Click += new System.EventHandler(this.btnBrowseGetMassTagProteinNameMap_Click);
            // 
            // btnOpenDB
            // 
            this.btnOpenDB.Location = new System.Drawing.Point(8, 16);
            this.btnOpenDB.Name = "btnOpenDB";
            this.btnOpenDB.Size = new System.Drawing.Size(264, 24);
            this.btnOpenDB.TabIndex = 15;
            this.btnOpenDB.Text = "Browse Database";
            this.btnOpenDB.Click += new System.EventHandler(this.btnOpenDB_Click);
            // 
            // btnBrowseMassTagPepProphetStats
            // 
            this.btnBrowseMassTagPepProphetStats.Location = new System.Drawing.Point(8, 168);
            this.btnBrowseMassTagPepProphetStats.Name = "btnBrowseMassTagPepProphetStats";
            this.btnBrowseMassTagPepProphetStats.Size = new System.Drawing.Size(264, 24);
            this.btnBrowseMassTagPepProphetStats.TabIndex = 14;
            this.btnBrowseMassTagPepProphetStats.Text = "Browse MassTags+PepProphetStats worker";
            this.btnBrowseMassTagPepProphetStats.Click += new System.EventHandler(this.btnBrowseMassTagPepProphetStats_Click);
            // 
            // txtMassTagsPlusPepProphetStatsWorker
            // 
            this.txtMassTagsPlusPepProphetStatsWorker.Location = new System.Drawing.Point(288, 168);
            this.txtMassTagsPlusPepProphetStatsWorker.Name = "txtMassTagsPlusPepProphetStatsWorker";
            this.txtMassTagsPlusPepProphetStatsWorker.Size = new System.Drawing.Size(560, 20);
            this.txtMassTagsPlusPepProphetStatsWorker.TabIndex = 13;
            this.txtMassTagsPlusPepProphetStatsWorker.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagsPassingFi" +
                "ltersWork.txt";
            // 
            // txtMassTagsPlusPepProphetStats
            // 
            this.txtMassTagsPlusPepProphetStats.Location = new System.Drawing.Point(288, 136);
            this.txtMassTagsPlusPepProphetStats.Name = "txtMassTagsPlusPepProphetStats";
            this.txtMassTagsPlusPepProphetStats.Size = new System.Drawing.Size(560, 20);
            this.txtMassTagsPlusPepProphetStats.TabIndex = 11;
            this.txtMassTagsPlusPepProphetStats.Text = "C:\\Documents and Settings\\d3m276\\Desktop\\MA_Access_TestFiles\\GetMassTagsPlusPepPr" +
                "ophetStats.txt";
            // 
            // btnBrowseMassTagsPlusPepProphetStats
            // 
            this.btnBrowseMassTagsPlusPepProphetStats.Location = new System.Drawing.Point(8, 136);
            this.btnBrowseMassTagsPlusPepProphetStats.Name = "btnBrowseMassTagsPlusPepProphetStats";
            this.btnBrowseMassTagsPlusPepProphetStats.Size = new System.Drawing.Size(264, 24);
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
            this.tabRunStoredQuery.Size = new System.Drawing.Size(864, 499);
            this.tabRunStoredQuery.TabIndex = 1;
            this.tabRunStoredQuery.Text = "Stored Query Reader";
            // 
            // btnRunGetProphetStats
            // 
            this.btnRunGetProphetStats.Location = new System.Drawing.Point(288, 72);
            this.btnRunGetProphetStats.Name = "btnRunGetProphetStats";
            this.btnRunGetProphetStats.Size = new System.Drawing.Size(264, 24);
            this.btnRunGetProphetStats.TabIndex = 19;
            this.btnRunGetProphetStats.Text = "Run Get Protein Prophet Stats";
            // 
            // txtStoredQueryDatabase
            // 
            this.txtStoredQueryDatabase.Location = new System.Drawing.Point(288, 8);
            this.txtStoredQueryDatabase.Name = "txtStoredQueryDatabase";
            this.txtStoredQueryDatabase.Size = new System.Drawing.Size(560, 20);
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
            this.mobj_dataGrid.Location = new System.Drawing.Point(0, 115);
            this.mobj_dataGrid.Name = "mobj_dataGrid";
            this.mobj_dataGrid.Size = new System.Drawing.Size(864, 384);
            this.mobj_dataGrid.TabIndex = 15;
            // 
            // btnRunProteinNamedMap
            // 
            this.btnRunProteinNamedMap.Location = new System.Drawing.Point(8, 72);
            this.btnRunProteinNamedMap.Name = "btnRunProteinNamedMap";
            this.btnRunProteinNamedMap.Size = new System.Drawing.Size(264, 24);
            this.btnRunProteinNamedMap.TabIndex = 14;
            this.btnRunProteinNamedMap.Text = "Run Get Protein Name Map Stored Query";
            this.btnRunProteinNamedMap.Click += new System.EventHandler(this.btnRunProteinNamedMap_Click);
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(0, 525);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(872, 16);
            this.status.TabIndex = 16;
            this.status.Text = "Ready.";
            // 
            // frmTestMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(872, 541);
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
			clsAccessMassTagDBStoredQueryReader query = new clsAccessMassTagDBStoredQueryReader(filename);
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
			clsMassTagDBPeptideStoredQueryCreator creator = new clsMassTagDBPeptideStoredQueryCreator(txtDatabasename.Text);			
			bool val = creator.CreateGetMassTagToProteinNameMapFromFile(
				txtMassTagProteinNameMap.Text, 
				txtVIFC.Text );						
			val = val && creator.CreateGetMassTagsPlusPepProphetStatsFromFile(
				txtMassTagsPlusPepProphetStats.Text,
				txtMassTagsPlusPepProphetStatsWorker.Text);		
			val = val && creator.CreateGetMassTagsMatchCountFromFile(txtMassTagCount.Text);
			if (val == false)
				status.Text = "Creating Stored Queries Failed.";
			else
				status.Text = "Creating Stored Queries Success.";
				
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

	}
}
