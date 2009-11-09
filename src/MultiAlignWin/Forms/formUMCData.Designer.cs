namespace MultiAlignWin.Forms
{
    partial class formUMCData
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
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mcontrol_umcData = new MultiAlignWin.Forms.controlUMCData();
            this.SuspendLayout();
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.Location = new System.Drawing.Point(225, 236);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(64, 24);
            this.mbutton_ok.TabIndex = 1;
            this.mbutton_ok.Text = "Ok";
            this.mbutton_ok.UseVisualStyleBackColor = true;
            this.mbutton_ok.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // mcontrol_umcData
            // 
            this.mcontrol_umcData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcontrol_umcData.Location = new System.Drawing.Point(2, 0);
            this.mcontrol_umcData.Name = "mcontrol_umcData";
            this.mcontrol_umcData.Size = new System.Drawing.Size(287, 230);
            this.mcontrol_umcData.TabIndex = 0;
            // 
            // formUMCData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.mbutton_ok);
            this.Controls.Add(this.mcontrol_umcData);
            this.Name = "formUMCData";
            this.Text = "UMC Data";
            this.ResumeLayout(false);

        }

        #endregion

        private controlUMCData mcontrol_umcData;
        private System.Windows.Forms.Button mbutton_ok;
    }
}