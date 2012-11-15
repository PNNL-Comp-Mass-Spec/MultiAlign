namespace MultiAlignParameterFileEditor
{
    partial class ParameterFileEditor
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
            this.parameterSelectPanel = new System.Windows.Forms.Panel();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.parameterEditor = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.optionDescription = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.informationPanel = new System.Windows.Forms.Panel();
            this.exportHTML = new System.Windows.Forms.Button();
            this.presetLabel = new System.Windows.Forms.Label();
            this.presetList = new System.Windows.Forms.ComboBox();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.editorPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.informationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // parameterSelectPanel
            // 
            this.parameterSelectPanel.AutoScroll = true;
            this.parameterSelectPanel.BackColor = System.Drawing.Color.White;
            this.parameterSelectPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.parameterSelectPanel.Location = new System.Drawing.Point(0, 0);
            this.parameterSelectPanel.Name = "parameterSelectPanel";
            this.parameterSelectPanel.Size = new System.Drawing.Size(239, 650);
            this.parameterSelectPanel.TabIndex = 0;
            // 
            // editorPanel
            // 
            this.editorPanel.Controls.Add(this.parameterEditor);
            this.editorPanel.Controls.Add(this.panel1);
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(239, 0);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(573, 650);
            this.editorPanel.TabIndex = 1;
            // 
            // parameterEditor
            // 
            this.parameterEditor.BackColor = System.Drawing.Color.White;
            this.parameterEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterEditor.Location = new System.Drawing.Point(0, 51);
            this.parameterEditor.Name = "parameterEditor";
            this.parameterEditor.Size = new System.Drawing.Size(573, 599);
            this.parameterEditor.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.optionDescription);
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 51);
            this.panel1.TabIndex = 3;
            // 
            // optionDescription
            // 
            this.optionDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionDescription.Location = new System.Drawing.Point(0, 0);
            this.optionDescription.Name = "optionDescription";
            this.optionDescription.Size = new System.Drawing.Size(552, 51);
            this.optionDescription.TabIndex = 3;
            this.optionDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // closeButton
            // 
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Location = new System.Drawing.Point(552, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(21, 51);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "X";
            this.closeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // informationPanel
            // 
            this.informationPanel.BackColor = System.Drawing.Color.White;
            this.informationPanel.Controls.Add(this.exportHTML);
            this.informationPanel.Controls.Add(this.presetLabel);
            this.informationPanel.Controls.Add(this.presetList);
            this.informationPanel.Controls.Add(this.saveAsButton);
            this.informationPanel.Controls.Add(this.saveButton);
            this.informationPanel.Controls.Add(this.pathLabel);
            this.informationPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.informationPanel.Location = new System.Drawing.Point(0, 650);
            this.informationPanel.Name = "informationPanel";
            this.informationPanel.Size = new System.Drawing.Size(812, 64);
            this.informationPanel.TabIndex = 4;
            // 
            // exportHTML
            // 
            this.exportHTML.Location = new System.Drawing.Point(14, 6);
            this.exportHTML.Name = "exportHTML";
            this.exportHTML.Size = new System.Drawing.Size(161, 27);
            this.exportHTML.TabIndex = 7;
            this.exportHTML.Text = "Export Parameters to HTML";
            this.exportHTML.UseVisualStyleBackColor = true;
            this.exportHTML.Click += new System.EventHandler(this.exportHTML_Click);
            // 
            // presetLabel
            // 
            this.presetLabel.AutoSize = true;
            this.presetLabel.Location = new System.Drawing.Point(11, 42);
            this.presetLabel.Name = "presetLabel";
            this.presetLabel.Size = new System.Drawing.Size(94, 13);
            this.presetLabel.TabIndex = 6;
            this.presetLabel.Text = "Instrument Presets";
            this.presetLabel.Visible = false;
            // 
            // presetList
            // 
            this.presetList.Enabled = false;
            this.presetList.FormattingEnabled = true;
            this.presetList.Location = new System.Drawing.Point(111, 39);
            this.presetList.Name = "presetList";
            this.presetList.Size = new System.Drawing.Size(223, 21);
            this.presetList.TabIndex = 5;
            this.presetList.Visible = false;
            // 
            // saveAsButton
            // 
            this.saveAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveAsButton.Location = new System.Drawing.Point(748, 12);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(61, 27);
            this.saveAsButton.TabIndex = 4;
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(676, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(66, 27);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(9, 42);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(0, 13);
            this.pathLabel.TabIndex = 1;
            // 
            // ParameterFileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.parameterSelectPanel);
            this.Controls.Add(this.informationPanel);
            this.Name = "ParameterFileEditor";
            this.Size = new System.Drawing.Size(812, 714);
            this.editorPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.informationPanel.ResumeLayout(false);
            this.informationPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel parameterSelectPanel;
        private System.Windows.Forms.Panel editorPanel;
        private System.Windows.Forms.PropertyGrid parameterEditor;
        private System.Windows.Forms.Panel informationPanel;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label presetLabel;
        private System.Windows.Forms.ComboBox presetList;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label optionDescription;
        private System.Windows.Forms.Button exportHTML;
    }
}
