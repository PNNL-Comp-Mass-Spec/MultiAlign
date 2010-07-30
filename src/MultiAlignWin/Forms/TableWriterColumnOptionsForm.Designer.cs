namespace MultiAlignWin.Forms
{
    partial class TableWriterColumnOptionsForm
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
            this.featureGroupBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.msFeatureCountCheckbox = new System.Windows.Forms.CheckBox();
            this.cmcCheckbox = new System.Windows.Forms.CheckBox();
            this.abundanceMaxCheckbox = new System.Windows.Forms.CheckBox();
            this.abundanceSumCheckbox = new System.Windows.Forms.CheckBox();
            this.driftTimeCheckbox = new System.Windows.Forms.CheckBox();
            this.umcIndexCheckbox = new System.Windows.Forms.CheckBox();
            this.massCalibratedCheckbox = new System.Windows.Forms.CheckBox();
            this.scanCheckbox = new System.Windows.Forms.CheckBox();
            this.netAlignedCheckbox = new System.Windows.Forms.CheckBox();
            this.massCheckbox = new System.Windows.Forms.CheckBox();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_setToDefaultsButton = new System.Windows.Forms.Button();
            this.chargeStateCheckbox = new System.Windows.Forms.CheckBox();
            this.featureGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // featureGroupBox
            // 
            this.featureGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.featureGroupBox.Controls.Add(this.chargeStateCheckbox);
            this.featureGroupBox.Controls.Add(this.m_okButton);
            this.featureGroupBox.Controls.Add(this.m_setToDefaultsButton);
            this.featureGroupBox.Controls.Add(this.label5);
            this.featureGroupBox.Controls.Add(this.label4);
            this.featureGroupBox.Controls.Add(this.label3);
            this.featureGroupBox.Controls.Add(this.label2);
            this.featureGroupBox.Controls.Add(this.label1);
            this.featureGroupBox.Controls.Add(this.msFeatureCountCheckbox);
            this.featureGroupBox.Controls.Add(this.cmcCheckbox);
            this.featureGroupBox.Controls.Add(this.abundanceMaxCheckbox);
            this.featureGroupBox.Controls.Add(this.abundanceSumCheckbox);
            this.featureGroupBox.Controls.Add(this.driftTimeCheckbox);
            this.featureGroupBox.Controls.Add(this.umcIndexCheckbox);
            this.featureGroupBox.Controls.Add(this.massCalibratedCheckbox);
            this.featureGroupBox.Controls.Add(this.scanCheckbox);
            this.featureGroupBox.Controls.Add(this.netAlignedCheckbox);
            this.featureGroupBox.Controls.Add(this.massCheckbox);
            this.featureGroupBox.Location = new System.Drawing.Point(12, 12);
            this.featureGroupBox.Name = "featureGroupBox";
            this.featureGroupBox.Size = new System.Drawing.Size(449, 326);
            this.featureGroupBox.TabIndex = 0;
            this.featureGroupBox.TabStop = false;
            this.featureGroupBox.Text = "Feature Columns (Per Dataset)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(306, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Indices and Counts";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(306, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Ion Mobility";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(143, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Liquid Chromatography";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Abundances";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "MS";
            // 
            // msFeatureCountCheckbox
            // 
            this.msFeatureCountCheckbox.AutoSize = true;
            this.msFeatureCountCheckbox.Location = new System.Drawing.Point(309, 182);
            this.msFeatureCountCheckbox.Name = "msFeatureCountCheckbox";
            this.msFeatureCountCheckbox.Size = new System.Drawing.Size(112, 17);
            this.msFeatureCountCheckbox.TabIndex = 9;
            this.msFeatureCountCheckbox.Text = "MS Feature Count";
            this.msFeatureCountCheckbox.UseVisualStyleBackColor = true;
            // 
            // cmcCheckbox
            // 
            this.cmcCheckbox.AutoSize = true;
            this.cmcCheckbox.Location = new System.Drawing.Point(13, 205);
            this.cmcCheckbox.Name = "cmcCheckbox";
            this.cmcCheckbox.Size = new System.Drawing.Size(263, 17);
            this.cmcCheckbox.TabIndex = 8;
            this.cmcCheckbox.Text = "Summed Abundances at each charge state (CMC)";
            this.cmcCheckbox.UseVisualStyleBackColor = true;
            // 
            // abundanceMaxCheckbox
            // 
            this.abundanceMaxCheckbox.AutoSize = true;
            this.abundanceMaxCheckbox.Location = new System.Drawing.Point(13, 182);
            this.abundanceMaxCheckbox.Name = "abundanceMaxCheckbox";
            this.abundanceMaxCheckbox.Size = new System.Drawing.Size(104, 17);
            this.abundanceMaxCheckbox.TabIndex = 7;
            this.abundanceMaxCheckbox.Text = "Abundance Max";
            this.abundanceMaxCheckbox.UseVisualStyleBackColor = true;
            // 
            // abundanceSumCheckbox
            // 
            this.abundanceSumCheckbox.AutoSize = true;
            this.abundanceSumCheckbox.Location = new System.Drawing.Point(13, 159);
            this.abundanceSumCheckbox.Name = "abundanceSumCheckbox";
            this.abundanceSumCheckbox.Size = new System.Drawing.Size(105, 17);
            this.abundanceSumCheckbox.TabIndex = 6;
            this.abundanceSumCheckbox.Text = "Abundance Sum";
            this.abundanceSumCheckbox.UseVisualStyleBackColor = true;
            // 
            // driftTimeCheckbox
            // 
            this.driftTimeCheckbox.AutoSize = true;
            this.driftTimeCheckbox.Location = new System.Drawing.Point(307, 42);
            this.driftTimeCheckbox.Name = "driftTimeCheckbox";
            this.driftTimeCheckbox.Size = new System.Drawing.Size(71, 17);
            this.driftTimeCheckbox.TabIndex = 5;
            this.driftTimeCheckbox.Text = "Drift Time";
            this.driftTimeCheckbox.UseVisualStyleBackColor = true;
            // 
            // umcIndexCheckbox
            // 
            this.umcIndexCheckbox.AutoSize = true;
            this.umcIndexCheckbox.Location = new System.Drawing.Point(309, 159);
            this.umcIndexCheckbox.Name = "umcIndexCheckbox";
            this.umcIndexCheckbox.Size = new System.Drawing.Size(79, 17);
            this.umcIndexCheckbox.TabIndex = 4;
            this.umcIndexCheckbox.Text = "UMC Index";
            this.umcIndexCheckbox.UseVisualStyleBackColor = true;
            // 
            // massCalibratedCheckbox
            // 
            this.massCalibratedCheckbox.AutoSize = true;
            this.massCalibratedCheckbox.Location = new System.Drawing.Point(13, 65);
            this.massCalibratedCheckbox.Name = "massCalibratedCheckbox";
            this.massCalibratedCheckbox.Size = new System.Drawing.Size(101, 17);
            this.massCalibratedCheckbox.TabIndex = 3;
            this.massCalibratedCheckbox.Text = "Mass Calibrated";
            this.massCalibratedCheckbox.UseVisualStyleBackColor = true;
            // 
            // scanCheckbox
            // 
            this.scanCheckbox.AutoSize = true;
            this.scanCheckbox.Location = new System.Drawing.Point(146, 42);
            this.scanCheckbox.Name = "scanCheckbox";
            this.scanCheckbox.Size = new System.Drawing.Size(54, 17);
            this.scanCheckbox.TabIndex = 2;
            this.scanCheckbox.Text = "Scan ";
            this.scanCheckbox.UseVisualStyleBackColor = true;
            // 
            // netAlignedCheckbox
            // 
            this.netAlignedCheckbox.AutoSize = true;
            this.netAlignedCheckbox.Location = new System.Drawing.Point(146, 65);
            this.netAlignedCheckbox.Name = "netAlignedCheckbox";
            this.netAlignedCheckbox.Size = new System.Drawing.Size(86, 17);
            this.netAlignedCheckbox.TabIndex = 1;
            this.netAlignedCheckbox.Text = "NET Aligned";
            this.netAlignedCheckbox.UseVisualStyleBackColor = true;
            // 
            // massCheckbox
            // 
            this.massCheckbox.AutoSize = true;
            this.massCheckbox.Location = new System.Drawing.Point(13, 42);
            this.massCheckbox.Name = "massCheckbox";
            this.massCheckbox.Size = new System.Drawing.Size(54, 17);
            this.massCheckbox.TabIndex = 0;
            this.massCheckbox.Text = "Mass ";
            this.massCheckbox.UseVisualStyleBackColor = true;
            // 
            // m_okButton
            // 
            this.m_okButton.Location = new System.Drawing.Point(362, 280);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(81, 30);
            this.m_okButton.TabIndex = 1;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            this.m_okButton.Click += new System.EventHandler(this.m_okButton_Click);
            // 
            // m_setToDefaultsButton
            // 
            this.m_setToDefaultsButton.Location = new System.Drawing.Point(6, 280);
            this.m_setToDefaultsButton.Name = "m_setToDefaultsButton";
            this.m_setToDefaultsButton.Size = new System.Drawing.Size(118, 30);
            this.m_setToDefaultsButton.TabIndex = 2;
            this.m_setToDefaultsButton.Text = "Set To Defaults";
            this.m_setToDefaultsButton.UseVisualStyleBackColor = true;
            this.m_setToDefaultsButton.Click += new System.EventHandler(this.m_setToDefaultsButton_Click);
            // 
            // chargeStateCheckbox
            // 
            this.chargeStateCheckbox.AutoSize = true;
            this.chargeStateCheckbox.Location = new System.Drawing.Point(13, 88);
            this.chargeStateCheckbox.Name = "chargeStateCheckbox";
            this.chargeStateCheckbox.Size = new System.Drawing.Size(88, 17);
            this.chargeStateCheckbox.TabIndex = 15;
            this.chargeStateCheckbox.Text = "Charge State";
            this.chargeStateCheckbox.UseVisualStyleBackColor = true;
            // 
            // TableWriterColumnOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 350);
            this.Controls.Add(this.featureGroupBox);
            this.Name = "TableWriterColumnOptionsForm";
            this.Text = "Data Column Selection";
            this.featureGroupBox.ResumeLayout(false);
            this.featureGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox featureGroupBox;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.Button m_setToDefaultsButton;
        private System.Windows.Forms.CheckBox massCheckbox;
        private System.Windows.Forms.CheckBox cmcCheckbox;
        private System.Windows.Forms.CheckBox abundanceMaxCheckbox;
        private System.Windows.Forms.CheckBox abundanceSumCheckbox;
        private System.Windows.Forms.CheckBox driftTimeCheckbox;
        private System.Windows.Forms.CheckBox umcIndexCheckbox;
        private System.Windows.Forms.CheckBox massCalibratedCheckbox;
        private System.Windows.Forms.CheckBox scanCheckbox;
        private System.Windows.Forms.CheckBox netAlignedCheckbox;
        private System.Windows.Forms.CheckBox msFeatureCountCheckbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chargeStateCheckbox;
    }
}