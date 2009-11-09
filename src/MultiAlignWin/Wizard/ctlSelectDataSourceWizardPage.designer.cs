using System.Windows.Forms;
using System.ComponentModel;

namespace MultiAlignWin
{
    partial class ctlSelectDataSourceWizardPage
    {

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

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.mpanelOptions = new System.Windows.Forms.Panel();
            this.labelInsidePNNLOnly = new System.Windows.Forms.Label();
            this.mbtnLoadPara = new System.Windows.Forms.Button();
            this.labelLoadParam = new System.Windows.Forms.Label();
            this.btnEnableDatabase = new System.Windows.Forms.Button();
            this.panelStep = new System.Windows.Forms.Panel();
            this.mbutton_datasetSearch = new System.Windows.Forms.Button();
            this.textfilterBox = new System.Windows.Forms.TextBox();
            this.labelSelect = new System.Windows.Forms.Label();
            this.mbutton_selectFilesFromDisk = new System.Windows.Forms.Button();
            this.mbutton_extractDatasets = new System.Windows.Forms.Button();
            this.mbutton_selectDatasetIDFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mlistView_data = new System.Windows.Forms.ListView();
            this.datasetID = new System.Windows.Forms.ColumnHeader();
            this.jobnum = new System.Windows.Forms.ColumnHeader();
            this.fileName = new System.Windows.Forms.ColumnHeader();
            this.alias = new System.Windows.Forms.ColumnHeader();
            this.block = new System.Windows.Forms.ColumnHeader();
            this.runOrder = new System.Windows.Forms.ColumnHeader();
            this.mgroupBox_selectData = new System.Windows.Forms.GroupBox();
            this.lblConnection = new System.Windows.Forms.Label();
            this.progressBarConnection = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mbutton_selectAll = new System.Windows.Forms.Button();
            this.mbutton_uncheckAll = new System.Windows.Forms.Button();
            this.mbutton_clearAll = new System.Windows.Forms.Button();
            this.mbutton_toggleAll = new System.Windows.Forms.Button();
            this.mpanel_line = new System.Windows.Forms.Panel();
            this.mcontextMenu_dataset = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearUnSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mbutton_clearChecked = new System.Windows.Forms.Button();
            this.mbutton_clearUnchecked = new System.Windows.Forms.Button();
            this.panelOptions.SuspendLayout();
            this.mpanelOptions.SuspendLayout();
            this.panelStep.SuspendLayout();
            this.mgroupBox_selectData.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mcontextMenu_dataset.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(764, 64);
            this.Banner.Subtitle = "Choose from different options of getting files";
            this.Banner.Title = "Step 1. Select Data";
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.mpanelOptions);
            this.panelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOptions.Location = new System.Drawing.Point(134, 64);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(764, 752);
            this.panelOptions.TabIndex = 2;
            // 
            // mpanelOptions
            // 
            this.mpanelOptions.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.mpanelOptions.Controls.Add(this.mlistView_data);
            this.mpanelOptions.Controls.Add(this.panelStep);
            this.mpanelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanelOptions.Location = new System.Drawing.Point(0, 0);
            this.mpanelOptions.Name = "mpanelOptions";
            this.mpanelOptions.Padding = new System.Windows.Forms.Padding(5);
            this.mpanelOptions.Size = new System.Drawing.Size(764, 752);
            this.mpanelOptions.TabIndex = 0;
            // 
            // labelInsidePNNLOnly
            // 
            this.labelInsidePNNLOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInsidePNNLOnly.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.labelInsidePNNLOnly.Location = new System.Drawing.Point(46, 85);
            this.labelInsidePNNLOnly.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelInsidePNNLOnly.Name = "labelInsidePNNLOnly";
            this.labelInsidePNNLOnly.Size = new System.Drawing.Size(190, 22);
            this.labelInsidePNNLOnly.TabIndex = 41;
            this.labelInsidePNNLOnly.Text = "Inside PNNL Network Only";
            this.labelInsidePNNLOnly.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // mbtnLoadPara
            // 
            this.mbtnLoadPara.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnLoadPara.Enabled = false;
            this.mbtnLoadPara.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbtnLoadPara.Location = new System.Drawing.Point(281, 24);
            this.mbtnLoadPara.Name = "mbtnLoadPara";
            this.mbtnLoadPara.Size = new System.Drawing.Size(32, 23);
            this.mbtnLoadPara.TabIndex = 9;
            this.mbtnLoadPara.Text = "...";
            this.mbtnLoadPara.UseVisualStyleBackColor = false;
            this.mbtnLoadPara.Click += new System.EventHandler(this.mbtnLoadParam_Click);
            // 
            // labelLoadParam
            // 
            this.labelLoadParam.Enabled = false;
            this.labelLoadParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLoadParam.Location = new System.Drawing.Point(44, 27);
            this.labelLoadParam.Name = "labelLoadParam";
            this.labelLoadParam.Size = new System.Drawing.Size(152, 23);
            this.labelLoadParam.TabIndex = 8;
            this.labelLoadParam.Text = "Load Parameters from a File:";
            // 
            // btnEnableDatabase
            // 
            this.btnEnableDatabase.Location = new System.Drawing.Point(0, 0);
            this.btnEnableDatabase.Name = "btnEnableDatabase";
            this.btnEnableDatabase.Size = new System.Drawing.Size(75, 23);
            this.btnEnableDatabase.TabIndex = 0;
            // 
            // panelStep
            // 
            this.panelStep.Controls.Add(this.mbutton_clearUnchecked);
            this.panelStep.Controls.Add(this.mbutton_clearChecked);
            this.panelStep.Controls.Add(this.mbutton_selectAll);
            this.panelStep.Controls.Add(this.mbutton_uncheckAll);
            this.panelStep.Controls.Add(this.mbutton_clearAll);
            this.panelStep.Controls.Add(this.mbutton_toggleAll);
            this.panelStep.Controls.Add(this.groupBox1);
            this.panelStep.Controls.Add(this.mgroupBox_selectData);
            this.panelStep.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStep.Location = new System.Drawing.Point(5, 5);
            this.panelStep.Name = "panelStep";
            this.panelStep.Size = new System.Drawing.Size(754, 287);
            this.panelStep.TabIndex = 48;
            // 
            // mbutton_datasetSearch
            // 
            this.mbutton_datasetSearch.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_datasetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_datasetSearch.Location = new System.Drawing.Point(437, 126);
            this.mbutton_datasetSearch.Name = "mbutton_datasetSearch";
            this.mbutton_datasetSearch.Size = new System.Drawing.Size(54, 24);
            this.mbutton_datasetSearch.TabIndex = 1;
            this.mbutton_datasetSearch.Text = "Search";
            this.mbutton_datasetSearch.UseVisualStyleBackColor = false;
            // 
            // textfilterBox
            // 
            this.textfilterBox.AcceptsReturn = true;
            this.textfilterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textfilterBox.Location = new System.Drawing.Point(283, 127);
            this.textfilterBox.Name = "textfilterBox";
            this.textfilterBox.Size = new System.Drawing.Size(148, 21);
            this.textfilterBox.TabIndex = 2;
            this.textfilterBox.Text = "Caulo_049_[2-4]";
            // 
            // labelSelect
            // 
            this.labelSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelect.Location = new System.Drawing.Point(46, 116);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(207, 40);
            this.labelSelect.TabIndex = 0;
            this.labelSelect.Text = "Specify part of the name of the dataset:";
            this.labelSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mbutton_selectFilesFromDisk
            // 
            this.mbutton_selectFilesFromDisk.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_selectFilesFromDisk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_selectFilesFromDisk.Location = new System.Drawing.Point(283, 24);
            this.mbutton_selectFilesFromDisk.Name = "mbutton_selectFilesFromDisk";
            this.mbutton_selectFilesFromDisk.Size = new System.Drawing.Size(32, 24);
            this.mbutton_selectFilesFromDisk.TabIndex = 7;
            this.mbutton_selectFilesFromDisk.Text = "...";
            this.mbutton_selectFilesFromDisk.UseVisualStyleBackColor = false;
            // 
            // mbutton_extractDatasets
            // 
            this.mbutton_extractDatasets.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_extractDatasets.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_extractDatasets.Location = new System.Drawing.Point(283, 55);
            this.mbutton_extractDatasets.Name = "mbutton_extractDatasets";
            this.mbutton_extractDatasets.Size = new System.Drawing.Size(32, 24);
            this.mbutton_extractDatasets.TabIndex = 8;
            this.mbutton_extractDatasets.Text = "...";
            this.mbutton_extractDatasets.UseVisualStyleBackColor = false;
            // 
            // mbutton_selectDatasetIDFile
            // 
            this.mbutton_selectDatasetIDFile.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_selectDatasetIDFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_selectDatasetIDFile.Location = new System.Drawing.Point(283, 156);
            this.mbutton_selectDatasetIDFile.Name = "mbutton_selectDatasetIDFile";
            this.mbutton_selectDatasetIDFile.Size = new System.Drawing.Size(32, 24);
            this.mbutton_selectDatasetIDFile.TabIndex = 47;
            this.mbutton_selectDatasetIDFile.Text = "...";
            this.mbutton_selectDatasetIDFile.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(46, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 24);
            this.label1.TabIndex = 46;
            this.label1.Text = "Select File with Analysis Job Numbers:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mlistView_data
            // 
            this.mlistView_data.AllowDrop = true;
            this.mlistView_data.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.datasetID,
            this.jobnum,
            this.fileName,
            this.alias,
            this.block,
            this.runOrder});
            this.mlistView_data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mlistView_data.FullRowSelect = true;
            this.mlistView_data.GridLines = true;
            this.mlistView_data.Location = new System.Drawing.Point(5, 292);
            this.mlistView_data.Name = "mlistView_data";
            this.mlistView_data.Size = new System.Drawing.Size(754, 455);
            this.mlistView_data.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.mlistView_data.TabIndex = 49;
            this.mlistView_data.UseCompatibleStateImageBehavior = false;
            this.mlistView_data.View = System.Windows.Forms.View.Details;
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
            // mgroupBox_selectData
            // 
            this.mgroupBox_selectData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_selectData.Controls.Add(this.mpanel_line);
            this.mgroupBox_selectData.Controls.Add(this.lblConnection);
            this.mgroupBox_selectData.Controls.Add(this.progressBarConnection);
            this.mgroupBox_selectData.Controls.Add(this.label3);
            this.mgroupBox_selectData.Controls.Add(this.label2);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_selectFilesFromDisk);
            this.mgroupBox_selectData.Controls.Add(this.labelSelect);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_selectDatasetIDFile);
            this.mgroupBox_selectData.Controls.Add(this.textfilterBox);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_datasetSearch);
            this.mgroupBox_selectData.Controls.Add(this.labelInsidePNNLOnly);
            this.mgroupBox_selectData.Controls.Add(this.label1);
            this.mgroupBox_selectData.Controls.Add(this.mbutton_extractDatasets);
            this.mgroupBox_selectData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mgroupBox_selectData.Location = new System.Drawing.Point(4, 3);
            this.mgroupBox_selectData.Name = "mgroupBox_selectData";
            this.mgroupBox_selectData.Size = new System.Drawing.Size(747, 187);
            this.mgroupBox_selectData.TabIndex = 48;
            this.mgroupBox_selectData.TabStop = false;
            this.mgroupBox_selectData.Text = "Select Data";
            this.mgroupBox_selectData.Enter += new System.EventHandler(this.mgroupBox_selectData_Enter);
            // 
            // lblConnection
            // 
            this.lblConnection.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblConnection.Location = new System.Drawing.Point(280, 88);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(195, 18);
            this.lblConnection.TabIndex = 38;
            this.lblConnection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblConnection.Visible = false;
            // 
            // progressBarConnection
            // 
            this.progressBarConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarConnection.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.progressBarConnection.ForeColor = System.Drawing.Color.Lime;
            this.progressBarConnection.Location = new System.Drawing.Point(481, 88);
            this.progressBarConnection.Name = "progressBarConnection";
            this.progressBarConnection.Size = new System.Drawing.Size(260, 18);
            this.progressBarConnection.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarConnection.TabIndex = 39;
            this.progressBarConnection.Visible = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(46, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 22);
            this.label2.TabIndex = 48;
            this.label2.Text = "Select .PEK, _is.CSV files from disk:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(46, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(225, 22);
            this.label3.TabIndex = 49;
            this.label3.Text = "Extract datasets from MultiAlign Analysis File:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelLoadParam);
            this.groupBox1.Controls.Add(this.mbtnLoadPara);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(748, 60);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load Parameters";
            // 
            // mbutton_selectAll
            // 
            this.mbutton_selectAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_selectAll.Enabled = false;
            this.mbutton_selectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_selectAll.Location = new System.Drawing.Point(163, 255);
            this.mbutton_selectAll.Name = "mbutton_selectAll";
            this.mbutton_selectAll.Size = new System.Drawing.Size(77, 24);
            this.mbutton_selectAll.TabIndex = 53;
            this.mbutton_selectAll.Text = "Check All";
            this.mbutton_selectAll.UseVisualStyleBackColor = false;
            // 
            // mbutton_uncheckAll
            // 
            this.mbutton_uncheckAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_uncheckAll.Enabled = false;
            this.mbutton_uncheckAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_uncheckAll.Location = new System.Drawing.Point(74, 255);
            this.mbutton_uncheckAll.Name = "mbutton_uncheckAll";
            this.mbutton_uncheckAll.Size = new System.Drawing.Size(83, 24);
            this.mbutton_uncheckAll.TabIndex = 52;
            this.mbutton_uncheckAll.Text = "Uncheck All";
            this.mbutton_uncheckAll.UseVisualStyleBackColor = false;
            // 
            // mbutton_clearAll
            // 
            this.mbutton_clearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_clearAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_clearAll.Enabled = false;
            this.mbutton_clearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clearAll.Location = new System.Drawing.Point(681, 255);
            this.mbutton_clearAll.Name = "mbutton_clearAll";
            this.mbutton_clearAll.Size = new System.Drawing.Size(64, 24);
            this.mbutton_clearAll.TabIndex = 51;
            this.mbutton_clearAll.Text = "Clear All";
            this.mbutton_clearAll.UseVisualStyleBackColor = false;
            // 
            // mbutton_toggleAll
            // 
            this.mbutton_toggleAll.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_toggleAll.Enabled = false;
            this.mbutton_toggleAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_toggleAll.Location = new System.Drawing.Point(4, 255);
            this.mbutton_toggleAll.Name = "mbutton_toggleAll";
            this.mbutton_toggleAll.Size = new System.Drawing.Size(64, 24);
            this.mbutton_toggleAll.TabIndex = 50;
            this.mbutton_toggleAll.Text = "Toggle All";
            this.mbutton_toggleAll.UseVisualStyleBackColor = false;
            // 
            // mpanel_line
            // 
            this.mpanel_line.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mpanel_line.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpanel_line.Location = new System.Drawing.Point(49, 109);
            this.mpanel_line.Name = "mpanel_line";
            this.mpanel_line.Size = new System.Drawing.Size(693, 1);
            this.mpanel_line.TabIndex = 50;
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
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect All";
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
            // 
            // clearSelectedToolStripMenuItem
            // 
            this.clearSelectedToolStripMenuItem.Name = "clearSelectedToolStripMenuItem";
            this.clearSelectedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.clearSelectedToolStripMenuItem.Text = "Clear Selected";
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
            // 
            // mbutton_clearChecked
            // 
            this.mbutton_clearChecked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_clearChecked.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_clearChecked.Enabled = false;
            this.mbutton_clearChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clearChecked.Location = new System.Drawing.Point(469, 255);
            this.mbutton_clearChecked.Name = "mbutton_clearChecked";
            this.mbutton_clearChecked.Size = new System.Drawing.Size(100, 24);
            this.mbutton_clearChecked.TabIndex = 55;
            this.mbutton_clearChecked.Text = "Clear Checked";
            this.mbutton_clearChecked.UseVisualStyleBackColor = false;
            // 
            // mbutton_clearUnchecked
            // 
            this.mbutton_clearUnchecked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_clearUnchecked.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_clearUnchecked.Enabled = false;
            this.mbutton_clearUnchecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_clearUnchecked.Location = new System.Drawing.Point(575, 255);
            this.mbutton_clearUnchecked.Name = "mbutton_clearUnchecked";
            this.mbutton_clearUnchecked.Size = new System.Drawing.Size(100, 24);
            this.mbutton_clearUnchecked.TabIndex = 56;
            this.mbutton_clearUnchecked.Text = "Clear Unchecked";
            this.mbutton_clearUnchecked.UseVisualStyleBackColor = false;
            // 
            // ctlSelectDataSourceWizardPage
            // 
            this.Controls.Add(this.panelOptions);
            this.Name = "ctlSelectDataSourceWizardPage";
            this.Size = new System.Drawing.Size(898, 816);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.panelOptions, 0);
            this.panelOptions.ResumeLayout(false);
            this.mpanelOptions.ResumeLayout(false);
            this.panelStep.ResumeLayout(false);
            this.mgroupBox_selectData.ResumeLayout(false);
            this.mgroupBox_selectData.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.mcontextMenu_dataset.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private Panel panelOptions;
        private Panel mpanelOptions;
        private Button mbtnLoadPara;
        private Label labelLoadParam;
        private IContainer components = null;
        private Button btnEnableDatabase;
        private Label labelInsidePNNLOnly;
        private Panel panelStep;
        private Button mbutton_datasetSearch;
        private TextBox textfilterBox;
        private Label labelSelect;
        private Button mbutton_extractDatasets;
        private Button mbutton_selectFilesFromDisk;
        private Button mbutton_selectDatasetIDFile;
        private Label label1;
        private ListView mlistView_data;
        private ColumnHeader datasetID;
        private ColumnHeader jobnum;
        private ColumnHeader fileName;
        private ColumnHeader alias;
        private ColumnHeader block;
        private ColumnHeader runOrder;
        private GroupBox mgroupBox_selectData;
        private ProgressBar progressBarConnection;
        private Label lblConnection;
        private Label label2;
        private Label label3;
        private GroupBox groupBox1;
        private Button mbutton_selectAll;
        private Button mbutton_uncheckAll;
        private Button mbutton_clearAll;
        private Button mbutton_toggleAll;
        private Panel mpanel_line;
        private ContextMenuStrip mcontextMenu_dataset;
        private ToolStripMenuItem toggleAllToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem deselectAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem clearUnSelectedToolStripMenuItem;
        private ToolStripMenuItem clearSelectedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem clearAllToolStripMenuItem;
        private Button mbutton_clearUnchecked;
        private Button mbutton_clearChecked;
		
    }
}



