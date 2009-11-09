namespace PNNLControls.Drawing.Charting
{
    partial class formLinearPointFilter
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mtrackbar_xMinimum = new System.Windows.Forms.TrackBar();
            this.mtrackbar_xMaximum = new System.Windows.Forms.TrackBar();
            this.mlabel_xMinimum = new System.Windows.Forms.Label();
            this.mlabel_xMaximum = new System.Windows.Forms.Label();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mgroupBox_filtersX = new System.Windows.Forms.GroupBox();
            this.mgroupBox_filtersY = new System.Windows.Forms.GroupBox();
            this.mtrackbar_yMinimum = new System.Windows.Forms.TrackBar();
            this.mtrackbar_yMaximum = new System.Windows.Forms.TrackBar();
            this.mlabel_yMaximum = new System.Windows.Forms.Label();
            this.mlabel_yMinimum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_xMinimum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_xMaximum)).BeginInit();
            this.mgroupBox_filtersX.SuspendLayout();
            this.mgroupBox_filtersY.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_yMinimum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_yMaximum)).BeginInit();
            this.SuspendLayout();
            // 
            // mtrackbar_xMinimum
            // 
            this.mtrackbar_xMinimum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtrackbar_xMinimum.Location = new System.Drawing.Point(68, 13);
            this.mtrackbar_xMinimum.Name = "mtrackbar_xMinimum";
            this.mtrackbar_xMinimum.Size = new System.Drawing.Size(572, 39);
            this.mtrackbar_xMinimum.TabIndex = 0;
            this.mtrackbar_xMinimum.Scroll += new System.EventHandler(this.mtrackbar_xMinimum_Scroll);
            // 
            // mtrackbar_xMaximum
            // 
            this.mtrackbar_xMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtrackbar_xMaximum.Location = new System.Drawing.Point(68, 55);
            this.mtrackbar_xMaximum.Name = "mtrackbar_xMaximum";
            this.mtrackbar_xMaximum.Size = new System.Drawing.Size(572, 39);
            this.mtrackbar_xMaximum.TabIndex = 1;
            this.mtrackbar_xMaximum.Scroll += new System.EventHandler(this.mtrackbar_xMaximum_Scroll);
            // 
            // mlabel_xMinimum
            // 
            this.mlabel_xMinimum.Location = new System.Drawing.Point(11, 16);
            this.mlabel_xMinimum.Name = "mlabel_xMinimum";
            this.mlabel_xMinimum.Size = new System.Drawing.Size(51, 39);
            this.mlabel_xMinimum.TabIndex = 2;
            this.mlabel_xMinimum.Text = "Minimum ";
            // 
            // mlabel_xMaximum
            // 
            this.mlabel_xMaximum.Location = new System.Drawing.Point(11, 55);
            this.mlabel_xMaximum.Name = "mlabel_xMaximum";
            this.mlabel_xMaximum.Size = new System.Drawing.Size(51, 39);
            this.mlabel_xMaximum.TabIndex = 3;
            this.mlabel_xMaximum.Text = "Maximum";
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(606, 267);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(52, 24);
            this.mbutton_ok.TabIndex = 4;
            this.mbutton_ok.Text = "Ok";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            // 
            // mgroupBox_filtersX
            // 
            this.mgroupBox_filtersX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_filtersX.Controls.Add(this.mtrackbar_xMinimum);
            this.mgroupBox_filtersX.Controls.Add(this.mtrackbar_xMaximum);
            this.mgroupBox_filtersX.Controls.Add(this.mlabel_xMaximum);
            this.mgroupBox_filtersX.Controls.Add(this.mlabel_xMinimum);
            this.mgroupBox_filtersX.Location = new System.Drawing.Point(12, 12);
            this.mgroupBox_filtersX.Name = "mgroupBox_filtersX";
            this.mgroupBox_filtersX.Size = new System.Drawing.Size(646, 116);
            this.mgroupBox_filtersX.TabIndex = 5;
            this.mgroupBox_filtersX.TabStop = false;
            this.mgroupBox_filtersX.Text = "X-Filters";
            // 
            // mgroupBox_filtersY
            // 
            this.mgroupBox_filtersY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_filtersY.Controls.Add(this.mtrackbar_yMinimum);
            this.mgroupBox_filtersY.Controls.Add(this.mtrackbar_yMaximum);
            this.mgroupBox_filtersY.Controls.Add(this.mlabel_yMaximum);
            this.mgroupBox_filtersY.Controls.Add(this.mlabel_yMinimum);
            this.mgroupBox_filtersY.Location = new System.Drawing.Point(12, 134);
            this.mgroupBox_filtersY.Name = "mgroupBox_filtersY";
            this.mgroupBox_filtersY.Size = new System.Drawing.Size(646, 125);
            this.mgroupBox_filtersY.TabIndex = 6;
            this.mgroupBox_filtersY.TabStop = false;
            this.mgroupBox_filtersY.Text = "Y-Filters";
            // 
            // mtrackbar_yMinimum
            // 
            this.mtrackbar_yMinimum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtrackbar_yMinimum.Location = new System.Drawing.Point(68, 19);
            this.mtrackbar_yMinimum.Name = "mtrackbar_yMinimum";
            this.mtrackbar_yMinimum.Size = new System.Drawing.Size(572, 45);
            this.mtrackbar_yMinimum.TabIndex = 0;
            this.mtrackbar_yMinimum.Scroll += new System.EventHandler(this.mtrackbar_yMinimum_Scroll);
            // 
            // mtrackbar_yMaximum
            // 
            this.mtrackbar_yMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mtrackbar_yMaximum.Location = new System.Drawing.Point(68, 70);
            this.mtrackbar_yMaximum.Name = "mtrackbar_yMaximum";
            this.mtrackbar_yMaximum.Size = new System.Drawing.Size(572, 45);
            this.mtrackbar_yMaximum.TabIndex = 1;
            this.mtrackbar_yMaximum.Scroll += new System.EventHandler(this.mtrackbar_yMaximum_Scroll);
            // 
            // mlabel_yMaximum
            // 
            this.mlabel_yMaximum.Location = new System.Drawing.Point(11, 70);
            this.mlabel_yMaximum.Name = "mlabel_yMaximum";
            this.mlabel_yMaximum.Size = new System.Drawing.Size(51, 32);
            this.mlabel_yMaximum.TabIndex = 3;
            this.mlabel_yMaximum.Text = "Maximum";
            // 
            // mlabel_yMinimum
            // 
            this.mlabel_yMinimum.Location = new System.Drawing.Point(11, 19);
            this.mlabel_yMinimum.Name = "mlabel_yMinimum";
            this.mlabel_yMinimum.Size = new System.Drawing.Size(51, 45);
            this.mlabel_yMinimum.TabIndex = 2;
            this.mlabel_yMinimum.Text = "Minimum ";
            // 
            // formLinearPointFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(670, 299);
            this.Controls.Add(this.mgroupBox_filtersY);
            this.Controls.Add(this.mgroupBox_filtersX);
            this.Controls.Add(this.mbutton_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formLinearPointFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Linear Point Filter";
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_xMinimum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_xMaximum)).EndInit();
            this.mgroupBox_filtersX.ResumeLayout(false);
            this.mgroupBox_filtersX.PerformLayout();
            this.mgroupBox_filtersY.ResumeLayout(false);
            this.mgroupBox_filtersY.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_yMinimum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mtrackbar_yMaximum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar mtrackbar_xMinimum;
        private System.Windows.Forms.TrackBar mtrackbar_xMaximum;
        private System.Windows.Forms.Label mlabel_xMinimum;
        private System.Windows.Forms.Label mlabel_xMaximum;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.GroupBox mgroupBox_filtersX;
        private System.Windows.Forms.GroupBox mgroupBox_filtersY;
        private System.Windows.Forms.TrackBar mtrackbar_yMinimum;
        private System.Windows.Forms.TrackBar mtrackbar_yMaximum;
        private System.Windows.Forms.Label mlabel_yMaximum;
        private System.Windows.Forms.Label mlabel_yMinimum;
    }
}