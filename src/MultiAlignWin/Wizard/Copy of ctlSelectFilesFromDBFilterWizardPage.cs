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
using MultiAlignWin.Data ;
using ExternalControls;


namespace MultiAlignWin
{
	public class ctlSelectFilesFromDBFilterWizardPage : Wizard.UI.InternalWizardPage
	{


        /// <summary>
        /// Delegate method definition for adding a dataset to the listview controls and the internal array lists.
        /// </summary>
        /// <param name="dataset"></param>
        internal delegate void DelegateAddDataSetToList(MultiAlign.clsDatasetInfo dataset);

        /// <summary>
        /// Delegate method for when a data set is loaded, to update the progress of the loading progress bar.
        /// </summary>
        /// <param name="progressValue"></param>
        internal delegate void DelegateUpdateLoadingProgress(int datasetNumber);

		public enum enmToolID {ICR2LS=2, QTOFPek=7, MLynxPek=10, AgilentTOFPek=11, LTQ_FTPek=12, 
			Decon2LS=16, Decon2LS_Agilent=18} ; 
		
		public string mstrQuery ; 
		private System.Windows.Forms.Panel panelStep;
		private System.Windows.Forms.Label labelSelect;
		private System.Windows.Forms.Panel panelFileNames;
		private System.ComponentModel.IContainer components = null;
				
		private ArrayList marrDatasetInfo = new ArrayList() ;
		private ArrayList marrDatasetInfoChecked = new ArrayList();
		private ArrayList marrDatasetInfoCurrent = new ArrayList() ;
		private ArrayList marrDatasetIDs = new ArrayList();

		private System.Windows.Forms.TextBox textfilterBox;

		private System.Windows.Forms.ListView joblistView;
		private System.Windows.Forms.ColumnHeader datasetID;
		private System.Windows.Forms.ColumnHeader jobnum;
		private System.Windows.Forms.ColumnHeader fileName;
		private System.Windows.Forms.ColumnHeader alias;
		private System.Windows.Forms.ColumnHeader block ;
		private System.Windows.Forms.ColumnHeader runOrder ;

		private System.Windows.Forms.Button mbtnFilter;
		private System.Windows.Forms.Button buttonToggleAll;
		
		private bool FilesSelected = false ;
		private const int NUM_COLUMNS = 8 ;
		private ListViewItemComparer _lvwItemComparer ;

		bool defaultSelected = false ;
		
		MultiAlignWin.enmSelectType selection ;
		string datasetId ;
		private System.Windows.Forms.ColumnHeader lcColumnID;
		private System.Windows.Forms.ColumnHeader batchID;
        private System.Windows.Forms.Button buttonClearAll;
        private Panel mpanel_loading;
        private Label mlabel_percentLoadingComplete;
        private ProgressBar mprogressBar_datasetLoading;


        /// <summary>
        /// A loading thread for loading datasets given a filter id.
        /// </summary>
        private Thread mobj_loadingThread;

		public ctlSelectFilesFromDBFilterWizardPage()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
						
			_lvwItemComparer = new ListViewItemComparer();
			joblistView.ListViewItemSorter = _lvwItemComparer;			
			SetActive                     += new System.ComponentModel.CancelEventHandler(this.ctlSelectFilesFromDBFilterWizardPage_SetActive) ;
                        
            textfilterBox.Text           = Properties.Settings.Default.DMSDataSetRegularExpression;
            textfilterBox.TextChanged   += new EventHandler(textfilterBox_TextChanged);
		}

        /// <summary>
        /// Handles overriding the ability to cancel the loading of this wizard page.
        /// </summary>
        /// <param name="e"></param>
        public override void OnQueryCancel(CancelEventArgs e)
        {            
            CancelLoadingDatasetFileNames();
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


        /// <summary>
        /// Cancels the loading of the file names from the DB. 
        /// </summary>
        public void CancelLoadingDatasetFileNames()
        {
            try
            {
                if (mobj_loadingThread != null)
                {
                    mobj_loadingThread.Abort();
                }
            }
            catch
            {
                /// 
                /// Handle an exception while trying to abort the dataset file name loading.
                /// 

            }
            finally
            {
                mprogressBar_datasetLoading.Visible = false;
                mlabel_percentLoadingComplete.Visible = false;
            }
        }
		
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panelStep = new System.Windows.Forms.Panel();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonToggleAll = new System.Windows.Forms.Button();
            this.mbtnFilter = new System.Windows.Forms.Button();
            this.textfilterBox = new System.Windows.Forms.TextBox();
            this.labelSelect = new System.Windows.Forms.Label();
            this.panelFileNames = new System.Windows.Forms.Panel();
            this.joblistView = new System.Windows.Forms.ListView();
            this.datasetID = new System.Windows.Forms.ColumnHeader();
            this.jobnum = new System.Windows.Forms.ColumnHeader();
            this.fileName = new System.Windows.Forms.ColumnHeader();
            this.alias = new System.Windows.Forms.ColumnHeader();
            this.block = new System.Windows.Forms.ColumnHeader();
            this.runOrder = new System.Windows.Forms.ColumnHeader();
            this.lcColumnID = new System.Windows.Forms.ColumnHeader();
            this.batchID = new System.Windows.Forms.ColumnHeader();
            this.mpanel_loading = new System.Windows.Forms.Panel();
            this.mlabel_percentLoadingComplete = new System.Windows.Forms.Label();
            this.mprogressBar_datasetLoading = new System.Windows.Forms.ProgressBar();
            this.panelStep.SuspendLayout();
            this.panelFileNames.SuspendLayout();
            this.mpanel_loading.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(679, 64);
            this.Banner.Subtitle = "Specify patial names for datasets in DMS";
            this.Banner.Title = "Step 2. Select Job Numbers for Analysis";
            // 
            // panelStep
            // 
            this.panelStep.Controls.Add(this.buttonClearAll);
            this.panelStep.Controls.Add(this.buttonToggleAll);
            this.panelStep.Controls.Add(this.mbtnFilter);
            this.panelStep.Controls.Add(this.textfilterBox);
            this.panelStep.Controls.Add(this.labelSelect);
            this.panelStep.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStep.Location = new System.Drawing.Point(134, 64);
            this.panelStep.Name = "panelStep";
            this.panelStep.Size = new System.Drawing.Size(679, 40);
            this.panelStep.TabIndex = 2;
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonClearAll.Enabled = false;
            this.buttonClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearAll.Location = new System.Drawing.Point(382, 8);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(64, 24);
            this.buttonClearAll.TabIndex = 4;
            this.buttonClearAll.Text = "Clear All";
            this.buttonClearAll.UseVisualStyleBackColor = false;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonToggleAll
            // 
            this.buttonToggleAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonToggleAll.Enabled = false;
            this.buttonToggleAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonToggleAll.Location = new System.Drawing.Point(312, 8);
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
            this.panelFileNames.Size = new System.Drawing.Size(679, 446);
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
            this.runOrder,
            this.lcColumnID,
            this.batchID});
            this.joblistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.joblistView.FullRowSelect = true;
            this.joblistView.GridLines = true;
            this.joblistView.Location = new System.Drawing.Point(0, 0);
            this.joblistView.Name = "joblistView";
            this.joblistView.Size = new System.Drawing.Size(679, 446);
            this.joblistView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.joblistView.TabIndex = 0;
            this.joblistView.UseCompatibleStateImageBehavior = false;
            this.joblistView.View = System.Windows.Forms.View.Details;
            this.joblistView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.joblistView_ItemCheck);
            this.joblistView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.joblistView_ColumnClick);
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
            // lcColumnID
            // 
            this.lcColumnID.Text = "Column ID";
            this.lcColumnID.Width = 77;
            // 
            // batchID
            // 
            this.batchID.Text = "Batch ID";
            // 
            // mpanel_loading
            // 
            this.mpanel_loading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpanel_loading.Controls.Add(this.mprogressBar_datasetLoading);
            this.mpanel_loading.Controls.Add(this.mlabel_percentLoadingComplete);
            this.mpanel_loading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mpanel_loading.Location = new System.Drawing.Point(134, 550);
            this.mpanel_loading.Name = "mpanel_loading";
            this.mpanel_loading.Padding = new System.Windows.Forms.Padding(3);
            this.mpanel_loading.Size = new System.Drawing.Size(679, 27);
            this.mpanel_loading.TabIndex = 4;
            // 
            // mlabel_percentLoadingComplete
            // 
            this.mlabel_percentLoadingComplete.BackColor = System.Drawing.Color.Transparent;
            this.mlabel_percentLoadingComplete.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mlabel_percentLoadingComplete.Dock = System.Windows.Forms.DockStyle.Right;
            this.mlabel_percentLoadingComplete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mlabel_percentLoadingComplete.Location = new System.Drawing.Point(635, 3);
            this.mlabel_percentLoadingComplete.MaximumSize = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.MinimumSize = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.Name = "mlabel_percentLoadingComplete";
            this.mlabel_percentLoadingComplete.Size = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.TabIndex = 10;
            this.mlabel_percentLoadingComplete.Text = "100%";
            this.mlabel_percentLoadingComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mlabel_percentLoadingComplete.Visible = false;
            // 
            // mprogressBar_datasetLoading
            // 
            this.mprogressBar_datasetLoading.BackColor = System.Drawing.Color.White;
            this.mprogressBar_datasetLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mprogressBar_datasetLoading.ForeColor = System.Drawing.Color.Lime;
            this.mprogressBar_datasetLoading.Location = new System.Drawing.Point(3, 3);
            this.mprogressBar_datasetLoading.Name = "mprogressBar_datasetLoading";
            this.mprogressBar_datasetLoading.Size = new System.Drawing.Size(632, 19);
            this.mprogressBar_datasetLoading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.mprogressBar_datasetLoading.TabIndex = 9;
            this.mprogressBar_datasetLoading.Visible = false;
            // 
            // ctlSelectFilesFromDBFilterWizardPage
            // 
            this.Controls.Add(this.panelFileNames);
            this.Controls.Add(this.mpanel_loading);
            this.Controls.Add(this.panelStep);
            this.Name = "ctlSelectFilesFromDBFilterWizardPage";
            this.Size = new System.Drawing.Size(813, 577);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.panelStep, 0);
            this.Controls.SetChildIndex(this.mpanel_loading, 0);
            this.Controls.SetChildIndex(this.panelFileNames, 0);
            this.panelStep.ResumeLayout(false);
            this.panelStep.PerformLayout();
            this.panelFileNames.ResumeLayout(false);
            this.mpanel_loading.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void ctlSelectFilesFromDBFilterWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SetWizardButtons(Wizard.UI.WizardButtons.Back | Wizard.UI.WizardButtons.Next);
			if (joblistView.Items.Count > 0)
			{
				this.buttonToggleAll.Enabled = true;
			}
		}

		private void joblistView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{

			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == _lvwItemComparer.SortColumn)
			{
				// Reverse the current sort direction for this column.
                if (_lvwItemComparer.Order == System.Windows.Forms.SortOrder.Ascending)
				{
                    _lvwItemComparer.Order = System.Windows.Forms.SortOrder.Descending;
				}
				else
				{
                    _lvwItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				_lvwItemComparer.SortColumn = e.Column;
                _lvwItemComparer.Order = System.Windows.Forms.SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.joblistView.Sort();
        }


        #region Loading Data Sets Filters


        /// <summary>
        /// Invokable method for adding a dataset to the listview and internal array list structure.
        /// </summary>
        /// <param name="dataset"></param>
        private void AddDatasetToLists(MultiAlign.clsDatasetInfo dataset)
        {
            joblistView.BeginUpdate();
            AddToList(dataset);
            AddToArrayList(dataset);
            joblistView.EndUpdate();
        }

        /// <summary>
        /// Updates the loading progress bar to tell the user how much has been loaded.
        /// </summary>
        /// <param name="value"></param>
        private void UpdateDatasetLoadingProgressBar(int value)
        {
            mprogressBar_datasetLoading.Value = value;
            int percent = System.Convert.ToInt32(100.0*(System.Convert.ToDouble(value) / System.Convert.ToDouble(mprogressBar_datasetLoading.Maximum)));
            mlabel_percentLoadingComplete.Text = percent.ToString() + "%";

            if (value == mprogressBar_datasetLoading.Maximum)
            {
                mprogressBar_datasetLoading.Visible = false;
                mlabel_percentLoadingComplete.Visible = false;
            }
        }

        /// <summary>
        /// Loads the datasets from the filter provided by the UI.  The datasets should have already been loaded to the marrDatasetID's array.
        /// </summary>
        private void LoadDatasetsFromFilters()
        {
            ///
            /// Turn off the updates for the list view if we have a large number of them.                
            ///
            /*for (int numDataset = 0; numDataset < marrDatasetIDs.Count; numDataset++)
            {*/

            //}

            marrDatasetInfoCurrent.Clear();
            string alias = "";  //TODO: BLL Change the alias here.  marrDatasetIDs[numDataset].ToString();   // Aliases                    
            datasetId    = "";  //TODO: BLL CHANGED THIS marrDatasetIDs[numDataset].ToString();      // Dataset id found.
           
            //BLL CHANGED SetupQuery(datasetId, 0);                               // query all fields (flag = 0)
            string query = SetupQuery(textfilterBox.Text, 0);                               // query all fields (flag = 0)
            LoadData(query);                              // this will fill the marrDatasetInfoCurrent

            /// 
            /// Add datasets to the list
            /// 
            for (int i = 0; i < marrDatasetInfoCurrent.Count; i++)
            {
                if (InvokeRequired == true)
                {
                    Invoke(new DelegateAddDataSetToList(AddDatasetToLists), marrDatasetInfoCurrent[i]);
                }
                else
                {
                    AddDatasetToLists(marrDatasetInfoCurrent[i] as MultiAlign.clsDatasetInfo);
                }
            }

            /// 
            /// Update the ui progress bar, by giving it the progress of the number of files loaded.
            /// 
            int num = 410;
            if (InvokeRequired == true)
                Invoke(new DelegateUpdateLoadingProgress(UpdateDatasetLoadingProgressBar), num);
            else
                UpdateDatasetLoadingProgressBar(num); 
        }

        public void LoadFilteredData()
        {
            /// 
            /// Make sure the user supplied a regular expression filter
            /// 
            if (textfilterBox.Text.Length == 0)
            {
                MessageBox.Show("You must enter a part of the dataset name first!", "Ooops!");
                return;
            }
            bool getIDs = false;

            string wildcardFilter = textfilterBox.Text + "%";            
            string query          = SetupQuery(wildcardFilter, 1);                      // query only datasetIDs (flag <> 0)

            int numberOfDatasetIDs = GetDatasetIDCount(query);
            //RemoveDuplicateIDs();

            /*if (marrDatasetIDs.Count == 0)
            {
                MessageBox.Show("No files found! Modify your search string.", "Try again...", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            else if (marrDatasetIDs.Count > 100) //max number of files
            {                
                DialogResult result = MessageBox.Show(this, "This query will fetch " +
                    Convert.ToString(marrDatasetIDs.Count) + " files. Continue?",
                    "Continue ?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    getIDs = true;
                else
                    marrDatasetIDs.Clear();
            }
            else
            {
                getIDs = true;
            }

            if (getIDs == true)
            {
             *
             */

            ///
            /// Make sure we have some dataset ID's to retrieve
            /// 
            if (numberOfDatasetIDs > 0)
            {
                /// 
                /// We are ok to start grabbing files now, so grab them
                /// 
                mprogressBar_datasetLoading.Visible     = true;                
                mlabel_percentLoadingComplete.Visible   = true;
                mprogressBar_datasetLoading.Minimum = 0;
                mprogressBar_datasetLoading.Maximum = marrDatasetIDs.Count;

                /// 
                /// Create a new thread to do the loading.  
                /// 
                ThreadStart threadStart = new ThreadStart(LoadDatasetsFromFilters);
                if (mobj_loadingThread != null)
                {
                    try
                    {
                       mobj_loadingThread.Abort();
                    }catch(ThreadAbortException abortException)
                    {
                    }
                    mobj_loadingThread = null;
                }
                mobj_loadingThread = new Thread(threadStart);

                try
                {
                    mobj_loadingThread.Start();                    
                }
                catch(Exception ex)
                {
                    /// 
                    /// Do nothing here... we should say that the loading failed though!
                    /// 
                    MessageBox.Show("Loading the datasets failed for an unknown reason.  " + ex.Message);
                }             
            }
        }

        #endregion

        /// <summary>
        /// Event handler when the ok button to find regular expressions is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void mbtnOKFilter_Click(object sender, System.EventArgs e)
		{
            LoadFilteredData();
		}


		private int SelectDataSet()
		{
			int selectedFIndex = 0 ;
			string []dataset_names = new string [marrDatasetInfoCurrent.Count] ; 
			string []file_names = new string [marrDatasetInfoCurrent.Count] ;

			for (int fileNum = 0 ; fileNum < marrDatasetInfoCurrent.Count ; fileNum++)
			{
				dataset_names[fileNum] = 
					((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrResultsFolder; 
				file_names[fileNum] = 
					((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrDatasetName;
			}

			if (!defaultSelected)
			{
				frmSelectOneDataset frmOneDataset = new frmSelectOneDataset() ; 
				frmOneDataset.SetDatasets(dataset_names) ;
				frmOneDataset.SetDatasetNames(file_names) ;
				frmOneDataset.setDatasetID = datasetId;
				if(frmOneDataset.ShowDialog(this) == DialogResult.OK)
				{
					selectedFIndex = frmOneDataset.SelectedFileIndex ; 
					selection = frmOneDataset.Selection ;
					defaultSelected = frmOneDataset.DefaultChecked ;
					if (selectedFIndex < 0)
						selectedFIndex = SwitchSelection(selection) ;
				}
				else
				{
					MessageBox.Show("Since nothing was chosen, skipping file.") ; 
				}
			}
			else
			{
				selectedFIndex = SwitchSelection(selection) ;
			}
			
			return selectedFIndex ;
		}


		private int SwitchSelection(enmSelectType selection)
		{
			int selectedFIndex = 0 ;
			
			switch (selection)
			{
				case enmSelectType.Decon2LS :
					
					for (int fileNum = 0 ; fileNum < marrDatasetInfoCurrent.Count ; fileNum++)
					{
						string datasetName = ((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrDatasetName;
						if (datasetName.ToLower().IndexOf("_isos.csv") > 0 )
						{
							selectedFIndex = fileNum ;
							//break ;
						}
					}
					break ;
				case enmSelectType.ICR2LS :
					for (int fileNum = 0 ; fileNum < marrDatasetInfoCurrent.Count ; fileNum++)
					{
						string datasetName = ((MultiAlign.clsDatasetInfo) marrDatasetInfoCurrent[fileNum]).mstrDatasetName;
						if (datasetName.ToLower().IndexOf(".pek") > 0 )
						{
							selectedFIndex = fileNum ;
							//break ;
						}
					}
					break ;
				case enmSelectType.NEWEST :
					selectedFIndex = marrDatasetInfoCurrent.Count - 1 ;
					break ;
				default :
					selectedFIndex = -1 ;
					break ;
			}
			return selectedFIndex ;
		}


		private void AddToList(MultiAlign.clsDatasetInfo datasetInfo)
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
			
			if (datasetInfo.mintColumnID == 0)
				dataItem.SubItems.Add("NA") ;
			else
				dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintColumnID)) ;
			
			if (datasetInfo.mintBatchID == 0)
				dataItem.SubItems.Add("NA") ;
			else
				dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintBatchID)) ;

			dataItem.Checked = datasetInfo.selected ;
			dataItem.Tag = joblistView.Items.Count ;
			joblistView.Items.Add(dataItem) ;
		}

		
		private void AddToArrayList(MultiAlign.clsDatasetInfo datasetInfo)
		{
			marrDatasetInfo.Add(datasetInfo) ; 
		}

		
		private string SetupQuery(string field, int flag)
		{
			string selectQry = null;
			if (flag == 0)
			{
                /// 
                /// SELECT
                /// 
				selectQry = "SELECT     ";
                selectQry += "  dbo.T_Dataset.Dataset_ID AS DatasetID " ;                               // 0
				selectQry += ", dbo.t_storage_path.SP_vol_name_client as volName " ;                    // 1 
				selectQry += ", dbo.t_storage_path.SP_path as path " ;                                  // 2
				selectQry += ", dbo.T_Dataset.DS_folder_name as datasetFolder " ;                       // 3
				selectQry += ", dbo.T_Analysis_Job.AJ_resultsFolderName resultsFolder " ;               // 4
				selectQry += ", dbo.T_Dataset.Dataset_Num AS datasetName " ;                            // 5
				selectQry += ", dbo.T_Analysis_Job.AJ_jobID as JobId " ;                                // 6
				selectQry += ", dbo.T_Dataset.DS_LC_column_ID as ColumnID " ;                           // 7
				selectQry += ", dbo.T_Dataset.Acq_Time_Start as AcquisitionTime" ;                      // 8
				selectQry += ", dbo.T_Experiments.Ex_Labelling as Labelling" ;                          // 9
				selectQry += ", dbo.T_Instrument_Name.IN_Name as InstrumentName" ;                      //10
				selectQry += ", dbo.T_Analysis_Job.AJ_analysisToolID as ToolID" ;                       //11
				selectQry += ", dbo.T_Requested_Run_History.RDS_Block as BlockNum" ;                    //12
				selectQry += ", dbo.T_Requested_Run_History.RDS_Name as ReplicateName" ;                //13
				selectQry += ", dbo.T_Dataset.Exp_ID as ExperimentID" ;                                 //14
				selectQry += ", dbo.T_Requested_Run_History.RDS_Run_Order as RunOrder" ;                //15
				selectQry += ", dbo.T_Requested_Run_History.RDS_BatchID as BatchID" ;                   //16
				selectQry += ", dbo.V_Dataset_Folder_Paths.Archive_Folder_Path AS ArchPath" ;           //17 
				selectQry += ", dbo.V_Dataset_Folder_Paths.Dataset_Folder_Path AS DatasetFullPath" ;    //18
                ///
                /// From - Inner Join
                /// 
				selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON " ; 
				selectQry += " dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID " ; 
				selectQry += " INNER JOIN dbo.T_Experiments ON dbo.T_Dataset.Exp_ID = dbo.T_Experiments.Exp_ID" ; 
				selectQry += " INNER JOIN dbo.T_Instrument_Name ON dbo.T_Dataset.DS_instrument_name_id = dbo.T_Instrument_Name.Instrument_ID " ; 
				selectQry += " INNER JOIN dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID " ;
				selectQry += " left outer JOIN dbo.T_Requested_Run_History ON dbo.T_Requested_Run_History.DatasetID = dbo.T_Dataset.Dataset_ID" ; 
				selectQry += " INNER JOIN dbo.V_Dataset_Folder_Paths ON dbo.T_Dataset.Dataset_ID = dbo.V_Dataset_Folder_Paths.Dataset_ID" ;
                /// 
                /// WHERE
                /// 
				selectQry += " WHERE  (dbo.T_Analysis_Job.AJ_analysisToolID = 2 OR ";
                selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 7 OR ";
                selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 10 OR ";
                selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 11 OR ";
                selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 12 OR ";
                selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 16 OR ";
                selectQry += "         dbo.T_Analysis_Job.AJ_analysisToolID = 18 ) ";
                selectQry += "         AND dbo.T_Dataset.Dataset_Num LIKE \'" + field + "\'";    
                
                /// 
                /// BLL - Try this, instead of pounding the database with a crazy amount of 
                /// queries and reads, lets just do one, for all datasets that have this regular expression                
                /// 
                //selectQry += " AND dbo.T_Dataset.Dataset_ID = " + field ;        
			}
			else
			{
                /// 
                /// SELECT
                /// 
				selectQry = "SELECT     dbo.T_Dataset.Dataset_ID AS DatasetID " ;
                /// From - Inner Join
				selectQry += " FROM dbo.T_Dataset INNER JOIN dbo.T_Analysis_Job ON " ; 
				selectQry += "      dbo.T_Dataset.Dataset_ID = dbo.T_Analysis_Job.AJ_datasetID INNER JOIN " ; 
				selectQry += "      dbo.t_storage_path ON dbo.T_Dataset.DS_storage_path_ID = dbo.t_storage_path.SP_path_ID " ;
                /// 
                /// WHERE
                /// 
				selectQry += " WHERE  ( dbo.T_Analysis_Job.AJ_analysisToolID = 2 OR";
                selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 7  OR ";
                selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 10 OR ";
                selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 11 OR ";
                selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 12 OR ";
                selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 16 OR ";
                selectQry += "          dbo.T_Analysis_Job.AJ_analysisToolID = 18 ) "; 
				selectQry += "          AND dbo.T_Dataset.Dataset_Num like \'" + field + "\'" ; 
			}			
			mstrQuery = selectQry ;

            return selectQry;
		}


		#region Get data from the DB

        /// <summary>
        /// Gets the number of dataset ID's stored in the database.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
		private int GetDatasetIDCount(string query)
		{
            int datasetCount = 0;
			string server = "gigasax" ; 
			string userName = "dmswebuser" ; 
			string passwd = "icr4fun" ; 
			string cString = String.Format("database=DMS5;server={0};user id={1};Password={2}", 
					server, userName, passwd);

			SqlConnection myConnection = new SqlConnection(cString);
			try
			{
				myConnection.Open();
			}
			catch(Exception e)
			{
				MessageBox.Show("Cannot open connection to database.", "Database Connection Error.");
				Console.WriteLine(e.Message);
				return 0;
			}
			
			//SqlCommand myCommand = new SqlCommand(mstrQuery,myConnection);
			//myCommand.CommandType = CommandType.Text;
			//SqlDataReader myReader = myCommand.ExecuteReader();

            DataTable table         = new DataTable();
            SqlDataAdapter adapter  = new SqlDataAdapter(query, myConnection);
			try 
			{
                adapter.Fill(table);

				/*while (myReader.Read()) 
				{
					string dataSetID = null;
				    dataSetID = Convert.ToString(myReader.GetInt32(0)); 
					marrDatasetIDs.Add(dataSetID) ; 
				}
                 */

                datasetCount = table.Rows.Count;

			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message +  "DMS get information error") ;  
			}
			finally 
			{
				// always call Close when done reading.
				//myReader.Close();
				// always call Close when done reading.
				myConnection.Close();
			}

            return datasetCount;
		}

        /// <summary>
        /// Gets the file name from the database path provided.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
		private string GetFileNameFromDBPath(string sourcePath)
		{
			string FileName = null ;

			System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourcePath);
			foreach (System.IO.FileInfo f in dir.GetFiles())
			{
				if (f.Name.ToLower().IndexOf(".pek") > 0)
				{
					if(f.Name.IndexOf("_ic.pek") == -1)
						FileName = f.Name ;
				}
				else if (f.Name.ToLower().IndexOf("_isos.csv") != -1)
				{
					FileName = f.Name ;
				}
			}
			return FileName ;
		}

				
        /*
		private void GetData(string datasetId, string alias)
		{
			string pekFilePath ;
			string FileName ;

			string server = "gigasax" ; 
			string userName = "dmswebuser" ; 
			string passwd = "icr4fun" ; 
			string cString = String.Format("database=DMS5;server={0};user id={1};Password={2}", 
				server, userName, passwd);

			//			string cString = "Persist Security Info=False;Integrated Security=SSPI;database=DMS5;server=gigasax";
			SqlConnection myConnection = new SqlConnection(cString);
			myConnection.Open();


			SqlCommand myCommand = new SqlCommand(mstrQuery,myConnection);
			myCommand.CommandType = CommandType.Text;
			SqlDataReader myReader = myCommand.ExecuteReader();
			try 
			{
				Type labelType = typeof(MultiAlign.LabelingType) ; 
				MultiAlign.LabelingType [] labelTypes = (MultiAlign.LabelingType []) Enum.GetValues(labelType) ; 
				while (myReader.Read()) 
				{
					MultiAlign.clsDatasetInfo datasetInfo = new MultiAlign.clsDatasetInfo() ; 
					datasetInfo.mstrDatasetId = Convert.ToString(myReader.GetInt32(0)) ; 
					datasetInfo.mstrVolume = myReader.GetString(1) ; 
					datasetInfo.mstrInstrumentFolder = myReader.GetString(2) ; 
					datasetInfo.mstrDatasetPath = myReader.GetString(3) ;
					datasetInfo.mstrResultsFolder = myReader.GetString(4) ;
					//datasetInfo.mstrDatasetName = myReader.GetString(5) ;
					datasetInfo.mstrAnalysisJobId = Convert.ToString(myReader.GetInt32(6)) ;

					datasetInfo.mintColumnID = myReader.GetInt32(7) ;
					datasetInfo.mdateAcquisitionStart = myReader.GetDateTime(8) ;
					string labelMedia = myReader.GetString(9) ; 
					labelMedia.Replace("_", "") ; 
					labelMedia.Replace(" ", "") ; 
					labelMedia.Replace("/", "") ; 
					labelMedia.Replace("-", "") ; 
					foreach (MultiAlign.LabelingType type in labelTypes)
					{
						if (type.ToString() == labelMedia)
						{
							datasetInfo.menmLabelingType = type ; 
							break ; 
						}
					}
					datasetInfo.mstrInstrment = myReader.GetString(10) ; 
					int toolId = myReader.GetInt32(11) ; 
					if (toolId == 16 || toolId == 18)
					{
						datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.Decon2LS ; 
					}
					else
					{
						datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.ICR2LS ; 
					}
					if (!myReader.IsDBNull(12))
					{
						datasetInfo.mintBlockID = myReader.GetInt32(12) ; 
					}
					else
					{
						datasetInfo.mintBlockID = 0 ; 
					}
					if (!myReader.IsDBNull(13))
					{
						datasetInfo.mstrReplicateName = myReader.GetString(13) ; 
					}
					else
					{
						datasetInfo.mstrReplicateName = "" ; 
					}
					if (!myReader.IsDBNull(14))
					{
						datasetInfo.mintExperimentID = myReader.GetInt32(14) ; 
					}
					else
					{
						datasetInfo.mintExperimentID = 0 ; 
					}
					if (!myReader.IsDBNull(15))
					{
						datasetInfo.mintRunOrder = myReader.GetInt32(15) ; 
					}
					else
					{
						datasetInfo.mintRunOrder = 0 ; 
					}
					if (!myReader.IsDBNull(16))
					{
						datasetInfo.mintBatchID = myReader.GetInt32(16) ; 
					}
					else
					{
						datasetInfo.mintBatchID = 0 ; 
					}

					pekFilePath = myReader.GetString(17) + "\\" + datasetInfo.mstrResultsFolder ;
					FileName = GetFileNameFromDBPath(pekFilePath) ;
					datasetInfo.mstrLocalPath = pekFilePath + "\\" + FileName ;
					datasetInfo.mstrDatasetName = FileName ;

					datasetInfo.mstrAlias = alias ; 
					datasetInfo.selected = false;
					marrDatasetInfoCurrent.Add(datasetInfo) ; 
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message +  "DMS get information error") ;  
			}
			finally 
			{
				// always call Close when done reading.
				myReader.Close();
				// always call Close when done reading.
				myConnection.Close();
			}         
		}
         */

        /// <summary>
        /// Loads the data from the data table into the marr dataset array.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="pekFilePath"></param>
        /// <param name="FileName"></param>
        private void LoadDataAdapterResultsIntoArray(DataTable table)
        {
                string pekFilePath;
                string fileName;

                Type labelType = typeof(MultiAlign.LabelingType) ; 
				MultiAlign.LabelingType [] labelTypes = (MultiAlign.LabelingType []) Enum.GetValues(labelType) ;

                /// 
                /// For each result stored in a row of the datatable found, load it 
                /// into a dataset info class.
                /// 
                foreach (DataRow row in table.Rows)
                {

                    string alias;
                    MultiAlign.clsDatasetInfo datasetInfo   = new MultiAlign.clsDatasetInfo();
                    datasetInfo.mstrDatasetId               = Convert.ToString(row[0]);
                    alias = datasetInfo.mstrDatasetId;      /// This was set by default to be the same thing as the dataset id.
                    datasetInfo.mstrVolume                  = Convert.ToString(row[1]);
                    datasetInfo.mstrInstrumentFolder        = Convert.ToString(row[2]);
                    datasetInfo.mstrDatasetPath             = Convert.ToString(row[3]);
                    datasetInfo.mstrResultsFolder           = Convert.ToString(row[4]);
                    datasetInfo.mstrAnalysisJobId           = Convert.ToString(row[6]);
                    datasetInfo.mintColumnID                = Convert.ToInt32(row[7]);
                    datasetInfo.mdateAcquisitionStart       = Convert.ToDateTime(row[8]);
                    string labelMedia                       = Convert.ToString(row[9]);

                    labelMedia.Replace("_", "");
                    labelMedia.Replace(" ", "");
                    labelMedia.Replace("/", "");
                    labelMedia.Replace("-", "");

                    foreach (MultiAlign.LabelingType type in labelTypes)
                    {
                        if (type.ToString() == labelMedia)
                        {
                            datasetInfo.menmLabelingType = type;
                            break;
                        }
                    }

                    datasetInfo.mstrInstrment = Convert.ToString(row[10]);
                    int toolId = Convert.ToInt32(row[11]);
                    /// 
                    /// Tool Type
                    /// 
                    if (toolId == 16 || toolId == 18)
                    {
                        datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.Decon2LS;
                    }
                    else
                    {
                        datasetInfo.menmDeisotopingTool = MultiAlign.DeisotopingTool.ICR2LS;
                    }

                    /// 
                    /// Block ID
                    /// 
                    if (row.IsNull(12) == false)
                    {
                        datasetInfo.mintBlockID = Convert.ToInt32(row[12]);
                    }
                    else
                    {
                        datasetInfo.mintBlockID = 0;
                    }

                    /// 
                    /// Replicate Name
                    /// 
                    
                    if (row.IsNull(13) == false)
                    {
                        datasetInfo.mstrReplicateName = Convert.ToString(row[13]);
                    }
                    else
                    {
                        datasetInfo.mstrReplicateName = "";
                    }

                    /// 
                    /// Experiment ID
                    /// 
                    if (row.IsNull(14) == false) 
                    {
                        datasetInfo.mintExperimentID = Convert.ToInt32(row[14]);
                    }
                    else
                    {
                        datasetInfo.mintExperimentID = 0;
                    }

                    /// 
                    /// Run Order
                    /// 
                    if (row.IsNull(15) == false)
                    {
                        datasetInfo.mintRunOrder = Convert.ToInt32(row[15]);
                    }
                    else
                    {
                        datasetInfo.mintRunOrder = 0;
                    }

                    /// 
                    /// Batch ID
                    /// 
                    if (row.IsNull(16) == false)
                    {
                        datasetInfo.mintBatchID = Convert.ToInt32(row[16]); 
                    }
                    else
                    {
                        datasetInfo.mintBatchID = 0;
                    }

                    pekFilePath                 = row[17] + "\\" + datasetInfo.mstrResultsFolder;
                    fileName                    = GetFileNameFromDBPath(pekFilePath);
                    datasetInfo.mstrLocalPath   = pekFilePath + "\\" + fileName;
                    datasetInfo.mstrDatasetName = fileName;

                    datasetInfo.mstrAlias = alias;
                    datasetInfo.selected = false;
                    marrDatasetInfoCurrent.Add(datasetInfo);
                }
        }

        /// <summary>
        /// Gets data for the given datset id.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <param name="alias"></param>
        private void LoadData(string query)
        {
            string server = "gigasax";
            string userName = "dmswebuser";
            string passwd = "icr4fun";
            string cString = String.Format("database=DMS5;server={0};user id={1};Password={2}",
                server, userName, passwd);

            SqlConnection myConnection = new SqlConnection(cString);
            myConnection.Open();


            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, myConnection);

            try
            {
                DataTable results = new DataTable();
                dataAdapter.Fill(results);
                LoadDataAdapterResultsIntoArray(results);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("An error occured while trying to load data for dataset id {0}.  {1}",
                                                                    datasetId, ex.Message));
            }
            finally
            {
                myConnection.Close();
                myConnection.Dispose();
            }
        }

		
		#endregion

		private void RemoveDuplicateIDs()
		{
			marrDatasetIDs.Sort();
			int i = 0;
			while(i < marrDatasetIDs.Count - 1) 
			{
				if (marrDatasetIDs[i].ToString() == marrDatasetIDs[i+1].ToString())
				{
					marrDatasetIDs.RemoveAt(i);
				} 
				else 
				{
					i++;
				}
			}
		}


		private bool DuplicateDatasetExists(MultiAlign.clsDatasetInfo mclsDsetInfo)
		{
			int idxFound = 0;
			bool found = false ;
			string currAnalJID = mclsDsetInfo.mstrAnalysisJobId ;
			while(idxFound < marrDatasetInfo.Count) 
			{
				string orgAnalJID = ((MultiAlign.clsDatasetInfo)marrDatasetInfo[idxFound]).mstrAnalysisJobId ;

				if (currAnalJID.Equals(orgAnalJID))
				{
					found = true ;
					break ;
				} 
				else 
				{
					found = false ;
				}
				idxFound++ ;
			}
			return found ;
		}

		private void filterBox_TextChanged(object sender, System.EventArgs e)
		{
			if (textfilterBox.Text.Length == 0)
				mbtnFilter.Enabled = false;
			else
				mbtnFilter.Enabled = true;
		}

		
		private void SelectCheckedItems()
		{
			int originalIndex ;
			marrDatasetInfoChecked.Clear() ;
			ListView.CheckedIndexCollection indexes = joblistView.CheckedIndices ;
			//int N = indexes.Count ;
			foreach ( int i in indexes )
			{
				originalIndex = Convert.ToInt16(joblistView.Items[i].Tag) ;
				marrDatasetInfoChecked.Add(marrDatasetInfo[originalIndex]) ;
			}
			//FilesSelected = true;
		}

		
		private void joblistView_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (joblistView.CheckedIndices.Count == 0 && e.NewValue == CheckState.Unchecked)
			{
				FilesSelected = false;
			}
			else
			{
				FilesSelected = true;
				//SelectCheckedItems() ;
			}
		}

		
		private void buttonToggleAll_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < joblistView.Items.Count; i++)
			{
				if (joblistView.Items[i].Checked == true)
				{
					joblistView.Items[i].Checked = false;
					((MultiAlign.clsDatasetInfo)marrDatasetInfo[i]).selected = false;
				}
				else
				{
					joblistView.Items[i].Checked = true;
					((MultiAlign.clsDatasetInfo)marrDatasetInfo[i]).selected = true;
				}
			}
		}

		private void buttonClearAll_Click(object sender, System.EventArgs e)
		{
			joblistView.Items.Clear();
			marrDatasetInfo.Clear();
			buttonClearAll.Enabled = false ;
			buttonToggleAll.Enabled = false ;
		}
							
		#region Accessors -----------------------------------------

		public ArrayList DataSetInfo
		{
			get
			{
				if (FilesSelected)
					SelectCheckedItems();

				return marrDatasetInfoChecked ;
			}
			set
			{
				marrDatasetInfoChecked = value ;
			}
		}
		#endregion
	}
	

	#region Class that Implements the manual sorting of items by columns.
	// Implements the manual sorting of items by columns.
	public class ListViewItemComparer : IComparer
	{
		// Specifies the column to be sorted
		private int ColumnToSort;

		// Specifies the order in which to sort (i.e. 'Ascending').
		private System.Windows.Forms.SortOrder OrderOfSort;

		// Case insensitive comparer object
		private CaseInsensitiveComparer ObjectCompare;

		// Class constructor, initializes various elements
		public ListViewItemComparer()
		{
			// Initialize the column to '0'
			ColumnToSort = 0;

			// Initialize the sort order to 'none'
            OrderOfSort = System.Windows.Forms.SortOrder.None;

			// Initialize the CaseInsensitiveComparer object
			ObjectCompare = new CaseInsensitiveComparer();
		}

		// This method is inherited from the IComparer interface.
		// It compares the two objects passed using a case
		// insensitive comparison.
		//
		// x: First object to be compared
		// y: Second object to be compared
		//
		// The result of the comparison. "0" if equal,
		// negative if 'x' is less than 'y' and
		// positive if 'x' is greater than 'y'
		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			// Case insensitive Compare
			compareResult = ObjectCompare.Compare (
				listviewX.SubItems[ColumnToSort].Text,
				listviewY.SubItems[ColumnToSort].Text
				);

			// Calculate correct return value based on object comparison
            if (OrderOfSort == System.Windows.Forms.SortOrder.Ascending)
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
            else if (OrderOfSort == System.Windows.Forms.SortOrder.Descending)
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}

		// Gets or sets the number of the column to which to
		// apply the sorting operation (Defaults to '0').
		public int SortColumn
		{
			set
			{
				ColumnToSort = value;
			}
			get
			{
				return ColumnToSort;
			}
		}

		// Gets or sets the order of sorting to apply
		// (for example, 'Ascending' or 'Descending').
        public System.Windows.Forms.SortOrder Order
		{
			set
			{
				OrderOfSort = value;
			}
			get
			{
				return OrderOfSort;
			}
		}
	}
	#endregion
}
