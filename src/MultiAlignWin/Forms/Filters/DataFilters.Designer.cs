namespace MultiAlignWin.Forms.Filters
{
    partial class DataFilters
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
            this.filterContainer = new System.Windows.Forms.SplitContainer();
            this.filterDescriptionLabel = new System.Windows.Forms.Label();
            this.addFilterButton = new System.Windows.Forms.Button();
            this.filterComboBox = new System.Windows.Forms.ComboBox();
            this.removeFilterButton = new System.Windows.Forms.Button();
            this.filterListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.filterContainer.Panel1.SuspendLayout();
            this.filterContainer.Panel2.SuspendLayout();
            this.filterContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterContainer
            // 
            this.filterContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filterContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filterContainer.Location = new System.Drawing.Point(0, 0);
            this.filterContainer.Name = "filterContainer";
            // 
            // filterContainer.Panel1
            // 
            this.filterContainer.Panel1.Controls.Add(this.filterDescriptionLabel);
            this.filterContainer.Panel1.Controls.Add(this.addFilterButton);
            this.filterContainer.Panel1.Controls.Add(this.filterComboBox);
            this.filterContainer.Panel1.Controls.Add(this.removeFilterButton);
            this.filterContainer.Panel1.Controls.Add(this.filterListBox);
            // 
            // filterContainer.Panel2
            // 
            this.filterContainer.Panel2.AutoScroll = true;
            this.filterContainer.Panel2.Controls.Add(this.label1);
            this.filterContainer.Size = new System.Drawing.Size(555, 298);
            this.filterContainer.SplitterDistance = 304;
            this.filterContainer.TabIndex = 0;
            // 
            // filterDescriptionLabel
            // 
            this.filterDescriptionLabel.AutoSize = true;
            this.filterDescriptionLabel.Location = new System.Drawing.Point(3, 14);
            this.filterDescriptionLabel.Name = "filterDescriptionLabel";
            this.filterDescriptionLabel.Size = new System.Drawing.Size(34, 13);
            this.filterDescriptionLabel.TabIndex = 3;
            this.filterDescriptionLabel.Text = "Filters";
            // 
            // addFilterButton
            // 
            this.addFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addFilterButton.Location = new System.Drawing.Point(249, 30);
            this.addFilterButton.Name = "addFilterButton";
            this.addFilterButton.Size = new System.Drawing.Size(50, 27);
            this.addFilterButton.TabIndex = 2;
            this.addFilterButton.Text = "Add ";
            this.addFilterButton.UseVisualStyleBackColor = true;
            this.addFilterButton.Click += new System.EventHandler(this.addFilterButton_Click);
            // 
            // filterComboBox
            // 
            this.filterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filterComboBox.FormattingEnabled = true;
            this.filterComboBox.Location = new System.Drawing.Point(3, 34);
            this.filterComboBox.Name = "filterComboBox";
            this.filterComboBox.Size = new System.Drawing.Size(241, 21);
            this.filterComboBox.TabIndex = 1;
            // 
            // removeFilterButton
            // 
            this.removeFilterButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.removeFilterButton.Location = new System.Drawing.Point(75, 262);
            this.removeFilterButton.Name = "removeFilterButton";
            this.removeFilterButton.Size = new System.Drawing.Size(159, 27);
            this.removeFilterButton.TabIndex = 1;
            this.removeFilterButton.Text = "Remove Selected Filter";
            this.removeFilterButton.UseVisualStyleBackColor = true;
            this.removeFilterButton.Click += new System.EventHandler(this.removeFilterButton_Click);
            // 
            // filterListBox
            // 
            this.filterListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filterListBox.FormattingEnabled = true;
            this.filterListBox.Location = new System.Drawing.Point(3, 61);
            this.filterListBox.Name = "filterListBox";
            this.filterListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.filterListBox.Size = new System.Drawing.Size(296, 173);
            this.filterListBox.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(407, 304);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(65, 32);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(478, 304);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(65, 32);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 27);
            this.label1.TabIndex = 4;
            this.label1.Text = "Selected Filters";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DataFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 348);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.filterContainer);
            this.Name = "DataFilters";
            this.Text = "Data Filters";
            this.filterContainer.Panel1.ResumeLayout(false);
            this.filterContainer.Panel1.PerformLayout();
            this.filterContainer.Panel2.ResumeLayout(false);
            this.filterContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer filterContainer;
        private System.Windows.Forms.ListBox filterListBox;
        private System.Windows.Forms.Button removeFilterButton;
        private System.Windows.Forms.Button addFilterButton;
        private System.Windows.Forms.ComboBox filterComboBox;
        private System.Windows.Forms.Label filterDescriptionLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
    }
}