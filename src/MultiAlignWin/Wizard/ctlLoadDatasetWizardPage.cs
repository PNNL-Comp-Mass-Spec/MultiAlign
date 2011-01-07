using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using MultiAlignEngine;
using PNNLControls;
using PNNLProteomics.IO;
using PNNLProteomics.Data;

using MultiAlignWin.Forms;
using MultiAlignWin.Network;




namespace MultiAlignWin
{
    /// <summary>
    /// Class that determines how to load a dataset from the wizard page.
    /// </summary>
    public partial class ctlLoadDatasetWizardPage : Wizard.UI.InternalWizardPage
    {        
        #region Members
        /// <summary>
        /// OpenFileDialog that will allow the user to select files to load from the disk.
        /// </summary>
        private OpenFileDialog mobj_openFileDialog;
        /// <summary>
        /// File dialog for loading datasets from a file that has dataset id information.
        /// </summary>
        private OpenFileDialog mobj_openJobIDDialog;
        /// <summary>
        /// Class that tests the connection to DMS.
        /// </summary>
        private clsDMSConnectionTester mobj_connectionTester;
        /// <summary>
        /// Loader to load files from disk.
        /// </summary>
        private DiskDatasetLoader mobj_diskLoader;
        /// <summary>
        /// Loader to load files from DMS.
        /// </summary>
        private DatabaseDatasetLoader mobj_dmsLoader;
        /// <summary>
        /// Loader that takes dataset information from a file and finds the id's in the DMS database.
        /// </summary>
        private DatabaseDatasetIDLoader mobj_dataJobLoader;
        /// <summary>
        /// ListViewItem comparer for moving columns and sorting.
        /// </summary>
        private ListViewItemComparer mobj_listViewItemComparer;
        /// <summary>
        /// Holds how many items are currently checked.
        /// </summary>
        private int mint_numberCheckedItems;
        /// <summary>
        /// The total number of datasets found by a loader for a load process.
        /// </summary>
        private int mint_datasetsFound;
        /// <summary>
        /// DMS server connection information.
        /// </summary>
        private clsDMSServerInformation mobj_serverInformation;
        /// <summary>
        /// Flag indicating if the system is loading dataset files.
        /// </summary>
        private bool mbool_isLoading;
        /// <summary>
        /// Flag indicating if the items have been sorted based off if they are checked.
        /// </summary>
        private bool mbool_sortChecked; 
        /// <summary>
        /// DMS Search options for filtering data. 
        /// </summary>
        private DMSDatasetSearchOptions m_searchOptions;
        #endregion
        
        /// <summary>
        /// Default constructor for the loading of the datasets wizard page.
        /// </summary>
        public ctlLoadDatasetWizardPage( )
        {
            m_searchOptions = new DMSDatasetSearchOptions();

            clsDMSServerInformation info = new clsDMSServerInformation();
            info = new clsDMSServerInformation();
            info.ServerName = Properties.Settings.Default.DMSServerName;
            info.DatabaseName = Properties.Settings.Default.DMSDatabaseName;
            info.Username = "dmswebuser";
            info.Password = "icr4fun";
            info.ConnectionTimeout = Properties.Settings.Default.DMSConnectionTimeout;
            Init(info);            
        }
        /// <summary>
        /// Constructor that uses server information to load the datasets from.
        /// </summary>
        /// <param name="info"></param>
        public ctlLoadDatasetWizardPage(clsDMSServerInformation info)
        {

            if (info == null)
            {
                info = new clsDMSServerInformation();
                info.ServerName        = Properties.Settings.Default.DMSServerName;
                info.DatabaseName      = Properties.Settings.Default.DMSDatabaseName;
                info.ConnectionTimeout = Properties.Settings.Default.DMSConnectionTimeout;
                info.Username = "dmswebuser";
                info.Password = "icr4fun";
            }
            Init(info);            
        }

        /// <summary>
        /// Initializes the source code.
        /// </summary>
        public void Init(clsDMSServerInformation information)
        {

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();


            mobj_serverInformation = information;

            mobj_openFileDialog         = new OpenFileDialog();
            mobj_openJobIDDialog        = new OpenFileDialog();
            mobj_dmsLoader              = new DatabaseDatasetLoader();
            mobj_diskLoader             = new DiskDatasetLoader();
            mobj_dataJobLoader          = new DatabaseDatasetIDLoader();
            mobj_connectionTester       = new clsDMSConnectionTester(information.ConnectionExists, information);
            mobj_listViewItemComparer   = new ListViewItemComparer();

            mobj_openFileDialog.Multiselect			= true;
            mobj_openFileDialog.Filter = "*_isos.csv files (*_isos.csv)|*_isos.csv|*.pek files (*.pek)|*.pek|LC-MS Feature files (*_LCMSFeatures.txt)|*_LCMSFeatures.txt|SQLite DB3 files (*.db3)|*.db3|All files (*.*)|*.*";
            mobj_openFileDialog.FilterIndex			= 1;
            mobj_openFileDialog.InitialDirectory	= Properties.Settings.Default.RawDataPath;

            mobj_openJobIDDialog.Multiselect         = false;
            mobj_openJobIDDialog.Filter              = "*.csv files (*.csv)|*.csv|All files (*.*)|*.*";
            mobj_openJobIDDialog.FilterIndex         = 1;
            mobj_openJobIDDialog.InitialDirectory    = Properties.Settings.Default.RawDataPath;


            mobj_dmsLoader.LoadedDataset            += new DelegateDataSetLoaded(LoadedDataset);
            mobj_dmsLoader.LoadingComplete          += new DelegateUpdateLoadingComplete(LoadingComplete);
            mobj_dmsLoader.LoadingProgress          += new DelegateUpdateLoadingPercentLoaded(LoadingProgress);
            mobj_dmsLoader.DatasetsFound            += new DelegateTotalDatasetsFound(DatasetsFound);
            
            mobj_diskLoader.LoadedDataset           += new DelegateDataSetLoaded(LoadedDataset);
            mobj_diskLoader.LoadingComplete         += new DelegateUpdateLoadingComplete(LoadingComplete);
            mobj_diskLoader.LoadingProgress         += new DelegateUpdateLoadingPercentLoaded(LoadingProgress);
            mobj_diskLoader.DatasetsFound           += new DelegateTotalDatasetsFound(DatasetsFound);

            mobj_dataJobLoader.LoadedDataset        += new DelegateDataSetLoaded(LoadedDataset);
            mobj_dataJobLoader.LoadingComplete      += new DelegateUpdateLoadingComplete(LoadingComplete);
            mobj_dataJobLoader.LoadingProgress      += new DelegateUpdateLoadingPercentLoaded(LoadingProgress);
            mobj_dataJobLoader.DatasetsFound        += new DelegateTotalDatasetsFound(DatasetsFound);
                        
            mobj_connectionTester.ConnectionPercent += new DelegateConnectionToDMSMadePercent(mobj_connectionTester_ConnectionPercent);
            mobj_connectionTester.ConnectionStatus  += new DelegateConnectionToDMS(mobj_connectionTester_ConnectionStatus);

            mlistView_datasets.ListViewItemSorter    = mobj_listViewItemComparer;
            mlistView_datasets.ColumnClick          += new ColumnClickEventHandler(mlistView_datasets_ColumnClick);
            mlistView_datasets.ContextMenuStrip      = mcontextMenu_dataset;
            mlistView_datasets.ItemChecked          += new ItemCheckedEventHandler(mlistView_datasets_ItemChecked);
            mlistView_datasets.DragDrop             += new DragEventHandler(mlistView_datasets_DragDrop);
            
            mobj_dmsLoader.ServerInformation         = information;
            mobj_connectionTester.ServerInformation  = information;
            mobj_dataJobLoader.ServerInformation     = information;

            /// 
            /// Load the dataset name used last by the user
            /// 
            mtextbox_databaseFilterName.Text = Properties.Settings.Default.UserDataLoaderRegEx;

            /// 
            /// Update how much data is available
            /// 
            mint_numberCheckedItems  = 0;
            SetActive               += new System.ComponentModel.CancelEventHandler(this.ctlSelectDataSourceWizardPage_SetActive);

            QueryCancel += new CancelEventHandler(ctlLoadDatasetWizardPage_QueryCancel);
        }

        void ctlLoadDatasetWizardPage_QueryCancel(object sender, CancelEventArgs e)
        {
            mobj_dataJobLoader.CancelLoadingDatasets();
            mobj_dmsLoader.CancelLoadingDatasets();
            mobj_diskLoader.Abort();
        }

        void mlistView_datasets_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            string[] formats = data.GetFormats();            
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
            mtextbox_databaseFilterName.Enabled         = canLoadFromDMS;
            mbutton_databaseDatasetSearch.Enabled       = canLoadFromDMS;
            m_showDMSSearchOptionsButton.Enabled = canLoadFromDMS;
            mbutton_databaseSelectDatasetIDFile.Enabled = canLoadFromDMS;
            if (canLoadFromDMS == true)
            {
                mlabel_networkConnectionTest.Text       = "Connection to DMS found!";
                mlabel_networkConnectionTest.ForeColor  = Color.Black;
            }
            else
            {
                mlabel_networkConnectionTest.Text       = "Could not find connection to DMS!";
                mlabel_networkConnectionTest.ForeColor  = Color.Red; 
            }
        }
        /// <summary>
        /// Hids the network connection test controls.
        /// </summary>
        private void HideNetworkControls()
        {
            mprogressBar_connectionTest.Visible     = false;
            mlink_showPNNLProperties.Enabled        = true;
            mlinkLabel_testConnection.Enabled       = true; 
        }
        /// <summary>
        /// Makes network testing controls visible.
        /// </summary>
        private void ShowNetworkControls()
        {
            mlabel_networkConnectionTest.Text = "Testing Connection To DMS...";
            mlabel_networkConnectionTest.ForeColor = Color.Black;
            
            mprogressBar_connectionTest.Visible     = true;
            mlink_showPNNLProperties.Enabled        = false;
            mlinkLabel_testConnection.Enabled       = false;
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
        private void TestNetworkConnection()
        {
            if (mobj_connectionTester.HasDMSConnection == false)
            {
                ShowNetworkControls();
                mobj_connectionTester.TestConnection();
            }
            else
            {
            }

        }
        private void mlinkLabel_testConnection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TestNetworkConnection();
        }
        #endregion

        #region Wizard Actions
        /// <summary>
        /// Handles when this becomes the active wizard page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctlSelectDataSourceWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetWizardButtons(Wizard.UI.WizardButtons.Back | Wizard.UI.WizardButtons.Next);
            UpdateTotalChecked(mint_numberCheckedItems);
            TestNetworkConnection();            
        }
        #endregion
        
        #region Data Loading Event Updates 
        /// <summary>
        /// Handles when a loader knows how many datasets are available.
        /// </summary>
        /// <param name="found"></param>
        void DatasetsFound(int found)
        {
            mint_datasetsFound = found;    
        }
        /// <summary>
        /// Updates the progress bar with the value provided.  The value is first clamped at the minimum and maximum of the progress bar.
        /// </summary>
        /// <param name="percent"></param>
        private void UpdateDatasetLoadingProgressBar(double percent)
        {
            mprogressBar_datasetLoading.Style = ProgressBarStyle.Continuous;
            mprogressBar_datasetLoading.Value = Math.Max(mprogressBar_datasetLoading.Minimum, 
                                                    Math.Min(mprogressBar_datasetLoading.Maximum, 
                                                    Convert.ToInt32(percent)));
            mlabel_percentLoadingComplete.Text = string.Format("{0}% Loaded.  {1} Datasets Left.",
                                                                    mprogressBar_datasetLoading.Value,
                                                                    mint_datasetsFound
                                                                    );
        }
        /// <summary>
        /// Captures when the loading progress is updated.
        /// </summary>
        /// <param name="percentLoaded">Amount was currently loaded.</param>
        void LoadingProgress(double percentLoaded)
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
        /// Captures when loading is complete.
        /// </summary>
        void LoadingComplete()
        {
            if (InvokeRequired == true)
            {
                Invoke(new DelegateUpdateLoadingState(SetLoadingState), false);
            }
            else
            {
                SetLoadingState(false);
            }
        }
        /// <summary>
        /// Captures when a data set is loaded.
        /// </summary>
        /// <param name="dataset">Dataset to load.</param>
        void LoadedDataset(DatasetInformation dataset)
        {
            if (InvokeRequired == true)
            {
                mint_datasetsFound--;
                Invoke(new DelegateDataSetLoaded(AddDatasetToListView), dataset);
            }
            else
            {
                AddDatasetToListView(dataset);
            }
        }
        #endregion

        #region Dataset Loading
        /// <summary>
        /// Hides the label and progress bar when the loading is complete.
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            try
            {
                mpanel_loading.Visible = isLoading;
                bool enableButtons = (isLoading == false);
                mbool_isLoading = isLoading;
                mbutton_clearAll.Enabled = enableButtons;
                mbutton_toggleAll.Enabled = enableButtons;
                mbutton_uncheckAll.Enabled = enableButtons;
                mbutton_checkAll.Enabled = enableButtons;
                mbutton_clearChecked.Enabled = enableButtons;
                mbutton_clearUnchecked.Enabled = enableButtons;
                toggleAllToolStripMenuItem.Enabled = enableButtons;
                selectAllToolStripMenuItem.Enabled = enableButtons;
                deselectAllToolStripMenuItem.Enabled = enableButtons;
                clearSelectedToolStripMenuItem.Enabled = enableButtons;
                clearAllToolStripMenuItem.Enabled = enableButtons;
                clearUnSelectedToolStripMenuItem.Enabled = enableButtons;
                mlink_showPNNLProperties.Enabled = enableButtons;

                bool enableDatabaseFeatures = false;
                if (enableButtons == true)
                {
                    enableDatabaseFeatures = mobj_connectionTester.HasDMSConnection;
                }
                mbutton_databaseDatasetSearch.Enabled       = enableDatabaseFeatures;
                m_showDMSSearchOptionsButton.Enabled                    = enableDatabaseFeatures;
                mtextbox_databaseFilterName.Enabled         = enableDatabaseFeatures;
                mbutton_databaseSelectDatasetIDFile.Enabled = enableDatabaseFeatures;

                mbutton_extractDatasets.Enabled = false;
                mbutton_selectFilesFromDisk.Enabled = enableButtons;
                mlabel_downloadingImage.Visible = isLoading;

                /// 
                /// Only enable if we arent loading and if we have checked items.
                /// 
                if (isLoading == false)
                {
                    if (mint_numberCheckedItems > 0)
                    {
                        NextButtonEnabled = true;
                    }
                    else
                    {
                        NextButtonEnabled = false;
                    }
                }
                else
                {
                    NextButtonEnabled = false;
                }

                BackButtonEnabled = enableButtons;

            }catch
            {
            }
        }
        /// <summary>
        /// Loads the data from the database.
        /// </summary>
        public void LoadDMSFilterData()
        {
            /// 
            /// Make sure the user supplied a regular expression filter
            /// 
            if (mtextbox_databaseFilterName.Text.Length == 0)
            {
                MessageBox.Show("You must enter a part of the dataset name first!", "Ooops!");
                return;
            }

            string wildcardFilter = mtextbox_databaseFilterName.Text + "%";

            /// 
            /// Set loading messages
            /// 
            mlabel_percentLoadingComplete.Text = "Querying database for dataset information.  Please Wait...";
            mprogressBar_datasetLoading.Style  = ProgressBarStyle.Marquee;
            SetLoadingState(true);
            
            m_searchOptions.DatasetName = wildcardFilter;
            mobj_dmsLoader.LoadDatasetsFromDatasetNames(m_searchOptions, true);
        }
        /// <summary>
        /// Given a filename of dataset id's, this method loads that file, reads the ID's and pulls the
        /// dataset information from the DMS database.
        /// </summary>
        private void LoadDatasetIDsFromDatabase()
        {
            ///
            /// Allow the user to check what files to load.
            /// 
            if (mobj_openJobIDDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo info = new System.IO.FileInfo(mobj_openJobIDDialog.FileName);
                Properties.Settings.Default.RawDataPath = info.Directory.FullName;
                Properties.Settings.Default.Save();

                /// 
                /// Set loading messages
                /// 
                mlabel_percentLoadingComplete.Text = "Reading datasets from file and querying database for dataset information.  Please Wait...";
                mprogressBar_datasetLoading.Style = ProgressBarStyle.Marquee;
                SetLoadingState(true);

                
                mobj_dataJobLoader.LoadDatasets(mobj_openJobIDDialog.FileName, true);
            }
        }
        /// <summary>
        /// Starts the loading process for finding files on disk.
        /// </summary>
        public void LoadDiskData()
        {
            ///
            /// Allow the user to check what files to load.
            /// 
            if (mobj_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo info   = new System.IO.FileInfo(mobj_openFileDialog.FileName);
                Properties.Settings.Default.RawDataPath = info.Directory.FullName;
                Properties.Settings.Default.Save();

                /// 
                /// Set loading messages
                /// 
                mlabel_percentLoadingComplete.Text  = "Locating datasets on disk.  Please Wait...";
                mprogressBar_datasetLoading.Style   = ProgressBarStyle.Marquee;
                SetLoadingState(true);
                mobj_diskLoader.LoadData(mobj_openFileDialog.FileNames, true);
            }
        }
        /// <summary>
        /// Adds the dataset to the list view.
        /// </summary>
        /// <param name="datasetInfo"></param>
        private void AddDatasetToListView(DatasetInformation datasetInfo)
        {
            mlistView_datasets.BeginUpdate();
            
            ListViewItem dataItem = new ListViewItem(datasetInfo.mstrDatasetId);
            dataItem.UseItemStyleForSubItems = false;
            dataItem.SubItems.Add(datasetInfo.mstrAnalysisJobId);
            dataItem.SubItems.Add(datasetInfo.DatasetName);            
            dataItem.SubItems.Add(datasetInfo.mstrAlias);

            if (datasetInfo.mintBatchID == 0)
                dataItem.SubItems.Add("NA");
            else
                dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintBatchID));

            if (datasetInfo.mintBlockID == 0)
                dataItem.SubItems.Add("NA");
            else
                dataItem.SubItems.Add(datasetInfo.mintBlockID.ToString());

            if (datasetInfo.mintRunOrder == 0)
                dataItem.SubItems.Add("NA");
            else
                dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintRunOrder));

            if (datasetInfo.mintColumnID == 0)
                dataItem.SubItems.Add("NA");
            else
                dataItem.SubItems.Add(Convert.ToString(datasetInfo.mintColumnID));


            dataItem.SubItems.Add(datasetInfo.mdateAcquisitionStart.ToShortDateString());
            dataItem.SubItems.Add(System.IO.Path.GetExtension(datasetInfo.mstrLocalPath));                
            dataItem.SubItems.Add(datasetInfo.menmDeisotopingTool.ToString());
            /// 
            /// the list control is stupid, so we have to make sure that 
            /// it doesnt eff up our count for a new item.
            /// 
            dataItem.Checked = datasetInfo.Selected;

            if (dataItem.Checked == true)
                dataItem.BackColor = Color.LightGray;
            else
                dataItem.BackColor = Color.White;

            mint_numberCheckedItems++;
            if (dataItem.Checked == true)
            {
                mint_numberCheckedItems++;
            }
            dataItem.SubItems.Add(datasetInfo.mstrInstrment);
            dataItem.SubItems.Add(System.IO.Path.GetFileNameWithoutExtension(datasetInfo.ParameterFileName));

            dataItem.Name = datasetInfo.DatasetName; 
            dataItem.Tag  = datasetInfo;

            if (mlistView_datasets.Items.ContainsKey(dataItem.Name) == true)
            {
                mlabel_similarNamesFound.Visible = true;
                dataItem.SubItems[2].BackColor   = Color.Salmon;                                
            }
            
            mlistView_datasets.Items.Add(dataItem);
            mlistView_datasets.UpdateItem(dataItem.Index);

            UpdateTotalChecked(mint_numberCheckedItems);
            mlistView_datasets.EndUpdate();
        }
        /// <summary>
        /// Updates the number of checked items there are.
        /// </summary>
        /// <param name="numberChecked"></param>
        private void UpdateTotalChecked(int numberChecked)
        {
            int totalItems = mlistView_datasets.Items.Count;

            if (totalItems == 0)
            {
                mlabel_totalChecked.Text = "No datasets loaded.";
                mlabel_totalChecked.ForeColor = Color.Red;
            }
            else
            {
                if (numberChecked <= 0)
                {
                    mlabel_totalChecked.ForeColor = Color.Red;
                }
                else
                {
                    mlabel_totalChecked.ForeColor = Color.Black;
                }
                mlabel_totalChecked.Text = string.Format("({0}/{1}) Datasets checked",
                                                          numberChecked, totalItems);
            }
            mint_numberCheckedItems = numberChecked;

            /// 
            /// Enable only if we are not loading, and we have items that are checked.
            /// 
            if (mbool_isLoading == false)
            {
                if (mint_numberCheckedItems > 0)
                {
                    NextButtonEnabled = true;
                }
                else
                {
                   NextButtonEnabled = false;
                }
            }
            else 
            {
                NextButtonEnabled = false;
            }
        }       
        #endregion

        #region ListView Dataset Selection
        /// <summary>
        /// Updates what items are checked in the internal array.
        /// </summary>
        private List<DatasetInformation> SelectCheckedItems()
        {
            List<DatasetInformation> list = new List<DatasetInformation>();
            foreach (ListViewItem item in mlistView_datasets.CheckedItems)
            {
                DatasetInformation info = item.Tag as DatasetInformation;
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
        private void UncheckAll()
        {
            int totalChecked = 0;

            mlistView_datasets.BeginUpdate();
            foreach (ListViewItem item in mlistView_datasets.Items)
            {
                item.Checked = false;
                item.BackColor = Color.White;
            }
            mlistView_datasets.EndUpdate();

            UpdateTotalChecked(totalChecked);
        }
        /// <summary>
        /// Selects all the datasets.
        /// </summary>
        private void CheckAll()
        {
            int totalChecked = mlistView_datasets.Items.Count;
            mlistView_datasets.BeginUpdate();
            foreach (ListViewItem item in mlistView_datasets.Items)
            {
                item.Checked = true;
                item.BackColor = Color.LightGray;                
            }
            mlistView_datasets.EndUpdate();
            UpdateTotalChecked(totalChecked);
        }
        /// <summary>
        /// Toggles the selection for all the datasets.
        /// </summary>
        private void ToggleAll()
        {
            int totalChecked = 0;
            mlistView_datasets.BeginUpdate();
            foreach (ListViewItem item in mlistView_datasets.Items)
            {
                item.Checked = (item.Checked == false);
                if (item.Checked == true)
                {
                    totalChecked++;
                }
            }
            mlistView_datasets.EndUpdate();
            UpdateTotalChecked(totalChecked);
        }
        /// <summary>
        /// Clears all the datasets from the list.
        /// </summary>
        private void ClearAll()
        {
            int totalChecked = 0;
            mlistView_datasets.Items.Clear();
            
            UpdateTotalChecked(totalChecked);
            FindSimilarNames();
        }
        /// <summary>
        /// Clears all the selected items.
        /// </summary>
        private void ClearChecked()
        {            
            mlistView_datasets.BeginUpdate();
            foreach (ListViewItem item in mlistView_datasets.CheckedItems)
            {
                mlistView_datasets.Items.Remove(item);
            }
            mlistView_datasets.EndUpdate();

            int totalChecked = 0;
            FindSimilarNames();
            UpdateTotalChecked(totalChecked);
        }
        /// <summary>
        /// Clears all the selected items.
        /// </summary>
        private void ClearUnchecked()
        {
            int totalChecked = mlistView_datasets.CheckedItems.Count;

            mlistView_datasets.BeginUpdate();
            foreach (ListViewItem item in mlistView_datasets.Items)
            {
                if (item.Checked == false)
                {
                    mlistView_datasets.Items.Remove(item);
                }
            }
            FindSimilarNames();
            mlistView_datasets.EndUpdate();
            UpdateTotalChecked(totalChecked);
        }
        private void sortByCheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortByChecked();
        }
        /// <summary>
        /// Discovers if there are similar names in the listview.
        /// </summary>
        private void FindSimilarNames()
        {
            List<string> names = new List<string>();
            mlabel_similarNamesFound.Visible = false;
            foreach (ListViewItem item in mlistView_datasets.Items)
            {
                string name = item.SubItems[2].Text;
                item.SubItems[2].BackColor = Color.White;
                if (names.Contains(name))
                {
                    mlabel_similarNamesFound.Visible = true;
                    item.SubItems[2].BackColor = Color.Salmon;                    
                }
                names.Add(name);
            }
        }
        #endregion

        #region Button and ListView Event Handlers 
        private void mbutton_selectDatasetIDFile_Click(object sender, EventArgs e)
        {
            LoadDatasetIDsFromDatabase();
        }       
        private void mbutton_datasetSearch_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserDataLoaderRegEx = mtextbox_databaseFilterName.Text;
            Properties.Settings.Default.Save();

            LoadDMSFilterData();
        }
        private void mbutton_selectFilesFromDisk_Click(object sender, EventArgs e)
        {
            LoadDiskData();
        }
        private void mbutton_toggleAll_Click(object sender, EventArgs e)
        {
            ToggleAll();
        }        
        private void mbutton_uncheckAll_Click(object sender, EventArgs e)
        {
            UncheckAll();
        }
        private void mbutton_checkAll_Click(object sender, EventArgs e)
        {
            CheckAll();
        }
        private void mbutton_clearChecked_Click(object sender, EventArgs e)
        {
            ClearChecked();
        }
        private void mbutton_clearUnchecked_Click(object sender, EventArgs e)
        {
            ClearUnchecked();
        }
        private void mbutton_clearAll_Click(object sender, EventArgs e)
        {
            ClearAll();
        }
        private void mlistView_datasets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == mobj_listViewItemComparer.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (mobj_listViewItemComparer.Order == SortOrder.Ascending)
                {
                    mobj_listViewItemComparer.Order = SortOrder.Descending;
                }
                else 
                {
                    mobj_listViewItemComparer.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                mobj_listViewItemComparer.SortColumn = e.Column;
                mobj_listViewItemComparer.Order      = SortOrder.Ascending;
            }

            mbool_sortChecked = false;
            // Perform the sort with these new sort options.
            mlistView_datasets.Sort();            
        }
        /// <summary>
        /// Sorts items based on if they are checked.
        /// </summary>
        private void SortByChecked()
        {            
            // Determine if clicked column is already the column that is being sorted.
            if (mbool_sortChecked == true)
            {
                // Reverse the current sort direction for this column.
                if (mobj_listViewItemComparer.Order == SortOrder.Ascending)
                {
                    mobj_listViewItemComparer.Order = SortOrder.Descending;
                }
                else 
                {
                    mobj_listViewItemComparer.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                mobj_listViewItemComparer.SortColumn = -1;
                mobj_listViewItemComparer.Order      = SortOrder.Ascending;
            }

            mbool_sortChecked = true;

            // Perform the sort with these new sort options.
            mlistView_datasets.Sort();
        }
        /// <summary>
        /// Displays the server information to the user so they can connect to a different database to get DMS information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>                       
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
                mobj_connectionTester.ServerInformation     = info;
                mobj_dmsLoader.ServerInformation            = info;
                mobj_serverInformation                      = info;
                ///
                /// Save these settings as the default
                /// 
                Properties.Settings.Default.DMSDatabaseName = info.DatabaseName;
                Properties.Settings.Default.DMSServerName   = info.ServerName;
                Properties.Settings.Default.Save();
            }
        }      
        private void mbutton_stopLoading_Click(object sender, EventArgs e)
        {
            /// 
            /// This needs to be come an abstract class we can hold on to.
            /// 
            mobj_diskLoader.Abort();
            mobj_dmsLoader.Abort();
        }
        void mlistView_datasets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked == false)
            {
                mint_numberCheckedItems--;
                e.Item.BackColor = Color.White;
            }
            else
            {
                e.Item.BackColor = Color.LightGray;
                mint_numberCheckedItems++;
            }

            

            UpdateTotalChecked(mint_numberCheckedItems);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the server information needed to make a connection to the DMS database on a PRISM server inside of PNNL.
        /// </summary>
        public clsDMSServerInformation ServerInformation
        {
            get
            {
                return mobj_serverInformation;
            }
            set
            {
                mobj_serverInformation = value;
            }
        }
        /// <summary>
        /// Gets a list of available datasets.
        /// </summary>
        public List<DatasetInformation> Datasets
        {
            get
            {
                return SelectCheckedItems();
            }
        }
        #endregion

        #region Context Menu Event Handlers
        private void toggleAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleAll();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckAll();
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
        }

        private void clearUnSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearUnchecked();
        }

        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearChecked();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();
        }
        #endregion                       

        private void m_showAllOptions_Click(object sender, EventArgs e)
        {
            m_searchOptions.DatasetName                 = mtextbox_databaseFilterName.Text;

            DMSDatasetSearchOptionsForm searchOptions   = new DMSDatasetSearchOptionsForm(m_searchOptions);            
            searchOptions.StartPosition                 = FormStartPosition.CenterScreen;

            if (searchOptions.ShowDialog() == DialogResult.OK)
            {
                m_searchOptions                  = searchOptions.Options;
                mtextbox_databaseFilterName.Text = m_searchOptions.DatasetName;
            }
        }
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

            if (ColumnToSort >= 0)
            {
                // Case insensitive Compare
                compareResult = ObjectCompare.Compare(
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
            else
            {
                if (OrderOfSort == SortOrder.Ascending)
                {
                   return (listviewX.Checked.CompareTo(listviewY.Checked));
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                   return (listviewY.Checked.CompareTo(listviewX.Checked));
                }else
                {
                    return 0;
                }
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
