namespace PNNLControls.Drawing.Plotting
{
    partial class controlHeatMap
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
            this.mpanel_drawingArea = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mpanel_drawingArea
            // 
            this.mpanel_drawingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_drawingArea.Location = new System.Drawing.Point(0, 0);
            this.mpanel_drawingArea.Name = "mpanel_drawingArea";
            this.mpanel_drawingArea.Size = new System.Drawing.Size(100, 100);
            this.mpanel_drawingArea.TabIndex = 0;
            // 
            // controlHeatMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mpanel_drawingArea);
            this.Name = "controlHeatMap";
            this.Size = new System.Drawing.Size(100, 100);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mpanel_drawingArea;
    }
}
