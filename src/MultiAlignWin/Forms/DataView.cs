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


using MultiAlignWin.IO;
using MultiAlignWin.Forms;
using MultiAlignEngine.Features;
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
		private MultiAlignWin.ctlClusterChart mobjClusterChart ;
        private MultiAlignAnalysis mobjAnalysis;
		private bool mblnDatasetModified ;
        private string mstrCurrentFileName;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem saveTableToolStripMenuItem;
        private ToolStripMenuItem saveFactorsToolStripMenuItem;
        private ToolStripMenuItem factorDefinitionsToolStripMenuItem;
        private ToolStripMenuItem factorAssignmentsToolStripMenuItem;
        private ToolStripMenuItem saveSummariesToolStripMenuItem;
        private ToolStripMenuItem globalToolStripMenuItem;
        private ToolStripMenuItem datasetToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ImageList mimageList_tabImages;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel mlabel_rows;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TabControl mtabcontrol_data;
        private TabPage tabPageOverlayPlot;
        private TabPage mtabPage_dataSummary;
        private ctlSummaryPages mcontrol_resultSummaryPages;
        private TabPage mtabPage_analysisInformation;
        private ctlSummaryPages mcontrol_analysisInformation;
        private TabPage mtabPage_datasetSummary;
        private Panel mpanel_dataControls;
        private TabPage mtabPage_proteinMaps;
        private ListView mlistView_proteinPeptideTable;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private TreeView mtreeView_proteinViewer;
        private ToolStripSeparator toolStripSeparator4;
        private IContainer components;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem parametersToolStripMenuItem;
        private TabPage mtabPage_clusterPlots;
        /// <summary>
        /// Options for writing columns to table output files.
        /// </summary>
        private TableWriterColumnOptions m_columnOutputOptions;
        private controlClusterInformation mcontrol_clusterInformation;
        /// <summary>
        /// List of datasets information controls to render.
        /// </summary>
        private List<controlDatasetInformation> mlist_datasets;
        private ToolStripMenuItem filtersToolStripMenuItem;
        private ToolStripMenuItem featuresToolStripMenuItem;
        private ToolStripMenuItem clustersToolStripMenuItem;
        private Panel panel1;
        private Button mbutton_reCluster;
        /// <summary>
        /// Preview rendering thread.
        /// </summary>
        private Thread mobj_datasetRenderThread;
        private Thread mobj_clusterRenderThread;
        #endregion 


        #region Constructors and Initialization
        /// <summary>
        /// Default constructor for the data view.
        /// </summary>
		public frmDataView()
		{
			InitializeComponent();
            Initialize();

            /// 
            /// column output options            
            /// 
            m_columnOutputOptions = new TableWriterColumnOptions();
            
        }
        private void Initialize()
        {
			Closing += new CancelEventHandler(frmDataView_Closing);

			
            mblnDatasetModified = false;
            			
			mobjClusterChart                = new MultiAlignWin.ctlClusterChart() ;
            mobjClusterChart.LegendVisible  = true;
			mobjClusterChart.Dock           = DockStyle.Fill ;
            
			//TODO: Map proteins to tree view mobjClusterGrid.ProteinsMapped += new ctlClusterGrid.DelegatePeptidesMatchedToProteins(mobjClusterGrid_ProteinsMapped);

            mtreeView_proteinViewer.Nodes.Add("Global Proteins Identified");
            mlistView_proteinPeptideTable.SelectedIndexChanged += new EventHandler(mlistView_proteinPeptideTable_SelectedIndexChanged);
			tabPageOverlayPlot.Controls.Add(mobjClusterChart) ; 

			Closing                                         +=  new CancelEventHandler(frmDataView_Closing);
            
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
        private void  DisplayProteinMaps(Dictionary<string,List<string>> proteins)
        {
            if (mobjAnalysis.PeakMatchedToMassTagDB == false)
            {
                mtabcontrol_data.TabPages.Remove(mtabPage_proteinMaps);
                return; 
            }
            
            mlistView_proteinPeptideTable.BeginUpdate();
            mtreeView_proteinViewer.BeginUpdate();
            mtreeView_proteinViewer.Nodes[0].Nodes.Clear();
            TreeNode rootNode = mtreeView_proteinViewer.Nodes[0];
            
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
            rootNode.ExpandAll();
            mtreeView_proteinViewer.Sort();
            mlistView_proteinPeptideTable.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistView_proteinPeptideTable.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            mlistView_proteinPeptideTable.EndUpdate();
            mtreeView_proteinViewer.EndUpdate();
        }
        
        #region Properties  
        /// <summary>
        /// Sets the analysis object.
        /// </summary>
		public MultiAlignAnalysis Analysis
		{
			set
			{
                /// 
                /// Build factor trees and tables...
                /// 
				mobjAnalysis              = value ; 
				mobjClusterChart.Analysis = mobjAnalysis ;
				//mobjAnalysis.BuildFactorTree();

                /// 
                /// Display the protein maps if any matched
                /// 
                ProteinMapExtractor extractor = new ProteinMapExtractor();
                Dictionary<string, List<string>> proteins = extractor.ExtractProteinMaps(mobjAnalysis);
                DisplayProteinMaps(proteins);

                /// 
                /// Create Cluster data 
                /// 
                CreateClusterPlots();

                /// 
                /// Finally update all the list views...
                /// 
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
                AbortDatasetRenderThread();
                AbortClusterRenderThread();

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
            this.panelCharts = new System.Windows.Forms.Panel();
            this.mtabcontrol_data = new System.Windows.Forms.TabControl();
            this.tabPageOverlayPlot = new System.Windows.Forms.TabPage();
            this.mtabPage_analysisInformation = new System.Windows.Forms.TabPage();
            this.mcontrol_analysisInformation = new MultiAlignWin.ctlSummaryPages();
            this.mtabPage_clusterPlots = new System.Windows.Forms.TabPage();
            this.mtabPage_datasetSummary = new System.Windows.Forms.TabPage();
            this.mpanel_dataControls = new System.Windows.Forms.Panel();
            this.mtabPage_proteinMaps = new System.Windows.Forms.TabPage();
            this.mlistView_proteinPeptideTable = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.mtreeView_proteinViewer = new System.Windows.Forms.TreeView();
            this.mtabPage_dataSummary = new System.Windows.Forms.TabPage();
            this.mcontrol_resultSummaryPages = new MultiAlignWin.ctlSummaryPages();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mbutton_reCluster = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFactorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.factorDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.factorAssignmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSummariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.datasetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.saveTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clustersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mimageList_tabImages = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mlabel_rows = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelCharts.SuspendLayout();
            this.mtabcontrol_data.SuspendLayout();
            this.mtabPage_analysisInformation.SuspendLayout();
            this.mtabPage_datasetSummary.SuspendLayout();
            this.mtabPage_proteinMaps.SuspendLayout();
            this.mtabPage_dataSummary.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 549);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1046, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panelCharts
            // 
            this.panelCharts.Controls.Add(this.mtabcontrol_data);
            this.panelCharts.Controls.Add(this.panel1);
            this.panelCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCharts.Location = new System.Drawing.Point(0, 24);
            this.panelCharts.Name = "panelCharts";
            this.panelCharts.Size = new System.Drawing.Size(1046, 525);
            this.panelCharts.TabIndex = 2;
            // 
            // mtabcontrol_data
            // 
            this.mtabcontrol_data.Controls.Add(this.tabPageOverlayPlot);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_analysisInformation);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_clusterPlots);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_datasetSummary);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_proteinMaps);
            this.mtabcontrol_data.Controls.Add(this.mtabPage_dataSummary);
            this.mtabcontrol_data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mtabcontrol_data.Location = new System.Drawing.Point(0, 0);
            this.mtabcontrol_data.Name = "mtabcontrol_data";
            this.mtabcontrol_data.SelectedIndex = 0;
            this.mtabcontrol_data.Size = new System.Drawing.Size(1046, 490);
            this.mtabcontrol_data.TabIndex = 1;
            // 
            // tabPageOverlayPlot
            // 
            this.tabPageOverlayPlot.Location = new System.Drawing.Point(4, 22);
            this.tabPageOverlayPlot.Name = "tabPageOverlayPlot";
            this.tabPageOverlayPlot.Size = new System.Drawing.Size(1038, 464);
            this.tabPageOverlayPlot.TabIndex = 0;
            this.tabPageOverlayPlot.Text = "Feature Cluster Plot";
            this.tabPageOverlayPlot.UseVisualStyleBackColor = true;
            // 
            // mtabPage_analysisInformation
            // 
            this.mtabPage_analysisInformation.Controls.Add(this.mcontrol_analysisInformation);
            this.mtabPage_analysisInformation.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_analysisInformation.Name = "mtabPage_analysisInformation";
            this.mtabPage_analysisInformation.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_analysisInformation.Size = new System.Drawing.Size(1038, 464);
            this.mtabPage_analysisInformation.TabIndex = 2;
            this.mtabPage_analysisInformation.Text = "Options";
            this.mtabPage_analysisInformation.UseVisualStyleBackColor = true;
            // 
            // mcontrol_analysisInformation
            // 
            this.mcontrol_analysisInformation.BackColor = System.Drawing.SystemColors.Control;
            this.mcontrol_analysisInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mcontrol_analysisInformation.Location = new System.Drawing.Point(3, 3);
            this.mcontrol_analysisInformation.Name = "mcontrol_analysisInformation";
            this.mcontrol_analysisInformation.Size = new System.Drawing.Size(1032, 458);
            this.mcontrol_analysisInformation.TabIndex = 2;
            // 
            // mtabPage_clusterPlots
            // 
            this.mtabPage_clusterPlots.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_clusterPlots.Name = "mtabPage_clusterPlots";
            this.mtabPage_clusterPlots.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_clusterPlots.Size = new System.Drawing.Size(1038, 464);
            this.mtabPage_clusterPlots.TabIndex = 8;
            this.mtabPage_clusterPlots.Text = "Analysis Plots";
            this.mtabPage_clusterPlots.UseVisualStyleBackColor = true;
            // 
            // mtabPage_datasetSummary
            // 
            this.mtabPage_datasetSummary.AutoScroll = true;
            this.mtabPage_datasetSummary.Controls.Add(this.mpanel_dataControls);
            this.mtabPage_datasetSummary.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_datasetSummary.Name = "mtabPage_datasetSummary";
            this.mtabPage_datasetSummary.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_datasetSummary.Size = new System.Drawing.Size(1038, 464);
            this.mtabPage_datasetSummary.TabIndex = 3;
            this.mtabPage_datasetSummary.Text = "Dataset Plots";
            this.mtabPage_datasetSummary.UseVisualStyleBackColor = true;
            // 
            // mpanel_dataControls
            // 
            this.mpanel_dataControls.AutoScroll = true;
            this.mpanel_dataControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_dataControls.Location = new System.Drawing.Point(3, 3);
            this.mpanel_dataControls.Name = "mpanel_dataControls";
            this.mpanel_dataControls.Size = new System.Drawing.Size(1032, 458);
            this.mpanel_dataControls.TabIndex = 0;
            // 
            // mtabPage_proteinMaps
            // 
            this.mtabPage_proteinMaps.Controls.Add(this.mlistView_proteinPeptideTable);
            this.mtabPage_proteinMaps.Controls.Add(this.mtreeView_proteinViewer);
            this.mtabPage_proteinMaps.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_proteinMaps.Name = "mtabPage_proteinMaps";
            this.mtabPage_proteinMaps.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_proteinMaps.Size = new System.Drawing.Size(1038, 464);
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
            this.mlistView_proteinPeptideTable.Size = new System.Drawing.Size(627, 510);
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
            this.mtreeView_proteinViewer.Size = new System.Drawing.Size(466, 513);
            this.mtreeView_proteinViewer.TabIndex = 0;
            // 
            // mtabPage_dataSummary
            // 
            this.mtabPage_dataSummary.Controls.Add(this.mcontrol_resultSummaryPages);
            this.mtabPage_dataSummary.Location = new System.Drawing.Point(4, 22);
            this.mtabPage_dataSummary.Name = "mtabPage_dataSummary";
            this.mtabPage_dataSummary.Padding = new System.Windows.Forms.Padding(3);
            this.mtabPage_dataSummary.Size = new System.Drawing.Size(1038, 464);
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
            this.mcontrol_resultSummaryPages.Size = new System.Drawing.Size(1032, 458);
            this.mcontrol_resultSummaryPages.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mbutton_reCluster);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 490);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1046, 35);
            this.panel1.TabIndex = 3;
            // 
            // mbutton_reCluster
            // 
            this.mbutton_reCluster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mbutton_reCluster.Location = new System.Drawing.Point(12, 3);
            this.mbutton_reCluster.Name = "mbutton_reCluster";
            this.mbutton_reCluster.Size = new System.Drawing.Size(85, 28);
            this.mbutton_reCluster.TabIndex = 4;
            this.mbutton_reCluster.Text = "Cluster";
            this.mbutton_reCluster.UseVisualStyleBackColor = true;
            this.mbutton_reCluster.Visible = false;
            this.mbutton_reCluster.Click += new System.EventHandler(this.mbutton_reCluster_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.filtersToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1046, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveFactorsToolStripMenuItem,
            this.saveSummariesToolStripMenuItem,
            this.toolStripSeparator4,
            this.saveTableToolStripMenuItem});
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
            this.saveToolStripMenuItem.MergeIndex = 3;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveToolStripMenuItem.Text = "Save Analysis";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsToolStripMenuItem.MergeIndex = 4;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveAsToolStripMenuItem.Text = "Save Analysis As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveFactorsToolStripMenuItem
            // 
            this.saveFactorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.factorDefinitionsToolStripMenuItem,
            this.factorAssignmentsToolStripMenuItem});
            this.saveFactorsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveFactorsToolStripMenuItem.MergeIndex = 5;
            this.saveFactorsToolStripMenuItem.Name = "saveFactorsToolStripMenuItem";
            this.saveFactorsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
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
            this.saveSummariesToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveSummariesToolStripMenuItem.Text = "Save Summaries";
            // 
            // globalToolStripMenuItem
            // 
            this.globalToolStripMenuItem.Name = "globalToolStripMenuItem";
            this.globalToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.globalToolStripMenuItem.Text = "Global";
            // 
            // datasetToolStripMenuItem
            // 
            this.datasetToolStripMenuItem.Name = "datasetToolStripMenuItem";
            this.datasetToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.datasetToolStripMenuItem.Text = "Dataset";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(120, 6);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator4.MergeIndex = 7;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(163, 6);
            // 
            // saveTableToolStripMenuItem
            // 
            this.saveTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataToolStripMenuItem,
            this.parametersToolStripMenuItem});
            this.saveTableToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveTableToolStripMenuItem.MergeIndex = 8;
            this.saveTableToolStripMenuItem.Name = "saveTableToolStripMenuItem";
            this.saveTableToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveTableToolStripMenuItem.Text = "Export";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.dataToolStripMenuItem.Text = "Data";
            this.dataToolStripMenuItem.Click += new System.EventHandler(this.dataToolStripMenuItem_Click);
            // 
            // parametersToolStripMenuItem
            // 
            this.parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            this.parametersToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.parametersToolStripMenuItem.Text = "Parameters";
            this.parametersToolStripMenuItem.Click += new System.EventHandler(this.parametersToolStripMenuItem_Click);
            // 
            // filtersToolStripMenuItem
            // 
            this.filtersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.featuresToolStripMenuItem,
            this.clustersToolStripMenuItem});
            this.filtersToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Replace;
            this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            this.filtersToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.filtersToolStripMenuItem.Text = "Filters";
            // 
            // featuresToolStripMenuItem
            // 
            this.featuresToolStripMenuItem.Name = "featuresToolStripMenuItem";
            this.featuresToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.featuresToolStripMenuItem.Text = "Features";
            // 
            // clustersToolStripMenuItem
            // 
            this.clustersToolStripMenuItem.Name = "clustersToolStripMenuItem";
            this.clustersToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.clustersToolStripMenuItem.Text = "Clusters";
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 553);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1046, 22);
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
            // frmDataView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1046, 575);
            this.Controls.Add(this.panelCharts);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDataView";
            this.Text = "Data View";
            this.Load += new System.EventHandler(this.frmDataView_Load);
            this.panelCharts.ResumeLayout(false);
            this.mtabcontrol_data.ResumeLayout(false);
            this.mtabPage_analysisInformation.ResumeLayout(false);
            this.mtabPage_datasetSummary.ResumeLayout(false);
            this.mtabPage_proteinMaps.ResumeLayout(false);
            this.mtabPage_dataSummary.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        #region Previews and Dataset Information Display

        /// <summary>
        /// Kills the rendering thread.
        /// </summary>
        private void AbortDatasetRenderThread()
        {
            if (mobj_datasetRenderThread == null)
                return;

            try
            {
                mobj_datasetRenderThread.Abort();
            }
            catch
            {
            }
            finally
            {
                mobj_datasetRenderThread = null;
            }
        }
        /// <summary>
        /// Kills the rendering thread.
        /// </summary>
        private void AbortClusterRenderThread()
        {
            if (mobj_clusterRenderThread == null)
                return;

            try
            {
                mobj_clusterRenderThread.Abort();
            }
            catch
            {
            }
            finally
            {
                mobj_clusterRenderThread = null;
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
            if (mobj_datasetRenderThread != null)
                AbortDatasetRenderThread();

            if (mobj_clusterRenderThread != null)
                AbortClusterRenderThread();

            ThreadStart start = new ThreadStart(DatasetPreviewThreadStart);
            mobj_datasetRenderThread = new Thread(start);
            mobj_datasetRenderThread.Start();


            ThreadStart startCluster = new ThreadStart(ClusterPreviewThreadStart);
            mobj_clusterRenderThread = new Thread(startCluster);
            mobj_clusterRenderThread.Start();
        }
        /// <summary>
        /// Starts rendering the preview icons.
        /// </summary>
        private void DatasetPreviewThreadStart()
        {
            foreach (controlDatasetInformation info in mlist_datasets)
            {
                info.RenderPreviews();
            }
        }
        /// <summary>
        /// Starts rendering the preview icons.
        /// </summary>
        private void ClusterPreviewThreadStart()
        {            
            if (mcontrol_clusterInformation != null)
                mcontrol_clusterInformation.RenderPreviews();
        }
        #endregion

        #region Utility
        /// <summary>
        /// Adds the SMART FDR Table to the summary view.
        /// </summary>
        /// <param name="STACResults">SMART Results.</param>
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

            mcontrol_resultSummaryPages.AddCustomSummary("STAC Summary Table", smartView);
        }

        /// <summary>
        /// Updates both listview with appropiate cluster information
        /// </summary>
        private void UpdateListViews()
        {
            mcontrol_resultSummaryPages.CreateSummary("Global Summary", mobjAnalysis);            
            mcontrol_resultSummaryPages.CreateSummary("Feature Data", mobjAnalysis.UMCData);
            mcontrol_resultSummaryPages.CreateSummary("Cluster Data", mobjAnalysis.UMCData.mobjClusterData);
            

            mcontrol_analysisInformation.CreateSummary("Feature Finding Options", mobjAnalysis.UMCFindingOptions);
            mcontrol_analysisInformation.CreateSummary("Alignment Options", mobjAnalysis.DefaultAlignmentOptions);
            mcontrol_analysisInformation.CreateSummary("Mass Tag Database Options (MTDB)", mobjAnalysis.MassTagDBOptions);
            mcontrol_analysisInformation.CreateSummary("Cluster Options", mobjAnalysis.ClusterOptions); 

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
        #endregion
        
        #region Cluster Grid Event HAndlers
        private void ExpressionPlotOpenClicked()
        {
            frmIntensityDiff intensityDiffForm = new frmIntensityDiff(mobjAnalysis);
            intensityDiffForm.ShowDialog(this);
        }
        #endregion

        #region Form Event Handlers
        //private void ScatterPlotOpenClicked()
        //{
        //    //frmScatterPlot scatterForm = new frmScatterPlot();
        //    //scatterForm.SetAnalysis(mobjAnalysis);
        //    //scatterForm.ShowDialog(this);
        //}
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
        }
        private void frmDataView_Load(object sender, EventArgs e)
        {
            if (IsMdiChild)
            {
                this.menuStrip.Visible = false;
            }
        }
        #endregion

        #region Data Saving Menu Strip Item Handlers 

        private PNNLProteomics.Filters.UMCFilters m_umcFilters            = new PNNLProteomics.Filters.UMCFilters();
        private PNNLProteomics.Filters.UMCClusterFilters m_clusterFilters = new PNNLProteomics.Filters.UMCClusterFilters();


        private void Export()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Analysis As";
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "*.csv";
            dialog.DereferenceLinks = true;
            dialog.ValidateNames = true;
            dialog.Filter = "Comma Delimited (*.csv)|*.csv|Tab Delimited (*.txt)|*.txt|SQLite Database File (*.db3)|*.db3|All Files (*.*)|*.*";
            dialog.FilterIndex = 1;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                IAnalysisWriter writer = null;

                string filename = dialog.FileNames[0];
                string extension = System.IO.Path.GetExtension(filename);
                AnalysisTableWriter tableWriter = null;

                TableWriterColumnOptionsForm optionsForm = new TableWriterColumnOptionsForm();
                optionsForm.Options                      = m_columnOutputOptions;                
                optionsForm.Icon                         = this.ParentForm.Icon;
                optionsForm.StartPosition                = FormStartPosition.CenterParent;

                MultiAlignWin.Forms.Filters.DataFilters filters = new MultiAlignWin.Forms.Filters.DataFilters(m_umcFilters, m_clusterFilters);
                if (filters.ShowDialog() != DialogResult.OK)
                    return;

                switch (extension)
                {
                    case ".csv":
                        optionsForm.ShowDialog();
                        m_columnOutputOptions = optionsForm.Options;
                        tableWriter = new AnalysisTableWriter(m_columnOutputOptions);

                        tableWriter.Delimeter = ",";
                        writer = tableWriter;
                        break;
                    case ".txt":
                        optionsForm.ShowDialog();
                        m_columnOutputOptions = optionsForm.Options;
                        tableWriter = new AnalysisTableWriter(m_columnOutputOptions);

                        tableWriter.Delimeter = "\t";
                        writer = tableWriter;
                        break;
                    case ".db3":
                        writer = new AnalysisSQLiteDBWriter();
                        break;
                    default:
                        optionsForm.ShowDialog();
                        m_columnOutputOptions = optionsForm.Options;
                        tableWriter = new AnalysisTableWriter(m_columnOutputOptions);

                        tableWriter.Delimeter = ",";
                        writer = tableWriter;
                        break;
                }

                writer.WriteAnalysis(filename,
                                    mobjAnalysis, 
                                    m_umcFilters.GetFilterList(),
                                    m_clusterFilters.GetFilterList());
            }
            dialog.Dispose();
        }
        private void ExportParameters()
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
        private void parametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportParameters();
        }
        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export();
        }
        #endregion                

        /// <summary>
        /// Create clusters plots 
        /// </summary>
        private void CreateClusterPlots()
        {
            controlClusterInformation clusterInformation = new controlClusterInformation(mobjAnalysis);
            mtabPage_clusterPlots.Controls.Add(clusterInformation);
            clusterInformation.Dock = DockStyle.Fill;
            mcontrol_clusterInformation = clusterInformation;
        }

        private void mbutton_reCluster_Click(object sender, EventArgs e)
        {
            frmClusterParameters parameters = new frmClusterParameters();
            parameters.ClusterOptions = mobjAnalysis.ClusterOptions;

            // Ask the user if they want to really re-cluster the data.            
            if (parameters.ShowDialog() == DialogResult.OK)
            {
                mobjAnalysis.ClusterOptions = parameters.ClusterOptions;                
                mobjAnalysis.PerformClustering();


                // Render the preview afterwards.
                if (mobj_clusterRenderThread != null)
                    AbortClusterRenderThread();

                ThreadStart startCluster = new ThreadStart(ClusterPreviewThreadStart);
                mobj_clusterRenderThread = new Thread(startCluster);
                mobj_clusterRenderThread.Start();
            }                        
        }

        private void clusterFiltersButton_Click(object sender, EventArgs e)
        {
            MultiAlignWin.Forms.Filters.DataFilters filters = new MultiAlignWin.Forms.Filters.DataFilters(m_umcFilters, m_clusterFilters);
            if (filters.ShowDialog() == DialogResult.OK)
            {
                //TODO: something here? references should have already been updated.
            }
        }
    }
}
