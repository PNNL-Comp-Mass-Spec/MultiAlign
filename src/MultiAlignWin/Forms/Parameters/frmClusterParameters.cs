using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmClusterParameters.
	/// </summary>
	public class frmClusterParameters : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelMassTol;
        private System.Windows.Forms.Label labelNETTol;
		private System.Windows.Forms.RadioButton radioButtonMaxData;
		private System.Windows.Forms.RadioButton radioButtonSumData;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxIntensity;
		private System.Windows.Forms.GroupBox groupBoxClassRep;
		private System.Windows.Forms.RadioButton radioButtonMedian;
		private System.Windows.Forms.RadioButton radioButtonMean;
		private System.Windows.Forms.Button mbtnDefaults;
        private NumericUpDown mnum_MassTol;
        private NumericUpDown mnum_NETTol;
        private NumericUpDown mnum_driftTimeTolerance;
        private Label mlabel_driftTimeTolerance;
        private CheckBox mcheckBox_ignoreChargeStates;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmClusterParameters()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClusterParameters));
            this.labelMassTol = new System.Windows.Forms.Label();
            this.labelNETTol = new System.Windows.Forms.Label();
            this.groupBoxIntensity = new System.Windows.Forms.GroupBox();
            this.radioButtonSumData = new System.Windows.Forms.RadioButton();
            this.radioButtonMaxData = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxClassRep = new System.Windows.Forms.GroupBox();
            this.radioButtonMedian = new System.Windows.Forms.RadioButton();
            this.radioButtonMean = new System.Windows.Forms.RadioButton();
            this.mbtnDefaults = new System.Windows.Forms.Button();
            this.mnum_MassTol = new System.Windows.Forms.NumericUpDown();
            this.mnum_NETTol = new System.Windows.Forms.NumericUpDown();
            this.mnum_driftTimeTolerance = new System.Windows.Forms.NumericUpDown();
            this.mlabel_driftTimeTolerance = new System.Windows.Forms.Label();
            this.mcheckBox_ignoreChargeStates = new System.Windows.Forms.CheckBox();
            this.groupBoxIntensity.SuspendLayout();
            this.groupBoxClassRep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_MassTol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_NETTol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_driftTimeTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMassTol
            // 
            this.labelMassTol.Location = new System.Drawing.Point(25, 24);
            this.labelMassTol.Name = "labelMassTol";
            this.labelMassTol.Size = new System.Drawing.Size(96, 23);
            this.labelMassTol.TabIndex = 0;
            this.labelMassTol.Text = "Mass Tolerance :";
            // 
            // labelNETTol
            // 
            this.labelNETTol.Location = new System.Drawing.Point(25, 47);
            this.labelNETTol.Name = "labelNETTol";
            this.labelNETTol.Size = new System.Drawing.Size(88, 23);
            this.labelNETTol.TabIndex = 1;
            this.labelNETTol.Text = "NET Tolerance :";
            // 
            // groupBoxIntensity
            // 
            this.groupBoxIntensity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxIntensity.Controls.Add(this.radioButtonSumData);
            this.groupBoxIntensity.Controls.Add(this.radioButtonMaxData);
            this.groupBoxIntensity.Location = new System.Drawing.Point(12, 146);
            this.groupBoxIntensity.Name = "groupBoxIntensity";
            this.groupBoxIntensity.Size = new System.Drawing.Size(327, 96);
            this.groupBoxIntensity.TabIndex = 4;
            this.groupBoxIntensity.TabStop = false;
            this.groupBoxIntensity.Text = "Dataset Intensity";
            // 
            // radioButtonSumData
            // 
            this.radioButtonSumData.Location = new System.Drawing.Point(24, 56);
            this.radioButtonSumData.Name = "radioButtonSumData";
            this.radioButtonSumData.Size = new System.Drawing.Size(104, 24);
            this.radioButtonSumData.TabIndex = 6;
            this.radioButtonSumData.Text = "Sum in Dataset";
            // 
            // radioButtonMaxData
            // 
            this.radioButtonMaxData.Checked = true;
            this.radioButtonMaxData.Location = new System.Drawing.Point(24, 24);
            this.radioButtonMaxData.Name = "radioButtonMaxData";
            this.radioButtonMaxData.Size = new System.Drawing.Size(136, 24);
            this.radioButtonMaxData.TabIndex = 5;
            this.radioButtonMaxData.TabStop = true;
            this.radioButtonMaxData.Text = "Maximum in Dataset";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(141, 386);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(226, 385);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // groupBoxClassRep
            // 
            this.groupBoxClassRep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxClassRep.Controls.Add(this.radioButtonMedian);
            this.groupBoxClassRep.Controls.Add(this.radioButtonMean);
            this.groupBoxClassRep.Location = new System.Drawing.Point(12, 248);
            this.groupBoxClassRep.Name = "groupBoxClassRep";
            this.groupBoxClassRep.Size = new System.Drawing.Size(327, 96);
            this.groupBoxClassRep.TabIndex = 7;
            this.groupBoxClassRep.TabStop = false;
            this.groupBoxClassRep.Text = "Cluster Representation";
            // 
            // radioButtonMedian
            // 
            this.radioButtonMedian.Checked = true;
            this.radioButtonMedian.Location = new System.Drawing.Point(24, 56);
            this.radioButtonMedian.Name = "radioButtonMedian";
            this.radioButtonMedian.Size = new System.Drawing.Size(104, 24);
            this.radioButtonMedian.TabIndex = 6;
            this.radioButtonMedian.TabStop = true;
            this.radioButtonMedian.Text = "Median";
            // 
            // radioButtonMean
            // 
            this.radioButtonMean.Location = new System.Drawing.Point(24, 24);
            this.radioButtonMean.Name = "radioButtonMean";
            this.radioButtonMean.Size = new System.Drawing.Size(136, 24);
            this.radioButtonMean.TabIndex = 5;
            this.radioButtonMean.Text = "Mean";
            // 
            // mbtnDefaults
            // 
            this.mbtnDefaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbtnDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnDefaults.Location = new System.Drawing.Point(48, 385);
            this.mbtnDefaults.Name = "mbtnDefaults";
            this.mbtnDefaults.Size = new System.Drawing.Size(80, 24);
            this.mbtnDefaults.TabIndex = 8;
            this.mbtnDefaults.Text = "Use Defaults";
            this.mbtnDefaults.UseVisualStyleBackColor = false;
            this.mbtnDefaults.Click += new System.EventHandler(this.mbtnDefaults_Click);
            // 
            // mnum_MassTol
            // 
            this.mnum_MassTol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_MassTol.DecimalPlaces = 2;
            this.mnum_MassTol.Location = new System.Drawing.Point(123, 22);
            this.mnum_MassTol.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.mnum_MassTol.Name = "mnum_MassTol";
            this.mnum_MassTol.Size = new System.Drawing.Size(216, 20);
            this.mnum_MassTol.TabIndex = 9;
            this.mnum_MassTol.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // mnum_NETTol
            // 
            this.mnum_NETTol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_NETTol.DecimalPlaces = 2;
            this.mnum_NETTol.Location = new System.Drawing.Point(123, 47);
            this.mnum_NETTol.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.mnum_NETTol.Name = "mnum_NETTol";
            this.mnum_NETTol.Size = new System.Drawing.Size(216, 20);
            this.mnum_NETTol.TabIndex = 10;
            this.mnum_NETTol.Value = new decimal(new int[] {
            3,
            0,
            0,
            131072});
            // 
            // mnum_driftTimeTolerance
            // 
            this.mnum_driftTimeTolerance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mnum_driftTimeTolerance.DecimalPlaces = 2;
            this.mnum_driftTimeTolerance.Location = new System.Drawing.Point(123, 75);
            this.mnum_driftTimeTolerance.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.mnum_driftTimeTolerance.Name = "mnum_driftTimeTolerance";
            this.mnum_driftTimeTolerance.Size = new System.Drawing.Size(216, 20);
            this.mnum_driftTimeTolerance.TabIndex = 12;
            this.mnum_driftTimeTolerance.Value = new decimal(new int[] {
            3,
            0,
            0,
            131072});
            // 
            // mlabel_driftTimeTolerance
            // 
            this.mlabel_driftTimeTolerance.Location = new System.Drawing.Point(25, 77);
            this.mlabel_driftTimeTolerance.Name = "mlabel_driftTimeTolerance";
            this.mlabel_driftTimeTolerance.Size = new System.Drawing.Size(88, 23);
            this.mlabel_driftTimeTolerance.TabIndex = 11;
            this.mlabel_driftTimeTolerance.Text = "Drift Time";
            // 
            // mcheckBox_ignoreChargeStates
            // 
            this.mcheckBox_ignoreChargeStates.AutoSize = true;
            this.mcheckBox_ignoreChargeStates.Checked = true;
            this.mcheckBox_ignoreChargeStates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mcheckBox_ignoreChargeStates.Location = new System.Drawing.Point(12, 103);
            this.mcheckBox_ignoreChargeStates.Name = "mcheckBox_ignoreChargeStates";
            this.mcheckBox_ignoreChargeStates.Size = new System.Drawing.Size(333, 17);
            this.mcheckBox_ignoreChargeStates.TabIndex = 13;
            this.mcheckBox_ignoreChargeStates.Text = "Ignore Charge States (allows clusters with different charge states)";
            this.mcheckBox_ignoreChargeStates.UseVisualStyleBackColor = true;
            // 
            // frmClusterParameters
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(351, 420);
            this.Controls.Add(this.mcheckBox_ignoreChargeStates);
            this.Controls.Add(this.mnum_driftTimeTolerance);
            this.Controls.Add(this.mlabel_driftTimeTolerance);
            this.Controls.Add(this.mnum_NETTol);
            this.Controls.Add(this.mnum_MassTol);
            this.Controls.Add(this.mbtnDefaults);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxIntensity);
            this.Controls.Add(this.labelNETTol);
            this.Controls.Add(this.labelMassTol);
            this.Controls.Add(this.groupBoxClassRep);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmClusterParameters";
            this.Text = "LC-MS Feature Clustering";
            this.groupBoxIntensity.ResumeLayout(false);
            this.groupBoxClassRep.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mnum_MassTol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_NETTol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_driftTimeTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


        /// <summary>
        /// Saves the options used in this analysis.
        /// </summary>
        private void SaveOptions()
        {
            Properties.Settings.Default.UserClusterOptionsMassTolerance         = MassTolerance;
            Properties.Settings.Default.UserClusterOptionsNETTolerance          = NETTolerance;
            Properties.Settings.Default.UserClusterOptionsUseMaxInDataset       = radioButtonMaxData.Checked;
            Properties.Settings.Default.UserClusterOptionsUseMeanRepresentation = (ClusterRepresentativeType == MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEAN);
            Properties.Settings.Default.UserClusterOptionsDriftTimeTolerance    = DriftTimeTolerance;
            Properties.Settings.Default.UserClusterOptionsIgnoreChargeStates    = IgnoreChargeStates;
            Properties.Settings.Default.Save();
        }

		private void mbutton_ok_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK ;
            SaveOptions();

			this.Hide() ; 
		}

		private void mbutton_cancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel ; 
			this.Hide() ; 		
		}

		private void mbtnDefaults_Click(object sender, System.EventArgs e)
		{
			MultiAlignEngine.Clustering.clsClusterOptions defaults = new MultiAlignEngine.Clustering.clsClusterOptions() ;
			ClusterOptions = defaults ;
		}

        /// <summary>
        /// Gets or sets the mass tolerance.
        /// </summary>
		public double MassTolerance
		{
			get
			{
				return (Convert.ToDouble(mnum_MassTol.Value)) ;
			}
			set
			{
				mnum_MassTol.Value = Convert.ToDecimal(value) ;
			}
		}
        /// <summary>
        /// Gets or sets whether to ignore charge states durign clustering.
        /// </summary>
        public bool IgnoreChargeStates
        {
            get
            {
                return mcheckBox_ignoreChargeStates.Checked;
            }
            set
            {
                mcheckBox_ignoreChargeStates.Checked = value;
            }            
        }
        /// <summary>
        /// Gets or sets the NET tolerance.
        /// </summary>
		public double NETTolerance
		{
			get
			{
				return (Convert.ToDouble(mnum_NETTol.Value)); 
			}
			set
			{
				mnum_NETTol.Value = Convert.ToDecimal(value) ;
			}
		}
        /// <summary>
        /// Gets or sets the drift time tolerance.
        /// </summary>
        public double DriftTimeTolerance
        {
            get
            {
                return (Convert.ToDouble(mnum_driftTimeTolerance.Value));
            }
            set
            {
                mnum_driftTimeTolerance.Value = Convert.ToDecimal(value);
            }
        }

		public MultiAlignEngine.Clustering.enmClusterIntensityType ClusterIntensityType
		{
			get
			{
				if (radioButtonMaxData.Checked)
					return MultiAlignEngine.Clustering.enmClusterIntensityType.MAX_PER_DATASET ;
				else
					return MultiAlignEngine.Clustering.enmClusterIntensityType.SUM_PER_DATASET ;

			}
			set
			{
				if (value == MultiAlignEngine.Clustering.enmClusterIntensityType.MAX_PER_DATASET)
					radioButtonMaxData.Checked = true ;
				else
					radioButtonSumData.Checked = true ;
			}
		}

		public MultiAlignEngine.Clustering.enmClusterRepresentativeType ClusterRepresentativeType
		{
			get
			{
				if (radioButtonMean.Checked)
					return MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEAN ;
				else
					return MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEDIAN ;
			}
			set
			{
				if (value == MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEAN)
					radioButtonMean.Checked = true ;
				else
					radioButtonMedian.Checked = true ;
			}
		}
        /// <summary>
        /// Gets or sets the clustering options.
        /// </summary>
		public MultiAlignEngine.Clustering.clsClusterOptions ClusterOptions
		{
			get
			{
				MultiAlignEngine.Clustering.clsClusterOptions options = new MultiAlignEngine.Clustering.clsClusterOptions() ;
				options.MassTolerance = MassTolerance ;
				options.NETTolerance = NETTolerance ;
				options.ClusterIntensityType = ClusterIntensityType ;
				options.ClusterRepresentativeType = ClusterRepresentativeType ;
                options.DriftTimeTolerance = DriftTimeTolerance;
                options.IgnoreCharge = IgnoreChargeStates;
				return options ;
			}
			set
			{
				MultiAlignEngine.Clustering.clsClusterOptions options = value ;
				MassTolerance = options.MassTolerance ;
				NETTolerance = options.NETTolerance ;
				ClusterIntensityType = options.ClusterIntensityType ;
				ClusterRepresentativeType = options.ClusterRepresentativeType ;
                DriftTimeTolerance = options.DriftTimeTolerance;
                IgnoreChargeStates = options.IgnoreCharge;
			}
		}
	}
}
