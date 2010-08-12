using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using PNNLControls.Forms;

namespace MultiAlignWin
{
	partial class ctlLoadDatasetWizardPage
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			/// 
			/// Kill the network connection tester
			/// 
			if (mobj_connectionTester != null)
			{
				mobj_connectionTester.Dispose();
			}
			/// 
			/// Kill the disk dataset loader
			/// 
			if (mobj_diskLoader != null)
			{
				mobj_diskLoader.Dispose();
			}
			/// 
			/// Kill the dms dataset loader 
			/// 
			if (mobj_dmsLoader != null)
			{
				mobj_dmsLoader.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlLoadDatasetWizardPage));
            this.btnEnableDatabase = new System.Windows.Forms.Button();
            this.mcontextMenu_dataset = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sortByCheckedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearUnSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mpanel_loading = new System.Windows.Forms.Panel();
            this.mprogressBar_datasetLoading = new System.Windows.Forms.ProgressBar();
            this.mlabel_percentLoadingComplete = new System.Windows.Forms.Label();
            this.mlabel_downloadingImage = new System.Windows.Forms.Label();
            this.mbutton_stopLoading = new System.Windows.Forms.Button();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.mlabel_totalChecked = new System.Windows.Forms.Label();
            this.mlistView_datasets = new PNNLControls.Forms.ctlListViewFlickerless();
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
            this.m_instrumentColumn = new System.Windows.Forms.ColumnHeader();
            this.mbutton_clearUnchecked = new System.Windows.Forms.Button();
            this.mbutton_clearChecked = new System.Windows.Forms.Button();
            this.mbutton_checkAll = new System.Windows.Forms.Button();
            this.mgroupBox_selectData = new System.Windows.Forms.GroupBox();
            this.m_showDMSSearchOptionsButton = new System.Windows.Forms.Button();
            this.mlabel_similarNamesFound = new System.Windows.Forms.Label();
            this.mlinkLabel_testConnection = new System.Windows.Forms.LinkLabel();
            this.mlink_showPNNLProperties = new System.Windows.Forms.LinkLabel();
            this.mlabel_networkConnectionTest = new System.Windows.Forms.Label();
            this.mprogressBar_connectionTest = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mbutton_selectFilesFromDisk = new System.Windows.Forms.Button();
            this.labelSelect = new System.Windows.Forms.Label();
            this.mbutton_databaseSelectDatasetIDFile = new System.Windows.Forms.Button();
            this.mtextbox_databaseFilterName = new System.Windows.Forms.TextBox();
            this.mbutton_databaseDatasetSearch = new System.Windows.Forms.Button();
            this.mlabel_datasetAnalysisIDNumbers = new System.Windows.Forms.Label();
            this.mbutton_extractDatasets = new System.Windows.Forms.Button();
            this.mline = new ExternalControls.NiceLine();
            this.mbutton_uncheckAll = new System.Windows.Forms.Button();
            this.mbutton_clearAll = new System.Windows.Forms.Button();
            this.mbutton_toggleAll = new System.Windows.Forms.Button();
            this.mcontextMenu_dataset.SuspendLayout();
            this.mpanel_loading.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.mgroupBox_selectData.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.BackColor = System.Drawing.Color.White;
            this.Banner.Size = new System.Drawing.Size(1050, 50);
            this.Banner.Subtitle = "Choose from different options of getting files.";
            this.Banner.Title = "Step 1. Select Data";
            // 
            // btnEnableDatabase
            // 
            this.btnEnableDatabase.Location = new System.Drawing.Point(0, 0);
            this.btnEnableDatabase.Name = "btnEnableDatabase";
            this.btnEnableDatabase.Size = new System.Drawing.Size(75, 23);
            this.btnEnableDatabase.TabIndex = 0;
            // 
            // mcontextMenu_dataset
            // 
            this.mcontextMenu_dataset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByCheckedToolStripMenuItem,
            this.toolStripSeparator3,
            this.toggleAllToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearSelectedToolStripMenuItem,
            this.clearUnSelectedToolStripMenuItem,
            this.toolStripSeparator2,
            this.clearAllToolStripMenuItem});
            this.mcontextMenu_dataset.Name = "mcontextMenuStrip";
            this.mcontextMenu_dataset.Size = new System.Drawing.Size(166, 176);
            // 
            // sortByCheckedToolStripMenuItem
            // 
            this.sortByCheckedToolStripMenuItem.Name = "sortByCheckedToolStripMenuItem";
            this.sortByCheckedToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.sortByCheckedToolStripMenuItem.Text = "Sort By Checked";
            this.sortByCheckedToolStripMenuItem.Click += new System.EventHandler(this.sortByCheckedToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(162, 6);
            // 
            // toggleAllToolStripMenuItem
            // 
            this.toggleAllToolStripMenuItem.Name = "toggleAllToolStripMenuItem";
            this.toggleAllToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.toggleAllToolStripMenuItem.Text = "Toggle All";
            this.toggleAllToolStripMenuItem.Click += new System.EventHandler(this.toggleAllToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.selectAllToolStripMenuItem.Text = "Check All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.deselectAllToolStripMenuItem.Text = "Uncheck All";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.deselectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // clearSelectedToolStripMenuItem
            // 
            this.clearSelectedToolStripMenuItem.Name = "clearSelectedToolStripMenuItem";
            this.clearSelectedToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.clearSelectedToolStripMenuItem.Text = "Clear Checked";
            this.clearSelectedToolStripMenuItem.Click += new System.EventHandler(this.clearSelectedToolStripMenuItem_Click);
            // 
            // clearUnSelectedToolStripMenuItem
            // 
            this.clearUnSelectedToolStripMenuItem.Name = "clearUnSelectedToolStripMenuItem";
            this.clearUnSelectedToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.clearUnSelectedToolStripMenuItem.Text = "Clear Unchecked";
            this.clearUnSelectedToolStripMenuItem.Click += new System.EventHandler(this.clearUnSelectedToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(162, 6);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.clearAllToolStripMenuItem.Text = "Clear All";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // mpanel_loading
            // 
            this.mpanel_loading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mpanel_loading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpanel_loading.Controls.Add(this.mprogressBar_datasetLoading);
            this.mpanel_loading.Controls.Add(this.mlabel_percentLoadingComplete);
            this.mpanel_loading.Controls.Add(this.mlabel_downloadingImage);
            this.mpanel_loading.Controls.Add(this.mbutton_stopLoading);
            this.mpanel_loading.Location = new System.Drawing.Point(6, 750);
            this.mpanel_loading.Name = "mpanel_loading";
            this.mpanel_loading.Padding = new System.Windows.Forms.Padding(3);
            this.mpanel_loading.Size = new System.Drawing.Size(1036, 33);
            this.mpanel_loading.TabIndex = 5;
            this.mpanel_loading.Visible = false;
            // 
            // mprogressBar_datasetLoading
            // 
            this.mprogressBar_datasetLoading.BackColor = System.Drawing.Color.White;
            this.mprogressBar_datasetLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mprogressBar_datasetLoading.ForeColor = System.Drawing.Color.Lime;
            this.mprogressBar_datasetLoading.Location = new System.Drawing.Point(3, 3);
            this.mprogressBar_datasetLoading.Name = "mprogressBar_datasetLoading";
            this.mprogressBar_datasetLoading.Size = new System.Drawing.Size(862, 25);
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
            this.mlabel_percentLoadingComplete.Location = new System.Drawing.Point(865, 3);
            this.mlabel_percentLoadingComplete.MinimumSize = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.Name = "mlabel_percentLoadingComplete";
            this.mlabel_percentLoadingComplete.Size = new System.Drawing.Size(39, 22);
            this.mlabel_percentLoadingComplete.TabIndex = 10;
            this.mlabel_percentLoadingComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_downloadingImage
            // 
            this.mlabel_downloadingImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.mlabel_downloadingImage.Image = global::MultiAlignWin.Properties.Resources.DOWNLOAD_00;
            this.mlabel_downloadingImage.Location = new System.Drawing.Point(904, 3);
            this.mlabel_downloadingImage.Name = "mlabel_downloadingImage";
            this.mlabel_downloadingImage.Size = new System.Drawing.Size(31, 25);
            this.mlabel_downloadingImage.TabIndex = 61;
            this.mlabel_downloadingImage.Visible = false;
            // 
            // mbutton_stopLoading
            // 
            this.mbutton_stopLoading.Dock = System.Windows.Forms.DockStyle.Right;
            this.mbutton_stopLoading.Image = global::MultiAlignWin.Properties.Resources.Critical;
            this.mbutton_stopLoading.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbutton_stopLoading.Location = new System.Drawing.Point(935, 3);
            this.mbutton_stopLoading.Name = "mbutton_stopLoading";
            this.mbutton_stopLoading.Size = new System.Drawing.Size(96, 25);
            this.mbutton_stopLoading.TabIndex = 60;
            this.mbutton_stopLoading.Text = "Stop Loading";
            this.mbutton_stopLoading.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mbutton_stopLoading.UseVisualStyleBackColor = true;
            this.mbutton_stopLoading.Click += new System.EventHandler(this.mbutton_stopLoading_Click);
            // 
            // panelOptions
            // 
            this.panelOptions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelOptions.Controls.Add(this.mlabel_totalChecked);
            this.panelOptions.Controls.Add(this.mlistView_datasets);
            this.panelOptions.Controls.Add(this.mbutton_clearUnchecked);
            this.panelOptions.Controls.Add(this.mpanel_loading);
            this.panelOptions.Controls.Add(this.mbutton_clearChecked);
            this.panelOptions.Controls.Add(this.mbutton_checkAll);
            this.panelOptions.Controls.Add(this.mgroupBox_selectData);
            this.panelOptions.Controls.Add(this.mbutton_uncheckAll);
            this.panelOptions.Controls.Add(this.mbutton_clearAll);
            this.panelOptions.Controls.Add(this.mbutton_toggleAll);
            this.panelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOptions.Location = new System.Drawing.Point(134, 0);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(1050, 786);
            this.panelOptions.TabIndex = 2;
            // 
            // mlabel_totalChecked
            // 
            this.mlabel_totalChecked.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_totalChecked.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mlabel_totalChecked.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mlabel_totalChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_totalChecked.Location = new System.Drawing.Point(299, 286);
            this.mlabel_totalChecked.Name = "mlabel_totalChecked";
            this.mlabel_totalChecked.Size = new System.Drawing.Size(476, 24);
            this.mlabel_totalChecked.TabIndex = 59;
            this.mlabel_totalChecked.Text = "(0/0) Datasets Checked";
            this.mlabel_totalChecked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mlistView_datasets
            // 
            this.mlistView_datasets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistView_datasets.CheckBoxes = true;
            this.mlistView_datasets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
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
            this.columnHeader_tool,
            this.m_instrumentColumn});
            this.mlistView_datasets.FullRowSelect = true;
            this.mlistView_datasets.GridLines = true;
            this.mlistView_datasets.Location = new System.Drawing.Point(6, 316);
            this.mlistView_datasets.Name = "mlistView_datasets";
            this.mlistView_datasets.Size = new System.Drawing.Size(1036, 428);
            this.mlistView_datasets.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.mlistView_datasets.TabIndex = 58;
            this.mlistView_datasets.UseCompatibleStateImageBehavior = false;
            this.mlistView_datasets.View = System.Windows.Forms.View.Details;
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
            this.mcolumnHeader_Date.Text = "Date Acquisition Start";
            this.mcolumnHeader_Date.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mcolumnHeader_Date.Width = 122;
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
            this.columnHeader_tool.Width = 74;
            // 
            // m_instrumentColumn
            // 
            this.m_instrumentColumn.Text = "Instrument";
            this.m_instrumentColumn.Width = 70;
            // 
            // mbutton_clearUnchecked
            // 
            this.mbutton_clearUnchecked.BackColor = System.Drawing.Color.Red;
            this.mbutton_clearUnchecked.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mbutton_clearUnchecked.BackgroundImage")));
            this.mbutton_clearUnchecked.Enabled = false;
            this.mbutton_clearUnchecked.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_clearUnchecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clearUnchecked.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mbutton_clearUnchecked.Location = new System.Drawing.Point(112, 286);
            this.mbutton_clearUnchecked.Name = "mbutton_clearUnchecked";
            this.mbutton_clearUnchecked.Size = new System.Drawing.Size(110, 24);
            this.mbutton_clearUnchecked.TabIndex = 56;
            this.mbutton_clearUnchecked.Text = "Remove Unchecked";
            this.mbutton_clearUnchecked.UseVisualStyleBackColor = false;
            this.mbutton_clearUnchecked.Click += new System.EventHandler(this.mbutton_clearUnchecked_Click);
            // 
            // mbutton_clearChecked
            // 
            this.mbutton_clearChecked.BackColor = System.Drawing.Color.Red;
            this.mbutton_clearChecked.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mbutton_clearChecked.BackgroundImage")));
            this.mbutton_clearChecked.Enabled = false;
            this.mbutton_clearChecked.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_clearChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clearChecked.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mbutton_clearChecked.Location = new System.Drawing.Point(6, 286);
            this.mbutton_clearChecked.Name = "mbutton_clearChecked";
            this.mbutton_clearChecked.Size = new System.Drawing.Size(100, 24);
            this.mbutton_clearChecked.TabIndex = 55;
            this.mbutton_clearChecked.Text = "Remove Checked";
            this.mbutton_clearChecked.UseVisualStyleBackColor = false;
            this.mbutton_clearChecked.Click += new System.EventHandler(this.mbutton_clearChecked_Click);
            // 
            // mbutton_checkAll
            // 
            this.mbutton_checkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_checkAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_checkAll.Enabled = false;
            this.mbutton_checkAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_checkAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_checkAll.Location = new System.Drawing.Point(872, 286);
            this.mbutton_checkAll.Name = "mbutton_checkAll";
            this.mbutton_checkAll.Size = new System.Drawing.Size(83, 24);
            this.mbutton_checkAll.TabIndex = 53;
            this.mbutton_checkAll.Text = "Check All";
            this.mbutton_checkAll.UseVisualStyleBackColor = false;
            this.mbutton_checkAll.Click += new System.EventHandler(this.mbutton_checkAll_Click);
            // 
            // mgroupBox_selectData
            // 
            this.mgroupBox_selectData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_selectData.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mgroupBox_selectData.Controls.Add(this.m_showDMSSearchOptionsButton);
            this.mgroupBox_selectData.Controls.Add(this.mlabel_similarNamesFound);
            this.mgroupBox_selectData.Controls.Add(this.mlinkLabel_testConnection);
            this.mgroupBox_selectData.Controls.Add(this.mlink_showPNNLProperties);
            this.mgroupBox_selectData.Controls.Add(this.mlabel_networkConnectionTest);
            this.mgroupBox_selectData.Controls.Add(this.mprogressBar_connectionTest);
            this.mgroupBox_selectData.Controls.Add(this.label3);
            this.mgroupBox_selectData.Controls.Add(this.label2);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_selectFilesFromDisk);
            this.mgroupBox_selectData.Controls.Add(this.labelSelect);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_databaseSelectDatasetIDFile);
            this.mgroupBox_selectData.Controls.Add(this.mtextbox_databaseFilterName);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_databaseDatasetSearch);
            this.mgroupBox_selectData.Controls.Add(this.mlabel_datasetAnalysisIDNumbers);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_extractDatasets);
            this.mgroupBox_selectData.Controls.Add(this.mline);
            this.mgroupBox_selectData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_selectData.Location = new System.Drawing.Point(6, 56);
            this.mgroupBox_selectData.Name = "mgroupBox_selectData";
            this.mgroupBox_selectData.Size = new System.Drawing.Size(1036, 204);
            this.mgroupBox_selectData.TabIndex = 48;
            this.mgroupBox_selectData.TabStop = false;
            this.mgroupBox_selectData.Text = "Select Data ";
            // 
            // m_showDMSSearchOptionsButton
            // 
            this.m_showDMSSearchOptionsButton.BackColor = System.Drawing.SystemColors.Control;
            this.m_showDMSSearchOptionsButton.Enabled = false;
            this.m_showDMSSearchOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_showDMSSearchOptionsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_showDMSSearchOptionsButton.Location = new System.Drawing.Point(575, 124);
            this.m_showDMSSearchOptionsButton.Name = "m_showDMSSearchOptionsButton";
            this.m_showDMSSearchOptionsButton.Size = new System.Drawing.Size(99, 37);
            this.m_showDMSSearchOptionsButton.TabIndex = 57;
            this.m_showDMSSearchOptionsButton.Text = "Search Options";
            this.m_showDMSSearchOptionsButton.UseVisualStyleBackColor = false;
            this.m_showDMSSearchOptionsButton.Click += new System.EventHandler(this.m_showAllOptions_Click);
            // 
            // mlabel_similarNamesFound
            // 
            this.mlabel_similarNamesFound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mlabel_similarNamesFound.AutoSize = true;
            this.mlabel_similarNamesFound.ForeColor = System.Drawing.Color.Red;
            this.mlabel_similarNamesFound.Location = new System.Drawing.Point(767, 185);
            this.mlabel_similarNamesFound.Name = "mlabel_similarNamesFound";
            this.mlabel_similarNamesFound.Size = new System.Drawing.Size(265, 16);
            this.mlabel_similarNamesFound.TabIndex = 56;
            this.mlabel_similarNamesFound.Text = "Potential Duplicate Data Files Found!";
            this.mlabel_similarNamesFound.Visible = false;
            // 
            // mlinkLabel_testConnection
            // 
            this.mlinkLabel_testConnection.AutoSize = true;
            this.mlinkLabel_testConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlinkLabel_testConnection.Location = new System.Drawing.Point(253, 96);
            this.mlinkLabel_testConnection.Name = "mlinkLabel_testConnection";
            this.mlinkLabel_testConnection.Size = new System.Drawing.Size(103, 13);
            this.mlinkLabel_testConnection.TabIndex = 54;
            this.mlinkLabel_testConnection.TabStop = true;
            this.mlinkLabel_testConnection.Text = "(Re-test connection)";
            this.mlinkLabel_testConnection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mlinkLabel_testConnection_LinkClicked);
            // 
            // mlink_showPNNLProperties
            // 
            this.mlink_showPNNLProperties.AutoSize = true;
            this.mlink_showPNNLProperties.Location = new System.Drawing.Point(46, 94);
            this.mlink_showPNNLProperties.Name = "mlink_showPNNLProperties";
            this.mlink_showPNNLProperties.Size = new System.Drawing.Size(189, 16);
            this.mlink_showPNNLProperties.TabIndex = 53;
            this.mlink_showPNNLProperties.TabStop = true;
            this.mlink_showPNNLProperties.Text = "Inside PNNL Network Only";
            this.mlink_showPNNLProperties.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mlink_showPNNLProperties_LinkClicked);
            // 
            // mlabel_networkConnectionTest
            // 
            this.mlabel_networkConnectionTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mlabel_networkConnectionTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_networkConnectionTest.Location = new System.Drawing.Point(362, 96);
            this.mlabel_networkConnectionTest.Name = "mlabel_networkConnectionTest";
            this.mlabel_networkConnectionTest.Size = new System.Drawing.Size(207, 18);
            this.mlabel_networkConnectionTest.TabIndex = 38;
            this.mlabel_networkConnectionTest.Text = "Testing Connection To DMS...";
            this.mlabel_networkConnectionTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mprogressBar_connectionTest
            // 
            this.mprogressBar_connectionTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mprogressBar_connectionTest.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.mprogressBar_connectionTest.ForeColor = System.Drawing.Color.Lime;
            this.mprogressBar_connectionTest.Location = new System.Drawing.Point(575, 96);
            this.mprogressBar_connectionTest.Name = "mprogressBar_connectionTest";
            this.mprogressBar_connectionTest.Size = new System.Drawing.Size(453, 18);
            this.mprogressBar_connectionTest.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.mprogressBar_connectionTest.TabIndex = 39;
            this.mprogressBar_connectionTest.Value = 2;
            this.mprogressBar_connectionTest.Visible = false;
            // 
            // label3
            // 
            this.label3.Enabled = false;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(46, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(225, 22);
            this.label3.TabIndex = 49;
            this.label3.Text = "Extract datasets from MultiAlign Analysis File:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(46, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 22);
            this.label2.TabIndex = 48;
            this.label2.Text = "Select files from disk:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbutton_selectFilesFromDisk
            // 
            this.mbutton_selectFilesFromDisk.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_selectFilesFromDisk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_selectFilesFromDisk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_selectFilesFromDisk.Location = new System.Drawing.Point(283, 24);
            this.mbutton_selectFilesFromDisk.Name = "mbutton_selectFilesFromDisk";
            this.mbutton_selectFilesFromDisk.Size = new System.Drawing.Size(32, 24);
            this.mbutton_selectFilesFromDisk.TabIndex = 7;
            this.mbutton_selectFilesFromDisk.Text = "...";
            this.mbutton_selectFilesFromDisk.UseVisualStyleBackColor = false;
            this.mbutton_selectFilesFromDisk.Click += new System.EventHandler(this.mbutton_selectFilesFromDisk_Click);
            // 
            // labelSelect
            // 
            this.labelSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelect.Location = new System.Drawing.Point(46, 130);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(225, 40);
            this.labelSelect.TabIndex = 0;
            this.labelSelect.Text = "Specify part of the name of the dataset:\r\nYou can use regex (e.g. * for several)\r" +
                "\n";
            this.labelSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbutton_databaseSelectDatasetIDFile
            // 
            this.mbutton_databaseSelectDatasetIDFile.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_databaseSelectDatasetIDFile.Enabled = false;
            this.mbutton_databaseSelectDatasetIDFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_databaseSelectDatasetIDFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_databaseSelectDatasetIDFile.Location = new System.Drawing.Point(283, 170);
            this.mbutton_databaseSelectDatasetIDFile.Name = "mbutton_databaseSelectDatasetIDFile";
            this.mbutton_databaseSelectDatasetIDFile.Size = new System.Drawing.Size(32, 24);
            this.mbutton_databaseSelectDatasetIDFile.TabIndex = 47;
            this.mbutton_databaseSelectDatasetIDFile.Text = "...";
            this.mbutton_databaseSelectDatasetIDFile.UseVisualStyleBackColor = false;
            this.mbutton_databaseSelectDatasetIDFile.Click += new System.EventHandler(this.mbutton_selectDatasetIDFile_Click);
            // 
            // mtextbox_databaseFilterName
            // 
            this.mtextbox_databaseFilterName.AcceptsReturn = true;
            this.mtextbox_databaseFilterName.Enabled = false;
            this.mtextbox_databaseFilterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtextbox_databaseFilterName.Location = new System.Drawing.Point(283, 133);
            this.mtextbox_databaseFilterName.Name = "mtextbox_databaseFilterName";
            this.mtextbox_databaseFilterName.Size = new System.Drawing.Size(208, 21);
            this.mtextbox_databaseFilterName.TabIndex = 2;
            this.mtextbox_databaseFilterName.Text = "Caulo_049_[2-4]";
            // 
            // mbutton_databaseDatasetSearch
            // 
            this.mbutton_databaseDatasetSearch.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_databaseDatasetSearch.Enabled = false;
            this.mbutton_databaseDatasetSearch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_databaseDatasetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_databaseDatasetSearch.Location = new System.Drawing.Point(497, 124);
            this.mbutton_databaseDatasetSearch.Name = "mbutton_databaseDatasetSearch";
            this.mbutton_databaseDatasetSearch.Size = new System.Drawing.Size(72, 37);
            this.mbutton_databaseDatasetSearch.TabIndex = 1;
            this.mbutton_databaseDatasetSearch.Text = "GO!";
            this.mbutton_databaseDatasetSearch.UseVisualStyleBackColor = false;
            this.mbutton_databaseDatasetSearch.Click += new System.EventHandler(this.mbutton_datasetSearch_Click);
            // 
            // mlabel_datasetAnalysisIDNumbers
            // 
            this.mlabel_datasetAnalysisIDNumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_datasetAnalysisIDNumbers.Location = new System.Drawing.Point(46, 170);
            this.mlabel_datasetAnalysisIDNumbers.Name = "mlabel_datasetAnalysisIDNumbers";
            this.mlabel_datasetAnalysisIDNumbers.Size = new System.Drawing.Size(190, 24);
            this.mlabel_datasetAnalysisIDNumbers.TabIndex = 46;
            this.mlabel_datasetAnalysisIDNumbers.Text = "Select File with dataset ID numbers:";
            this.mlabel_datasetAnalysisIDNumbers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbutton_extractDatasets
            // 
            this.mbutton_extractDatasets.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_extractDatasets.Enabled = false;
            this.mbutton_extractDatasets.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_extractDatasets.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_extractDatasets.Location = new System.Drawing.Point(283, 55);
            this.mbutton_extractDatasets.Name = "mbutton_extractDatasets";
            this.mbutton_extractDatasets.Size = new System.Drawing.Size(32, 24);
            this.mbutton_extractDatasets.TabIndex = 8;
            this.mbutton_extractDatasets.Text = "...";
            this.mbutton_extractDatasets.UseVisualStyleBackColor = false;
            // 
            // mline
            // 
            this.mline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mline.Location = new System.Drawing.Point(49, 110);
            this.mline.Name = "mline";
            this.mline.Size = new System.Drawing.Size(977, 17);
            this.mline.TabIndex = 55;
            // 
            // mbutton_uncheckAll
            // 
            this.mbutton_uncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_uncheckAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_uncheckAll.Enabled = false;
            this.mbutton_uncheckAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_uncheckAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_uncheckAll.Location = new System.Drawing.Point(783, 286);
            this.mbutton_uncheckAll.Name = "mbutton_uncheckAll";
            this.mbutton_uncheckAll.Size = new System.Drawing.Size(83, 24);
            this.mbutton_uncheckAll.TabIndex = 52;
            this.mbutton_uncheckAll.Text = "Uncheck All";
            this.mbutton_uncheckAll.UseVisualStyleBackColor = false;
            this.mbutton_uncheckAll.Click += new System.EventHandler(this.mbutton_uncheckAll_Click);
            // 
            // mbutton_clearAll
            // 
            this.mbutton_clearAll.BackColor = System.Drawing.Color.Red;
            this.mbutton_clearAll.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mbutton_clearAll.BackgroundImage")));
            this.mbutton_clearAll.Enabled = false;
            this.mbutton_clearAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_clearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clearAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mbutton_clearAll.Location = new System.Drawing.Point(229, 286);
            this.mbutton_clearAll.Name = "mbutton_clearAll";
            this.mbutton_clearAll.Size = new System.Drawing.Size(64, 24);
            this.mbutton_clearAll.TabIndex = 51;
            this.mbutton_clearAll.Text = "Remove All";
            this.mbutton_clearAll.UseVisualStyleBackColor = false;
            this.mbutton_clearAll.Click += new System.EventHandler(this.mbutton_clearAll_Click);
            // 
            // mbutton_toggleAll
            // 
            this.mbutton_toggleAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_toggleAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_toggleAll.Enabled = false;
            this.mbutton_toggleAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_toggleAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_toggleAll.Location = new System.Drawing.Point(959, 286);
            this.mbutton_toggleAll.Name = "mbutton_toggleAll";
            this.mbutton_toggleAll.Size = new System.Drawing.Size(83, 24);
            this.mbutton_toggleAll.TabIndex = 50;
            this.mbutton_toggleAll.Text = "Toggle All";
            this.mbutton_toggleAll.UseVisualStyleBackColor = false;
            this.mbutton_toggleAll.Click += new System.EventHandler(this.mbutton_toggleAll_Click);
            // 
            // ctlLoadDatasetWizardPage
            // 
            this.Controls.Add(this.panelOptions);
            this.Name = "ctlLoadDatasetWizardPage";
            this.Size = new System.Drawing.Size(1184, 786);
            this.Controls.SetChildIndex(this.panelOptions, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.mcontextMenu_dataset.ResumeLayout(false);
            this.mpanel_loading.ResumeLayout(false);
            this.mpanel_loading.PerformLayout();
            this.panelOptions.ResumeLayout(false);
            this.mgroupBox_selectData.ResumeLayout(false);
            this.mgroupBox_selectData.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private Button btnEnableDatabase;
		private ContextMenuStrip mcontextMenu_dataset;
		private ToolStripMenuItem toggleAllToolStripMenuItem;
		private ToolStripMenuItem selectAllToolStripMenuItem;
		private ToolStripMenuItem deselectAllToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem clearUnSelectedToolStripMenuItem;
		private ToolStripMenuItem clearSelectedToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem clearAllToolStripMenuItem;
		private Panel mpanel_loading;
		private ProgressBar mprogressBar_datasetLoading;
		private Label mlabel_percentLoadingComplete;
		private Panel panelOptions;
		private Button mbutton_clearUnchecked;
		private Button mbutton_clearChecked;
		private Button mbutton_checkAll;
		private GroupBox mgroupBox_selectData;
		private Label mlabel_networkConnectionTest;
		private ProgressBar mprogressBar_connectionTest;
		private Label label3;
		private Label label2;
		private Button mbutton_selectFilesFromDisk;
		private Label labelSelect;
		private Button mbutton_databaseSelectDatasetIDFile;
		private TextBox mtextbox_databaseFilterName;
		private Button mbutton_databaseDatasetSearch;
		private Label mlabel_datasetAnalysisIDNumbers;
		private Button mbutton_extractDatasets;
		private Button mbutton_uncheckAll;
		private Button mbutton_clearAll;
		private Button mbutton_toggleAll;


		private ColumnHeader mcolumnHeader_datasetID;
		private ColumnHeader mcolumnHeader_jobnum;
		private ColumnHeader mcolumnHeader_fileName;
		private ColumnHeader mcolumnHeader_alias;
		private ColumnHeader mcolumnHeader_block;
		private ColumnHeader mcolumnHeader_runOrder;
		private ColumnHeader lcColumnID;
		private ColumnHeader batchID;
		private ColumnHeader mcolumnHeader_Date;
		private ColumnHeader mcolumnHeader_FileType;
		private ColumnHeader columnHeader_tool;
		private Label mlabel_totalChecked;
		private LinkLabel mlink_showPNNLProperties;
		private Button mbutton_stopLoading;
		private ctlListViewFlickerless mlistView_datasets;
		private Label mlabel_downloadingImage;
		private ToolStripMenuItem sortByCheckedToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private LinkLabel mlinkLabel_testConnection;
		private ExternalControls.NiceLine mline;
		private Label mlabel_similarNamesFound;
        private ColumnHeader m_instrumentColumn;
        private Button m_showDMSSearchOptionsButton;
	}
}
