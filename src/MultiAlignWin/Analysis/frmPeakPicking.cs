using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWinNew
{
	/// <summary>
	/// Summary description for frmPeakPicking.
	/// </summary>
	public class frmPeakPicking : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelAveMass;
		private System.Windows.Forms.Label labelLogAbund;
		private System.Windows.Forms.Label labelMonoMass;
		private System.Windows.Forms.Label labelScan;
		private System.Windows.Forms.Label labelNET;
		private System.Windows.Forms.Label labelFit;
		private System.Windows.Forms.TextBox textBoxMonoMass;
		private System.Windows.Forms.TextBox textBoxAveMass;
		private System.Windows.Forms.TextBox textBoxLogAbund;
		private System.Windows.Forms.TextBox textBoxScan;
		private System.Windows.Forms.TextBox textBoxNET;
		private System.Windows.Forms.TextBox textBoxFit;
		private System.Windows.Forms.Label labelMonoMassCt;
		private System.Windows.Forms.Label labelAveMassCt;
		private System.Windows.Forms.TextBox textBoxMonoMassCt;
		private System.Windows.Forms.TextBox textBoxAveMassCt;
		private System.Windows.Forms.Label labelMMppm;
		private System.Windows.Forms.Label labelAveMppm;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxUseNET;
		private System.Windows.Forms.Label labelMaxDist;
		private System.Windows.Forms.TextBox textBoxMaxDist;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPeakPicking()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.labelMonoMass = new System.Windows.Forms.Label();
			this.labelAveMass = new System.Windows.Forms.Label();
			this.labelLogAbund = new System.Windows.Forms.Label();
			this.labelScan = new System.Windows.Forms.Label();
			this.labelNET = new System.Windows.Forms.Label();
			this.labelFit = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.labelMonoMassCt = new System.Windows.Forms.Label();
			this.labelMMppm = new System.Windows.Forms.Label();
			this.textBoxMonoMass = new System.Windows.Forms.TextBox();
			this.textBoxAveMass = new System.Windows.Forms.TextBox();
			this.textBoxLogAbund = new System.Windows.Forms.TextBox();
			this.textBoxScan = new System.Windows.Forms.TextBox();
			this.textBoxNET = new System.Windows.Forms.TextBox();
			this.textBoxFit = new System.Windows.Forms.TextBox();
			this.labelAveMassCt = new System.Windows.Forms.Label();
			this.textBoxMonoMassCt = new System.Windows.Forms.TextBox();
			this.textBoxAveMassCt = new System.Windows.Forms.TextBox();
			this.labelAveMppm = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxUseNET = new System.Windows.Forms.CheckBox();
			this.labelMaxDist = new System.Windows.Forms.Label();
			this.textBoxMaxDist = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// labelMonoMass
			// 
			this.labelMonoMass.Location = new System.Drawing.Point(16, 40);
			this.labelMonoMass.Name = "labelMonoMass";
			this.labelMonoMass.Size = new System.Drawing.Size(128, 23);
			this.labelMonoMass.TabIndex = 0;
			this.labelMonoMass.Text = "Monoisotopic Mass :";
			// 
			// labelAveMass
			// 
			this.labelAveMass.Location = new System.Drawing.Point(16, 64);
			this.labelAveMass.Name = "labelAveMass";
			this.labelAveMass.TabIndex = 1;
			this.labelAveMass.Text = "Average Mass :";
			// 
			// labelLogAbund
			// 
			this.labelLogAbund.Location = new System.Drawing.Point(16, 88);
			this.labelLogAbund.Name = "labelLogAbund";
			this.labelLogAbund.TabIndex = 2;
			this.labelLogAbund.Text = "Log (Abundance) :";
			// 
			// labelScan
			// 
			this.labelScan.Location = new System.Drawing.Point(16, 112);
			this.labelScan.Name = "labelScan";
			this.labelScan.TabIndex = 3;
			this.labelScan.Text = "Scan :";
			// 
			// labelNET
			// 
			this.labelNET.Location = new System.Drawing.Point(16, 136);
			this.labelNET.Name = "labelNET";
			this.labelNET.TabIndex = 4;
			this.labelNET.Text = "NET :";
			// 
			// labelFit
			// 
			this.labelFit.Location = new System.Drawing.Point(16, 160);
			this.labelFit.Name = "labelFit";
			this.labelFit.TabIndex = 5;
			this.labelFit.Text = "Fit :";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(120, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 16);
			this.label7.TabIndex = 6;
			this.label7.Text = "Weight Factor";
			// 
			// labelMonoMassCt
			// 
			this.labelMonoMassCt.Location = new System.Drawing.Point(200, 40);
			this.labelMonoMassCt.Name = "labelMonoMassCt";
			this.labelMonoMassCt.Size = new System.Drawing.Size(56, 23);
			this.labelMonoMassCt.TabIndex = 7;
			this.labelMonoMassCt.Text = "Constraint";
			// 
			// labelMMppm
			// 
			this.labelMMppm.Location = new System.Drawing.Point(336, 40);
			this.labelMMppm.Name = "labelMMppm";
			this.labelMMppm.Size = new System.Drawing.Size(32, 23);
			this.labelMMppm.TabIndex = 8;
			this.labelMMppm.Text = "ppm";
			// 
			// textBoxMonoMass
			// 
			this.textBoxMonoMass.Location = new System.Drawing.Point(128, 40);
			this.textBoxMonoMass.Name = "textBoxMonoMass";
			this.textBoxMonoMass.Size = new System.Drawing.Size(56, 20);
			this.textBoxMonoMass.TabIndex = 9;
			this.textBoxMonoMass.Text = "0.01";
			this.textBoxMonoMass.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxAveMass
			// 
			this.textBoxAveMass.Location = new System.Drawing.Point(128, 64);
			this.textBoxAveMass.Name = "textBoxAveMass";
			this.textBoxAveMass.Size = new System.Drawing.Size(56, 20);
			this.textBoxAveMass.TabIndex = 10;
			this.textBoxAveMass.Text = "0.01";
			this.textBoxAveMass.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxLogAbund
			// 
			this.textBoxLogAbund.Location = new System.Drawing.Point(128, 88);
			this.textBoxLogAbund.Name = "textBoxLogAbund";
			this.textBoxLogAbund.Size = new System.Drawing.Size(56, 20);
			this.textBoxLogAbund.TabIndex = 11;
			this.textBoxLogAbund.Text = "0.1";
			this.textBoxLogAbund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxScan
			// 
			this.textBoxScan.Location = new System.Drawing.Point(128, 112);
			this.textBoxScan.Name = "textBoxScan";
			this.textBoxScan.Size = new System.Drawing.Size(56, 20);
			this.textBoxScan.TabIndex = 12;
			this.textBoxScan.Text = "0.01";
			this.textBoxScan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxNET
			// 
			this.textBoxNET.Location = new System.Drawing.Point(128, 136);
			this.textBoxNET.Name = "textBoxNET";
			this.textBoxNET.Size = new System.Drawing.Size(56, 20);
			this.textBoxNET.TabIndex = 13;
			this.textBoxNET.Text = "0.1";
			this.textBoxNET.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxFit
			// 
			this.textBoxFit.Location = new System.Drawing.Point(128, 160);
			this.textBoxFit.Name = "textBoxFit";
			this.textBoxFit.Size = new System.Drawing.Size(56, 20);
			this.textBoxFit.TabIndex = 14;
			this.textBoxFit.Text = "0.1";
			this.textBoxFit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelAveMassCt
			// 
			this.labelAveMassCt.Location = new System.Drawing.Point(200, 64);
			this.labelAveMassCt.Name = "labelAveMassCt";
			this.labelAveMassCt.Size = new System.Drawing.Size(64, 23);
			this.labelAveMassCt.TabIndex = 15;
			this.labelAveMassCt.Text = "Constraint";
			// 
			// textBoxMonoMassCt
			// 
			this.textBoxMonoMassCt.Location = new System.Drawing.Point(264, 40);
			this.textBoxMonoMassCt.Name = "textBoxMonoMassCt";
			this.textBoxMonoMassCt.Size = new System.Drawing.Size(56, 20);
			this.textBoxMonoMassCt.TabIndex = 16;
			this.textBoxMonoMassCt.Text = "10.0";
			this.textBoxMonoMassCt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxAveMassCt
			// 
			this.textBoxAveMassCt.Location = new System.Drawing.Point(264, 64);
			this.textBoxAveMassCt.Name = "textBoxAveMassCt";
			this.textBoxAveMassCt.Size = new System.Drawing.Size(56, 20);
			this.textBoxAveMassCt.TabIndex = 17;
			this.textBoxAveMassCt.Text = "10.0";
			this.textBoxAveMassCt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelAveMppm
			// 
			this.labelAveMppm.Location = new System.Drawing.Point(336, 64);
			this.labelAveMppm.Name = "labelAveMppm";
			this.labelAveMppm.Size = new System.Drawing.Size(32, 23);
			this.labelAveMppm.TabIndex = 18;
			this.labelAveMppm.Text = "ppm";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(104, 224);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 19;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(192, 224);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 20;
			this.buttonCancel.Text = "Cancel";
			// 
			// checkBoxUseNET
			// 
			this.checkBoxUseNET.Location = new System.Drawing.Point(200, 136);
			this.checkBoxUseNET.Name = "checkBoxUseNET";
			this.checkBoxUseNET.Size = new System.Drawing.Size(72, 24);
			this.checkBoxUseNET.TabIndex = 21;
			this.checkBoxUseNET.Text = "Use NET";
			this.checkBoxUseNET.CheckedChanged += new System.EventHandler(this.UseNETCheckChanged_event);
			// 
			// labelMaxDist
			// 
			this.labelMaxDist.Location = new System.Drawing.Point(16, 184);
			this.labelMaxDist.Name = "labelMaxDist";
			this.labelMaxDist.TabIndex = 22;
			this.labelMaxDist.Text = "Max Distance :";
			// 
			// textBoxMaxDist
			// 
			this.textBoxMaxDist.Location = new System.Drawing.Point(128, 184);
			this.textBoxMaxDist.Name = "textBoxMaxDist";
			this.textBoxMaxDist.Size = new System.Drawing.Size(56, 20);
			this.textBoxMaxDist.TabIndex = 23;
			this.textBoxMaxDist.Text = "0.1";
			this.textBoxMaxDist.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// frmPeakPicking
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(378, 263);
			this.Controls.Add(this.textBoxMaxDist);
			this.Controls.Add(this.labelMaxDist);
			this.Controls.Add(this.checkBoxUseNET);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelAveMppm);
			this.Controls.Add(this.textBoxAveMassCt);
			this.Controls.Add(this.textBoxMonoMassCt);
			this.Controls.Add(this.labelAveMassCt);
			this.Controls.Add(this.textBoxFit);
			this.Controls.Add(this.textBoxNET);
			this.Controls.Add(this.textBoxScan);
			this.Controls.Add(this.textBoxLogAbund);
			this.Controls.Add(this.textBoxAveMass);
			this.Controls.Add(this.textBoxMonoMass);
			this.Controls.Add(this.labelMMppm);
			this.Controls.Add(this.labelMonoMassCt);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.labelFit);
			this.Controls.Add(this.labelNET);
			this.Controls.Add(this.labelScan);
			this.Controls.Add(this.labelLogAbund);
			this.Controls.Add(this.labelAveMass);
			this.Controls.Add(this.labelMonoMass);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPeakPicking";
			this.Text = "Peak Picking Parameters";
			this.ResumeLayout(false);

		}
		#endregion

		private void UseNETCheckChanged_event(object sender, System.EventArgs e)
		{
			if (checkBoxUseNET.Checked)
			{
				labelScan.Enabled = false ;
				textBoxScan.Enabled = false ;
			}
		}

		public float MonoMassWeight
		{
			get
			{
				return Convert.ToSingle(textBoxMonoMass.Text) ; 
			}
		}

		public float AveMassWeight
		{
			get
			{
				return Convert.ToSingle(textBoxAveMass.Text) ; 
			}
		}

		public float LogDistanceWeight
		{
			get
			{
				return Convert.ToSingle(textBoxLogAbund.Text) ; 
			}
		}

		public float ScanWeight
		{
			get
			{
				return Convert.ToSingle(textBoxScan.Text) ; 
			}
		}

		public float FitWeight
		{
			get
			{
				return Convert.ToSingle(textBoxFit.Text) ; 
			}
		}

		public float NETWeight
		{
			get
			{
				return Convert.ToSingle(textBoxNET.Text) ; 
			}
		}


		public float ConstraintMonoMass
		{
			get
			{
				return Convert.ToSingle(textBoxMonoMassCt.Text) ; 
			}
		}
		public float ConstraintAveMass
		{
			get
			{
				return Convert.ToSingle(textBoxAveMassCt.Text) ; 
			}
		}


		public double MaxDistance
		{
			get
			{
				return Convert.ToDouble(textBoxMaxDist.Text) ; 
			}
		}
		public bool UseNET
		{
			get
			{
				return checkBoxUseNET.Checked ; 
			}
		}
	}
}
