using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection.Emit;


using MultiAlignEngine;
using PNNLControls;

using PNNLProteomics.Data;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Analysis;
using PNNLProteomics.Data.Factors;

using MultiAlignWin.Forms.Factors;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmAlignmentDataOptions.
	/// </summary>
	public class frmAnalysisSummary : Form
	{
		
		private const int DATASET_COLUMN  = 0;
		private const int BASELINE_COLUMN = 1;
		private const int RSQUARED_COLUMN = 2;
		
		private clsMultiAlignAnalysis mobjAnalysis ;
		private ColumnHeader	columnDataset;
		private ColumnHeader	columnBaseline;
		private ColumnHeader	columnRsquared;
		private Panel			panelDatasetDescription;		
		private Splitter		splitter;
		private string			m_baseline = "";
		private System.Windows.Forms.Label label1;
		private Form mfrmCurrentPreviewForm ;
		private System.Windows.Forms.MenuItem menuItemTools;
		private System.Windows.Forms.MenuItem menuItemRecluster; 
		private frmStatus mfrmStatus ; 
		private bool mblnDatasetModified ;
		private System.Windows.Forms.MenuItem menuItemReAlign;
		private System.Windows.Forms.MenuItem menuItemSelectMassTagDatabase;
		private System.Windows.Forms.MenuItem menuItemPeakMatch;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuChangeFactorOrdering; 

		private frmLabelGroupEdit m_factorEdit = new frmLabelGroupEdit();
		private System.Windows.Forms.MenuItem mnuReassignFactorToData;
		private System.Windows.Forms.MenuItem mnuAddDeleteFactor;
		private System.Windows.Forms.MenuItem mnuLoadFactors;
		private System.Windows.Forms.MenuItem mnuLoadFactorAssignments;
		private System.Windows.Forms.MenuItem mnuSaveFactorAssignments;
		private System.Windows.Forms.MenuItem mnuSaveFactors;
        private System.Windows.Forms.MenuItem mnuLoadFactorsParent;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnuSaveDataSummary;
		private System.Windows.Forms.MenuItem menuFileSave;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.Panel panelGlobalSummary;
		private MultiAlignWin.ctlSummaryPages mcontrol_resultSummaryPages;
		private System.Windows.Forms.MenuItem mnuSaveDatasetSummary;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem mnuSaveAnalysisAs;
		private System.Windows.Forms.MenuItem mnuSaveAnalysis;
		private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem5;
        private Button mbutton_Ok;
        private Button mbutton_cancel;
        private System.Windows.Forms.Label mlabel_options;
        private ListViewEx listDataset;
        private IContainer components;

		public frmAnalysisSummary()
		{					
			InitializeComponent();
			InitializeControl();
			Init();
		}

		public frmAnalysisSummary(clsMultiAlignAnalysis analysis)
		{				
			InitializeComponent();
			InitializeControl();
			Analysis = analysis;
			Init();
		}

		private void Init()
		{
			mfrmStatus = new frmStatus() ; 
			mblnDatasetModified = false ; 
			this.Closing += new CancelEventHandler(frmDataAlignmentSummary_Closing);
		}

		/// <summary>
		/// Runs all initialization for the control including calling the windows initializecomponent method.
		/// </summary>
		protected void InitializeControl()
		{
			Closing += new CancelEventHandler(frmDataAlignmentSummary_Closing);

            listDataset.Columns.Clear();
            listDataset.Columns.Add("Dataset #", -2, HorizontalAlignment.Left);
            listDataset.Columns.Add("Dataset",   -1, HorizontalAlignment.Left);       
            listDataset.Columns.Add("Baseline",  -1, HorizontalAlignment.Left);
            listDataset.Columns.Add("R-Squared", -2, HorizontalAlignment.Left);
            listDataset.Columns.Add("Net vs. Scan Preview",   -2, HorizontalAlignment.Left);
		}

		/// <summary>
		/// MultiAlign analysis information.
		/// </summary>
		public clsMultiAlignAnalysis Analysis
		{
			get
			{
				return mobjAnalysis ;	
			}
			set
			{
				mobjAnalysis = value;
				if (value != null)
				{
					m_baseline  = mobjAnalysis.BaselineDataset; 
					Text		= "Analysis Summary for: " + mobjAnalysis.AnalysisName;
				}
				UpdateListViews();
			}
		}

		public bool DatasetModified
		{
			get
			{
				return mblnDatasetModified ; 
			}
			set
			{
				mblnDatasetModified = value ; 
			}
		}


		private void UpdateDatasetSummary()
		{
			/* 
						 *   Baseline Summary for each dataset
						 */
			if (mobjAnalysis == null) return;
			listDataset.Items.Clear();			
			string []names = mobjAnalysis.UMCData.DatasetName ;
			for(int datasetIndex = 0 ; datasetIndex < names.Length; datasetIndex++)
			{
				MultiAlignEngine.Alignment.clsAlignmentOptions alignmentOptions =  mobjAnalysis.AlignmentOptions[datasetIndex]
					as MultiAlignEngine.Alignment.clsAlignmentOptions; 

				string [] baselineStrings = alignmentOptions.AlignmentBaselineName.Split('\\');
				string baselineName = string.Empty;
				baselineName = baselineStrings[baselineStrings.Length - 1];

				ListViewExItem	item		            = new PNNLControls.ListViewExItem(datasetIndex.ToString());
				ListViewExLinkLabelItem	dataItem	    = new ListViewExLinkLabelItem(names[datasetIndex], listDataset);
				ListViewExTextItem		rsqItem		    = new ListViewExTextItem("-");
				ListViewExLinkLabelItem	previewItem	    = new ListViewExLinkLabelItem("Preview", listDataset);
				ListViewExLinkLabelItem	baselineItem	= new ListViewExLinkLabelItem(baselineName, listDataset);
				baselineItem.Font		 = listDataset.Font;
                baselineItem.Parent      = item;
				baselineItem.Action	    += new DelegateListViewExEvent(baselineItem_Action);				

				previewItem.Text		 = "Preview";
				previewItem.Font		 = listDataset.Font;
				previewItem.Action		+= new DelegateListViewExEvent(linkItem_Action);
				
				dataItem.Action			+= new DelegateListViewExEvent(dataItem_Action);
				dataItem.Font			 = listDataset.Font;
				
				/* add sub items */				
				item.SubItems.Add(dataItem);
				item.SubItems.Add(baselineItem);
				item.SubItems.Add(rsqItem);
				item.Tag = datasetIndex ; 								
				item.SubItems.Add(previewItem);
				
				/* Add to listview */
				listDataset.Items.Add(item);		
				listDataset.Items[datasetIndex].UseItemStyleForSubItems = false;
			}

			listDataset.UpdateListViewWidths();			
		}
		/// <summary>
		/// Updates both listview with appropiate cluster information
		/// </summary>
		private void UpdateListViews()
		{
			mcontrol_resultSummaryPages.CreateSummary("Global Summary", mobjAnalysis);
			mcontrol_resultSummaryPages.CreateSummary("Alignment Options", mobjAnalysis.DefaultAlignmentOptions);
			
			mcontrol_resultSummaryPages.CreateSummary("UMC Data", mobjAnalysis.UMCData);
			mcontrol_resultSummaryPages.CreateSummary("Cluster Data", mobjAnalysis.UMCData.mobjClusterData);
			mcontrol_resultSummaryPages.CreateSummary("UMC Finding Options", mobjAnalysis.UMCFindingOptions);
			mcontrol_resultSummaryPages.CreateSummary("Mass Tag Database Options (MTDB)", mobjAnalysis.MassTagDBOptions);

			/// 
			/// In case the user didnt match against the MTDB
			/// 
			if (mobjAnalysis.PeakMatchingResults != null)
				mcontrol_resultSummaryPages.CreateSummary("Peak Matching Results", mobjAnalysis.PeakMatchingResults);

			UpdateDatasetSummary();
			UpdateReClusterInfo();
			mcontrol_resultSummaryPages.UpdateColumnWidths();
		}

		private void UpdateObject(ListView list, object o)
		{
			list.Items.Clear();
			UpdateSummary(list, o);
			list.Columns.Clear();
			list.Columns.Add("Description",-1,System.Windows.Forms.HorizontalAlignment.Left);
			list.Columns.Add("Value",-1,System.Windows.Forms.HorizontalAlignment.Left);	
			UpdateListWidths(list);
		}

		private void UpdateListWidths(ListView list)
		{
			int cols = list.Columns.Count;
			if (cols <= 0)
				return;
			
			for(int i = 0 ; i < list.Columns.Count ; i++)
			{
				list.Columns[i].Width = list.Width / cols;
			}
		}
		
		/// <summary>
		/// Add a cluster to the summary.  Reads the appropiate attribute from the property of the cluster objects.
		/// </summary>
		/// <param name="o"></param>
		private void UpdateSummary(ListView list, object o)
		{
			if (o != null)
			{
				foreach(PropertyInfo prop in o.GetType().GetProperties())
				{
					object[] customAttributes = prop.GetCustomAttributes(typeof(MultiAlignEngine.clsDataSummaryAttribute),true);
					if (customAttributes.Length > 0 && prop.CanRead)
					{
						MultiAlignEngine.clsDataSummaryAttribute attr = customAttributes[0] as MultiAlignEngine.clsDataSummaryAttribute ;						
						object objectValue = prop.GetValue(o,System.Reflection.BindingFlags.GetProperty,
							null,null,null);
						if (objectValue != null && attr != null)
						{
							AddGlobalSummaryListItem(list, attr.Description,objectValue.ToString(), Color.Black, Color.White);
						}
					}
				}				
				
				foreach(FieldInfo field in o.GetType().GetFields())
				{
					object[] customAttributes = field.GetCustomAttributes(typeof(MultiAlignEngine.clsDataSummaryAttribute),true);
					if (customAttributes.Length > 0)
					{
						MultiAlignEngine.clsDataSummaryAttribute attr = customAttributes[0] as MultiAlignEngine.clsDataSummaryAttribute ;						
						object objectValue = field.GetValue(o);
						if (objectValue != null && attr != null)
						{
							AddGlobalSummaryListItem(list, attr.Description, objectValue.ToString(), Color.Black, Color.White);
						}
					}
				}	
			}
		}


		/// <summary>
		/// Adds a text and value to the global summary list view
		/// </summary>
		/// <param name="description">Description to be drawn</param>
		/// <param name="val">Value to be displayed</param>
		/// <param name="foreColor">Forecolor of value text</param>
		/// <param name="backColor">Backcolor of value text</param>
		private void AddGlobalSummaryListItem(ListView list, string description, string val, Color foreColor, Color backColor)
		{
			/* Create a new list view item */
			ListViewItem summaryItem = new ListViewItem();
			
			/* Make the item and enable dynamic formatting */
			summaryItem.Text = description;
			summaryItem.SubItems.Add(val);	
			summaryItem.UseItemStyleForSubItems = false;

			/* Make it look pretty */
			summaryItem.SubItems[1].ForeColor   = foreColor;
			summaryItem.SubItems[1].BackColor   = backColor;
						
			list.Items.Add(summaryItem);
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
                mobjAnalysis = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAnalysisSummary));
            this.columnDataset = new System.Windows.Forms.ColumnHeader();
            this.columnBaseline = new System.Windows.Forms.ColumnHeader();
            this.columnRsquared = new System.Windows.Forms.ColumnHeader();
            this.panelGlobalSummary = new System.Windows.Forms.Panel();
            this.mlabel_options = new System.Windows.Forms.Label();
            this.splitter = new System.Windows.Forms.Splitter();
            this.panelDatasetDescription = new System.Windows.Forms.Panel();
            this.listDataset = new PNNLControls.ListViewEx();
            this.mbutton_Ok = new System.Windows.Forms.Button();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuFileSave = new System.Windows.Forms.MenuItem();
            this.mnuSaveDataSummary = new System.Windows.Forms.MenuItem();
            this.mnuSaveDatasetSummary = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mnuSaveFactors = new System.Windows.Forms.MenuItem();
            this.mnuSaveFactorAssignments = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.mnuSaveAnalysis = new System.Windows.Forms.MenuItem();
            this.mnuSaveAnalysisAs = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.mnuLoadFactorsParent = new System.Windows.Forms.MenuItem();
            this.mnuLoadFactors = new System.Windows.Forms.MenuItem();
            this.mnuLoadFactorAssignments = new System.Windows.Forms.MenuItem();
            this.menuItemTools = new System.Windows.Forms.MenuItem();
            this.menuItemRecluster = new System.Windows.Forms.MenuItem();
            this.menuItemReAlign = new System.Windows.Forms.MenuItem();
            this.menuItemSelectMassTagDatabase = new System.Windows.Forms.MenuItem();
            this.menuItemPeakMatch = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuAddDeleteFactor = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.mnuChangeFactorOrdering = new System.Windows.Forms.MenuItem();
            this.mnuReassignFactorToData = new System.Windows.Forms.MenuItem();
            this.mcontrol_resultSummaryPages = new MultiAlignWin.ctlSummaryPages();
            this.panelGlobalSummary.SuspendLayout();
            this.panelDatasetDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnDataset
            // 
            resources.ApplyResources(this.columnDataset, "columnDataset");
            // 
            // columnBaseline
            // 
            resources.ApplyResources(this.columnBaseline, "columnBaseline");
            // 
            // columnRsquared
            // 
            resources.ApplyResources(this.columnRsquared, "columnRsquared");
            // 
            // panelGlobalSummary
            // 
            this.panelGlobalSummary.BackColor = System.Drawing.SystemColors.Control;
            this.panelGlobalSummary.Controls.Add(this.mlabel_options);
            this.panelGlobalSummary.Controls.Add(this.mcontrol_resultSummaryPages);
            resources.ApplyResources(this.panelGlobalSummary, "panelGlobalSummary");
            this.panelGlobalSummary.Name = "panelGlobalSummary";
            // 
            // mlabel_options
            // 
            this.mlabel_options.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.mlabel_options, "mlabel_options");
            this.mlabel_options.Name = "mlabel_options";
            // 
            // splitter
            // 
            this.splitter.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.splitter, "splitter");
            this.splitter.Name = "splitter";
            this.splitter.TabStop = false;
            // 
            // panelDatasetDescription
            // 
            this.panelDatasetDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelDatasetDescription.Controls.Add(this.listDataset);
            this.panelDatasetDescription.Controls.Add(this.mbutton_Ok);
            this.panelDatasetDescription.Controls.Add(this.mbutton_cancel);
            this.panelDatasetDescription.Controls.Add(this.label1);
            resources.ApplyResources(this.panelDatasetDescription, "panelDatasetDescription");
            this.panelDatasetDescription.Name = "panelDatasetDescription";
            // 
            // listDataset
            // 
            this.listDataset.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.listDataset, "listDataset");
            this.listDataset.FullRowSelect = true;
            this.listDataset.GridLines = true;
            this.listDataset.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listDataset.MultiSelect = false;
            this.listDataset.Name = "listDataset";
            this.listDataset.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listDataset.UseCompatibleStateImageBehavior = false;
            this.listDataset.View = System.Windows.Forms.View.Details;
            // 
            // mbutton_Ok
            // 
            resources.ApplyResources(this.mbutton_Ok, "mbutton_Ok");
            this.mbutton_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_Ok.Name = "mbutton_Ok";
            this.mbutton_Ok.UseVisualStyleBackColor = true;
            // 
            // mbutton_cancel
            // 
            resources.ApplyResources(this.mbutton_cancel, "mbutton_cancel");
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3,
            this.menuItemTools,
            this.menuItem1});
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 0;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileSave,
            this.menuItem2,
            this.mnuLoadFactorsParent});
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuFileSave
            // 
            this.menuFileSave.Index = 0;
            this.menuFileSave.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuSaveDataSummary,
            this.mnuSaveDatasetSummary,
            this.menuItem4,
            this.mnuSaveFactors,
            this.mnuSaveFactorAssignments,
            this.menuItem5,
            this.mnuSaveAnalysis,
            this.mnuSaveAnalysisAs});
            resources.ApplyResources(this.menuFileSave, "menuFileSave");
            // 
            // mnuSaveDataSummary
            // 
            this.mnuSaveDataSummary.Index = 0;
            resources.ApplyResources(this.mnuSaveDataSummary, "mnuSaveDataSummary");
            this.mnuSaveDataSummary.Click += new System.EventHandler(this.mnuSaveDataSummary_Click);
            // 
            // mnuSaveDatasetSummary
            // 
            this.mnuSaveDatasetSummary.Index = 1;
            resources.ApplyResources(this.mnuSaveDatasetSummary, "mnuSaveDatasetSummary");
            this.mnuSaveDatasetSummary.Click += new System.EventHandler(this.mnuSaveDatasetSummary_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // mnuSaveFactors
            // 
            this.mnuSaveFactors.Index = 3;
            resources.ApplyResources(this.mnuSaveFactors, "mnuSaveFactors");
            this.mnuSaveFactors.Click += new System.EventHandler(this.mnuSaveFactors_Click);
            // 
            // mnuSaveFactorAssignments
            // 
            this.mnuSaveFactorAssignments.Index = 4;
            resources.ApplyResources(this.mnuSaveFactorAssignments, "mnuSaveFactorAssignments");
            this.mnuSaveFactorAssignments.Click += new System.EventHandler(this.mnuSaveFactorAssignments_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 5;
            resources.ApplyResources(this.menuItem5, "menuItem5");
            // 
            // mnuSaveAnalysis
            // 
            this.mnuSaveAnalysis.Index = 6;
            resources.ApplyResources(this.mnuSaveAnalysis, "mnuSaveAnalysis");
            this.mnuSaveAnalysis.Click += new System.EventHandler(this.mnuSaveAnalysis_Click);
            // 
            // mnuSaveAnalysisAs
            // 
            this.mnuSaveAnalysisAs.Index = 7;
            resources.ApplyResources(this.mnuSaveAnalysisAs, "mnuSaveAnalysisAs");
            this.mnuSaveAnalysisAs.Click += new System.EventHandler(this.mnuSaveAnalysisAs_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // mnuLoadFactorsParent
            // 
            this.mnuLoadFactorsParent.Index = 2;
            this.mnuLoadFactorsParent.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuLoadFactors,
            this.mnuLoadFactorAssignments});
            resources.ApplyResources(this.mnuLoadFactorsParent, "mnuLoadFactorsParent");
            // 
            // mnuLoadFactors
            // 
            this.mnuLoadFactors.Index = 0;
            resources.ApplyResources(this.mnuLoadFactors, "mnuLoadFactors");
            this.mnuLoadFactors.Click += new System.EventHandler(this.mnuLoadFactors_Click);
            // 
            // mnuLoadFactorAssignments
            // 
            this.mnuLoadFactorAssignments.Index = 1;
            resources.ApplyResources(this.mnuLoadFactorAssignments, "mnuLoadFactorAssignments");
            this.mnuLoadFactorAssignments.Click += new System.EventHandler(this.mnuLoadFactorAssignments_Click);
            // 
            // menuItemTools
            // 
            this.menuItemTools.Index = 1;
            this.menuItemTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemRecluster,
            this.menuItemReAlign,
            this.menuItemSelectMassTagDatabase,
            this.menuItemPeakMatch});
            resources.ApplyResources(this.menuItemTools, "menuItemTools");
            // 
            // menuItemRecluster
            // 
            this.menuItemRecluster.Index = 0;
            resources.ApplyResources(this.menuItemRecluster, "menuItemRecluster");
            this.menuItemRecluster.Click += new System.EventHandler(this.menuItemRecluster_Click);
            // 
            // menuItemReAlign
            // 
            this.menuItemReAlign.Index = 1;
            resources.ApplyResources(this.menuItemReAlign, "menuItemReAlign");
            this.menuItemReAlign.Click += new System.EventHandler(this.menuItemReAlign_Click);
            // 
            // menuItemSelectMassTagDatabase
            // 
            this.menuItemSelectMassTagDatabase.Index = 2;
            resources.ApplyResources(this.menuItemSelectMassTagDatabase, "menuItemSelectMassTagDatabase");
            this.menuItemSelectMassTagDatabase.Click += new System.EventHandler(this.menuItemSelectMassTagDatabase_Click);
            // 
            // menuItemPeakMatch
            // 
            this.menuItemPeakMatch.Index = 3;
            resources.ApplyResources(this.menuItemPeakMatch, "menuItemPeakMatch");
            this.menuItemPeakMatch.Click += new System.EventHandler(this.menuItemPeakMatch_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAddDeleteFactor,
            this.menuItem7,
            this.mnuChangeFactorOrdering,
            this.mnuReassignFactorToData});
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // mnuAddDeleteFactor
            // 
            this.mnuAddDeleteFactor.Index = 0;
            resources.ApplyResources(this.mnuAddDeleteFactor, "mnuAddDeleteFactor");
            this.mnuAddDeleteFactor.Click += new System.EventHandler(this.mnuAddDeleteFactor_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            resources.ApplyResources(this.menuItem7, "menuItem7");
            // 
            // mnuChangeFactorOrdering
            // 
            this.mnuChangeFactorOrdering.Index = 2;
            resources.ApplyResources(this.mnuChangeFactorOrdering, "mnuChangeFactorOrdering");
            this.mnuChangeFactorOrdering.Click += new System.EventHandler(this.mnuChangeFactorOrdering_Click);
            // 
            // mnuReassignFactorToData
            // 
            this.mnuReassignFactorToData.Index = 3;
            resources.ApplyResources(this.mnuReassignFactorToData, "mnuReassignFactorToData");
            this.mnuReassignFactorToData.Click += new System.EventHandler(this.mnuReassignFactorToData_Click);
            // 
            // mcontrol_resultSummaryPages
            // 
            this.mcontrol_resultSummaryPages.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.mcontrol_resultSummaryPages, "mcontrol_resultSummaryPages");
            this.mcontrol_resultSummaryPages.Name = "mcontrol_resultSummaryPages";
            this.mcontrol_resultSummaryPages.Load += new System.EventHandler(this.ctlSummaryPages_Load);
            // 
            // frmAnalysisSummary
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panelDatasetDescription);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.panelGlobalSummary);
            this.Menu = this.mainMenu;
            this.Name = "frmAnalysisSummary";
            this.Load += new System.EventHandler(this.frmAnalysisSummary_Load);
            this.panelGlobalSummary.ResumeLayout(false);
            this.panelDatasetDescription.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
				
		/// <summary>
		/// Updates the cluster class and UI to show that a recluster is required.
		/// </summary>
		private void UpdateReClusterInfo()
		{
		}

		/// <summary>
		/// Handles when a column has changed.
		/// </summary>
		/// <param name="column"></param>
		private void listDataset_OnColumnChanged(int row, int column, string text)
		{			
		}

		/// <summary>
		/// Event handler when the user or application fires a to close event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmDataAlignmentSummary_Closing(object sender, CancelEventArgs e)
		{
			/* Make sure we have a valid cluster reference */
			if (mobjAnalysis != null)
			{
				/* 
				 * Check each cluster, make sure its aligned with the appropiate cluster baseline. 
				 * If not then flag that it needs to be reclustered. 
				 */
				UpdateReClusterInfo();
			}
		}

		Bitmap bit; 
		/// <summary>
		/// Event handler when the link list view item has been clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void linkItem_Action(object sender, EventArgs e)
		{
			if (mfrmCurrentPreviewForm != null)
			{
				mfrmCurrentPreviewForm.Close() ; 
				mfrmCurrentPreviewForm = null ; 
			}

			PNNLControls.ListViewExEventArgs args = e as PNNLControls.ListViewExEventArgs;
			if (args != null)
			{
				Form f = new Form();
				f.Text = "Preview Pane";
				f.FormBorderStyle = FormBorderStyle.None;

				// set the data for the chart. 
				// get current datasetindex, go through each cluster that this dataset was seen in
				// and plot scan vs net of cluster. 
				int numClusters = mobjAnalysis.UMCData.mobjClusterData.NumClusters ; 
				int numDatasets = mobjAnalysis.UMCData.NumDatasets ; 
				int datasetNum = 0 ; 
				for (int itemIndex = 0 ; itemIndex < listDataset.Items.Count ; itemIndex++)
				{
					if (listDataset.Items[itemIndex].SubItems.Contains((PNNLControls.ListViewExLinkLabelItem)sender))
					{
						datasetNum = (int) listDataset.Items[itemIndex].Tag ; 
					}
				}

				System.Collections.ArrayList clusterIndices = new System.Collections.ArrayList() ; 
				for (int clusterNum = 0 ; clusterNum < numClusters ; clusterNum++)
				{
					int umcIndex = mobjAnalysis.UMCData.mobjClusterData.marrClusterMainMemberIndex[clusterNum*numDatasets + datasetNum] ;
					if (umcIndex != -1)
					{
						clusterIndices.Add(clusterNum) ; 
					}
				}

				Graphics g ; 
				Bitmap bmp ; 
				if (clusterIndices.Count != 0)
				{
					int numPoints = clusterIndices.Count ; 
					float [] scanNums = new float [numPoints] ; 
					float [] nets = new float [numPoints] ; 
					for (int index = 0 ; index < numPoints ; index++)
					{
						int clusterIndex = (int) clusterIndices[index] ;
						int umcIndex =  mobjAnalysis.UMCData.mobjClusterData.marrClusterMainMemberIndex[clusterIndex*numDatasets+datasetNum] ;
						scanNums[index] = mobjAnalysis.UMCData.marr_umcs[umcIndex].mint_scan ; 
						nets[index] = (float) mobjAnalysis.UMCData.mobjClusterData.GetCluster(clusterIndex).mdouble_net ; 
					}
					// now add data points to the chart. 
					PNNLControls.ctlScatterChart chart = new PNNLControls.ctlScatterChart() ; 
					chart.XAxisLabel = "Scan #" ; 
					chart.YAxisLabel = "Cluster NET" ; 
					int ptSize = 1 ; 
					Color clr = Color.Red ; 
					PNNLControls.clsShape shape = new PNNLControls.BubbleShape(ptSize, false) ;  ; 
					PNNLControls.clsPlotParams plt_params = new PNNLControls.clsPlotParams(shape, clr) ; 
					plt_params.Name = mobjAnalysis.UMCData.DatasetName[datasetNum] ; 
					chart.AutoViewPortOnAddition = true ; 
					PNNLControls.clsSeries series = new PNNLControls.clsSeries(ref scanNums, ref nets, plt_params) ; 
					chart.AddSeries(series) ; 
					chart.ViewPortHistory.Clear(); 
					chart.Dock = DockStyle.Fill ; 
					f.Controls.Add(chart) ; 
					chart.Click +=new EventHandler(Chart_Click);
					chart.KeyUp +=new KeyEventHandler(PreviewChartKeyUp);
				}
				else
				{
					bmp = new Bitmap(f.Width,f.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					g = Graphics.FromImage(bmp);
					g.FillRectangle(new System.Drawing.SolidBrush(Color.Black),0,0,f.Width,f.Height);
					g.DrawLine(new Pen(Color.White),f.Left, f.Height, f.Width,0);
					g.DrawString("Click to close", new Font("Ariel",8), new SolidBrush(Color.White), f.Width/2, 0);
					bit = bmp;
					f.Click += new EventHandler(b_Click);
					f.KeyUp += new KeyEventHandler(PreviewFormKeyUp);
				}
				mfrmCurrentPreviewForm = f ; 				
				f.Show();
				f.Location = Cursor.Position;
			}
		}

		/// <summary>
		/// Event handler when the comboItem changes. 
		/// </summary>
		/// <param name="sender">object where event propagated from.</param>
		/// <param name="e">Combo box arguments</param>
		private void baselineItem_Action(object sender, EventArgs e)
		{
			ListViewExLinkLabelItem subItem = sender as ListViewExLinkLabelItem;
			
			if (subItem == null)
				return;

            /// 
            /// Find the index of the dataset to display the alignment summary for.
            /// 
            ListViewItem item = subItem.Parent as ListViewItem;
            if (item == null)
                return;            
            int aligneeIndex = listDataset.Items.IndexOf(item as ListViewItem);
            if (aligneeIndex < 0)
                return;

            /// 
            /// Get the datasets filename
            /// 
            string info = mobjAnalysis.FileNames[aligneeIndex];                
		    frmAlignmentPreview alignment = new frmAlignmentPreview(mobjAnalysis, aligneeIndex);
            alignment.Text += " For : " + info;

            /// 
            /// Display the alignment preview and let the user do what they want with it.
            /// 
		    DialogResult result = alignment.ShowDialog();

            /// 
            /// If the user clicked ok, 
            /// 
            if (result == DialogResult.OK)
            {
                //TODO: What?
            }            
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
				PNNLControls.ctlScatterChart chart = sender as PNNLControls.ctlScatterChart ;
				Form f = chart.Parent as Form ;
				if (f != null)
					f.Close();
			}
		}

		private void PreviewFormKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
			{
				Form f = sender as Form;
				if (f != null)
					f.Close();
			}
		}

		private void b_Click(object sender, EventArgs e)
		{
			Form f = sender as Form;
			f.Close();
		}
		private void Chart_Click(object sender, EventArgs e)
		{
			PNNLControls.ctlScatterChart chart = sender as PNNLControls.ctlScatterChart ;
			Form f = chart.Parent as Form ;
			if (f == mfrmCurrentPreviewForm)
				mfrmCurrentPreviewForm = null ; 
			f.Close() ; 
		}

		private void f_Paint(object sender, PaintEventArgs e)
		{
			if (bit != null)
				e.Graphics.DrawImage(bit,0,0);
		}

		private void menuItemRecluster_Click(object sender, System.EventArgs e)
		{

			DelegateSetPercentComplete percentCompleteDlg = new DelegateSetPercentComplete(mfrmStatus.mevntPercentComplete);
			mobjAnalysis.PercentComplete += percentCompleteDlg ;
			DelegateSetStatusMessage statusMessageDlg = new DelegateSetStatusMessage(mfrmStatus.mevntStatusMessage);
			mobjAnalysis.StatusMessage  += statusMessageDlg ;

            /// 
            /// Start the threading
            /// 
			ThreadStart ts = new System.Threading.ThreadStart(PerformClustering) ; 			
            Thread thread  = new System.Threading.Thread(ts) ; 
			thread.Start(); 

			mfrmStatus.ShowDialog(this); 

			mobjAnalysis.PercentComplete -= percentCompleteDlg ; 
			mobjAnalysis.StatusMessage   -= statusMessageDlg ; 

		}
		
		private void PerformClustering()
		{
			mblnDatasetModified = true ; 
			mobjAnalysis.PerformClustering() ; 
			if (mobjAnalysis.MassTagDBOptions == null || mobjAnalysis.MassTagDBOptions.mstrDatabase == null 
				|| mobjAnalysis.MassTagDBOptions.mstrDatabase == "")
			{
				mfrmStatus.Hide() ; 
				return ; 
			}
			try
			{
				mobjAnalysis.LoadMassTagDB() ; 
				mobjAnalysis.PerformPeakMatching() ; 
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("Could not open mass tag database. " + ex.Message) ; 
			}
			mfrmStatus.Hide() ;
		}

		private void menuItemReAlign_Click(object sender, System.EventArgs e)
		{
		}

		private void menuItemSelectMassTagDatabase_Click(object sender, System.EventArgs e)
		{
			frmDBName dbForm = new frmDBName();

			if (mobjAnalysis.MassTagDBOptions == null)
				mobjAnalysis.MassTagDBOptions = new MultiAlignEngine.MassTags.clsMassTagDatabaseOptions() ; 
			dbForm.MassTagDatabaseOptions = mobjAnalysis.MassTagDBOptions ; 
			if (dbForm.ShowDialog() == DialogResult.OK)
			{
				mobjAnalysis.MassTagDBOptions = dbForm.MassTagDatabaseOptions ; 
				mblnDatasetModified = true ; 
				if (mobjAnalysis.MassTagDBOptions.mstrDatabase != null &&
					mobjAnalysis.MassTagDBOptions.mstrDatabase != "")
				{
					mobjAnalysis.MassTagDBOptions.mstrDatabase = mobjAnalysis.MassTagDBOptions.mstrDatabase ;
				}
			}		
		}

		private void menuItemPeakMatch_Click(object sender, System.EventArgs e)
		{
			DelegateSetPercentComplete percentCompleteDlg = new DelegateSetPercentComplete(mfrmStatus.mevntPercentComplete);
			mobjAnalysis.PercentComplete += percentCompleteDlg ;
			DelegateSetStatusMessage statusMessageDlg = new DelegateSetStatusMessage(mfrmStatus.mevntStatusMessage);
			mobjAnalysis.StatusMessage += statusMessageDlg ;
			System.Threading.ThreadStart ts = new System.Threading.ThreadStart(PeakMatch) ; 
			System.Threading.Thread thread = new System.Threading.Thread(ts) ; 
			thread.Start() ; 
			mfrmStatus.ShowDialog(this) ; 
			mobjAnalysis.PercentComplete -= percentCompleteDlg ; 
			mobjAnalysis.StatusMessage -= statusMessageDlg ; 
			mblnDatasetModified = true ; 
		}

		public void PeakMatch()
		{
			string massTagDBName = mobjAnalysis.MassTagDBOptions.mstrDatabase ; 
			if (massTagDBName != null && massTagDBName != "")
			{
				mobjAnalysis.LoadMassTagDB() ; 
			}
			if (mobjAnalysis.MassTagDBOptions.mstrDatabase != null 
				&& mobjAnalysis.MassTagDBOptions.mstrDatabase != "")
			{
				mobjAnalysis.PerformPeakMatching() ; 
			}
		}

		private void mnuChangeFactorOrdering_Click(object sender, System.EventArgs e)
		{
			m_factorEdit.Icon = Icon;
			m_factorEdit.FactorTree = mobjAnalysis.FactorTree;			
			DialogResult result = m_factorEdit.ShowDialog();
			if (result == DialogResult.OK)
			{
				m_factorEdit.BuildTree();
				mobjAnalysis.FactorTree = m_factorEdit.FactorTree;
				mblnDatasetModified  = true;
			}
		}

		private void mnuReassignFactorToData_Click(object sender, System.EventArgs e)
		{
			MultiAlignWin.frmDataFactorSelection mFactorAssignment = new frmDataFactorSelection(mobjAnalysis);
			DialogResult result = mFactorAssignment.ShowDialog();
			if (result == DialogResult.OK)
			{
				mblnDatasetModified  = true;
			}
		}

		private void mnuAddDeleteFactor_Click(object sender, System.EventArgs e)
		{
            /*
			frmFactorDefinition factorDefinitionForm;
			ArrayList factorList = new ArrayList();  

			/// 
			/// Create the list of factors 
			/// 
			foreach(PNNLControls.clsFactor singleFactor in mobjAnalysis.FactorTree.Factors)
			{
				MultiAlignEngine.clsFactorInfo newFactor = new MultiAlignEngine.clsFactorInfo();
				newFactor.mstrFactor = singleFactor.Name;
				newFactor.marrValues = new ArrayList();
				foreach(string factorValue in singleFactor.Values.Keys)
				{
					newFactor.marrValues.Add(factorValue);
				}
				factorList.Add(newFactor);
			}

			/// 
			/// Show the form
			/// 
			factorDefinitionForm = new frmFactorDefinition(factorList);
			if (factorDefinitionForm.ShowDialog() == DialogResult.OK)
			{
				LoadFactorsFromDefinitionForm(factorDefinitionForm);
				mblnDatasetModified = true;
			}             
            */

		}

		private void LoadFactorsFromDefinitionForm(frmFactorDefinition factorDefinitionForm)
		{
            /*
             * 
			/// 
			/// If the user says "yes" - then add the factors back in.
			/// 
			foreach(MultiAlignEngine.clsDatasetInfo dataInfo in mobjAnalysis.Files)
			{
				/// 
				/// The difference will tell us how many factors are defined in this dataset
				///    If we have a negative ( < 0) difference then: we need to remove those from the dataset
				///    If we have a positive ( > 0) difference then: we don't care.  
				/// 
				int difference = factorDefinitionForm.FactorInfoArray.Count - dataInfo.marrFactorInfo.Count;
				if (difference < 0)
				{
					for(int j = 0; j < -difference; j++)
					{
						dataInfo.marrFactorInfo.RemoveAt(dataInfo.marrFactorInfo.Count - 1);
					}
				}

				///
				/// Iterate over every factor info defined in the factor form
				///		Updating the current data set 
				/// 
				for(int i =0 ; i < factorDefinitionForm.FactorInfoArray.Count; i++)
				{					
					MultiAlignEngine.clsFactorInfo factorInfo = factorDefinitionForm.FactorInfoArray[i] as MultiAlignEngine.clsFactorInfo;
					if (factorInfo == null)
						continue;						
																			
					MultiAlignEngine.clsFactorInfo dataFactorInfo = null;
					/// 
					/// 1.  The factor info does not exist in this index, create a new one
					/// 2.  else - use the one at the current index.
					///
					if (i >= dataInfo.marrFactorInfo.Count)
					{
						dataFactorInfo = new MultiAlignEngine.clsFactorInfo();
						dataFactorInfo.mstrFactor = string.Empty;
						dataInfo.marrFactorInfo.Add(dataFactorInfo);
					}
					else
					{
						dataFactorInfo = dataInfo.marrFactorInfo[i] as MultiAlignEngine.clsFactorInfo;
					}
						
					if (i >= dataInfo.AssignedFactorValues.Count)							
						dataInfo.AssignedFactorValues.Add(string.Empty);

					string previousFactorValue = string.Empty;
					if ( dataInfo.AssignedFactorValues.Count > i )
						previousFactorValue = dataInfo.AssignedFactorValues[i] as String;						
						
					dataFactorInfo.marrValues.Clear();
					dataFactorInfo.mstrFactor = factorInfo.mstrFactor;						
						
					bool found = false;
					foreach(string factorValue in factorInfo.marrValues)
					{
						if (factorValue == previousFactorValue)							
							found = true;
						dataFactorInfo.marrValues.Add(factorValue);
					}

					if (found == false)
						previousFactorValue = factorInfo.marrValues[0] as string;
					dataInfo.AssignedFactorValues[i] = previousFactorValue;							
				}
			}
			mobjAnalysis.BuildFactorTree();
             * 
             **/
		}

		private void mnuLoadFactors_Click(object sender, System.EventArgs e)
		{
            /*
			System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Load Factors";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				frmLoadFactorsForm factorLoader = new frmLoadFactorsForm(dialog.FileName);
				if (factorLoader.ShowDialog() == DialogResult.OK)
				{
					ArrayList factors = new ArrayList();
					frmFactorDefinition defineFactors;
					foreach(string header in factorLoader.Headers)
					{
						MultiAlignEngine.clsFactorInfo factor = new MultiAlignEngine.clsFactorInfo();
						factor.mstrFactor = header;
						ArrayList data = factorLoader.Data[header] as ArrayList;
						foreach(string dataName in data)
						{
							factor.marrValues.Add(dataName);
						}
						factors.Add(factor);
					}
					defineFactors = new frmFactorDefinition(factors);					
					if (defineFactors.ShowDialog() == DialogResult.OK)
					{
						LoadFactorsFromDefinitionForm(defineFactors);
						frmDataFactorSelection defineSelection = new frmDataFactorSelection(mobjAnalysis);
						defineSelection.Text = "Assign datasets to factor values";
						defineSelection.ShowDialog();
						mobjAnalysis.BuildFactorTree();
						int x = 9;
						x++;
					}
				}
			}
             * */
		}

		private void mnuSaveFactors_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save Factors";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.OverwritePrompt = true;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				frmSaveFactors saveform = new frmSaveFactors();
				saveform.Path  = dialog.FileName;

				foreach(clsFactor factor in mobjAnalysis.FactorTree.Factors)
				{
					saveform.Headers.Add(factor.Name);
					ArrayList valueData = new ArrayList();
					foreach(string factorValue in factor.Values.Keys)
					{
						valueData.Add(factorValue);
					}
					saveform.Data.Add(factor.Name, valueData);
				}
				if (saveform.ShowDialog() == DialogResult.OK)
				{
					saveform.WriteFile();
				}
			}
		}


		private void LoadFactorAssignments(string path)
		{
			frmLoadFactorsForm factorsForm = new frmLoadFactorsForm(path);
				
			if (factorsForm.ShowDialog() == DialogResult.OK)
			{
				/// 
				/// Get the factor list
				/// 
				if (factorsForm.Data.ContainsKey("Factor") == false)
				{
					return;
				}
				ArrayList factors = factorsForm.Data["Factor"] as ArrayList;
				if (factors == null)
					return;

				
				/// 
				/// Load up each dataset
				/// 
				foreach(string datasetName in factorsForm.Data.Keys)
				{
					System.Console.WriteLine(datasetName);
					/// 
					/// find the dataset index since we use indexes not hashes
					/// 
					for(int i = 0; i < mobjAnalysis.Files.Count; i++)
					{
						clsDatasetInfo data = mobjAnalysis.Files[i] as clsDatasetInfo;
						if (data.mstrDatasetName == datasetName)
						{
							ArrayList factorValues = factorsForm.Data[datasetName] as ArrayList;
		
							/// 
							/// Find out of the factor is defined in the current analysis.
							/// 
							
							for(int k = 0; k < factors.Count; k++)
							{
								string factorName = factors[k] as String;	
								for(int j = 0; j < data.Factors.Count; j++)
								{
									MultiAlignEngine.clsFactorInfo factInfo = data.Factors[j] as MultiAlignEngine.clsFactorInfo;
									if (factInfo.mstrFactor == factorName)
									{
										/// 
										/// Find out if the factor value is defined.
										/// 
										for(int ii = 0; ii < factInfo.marrValues.Count; ii++)
										{
											string definedFactorValue = factInfo.marrValues[ii] as string;
											string newFactorValue	  = factorValues[k] as string; 
											if (definedFactorValue == newFactorValue)
											{
												data.AssignedFactorValues[j] = newFactorValue;
											}
										}
										break;
									}
								}
							}
							break;
						}
					}
				
					mobjAnalysis.BuildFactorTree();
				}
			}
		}

		/// <summary>
		/// Loads the factor assignments.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuLoadFactorAssignments_Click(object sender, System.EventArgs e)
		{

			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Load Factor Assignments";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				 LoadFactorAssignments(dialog.FileName);
			}
		}

		/// <summary>
		/// Save the factor assignments.  The data is saved in column format, comma separated, with the dataset names as the column names.
		/// This format is the same as used in DAnTE.
		/// </summary>
		/// <param name="path">File location to save to.</param>
		private void SaveFactorAssignments(string path)
		{
			PNNLControls.clsTextDelimitedFileWriter writer = new PNNLControls.clsTextDelimitedFileWriter();
			writer.Headers.Add("Factor");
			writer.Delimiter = ',';
			writer.LinesBeforeHeader = 0;			

			ArrayList factorList = new ArrayList();
			foreach(clsFactor factor in this.mobjAnalysis.FactorTree.Factors)
			{
				factorList.Add(factor.Name);
			}
			
			///  
			///  Save the data names as column headers
			///  
			foreach(clsFactorDataset data in this.mobjAnalysis.FactorTree.Data)
			{
				writer.Headers.Add(data.Name);
				ArrayList list = new ArrayList();
				foreach(string key in factorList)
				{
					list.Add(data.Values[key]);
				}
				writer.Data.Add(data.Name, list);
			}

			/// 
			/// The factor names are saved in a column as the data items.
			/// 
			writer.Data.Add("Factor", factorList);			
			writer.Write(path, true);
		}

		/// <summary>
		/// Handles the event when the user decides to save the factor assignments for this analysis as a text file.  These
		/// assignments are particular to these dataset names.  
		/// </summary>
		/// <param name="sender">Menu control that invoked the event.</param>
		/// <param name="e">Event arguments sent by sender.</param>
		private void mnuSaveFactorAssignments_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save Factor Assignments";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.OverwritePrompt = true;
			dialog.FilterIndex = 1;
				
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				SaveFactorAssignments(dialog.FileName);
			}
		}

		private void dataItem_Action(object sender, EventArgs e)
		{
			PNNLControls.ListViewExEventArgs args = e as PNNLControls.ListViewExEventArgs;
			if (args != null)
			{
				int datasetNum = -1;
				for (int itemIndex = 0 ; itemIndex < listDataset.Items.Count ; itemIndex++)
				{
					if (listDataset.Items[itemIndex].SubItems[1].Text == args.Text)
					{
						datasetNum = (int) listDataset.Items[itemIndex].Tag;
					}
				}
				
				if (datasetNum >= 0)
				{
					/*frmDatasetSummary newDatasummary = new frmDatasetSummary();
					newDatasummary.DatasetNumber = datasetNum;
					newDatasummary.Analysis = mobjAnalysis;
					newDatasummary.ShowDialog();
                     */
				}
			}
		}

		/// <summary>
		/// Saves the global data summary to file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuSaveDataSummary_Click(object sender, System.EventArgs e)
		{			
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save Data Summary";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				frmTextDelimitedFileSave saveForm = new frmTextDelimitedFileSave();
				saveForm.Icon = this.Icon;
				saveForm.Text = "Save Format Options";
				saveForm.LinesToSkip = 2;
				saveForm.Format = TextDataFileFormat.COLUMN;
				if (saveForm.ShowDialog() == DialogResult.OK)
				{
					mcontrol_resultSummaryPages.Save(dialog.FileName, saveForm.Delimiter, saveForm.LinesToSkip);
				}
				saveForm.Dispose();
			}		
			dialog.Dispose();
		}

		/// <summary>
		/// Saves all the datasets in the summary page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuSaveDatasetSummary_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save Dataset Summary";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.txt";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*" ;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				frmTextDelimitedFileSave saveForm = new frmTextDelimitedFileSave();
				saveForm.Icon = this.Icon;
				saveForm.Text = "Save Format Options";
				saveForm.LinesToSkip = 2;				
				saveForm.Format = TextDataFileFormat.COLUMN;

				/// 
				/// Show ther user an options form to change the format of the file for better
				/// exporting to other analysis tools
				/// 
				if (saveForm.ShowDialog() == DialogResult.OK)
				{
					int datasetNum = -1;
					/// 
					/// Foreach dataset save it respectively to file
					/// 
					for (int itemIndex = 0 ; itemIndex < listDataset.Items.Count ; itemIndex++)
					{
						datasetNum = (int) listDataset.Items[itemIndex].Tag;
						/*frmDatasetSummary newDatasummary = new frmDatasetSummary();
						newDatasummary.DatasetNumber = datasetNum;
						newDatasummary.Analysis = mobjAnalysis;
						newDatasummary.Save(dialog.FileName, saveForm.Delimiter, saveForm.LinesToSkip);
						newDatasummary.Dispose();*/
					}
				}
				saveForm.Dispose();
			}		
			dialog.Dispose();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			frmDataFactorListView f = new frmDataFactorListView();
			f.ShowDialog();
		}

		private void ctlSummaryPages_Load(object sender, System.EventArgs e)
		{
		
		}

		private void mnuSaveAnalysisAs_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save Analysis As";
			dialog.AddExtension = true;
			dialog.CheckPathExists = true;
			dialog.DefaultExt = "*.mln";
			dialog.DereferenceLinks = true;
			dialog.ValidateNames = true;
			dialog.Filter = "MultiAlign Analysis (*.mln)|*.mln|All Files (*.*)|*.*" ;
			dialog.FilterIndex = 1;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				mobjAnalysis.SerializeAnalysisToFile(dialog.FileName);
			}		
			dialog.Dispose();
		}

		private void mnuSaveAnalysis_Click(object sender, System.EventArgs e)
		{			
			mobjAnalysis.SerializeAnalysisToFile(mobjAnalysis.PathName);
		}

        private void frmAnalysisSummary_Load(object sender, EventArgs e)
        {

        }
	}
}
