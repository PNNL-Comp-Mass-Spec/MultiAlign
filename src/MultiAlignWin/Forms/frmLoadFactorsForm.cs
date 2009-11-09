using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmLoadFactorsForm.
	/// </summary>
	public class frmLoadFactorsForm : PNNLControls.frmDialogBase
	{
		private PNNLControls.ctlTextDelimitedFileLoader ctlFileLoader;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmLoadFactorsForm()
		{
			InitializeComponent();
		}

		public frmLoadFactorsForm(string path)
		{
			InitializeComponent();
			ctlFileLoader.Path = path;
			ctlFileLoader.LoadFile(path);
		}


		public ArrayList Headers
		{		
			get
			{
				return ctlFileLoader.Headers;
			}
			set
			{
				ctlFileLoader.Headers = value;
			}
		}

		public Hashtable Data
		{
			get
			{
				return ctlFileLoader.Data;
			}
			set
			{
				ctlFileLoader.Data = value;
			}
		}

		public string Path
		{
			get
			{
				return ctlFileLoader.Path;
			}
			set
			{
				ctlFileLoader.Path = value;
			}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmLoadFactorsForm));
			this.ctlFileLoader = new PNNLControls.ctlTextDelimitedFileLoader();
			this.SuspendLayout();
			// 
			// ctlFileLoader
			// 
			this.ctlFileLoader.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlFileLoader.Location = new System.Drawing.Point(0, 0);
			this.ctlFileLoader.Name = "ctlFileLoader";
			this.ctlFileLoader.Path = null;
			this.ctlFileLoader.Size = new System.Drawing.Size(1040, 581);
			this.ctlFileLoader.TabIndex = 0;
			// 
			// frmLoadFactorsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1040, 629);
			this.Controls.Add(this.ctlFileLoader);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmLoadFactorsForm";
			this.Text = "Load Factors";
			this.Controls.SetChildIndex(this.ctlFileLoader, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
