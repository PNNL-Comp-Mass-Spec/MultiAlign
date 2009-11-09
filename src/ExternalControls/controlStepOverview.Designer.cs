namespace ExternalControls
{
    partial class controlStepOverview
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
            this.mpanel_steps = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mpanel_steps
            // 
            this.mpanel_steps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpanel_steps.Location = new System.Drawing.Point(0, 0);
            this.mpanel_steps.Name = "mpanel_steps";
            this.mpanel_steps.Size = new System.Drawing.Size(609, 103);
            this.mpanel_steps.TabIndex = 12;
            // 
            // controlStepOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mpanel_steps);
            this.Name = "controlStepOverview";
            this.Size = new System.Drawing.Size(609, 103);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mpanel_steps;
    }
}
