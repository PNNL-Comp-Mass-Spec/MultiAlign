using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmPeakMatchingParameters
	/// </summary>
	public class frmPeakMatchingParameters : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelMassTol;
        private System.Windows.Forms.Label labelNETTol;
		private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button mbtnDefaults;
        private NumericUpDown mnum_MassTol;
        private NumericUpDown mnum_NETTol;
        private NumericUpDown mnum_driftTimeTolerance;
        private Label mlabel_driftTimeTolerance;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPeakMatchingParameters()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPeakMatchingParameters));
            this.labelMassTol = new System.Windows.Forms.Label();
            this.labelNETTol = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.mbtnDefaults = new System.Windows.Forms.Button();
            this.mnum_MassTol = new System.Windows.Forms.NumericUpDown();
            this.mnum_NETTol = new System.Windows.Forms.NumericUpDown();
            this.mnum_driftTimeTolerance = new System.Windows.Forms.NumericUpDown();
            this.mlabel_driftTimeTolerance = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_MassTol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_NETTol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_driftTimeTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMassTol
            // 
            this.labelMassTol.Location = new System.Drawing.Point(3, 22);
            this.labelMassTol.Name = "labelMassTol";
            this.labelMassTol.Size = new System.Drawing.Size(114, 23);
            this.labelMassTol.TabIndex = 0;
            this.labelMassTol.Text = "Mass Tolerance (ppm)";
            // 
            // labelNETTol
            // 
            this.labelNETTol.Location = new System.Drawing.Point(3, 45);
            this.labelNETTol.Name = "labelNETTol";
            this.labelNETTol.Size = new System.Drawing.Size(102, 23);
            this.labelNETTol.TabIndex = 1;
            this.labelNETTol.Text = "NET Tolerance (%)";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(142, 119);
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
            this.buttonCancel.Location = new System.Drawing.Point(227, 118);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // mbtnDefaults
            // 
            this.mbtnDefaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mbtnDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnDefaults.Location = new System.Drawing.Point(49, 118);
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
            this.mnum_MassTol.DecimalPlaces = 4;
            this.mnum_MassTol.Location = new System.Drawing.Point(123, 22);
            this.mnum_MassTol.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.mnum_MassTol.Name = "mnum_MassTol";
            this.mnum_MassTol.Size = new System.Drawing.Size(217, 20);
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
            this.mnum_NETTol.DecimalPlaces = 4;
            this.mnum_NETTol.Location = new System.Drawing.Point(123, 47);
            this.mnum_NETTol.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.mnum_NETTol.Name = "mnum_NETTol";
            this.mnum_NETTol.Size = new System.Drawing.Size(217, 20);
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
            this.mnum_driftTimeTolerance.DecimalPlaces = 4;
            this.mnum_driftTimeTolerance.Location = new System.Drawing.Point(123, 75);
            this.mnum_driftTimeTolerance.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.mnum_driftTimeTolerance.Name = "mnum_driftTimeTolerance";
            this.mnum_driftTimeTolerance.Size = new System.Drawing.Size(217, 20);
            this.mnum_driftTimeTolerance.TabIndex = 12;
            this.mnum_driftTimeTolerance.Value = new decimal(new int[] {
            3,
            0,
            0,
            131072});
            // 
            // mlabel_driftTimeTolerance
            // 
            this.mlabel_driftTimeTolerance.Location = new System.Drawing.Point(3, 77);
            this.mlabel_driftTimeTolerance.Name = "mlabel_driftTimeTolerance";
            this.mlabel_driftTimeTolerance.Size = new System.Drawing.Size(88, 23);
            this.mlabel_driftTimeTolerance.TabIndex = 11;
            this.mlabel_driftTimeTolerance.Text = "Drift Time (ms)";
            // 
            // frmPeakMatchingParameters
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(352, 153);
            this.Controls.Add(this.mnum_driftTimeTolerance);
            this.Controls.Add(this.mlabel_driftTimeTolerance);
            this.Controls.Add(this.mnum_NETTol);
            this.Controls.Add(this.mnum_MassTol);
            this.Controls.Add(this.mbtnDefaults);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelNETTol);
            this.Controls.Add(this.labelMassTol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPeakMatchingParameters";
            this.Text = "Peak Matching Parameters";
            ((System.ComponentModel.ISupportInitialize)(this.mnum_MassTol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_NETTol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnum_driftTimeTolerance)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion


        /// <summary>
        /// Saves the options used in this analysis.
        /// </summary>
        private void SaveOptions()
        {
            Properties.Settings.Default.UserPeakMatchingMassTolerance       = MassTolerance;
            Properties.Settings.Default.UserPeakMatchingNETTolerance        = NETTolerance;
            Properties.Settings.Default.UserPeakMatchingDriftTimeTolerance  = DriftTimeTolerance;
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
			MultiAlignEngine.PeakMatching.clsPeakMatchingOptions defaults = new MultiAlignEngine.PeakMatching.clsPeakMatchingOptions() ;
			PeakMatchingOptions = defaults ;
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
        /// <summary>
        /// Gets or sets the peak matching parameters.
        /// </summary>
		public MultiAlignEngine.PeakMatching.clsPeakMatchingOptions PeakMatchingOptions
		{
			get
			{
                MultiAlignEngine.PeakMatching.clsPeakMatchingOptions options = new MultiAlignEngine.PeakMatching.clsPeakMatchingOptions();
				options.MassTolerance       = MassTolerance ;
				options.NETTolerance        = NETTolerance ;				
                options.DriftTimeTolerance  = DriftTimeTolerance;
				return options ;
			}
			set
			{
				MultiAlignEngine.PeakMatching.clsPeakMatchingOptions options = value ;
				MassTolerance       = options.MassTolerance ;
				NETTolerance        = options.NETTolerance ;
				DriftTimeTolerance  = options.DriftTimeTolerance;
			}
		}
	}
}
