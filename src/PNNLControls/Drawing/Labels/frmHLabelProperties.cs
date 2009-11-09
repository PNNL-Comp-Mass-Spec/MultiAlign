using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmHLabelProperties.
	/// </summary>
	public class frmHLabelProperties  : frmDialogBase
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmHLabelProperties()
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

		ctlHLabelLevelProperties mLabelProps = null;
		public ctlHLabelLevelProperties LabelProps 
		{
			get{return this.mLabelProps;}
			set{
				this.mLabelProps = value;
				this.Controls.Add(this.mLabelProps);
				this.mLabelProps.Location = new Point(0,0);
				this.mLabelProps.Size = new Size(200, 40);
				this.mLabelProps.Visible = true;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// frmHLabelProperties
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "frmHLabelProperties";
			this.Text = "frmHLabelProperties";

		}
		#endregion
	}
}
