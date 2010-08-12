namespace MultiAlignWin.Forms
{
    partial class DMSDatasetSearchOptionsForm
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
            this.m_datasetNameTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.m_instrumentNameTextbox = new System.Windows.Forms.TextBox();
            this.m_okButton = new System.Windows.Forms.Button();
            this.m_cancelButton = new System.Windows.Forms.Button();
            this.m_toolGroupbox = new System.Windows.Forms.GroupBox();
            this.m_decon2lsVsCheckbox = new System.Windows.Forms.CheckBox();
            this.m_ltqftpekCheckBox = new System.Windows.Forms.CheckBox();
            this.m_decon2lsAgilentCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.m_lcmsFeatureFinderCheckBox = new System.Windows.Forms.CheckBox();
            this.m_icr2lsCheckBox = new System.Windows.Forms.CheckBox();
            this.m_decon2lsCheckBox = new System.Windows.Forms.CheckBox();
            this.m_datetime = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.m_fileExtensionTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.m_parameterFileTextbox = new System.Windows.Forms.TextBox();
            this.m_toolGroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_datasetNameTextbox
            // 
            this.m_datasetNameTextbox.Location = new System.Drawing.Point(167, 19);
            this.m_datasetNameTextbox.Name = "m_datasetNameTextbox";
            this.m_datasetNameTextbox.Size = new System.Drawing.Size(277, 20);
            this.m_datasetNameTextbox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dataset Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Instrument Name contains";
            // 
            // m_instrumentNameTextbox
            // 
            this.m_instrumentNameTextbox.Location = new System.Drawing.Point(167, 97);
            this.m_instrumentNameTextbox.Name = "m_instrumentNameTextbox";
            this.m_instrumentNameTextbox.Size = new System.Drawing.Size(277, 20);
            this.m_instrumentNameTextbox.TabIndex = 2;
            // 
            // m_okButton
            // 
            this.m_okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_okButton.Location = new System.Drawing.Point(289, 360);
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.Size = new System.Drawing.Size(74, 32);
            this.m_okButton.TabIndex = 4;
            this.m_okButton.Text = "OK";
            this.m_okButton.UseVisualStyleBackColor = true;
            this.m_okButton.Click += new System.EventHandler(this.m_okButton_Click);
            // 
            // m_cancelButton
            // 
            this.m_cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_cancelButton.Location = new System.Drawing.Point(369, 360);
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.Size = new System.Drawing.Size(74, 32);
            this.m_cancelButton.TabIndex = 5;
            this.m_cancelButton.Text = "Cancel";
            this.m_cancelButton.UseVisualStyleBackColor = true;
            this.m_cancelButton.Click += new System.EventHandler(this.m_cancelButton_Click);
            // 
            // m_toolGroupbox
            // 
            this.m_toolGroupbox.Controls.Add(this.m_ltqftpekCheckBox);
            this.m_toolGroupbox.Controls.Add(this.m_decon2lsAgilentCheckBox);
            this.m_toolGroupbox.Controls.Add(this.label6);
            this.m_toolGroupbox.Controls.Add(this.label5);
            this.m_toolGroupbox.Controls.Add(this.m_lcmsFeatureFinderCheckBox);
            this.m_toolGroupbox.Controls.Add(this.m_icr2lsCheckBox);
            this.m_toolGroupbox.Controls.Add(this.m_decon2lsCheckBox);
            this.m_toolGroupbox.Controls.Add(this.m_decon2lsVsCheckbox);
            this.m_toolGroupbox.Location = new System.Drawing.Point(15, 149);
            this.m_toolGroupbox.Name = "m_toolGroupbox";
            this.m_toolGroupbox.Size = new System.Drawing.Size(429, 205);
            this.m_toolGroupbox.TabIndex = 6;
            this.m_toolGroupbox.TabStop = false;
            this.m_toolGroupbox.Text = "Tools";
            // 
            // m_decon2lsVsCheckbox
            // 
            this.m_decon2lsVsCheckbox.Location = new System.Drawing.Point(16, 70);
            this.m_decon2lsVsCheckbox.Name = "m_decon2lsVsCheckbox";
            this.m_decon2lsVsCheckbox.Size = new System.Drawing.Size(93, 22);
            this.m_decon2lsVsCheckbox.TabIndex = 10;
            this.m_decon2lsVsCheckbox.Text = "Decon2ls_v2";
            this.m_decon2lsVsCheckbox.UseVisualStyleBackColor = true;
            // 
            // m_ltqftpekCheckBox
            // 
            this.m_ltqftpekCheckBox.AutoSize = true;
            this.m_ltqftpekCheckBox.Location = new System.Drawing.Point(295, 50);
            this.m_ltqftpekCheckBox.Name = "m_ltqftpekCheckBox";
            this.m_ltqftpekCheckBox.Size = new System.Drawing.Size(85, 17);
            this.m_ltqftpekCheckBox.TabIndex = 9;
            this.m_ltqftpekCheckBox.Text = "LTQ_FTPek";
            this.m_ltqftpekCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_decon2lsAgilentCheckBox
            // 
            this.m_decon2lsAgilentCheckBox.AutoSize = true;
            this.m_decon2lsAgilentCheckBox.Location = new System.Drawing.Point(115, 50);
            this.m_decon2lsAgilentCheckBox.Name = "m_decon2lsAgilentCheckBox";
            this.m_decon2lsAgilentCheckBox.Size = new System.Drawing.Size(103, 17);
            this.m_decon2lsAgilentCheckBox.TabIndex = 7;
            this.m_decon2lsAgilentCheckBox.Text = "Decon2lsAgilent";
            this.m_decon2lsAgilentCheckBox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(13, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Feature Finding (LC-MS)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(13, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(163, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Peak Picking  (Deisotoping - MS)";
            // 
            // m_lcmsFeatureFinderCheckBox
            // 
            this.m_lcmsFeatureFinderCheckBox.Location = new System.Drawing.Point(16, 130);
            this.m_lcmsFeatureFinderCheckBox.Name = "m_lcmsFeatureFinderCheckBox";
            this.m_lcmsFeatureFinderCheckBox.Size = new System.Drawing.Size(216, 24);
            this.m_lcmsFeatureFinderCheckBox.TabIndex = 2;
            this.m_lcmsFeatureFinderCheckBox.Text = "LCMS Feature Finder ";
            this.m_lcmsFeatureFinderCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_icr2lsCheckBox
            // 
            this.m_icr2lsCheckBox.AutoSize = true;
            this.m_icr2lsCheckBox.Location = new System.Drawing.Point(224, 50);
            this.m_icr2lsCheckBox.Name = "m_icr2lsCheckBox";
            this.m_icr2lsCheckBox.Size = new System.Drawing.Size(57, 17);
            this.m_icr2lsCheckBox.TabIndex = 1;
            this.m_icr2lsCheckBox.Text = "ICR2ls";
            this.m_icr2lsCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_decon2lsCheckBox
            // 
            this.m_decon2lsCheckBox.AutoSize = true;
            this.m_decon2lsCheckBox.Location = new System.Drawing.Point(16, 50);
            this.m_decon2lsCheckBox.Name = "m_decon2lsCheckBox";
            this.m_decon2lsCheckBox.Size = new System.Drawing.Size(71, 17);
            this.m_decon2lsCheckBox.TabIndex = 0;
            this.m_decon2lsCheckBox.Text = "Decon2ls";
            this.m_decon2lsCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_datetime
            // 
            this.m_datetime.CustomFormat = "MM, dd, yyyy ";
            this.m_datetime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.m_datetime.Location = new System.Drawing.Point(167, 123);
            this.m_datetime.Name = "m_datetime";
            this.m_datetime.Size = new System.Drawing.Size(277, 20);
            this.m_datetime.TabIndex = 9;
            this.m_datetime.Value = new System.DateTime(2010, 8, 11, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Acquired After";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(50, 360);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 32);
            this.button1.TabIndex = 11;
            this.button1.Text = "Defaults";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // m_fileExtensionTextbox
            // 
            this.m_fileExtensionTextbox.Location = new System.Drawing.Point(167, 43);
            this.m_fileExtensionTextbox.Name = "m_fileExtensionTextbox";
            this.m_fileExtensionTextbox.Size = new System.Drawing.Size(278, 20);
            this.m_fileExtensionTextbox.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "File Extension";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(148, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Parameter File Name contains";
            // 
            // m_parameterFileTextbox
            // 
            this.m_parameterFileTextbox.Location = new System.Drawing.Point(167, 69);
            this.m_parameterFileTextbox.Name = "m_parameterFileTextbox";
            this.m_parameterFileTextbox.Size = new System.Drawing.Size(277, 20);
            this.m_parameterFileTextbox.TabIndex = 14;
            // 
            // DMSDatasetSearchOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(457, 404);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.m_parameterFileTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_fileExtensionTextbox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_datetime);
            this.Controls.Add(this.m_toolGroupbox);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_instrumentNameTextbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_datasetNameTextbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DMSDatasetSearchOptionsForm";
            this.Text = "Search Options";
            this.m_toolGroupbox.ResumeLayout(false);
            this.m_toolGroupbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_datasetNameTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox m_instrumentNameTextbox;
        private System.Windows.Forms.Button m_okButton;
        private System.Windows.Forms.Button m_cancelButton;
        private System.Windows.Forms.GroupBox m_toolGroupbox;
        private System.Windows.Forms.DateTimePicker m_datetime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox m_lcmsFeatureFinderCheckBox;
        private System.Windows.Forms.CheckBox m_icr2lsCheckBox;
        private System.Windows.Forms.CheckBox m_decon2lsCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox m_ltqftpekCheckBox;
        private System.Windows.Forms.CheckBox m_decon2lsAgilentCheckBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox m_decon2lsVsCheckbox;
        private System.Windows.Forms.TextBox m_fileExtensionTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox m_parameterFileTextbox;
    }
}