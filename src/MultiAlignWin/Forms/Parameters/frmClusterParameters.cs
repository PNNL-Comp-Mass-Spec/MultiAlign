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
		private System.Windows.Forms.TextBox textBoxMassTol;
		private System.Windows.Forms.TextBox textBoxNETTol;
		private System.Windows.Forms.RadioButton radioButtonMaxData;
		private System.Windows.Forms.RadioButton radioButtonSumData;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxIntensity;
		private System.Windows.Forms.GroupBox groupBoxClassRep;
		private System.Windows.Forms.RadioButton radioButtonMedian;
		private System.Windows.Forms.RadioButton radioButtonMean;
		private System.Windows.Forms.Button mbtnDefaults;
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
            this.textBoxMassTol = new System.Windows.Forms.TextBox();
            this.textBoxNETTol = new System.Windows.Forms.TextBox();
            this.groupBoxIntensity = new System.Windows.Forms.GroupBox();
            this.radioButtonSumData = new System.Windows.Forms.RadioButton();
            this.radioButtonMaxData = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxClassRep = new System.Windows.Forms.GroupBox();
            this.radioButtonMedian = new System.Windows.Forms.RadioButton();
            this.radioButtonMean = new System.Windows.Forms.RadioButton();
            this.mbtnDefaults = new System.Windows.Forms.Button();
            this.groupBoxIntensity.SuspendLayout();
            this.groupBoxClassRep.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMassTol
            // 
            this.labelMassTol.Location = new System.Drawing.Point(24, 24);
            this.labelMassTol.Name = "labelMassTol";
            this.labelMassTol.Size = new System.Drawing.Size(100, 23);
            this.labelMassTol.TabIndex = 0;
            this.labelMassTol.Text = "Mass Tolerance :";
            // 
            // labelNETTol
            // 
            this.labelNETTol.Location = new System.Drawing.Point(240, 24);
            this.labelNETTol.Name = "labelNETTol";
            this.labelNETTol.Size = new System.Drawing.Size(88, 23);
            this.labelNETTol.TabIndex = 1;
            this.labelNETTol.Text = "NET Tolerance :";
            // 
            // textBoxMassTol
            // 
            this.textBoxMassTol.Location = new System.Drawing.Point(120, 24);
            this.textBoxMassTol.Name = "textBoxMassTol";
            this.textBoxMassTol.Size = new System.Drawing.Size(56, 20);
            this.textBoxMassTol.TabIndex = 2;
            this.textBoxMassTol.Text = "6";
            this.textBoxMassTol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxNETTol
            // 
            this.textBoxNETTol.Location = new System.Drawing.Point(328, 24);
            this.textBoxNETTol.Name = "textBoxNETTol";
            this.textBoxNETTol.Size = new System.Drawing.Size(56, 20);
            this.textBoxNETTol.TabIndex = 3;
            this.textBoxNETTol.Text = "0.03";
            this.textBoxNETTol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBoxIntensity
            // 
            this.groupBoxIntensity.Controls.Add(this.radioButtonSumData);
            this.groupBoxIntensity.Controls.Add(this.radioButtonMaxData);
            this.groupBoxIntensity.Location = new System.Drawing.Point(16, 56);
            this.groupBoxIntensity.Name = "groupBoxIntensity";
            this.groupBoxIntensity.Size = new System.Drawing.Size(184, 96);
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
            this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(176, 168);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.mbutton_ok_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(280, 168);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.mbutton_cancel_Click);
            // 
            // groupBoxClassRep
            // 
            this.groupBoxClassRep.Controls.Add(this.radioButtonMedian);
            this.groupBoxClassRep.Controls.Add(this.radioButtonMean);
            this.groupBoxClassRep.Location = new System.Drawing.Point(224, 56);
            this.groupBoxClassRep.Name = "groupBoxClassRep";
            this.groupBoxClassRep.Size = new System.Drawing.Size(184, 96);
            this.groupBoxClassRep.TabIndex = 7;
            this.groupBoxClassRep.TabStop = false;
            this.groupBoxClassRep.Text = "Cluster Representation";
            // 
            // radioButtonMedian
            // 
            this.radioButtonMedian.Location = new System.Drawing.Point(24, 56);
            this.radioButtonMedian.Name = "radioButtonMedian";
            this.radioButtonMedian.Size = new System.Drawing.Size(104, 24);
            this.radioButtonMedian.TabIndex = 6;
            this.radioButtonMedian.Text = "Median";
            // 
            // radioButtonMean
            // 
            this.radioButtonMean.Checked = true;
            this.radioButtonMean.Location = new System.Drawing.Point(24, 24);
            this.radioButtonMean.Name = "radioButtonMean";
            this.radioButtonMean.Size = new System.Drawing.Size(136, 24);
            this.radioButtonMean.TabIndex = 5;
            this.radioButtonMean.TabStop = true;
            this.radioButtonMean.Text = "Mean";
            // 
            // mbtnDefaults
            // 
            this.mbtnDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.mbtnDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mbtnDefaults.Location = new System.Drawing.Point(72, 168);
            this.mbtnDefaults.Name = "mbtnDefaults";
            this.mbtnDefaults.Size = new System.Drawing.Size(80, 24);
            this.mbtnDefaults.TabIndex = 8;
            this.mbtnDefaults.Text = "Use Defaults";
            this.mbtnDefaults.UseVisualStyleBackColor = false;
            this.mbtnDefaults.Click += new System.EventHandler(this.mbtnDefaults_Click);
            // 
            // frmClusterParameters
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(421, 206);
            this.Controls.Add(this.mbtnDefaults);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxIntensity);
            this.Controls.Add(this.textBoxNETTol);
            this.Controls.Add(this.textBoxMassTol);
            this.Controls.Add(this.labelNETTol);
            this.Controls.Add(this.labelMassTol);
            this.Controls.Add(this.groupBoxClassRep);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmClusterParameters";
            this.Text = "Clustering Parameters";
            this.groupBoxIntensity.ResumeLayout(false);
            this.groupBoxClassRep.ResumeLayout(false);
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
            Properties.Settings.Default.UserClusterOptionsUseMeanRepresentation = (ClusterRepresentativeType == MultiAlignEngine.Clustering.clsClusterOptions.enmClusterRepresentativeType.MEAN);
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

		public double MassTolerance
		{
			get
			{
				return (Convert.ToDouble(textBoxMassTol.Text)) ;
			}
			set
			{
				textBoxMassTol.Text = Convert.ToString(value) ;
			}
		}

		public double NETTolerance
		{
			get
			{
				return (Convert.ToDouble(textBoxNETTol.Text)) ;
			}
			set
			{
				textBoxNETTol.Text = Convert.ToString(value) ;
			}
		}

		public MultiAlignEngine.Clustering.clsClusterOptions.enmClusterIntensityType ClusterIntensityType
		{
			get
			{
				if (radioButtonMaxData.Checked)
					return MultiAlignEngine.Clustering.clsClusterOptions.enmClusterIntensityType.MAX_PER_DATASET ;
				else
					return MultiAlignEngine.Clustering.clsClusterOptions.enmClusterIntensityType.SUM_PER_DATASET ;

			}
			set
			{
				if (value == MultiAlignEngine.Clustering.clsClusterOptions.enmClusterIntensityType.MAX_PER_DATASET)
					radioButtonMaxData.Checked = true ;
				else
					radioButtonSumData.Checked = true ;
			}
		}

		public MultiAlignEngine.Clustering.clsClusterOptions.enmClusterRepresentativeType ClusterRepresentativeType
		{
			get
			{
				if (radioButtonMean.Checked)
					return MultiAlignEngine.Clustering.clsClusterOptions.enmClusterRepresentativeType.MEAN ;
				else
					return MultiAlignEngine.Clustering.clsClusterOptions.enmClusterRepresentativeType.MEDIAN ;
			}
			set
			{
				if (value == MultiAlignEngine.Clustering.clsClusterOptions.enmClusterRepresentativeType.MEAN)
					radioButtonMean.Checked = true ;
				else
					radioButtonMedian.Checked = true ;
			}
		}

		public MultiAlignEngine.Clustering.clsClusterOptions ClusterOptions
		{
			get
			{
				MultiAlignEngine.Clustering.clsClusterOptions options = new MultiAlignEngine.Clustering.clsClusterOptions() ;
				options.MassTolerance = MassTolerance ;
				options.NETTolerance = NETTolerance ;
				options.ClusterIntensityType = ClusterIntensityType ;
				options.ClusterRepresentativeType = ClusterRepresentativeType ;
				return options ;
			}
			set
			{
				MultiAlignEngine.Clustering.clsClusterOptions options = value ;
				MassTolerance = options.MassTolerance ;
				NETTolerance = options.NETTolerance ;
				ClusterIntensityType = options.ClusterIntensityType ;
				ClusterRepresentativeType = options.ClusterRepresentativeType ;
			}
		}
	}
}
