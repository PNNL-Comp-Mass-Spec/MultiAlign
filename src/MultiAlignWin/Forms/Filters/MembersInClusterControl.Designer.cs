namespace MultiAlignWin.Forms.Filters
{
    partial class MembersInClusterControl
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
            this.mnum_minMemberCount = new System.Windows.Forms.NumericUpDown();
            this.labelMaxMemberCount = new System.Windows.Forms.Label();
            this.mnum_maxCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minMemberCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maxCount)).BeginInit();
            this.SuspendLayout();
            // 
            // mnum_minMemberCount
            // 
            this.mnum_minMemberCount.Location = new System.Drawing.Point(6, 8);
            this.mnum_minMemberCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.mnum_minMemberCount.Name = "mnum_minMemberCount";
            this.mnum_minMemberCount.Size = new System.Drawing.Size(82, 20);
            this.mnum_minMemberCount.TabIndex = 0;
            this.mnum_minMemberCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelMaxMemberCount
            // 
            this.labelMaxMemberCount.AutoSize = true;
            this.labelMaxMemberCount.Location = new System.Drawing.Point(91, 12);
            this.labelMaxMemberCount.Name = "labelMaxMemberCount";
            this.labelMaxMemberCount.Size = new System.Drawing.Size(80, 13);
            this.labelMaxMemberCount.TabIndex = 3;
            this.labelMaxMemberCount.Text = "<= Members <=";
            // 
            // mnum_maxCount
            // 
            this.mnum_maxCount.Location = new System.Drawing.Point(177, 8);
            this.mnum_maxCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.mnum_maxCount.Name = "mnum_maxCount";
            this.mnum_maxCount.Size = new System.Drawing.Size(82, 20);
            this.mnum_maxCount.TabIndex = 2;
            this.mnum_maxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_maxCount.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // MembersInClusterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelMaxMemberCount);
            this.Controls.Add(this.mnum_maxCount);
            this.Controls.Add(this.mnum_minMemberCount);
            this.Name = "MembersInClusterControl";
            this.Size = new System.Drawing.Size(269, 46);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minMemberCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maxCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown mnum_minMemberCount;
        private System.Windows.Forms.Label labelMaxMemberCount;
        private System.Windows.Forms.NumericUpDown mnum_maxCount;
    }
}
