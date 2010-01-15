namespace MultiAlignWin.Forms.Parameters
{
    partial class formPeptideIDScoring
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
            this.mbutton_defaults = new System.Windows.Forms.Button();
            this.mbutton_cancel = new System.Windows.Forms.Button();
            this.mbutton_ok = new System.Windows.Forms.Button();
            this.mgroupBox_smart = new System.Windows.Forms.GroupBox();
            this.mlabel_minimumMSMSObservations = new System.Windows.Forms.Label();
            this.mnum_minimumMSMSObservations = new System.Windows.Forms.NumericUpDown();
            this.mcheckBox_usePriorProbabilities = new System.Windows.Forms.CheckBox();
            this.mnum_pairedMass = new System.Windows.Forms.NumericUpDown();
            this.mlabel_netTolerance = new System.Windows.Forms.Label();
            this.mnum_netTolerance = new System.Windows.Forms.NumericUpDown();
            this.mlabel_massTolerancePPM = new System.Windows.Forms.Label();
            this.mnum_massTolerance = new System.Windows.Forms.NumericUpDown();
            this.mlabel_minPeptideProphetProbability = new System.Windows.Forms.Label();
            this.mnum_minimumPeptideProphetProbability = new System.Windows.Forms.NumericUpDown();
            this.mlabel_pairedMass = new System.Windows.Forms.Label();
            this.mcheckBox_isDataPaired = new System.Windows.Forms.CheckBox();
            this.mgroupBox_smart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumMSMSObservations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pairedMass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_netTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_massTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumPeptideProphetProbability)).BeginInit();
            this.SuspendLayout();
            // 
            // mbutton_defaults
            // 
            this.mbutton_defaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_defaults.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_defaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_defaults.Location = new System.Drawing.Point(35, 243);
            this.mbutton_defaults.Name = "mbutton_defaults";
            this.mbutton_defaults.Size = new System.Drawing.Size(96, 24);
            this.mbutton_defaults.TabIndex = 11;
            this.mbutton_defaults.Text = "Use Defaults";
            this.mbutton_defaults.UseVisualStyleBackColor = false;
            this.mbutton_defaults.Click += new System.EventHandler(this.mbutton_defaults_Click);
            // 
            // mbutton_cancel
            // 
            this.mbutton_cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_cancel.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mbutton_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_cancel.Location = new System.Drawing.Point(243, 243);
            this.mbutton_cancel.Name = "mbutton_cancel";
            this.mbutton_cancel.Size = new System.Drawing.Size(91, 23);
            this.mbutton_cancel.TabIndex = 10;
            this.mbutton_cancel.Text = "Cancel";
            this.mbutton_cancel.UseVisualStyleBackColor = false;
            // 
            // mbutton_ok
            // 
            this.mbutton_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbutton_ok.BackColor = System.Drawing.SystemColors.Control;
            this.mbutton_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mbutton_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbutton_ok.Location = new System.Drawing.Point(139, 243);
            this.mbutton_ok.Name = "mbutton_ok";
            this.mbutton_ok.Size = new System.Drawing.Size(91, 23);
            this.mbutton_ok.TabIndex = 9;
            this.mbutton_ok.Text = "OK";
            this.mbutton_ok.UseVisualStyleBackColor = false;
            // 
            // mgroupBox_smart
            // 
            this.mgroupBox_smart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mgroupBox_smart.Controls.Add(this.mlabel_minimumMSMSObservations);
            this.mgroupBox_smart.Controls.Add(this.mnum_minimumMSMSObservations);
            this.mgroupBox_smart.Controls.Add(this.mcheckBox_usePriorProbabilities);
            this.mgroupBox_smart.Controls.Add(this.mcheckBox_isDataPaired);
            this.mgroupBox_smart.Controls.Add(this.mlabel_pairedMass);
            this.mgroupBox_smart.Controls.Add(this.mnum_pairedMass);
            this.mgroupBox_smart.Controls.Add(this.mlabel_netTolerance);
            this.mgroupBox_smart.Controls.Add(this.mnum_netTolerance);
            this.mgroupBox_smart.Controls.Add(this.mlabel_massTolerancePPM);
            this.mgroupBox_smart.Controls.Add(this.mnum_massTolerance);
            this.mgroupBox_smart.Controls.Add(this.mlabel_minPeptideProphetProbability);
            this.mgroupBox_smart.Controls.Add(this.mnum_minimumPeptideProphetProbability);
            this.mgroupBox_smart.Location = new System.Drawing.Point(12, 12);
            this.mgroupBox_smart.Name = "mgroupBox_smart";
            this.mgroupBox_smart.Size = new System.Drawing.Size(344, 225);
            this.mgroupBox_smart.TabIndex = 12;
            this.mgroupBox_smart.TabStop = false;
            this.mgroupBox_smart.Text = "SMART Parameters";
            // 
            // mlabel_minimumMSMSObservations
            // 
            this.mlabel_minimumMSMSObservations.AutoSize = true;
            this.mlabel_minimumMSMSObservations.Location = new System.Drawing.Point(17, 111);
            this.mlabel_minimumMSMSObservations.Name = "mlabel_minimumMSMSObservations";
            this.mlabel_minimumMSMSObservations.Size = new System.Drawing.Size(148, 13);
            this.mlabel_minimumMSMSObservations.TabIndex = 20;
            this.mlabel_minimumMSMSObservations.Text = "Minimum MSMS Observations";
            // 
            // mnum_minimumMSMSObservations
            // 
            this.mnum_minimumMSMSObservations.Location = new System.Drawing.Point(202, 111);
            this.mnum_minimumMSMSObservations.Name = "mnum_minimumMSMSObservations";
            this.mnum_minimumMSMSObservations.Size = new System.Drawing.Size(131, 20);
            this.mnum_minimumMSMSObservations.TabIndex = 16;
            this.mnum_minimumMSMSObservations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mcheckBox_usePriorProbabilities
            // 
            this.mcheckBox_usePriorProbabilities.AutoSize = true;
            this.mcheckBox_usePriorProbabilities.Enabled = false;
            this.mcheckBox_usePriorProbabilities.Location = new System.Drawing.Point(20, 146);
            this.mcheckBox_usePriorProbabilities.Name = "mcheckBox_usePriorProbabilities";
            this.mcheckBox_usePriorProbabilities.Size = new System.Drawing.Size(128, 17);
            this.mcheckBox_usePriorProbabilities.TabIndex = 13;
            this.mcheckBox_usePriorProbabilities.Text = "Use Prior Probabilities";
            this.mcheckBox_usePriorProbabilities.UseVisualStyleBackColor = true;
            // 
            // mnum_pairedMass
            // 
            this.mnum_pairedMass.DecimalPlaces = 4;
            this.mnum_pairedMass.Location = new System.Drawing.Point(202, 189);
            this.mnum_pairedMass.Name = "mnum_pairedMass";
            this.mnum_pairedMass.Size = new System.Drawing.Size(131, 20);
            this.mnum_pairedMass.TabIndex = 8;
            this.mnum_pairedMass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mnum_pairedMass.Visible = false;
            // 
            // mlabel_netTolerance
            // 
            this.mlabel_netTolerance.AutoSize = true;
            this.mlabel_netTolerance.Location = new System.Drawing.Point(17, 84);
            this.mlabel_netTolerance.Name = "mlabel_netTolerance";
            this.mlabel_netTolerance.Size = new System.Drawing.Size(80, 13);
            this.mlabel_netTolerance.TabIndex = 5;
            this.mlabel_netTolerance.Text = "NET Tolerance";
            // 
            // mnum_netTolerance
            // 
            this.mnum_netTolerance.DecimalPlaces = 4;
            this.mnum_netTolerance.Location = new System.Drawing.Point(202, 84);
            this.mnum_netTolerance.Name = "mnum_netTolerance";
            this.mnum_netTolerance.Size = new System.Drawing.Size(131, 20);
            this.mnum_netTolerance.TabIndex = 4;
            this.mnum_netTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mlabel_massTolerancePPM
            // 
            this.mlabel_massTolerancePPM.AutoSize = true;
            this.mlabel_massTolerancePPM.Location = new System.Drawing.Point(17, 58);
            this.mlabel_massTolerancePPM.Name = "mlabel_massTolerancePPM";
            this.mlabel_massTolerancePPM.Size = new System.Drawing.Size(115, 13);
            this.mlabel_massTolerancePPM.TabIndex = 3;
            this.mlabel_massTolerancePPM.Text = "Mass Tolerance (PPM)";
            // 
            // mnum_massTolerance
            // 
            this.mnum_massTolerance.DecimalPlaces = 4;
            this.mnum_massTolerance.Location = new System.Drawing.Point(202, 58);
            this.mnum_massTolerance.Name = "mnum_massTolerance";
            this.mnum_massTolerance.Size = new System.Drawing.Size(131, 20);
            this.mnum_massTolerance.TabIndex = 2;
            this.mnum_massTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mlabel_minPeptideProphetProbability
            // 
            this.mlabel_minPeptideProphetProbability.AutoSize = true;
            this.mlabel_minPeptideProphetProbability.Location = new System.Drawing.Point(17, 32);
            this.mlabel_minPeptideProphetProbability.Name = "mlabel_minPeptideProphetProbability";
            this.mlabel_minPeptideProphetProbability.Size = new System.Drawing.Size(178, 13);
            this.mlabel_minPeptideProphetProbability.TabIndex = 1;
            this.mlabel_minPeptideProphetProbability.Text = "Minimum Peptide Prophet Probability";
            // 
            // mnum_minimumPeptideProphetProbability
            // 
            this.mnum_minimumPeptideProphetProbability.DecimalPlaces = 4;
            this.mnum_minimumPeptideProphetProbability.Location = new System.Drawing.Point(202, 32);
            this.mnum_minimumPeptideProphetProbability.Name = "mnum_minimumPeptideProphetProbability";
            this.mnum_minimumPeptideProphetProbability.Size = new System.Drawing.Size(131, 20);
            this.mnum_minimumPeptideProphetProbability.TabIndex = 0;
            this.mnum_minimumPeptideProphetProbability.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mlabel_pairedMass
            // 
            this.mlabel_pairedMass.AutoSize = true;
            this.mlabel_pairedMass.Location = new System.Drawing.Point(48, 189);
            this.mlabel_pairedMass.Name = "mlabel_pairedMass";
            this.mlabel_pairedMass.Size = new System.Drawing.Size(65, 13);
            this.mlabel_pairedMass.TabIndex = 9;
            this.mlabel_pairedMass.Text = "Paired Mass";
            this.mlabel_pairedMass.Visible = false;
            // 
            // mcheckBox_isDataPaired
            // 
            this.mcheckBox_isDataPaired.AutoSize = true;
            this.mcheckBox_isDataPaired.Location = new System.Drawing.Point(20, 169);
            this.mcheckBox_isDataPaired.Name = "mcheckBox_isDataPaired";
            this.mcheckBox_isDataPaired.Size = new System.Drawing.Size(93, 17);
            this.mcheckBox_isDataPaired.TabIndex = 12;
            this.mcheckBox_isDataPaired.Text = "Is Data Paired";
            this.mcheckBox_isDataPaired.UseVisualStyleBackColor = true;
            this.mcheckBox_isDataPaired.Visible = false;
            // 
            // formPeptideIDScoring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 276);
            this.Controls.Add(this.mgroupBox_smart);
            this.Controls.Add(this.mbutton_defaults);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mbutton_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formPeptideIDScoring";
            this.Text = "SMART Options";
            this.mgroupBox_smart.ResumeLayout(false);
            this.mgroupBox_smart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumMSMSObservations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pairedMass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_netTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_massTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumPeptideProphetProbability)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbutton_defaults;
        private System.Windows.Forms.Button mbutton_cancel;
        private System.Windows.Forms.Button mbutton_ok;
        private System.Windows.Forms.GroupBox mgroupBox_smart;
        private System.Windows.Forms.CheckBox mcheckBox_usePriorProbabilities;
        private System.Windows.Forms.NumericUpDown mnum_pairedMass;
        private System.Windows.Forms.Label mlabel_netTolerance;
        private System.Windows.Forms.NumericUpDown mnum_netTolerance;
        private System.Windows.Forms.Label mlabel_massTolerancePPM;
        private System.Windows.Forms.NumericUpDown mnum_massTolerance;
        private System.Windows.Forms.Label mlabel_minPeptideProphetProbability;
        private System.Windows.Forms.NumericUpDown mnum_minimumPeptideProphetProbability;
        private System.Windows.Forms.NumericUpDown mnum_minimumMSMSObservations;
        private System.Windows.Forms.Label mlabel_minimumMSMSObservations;
        private System.Windows.Forms.CheckBox mcheckBox_isDataPaired;
        private System.Windows.Forms.Label mlabel_pairedMass;
    }
}