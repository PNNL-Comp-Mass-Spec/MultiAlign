namespace MultiAlignWin.Forms
{
    partial class controlClusterInformation
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
                CloseAllOpenCharts();

                components.Dispose();
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
            this.label3 = new System.Windows.Forms.Label();
            this.mpictureBox_massNetResiduals = new System.Windows.Forms.PictureBox();
            this.mlabel_mzMassResiduals = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mpictureBox_mzMassResidual = new System.Windows.Forms.PictureBox();
            this.mlabel_netResiduals = new System.Windows.Forms.Label();
            this.mpicture_massResiduals = new System.Windows.Forms.PictureBox();
            this.mlabel_netErrorHistogram = new System.Windows.Forms.Label();
            this.mpictureBox_netErrorHistogram = new System.Windows.Forms.PictureBox();
            this.mlabel_massErrorHistogram = new System.Windows.Forms.Label();
            this.mpictureBox_massErrorHistogram = new System.Windows.Forms.PictureBox();
            this.mlabel_alignment = new System.Windows.Forms.Label();
            this.mpicture_preview = new System.Windows.Forms.PictureBox();
            this.mpicture_alignmentHeatmap = new System.Windows.Forms.PictureBox();
            this.mlabel_stdevNET = new System.Windows.Forms.Label();
            this.mlabel_stdevMass = new System.Windows.Forms.Label();
            this.mpictureBox_peakMatchingResiduals = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mpictureBox_scoreVsClusterSize = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.mpictureBox_scoreHistograms = new System.Windows.Forms.PictureBox();
            this.mpictureBox_SMARTScoreHistogram = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.mpictureBox_clusterSizeHistogram = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.mpictureBox_unidentifiedScatterPlot = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mpictureBox_chargeStateHistogram = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.mlabel_Clustering = new System.Windows.Forms.Label();
            this.labelAlignment = new System.Windows.Forms.Label();
            this.mpictureBox_identifiedFeatures = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_massNetResiduals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_mzMassResidual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_massResiduals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_netErrorHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_massErrorHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignmentHeatmap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_peakMatchingResiduals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_scoreVsClusterSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_scoreHistograms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_SMARTScoreHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_clusterSizeHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_unidentifiedScatterPlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_chargeStateHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_identifiedFeatures)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(786, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 30);
            this.label3.TabIndex = 44;
            this.label3.Text = "Alignment Mass vs NET Residual";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_massNetResiduals
            // 
            this.mpictureBox_massNetResiduals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_massNetResiduals.Location = new System.Drawing.Point(789, 272);
            this.mpictureBox_massNetResiduals.Name = "mpictureBox_massNetResiduals";
            this.mpictureBox_massNetResiduals.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_massNetResiduals.TabIndex = 43;
            this.mpictureBox_massNetResiduals.TabStop = false;
            this.mpictureBox_massNetResiduals.Click += new System.EventHandler(this.mpictureBox_massNetResiduals_Click);
            // 
            // mlabel_mzMassResiduals
            // 
            this.mlabel_mzMassResiduals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_mzMassResiduals.Location = new System.Drawing.Point(268, 236);
            this.mlabel_mzMassResiduals.Name = "mlabel_mzMassResiduals";
            this.mlabel_mzMassResiduals.Size = new System.Drawing.Size(114, 32);
            this.mlabel_mzMassResiduals.TabIndex = 38;
            this.mlabel_mzMassResiduals.Text = "Mass Residuals (scan)";
            this.mlabel_mzMassResiduals.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(531, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 30);
            this.label2.TabIndex = 42;
            this.label2.Text = "Mass Residual (m/z)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_mzMassResidual
            // 
            this.mpictureBox_mzMassResidual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_mzMassResidual.Location = new System.Drawing.Point(528, 272);
            this.mpictureBox_mzMassResidual.Name = "mpictureBox_mzMassResidual";
            this.mpictureBox_mzMassResidual.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_mzMassResidual.TabIndex = 41;
            this.mpictureBox_mzMassResidual.TabStop = false;
            this.mpictureBox_mzMassResidual.Click += new System.EventHandler(this.mpictureBox_mzMassResidual_Click);
            // 
            // mlabel_netResiduals
            // 
            this.mlabel_netResiduals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_netResiduals.Location = new System.Drawing.Point(654, 238);
            this.mlabel_netResiduals.Name = "mlabel_netResiduals";
            this.mlabel_netResiduals.Size = new System.Drawing.Size(120, 30);
            this.mlabel_netResiduals.TabIndex = 40;
            this.mlabel_netResiduals.Text = "NET Residuals";
            this.mlabel_netResiduals.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpicture_massResiduals
            // 
            this.mpicture_massResiduals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_massResiduals.Location = new System.Drawing.Point(268, 271);
            this.mpicture_massResiduals.Name = "mpicture_massResiduals";
            this.mpicture_massResiduals.Size = new System.Drawing.Size(113, 108);
            this.mpicture_massResiduals.TabIndex = 37;
            this.mpicture_massResiduals.TabStop = false;
            this.mpicture_massResiduals.Click += new System.EventHandler(this.mpicture_massResiduals_Click);
            // 
            // mlabel_netErrorHistogram
            // 
            this.mlabel_netErrorHistogram.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_netErrorHistogram.Location = new System.Drawing.Point(139, 239);
            this.mlabel_netErrorHistogram.Name = "mlabel_netErrorHistogram";
            this.mlabel_netErrorHistogram.Size = new System.Drawing.Size(116, 30);
            this.mlabel_netErrorHistogram.TabIndex = 36;
            this.mlabel_netErrorHistogram.Text = "NET Error Histogram";
            this.mlabel_netErrorHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_netErrorHistogram
            // 
            this.mpictureBox_netErrorHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_netErrorHistogram.Location = new System.Drawing.Point(139, 272);
            this.mpictureBox_netErrorHistogram.Name = "mpictureBox_netErrorHistogram";
            this.mpictureBox_netErrorHistogram.Size = new System.Drawing.Size(116, 107);
            this.mpictureBox_netErrorHistogram.TabIndex = 35;
            this.mpictureBox_netErrorHistogram.TabStop = false;
            this.mpictureBox_netErrorHistogram.Click += new System.EventHandler(this.mpictureBox_netErrorHistogram_Click);
            // 
            // mlabel_massErrorHistogram
            // 
            this.mlabel_massErrorHistogram.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_massErrorHistogram.Location = new System.Drawing.Point(15, 239);
            this.mlabel_massErrorHistogram.Name = "mlabel_massErrorHistogram";
            this.mlabel_massErrorHistogram.Size = new System.Drawing.Size(114, 30);
            this.mlabel_massErrorHistogram.TabIndex = 34;
            this.mlabel_massErrorHistogram.Text = "Mass Error Histogram";
            this.mlabel_massErrorHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_massErrorHistogram
            // 
            this.mpictureBox_massErrorHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_massErrorHistogram.Location = new System.Drawing.Point(15, 272);
            this.mpictureBox_massErrorHistogram.Name = "mpictureBox_massErrorHistogram";
            this.mpictureBox_massErrorHistogram.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_massErrorHistogram.TabIndex = 33;
            this.mpictureBox_massErrorHistogram.TabStop = false;
            this.mpictureBox_massErrorHistogram.Click += new System.EventHandler(this.mpictureBox_massErrorHistogram_Click);
            // 
            // mlabel_alignment
            // 
            this.mlabel_alignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_alignment.Location = new System.Drawing.Point(396, 236);
            this.mlabel_alignment.Name = "mlabel_alignment";
            this.mlabel_alignment.Size = new System.Drawing.Size(117, 27);
            this.mlabel_alignment.TabIndex = 32;
            this.mlabel_alignment.Text = "Cluster Alignment Heatmap";
            this.mlabel_alignment.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpicture_preview
            // 
            this.mpicture_preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_preview.Location = new System.Drawing.Point(657, 272);
            this.mpicture_preview.Name = "mpicture_preview";
            this.mpicture_preview.Size = new System.Drawing.Size(116, 107);
            this.mpicture_preview.TabIndex = 30;
            this.mpicture_preview.TabStop = false;
            this.mpicture_preview.Click += new System.EventHandler(this.mpicture_preview_Click);
            // 
            // mpicture_alignmentHeatmap
            // 
            this.mpicture_alignmentHeatmap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpicture_alignmentHeatmap.Location = new System.Drawing.Point(395, 272);
            this.mpicture_alignmentHeatmap.Name = "mpicture_alignmentHeatmap";
            this.mpicture_alignmentHeatmap.Size = new System.Drawing.Size(118, 107);
            this.mpicture_alignmentHeatmap.TabIndex = 29;
            this.mpicture_alignmentHeatmap.TabStop = false;
            this.mpicture_alignmentHeatmap.Click += new System.EventHandler(this.mpicture_alignmentHeatmap_Click);
            // 
            // mlabel_stdevNET
            // 
            this.mlabel_stdevNET.BackColor = System.Drawing.Color.Transparent;
            this.mlabel_stdevNET.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_stdevNET.Location = new System.Drawing.Point(136, 382);
            this.mlabel_stdevNET.Name = "mlabel_stdevNET";
            this.mlabel_stdevNET.Size = new System.Drawing.Size(117, 30);
            this.mlabel_stdevNET.TabIndex = 46;
            this.mlabel_stdevNET.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mlabel_stdevMass
            // 
            this.mlabel_stdevMass.BackColor = System.Drawing.Color.Transparent;
            this.mlabel_stdevMass.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_stdevMass.Location = new System.Drawing.Point(15, 382);
            this.mlabel_stdevMass.Name = "mlabel_stdevMass";
            this.mlabel_stdevMass.Size = new System.Drawing.Size(114, 30);
            this.mlabel_stdevMass.TabIndex = 45;
            this.mlabel_stdevMass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_peakMatchingResiduals
            // 
            this.mpictureBox_peakMatchingResiduals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_peakMatchingResiduals.Location = new System.Drawing.Point(15, 514);
            this.mpictureBox_peakMatchingResiduals.Name = "mpictureBox_peakMatchingResiduals";
            this.mpictureBox_peakMatchingResiduals.Size = new System.Drawing.Size(112, 107);
            this.mpictureBox_peakMatchingResiduals.TabIndex = 47;
            this.mpictureBox_peakMatchingResiduals.TabStop = false;
            this.mpictureBox_peakMatchingResiduals.Click += new System.EventHandler(this.mpictureBox_peakMatchingResiduals_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 481);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 30);
            this.label1.TabIndex = 48;
            this.label1.Text = "Peak Matching Mass vs NET Residual";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_scoreVsClusterSize
            // 
            this.mpictureBox_scoreVsClusterSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_scoreVsClusterSize.Location = new System.Drawing.Point(139, 69);
            this.mpictureBox_scoreVsClusterSize.Name = "mpictureBox_scoreVsClusterSize";
            this.mpictureBox_scoreVsClusterSize.Size = new System.Drawing.Size(114, 107);
            this.mpictureBox_scoreVsClusterSize.TabIndex = 49;
            this.mpictureBox_scoreVsClusterSize.TabStop = false;
            this.mpictureBox_scoreVsClusterSize.Click += new System.EventHandler(this.mpictureBox_scoreVsClusterSize_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(139, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 30);
            this.label4.TabIndex = 50;
            this.label4.Text = "Cluster Score  vs. Cluster Size ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 30);
            this.label5.TabIndex = 50;
            this.label5.Text = "Cluster Score Histograms";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_scoreHistograms
            // 
            this.mpictureBox_scoreHistograms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_scoreHistograms.Location = new System.Drawing.Point(15, 69);
            this.mpictureBox_scoreHistograms.Name = "mpictureBox_scoreHistograms";
            this.mpictureBox_scoreHistograms.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_scoreHistograms.TabIndex = 49;
            this.mpictureBox_scoreHistograms.TabStop = false;
            this.mpictureBox_scoreHistograms.Click += new System.EventHandler(this.mpictureBox_scoreHistograms_Click);
            // 
            // mpictureBox_SMARTScoreHistogram
            // 
            this.mpictureBox_SMARTScoreHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_SMARTScoreHistogram.Location = new System.Drawing.Point(377, 514);
            this.mpictureBox_SMARTScoreHistogram.Name = "mpictureBox_SMARTScoreHistogram";
            this.mpictureBox_SMARTScoreHistogram.Size = new System.Drawing.Size(116, 107);
            this.mpictureBox_SMARTScoreHistogram.TabIndex = 51;
            this.mpictureBox_SMARTScoreHistogram.TabStop = false;
            this.mpictureBox_SMARTScoreHistogram.Click += new System.EventHandler(this.mpictureBox_SMARTScoreHistogram_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(377, 481);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 30);
            this.label6.TabIndex = 52;
            this.label6.Text = "STAC Score Histogram";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_clusterSizeHistogram
            // 
            this.mpictureBox_clusterSizeHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_clusterSizeHistogram.Location = new System.Drawing.Point(268, 69);
            this.mpictureBox_clusterSizeHistogram.Name = "mpictureBox_clusterSizeHistogram";
            this.mpictureBox_clusterSizeHistogram.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_clusterSizeHistogram.TabIndex = 53;
            this.mpictureBox_clusterSizeHistogram.TabStop = false;
            this.mpictureBox_clusterSizeHistogram.Click += new System.EventHandler(this.mpictureBox_clusterSizeHistogram_Click);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(265, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 30);
            this.label7.TabIndex = 54;
            this.label7.Text = "Cluster Size Histogram";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_unidentifiedScatterPlot
            // 
            this.mpictureBox_unidentifiedScatterPlot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_unidentifiedScatterPlot.Location = new System.Drawing.Point(135, 514);
            this.mpictureBox_unidentifiedScatterPlot.Name = "mpictureBox_unidentifiedScatterPlot";
            this.mpictureBox_unidentifiedScatterPlot.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_unidentifiedScatterPlot.TabIndex = 55;
            this.mpictureBox_unidentifiedScatterPlot.TabStop = false;
            this.mpictureBox_unidentifiedScatterPlot.Click += new System.EventHandler(this.mpictureBox_SMARTScoreSpatialPlot_Click);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(133, 472);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 39);
            this.label8.TabIndex = 56;
            this.label8.Text = "Unidentified Features Scatter Plot";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpictureBox_chargeStateHistogram
            // 
            this.mpictureBox_chargeStateHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_chargeStateHistogram.Location = new System.Drawing.Point(400, 69);
            this.mpictureBox_chargeStateHistogram.Name = "mpictureBox_chargeStateHistogram";
            this.mpictureBox_chargeStateHistogram.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_chargeStateHistogram.TabIndex = 57;
            this.mpictureBox_chargeStateHistogram.TabStop = false;
            this.mpictureBox_chargeStateHistogram.Click += new System.EventHandler(this.mpictureBox_chargeStateHistogram_Click);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(397, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 30);
            this.label9.TabIndex = 58;
            this.label9.Text = "Charge State Histogram";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label10.Location = new System.Drawing.Point(8, 442);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(168, 25);
            this.label10.TabIndex = 59;
            this.label10.Text = "Peak Matching";
            // 
            // mlabel_Clustering
            // 
            this.mlabel_Clustering.AutoSize = true;
            this.mlabel_Clustering.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlabel_Clustering.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.mlabel_Clustering.Location = new System.Drawing.Point(8, 11);
            this.mlabel_Clustering.Name = "mlabel_Clustering";
            this.mlabel_Clustering.Size = new System.Drawing.Size(119, 25);
            this.mlabel_Clustering.TabIndex = 60;
            this.mlabel_Clustering.Text = "Clustering";
            // 
            // labelAlignment
            // 
            this.labelAlignment.AutoSize = true;
            this.labelAlignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAlignment.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.labelAlignment.Location = new System.Drawing.Point(7, 203);
            this.labelAlignment.Name = "labelAlignment";
            this.labelAlignment.Size = new System.Drawing.Size(198, 25);
            this.labelAlignment.TabIndex = 61;
            this.labelAlignment.Text = "Cluster Alignment";
            // 
            // mpictureBox_identifiedFeatures
            // 
            this.mpictureBox_identifiedFeatures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpictureBox_identifiedFeatures.Location = new System.Drawing.Point(256, 514);
            this.mpictureBox_identifiedFeatures.Name = "mpictureBox_identifiedFeatures";
            this.mpictureBox_identifiedFeatures.Size = new System.Drawing.Size(113, 107);
            this.mpictureBox_identifiedFeatures.TabIndex = 62;
            this.mpictureBox_identifiedFeatures.TabStop = false;
            this.mpictureBox_identifiedFeatures.Click += new System.EventHandler(this.mpictureBox_identifiedFeatures_Click);
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(254, 472);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(117, 39);
            this.label11.TabIndex = 63;
            this.label11.Text = "Identified Features Scatter Plot";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlClusterInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mpictureBox_identifiedFeatures);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.labelAlignment);
            this.Controls.Add(this.mlabel_Clustering);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.mpictureBox_chargeStateHistogram);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.mpictureBox_unidentifiedScatterPlot);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.mpictureBox_clusterSizeHistogram);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.mpictureBox_SMARTScoreHistogram);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.mpictureBox_scoreHistograms);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mpictureBox_scoreVsClusterSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mpictureBox_peakMatchingResiduals);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mlabel_stdevMass);
            this.Controls.Add(this.mlabel_stdevNET);
            this.Controls.Add(this.mpictureBox_massErrorHistogram);
            this.Controls.Add(this.mpictureBox_massNetResiduals);
            this.Controls.Add(this.mpicture_alignmentHeatmap);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mlabel_mzMassResiduals);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mpictureBox_mzMassResidual);
            this.Controls.Add(this.mlabel_netResiduals);
            this.Controls.Add(this.mpicture_massResiduals);
            this.Controls.Add(this.mlabel_netErrorHistogram);
            this.Controls.Add(this.mpictureBox_netErrorHistogram);
            this.Controls.Add(this.mlabel_massErrorHistogram);
            this.Controls.Add(this.mlabel_alignment);
            this.Controls.Add(this.mpicture_preview);
            this.Name = "controlClusterInformation";
            this.Size = new System.Drawing.Size(1037, 673);
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_massNetResiduals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_mzMassResidual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_massResiduals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_netErrorHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_massErrorHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpicture_alignmentHeatmap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_peakMatchingResiduals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_scoreVsClusterSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_scoreHistograms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_SMARTScoreHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_clusterSizeHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_unidentifiedScatterPlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_chargeStateHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mpictureBox_identifiedFeatures)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox mpictureBox_massNetResiduals;
        private System.Windows.Forms.Label mlabel_mzMassResiduals;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox mpictureBox_mzMassResidual;
        private System.Windows.Forms.Label mlabel_netResiduals;
        private System.Windows.Forms.PictureBox mpicture_massResiduals;
        private System.Windows.Forms.Label mlabel_netErrorHistogram;
        private System.Windows.Forms.PictureBox mpictureBox_netErrorHistogram;
        private System.Windows.Forms.Label mlabel_massErrorHistogram;
        private System.Windows.Forms.PictureBox mpictureBox_massErrorHistogram;
        private System.Windows.Forms.Label mlabel_alignment;
        private System.Windows.Forms.PictureBox mpicture_preview;
        private System.Windows.Forms.PictureBox mpicture_alignmentHeatmap;
        private System.Windows.Forms.Label mlabel_stdevNET;
        private System.Windows.Forms.Label mlabel_stdevMass;
        private System.Windows.Forms.PictureBox mpictureBox_peakMatchingResiduals;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox mpictureBox_scoreVsClusterSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox mpictureBox_scoreHistograms;
        private System.Windows.Forms.PictureBox mpictureBox_SMARTScoreHistogram;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox mpictureBox_clusterSizeHistogram;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox mpictureBox_unidentifiedScatterPlot;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox mpictureBox_chargeStateHistogram;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label mlabel_Clustering;
        private System.Windows.Forms.Label labelAlignment;
        private System.Windows.Forms.PictureBox mpictureBox_identifiedFeatures;
        private System.Windows.Forms.Label label11;
    }
}
