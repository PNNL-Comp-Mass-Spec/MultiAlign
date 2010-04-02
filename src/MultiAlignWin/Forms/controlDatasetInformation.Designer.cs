namespace MultiAlignWin.Forms
{
    partial class controlDatasetInformation
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
                mpicture_alignmentHeatmap.Image.Dispose();
                mpicture_preview.Image.Dispose();
                mpicture_rawData.Image.Dispose();

                components.Dispose();

                this.mobj_analysis = null;
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
            this.mbutton_datasetName = new System.Windows.Forms.LinkLabel();
            this.mlabel_datasetNameDescription = new System.Windows.Forms.Label();
            this.mlabel_preview = new System.Windows.Forms.Label();
            this.mlabel_alignment = new System.Windows.Forms.Label();
            this.mlabel_datasetIDDescription = new System.Windows.Forms.Label();
            this.mlabel_datasetID = new System.Windows.Forms.Label();
            this.mpicture_preview = new System.Windows.Forms.PictureBox();
            this.mpicture_alignmentHeatmap = new System.Windows.Forms.PictureBox();
            this.mlabel_baselineDescription = new System.Windows.Forms.Label();
            this.mbutton_baseline = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.mpicture_rawData = new System.Windows.Forms.PictureBox();
            this.mlabel_umcs = new System.Windows.Forms.Label();
            this.mlabel_umcsDescription = new System.Windows.Forms.Label();
            this.mlabel_massErrorHistogram = new System.Windows.Forms.Label();
            this.mpictureBox_massErrorHistogram = new System.Windows.Forms.PictureBox();
            this.mlabel_netErrorHistogram = new System.Windows.Forms.Label();
            this.mpictureBox_netErrorHistogram = new System.Windows.Forms.PictureBox();
            this.mlabel_netResiduals = new System.Windows.Forms.Label();
            this.mpicture_netResiduals = new System.Windows.Forms.PictureBox();
            this.mlabel_mzMassResiduals = new System.Windows.Forms.Label();
            this.mpicture_massResiduals = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mpictureBox_mzMassResidual = new System.Windows.Forms.PictureBox();
            this.mlistview_stats = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignmentHeatmap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_rawData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_massErrorHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_netErrorHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_netResiduals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_massResiduals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_mzMassResidual)).BeginInit();
            this.SuspendLayout();
            // 
            // mbutton_datasetName
            // 
            this.mbutton_datasetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_datasetName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_datasetName.Location = new System.Drawing.Point(96, 14);
            this.mbutton_datasetName.Name = "mbutton_datasetName";
            this.mbutton_datasetName.Size = new System.Drawing.Size(1078, 22);
            this.mbutton_datasetName.TabIndex = 1;
            this.mbutton_datasetName.TabStop = true;
            this.mbutton_datasetName.Text = "datasetname";
            this.mbutton_datasetName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mbutton_datasetName_LinkClicked);
            // 
            // mlabel_datasetNameDescription
            // 
            this.mlabel_datasetNameDescription.AutoSize = true;
            this.mlabel_datasetNameDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_datasetNameDescription.Location = new System.Drawing.Point(97, 1);
            this.mlabel_datasetNameDescription.Name = "mlabel_datasetNameDescription";
            this.mlabel_datasetNameDescription.Size = new System.Drawing.Size(87, 13);
            this.mlabel_datasetNameDescription.TabIndex = 2;
            this.mlabel_datasetNameDescription.Text = "Dataset Name";
            // 
            // mlabel_preview
            // 
            this.mlabel_preview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_preview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_preview.Location = new System.Drawing.Point(643, 199);
            this.mlabel_preview.Name = "mlabel_preview";
            this.mlabel_preview.Size = new System.Drawing.Size(84, 30);
            this.mlabel_preview.TabIndex = 5;
            this.mlabel_preview.Text = "Scan vs. Net";
            this.mlabel_preview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_alignment
            // 
            this.mlabel_alignment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_alignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_alignment.Location = new System.Drawing.Point(97, 199);
            this.mlabel_alignment.Name = "mlabel_alignment";
            this.mlabel_alignment.Size = new System.Drawing.Size(84, 30);
            this.mlabel_alignment.TabIndex = 6;
            this.mlabel_alignment.Text = "Alignment";
            this.mlabel_alignment.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mlabel_alignment.Visible = false;
            // 
            // mlabel_datasetIDDescription
            // 
            this.mlabel_datasetIDDescription.AutoSize = true;
            this.mlabel_datasetIDDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_datasetIDDescription.Location = new System.Drawing.Point(6, 3);
            this.mlabel_datasetIDDescription.Name = "mlabel_datasetIDDescription";
            this.mlabel_datasetIDDescription.Size = new System.Drawing.Size(68, 13);
            this.mlabel_datasetIDDescription.TabIndex = 7;
            this.mlabel_datasetIDDescription.Text = "Dataset ID";
            // 
            // mlabel_datasetID
            // 
            this.mlabel_datasetID.Location = new System.Drawing.Point(6, 19);
            this.mlabel_datasetID.Name = "mlabel_datasetID";
            this.mlabel_datasetID.Size = new System.Drawing.Size(58, 13);
            this.mlabel_datasetID.TabIndex = 8;
            this.mlabel_datasetID.Text = "0";
            this.mlabel_datasetID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpicture_preview
            // 
            this.mpicture_preview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpicture_preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_preview.Location = new System.Drawing.Point(643, 127);
            this.mpicture_preview.Name = "mpicture_preview";
            this.mpicture_preview.Size = new System.Drawing.Size(84, 69);
            this.mpicture_preview.TabIndex = 4;
            this.mpicture_preview.TabStop = false;
            this.mpicture_preview.Click += new System.EventHandler(this.mpicture_preview_Click);
            // 
            // mpicture_alignmentHeatmap
            // 
            this.mpicture_alignmentHeatmap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpicture_alignmentHeatmap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_alignmentHeatmap.Location = new System.Drawing.Point(97, 127);
            this.mpicture_alignmentHeatmap.Name = "mpicture_alignmentHeatmap";
            this.mpicture_alignmentHeatmap.Size = new System.Drawing.Size(84, 69);
            this.mpicture_alignmentHeatmap.TabIndex = 3;
            this.mpicture_alignmentHeatmap.TabStop = false;
            this.mpicture_alignmentHeatmap.Visible = false;
            // 
            // mlabel_baselineDescription
            // 
            this.mlabel_baselineDescription.AutoSize = true;
            this.mlabel_baselineDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_baselineDescription.Location = new System.Drawing.Point(99, 45);
            this.mlabel_baselineDescription.Name = "mlabel_baselineDescription";
            this.mlabel_baselineDescription.Size = new System.Drawing.Size(55, 13);
            this.mlabel_baselineDescription.TabIndex = 10;
            this.mlabel_baselineDescription.Text = "Baseline";
            // 
            // mbutton_baseline
            // 
            this.mbutton_baseline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_baseline.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbutton_baseline.Location = new System.Drawing.Point(98, 58);
            this.mbutton_baseline.Name = "mbutton_baseline";
            this.mbutton_baseline.Size = new System.Drawing.Size(629, 44);
            this.mbutton_baseline.TabIndex = 9;
            this.mbutton_baseline.TabStop = true;
            this.mbutton_baseline.Text = "baseline";
            this.mbutton_baseline.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mbutton_baseline_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 30);
            this.label1.TabIndex = 12;
            this.label1.Text = "Features";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpicture_rawData
            // 
            this.mpicture_rawData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpicture_rawData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_rawData.Location = new System.Drawing.Point(7, 127);
            this.mpicture_rawData.Name = "mpicture_rawData";
            this.mpicture_rawData.Size = new System.Drawing.Size(84, 69);
            this.mpicture_rawData.TabIndex = 11;
            this.mpicture_rawData.TabStop = false;
            this.mpicture_rawData.Click += new System.EventHandler(this.mpicture_rawData_Click);
            // 
            // mlabel_umcs
            // 
            this.mlabel_umcs.AutoSize = true;
            this.mlabel_umcs.Location = new System.Drawing.Point(144, 89);
            this.mlabel_umcs.Name = "mlabel_umcs";
            this.mlabel_umcs.Size = new System.Drawing.Size(13, 13);
            this.mlabel_umcs.TabIndex = 14;
            this.mlabel_umcs.Text = "0";
            this.mlabel_umcs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_umcsDescription
            // 
            this.mlabel_umcsDescription.AutoSize = true;
            this.mlabel_umcsDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_umcsDescription.Location = new System.Drawing.Point(96, 89);
            this.mlabel_umcsDescription.Name = "mlabel_umcsDescription";
            this.mlabel_umcsDescription.Size = new System.Drawing.Size(40, 13);
            this.mlabel_umcsDescription.TabIndex = 13;
            this.mlabel_umcsDescription.Text = "UMCs";
            // 
            // mlabel_massErrorHistogram
            // 
            this.mlabel_massErrorHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_massErrorHistogram.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_massErrorHistogram.Location = new System.Drawing.Point(187, 199);
            this.mlabel_massErrorHistogram.Name = "mlabel_massErrorHistogram";
            this.mlabel_massErrorHistogram.Size = new System.Drawing.Size(84, 30);
            this.mlabel_massErrorHistogram.TabIndex = 16;
            this.mlabel_massErrorHistogram.Text = "Mass Error Histogram";
            this.mlabel_massErrorHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_massErrorHistogram
            // 
            this.mpictureBox_massErrorHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpictureBox_massErrorHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_massErrorHistogram.Location = new System.Drawing.Point(187, 127);
            this.mpictureBox_massErrorHistogram.Name = "mpictureBox_massErrorHistogram";
            this.mpictureBox_massErrorHistogram.Size = new System.Drawing.Size(84, 69);
            this.mpictureBox_massErrorHistogram.TabIndex = 15;
            this.mpictureBox_massErrorHistogram.TabStop = false;
            this.mpictureBox_massErrorHistogram.Click += new System.EventHandler(this.mpictureBox_massErrorHistogram_Click);
            // 
            // mlabel_netErrorHistogram
            // 
            this.mlabel_netErrorHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_netErrorHistogram.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_netErrorHistogram.Location = new System.Drawing.Point(277, 199);
            this.mlabel_netErrorHistogram.Name = "mlabel_netErrorHistogram";
            this.mlabel_netErrorHistogram.Size = new System.Drawing.Size(84, 30);
            this.mlabel_netErrorHistogram.TabIndex = 18;
            this.mlabel_netErrorHistogram.Text = "NET Error Histogram";
            this.mlabel_netErrorHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_netErrorHistogram
            // 
            this.mpictureBox_netErrorHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpictureBox_netErrorHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_netErrorHistogram.Location = new System.Drawing.Point(277, 127);
            this.mpictureBox_netErrorHistogram.Name = "mpictureBox_netErrorHistogram";
            this.mpictureBox_netErrorHistogram.Size = new System.Drawing.Size(84, 69);
            this.mpictureBox_netErrorHistogram.TabIndex = 17;
            this.mpictureBox_netErrorHistogram.TabStop = false;
            this.mpictureBox_netErrorHistogram.Click += new System.EventHandler(this.mpictureBox_netErrorHistogram_Click);
            // 
            // mlabel_netResiduals
            // 
            this.mlabel_netResiduals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_netResiduals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_netResiduals.Location = new System.Drawing.Point(553, 199);
            this.mlabel_netResiduals.Name = "mlabel_netResiduals";
            this.mlabel_netResiduals.Size = new System.Drawing.Size(84, 30);
            this.mlabel_netResiduals.TabIndex = 22;
            this.mlabel_netResiduals.Text = "NET Residuals";
            this.mlabel_netResiduals.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpicture_netResiduals
            // 
            this.mpicture_netResiduals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpicture_netResiduals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_netResiduals.Location = new System.Drawing.Point(553, 127);
            this.mpicture_netResiduals.Name = "mpicture_netResiduals";
            this.mpicture_netResiduals.Size = new System.Drawing.Size(84, 69);
            this.mpicture_netResiduals.TabIndex = 21;
            this.mpicture_netResiduals.TabStop = false;
            this.mpicture_netResiduals.Click += new System.EventHandler(this.mpicture_netResiduals_Click);
            // 
            // mlabel_mzMassResiduals
            // 
            this.mlabel_mzMassResiduals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mlabel_mzMassResiduals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_mzMassResiduals.Location = new System.Drawing.Point(367, 199);
            this.mlabel_mzMassResiduals.Name = "mlabel_mzMassResiduals";
            this.mlabel_mzMassResiduals.Size = new System.Drawing.Size(96, 30);
            this.mlabel_mzMassResiduals.TabIndex = 20;
            this.mlabel_mzMassResiduals.Text = "Mass Residuals (scan)";
            this.mlabel_mzMassResiduals.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpicture_massResiduals
            // 
            this.mpicture_massResiduals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpicture_massResiduals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_massResiduals.Location = new System.Drawing.Point(373, 127);
            this.mpicture_massResiduals.Name = "mpicture_massResiduals";
            this.mpicture_massResiduals.Size = new System.Drawing.Size(84, 69);
            this.mpicture_massResiduals.TabIndex = 19;
            this.mpicture_massResiduals.TabStop = false;
            this.mpicture_massResiduals.Click += new System.EventHandler(this.mpicture_massResiduals_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(457, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 30);
            this.label2.TabIndex = 24;
            this.label2.Text = "Mass Residual (m/z)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_mzMassResidual
            // 
            this.mpictureBox_mzMassResidual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mpictureBox_mzMassResidual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_mzMassResidual.Location = new System.Drawing.Point(463, 127);
            this.mpictureBox_mzMassResidual.Name = "mpictureBox_mzMassResidual";
            this.mpictureBox_mzMassResidual.Size = new System.Drawing.Size(84, 69);
            this.mpictureBox_mzMassResidual.TabIndex = 23;
            this.mpictureBox_mzMassResidual.TabStop = false;
            this.mpictureBox_mzMassResidual.Click += new System.EventHandler(this.mpictureBox_mzMassResidual_Click);
            // 
            // mlistview_stats
            // 
            this.mlistview_stats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.mlistview_stats.GridLines = true;
            this.mlistview_stats.Location = new System.Drawing.Point(733, 19);
            this.mlistview_stats.Name = "mlistview_stats";
            this.mlistview_stats.Size = new System.Drawing.Size(410, 177);
            this.mlistview_stats.TabIndex = 25;
            this.mlistview_stats.UseCompatibleStateImageBehavior = false;
            this.mlistview_stats.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 345;
            // 
            // controlDatasetInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mlistview_stats);
            this.Controls.Add(this.mlabel_mzMassResiduals);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mpictureBox_mzMassResidual);
            this.Controls.Add(this.mlabel_netResiduals);
            this.Controls.Add(this.mpicture_netResiduals);
            this.Controls.Add(this.mpicture_massResiduals);
            this.Controls.Add(this.mlabel_netErrorHistogram);
            this.Controls.Add(this.mpictureBox_netErrorHistogram);
            this.Controls.Add(this.mlabel_massErrorHistogram);
            this.Controls.Add(this.mpictureBox_massErrorHistogram);
            this.Controls.Add(this.mlabel_umcs);
            this.Controls.Add(this.mlabel_umcsDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mpicture_rawData);
            this.Controls.Add(this.mbutton_datasetName);
            this.Controls.Add(this.mlabel_baselineDescription);
            this.Controls.Add(this.mbutton_baseline);
            this.Controls.Add(this.mlabel_datasetID);
            this.Controls.Add(this.mlabel_datasetIDDescription);
            this.Controls.Add(this.mlabel_alignment);
            this.Controls.Add(this.mlabel_preview);
            this.Controls.Add(this.mpicture_preview);
            this.Controls.Add(this.mpicture_alignmentHeatmap);
            this.Controls.Add(this.mlabel_datasetNameDescription);
            this.Name = "controlDatasetInformation";
            this.Size = new System.Drawing.Size(1177, 238);
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignmentHeatmap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_rawData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_massErrorHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_netErrorHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_netResiduals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_massResiduals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_mzMassResidual)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel mbutton_datasetName;
        private System.Windows.Forms.Label mlabel_datasetNameDescription;
        private System.Windows.Forms.PictureBox mpicture_alignmentHeatmap;
        private System.Windows.Forms.PictureBox mpicture_preview;
        private System.Windows.Forms.Label mlabel_preview;
        private System.Windows.Forms.Label mlabel_alignment;
        private System.Windows.Forms.Label mlabel_datasetIDDescription;
        private System.Windows.Forms.Label mlabel_datasetID;
        private System.Windows.Forms.Label mlabel_baselineDescription;
        private System.Windows.Forms.LinkLabel mbutton_baseline;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox mpicture_rawData;
        private System.Windows.Forms.Label mlabel_umcs;
        private System.Windows.Forms.Label mlabel_umcsDescription;
        private System.Windows.Forms.Label mlabel_massErrorHistogram;
        private System.Windows.Forms.PictureBox mpictureBox_massErrorHistogram;
        private System.Windows.Forms.Label mlabel_netErrorHistogram;
        private System.Windows.Forms.PictureBox mpictureBox_netErrorHistogram;
        private System.Windows.Forms.Label mlabel_netResiduals;
        private System.Windows.Forms.PictureBox mpicture_netResiduals;
        private System.Windows.Forms.Label mlabel_mzMassResiduals;
        private System.Windows.Forms.PictureBox mpicture_massResiduals;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox mpictureBox_mzMassResidual;
        private System.Windows.Forms.ListView mlistview_stats;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;

    }
}
