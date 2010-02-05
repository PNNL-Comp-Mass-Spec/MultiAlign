using System;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

using MultiAlignEngine;
using PNNLControls;
using PNNLProteomics.Data;
using MultiAlignWin.Forms;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;

using PNNLProteomics.SMART;
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin
{
	/// <summary>
	/// Form that displays all data related to an analysis.
	/// </summary>
	public class frmDataView : Form
    {        
        #region Members        
        private Splitter splitter1;
		private Panel panelCharts;
		private TabControl mtabcontrol_data;
		private TabPage tabPageOverlayPlot;
		private MultiAlignWin.ctlClusterChart mobjClusterChart ; 
		private MultiAlignWin.ctlClusterGrid mobjClusterGrid ; 
		private clsMultiAlignAnalysis mobjAnalysis ;
		private PNNLControls.ExpandPanel expandPanelBottom;
		private bool mblnDatasetModified ;
        private string mstrCurrentFileName;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exportParametersToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem makeDataGridATabToolStripMenuItem;
        private ToolStripMenuItem saveTableToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton mtoolStrBtnSaveTable;
        private ToolStripButton mtoolstrBtnSaveMA;
        private ToolStripButton mtoolStrBtnMAsaveas;
        private TabPage mtabPage_dataSummary;
        private ctlSummaryPages mcontrol_resultSummaryPages;
        private TabPage mtabPage_analysisInformation;
        private ctlSummaryPages mcontrol_analysisInformation;
        private TabPage tabPage2;
        private ToolStripMenuItem factorsToolStripMenuItem;
        private ToolStripMenuItem modifyToolStripMenuItem;
        private ToolStripMenuItem changeOrderingToolStripMenuItem;
        private ToolStripMenuItem assignDatasetsToToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem reclusterFeaturesToolStripMenuItem;
        private ToolStripMenuItem realignDatasetsToolStripMenuItem;
        private ToolStripMenuItem selectMassTagDatabaseToolStripMenuItem;
        private ToolStripMenuItem peakMatchToMassTagDatabaseToolStripMenuItem;
        private ToolStripMenuItem saveFactorsToolStripMenuItem;
        private ToolStripMenuItem factorDefinitionsToolStripMenuItem;
        private ToolStripMenuItem factorAssignmentsToolStripMenuItem;
        private ToolStripMenuItem saveSummariesToolStripMenuItem;
        private ToolStripMenuItem globalToolStripMenuItem;
        private ToolStripMenuItem datasetToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ImageList mimageList_tabImages;
        private Panel mpanel_dataControls;
        private TabPage DataGrid;
        private TabPage mtabPage_proteinMaps;
        private TreeView mtreeView_proteinViewer;
        private ListView mlistView_proteinPeptideTable;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel mlabel_rows;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripMenuItem asIsToolStripMenuItem;
        private ToolStripMenuItem forDanteToolStripMenuItem;
        private IContainer components;
        #endregion

        #region Constructors and Initialization
        /// <summary>
        /// Default constructor for the data view.
        /// </summary>
		public frmDataView()
		{
			InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
			Closing += new CancelEventHandler(frmDataView_Closing);

			
            mblnDatasetModified = false;
            			
			mobjClusterChart                = new MultiAlignWin.ctlClusterChart() ;
            mobjClusterChart.LegendVisible  = true;
			mobjClusterChart.Dock           = DockStyle.Fill ;
            
            mobjClusterGrid                 = new MultiAlignWin.ctlClusterGrid() ; 
			mobjClusterGrid.ProteinsMapped += new ctlClusterGrid.DelegatePeptidesMatchedToProteins(mobjClusterGrid_ProteinsMapped);
            mtreeView_proteinViewer.Nodes.Add("Global Proteins Identified");
            mlistView_proteinPeptideTable.SelectedIndexChanged += new EventHandler(mlistView_proteinPeptideTable_SelectedIndexChanged);
			tabPageOverlayPlot.Controls.Add(mobjClusterChart) ; 

			mobjClusterGrid.Dock            = DockStyle.Fill ;
            mobjClusterGrid.Visible         = true;
			expandPanelBottom.Controls.Add(mobjClusterGrid) ; 

			mobjClusterGrid.mevntExpressionPlotOpenClicked  +=  new MethodInvoker(ExpressionPlotOpenClicked);
			mobjClusterGrid.mevntHeatMapClicked             +=  new MethodInvoker(ClustergramOpenClicked);
			mobjClusterGrid.mevntScatterPlotOpenClicked     +=  new MethodInvoker(ScatterPlotOpenClicked);			
			Closing                                         +=  new CancelEventHandler(frmDataView_Closing);
            /// 
            /// Context menu created when analysis is set for the cluster grid.
            /// 
            optionsToolStripMenuItem.Click                  +=  new EventHandler(mnu_Options_Click);

            /// 
            /// Make the data grid a tab
            /// 
            makeDataGridATabToolStripMenuItem.Checked         = true;
            makeDataGridATabToolStripMenuItem.CheckedChanged += new EventHandler(makeDataGridATabToolStripMenuItem_CheckedChanged);

            this.saveTableToolStripMenuItem.Click   += new EventHandler(mobjClusterGrid.SaveTableDelegateMethod);
            this.forDanteToolStripMenuItem.Click    += new EventHandler(mobjClusterGrid.SaveDanteDelegate);
            this.asIsToolStripMenuItem.Click        += new EventHandler(mobjClusterGrid.SaveAsIsDelegate);
            this.mtoolStrBtnSaveTable.Click         += new EventHandler(mobjClusterGrid.SaveTableDelegateMethod);

            expandPanelBottom.Visible = false;
            DataGrid.Controls.Add(mobjClusterGrid);

        }

        #endregion

        void mlistView_proteinPeptideTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mlistView_proteinPeptideTable.SelectedItems.Count < 1)
                return;

            
            try
            {
                string key = mlistView_proteinPeptideTable.SelectedItems[0].Text;
                TreeNode node = mtreeView_proteinViewer.Nodes[0].Nodes[key];
                mtreeView_proteinViewer.SelectedNode = node;
            }
            catch
            {

            }

        }
        /// <summary>
        /// Displays protein map information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proteins"></param>
        void  mobjClusterGrid_ProteinsMapped(object sender, Dictionary<string,List<string>> proteins)
        {

            mlistView_proteinPeptideTable.BeginUpdate();
            mtreeView_proteinViewer.BeginUpdate();
            mtreeView_proteinViewer.Nodes[0].Nodes.Clear();
            TreeNode rootNode = mtreeView_proteinViewer.Nodes[0];

            
            {
                /// 
                /// Iterate through the number of proteins to map out their peptides.
                /// 
                foreach (string proteinName in proteins.Keys)
                {

                    /// 
                    /// Make the table entry for how many peptides roll-up to the given protein.
                    /// 
                    ListViewItem proteinListItem = new ListViewItem();
                    proteinListItem.Text = proteinName;
                    ListViewItem.ListViewSubItem numPeptidesItem = new ListViewItem.ListViewSubItem();
                    numPeptidesItem.Text = proteins[proteinName].Count.ToString();
                    proteinListItem.SubItems.Add(numPeptidesItem);
                    mlistView_proteinPeptideTable.Items.Add(proteinListItem);

                    /// 
                    /// Display map from protein to peptide
                    /// 
                    TreeNode proteinNode = new TreeNode();
                    proteinNode.Text = proteinName;
                    foreach (string peptideName in proteins[proteinName])
                    {
                        TreeNode peptideNode = new TreeNode();
                        peptideNode.Text = peptideName;
                        proteinNode.Nodes.Add(peptideNode);
                    }
                    proteinNode.Expand();
                    proteinNode.Name = proteinName;                    
                    rootNode.Nodes.Add(proteinNode);
                }
            }
            rootNode.ExpandAll();
            mtreeView_proteinViewer.Sort();
            mlistView_proteinPeptideTable.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistView_proteinPeptideTable.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            mlistView_proteinPeptideTable.EndUpdate();
            mtreeView_proteinViewer.EndUpdate();
        }


        #region Data Grid Rendering
        /// <summary>
        /// Handles when the user clicks to change the status of the check box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void makeDataGridATabToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {

            if (makeDataGridATabToolStripMenuItem.Checked == true)
            {
                ShowDataGridInTabPage();
            }
            else
            {
                ShowDataGridInExpandPanel();
            }
        }
        /// <summary>
        /// Puts the data grid into a tab page.
        /// </summary>
        private void ShowDataGridInTabPage()
        {
            /// 
            /// Make the cluster grid invisible if the user wants.
            /// 
            if (expandPanelBottom.Controls.Contains(mobjClusterGrid) == true)
            {
                expandPanelBottom.Controls.Remove(mobjClusterGrid);
            }
            expandPanelBottom.Visible = false;

            /// 
            /// Create a new tab page with the data cluster grid.
            /// 
            TabPage tab = new TabPage();
            tab.Text = "Data Grid";
            tab.Name = "DataGrid";
            tab.Controls.Add(mobjClusterGrid);
            mobjClusterGrid.Visible = true;
            mtabcontrol_data.TabPages.Insert(1, tab);

        }
        /// <summary>
        /// Puts the data grid into the expand panel.
        /// </summary>
        private void ShowDataGridInExpandPanel()
        {
            if (mtabcontrol_data.TabPages.ContainsKey("DataGrid") == true)
            {
                mtabcontrol_data.TabPages.RemoveByKey("DataGrid");
            }
            expandPanelBottom.Controls.Add(mobjClusterGrid);
            expandPanelBottom.Visible = true;
        }
        #endregion

        #region Properties
        public ToolStrip DataViewToolStrip
        {
            get
            {
                return toolStrip1;
            }
        }      
        /// <summary>
        /// Sets the analysis object.
        /// </summary>
		public clsMultiAlignAnalysis Analysis
		{
			set
			{
				mobjAnalysis              = value ; 
				mobjClusterChart.Analysis = mobjAnalysis ;
				mobjClusterGrid.Analysis  = mobjAnalysis ; 
				mobjAnalysis.BuildFactorTree();

                mlabel_rows.Text = mobjClusterGrid.NumberOfRows.ToString() + " rows.";

                /// 
                /// Create Context Menu;
                /// 
                optionsToolStripMenuItem.DropDownItems.Clear();
                ContextMenuStrip strip = mobjClusterGrid.CreateContextMenu();
                ToolStripItem [] itemArray = new ToolStripItem[strip.Items.Count];
                strip.Items.CopyTo(itemArray, 0);
                optionsToolStripMenuItem.DropDownItems.AddRange(itemArray);

                UpdateListViews();
            }
		}
        /// <summary>
        /// Gets or sets the current filename associated with the data view.
        /// </summary>
		public string CurrentFileName
		{
			get
			{
				return mstrCurrentFileName ; 
			}
			set
			{
				mstrCurrentFileName = value ; 
			}
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {                
                AbortRenderThread();
                if (components != null)
                {
                    components.Dispose();
                }
                mobjClusterChart.Dispose();
                mobjAnalysis.Dispose();
            }
            base.Dispose(disposing);
            mlist_datasets.Clear();             
        }

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataView));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.expandPanelBottom = new PNNLControls.ExpandPanel(80);
            this.panelCharts = new System.Windows.Forms.Panel();
            this.mtabcontrol_data = new System.Windows.Forms.TabControl();
            this.tabPageOverlayPlot = new System.Windows.Forms.TabPage();
            this.DataGrid = new System.Windows.Forms.TabPage();
            this.mtabPage_dataSummary = new System.Windows.Forms.TabPage();
            this.mcontrol_resultSummaryPages = new MultiAlignWin.ctlSummaryPages();
            this.mtabPage_analysisInformation = new System.Windows.Forms.TabPage();
            this.mcontrol_analysisInformation = new MultiAlignWin.ctlSummaryPages();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mpanel_dataControls = new System.Windows.Forms.Panel();
            this.mtabPage_proteinMaps = new System.Windows.Forms.TabPage();
            this.mlistView_proteinPeptideTable = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.mtreeView_proteinViewer = new System.Windows.Forms.TreeView();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFactorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.factorDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.factorAssignmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSummariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.datasetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeDataGridATabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reclusterFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realignDatasetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectMassTagDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.peakMatchToMassTagDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.factorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeOrderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assignDatasetsToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mtoolstrBtnSaveMA = new System.Windows.Forms.ToolStripButton();
            this.mtoolStrBtnMAsaveas = new System.Windows.Forms.ToolStripButton();
            this.mtoolStrBtnSaveTable = new System.Windows.Forms.ToolStripButton();
            this.mimageList_tabImages = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mlabel_rows = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.asIsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forDanteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.expandPanelBottom)).BeginInit();
            this.panelCharts.SuspendLayout();
            this.mtabcontrol_data.SuspendLayout();
            this.mtabPage_dataSummary.SuspendLayout();
            this.mtabPage_analysisInformation.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.mtabPage_proteinMaps.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 570);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1051, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // expandPanelBottom
            // 
            this.expandPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.expandPanelBottom.HeaderRightToLeft = System.Windows.Forms.RightToLeft.No;
            this.expandPanelBottom.HeaderTextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.expandPanelBottom.Location = new System.Drawing.Point(0, 574);
            this.expandPanelBottom.Name = "expandPanelBottom";
            this.expandPanelBottom.Size = new System.Drawing.Size(1051, 100);
            this.expandPanelBottom.TabIndex = 3;
            // 
            // panelCharts
            // 
            this.panelCharts.Controls.Add(this.mtabcontrol_data);
            this.panelCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCharts.Location = new System.Drawing.Point(0, 49);
            this.panelCharts.Name = "panelCharts";
            this.panelCharts.Size = new System.Drawing.Size(1051, 521);
            this.panelCharts.TabIndex = 2;
            // 
            // mtabcontrol_data
            // 
            this.mtabcontrol_data.Controls.Add(this.tabPageOverlayPlot);
            this.mtabcontrol_data.Controls.Add(this.DataGrid);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_dataSummary);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_analysisInformation);
            this.mtabcontrol_data.Controls.Add(this.tabPage2);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_proteinMaps);
            this.mtabcontrol_data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mtabcontrol_data.Location = new System.Drawing.Point(0, 0);
            this.mtabcontrol_data.Name = "mtabcontrol_data";
            this.mtabcontrol_data.SelectedIndex = 0;
            this.mtabcontrol_data.Size = new System.Drawing.Size(1051, 521);
            this.mtabcontrol_data.TabIndex = 1;
            // 
            // tabPageOverlayPlot
            // 
            this.tabPageOverlayPlot.Location = new System.Drawing.Point(4, 22);
            this.tabPageOverlayPlot.Name = "tabPageOverlayPlot";
            this.tabPageOverlayPlot.Size = new System.Drawing.Size(1043, 495);
            this.tabPageOverlayPlot.TabIndex = 0;
            this.tabPageOverlayPlot.Text = "Feature Plot - Overlay ";
            this.tabPageOverlayPlot.UseVisualStyleBackColor = true;
            // 
            // DataGrid
            // 
            this.DataGrid.Location = new System.Drawing.Point(4, 22);
            this.DataGrid.Name = "DataGrid";
            this.DataGrid.Padding = new System.Windows.Forms.Padding(3);
            this.DataGrid.Size = new System.Drawing.Size(1043, 495);
            this.DataGrid.TabIndex = 4;
            this.DataGrid.Text = "Data Grid";
            this.DataGrid.UseVisualStyleBackColor = true;
            // 
            // mtabPage_dataSummary
            // 
            this.mtabPage_dataSummary.Controls.Add(this.mcontrol_resultSummaryPages);
            this.mtabPage_dataSummary.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_dataSummary.Name = "mtabPage_dataSummary";
            this.mtabPage_dataSummary.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_dataSummary.Size = new System.Drawing.Size(1043, 495);
            this.mtabPage_dataSummary.TabIndex = 1;
            this.mtabPage_dataSummary.Text = "Results Summary";
            this.mtabPage_dataSummary.UseVisualStyleBackColor = true;
            // 
            // mcontrol_resultSummaryPages
            // 
            this.mcontrol_resultSummaryPages.BackColor = System.Drawing.SystemColors.Control;
            this.mcontrol_resultSummaryPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_resultSummaryPages.Location = new System.Drawing.Point(3, 3);
            this.mcontrol_resultSummaryPages.Name = "mcontrol_resultSummaryPages";
            this.mcontrol_resultSummaryPages.Size = new System.Drawing.Size(1037, 489);
            this.mcontrol_resultSummaryPages.TabIndex = 1;
            // 
            // mtabPage_analysisInformation
            // 
            this.mtabPage_analysisInformation.Controls.Add(this.mcontrol_analysisInformation);
            this.mtabPage_analysisInformation.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_analysisInformation.Name = "mtabPage_analysisInformation";
            this.mtabPage_analysisInformation.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_analysisInformation.Size = new System.Drawing.Size(1043, 495);
            this.mtabPage_analysisInformation.TabIndex = 2;
            this.mtabPage_analysisInformation.Text = "Analysis Information";
            this.mtabPage_analysisInformation.UseVisualStyleBackColor = true;
            // 
            // mcontrol_analysisInformation
            // 
            this.mcontrol_analysisInformation.BackColor = System.Drawing.SystemColors.Control;
            this.mcontrol_analysisInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_analysisInformation.Location = new System.Drawing.Point(3, 3);
            this.mcontrol_analysisInformation.Name = "mcontrol_analysisInformation";
            this.mcontrol_analysisInformation.Size = new System.Drawing.Size(1037, 489);
            this.mcontrol_analysisInformation.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.mpanel_dataControls);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1043, 495);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Datasets";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // mpanel_dataControls
            // 
            this.mpanel_dataControls.AutoScroll = true;
            this.mpanel_dataControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_dataControls.Location = new System.Drawing.Point(3, 3);
            this.mpanel_dataControls.Name = "mpanel_dataControls";
            this.mpanel_dataControls.Size = new System.Drawing.Size(1037, 489);
            this.mpanel_dataControls.TabIndex = 0;
            // 
            // mtabPage_proteinMaps
            // 
            this.mtabPage_proteinMaps.Controls.Add(this.mlistView_proteinPeptideTable);
            this.mtabPage_proteinMaps.Controls.Add(this.mtreeView_proteinViewer);
            this.mtabPage_proteinMaps.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_proteinMaps.Name = "mtabPage_proteinMaps";
            this.mtabPage_proteinMaps.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_proteinMaps.Size = new System.Drawing.Size(1043, 495);
            this.mtabPage_proteinMaps.TabIndex = 5;
            this.mtabPage_proteinMaps.Text = "Protein Information";
            this.mtabPage_proteinMaps.UseVisualStyleBackColor = true;
            // 
            // mlistView_proteinPeptideTable
            // 
            this.mlistView_proteinPeptideTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistView_proteinPeptideTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.mlistView_proteinPeptideTable.GridLines = true;
            this.mlistView_proteinPeptideTable.Location = new System.Drawing.Point(484, 9);
            this.mlistView_proteinPeptideTable.Name = "mlistView_proteinPeptideTable";
            this.mlistView_proteinPeptideTable.Size = new System.Drawing.Size(551, 480);
            this.mlistView_proteinPeptideTable.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.mlistView_proteinPeptideTable.TabIndex = 1;
            this.mlistView_proteinPeptideTable.UseCompatibleStateImageBehavior = false;
            this.mlistView_proteinPeptideTable.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Proteins";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Number of Peptides";
            // 
            // mtreeView_proteinViewer
            // 
            this.mtreeView_proteinViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mtreeView_proteinViewer.Location = new System.Drawing.Point(8, 6);
            this.mtreeView_proteinViewer.Name = "mtreeView_proteinViewer";
            this.mtreeView_proteinViewer.Size = new System.Drawing.Size(466, 483);
            this.mtreeView_proteinViewer.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.factorsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1051, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveTableToolStripMenuItem,
            this.saveFactorsToolStripMenuItem,
            this.saveSummariesToolStripMenuItem,
            this.toolStripSeparator1,
            this.exportParametersToolStripMenuItem});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.MergeIndex = 0;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 2;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveToolStripMenuItem.Text = "Save Analysis";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsToolStripMenuItem.MergeIndex = 3;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveAsToolStripMenuItem.Text = "Save Analysis As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveTableToolStripMenuItem
            // 
            this.saveTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asIsToolStripMenuItem,
            this.forDanteToolStripMenuItem});
            this.saveTableToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveTableToolStripMenuItem.Image")));
            this.saveTableToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveTableToolStripMenuItem.MergeIndex = 4;
            this.saveTableToolStripMenuItem.Name = "saveTableToolStripMenuItem";
            this.saveTableToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveTableToolStripMenuItem.Text = "Save Table";            
            // 
            // saveFactorsToolStripMenuItem
            // 
            this.saveFactorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.factorDefinitionsToolStripMenuItem,
            this.factorAssignmentsToolStripMenuItem});
            this.saveFactorsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveFactorsToolStripMenuItem.MergeIndex = 5;
            this.saveFactorsToolStripMenuItem.Name = "saveFactorsToolStripMenuItem";
            this.saveFactorsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveFactorsToolStripMenuItem.Text = "Save Factors";
            // 
            // factorDefinitionsToolStripMenuItem
            // 
            this.factorDefinitionsToolStripMenuItem.Name = "factorDefinitionsToolStripMenuItem";
            this.factorDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.factorDefinitionsToolStripMenuItem.Text = "Factor Definitions";
            // 
            // factorAssignmentsToolStripMenuItem
            // 
            this.factorAssignmentsToolStripMenuItem.Name = "factorAssignmentsToolStripMenuItem";
            this.factorAssignmentsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.factorAssignmentsToolStripMenuItem.Text = "Factor Assignments";
            // 
            // saveSummariesToolStripMenuItem
            // 
            this.saveSummariesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.globalToolStripMenuItem,
            this.datasetToolStripMenuItem,
            this.toolStripSeparator2});
            this.saveSummariesToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveSummariesToolStripMenuItem.MergeIndex = 6;
            this.saveSummariesToolStripMenuItem.Name = "saveSummariesToolStripMenuItem";
            this.saveSummariesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveSummariesToolStripMenuItem.Text = "Save Summaries";
            // 
            // globalToolStripMenuItem
            // 
            this.globalToolStripMenuItem.Name = "globalToolStripMenuItem";
            this.globalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.globalToolStripMenuItem.Text = "Global";
            // 
            // datasetToolStripMenuItem
            // 
            this.datasetToolStripMenuItem.Name = "datasetToolStripMenuItem";
            this.datasetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.datasetToolStripMenuItem.Text = "Dataset";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator1.MergeIndex = 7;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);
            // 
            // exportParametersToolStripMenuItem
            // 
            this.exportParametersToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.exportParametersToolStripMenuItem.MergeIndex = 8;
            this.exportParametersToolStripMenuItem.Name = "exportParametersToolStripMenuItem";
            this.exportParametersToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exportParametersToolStripMenuItem.Text = "Export Parameters";
            this.exportParametersToolStripMenuItem.Click += new System.EventHandler(this.exportParametersToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.CheckOnClick = true;
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeDataGridATabToolStripMenuItem});
            this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.viewToolStripMenuItem.MergeIndex = 1;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // makeDataGridATabToolStripMenuItem
            // 
            this.makeDataGridATabToolStripMenuItem.MergeIndex = 2;
            this.makeDataGridATabToolStripMenuItem.Name = "makeDataGridATabToolStripMenuItem";
            this.makeDataGridATabToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.makeDataGridATabToolStripMenuItem.Text = "Make Data Grid a Tab";
            this.makeDataGridATabToolStripMenuItem.Click += new System.EventHandler(this.makeDataGridATabToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.optionsToolStripMenuItem.MergeIndex = 2;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reclusterFeaturesToolStripMenuItem,
            this.realignDatasetsToolStripMenuItem,
            this.selectMassTagDatabaseToolStripMenuItem,
            this.peakMatchToMassTagDatabaseToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // reclusterFeaturesToolStripMenuItem
            // 
            this.reclusterFeaturesToolStripMenuItem.Name = "reclusterFeaturesToolStripMenuItem";
            this.reclusterFeaturesToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.reclusterFeaturesToolStripMenuItem.Text = "Re-cluster features";
            // 
            // realignDatasetsToolStripMenuItem
            // 
            this.realignDatasetsToolStripMenuItem.Name = "realignDatasetsToolStripMenuItem";
            this.realignDatasetsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.realignDatasetsToolStripMenuItem.Text = "Re-align datasets";
            // 
            // selectMassTagDatabaseToolStripMenuItem
            // 
            this.selectMassTagDatabaseToolStripMenuItem.Name = "selectMassTagDatabaseToolStripMenuItem";
            this.selectMassTagDatabaseToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.selectMassTagDatabaseToolStripMenuItem.Text = "Select Mass Tag Database";
            // 
            // peakMatchToMassTagDatabaseToolStripMenuItem
            // 
            this.peakMatchToMassTagDatabaseToolStripMenuItem.Name = "peakMatchToMassTagDatabaseToolStripMenuItem";
            this.peakMatchToMassTagDatabaseToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.peakMatchToMassTagDatabaseToolStripMenuItem.Text = "Peak match to Mass Tag Database";
            // 
            // factorsToolStripMenuItem
            // 
            this.factorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modifyToolStripMenuItem,
            this.changeOrderingToolStripMenuItem,
            this.assignDatasetsToToolStripMenuItem});
            this.factorsToolStripMenuItem.Name = "factorsToolStripMenuItem";
            this.factorsToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.factorsToolStripMenuItem.Text = "Factors";
            // 
            // modifyToolStripMenuItem
            // 
            this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
            this.modifyToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.modifyToolStripMenuItem.Text = "Define";
            // 
            // changeOrderingToolStripMenuItem
            // 
            this.changeOrderingToolStripMenuItem.Name = "changeOrderingToolStripMenuItem";
            this.changeOrderingToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.changeOrderingToolStripMenuItem.Text = "Change Ordering";
            // 
            // assignDatasetsToToolStripMenuItem
            // 
            this.assignDatasetsToToolStripMenuItem.Name = "assignDatasetsToToolStripMenuItem";
            this.assignDatasetsToToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.assignDatasetsToToolStripMenuItem.Text = "Assign Datasets To";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mtoolstrBtnSaveMA,
            this.mtoolStrBtnMAsaveas,
            this.mtoolStrBtnSaveTable});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1051, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // mtoolstrBtnSaveMA
            // 
            this.mtoolstrBtnSaveMA.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mtoolstrBtnSaveMA.Image = ((System.Drawing.Image)(resources.GetObject("mtoolstrBtnSaveMA.Image")));
            this.mtoolstrBtnSaveMA.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mtoolstrBtnSaveMA.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.mtoolstrBtnSaveMA.MergeIndex = 4;
            this.mtoolstrBtnSaveMA.Name = "mtoolstrBtnSaveMA";
            this.mtoolstrBtnSaveMA.Size = new System.Drawing.Size(23, 22);
            this.mtoolstrBtnSaveMA.Text = "Save Analysis(.mln)";
            this.mtoolstrBtnSaveMA.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // mtoolStrBtnMAsaveas
            // 
            this.mtoolStrBtnMAsaveas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mtoolStrBtnMAsaveas.Image = ((System.Drawing.Image)(resources.GetObject("mtoolStrBtnMAsaveas.Image")));
            this.mtoolStrBtnMAsaveas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mtoolStrBtnMAsaveas.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.mtoolStrBtnMAsaveas.MergeIndex = 5;
            this.mtoolStrBtnMAsaveas.Name = "mtoolStrBtnMAsaveas";
            this.mtoolStrBtnMAsaveas.Size = new System.Drawing.Size(23, 22);
            this.mtoolStrBtnMAsaveas.Text = "Save Analysis As(.mln)";
            this.mtoolStrBtnMAsaveas.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // mtoolStrBtnSaveTable
            // 
            this.mtoolStrBtnSaveTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mtoolStrBtnSaveTable.Image = ((System.Drawing.Image)(resources.GetObject("mtoolStrBtnSaveTable.Image")));
            this.mtoolStrBtnSaveTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mtoolStrBtnSaveTable.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.mtoolStrBtnSaveTable.MergeIndex = 6;
            this.mtoolStrBtnSaveTable.Name = "mtoolStrBtnSaveTable";
            this.mtoolStrBtnSaveTable.Size = new System.Drawing.Size(23, 22);
            this.mtoolStrBtnSaveTable.Text = "&Save Data Table";
            // 
            // mimageList_tabImages
            // 
            this.mimageList_tabImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mimageList_tabImages.ImageStream")));
            this.mimageList_tabImages.TransparentColor = System.Drawing.Color.Transparent;
            this.mimageList_tabImages.Images.SetKeyName(0, "analysis.bmp");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mlabel_rows,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 674);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1051, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mlabel_rows
            // 
            this.mlabel_rows.Name = "mlabel_rows";
            this.mlabel_rows.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // asIsToolStripMenuItem
            // 
            this.asIsToolStripMenuItem.Name = "asIsToolStripMenuItem";
            this.asIsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.asIsToolStripMenuItem.Text = "As Displayed";
            // 
            // forDanteToolStripMenuItem
            // 
            this.forDanteToolStripMenuItem.Name = "forDanteToolStripMenuItem";
            this.forDanteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.forDanteToolStripMenuItem.Text = "For Dante";
            // 
            // frmDataView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1051, 696);
            this.Controls.Add(this.panelCharts);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.expandPanelBottom);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDataView";
            this.Text = "Data View";
            this.Load += new System.EventHandler(this.frmDataView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.expandPanelBottom)).EndInit();
            this.panelCharts.ResumeLayout(false);
            this.mtabcontrol_data.ResumeLayout(false);
            this.mtabPage_dataSummary.ResumeLayout(false);
            this.mtabPage_analysisInformation.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.mtabPage_proteinMaps.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        #region Previews and Dataset Information Display
        /// <summary>
        /// List of datasets information controls to render.
        /// </summary>
        private List<controlDatasetInformation> mlist_datasets;
        /// <summary>
        /// Preview rendering thread.
        /// </summary>
        private Thread mobj_renderThread;
        /// <summary>
        /// Kills the rendering thread.
        /// </summary>
        private void AbortRenderThread()
        {
            if (mobj_renderThread == null)
                return;

            try
            {
                mobj_renderThread.Abort();
            }
            catch
            {
            }
            finally
            {
                mobj_renderThread = null;            
            }
        }
        /// <summary>
        /// Updates the dataset summary controls.
        /// </summary>
        private void UpdateDatasetSummary()
        {
            if (mobjAnalysis == null) 
                return;
            if (mlist_datasets == null)
            {
                mlist_datasets = new List<controlDatasetInformation>();
            }
            else
            {
                mlist_datasets.Clear();
            }
            /// 
            /// Clear the controls that exist so we dont duplicate
            /// 
            mpanel_dataControls.Controls.Clear();
            string[] names = mobjAnalysis.UMCData.DatasetName;
            
            GC.Collect();

            for (int datasetIndex = 0; datasetIndex < names.Length; datasetIndex++)
            {
                controlDatasetInformation datasetControl = new controlDatasetInformation(mobjAnalysis, datasetIndex%names.Length);
                datasetControl.Dock = DockStyle.Top;
                mpanel_dataControls.Controls.Add(datasetControl);
                mlist_datasets.Add(datasetControl);
            }

            /// 
            /// Reverse the list so that the top objects are rendered first for previews
            /// 
            mlist_datasets.Reverse();

            /// 
            /// Start the preview rendering
            /// 
            if (mobj_renderThread != null)
                AbortRenderThread();

            ThreadStart start = new ThreadStart(PreviewThreadStart);
            mobj_renderThread = new Thread(start);
            mobj_renderThread.Start();
        }
        /// <summary>
        /// Starts rendering the preview icons.
        /// </summary>
        private void PreviewThreadStart()
        {
            foreach(controlDatasetInformation info in mlist_datasets)
            {
                info.RenderPreviews();
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Adds the SMART FDR Table to the summary view.
        /// </summary>
        /// <param name="smartResults">SMART Results.</param>
        private void AddSMARTFDRTableToSummaryView(classSMARTResults smartResults)
        {
            if (smartResults == null)
                return;

            /// 
            /// Construct a list view programmatically to add it to the tab pages.
            /// 
            ListView smartView = new ListView();
            smartView.Dock = DockStyle.Fill;
            smartView.View = View.Details;
            smartView.GridLines = true;

            smartView.BeginUpdate();

            smartView.Columns.Add("Cutoff");
            smartView.Columns.Add("Matches");
            smartView.Columns.Add("Error");
            smartView.Columns.Add("FDR");
            

            /// 
            /// Pull out the summary table from the analysis object to dump into the listview.
            /// 
            List<classSMARTFdrResult> summaries = smartResults.GetSummaries();
            ColumnHeaderAutoResizeStyle resizeStyle = ColumnHeaderAutoResizeStyle.HeaderSize;
            if (summaries != null)
            {
                /// 
                /// Iterate to add the FDR results to the results summary table 
                /// 
                foreach (classSMARTFdrResult fdr in summaries)
                {
                    ListViewItem item   = new ListViewItem();                    
                    string cutoff       = string.Format("{0:0.00}", fdr.Cutoff);
                    item.Text           = cutoff;
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, string.Format("{0:0}",     fdr.NumMatches)));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, string.Format("{0:0.00}",  fdr.Error)));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, string.Format("{0:0.00}",  fdr.FDR)));
                    
                    smartView.Items.Add(item);
                }
                /// 
                /// Since we have added data, we want the size of the columns to be of the data itself.
                /// 
                resizeStyle = ColumnHeaderAutoResizeStyle.ColumnContent;
            }
            /// 
            /// Resize the column content data.
            /// 
            foreach (ColumnHeader header in smartView.Columns) 
            {
                header.AutoResize(resizeStyle);            
            }            
            smartView.EndUpdate();

            mcontrol_resultSummaryPages.AddCustomSummary("SMART Summary Table", smartView);
        }

        /// <summary>
        /// Updates both listview with appropiate cluster information
        /// </summary>
        private void UpdateListViews()
        {
            mcontrol_resultSummaryPages.CreateSummary("Global Summary", mobjAnalysis);            
            mcontrol_resultSummaryPages.CreateSummary("UMC Data", mobjAnalysis.UMCData);
            mcontrol_resultSummaryPages.CreateSummary("Cluster Data", mobjAnalysis.UMCData.mobjClusterData);
            

            mcontrol_analysisInformation.CreateSummary("UMC Finding Options", mobjAnalysis.UMCFindingOptions);
            mcontrol_analysisInformation.CreateSummary("Alignment Options", mobjAnalysis.DefaultAlignmentOptions);
            mcontrol_analysisInformation.CreateSummary("Mass Tag Database Options (MTDB)", mobjAnalysis.MassTagDBOptions);

            /// 
            /// If the datasets were peak matched, then display this control page.
            /// 
            if (mobjAnalysis.PeakMatchingResults != null)
            {
                string peakMatchingResult = "Peak Matching Results";

                mcontrol_resultSummaryPages.CreateSummary("Peak Matching Results", mobjAnalysis.PeakMatchingResults);
                if (mobjAnalysis.UseSMART == false)
                {
                    mcontrol_resultSummaryPages.AddData(peakMatchingResult, "11-Da Shifted Number of Mass Tags Matched",
                        mobjAnalysis.PeakMatchingResultsShifted.NumMassTagsMatched.ToString());
                    mcontrol_resultSummaryPages.AddData(peakMatchingResult, "11-Da Shifted Number of Proteins Matched",
                        mobjAnalysis.PeakMatchingResultsShifted.NumProteinsMatched.ToString());
                    mcontrol_resultSummaryPages.AddData(peakMatchingResult, "11-Da Shifted Number of Matches",
                        mobjAnalysis.PeakMatchingResultsShifted.NumMatches.ToString());

                    mcontrol_resultSummaryPages.AddData("Peak Matching Results",
                                                    "FDR (11-da shift) Upper Bound",
                                                    string.Format("{0:0.00}", mobjAnalysis.FDRUpperBound));
                    mcontrol_resultSummaryPages.AddData("Peak Matching Results",
                                                    "FDR (11-da shift) Lower Bound",
                                                    string.Format("{0:0.00}", mobjAnalysis.FDRLowerBound));
                }
                else
                {
                    AddSMARTFDRTableToSummaryView(mobjAnalysis.SMARTResults);
                }
            }

            UpdateDatasetSummary();

            mcontrol_resultSummaryPages.UpdateColumnWidths();
        }
        /// <summary>
        /// Displays the analysis summary for the entire set of datasets.
        /// </summary>
        private void ShowAnalysisSummary()
        {
			frmAnalysisSummary alignmentSummary = new frmAnalysisSummary(mobjAnalysis) ; 
			alignmentSummary.DatasetModified = mblnDatasetModified ; 
			alignmentSummary.ShowDialog(this) ; 
			mblnDatasetModified = alignmentSummary.DatasetModified ;
        }
        #endregion
        
        #region Cluster Grid Event HAndlers
        private void ExpressionPlotOpenClicked()
        {
            frmIntensityDiff intensityDiffForm = new frmIntensityDiff(mobjAnalysis);
            intensityDiffForm.ShowDialog(this);
        }
        #endregion

        #region Form Event Handlers
        private void ClustergramOpenClicked()
        {
            frmClustergram heatForm = new frmClustergram();
            heatForm.SetAnalysis(mobjAnalysis);
            heatForm.ShowDialog(this);
        }
        private void ScatterPlotOpenClicked()
        {
            frmScatterPlot scatterForm = new frmScatterPlot();
            scatterForm.SetAnalysis(mobjAnalysis);
            scatterForm.ShowDialog(this);
        }
        /// <summary>
        /// Handles the key 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewChartKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                PNNLControls.ctlScatterChart chart = sender as PNNLControls.ctlScatterChart;
                Form f = chart.Parent as Form;
                if (f != null)
                    f.Close();
            }
        }
        /// <summary>
        /// Handles when the user lets go of a keyboard key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewFormKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Form f = sender as Form;
                if (f != null)
                    f.Close();
            }
        }
        private void frmDataView_Closing(object sender, CancelEventArgs e)
		{
			if (!mblnDatasetModified)
				return ; 
			DialogResult rs = MessageBox.Show("About to close file. Save Changes ?","Save Changes",
				MessageBoxButtons.YesNoCancel,MessageBoxIcon.Information);
			if (rs == DialogResult.Cancel)
			{
				e.Cancel = true ; 
				return ; 
			}
			if (rs == DialogResult.Yes)
			{
				mobjAnalysis.SerializeAnalysisToFile(mstrCurrentFileName) ; 
			}
		}		
		private void mnu_Options_Click(object sender, EventArgs e)
		{
			if (mobjAnalysis != null)
			{                
                /// 
                /// Create Context Menu;
                /// 
                optionsToolStripMenuItem.DropDownItems.Clear();
                ContextMenuStrip strip = mobjClusterGrid.CreateContextMenu();
                ToolStripItem[] itemArray = new ToolStripItem[strip.Items.Count];
                strip.Items.CopyTo(itemArray, 0);
                optionsToolStripMenuItem.DropDownItems.AddRange(itemArray);
                //if (makeDataGridATabToolStripMenuItem.Checked == true)
                //{
                //    tabControlCharts.SelectedTab = tabControlCharts.TabPages["DataGrid"];
                //}
			}
		}
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mobjAnalysis.SerializeAnalysisToFile(mobjAnalysis.PathName);
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Analysis As";
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "*.mln";
            dialog.DereferenceLinks = true;
            dialog.ValidateNames = true;
            dialog.Filter = "MultiAlign Analysis (*.mln)|*.mln|All Files (*.*)|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mobjAnalysis.SerializeAnalysisToFile(dialog.FileName);
            }
            dialog.Dispose();
        }
        private void exportParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mobjAnalysis != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mobjAnalysis.SaveParametersToFile(dialog.FileName);
                }
            }
        }
        private void makeDataGridATabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makeDataGridATabToolStripMenuItem.Checked = (makeDataGridATabToolStripMenuItem.Checked == false);
        }
        private void frmDataView_Load(object sender, EventArgs e)
        {
            if (IsMdiChild)
            {
                this.menuStrip.Visible = false;
                this.toolStrip1.Visible = false;
            }
        }
        #endregion
    }
}
