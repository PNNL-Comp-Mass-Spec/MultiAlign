namespace MultiAlignWin.Forms
{
    partial class controlUMCData
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mlistview_properties = new System.Windows.Forms.ListView();
            this.mlabel_stats = new System.Windows.Forms.Label();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // mlistview_properties
            // 
            this.mlistview_properties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mlistview_properties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.mlistview_properties.GridLines = true;
            this.mlistview_properties.Location = new System.Drawing.Point(3, 31);
            this.mlistview_properties.Name = "mlistview_properties";
            this.mlistview_properties.Size = new System.Drawing.Size(629, 344);
            this.mlistview_properties.TabIndex = 0;
            this.mlistview_properties.UseCompatibleStateImageBehavior = false;
            this.mlistview_properties.View = System.Windows.Forms.View.Details;
            // 
            // mlabel_stats
            // 
            this.mlabel_stats.AutoSize = true;
            this.mlabel_stats.Location = new System.Drawing.Point(9, 8);
            this.mlabel_stats.Name = "mlabel_stats";
            this.mlabel_stats.Size = new System.Drawing.Size(76, 13);
            this.mlabel_stats.TabIndex = 1;
            this.mlabel_stats.Text = "UMC Statistics";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "UMC Property";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            // 
            // controlUMCData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mlabel_stats);
            this.Controls.Add(this.mlistview_properties);
            this.Name = "controlUMCData";
            this.Size = new System.Drawing.Size(635, 378);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView mlistview_properties;
        private System.Windows.Forms.Label mlabel_stats;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
