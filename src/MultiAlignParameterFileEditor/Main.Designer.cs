namespace MultiAlignParameterFileEditor
{
    partial class Main
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
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.newButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.mainTabPage = new System.Windows.Forms.TabControl();
            this.startPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.newLabel = new System.Windows.Forms.Label();
            this.glyph = new System.Windows.Forms.PictureBox();
            this.statusBar.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.startPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.glyph)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 762);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1073, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 17);
            this.statusLabel.Text = "Ready.";
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(185, 18);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(186, 136);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(185, 202);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(186, 136);
            this.openButton.TabIndex = 2;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // mainTabPage
            // 
            this.mainTabPage.Controls.Add(this.startPage);
            this.mainTabPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainTabPage.Location = new System.Drawing.Point(0, 0);
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.SelectedIndex = 0;
            this.mainTabPage.Size = new System.Drawing.Size(1073, 762);
            this.mainTabPage.TabIndex = 3;
            // 
            // startPage
            // 
            this.startPage.Controls.Add(this.label1);
            this.startPage.Controls.Add(this.newLabel);
            this.startPage.Controls.Add(this.glyph);
            this.startPage.Controls.Add(this.newButton);
            this.startPage.Controls.Add(this.openButton);
            this.startPage.Location = new System.Drawing.Point(4, 29);
            this.startPage.Name = "startPage";
            this.startPage.Padding = new System.Windows.Forms.Padding(3);
            this.startPage.Size = new System.Drawing.Size(1065, 729);
            this.startPage.TabIndex = 0;
            this.startPage.Text = "Start";
            this.startPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(407, 202);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(366, 113);
            this.label1.TabIndex = 5;
            this.label1.Text = "Have an existing parameter file?  Click here to open an existing file.  ";
            // 
            // newLabel
            // 
            this.newLabel.Location = new System.Drawing.Point(407, 18);
            this.newLabel.Name = "newLabel";
            this.newLabel.Size = new System.Drawing.Size(366, 113);
            this.newLabel.TabIndex = 4;
            this.newLabel.Text = "New To MultiAlign?  Or need a fresh start? Click here to start a new parameter fi" +
    "le.";
            // 
            // glyph
            // 
            this.glyph.BackgroundImage = global::MultiAlignParameterFileEditor.Properties.Resources.ExampleSidebar;
            this.glyph.Dock = System.Windows.Forms.DockStyle.Left;
            this.glyph.Location = new System.Drawing.Point(3, 3);
            this.glyph.Name = "glyph";
            this.glyph.Size = new System.Drawing.Size(165, 723);
            this.glyph.TabIndex = 3;
            this.glyph.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 784);
            this.Controls.Add(this.mainTabPage);
            this.Controls.Add(this.statusBar);
            this.Name = "Main";
            this.Text = "MultiAlign Parameter File Editor";
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.mainTabPage.ResumeLayout(false);
            this.startPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.glyph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.TabControl mainTabPage;
        private System.Windows.Forms.TabPage startPage;
        private System.Windows.Forms.PictureBox glyph;
        private System.Windows.Forms.Label newLabel;
        private System.Windows.Forms.Label label1;
    }
}

