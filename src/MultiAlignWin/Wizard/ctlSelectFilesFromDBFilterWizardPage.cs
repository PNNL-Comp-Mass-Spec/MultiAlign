/*////////////////////////////////////////////////////////////////////////
 *  File  :  .cs
 *  Author: Ashoka Polpitya
 *          Brian LaMarche
 *          Navdeep Jaityl
 *  Date  : 9/08/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      User interface Wizard control page for loading dataset information from a 
 *      DMS PNNL database. 
 * 
 *  Revisions:
 *      9-8-2008:
 *          - Added this header.
 *          - Created the clsDatabaseDatasetLoader class.  
 *          - Moved database loading code into that class to separate the UI
 *            from the loading of a dataset.
 */
///////////////////////////////////////////////////////////////////////

	
using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data.SqlClient;

using Wizard.UI;
using MultiAlign;
using ExternalControls;
using MultiAlignWin.Data.Loaders;

namespace MultiAlignWin
{
    /// <summary>
    /// Wizard control class for loading dataset information from a DMS database.
    /// </summary>
	public class ctlSelectFilesFromDBFilterWizardPage : Wizard.UI.InternalWizardPage
    {        
        #region Members
        
        #region User Interface / Windows Forms
        private Panel panelStep;
		private Label labelSelect;
		private Panel panelFileNames;
		private IContainer components = null;
					
        private ColumnHeader lcColumnID;
        private ColumnHeader batchID;
        private Button buttonClearAll;

        /// <summary>
        /// Loading panel
        /// </summary>
        private Panel mpanel_loading;
        /// <summary>
        /// Percentage Loading Progress Bar Label
        /// </summary>
        private Label mlabel_percentLoadingComplete;
        /// <summary>
        /// Progress Bar for dataset loading.
        /// </summary>
        private ProgressBar mprogressBar_datasetLoading;

        private ListView joblistView;
        private ColumnHeader mcolumnHeader_datasetID;
        private ColumnHeader mcolumnHeader_jobnum;
        private ColumnHeader mcolumnHeader_fileName;
        private ColumnHeader mcolumnHeader_alias;
        private ColumnHeader mcolumnHeader_block;
        private ColumnHeader mcolumnHeader_runOrder;

        private Button mbtnFilter;
        private Button buttonToggleAll;
        private TextBox textfilterBox;

        private ListViewItemComparer listViewItemComparer;

        #endregion

        #region Internal Members 
        private bool mbool_filesSelected ;	  
        
        private ArrayList marrDatasetInfoChecked = new ArrayList();
        private ArrayList marrDatasetInfoCurrent = new ArrayList();
        private ArrayList marrDatasetIDs = new ArrayList();
        private Button mbutton_selectAll;
        private Button mbutton_deselectAll;
        private ColumnHeader mcolumnHeader_Date;
        private ColumnHeader mcolumnHeader_FileType;
        private ContextMenuStrip mcontextMenu_dataset;
        private ToolStripMenuItem toggleAllToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem deselectAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem clearSelectedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem clearAllToolStripMenuItem;
        private ToolStripMenuItem clearUnSelectedToolStripMenuItem;
        private ColumnHeader columnHeader_tool;

        /// <summary>
        /// Loader class for loading database files.
        /// </summary>
        private clsDatabaseDatasetLoader mobj_dataLoader;

        #endregion

        #endregion

        /// <summary>
        /// Default constructor for loading database files from the database for the user to select which ones to analyze.
        /// </summary>
		public ctlSelectFilesFromDBFilterWizardPage()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            
			// This call is required by the Windows Form Designer.
			InitializeComponent();
						
			listViewItemComparer = new ListViewItemComparer();
			joblistView.ListViewItemSorter = listViewItemComparer;			
			SetActive                     += new System.ComponentModel.CancelEventHandler(this.ctlSelectFilesFromDBFilterWizardPage_SetActive) ;
                        
            textfilterBox.Text           = Properties.Settings.Default.DMSDataSetRegularExpression;
            textfilterBox.TextChanged   += new EventHandler(textfilterBox_TextChanged);

            /// 
            /// A loader object for loading datasets from a database.
            /// 
            mobj_dataLoader = new clsDatabaseDatasetLoader();
            
            mbool_filesSelected = false;

            mprogressBar_datasetLoading.Minimum = 0;
            mprogressBar_datasetLoading.Maximum = 100;

            joblistView.ContextMenuStrip = mcontextMenu_dataset;
        }

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.panelStep = new System.Windows.Forms.Panel();
            this.mbutton_deselectAll = new System.Windows.Forms.Button();
            this.mbutton_selectAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonToggleAll = new System.Windows.Forms.Button();
            this.mbtnFilter = new System.Windows.Forms.Button();
            this.textfilterBox = new System.Windows.Forms.TextBox();
            this.labelSelect = new System.Windows.Forms.Label();
            this.panelFileNames = new System.Windows.Forms.Panel();
            this.joblistView = new System.Windows.Forms.ListView();
            this.mcolumnHeader_datasetID = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_jobnum = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_fileName = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_alias = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_block = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_runOrder = new System.Windows.Forms.ColumnHeader();
            this.lcColumnID = new System.Windows.Forms.ColumnHeader();
            this.batchID = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_Date = new System.Windows.Forms.ColumnHeader();
            this.mcolumnHeader_FileType = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_tool = new System.Windows.Forms.ColumnHeader();
            this.mpanel_loading = new System.Windows.Forms.Panel();
            this.mprogressBar_datasetLoading = new System.Windows.Forms.ProgressBar();
            this.mlabel_percentLoadingComplete = new System.Windows.Forms.Label();
            this.mcontextMenu_dataset = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearUnSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelStep.SuspendLayout();
            this.panelFileNames.SuspendLayout();
            this.mpanel_loading.SuspendLayout();
            this.mcontextMenu_dataset.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(1035, 64);
            this.Banner.Subtitle = "Specify patial names for datasets in DMS";
            this.Banner.Title = "Step 2. Select Job Numbers for Analysis";
            // 
            // panelStep
            // 
            this.panelStep.Controls.Add(this.mbutton_deselectAll);
            this.panelStep.Controls.Add(this.mbutton_selectAll);
            this.panelStep.Controls.Add(this.buttonClearAll);
            this.panelStep.Controls.Add(this.buttonToggleAll);
            this.panelStep.Controls.Add(this.mbtnFilter);
            this.panelStep.Controls.Add(this.textfilterBox);
            this.panelStep.Controls.Add(this.labelSelect);
            this.panelStep.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStep.Location = new System.Drawing.Point(134, 64);
            this.panelStep.Name = "panelStep";
            this.panelStep.Size = new System.Drawing.Size(1035, 40);
            this.panelStep.TabIndex = 2;
            // 
            // mbutton_deselectAll
            // 
            this.mbutton_deselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_deselectAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_deselectAll.Enabled = false;
            this.mbutton_deselectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_deselectAll.Location = new System.Drawing.Point(869, 8);
            this.mbutton_deselectAll.Name = "mbutton_deselectAll";
            this.mbutton_deselectAll.Size = new System.Drawing.Size(77, 24);
            this.mbutton_deselectAll.TabIndex = 6;
            this.mbutton_deselectAll.Text = "Deselect All";
            this.mbutton_deselectAll.UseVisualStyleBackColor = false;
            this.mbutton_deselectAll.Click += new System.EventHandler(this.mbutton_deselectAll_Click);
            // 
            // mbutton_selectAll
            // 
            this.mbutton_selectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_selectAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_selectAll.Enabled = false;
            this.mbutton_selectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_selectAll.Location = new System.Drawing.Point(799, 8);
            this.mbutton_selectAll.Name = "mbutton_selectAll";
            this.mbutton_selectAll.Size = new System.Drawing.Size(64, 24);
            this.mbutton_selectAll.TabIndex = 5;
            this.mbutton_selectAll.Text = "Select All";
            this.mbutton_selectAll.UseVisualStyleBackColor = false;
            this.mbutton_selectAll.Click += new System.EventHandler(this.mbutton_selectAll_Click);
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonClearAll.Enabled = false;
            this.buttonClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearAll.Location = new System.Drawing.Point(967, 8);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(64, 24);
            this.buttonClearAll.TabIndex = 4;
            this.buttonClearAll.Text = "Clear All";
            this.buttonClearAll.UseVisualStyleBackColor = false;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonToggleAll
            // 
            this.buttonToggleAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonToggleAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonToggleAll.Enabled = false;
            this.buttonToggleAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonToggleAll.Location = new System.Drawing.Point(729, 8);
            this.buttonToggleAll.Name = "buttonToggleAll";
            this.buttonToggleAll.Size = new System.Drawing.Size(64, 24);
            this.buttonToggleAll.TabIndex = 3;
            this.buttonToggleAll.Text = "Toggle All";
            this.buttonToggleAll.UseVisualStyleBackColor = false;
            this.buttonToggleAll.Click += new System.EventHandler(this.buttonToggleAll_Click);
            // 
            // mbtnFilter
            // 
            this.mbtnFilter.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnFilter.Location = new System.Drawing.Point(274, 8);
            this.mbtnFilter.Name = "mbtnFilter";
            this.mbtnFilter.Size = new System.Drawing.Size(32, 24);
            this.mbtnFilter.TabIndex = 1;
            this.mbtnFilter.Text = "OK";
            this.mbtnFilter.UseVisualStyleBackColor = false;
            this.mbtnFilter.Click += new System.EventHandler(this.mbtnOKFilter_Click);
            // 
            // textfilterBox
            // 
            this.textfilterBox.AcceptsReturn = true;
            this.textfilterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textfilterBox.Location = new System.Drawing.Point(120, 8);
            this.textfilterBox.Name = "textfilterBox";
            this.textfilterBox.Size = new System.Drawing.Size(148, 21);
            this.textfilterBox.TabIndex = 2;
            this.textfilterBox.Text = "Caulo_049_[2-4]";
            this.textfilterBox.TextChanged += new System.EventHandler(this.filterBox_TextChanged);
            // 
            // labelSelect
            // 
            this.labelSelect.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelSelect.Location = new System.Drawing.Point(0, 0);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(120, 40);
            this.labelSelect.TabIndex = 0;
            this.labelSelect.Text = "Specify part of the name of the dataset:";
            this.labelSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelFileNames
            // 
            this.panelFileNames.Controls.Add(this.joblistView);
            this.panelFileNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFileNames.Location = new System.Drawing.Point(134, 104);
            this.panelFileNames.Name = "panelFileNames";
            this.panelFileNames.Size = new System.Drawing.Size(1035, 614);
            this.panelFileNames.TabIndex = 3;
            // 
            // joblistView
            // 
            this.joblistView.AllowDrop = true;
            this.joblistView.CheckBoxes = true;
            this.joblistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mcolumnHeader_datasetID,
            this.mcolumnHeader_jobnum,
            this.mcolumnHeader_fileName,
            this.mcolumnHeader_alias,
            this.mcolumnHeader_block,
            this.mcolumnHeader_runOrder,
            this.lcColumnID,
            this.batchID,
            this.mcolumnHeader_Date,
            this.mcolumnHeader_FileType,
            this.columnHeader_tool});
            this.joblistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.joblistView.FullRowSelect = true;
            this.joblistView.GridLines = true;
            this.joblistView.Location = new System.Drawing.Point(0, 0);
            this.joblistView.Name = "joblistView";
            this.joblistView.Size = new System.Drawing.Size(1035, 614);
            this.joblistView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.joblistView.TabIndex = 0;
            this.joblistView.UseCompatibleStateImageBehavior = false;
            this.joblistView.View = System.Windows.Forms.View.Details;
            this.joblistView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.joblistView_ColumnClick);
            // 
            // mcolumnHeader_datasetID
            // 
            this.mcolumnHeader_datasetID.Text = "Dataset ID";
            this.mcolumnHeader_datasetID.Width = 77;
            // 
            // mcolumnHeader_jobnum
            // 
            this.mcolumnHeader_jobnum.Text = "Job #";
            // 
            // mcolumnHeader_fileName
            // 
            this.mcolumnHeader_fileName.Text = "File name";
            this.mcolumnHeader_fileName.Width = 227;
            // 
            // mcolumnHeader_alias
            // 
            this.mcolumnHeader_alias.Text = "Alias";
            this.mcolumnHeader_alias.Width = 69;
            // 
            // mcolumnHeader_block
            // 
            this.mcolumnHeader_block.Text = "Block";
            // 
            // mcolumnHeader_runOrder
            // 
            this.mcolumnHeader_runOrder.Text = "Run Order";
            this.mcolumnHeader_runOrder.Width = 70;
            // 
            // lcColumnID
            // 
            this.lcColumnID.Text = "Column ID";
            this.lcColumnID.Width = 77;
            // 
            // batchID
            // 
            this.batchID.Text = "Batch ID";
            // 
            // mcolumnHeader_Date
            // 
            this.mcolumnHeader_Date.Text = "Date Created";
            this.mcolumnHeader_Date.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mcolumnHeader_Date.Width = 89;
            // 
            // mcolumnHeader_FileType
            // 
            this.mcolumnHeader_FileType.Text = "File Type";
            this.mcolumnHeader_FileType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader_tool
            // 
            this.columnHeader_tool.Text = "Tool";
            this.columnHeader_tool.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mpanel_loading
            // 
            this.mpanel_loading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpanel_loading.Controls.Add(this.mprogressBar_datasetLoading);
            this.mpanel_loading.Controls.Add(this.mlabel_percentLoadingComplete);
            this.mpanel_loading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mpanel_loading.Location = new System.Drawing.Point(134, 718);
            this.mpanel_loading.Name = "mpanel_loading";
            this.mpanel_loading.Padding = new System.Windows.Forms.Padding(3);
            this.mpanel_loading.Size = new System.Drawing.Size(1035, 27);
            this.mpanel_loading.TabIndex = 4;
            this.mpanel_loading.Visible = false;
            // 
            // mprogressBar_datasetLoading
            // 
            this.mprogressBar_datasetLoading.BackColor = System.Drawing.Color.White;
            this.mprogressBar_datasetLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mprogressBar_datasetLoading.ForeColor = System.Drawing.Color.Lime;
            this.mprogressBar_datasetLoading.Location = new System.Drawing.Point(3, 3);
            this.mprogressBar_datasetLoading.Name = "mprogressBar_datasetLoading";
            this.mprogressBar_datasetLoading.Size = new System.Drawing.Size(988, 19);
            this.mprogressBar_datasetLoading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.mprogressBar_datasetLoading.TabIndex = 9;
            // 
            // mlabel_percentLoadingComplete
            // 
            this.mlabel_percentLoadingComplete.AutoSize = true;
            this.mlabel_percentLoadingComplete.BackColor = System.Drawing.Color.Transparent;
            this.mlabel_percentLoadingComplete.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mlabel_percentLoadingComplete.Dock = System.Windows.Forms.DockStyle.Right;
            this.mlabel_percentLoadingComplete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mlabel_percentLoadingComplete.Location = new System.Drawing.Point(991, 3);
            this.mlabel_percentLoadingComplete.MinimumSize = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.Name = "mlabel_percentLoadingComplete";
            this.mlabel_percentLoadingComplete.Size = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.TabIndex = 10;
            this.mlabel_percentLoadingComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mcontextMenu_dataset
            // 
            this.mcontextMenu_dataset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleAllToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearUnSelectedToolStripMenuItem,
            this.clearSelectedToolStripMenuItem,
            this.toolStripSeparator2,
            this.clearAllToolStripMenuItem});
            this.mcontextMenu_dataset.Name = "mcontextMenuStrip";
            this.mcontextMenu_dataset.Size = new System.Drawing.Size(168, 148);
            // 
            // toggleAllToolStripMenuItem
            // 
            this.toggleAllToolStripMenuItem.Name = "toggleAllToolStripMenuItem";
            this.toggleAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.toggleAllToolStripMenuItem.Text = "Toggle All";
            this.toggleAllToolStripMenuItem.Click += new System.EventHandler(this.toggleAllToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect All";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.deselectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // clearUnSelectedToolStripMenuItem
            // 
            this.clearUnSelectedToolStripMenuItem.Name = "clearUnSelectedToolStripMenuItem";
            this.clearUnSelectedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.clearUnSelectedToolStripMenuItem.Text = "Clear UnSelected";
            this.clearUnSelectedToolStripMenuItem.Click += new System.EventHandler(this.clearUnSelectedToolStripMenuItem_Click);
            // 
            // clearSelectedToolStripMenuItem
            // 
            this.clearSelectedToolStripMenuItem.Name = "clearSelectedToolStripMenuItem";
            this.clearSelectedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.clearSelectedToolStripMenuItem.Text = "Clear Selected";
            this.clearSelectedToolStripMenuItem.Click += new System.EventHandler(this.clearSelectedToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(164, 6);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.clearAllToolStripMenuItem.Text = "Clear All";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // ctlSelectFilesFromDBFilterWizardPage
            // 
            this.Controls.Add(this.panelFileNames);
            this.Controls.Add(this.mpanel_loading);
            this.Controls.Add(this.panelStep);
            this.MinimumSize = new System.Drawing.Size(750, 300);
            this.Name = "ctlSelectFilesFromDBFilterWizardPage";
            this.Size = new System.Drawing.Size(1169, 745);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.panelStep, 0);
            this.Controls.SetChildIndex(this.mpanel_loading, 0);
            this.Controls.SetChildIndex(this.panelFileNames, 0);
            this.panelStep.ResumeLayout(false);
            this.panelStep.PerformLayout();
            this.panelFileNames.ResumeLayout(false);
            this.mpanel_loading.ResumeLayout(false);
            this.mpanel_loading.PerformLayout();
            this.mcontextMenu_dataset.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion              
        #region Dataset Loader Event Handlers
        /// <summary>
        /// Updates the dataset loading progress bar.
        /// </summary>
        /// <param name="percentLoaded">Progress of the dataset loader.</param>
        void mobj_dataLoader_LoadingProgress(double percentLoaded)
        {
                if (InvokeRequired == true)
                {
                    Invoke(new DelegateUpdateLoadingPercentLoaded(UpdateDatasetLoadingProgressBar), percentLoaded);
                }
                else
                {
                    UpdateDatasetLoadingProgressBar(Convert.ToInt32(percentLoaded));
                }            
        }
        /// <summary>
        /// Handles when the loader class signals that loading is complete.
        /// </summary>
        void mobj_dataLoader_LoadingComplete()
        {           
                if (InvokeRequired == true)
                {
                    Invoke(new DelegateUpdateLoadingComplete(LoadingComplete));
                }
                else
                {
                    LoadingComplete();
                }           
        }
        /// <summary>
        /// Handles when a dataset is loaded from the dataset information loader.  Adds this to the listviews and the 
        /// internal arrays.  This call is blocking.
        /// </summary>
        /// <param name="dataset">Dataset to add to the list of datasets and listviews.</param>
        void mobj_dataLoader_LoadedDataset(MultiAlign.clsDatasetInfo dataset)
        {         
                if (InvokeRequired == true)
                {
                    Invoke(new DelegateDataSetLoaded(AddDatasetToLists), dataset);
                }
                else
                {
                    AddDatasetToLists(dataset);
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
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }		  
        /// <summary>
        /// Handles when the wizard page becomes active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctlSelectFilesFromDBFilterWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SetWizardButtons(Wizard.UI.WizardButtons.Back | Wizard.UI.WizardButtons.Next);			
            textfilterBox.Focus();
		}
        /// <summary>
        /// Handles when the user wants to sort the data by clicking on the column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void joblistView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{

			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == listViewItemComparer.SortColumn)
			{
				// Reverse the current sort direction for this column.
                if (listViewItemComparer.Order == System.Windows.Forms.SortOrder.Ascending)
				{
                    listViewItemComparer.Order = System.Windows.Forms.SortOrder.Descending;
				}
				else
				{
                    listViewItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				listViewItemComparer.SortColumn = e.Column;
                listViewItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			joblistView.Sort();
        }
        /// <summary>
        /// Handles overriding the ability to cancel the loading of this wizard page.
        /// </summary>
        /// <param name="e"></param>
        public override void OnQueryCancel(CancelEventArgs e)
        {
            mobj_dataLoader.CancelLoadingDatasets();
            base.OnQueryCancel(e);
        }
        /// <summary>
        /// Handles when the user changes the settings for the dataset name filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textfilterBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DMSDataSetRegularExpression = textfilterBox.Text;
        }
        /// <summary>
        /// Invokable method for adding a dataset to the listview and internal array list structure.
        /// </summary>
        /// <param name="dataset"></param>
        private void AddDatasetToLists(MultiAlign.clsDatasetInfo dataset)
        {
            joblistView.BeginUpdate();
           // AddDatasetToListView(dataset);            
            joblistView.EndUpdate();
        }
        /// <summary>
        /// Updates the loading progress bar to tell the user how much has been loaded.
        /// </summary>
        /// <param name="value"></param>
        private void UpdateDatasetLoadingProgressBar(double value)
        {
            mprogressBar_datasetLoading.Style  = ProgressBarStyle.Continuous;
            mprogressBar_datasetLoading.Value  = Math.Max(0, Math.Min(100, Convert.ToInt32(value)));
            mlabel_percentLoadingComplete.Text = string.Format("{0}%", Convert.ToInt32(value));
        }
        /// <summary>
        /// Turns off the loading ui controls.
        /// </summary>
        private void LoadingComplete()
        {
            SetLoadingState(false);                        
        }
        /// <summary>
        /// Hides the label and progress bar when the loading is complete.
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            mpanel_loading.Visible      = isLoading;
            buttonClearAll.Enabled      = (isLoading == false);
            buttonToggleAll.Enabled     = buttonClearAll.Enabled;
            mbutton_deselectAll.Enabled = buttonClearAll.Enabled;
            mbutton_selectAll.Enabled   = buttonClearAll.Enabled;
            toggleAllToolStripMenuItem.Enabled     = buttonClearAll.Enabled;
            selectAllToolStripMenuItem.Enabled     = buttonClearAll.Enabled;
            deselectAllToolStripMenuItem.Enabled   = buttonClearAll.Enabled;
            clearSelectedToolStripMenuItem.Enabled = buttonClearAll.Enabled;
            clearAllToolStripMenuItem.Enabled      = buttonClearAll.Enabled;
        }
     
        /// <summary>
        /// Event handler when the ok button to find regular expressions is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mbtnOKFilter_Click(object sender, System.EventArgs e)
		{
           // LoadData();
		}			

        /// <summary>
        /// Handles when the user filter text is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void filterBox_TextChanged(object sender, System.EventArgs e)
		{
			if (textfilterBox.Text.Length == 0)
				mbtnFilter.Enabled = false;
			else
				mbtnFilter.Enabled = true;
		}

		
        /// <summary>
        /// Handles when the user clicks to clear all the found datasets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void buttonClearAll_Click(object sender, System.EventArgs e)
		{
            ClearAll();
        }

        /// <summary>
        /// Gets or sets the dataset information array.
        /// </summary>
		public ArrayList DataSetInfo
		{
			get
			{
                return SelectCheckedItems();
			}
		}

        private void mbutton_selectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void mbutton_deselectAll_Click(object sender, EventArgs e)
        {
            DeselectAll();
        }

        /// <summary>
        /// Handles when the user clicks to select all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonToggleAll_Click(object sender, System.EventArgs e)
        {
            ToggleAll();
        }

        private void toggleAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleAll();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeselectAll();
        }

        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSelected();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void clearUnSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearUnSelected();
        }
	}
	
}
