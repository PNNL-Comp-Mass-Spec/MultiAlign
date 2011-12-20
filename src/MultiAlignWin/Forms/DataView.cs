using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Parameters;
using MultiAlignCustomControls.Charting;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using MultiAlignWin.Forms;
using PNNLProteomics.SMART;

namespace MultiAlignWin
{
	/// <summary>
	/// Form that displays all data related to an analysis.
	/// </summary>
	public class DataView : Form
    {

        /// <summary>
        /// Preview rendering thread.
        /// </summary>
        private Thread m_datasetRenderThread;
        private Thread m_clusterRenderThread;        
        private MultiAlignAnalysis m_analysis;


        /// <summary>
        /// List of datasets information controls to render.
        /// </summary>
        private List<controlDatasetInformation> m_datasetsDisplayControls;

        #region Members
        private Splitter splitter1;
        private Panel panelCharts;
		private ctlClusterChart mobjClusterChart ;	
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
        private ToolStripStatusLabel m_rows;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TabControl mtabcontrol_data;
        private TabPage tabPageOverlayPlot;
        private TabPage m_dataSummary;
        private ctlSummaryPages m_resultSummaryPages;
        private TabPage m_analysisInformationPage;
        private ctlSummaryPages m_analysisInformation;
        private TabPage m_datasetSummary;
        private Panel m_dataControls;
        private TabPage m_proteinMaps;
        private ListView m_proteinPeptideTable;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private TreeView m_proteinViewer;
        private ToolStripSeparator toolStripSeparator4;
        private IContainer components;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem parametersToolStripMenuItem;
        private TabPage m_clusterPlots;
        /// <summary>
        /// Options for writing columns to table output files.
        /// </summary>
        private TableWriterColumnOptions m_columnOutputOptions;
        private controlClusterInformation m_clusterInformation;
        private ToolStripMenuItem filtersToolStripMenuItem;
        private ToolStripMenuItem featuresToolStripMenuItem;
        private ToolStripMenuItem clustersToolStripMenuItem;
        private Panel panel1;
        private ToolStripSeparator toolStripSeparator1;

        private MultiAlignCore.Filters.UMCFilters m_umcFilters = new MultiAlignCore.Filters.UMCFilters();
        private MultiAlignCore.Filters.UMCClusterFilters m_clusterFilters = new MultiAlignCore.Filters.UMCClusterFilters();
        #endregion 


        #region Constructors and Initialization
        /// <summary>
        /// Default constructor for the data view.
        /// </summary>
		public DataView()
		{
			InitializeComponent();

            mobjClusterChart                = new ctlClusterChart();
            mobjClusterChart.LegendVisible  = true;
            mobjClusterChart.Dock           = DockStyle.Fill;
            
            m_proteinViewer.Nodes.Add("Global Proteins Identified");
            m_proteinPeptideTable.SelectedIndexChanged += new EventHandler(m_proteinPeptideTable_SelectedIndexChanged);
            tabPageOverlayPlot.Controls.Add(mobjClusterChart);                   

            /// 
            /// column output options            
            /// 
            m_columnOutputOptions = new TableWriterColumnOptions();            
        }        
        #endregion

        void m_proteinPeptideTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_proteinPeptideTable.SelectedItems.Count < 1)
                return;
            
            try
            {
                string key      = m_proteinPeptideTable.SelectedItems[0].Text;
                TreeNode node   = m_proteinViewer.Nodes[0].Nodes[key];
                m_proteinViewer.SelectedNode = node;
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
            if (m_analysis.PeakMatchedToMassTagDB == false)
            {
                mtabcontrol_data.TabPages.Remove(m_proteinMaps);
                return; 
            }
            
            m_proteinPeptideTable.BeginUpdate();
            m_proteinViewer.BeginUpdate();
            m_proteinViewer.Nodes[0].Nodes.Clear();
            TreeNode rootNode = m_proteinViewer.Nodes[0];
            
            // Iterate through the number of proteins to map out their peptides.
            foreach (string proteinName in proteins.Keys)
            {                
                // Make the table entry for how many peptides roll-up to the given protein.                
                ListViewItem proteinListItem = new ListViewItem();
                proteinListItem.Text = proteinName;
                ListViewItem.ListViewSubItem numPeptidesItem = new ListViewItem.ListViewSubItem();
                numPeptidesItem.Text = proteins[proteinName].Count.ToString();
                proteinListItem.SubItems.Add(numPeptidesItem);
                m_proteinPeptideTable.Items.Add(proteinListItem);

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
            m_proteinViewer.Sort();
            m_proteinPeptideTable.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            m_proteinPeptideTable.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            m_proteinPeptideTable.EndUpdate();
            m_proteinViewer.EndUpdate();
        }
        
        /// <summary>
        /// Sets the analysis object.
        /// </summary>
		public void SetAnalysis(MultiAlignAnalysis analysis)
		{			
			m_analysis = analysis; 
				
            //ProteinMapExtractor extractor               = new ProteinMapExtractor();
            //Dictionary<string, List<string>> proteins   = extractor.ExtractProteinMaps(m_analysis);
            //DisplayProteinMaps(proteins);

            CreateClusterPlots();
            UpdateListViews();
            UpdateDatasetSummary();            
		}

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
                m_analysis.Dispose();

            }
            m_datasetsDisplayControls.Clear();          
            base.Dispose(disposing);            
        }

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataView));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelCharts = new System.Windows.Forms.Panel();
            this.mtabcontrol_data = new System.Windows.Forms.TabControl();
            this.tabPageOverlayPlot = new System.Windows.Forms.TabPage();
            this.m_analysisInformationPage = new System.Windows.Forms.TabPage();
            this.m_analysisInformation = new MultiAlignWin.ctlSummaryPages();
            this.m_clusterPlots = new System.Windows.Forms.TabPage();
            this.m_datasetSummary = new System.Windows.Forms.TabPage();
            this.m_dataControls = new System.Windows.Forms.Panel();
            this.m_proteinMaps = new System.Windows.Forms.TabPage();
            this.m_proteinPeptideTable = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.m_proteinViewer = new System.Windows.Forms.TreeView();
            this.m_dataSummary = new System.Windows.Forms.TabPage();
            this.m_resultSummaryPages = new MultiAlignWin.ctlSummaryPages();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.m_rows = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panelCharts.SuspendLayout();
            this.mtabcontrol_data.SuspendLayout();
            this.m_analysisInformationPage.SuspendLayout();
            this.m_datasetSummary.SuspendLayout();
            this.m_proteinMaps.SuspendLayout();
            this.m_dataSummary.SuspendLayout();
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
            this.mtabcontrol_data.Controls.Add(this.m_analysisInformationPage);
            this.mtabcontrol_data.Controls.Add(this.m_clusterPlots);
            this.mtabcontrol_data.Controls.Add(this.m_datasetSummary);
            this.mtabcontrol_data.Controls.Add(this.m_proteinMaps);
            this.mtabcontrol_data.Controls.Add(this.m_dataSummary);
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
            // m_analysisInformation
            // 
            this.m_analysisInformationPage.Controls.Add(this.m_analysisInformation);
            this.m_analysisInformationPage.Location = new System.Drawing.Point(4, 22);
            this.m_analysisInformationPage.Name = "m_analysisInformationPage";
            this.m_analysisInformationPage.Padding = new System.Windows.Forms.Padding(3);
            this.m_analysisInformationPage.Size = new System.Drawing.Size(1038, 464);
            this.m_analysisInformationPage.TabIndex = 2;
            this.m_analysisInformationPage.Text = "Options";
            this.m_analysisInformationPage.UseVisualStyleBackColor = true;
            // 
            // m_analysisInformation
            // 
            this.m_analysisInformation.BackColor = System.Drawing.SystemColors.Control;
            this.m_analysisInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_analysisInformation.Location = new System.Drawing.Point(3, 3);
            this.m_analysisInformation.Name = "m_analysisInformation";
            this.m_analysisInformation.Size = new System.Drawing.Size(1032, 458);
            this.m_analysisInformation.TabIndex = 2;
            // 
            // m_clusterPlots
            // 
            this.m_clusterPlots.Location = new System.Drawing.Point(4, 22);
            this.m_clusterPlots.Name = "m_clusterPlots";
            this.m_clusterPlots.Padding = new System.Windows.Forms.Padding(3);
            this.m_clusterPlots.Size = new System.Drawing.Size(1038, 464);
            this.m_clusterPlots.TabIndex = 8;
            this.m_clusterPlots.Text = "Analysis Plots";
            this.m_clusterPlots.UseVisualStyleBackColor = true;
            // 
            // m_datasetSummary
            // 
            this.m_datasetSummary.AutoScroll = true;
            this.m_datasetSummary.Controls.Add(this.m_dataControls);
            this.m_datasetSummary.Location = new System.Drawing.Point(4, 22);
            this.m_datasetSummary.Name = "m_datasetSummary";
            this.m_datasetSummary.Padding = new System.Windows.Forms.Padding(3);
            this.m_datasetSummary.Size = new System.Drawing.Size(1038, 464);
            this.m_datasetSummary.TabIndex = 3;
            this.m_datasetSummary.Text = "Dataset Plots";
            this.m_datasetSummary.UseVisualStyleBackColor = true;
            // 
            // m_dataControls
            // 
            this.m_dataControls.AutoScroll = true;
            this.m_dataControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_dataControls.Location = new System.Drawing.Point(3, 3);
            this.m_dataControls.Name = "m_dataControls";
            this.m_dataControls.Size = new System.Drawing.Size(1032, 458);
            this.m_dataControls.TabIndex = 0;
            // 
            // m_proteinMaps
            // 
            this.m_proteinMaps.Controls.Add(this.m_proteinPeptideTable);
            this.m_proteinMaps.Controls.Add(this.m_proteinViewer);
            this.m_proteinMaps.Location = new System.Drawing.Point(4, 22);
            this.m_proteinMaps.Name = "m_proteinMaps";
            this.m_proteinMaps.Padding = new System.Windows.Forms.Padding(3);
            this.m_proteinMaps.Size = new System.Drawing.Size(1038, 464);
            this.m_proteinMaps.TabIndex = 5;
            this.m_proteinMaps.Text = "Protein Information";
            this.m_proteinMaps.UseVisualStyleBackColor = true;
            // 
            // m_proteinPeptideTable
            // 
            this.m_proteinPeptideTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_proteinPeptideTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.m_proteinPeptideTable.GridLines = true;
            this.m_proteinPeptideTable.Location = new System.Drawing.Point(484, 9);
            this.m_proteinPeptideTable.Name = "m_proteinPeptideTable";
            this.m_proteinPeptideTable.Size = new System.Drawing.Size(627, 510);
            this.m_proteinPeptideTable.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.m_proteinPeptideTable.TabIndex = 1;
            this.m_proteinPeptideTable.UseCompatibleStateImageBehavior = false;
            this.m_proteinPeptideTable.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Proteins";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Number of Peptides";
            // 
            // m_proteinViewer
            // 
            this.m_proteinViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.m_proteinViewer.Location = new System.Drawing.Point(8, 6);
            this.m_proteinViewer.Name = "m_proteinViewer";
            this.m_proteinViewer.Size = new System.Drawing.Size(466, 513);
            this.m_proteinViewer.TabIndex = 0;
            // 
            // m_dataSummary
            // 
            this.m_dataSummary.Controls.Add(this.m_resultSummaryPages);
            this.m_dataSummary.Location = new System.Drawing.Point(4, 22);
            this.m_dataSummary.Name = "m_dataSummary";
            this.m_dataSummary.Padding = new System.Windows.Forms.Padding(3);
            this.m_dataSummary.Size = new System.Drawing.Size(1038, 464);
            this.m_dataSummary.TabIndex = 1;
            this.m_dataSummary.Text = "Results Summary";
            this.m_dataSummary.UseVisualStyleBackColor = true;
            // 
            // m_resultSummaryPages
            // 
            this.m_resultSummaryPages.BackColor = System.Drawing.SystemColors.Control;
            this.m_resultSummaryPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_resultSummaryPages.Location = new System.Drawing.Point(3, 3);
            this.m_resultSummaryPages.Name = "m_resultSummaryPages";
            this.m_resultSummaryPages.Size = new System.Drawing.Size(1032, 458);
            this.m_resultSummaryPages.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 490);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1046, 35);
            this.panel1.TabIndex = 3;
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
            this.toolStripSeparator1,
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
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dataToolStripMenuItem.Text = "Data";
            this.dataToolStripMenuItem.Click += new System.EventHandler(this.dataToolStripMenuItem_Click);
            // 
            // parametersToolStripMenuItem
            // 
            this.parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            this.parametersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.m_rows,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 553);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1046, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // m_rows
            // 
            this.m_rows.Name = "m_rows";
            this.m_rows.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // DataView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1046, 575);
            this.Controls.Add(this.panelCharts);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataView";
            this.Text = "Data View";
            this.Load += new System.EventHandler(this.frmDataView_Load);
            this.panelCharts.ResumeLayout(false);
            this.mtabcontrol_data.ResumeLayout(false);
            this.m_analysisInformationPage.ResumeLayout(false);
            this.m_datasetSummary.ResumeLayout(false);
            this.m_proteinMaps.ResumeLayout(false);
            this.m_dataSummary.ResumeLayout(false);
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
            if (m_datasetRenderThread == null)
                return;

            try
            {
                m_datasetRenderThread.Abort();
            }
            catch
            {
            }
            finally
            {
                m_datasetRenderThread = null;
            }
        }
        /// <summary>
        /// Kills the rendering thread.
        /// </summary>
        private void AbortClusterRenderThread()
        {
            if (m_clusterRenderThread == null)
                return;

            try
            {
                m_clusterRenderThread.Abort();
            }
            catch
            {
            }
            finally
            {
                m_clusterRenderThread = null;
            }
        }
        /// <summary>
        /// Updates the dataset summary controls.
        /// </summary>
        private void UpdateDatasetSummary()
        {
            if (m_analysis == null) 
                return;

            if (m_datasetsDisplayControls == null)
            {
                m_datasetsDisplayControls = new List<controlDatasetInformation>();
            }
            else
            {
                m_datasetsDisplayControls.Clear();
            }
                        
            m_dataControls.Controls.Clear();
            for (int i = 0; i < m_analysis.MetaData.Datasets.Count; i++)
            {
                DatasetInformation  info                = m_analysis.MetaData.Datasets[i];
                classAlignmentData  alignmentData       = m_analysis.AlignmentData[i];
                clsAlignmentOptions alignmentOptions    = m_analysis.Options.DefaultAlignmentOptions;                               
                controlDatasetInformation datasetControl = new controlDatasetInformation(info,
                                                                                        alignmentData,
                                                                                        alignmentOptions,
                                                                                        m_analysis.DataProviders.FeatureCache);

                datasetControl.Dock = DockStyle.Top;
                m_dataControls.Controls.Add(datasetControl);
                m_datasetsDisplayControls.Add(datasetControl);
            }            
            m_datasetsDisplayControls.Reverse();
             
            if (m_datasetRenderThread != null)
                AbortDatasetRenderThread();

            if (m_clusterRenderThread != null)
                AbortClusterRenderThread();

            List<clsCluster> clusters = m_analysis.DataProviders.ClusterCache.FindAll();
            mobjClusterChart.AddClusters(clusters);

            ThreadStart start = new ThreadStart(DatasetPreviewThreadStart);
            m_datasetRenderThread = new Thread(start);
            m_datasetRenderThread.Start();

            ThreadStart startCluster = new ThreadStart(ClusterPreviewThreadStart);
            m_clusterRenderThread = new Thread(startCluster);
            m_clusterRenderThread.Start();
        }
        /// <summary>
        /// Starts rendering the preview icons.
        /// </summary>
        private void DatasetPreviewThreadStart()
        {
            foreach (controlDatasetInformation info in m_datasetsDisplayControls)
            {
                info.RenderPreviews();
            }
        }
        /// <summary>
        /// Starts rendering the preview icons.
        /// </summary>
        private void ClusterPreviewThreadStart()
        {
            if (m_clusterInformation != null)
            {
                m_clusterInformation.RenderPreviews();
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Adds the STAC FDR Table to the summary view.
        /// </summary>
        /// <param name="STACResults">SMART Results.</param>
        private void AddStacFdrTableToSummaryView(classSMARTResults smartResults)
        {
            if (smartResults == null)
                return;

            /// 
            /// Construct a list view programmatically to add it to the tab pages.
            /// 
            ListView smartView  = new ListView();
            smartView.Dock      = DockStyle.Fill;
            smartView.View      = View.Details;
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
                // Load FDR Table
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
                resizeStyle = ColumnHeaderAutoResizeStyle.ColumnContent;
            }
            
            foreach (ColumnHeader header in smartView.Columns) 
            {
                header.AutoResize(resizeStyle);            
            }            
            smartView.EndUpdate();

            m_resultSummaryPages.AddCustomSummary("STAC Summary Table", smartView);
        }

        /// <summary>
        /// Updates both listview with appropiate cluster information
        /// </summary>
        private void UpdateListViews()
        {
            m_resultSummaryPages.CreateSummary("Global Summary", m_analysis);            
            //m_resultSummaryPages.CreateSummary("Feature Data", mobjAnalysis.UMCData);
            //m_resultSummaryPages.CreateSummary("Cluster Data", mobjAnalysis.UMCData.mobjClusterData);

            m_analysisInformation.CreateSummary("Feature Finding Options", m_analysis.Options.FeatureFindingOptions);
            m_analysisInformation.CreateSummary("Alignment Options", m_analysis.Options.DefaultAlignmentOptions);
            m_analysisInformation.CreateSummary("Mass Tag Database Options (MTDB)", m_analysis.Options.MassTagDatabaseOptions);
            m_analysisInformation.CreateSummary("Cluster Options", m_analysis.Options.ClusterOptions); 

            /// 
            /// If the datasets were peak matched, then display this control page.
            /// 
            //if (m_analysis.PeakMatchingResults != null)
            //{
            //    string peakMatchingResult = "Peak Matching Results";

            //    m_resultSummaryPages.CreateSummary("Peak Matching Results", m_analysis.PeakMatchingResults);
            //    if (m_analysis.PeakMatchingOptions.UseSTAC == false)
            //    {
            //        m_resultSummaryPages.AddData(peakMatchingResult, "11-Da Shifted Number of Mass Tags Matched",
            //            m_analysis.PeakMatchingResultsShifted.NumMassTagsMatched.ToString());
            //        m_resultSummaryPages.AddData(peakMatchingResult, "11-Da Shifted Number of Proteins Matched",
            //            m_analysis.PeakMatchingResultsShifted.NumProteinsMatched.ToString());
            //        m_resultSummaryPages.AddData(peakMatchingResult, "11-Da Shifted Number of Matches",
            //            m_analysis.PeakMatchingResultsShifted.NumMatches.ToString());

            //        m_resultSummaryPages.AddData("Peak Matching Results",
            //                                        "FDR (11-da shift) Upper Bound",
            //                                        string.Format("{0:0.00}", m_analysis.FDRUpperBound));
            //        m_resultSummaryPages.AddData("Peak Matching Results",
            //                                        "FDR (11-da shift) Lower Bound",
            //                                        string.Format("{0:0.00}", m_analysis.FDRLowerBound));
            //    }
            //    else
            //    {
            //        AddStacFdrTableToSummaryView(m_analysis.STACResults);
            //    }
            //}            
            m_resultSummaryPages.UpdateColumnWidths();
        }
        #endregion
        
        #region Form Event Handlers
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalysisBinaryWriter writer = new AnalysisBinaryWriter();
            writer.WriteAnalysis(AnalysisPathUtils.BuildAnalysisName(m_analysis.MetaData.AnalysisPath, m_analysis.MetaData.AnalysisName + ".mln"), m_analysis);
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
                AnalysisBinaryWriter writer = new AnalysisBinaryWriter();
                writer.WriteAnalysis(dialog.FileName, m_analysis);
            }
            dialog.Dispose();
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
        private void Export()
        {
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Title = "Save Analysis As";
            //dialog.AddExtension = true;
            //dialog.CheckPathExists = true;
            //dialog.DefaultExt = "*.csv";
            //dialog.DereferenceLinks = true;
            //dialog.ValidateNames = true;
            //dialog.Filter = "Comma Delimited (*.csv)|*.csv|Tab Delimited (*.txt)|*.txt|Dataset UMC's (*.umc)|*.umc|SQLite Database File (*.db3)|*.db3|All Files (*.*)|*.*";
            //dialog.FilterIndex = 1;

            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    IAnalysisWriter writer = null;

            //    string filename = dialog.FileNames[0];
            //    string extension = System.IO.Path.GetExtension(filename);
            //    AnalysisTableWriter tableWriter = null;

            //    TableWriterColumnOptionsForm optionsForm = new TableWriterColumnOptionsForm();
            //    optionsForm.Options                      = m_columnOutputOptions;                
            //    optionsForm.Icon                         = this.ParentForm.Icon;
            //    optionsForm.StartPosition                = FormStartPosition.CenterParent;

            //    MultiAlignWin.Forms.Filters.DataFilters filters = new MultiAlignWin.Forms.Filters.DataFilters(m_umcFilters, m_clusterFilters);
            //    if (filters.ShowDialog() != DialogResult.OK)
            //        return;

            //    switch (extension)
            //    {
            //        case ".csv":
            //            optionsForm.ShowDialog();
            //            m_columnOutputOptions = optionsForm.Options;
            //            tableWriter = new AnalysisTableWriter(m_columnOutputOptions);

            //            tableWriter.Delimeter = ",";
            //            writer = tableWriter;
            //            break;
            //        case ".txt":
            //            optionsForm.ShowDialog();
            //            m_columnOutputOptions = optionsForm.Options;
            //            tableWriter = new AnalysisTableWriter(m_columnOutputOptions);

            //            tableWriter.Delimeter = "\t";
            //            writer = tableWriter;
            //            break;
            //        case ".db3":
            //            writer = new AnalysisSQLiteDBWriter();
            //            break;
            //        case ".umc":
            //            writer = new AnalysisDatasetUMCWriter();
            //            break;
            //        default:
            //            optionsForm.ShowDialog();
            //            m_columnOutputOptions = optionsForm.Options;
            //            tableWriter = new AnalysisTableWriter(m_columnOutputOptions);

            //            tableWriter.Delimeter = ",";
            //            writer = tableWriter;
            //            break;
            //    }

            //    writer.WriteAnalysis(filename,
            //                        m_analysis, 
            //                        m_umcFilters.GetFilterList(),
            //                        m_clusterFilters.GetFilterList());
            //}
            //dialog.Dispose();
        }
        private void ExportParameters()
        {
            if (m_analysis != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    XMLParameterFileWriter writer = new XMLParameterFileWriter();                    
                    writer.WriteParameterFile(dialog.FileName, m_analysis);
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
            controlClusterInformation clusterInformation = new controlClusterInformation(m_analysis);
            m_clusterPlots.Controls.Add(clusterInformation);

            clusterInformation.Dock     = DockStyle.Fill;
            m_clusterInformation = clusterInformation;
        }
    }
}
