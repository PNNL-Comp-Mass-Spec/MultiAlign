/*////////////////////////////////////////////////////////////////////////
 *  File  :  ctlSelectFilesFromDiskWizardPage.cs
 *  Author: Navdeep Jaitly
 *          Ashoka Polpitya
 *          Brian LaMarche
 *  Date  : 9/9/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Control for selecting datasets from the disk.  
 * 
 *  Revisions:
 *      9-9-2008:
 *          - Changed how the data is stored in this control.  Moving it from the 
 *              internal arrays to the tag portion of the listview.
 *          - Removed internal filename arrays
 *          - Changed Dataset from a get/set to a get Property
 *          - Added this header
 *          - Added comments to functions
 *          - Added check boxes to each listview item 
 */
///////////////////////////////////////////////////////////////////////

	
using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

using MultiAlign;

namespace MultiAlignWin
{
	public class ctlSelectFilesFromDiskWizardPage : Wizard.UI.InternalWizardPage
    {
        #region Members

        #region User Interface - Windows Forms
        private Panel panelStep;
        private Button mbtnSelectFiles;
        private Panel panelFileNames;
        private IContainer components = null;
        private ListView joblistView;
        private ColumnHeader datasetID;
        private ColumnHeader jobnum;
        private ColumnHeader fileName;
        private ColumnHeader alias;
        private ColumnHeader block;
        private Button mbutton_deselectAll;
        private Button mbutton_selectAll;
        private Button buttonClearAll;
        private Button buttonToggleAll;
        private ContextMenuStrip mcontextMenu_dataset;
        private ToolStripMenuItem toggleAllToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem deselectAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem clearUnSelectedToolStripMenuItem;
        private ToolStripMenuItem clearSelectedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem clearAllToolStripMenuItem;
        private ColumnHeader runOrder;
        private OpenFileDialog mobj_openFileDialog;
        #endregion

        #endregion

        /// <summary>
        /// Default constructor for loading data files from disk.
        /// </summary>
        public ctlSelectFilesFromDiskWizardPage()
		{
			InitializeComponent();
            joblistView.ContextMenuStrip = mcontextMenu_dataset;

            /// 
            /// Construct a new open file dialog box.
            /// 
            mobj_openFileDialog                     = new OpenFileDialog();            
            mobj_openFileDialog.Multiselect         = true;
            mobj_openFileDialog.Filter              = "*.pek files (*.pek)|*.pek|*_isos.csv files (*_isos.csv)|*_isos.csv|All files (*.*)|*.*";
            mobj_openFileDialog.FilterIndex         = 1;
            mobj_openFileDialog.InitialDirectory    = Properties.Settings.Default.RawDataPath;
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
            this.mbtnSelectFiles = new System.Windows.Forms.Button();
            this.panelFileNames = new System.Windows.Forms.Panel();
            this.joblistView = new System.Windows.Forms.ListView();
            this.datasetID = new System.Windows.Forms.ColumnHeader();
            this.jobnum = new System.Windows.Forms.ColumnHeader();
            this.fileName = new System.Windows.Forms.ColumnHeader();
            this.alias = new System.Windows.Forms.ColumnHeader();
            this.block = new System.Windows.Forms.ColumnHeader();
            this.runOrder = new System.Windows.Forms.ColumnHeader();
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
            this.mcontextMenu_dataset.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(616, 64);
            this.Banner.Subtitle = "Select files from your local or networked drives";
            this.Banner.Title = "Step 2. Select PEK files for Analysis";
            // 
            // panelStep
            // 
            this.panelStep.Controls.Add(this.mbutton_deselectAll);
            this.panelStep.Controls.Add(this.mbutton_selectAll);
            this.panelStep.Controls.Add(this.buttonClearAll);
            this.panelStep.Controls.Add(this.buttonToggleAll);
            this.panelStep.Controls.Add(this.mbtnSelectFiles);
            this.panelStep.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStep.Location = new System.Drawing.Point(134, 64);
            this.panelStep.Name = "panelStep";
            this.panelStep.Padding = new System.Windows.Forms.Padding(5);
            this.panelStep.Size = new System.Drawing.Size(616, 35);
            this.panelStep.TabIndex = 2;
            // 
            // mbutton_deselectAll
            // 
            this.mbutton_deselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_deselectAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_deselectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_deselectAll.Location = new System.Drawing.Point(435, 5);
            this.mbutton_deselectAll.Name = "mbutton_deselectAll";
            this.mbutton_deselectAll.Size = new System.Drawing.Size(77, 24);
            this.mbutton_deselectAll.TabIndex = 10;
            this.mbutton_deselectAll.Text = "Deselect All";
            this.mbutton_deselectAll.UseVisualStyleBackColor = true;
            this.mbutton_deselectAll.Click += new System.EventHandler(this.mbutton_deselectAll_Click);
            // 
            // mbutton_selectAll
            // 
            this.mbutton_selectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_selectAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_selectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_selectAll.Location = new System.Drawing.Point(365, 5);
            this.mbutton_selectAll.Name = "mbutton_selectAll";
            this.mbutton_selectAll.Size = new System.Drawing.Size(64, 24);
            this.mbutton_selectAll.TabIndex = 9;
            this.mbutton_selectAll.Text = "Select All";
            this.mbutton_selectAll.UseVisualStyleBackColor = true;
            this.mbutton_selectAll.Click += new System.EventHandler(this.mbutton_selectAll_Click);
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearAll.Location = new System.Drawing.Point(537, 5);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(64, 24);
            this.buttonClearAll.TabIndex = 8;
            this.buttonClearAll.Text = "Clear All";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonToggleAll
            // 
            this.buttonToggleAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonToggleAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonToggleAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonToggleAll.Location = new System.Drawing.Point(295, 5);
            this.buttonToggleAll.Name = "buttonToggleAll";
            this.buttonToggleAll.Size = new System.Drawing.Size(64, 24);
            this.buttonToggleAll.TabIndex = 7;
            this.buttonToggleAll.Text = "Toggle All";
            this.buttonToggleAll.UseVisualStyleBackColor = true;
            this.buttonToggleAll.Click += new System.EventHandler(this.buttonToggleAll_Click);
            // 
            // mbtnSelectFiles
            // 
            this.mbtnSelectFiles.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnSelectFiles.Location = new System.Drawing.Point(8, 5);
            this.mbtnSelectFiles.Name = "mbtnSelectFiles";
            this.mbtnSelectFiles.Size = new System.Drawing.Size(103, 24);
            this.mbtnSelectFiles.TabIndex = 1;
            this.mbtnSelectFiles.Text = "Select Pek Files...";
            this.mbtnSelectFiles.UseVisualStyleBackColor = false;
            this.mbtnSelectFiles.Click += new System.EventHandler(this.mbtnSelectFiles_Click);
            // 
            // panelFileNames
            // 
            this.panelFileNames.Controls.Add(this.joblistView);
            this.panelFileNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFileNames.Location = new System.Drawing.Point(134, 99);
            this.panelFileNames.Name = "panelFileNames";
            this.panelFileNames.Size = new System.Drawing.Size(616, 616);
            this.panelFileNames.TabIndex = 3;
            // 
            // joblistView
            // 
            this.joblistView.AllowDrop = true;
            this.joblistView.CheckBoxes = true;
            this.joblistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.datasetID,
            this.jobnum,
            this.fileName,
            this.alias,
            this.block,
            this.runOrder});
            this.joblistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.joblistView.FullRowSelect = true;
            this.joblistView.GridLines = true;
            this.joblistView.Location = new System.Drawing.Point(0, 0);
            this.joblistView.Name = "joblistView";
            this.joblistView.Size = new System.Drawing.Size(616, 616);
            this.joblistView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.joblistView.TabIndex = 1;
            this.joblistView.UseCompatibleStateImageBehavior = false;
            this.joblistView.View = System.Windows.Forms.View.Details;
            // 
            // datasetID
            // 
            this.datasetID.Text = "Dataset ID";
            this.datasetID.Width = 77;
            // 
            // jobnum
            // 
            this.jobnum.Text = "Job #";
            // 
            // fileName
            // 
            this.fileName.Text = "File name";
            this.fileName.Width = 222;
            // 
            // alias
            // 
            this.alias.Text = "Alias";
            this.alias.Width = 69;
            // 
            // block
            // 
            this.block.Text = "Block";
            // 
            // runOrder
            // 
            this.runOrder.Text = "Run Order";
            this.runOrder.Width = 70;
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
            // ctlSelectFilesFromDiskWizardPage
            // 
            this.Controls.Add(this.panelFileNames);
            this.Controls.Add(this.panelStep);
            this.MinimumSize = new System.Drawing.Size(750, 300);
            this.Name = "ctlSelectFilesFromDiskWizardPage";
            this.Size = new System.Drawing.Size(750, 715);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.panelStep, 0);
            this.Controls.SetChildIndex(this.panelFileNames, 0);
            this.panelStep.ResumeLayout(false);
            this.panelFileNames.ResumeLayout(false);
            this.mcontextMenu_dataset.ResumeLayout(false);
            this.ResumeLayout(false);

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
        /// Add a dataset to the list of available datasets.
        /// </summary>
        /// <param name="datasetInfo"></param>
		private void AddDatasetToList(clsDatasetInfo datasetInfo)
		{
			ListViewItem dataItem = new ListViewItem(datasetInfo.mstrDatasetId) ;
			dataItem.SubItems.Add(datasetInfo.mstrAnalysisJobId) ;
			dataItem.SubItems.Add(datasetInfo.mstrDatasetName) ;
			dataItem.SubItems.Add(datasetInfo.mstrAlias) ;

			if (datasetInfo.mintBlockID == 0)
				dataItem.SubItems.Add("NA") ;
			else
				dataItem.SubItems.Add(datasetInfo.mintBlockID.ToString()) ;

			if (datasetInfo.mintRunOrder == 0)
				dataItem.SubItems.Add("NA") ;
			else
				dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintRunOrder)) ;

            dataItem.Tag     = datasetInfo; 
			dataItem.Checked = datasetInfo.selected ;
			joblistView.Items.Add(dataItem) ;
		}
        /// <summary>
        /// Loads the set of files from disk into the array.
        /// </summary>
        /// <param name="fileNames">Paths of the files to load.</param>
        private void LoadDatasetFiles(string [] fileNames)
        {
            joblistView.BeginUpdate();

            for (int fileNum = 0; fileNum < fileNames.Length; fileNum++)
            {
                clsDatasetInfo datasetInfo = new clsDatasetInfo();
                string filePath = fileNames[fileNum];
                int index       = filePath.LastIndexOf("\\");
                string fileName = filePath;
                
                if (index > 0)
                {
                    fileName = fileName.Substring(index + 1);
                }

                index = fileName.ToLower().LastIndexOf(".pek");
                if (index < 0)
                    index = fileName.ToLower().LastIndexOf("_isos.csv");

                string fileNameAlias = null;
                if (index > 0)
                {
                    fileNameAlias = fileName.Substring(0, index);
                }
                else
                {
                    fileNameAlias = fileName;
                }

                datasetInfo.mstrDatasetName     = fileName;
                datasetInfo.mstrAlias           = fileNameAlias;
                datasetInfo.mstrDatasetId       = "NA";
                datasetInfo.mstrAnalysisJobId   = "NA";
                datasetInfo.mintBlockID         = 0;
                datasetInfo.mintRunOrder        = 0;
                datasetInfo.mstrLocalPath       = filePath;

                AddDatasetToList(datasetInfo);
            }
            joblistView.EndUpdate();
        }
		/// <summary>
		/// Handles when the user selects to load a file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mbtnSelectFiles_Click(object sender, System.EventArgs e)
		{
			if(mobj_openFileDialog.ShowDialog() == DialogResult.OK)
			{
                System.IO.FileInfo info = new System.IO.FileInfo(mobj_openFileDialog.FileName);
                Properties.Settings.Default.RawDataPath = info.Directory.FullName;
                Properties.Settings.Default.Save();
                
                LoadDatasetFiles(mobj_openFileDialog.FileNames);
			}		
		}
        #region Dataset Selection
        /// <summary>
        /// Collects a list of checked items from the listview.
        /// </summary>
        private ArrayList SelectCheckedItems()
        {
            ArrayList list = new ArrayList();
            foreach (ListViewItem item in joblistView.CheckedItems)
            {
                clsDatasetInfo info = item.Tag as clsDatasetInfo;
                if (info != null)
                {
                    list.Add(info);
                }
            }
            return list;
        }
        /// <summary>
        /// Deselects all the datasets.
        /// </summary>
        private void DeselectAll()
        {
            joblistView.BeginUpdate();
            foreach (ListViewItem item in joblistView.Items)
            {
                item.Checked = false;
            }
            joblistView.EndUpdate();
        }
        /// <summary>
        /// Selects all the datasets.
        /// </summary>
        private void SelectAll()
        {
            joblistView.BeginUpdate();
            foreach (ListViewItem item in joblistView.Items)
            {
                item.Checked = true;
            }
            joblistView.EndUpdate();
        }
        /// <summary>
        /// Toggles the selection for all the datasets.
        /// </summary>
        private void ToggleAll()
        {
            joblistView.BeginUpdate();
            foreach (ListViewItem item in joblistView.Items)
            {
                item.Checked = (item.Checked == false);
            }
            joblistView.EndUpdate();
        }
        /// <summary>
        /// Clears all the datasets from the list.
        /// </summary>
        private void ClearAll()
        {
            joblistView.Items.Clear();
        }
        /// <summary>
        /// Clears all the selected items.
        /// </summary>
        private void ClearSelected()
        {
            joblistView.BeginUpdate();
            foreach (ListViewItem item in joblistView.CheckedItems)
            {
                joblistView.Items.Remove(item);
            }
            joblistView.EndUpdate();
        }
        /// <summary>
        /// Clears all the selected items.
        /// </summary>
        private void ClearUnSelected()
        {
            joblistView.BeginUpdate();
            foreach (ListViewItem item in joblistView.Items)
            {
                if (item.Checked == false)
                {
                    joblistView.Items.Remove(item);
                }
            }
            joblistView.EndUpdate();
        }
        #endregion
        /// <summary>
        /// Gets the dataset information to load.
        /// </summary>
		public ArrayList DatasetInfo
		{
			get
			{
				return SelectCheckedItems();
			}
		}



        private void buttonToggleAll_Click(object sender, EventArgs e)
        {
            ToggleAll();
        }
        private void mbutton_selectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }
        private void mbutton_deselectAll_Click(object sender, EventArgs e)
        {
            DeselectAll();
        }
        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            ClearAll();
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
        private void clearUnSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearUnSelected();
        }
        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSelected();
        }
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

	}
}

