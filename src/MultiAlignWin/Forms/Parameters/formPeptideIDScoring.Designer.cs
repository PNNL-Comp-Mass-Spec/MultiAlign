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
            this.mlabel_modificationAA = new System.Windows.Forms.Label();
            this.mlabel_minimumMSMSObservations = new System.Windows.Forms.Label();
            this.mlabel_minimumTrypticState = new System.Windows.Forms.Label();
            this.mlabel_maximumModifications = new System.Windows.Forms.Label();
            this.mtextBox_modificationChar = new System.Windows.Forms.TextBox();
            this.mnum_minimumMSMSObservations = new System.Windows.Forms.NumericUpDown();
            this.mnum_minimumTrypticState = new System.Windows.Forms.NumericUpDown();
            this.mnum_maxModifications = new System.Windows.Forms.NumericUpDown();
            this.mcheckBox_useFScores = new System.Windows.Forms.CheckBox();
            this.mcheckBox_isDataPaired = new System.Windows.Forms.CheckBox();
            this.mcheckBox_applyModification = new System.Windows.Forms.CheckBox();
            this.mcheckBox_applyDynamicModification = new System.Windows.Forms.CheckBox();
            this.mlabel_pairedMass = new System.Windows.Forms.Label();
            this.mnum_pairedMass = new System.Windows.Forms.NumericUpDown();
            this.mlabel_modifiedMass = new System.Windows.Forms.Label();
            this.mnum_modifiedMass = new System.Windows.Forms.NumericUpDown();
            this.mlabel_netTolerance = new System.Windows.Forms.Label();
            this.mnum_netTolerance = new System.Windows.Forms.NumericUpDown();
            this.mlabel_massTolerancePPM = new System.Windows.Forms.Label();
            this.mnum_massTolerance = new System.Windows.Forms.NumericUpDown();
            this.mlabel_minPeptideProphetProbability = new System.Windows.Forms.Label();
            this.mnum_minimumPeptideProphetProbability = new System.Windows.Forms.NumericUpDown();
            this.mgroupBox_smart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumMSMSObservations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumTrypticState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maxModifications)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pairedMass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_modifiedMass)).BeginInit();
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
            this.mbutton_defaults.Location = new System.Drawing.Point(32, 411);
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
            this.mbutton_cancel.Location = new System.Drawing.Point(240, 411);
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
            this.mbutton_ok.Location = new System.Drawing.Point(136, 411);
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
            this.mgroupBox_smart.Controls.Add(this.mlabel_modificationAA);
            this.mgroupBox_smart.Controls.Add(this.mlabel_minimumMSMSObservations);
            this.mgroupBox_smart.Controls.Add(this.mlabel_minimumTrypticState);
            this.mgroupBox_smart.Controls.Add(this.mlabel_maximumModifications);
            this.mgroupBox_smart.Controls.Add(this.mtextBox_modificationChar);
            this.mgroupBox_smart.Controls.Add(this.mnum_minimumMSMSObservations);
            this.mgroupBox_smart.Controls.Add(this.mnum_minimumTrypticState);
            this.mgroupBox_smart.Controls.Add(this.mnum_maxModifications);
            this.mgroupBox_smart.Controls.Add(this.mcheckBox_useFScores);
            this.mgroupBox_smart.Controls.Add(this.mcheckBox_isDataPaired);
            this.mgroupBox_smart.Controls.Add(this.mcheckBox_applyModification);
            this.mgroupBox_smart.Controls.Add(this.mcheckBox_applyDynamicModification);
            this.mgroupBox_smart.Controls.Add(this.mlabel_pairedMass);
            this.mgroupBox_smart.Controls.Add(this.mnum_pairedMass);
            this.mgroupBox_smart.Controls.Add(this.mlabel_modifiedMass);
            this.mgroupBox_smart.Controls.Add(this.mnum_modifiedMass);
            this.mgroupBox_smart.Controls.Add(this.mlabel_netTolerance);
            this.mgroupBox_smart.Controls.Add(this.mnum_netTolerance);
            this.mgroupBox_smart.Controls.Add(this.mlabel_massTolerancePPM);
            this.mgroupBox_smart.Controls.Add(this.mnum_massTolerance);
            this.mgroupBox_smart.Controls.Add(this.mlabel_minPeptideProphetProbability);
            this.mgroupBox_smart.Controls.Add(this.mnum_minimumPeptideProphetProbability);
            this.mgroupBox_smart.Location = new System.Drawing.Point(12, 8);
            this.mgroupBox_smart.Name = "mgroupBox_smart";
            this.mgroupBox_smart.Size = new System.Drawing.Size(339, 397);
            this.mgroupBox_smart.TabIndex = 12;
            this.mgroupBox_smart.TabStop = false;
            this.mgroupBox_smart.Text = "SMART Parameters";
            // 
            // mlabel_modificationAA
            // 
            this.mlabel_modificationAA.AutoSize = true;
            this.mlabel_modificationAA.Location = new System.Drawing.Point(17, 352);
            this.mlabel_modificationAA.Name = "mlabel_modificationAA";
            this.mlabel_modificationAA.Size = new System.Drawing.Size(81, 13);
            this.mlabel_modificationAA.TabIndex = 21;
            this.mlabel_modificationAA.Text = "Modification AA";
            // 
            // mlabel_minimumMSMSObservations
            // 
            this.mlabel_minimumMSMSObservations.AutoSize = true;
            this.mlabel_minimumMSMSObservations.Location = new System.Drawing.Point(17, 326);
            this.mlabel_minimumMSMSObservations.Name = "mlabel_minimumMSMSObservations";
            this.mlabel_minimumMSMSObservations.Size = new System.Drawing.Size(148, 13);
            this.mlabel_minimumMSMSObservations.TabIndex = 20;
            this.mlabel_minimumMSMSObservations.Text = "Minimum MSMS Observations";
            // 
            // mlabel_minimumTrypticState
            // 
            this.mlabel_minimumTrypticState.AutoSize = true;
            this.mlabel_minimumTrypticState.Location = new System.Drawing.Point(17, 300);
            this.mlabel_minimumTrypticState.Name = "mlabel_minimumTrypticState";
            this.mlabel_minimumTrypticState.Size = new System.Drawing.Size(111, 13);
            this.mlabel_minimumTrypticState.TabIndex = 19;
            this.mlabel_minimumTrypticState.Text = "Minimum Tryptic State";
            // 
            // mlabel_maximumModifications
            // 
            this.mlabel_maximumModifications.AutoSize = true;
            this.mlabel_maximumModifications.Location = new System.Drawing.Point(17, 274);
            this.mlabel_maximumModifications.Name = "mlabel_maximumModifications";
            this.mlabel_maximumModifications.Size = new System.Drawing.Size(116, 13);
            this.mlabel_maximumModifications.TabIndex = 18;
            this.mlabel_maximumModifications.Text = "Maximum Modifications";
            // 
            // mtextBox_modificationChar
            // 
            this.mtextBox_modificationChar.Location = new System.Drawing.Point(202, 349);
            this.mtextBox_modificationChar.MaxLength = 1;
            this.mtextBox_modificationChar.Name = "mtextBox_modificationChar";
            this.mtextBox_modificationChar.Size = new System.Drawing.Size(51, 20);
            this.mtextBox_modificationChar.TabIndex = 17;
            // 
            // mnum_minimumMSMSObservations
            // 
            this.mnum_minimumMSMSObservations.Location = new System.Drawing.Point(202, 324);
            this.mnum_minimumMSMSObservations.Name = "mnum_minimumMSMSObservations";
            this.mnum_minimumMSMSObservations.Size = new System.Drawing.Size(131, 20);
            this.mnum_minimumMSMSObservations.TabIndex = 16;
            // 
            // mnum_minimumTrypticState
            // 
            this.mnum_minimumTrypticState.Location = new System.Drawing.Point(202, 298);
            this.mnum_minimumTrypticState.Name = "mnum_minimumTrypticState";
            this.mnum_minimumTrypticState.Size = new System.Drawing.Size(131, 20);
            this.mnum_minimumTrypticState.TabIndex = 15;
            // 
            // mnum_maxModifications
            // 
            this.mnum_maxModifications.Location = new System.Drawing.Point(202, 272);
            this.mnum_maxModifications.Name = "mnum_maxModifications";
            this.mnum_maxModifications.Size = new System.Drawing.Size(131, 20);
            this.mnum_maxModifications.TabIndex = 14;
            // 
            // mcheckBox_useFScores
            // 
            this.mcheckBox_useFScores.AutoSize = true;
            this.mcheckBox_useFScores.Enabled = false;
            this.mcheckBox_useFScores.Location = new System.Drawing.Point(20, 241);
            this.mcheckBox_useFScores.Name = "mcheckBox_useFScores";
            this.mcheckBox_useFScores.Size = new System.Drawing.Size(205, 17);
            this.mcheckBox_useFScores.TabIndex = 13;
            this.mcheckBox_useFScores.Text = "Use F-Scores from SEQUEST Results";
            this.mcheckBox_useFScores.UseVisualStyleBackColor = true;
            // 
            // mcheckBox_isDataPaired
            // 
            this.mcheckBox_isDataPaired.AutoSize = true;
            this.mcheckBox_isDataPaired.Location = new System.Drawing.Point(20, 218);
            this.mcheckBox_isDataPaired.Name = "mcheckBox_isDataPaired";
            this.mcheckBox_isDataPaired.Size = new System.Drawing.Size(93, 17);
            this.mcheckBox_isDataPaired.TabIndex = 12;
            this.mcheckBox_isDataPaired.Text = "Is Data Paired";
            this.mcheckBox_isDataPaired.UseVisualStyleBackColor = true;
            // 
            // mcheckBox_applyModification
            // 
            this.mcheckBox_applyModification.AutoSize = true;
            this.mcheckBox_applyModification.Location = new System.Drawing.Point(20, 195);
            this.mcheckBox_applyModification.Name = "mcheckBox_applyModification";
            this.mcheckBox_applyModification.Size = new System.Drawing.Size(112, 17);
            this.mcheckBox_applyModification.TabIndex = 11;
            this.mcheckBox_applyModification.Text = "Apply Modification";
            this.mcheckBox_applyModification.UseVisualStyleBackColor = true;
            // 
            // mcheckBox_applyDynamicModification
            // 
            this.mcheckBox_applyDynamicModification.AutoSize = true;
            this.mcheckBox_applyDynamicModification.Location = new System.Drawing.Point(20, 172);
            this.mcheckBox_applyDynamicModification.Name = "mcheckBox_applyDynamicModification";
            this.mcheckBox_applyDynamicModification.Size = new System.Drawing.Size(156, 17);
            this.mcheckBox_applyDynamicModification.TabIndex = 10;
            this.mcheckBox_applyDynamicModification.Text = "Apply Dynamic Modification";
            this.mcheckBox_applyDynamicModification.UseVisualStyleBackColor = true;
            // 
            // mlabel_pairedMass
            // 
            this.mlabel_pairedMass.AutoSize = true;
            this.mlabel_pairedMass.Location = new System.Drawing.Point(17, 136);
            this.mlabel_pairedMass.Name = "mlabel_pairedMass";
            this.mlabel_pairedMass.Size = new System.Drawing.Size(65, 13);
            this.mlabel_pairedMass.TabIndex = 9;
            this.mlabel_pairedMass.Text = "Paired Mass";
            // 
            // mnum_pairedMass
            // 
            this.mnum_pairedMass.DecimalPlaces = 4;
            this.mnum_pairedMass.Location = new System.Drawing.Point(202, 136);
            this.mnum_pairedMass.Name = "mnum_pairedMass";
            this.mnum_pairedMass.Size = new System.Drawing.Size(131, 20);
            this.mnum_pairedMass.TabIndex = 8;
            // 
            // mlabel_modifiedMass
            // 
            this.mlabel_modifiedMass.AutoSize = true;
            this.mlabel_modifiedMass.Location = new System.Drawing.Point(17, 110);
            this.mlabel_modifiedMass.Name = "mlabel_modifiedMass";
            this.mlabel_modifiedMass.Size = new System.Drawing.Size(75, 13);
            this.mlabel_modifiedMass.TabIndex = 7;
            this.mlabel_modifiedMass.Text = "Modified Mass";
            // 
            // mnum_modifiedMass
            // 
            this.mnum_modifiedMass.DecimalPlaces = 4;
            this.mnum_modifiedMass.Location = new System.Drawing.Point(202, 110);
            this.mnum_modifiedMass.Name = "mnum_modifiedMass";
            this.mnum_modifiedMass.Size = new System.Drawing.Size(131, 20);
            this.mnum_modifiedMass.TabIndex = 6;
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
            // 
            // formPeptideIDScoring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 444);
            this.Controls.Add(this.mgroupBox_smart);
            this.Controls.Add(this.mbutton_defaults);
            this.Controls.Add(this.mbutton_cancel);
            this.Controls.Add(this.mbutton_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formPeptideIDScoring";
            this.Text = "Peptide ID Scoring Parameters";
            this.mgroupBox_smart.ResumeLayout(false);
            this.mgroupBox_smart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumMSMSObservations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_minimumTrypticState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_maxModifications)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_pairedMass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_modifiedMass)).EndInit();
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
        private System.Windows.Forms.CheckBox mcheckBox_useFScores;
        private System.Windows.Forms.CheckBox mcheckBox_isDataPaired;
        private System.Windows.Forms.CheckBox mcheckBox_applyModification;
        private System.Windows.Forms.CheckBox mcheckBox_applyDynamicModification;
        private System.Windows.Forms.Label mlabel_pairedMass;
        private System.Windows.Forms.NumericUpDown mnum_pairedMass;
        private System.Windows.Forms.Label mlabel_modifiedMass;
        private System.Windows.Forms.NumericUpDown mnum_modifiedMass;
        private System.Windows.Forms.Label mlabel_netTolerance;
        private System.Windows.Forms.NumericUpDown mnum_netTolerance;
        private System.Windows.Forms.Label mlabel_massTolerancePPM;
        private System.Windows.Forms.NumericUpDown mnum_massTolerance;
        private System.Windows.Forms.Label mlabel_minPeptideProphetProbability;
        private System.Windows.Forms.NumericUpDown mnum_minimumPeptideProphetProbability;
        private System.Windows.Forms.TextBox mtextBox_modificationChar;
        private System.Windows.Forms.NumericUpDown mnum_minimumMSMSObservations;
        private System.Windows.Forms.NumericUpDown mnum_minimumTrypticState;
        private System.Windows.Forms.NumericUpDown mnum_maxModifications;
        private System.Windows.Forms.Label mlabel_modificationAA;
        private System.Windows.Forms.Label mlabel_minimumMSMSObservations;
        private System.Windows.Forms.Label mlabel_minimumTrypticState;
        private System.Windows.Forms.Label mlabel_maximumModifications;
    }
}