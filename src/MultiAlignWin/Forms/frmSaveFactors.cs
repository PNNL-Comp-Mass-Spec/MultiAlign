using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmSaveFactors.
	/// </summary>
	public class frmSaveFactors : PNNLControls.frmDialogBase
	{
		private PNNLControls.ctlTextDelimitedFileSave ctlSaveFactors;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSaveFactors()
		{
			InitializeComponent();
		}

		public ArrayList Headers
		{		
			get
			{
				return ctlSaveFactors.Headers;
			}
			set
			{
				ctlSaveFactors.Headers = value;
			}
		}

		public Hashtable Data
		{
			get
			{
				return ctlSaveFactors.Data;
			}
			set
			{
				ctlSaveFactors.Data = value;
			}
		}

		public string Path
		{
			get
			{
				return ctlSaveFactors.Path;
			}
			set
			{
				ctlSaveFactors.Path = value;
			}
		}


		public void WriteFile()
		{
			ctlSaveFactors.WriteFile();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSaveFactors));
			this.ctlSaveFactors = new PNNLControls.ctlTextDelimitedFileSave();
			this.SuspendLayout();
			// 
			// ctlSaveFactors
			// 
			this.ctlSaveFactors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlSaveFactors.Location = new System.Drawing.Point(0, 0);
			this.ctlSaveFactors.Name = "ctlSaveFactors";
			this.ctlSaveFactors.Size = new System.Drawing.Size(256, 285);
			this.ctlSaveFactors.TabIndex = 0;
			// 
			// frmSaveFactors
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(256, 333);
			this.Controls.Add(this.ctlSaveFactors);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmSaveFactors";
			this.Text = "Save Factors";
			this.Controls.SetChildIndex(this.ctlSaveFactors, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
