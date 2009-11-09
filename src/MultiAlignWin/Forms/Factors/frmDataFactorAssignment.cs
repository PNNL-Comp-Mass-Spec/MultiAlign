using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmDataFactorAssignment.
	/// </summary>
	public class frmDataFactorAssignment :  PNNLControls.frmDialogBase
	{
		private System.Windows.Forms.StatusBar status;
		private PNNLControls.ctlDataFactorValueAssignment ctlFactors;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		public frmDataFactorAssignment()
		{
			InitializeComponent();
		}

		public string GetDatasetFactorValue(string datasetName)
		{
			return this.ctlFactors.GetDatasetFactorValue(datasetName);
		}

		public void AddData(string datasetName, string factorValue) 
		{
			ctlFactors.AddDataset(datasetName, factorValue);
		}

		public void AddFactorValue(string factorEnumeration)
		{
			ctlFactors.AddFactorValue(factorEnumeration);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmDataFactorAssignment));
			this.ctlFactors = new PNNLControls.ctlDataFactorValueAssignment();
			this.status = new System.Windows.Forms.StatusBar();
			this.SuspendLayout();
			// 
			// ctlFactors
			// 
			this.ctlFactors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlFactors.Location = new System.Drawing.Point(0, 0);
			this.ctlFactors.Name = "ctlFactors";
			this.ctlFactors.Size = new System.Drawing.Size(744, 693);
			this.ctlFactors.TabIndex = 1;
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(0, 677);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(744, 16);
			this.status.TabIndex = 2;
			this.status.Text = "Drag data to the desired factor enumeration list";
			// 
			// frmDataFactorAssignment
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(744, 741);
			this.Controls.Add(this.status);
			this.Controls.Add(this.ctlFactors);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmDataFactorAssignment";
			this.Text = "Data to Factor Assignment";
			this.Controls.SetChildIndex(this.ctlFactors, 0);
			this.Controls.SetChildIndex(this.status, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
