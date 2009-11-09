using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmViewPortEditor.
	/// </summary>
	public class frmViewPortEditor : PNNLControls.frmDialogBase
	{
		private PNNLControls.ctlViewPortEditor ctlViewPortEditor1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmViewPortEditor()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmViewPortEditor));
			this.ctlViewPortEditor1 = new PNNLControls.ctlViewPortEditor();
			this.SuspendLayout();
			// 
			// ctlViewPortEditor1
			// 
			this.ctlViewPortEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctlViewPortEditor1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ctlViewPortEditor1.Location = new System.Drawing.Point(0, 0);
			this.ctlViewPortEditor1.Name = "ctlViewPortEditor1";
			this.ctlViewPortEditor1.SelectedValue = ((System.Drawing.RectangleF)(resources.GetObject("ctlViewPortEditor1.SelectedValue")));
			this.ctlViewPortEditor1.Size = new System.Drawing.Size(152, 115);
			this.ctlViewPortEditor1.TabIndex = 0;
			// 
			// frmViewPortEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(152, 163);
			this.Controls.Add(this.ctlViewPortEditor1);
			this.MinimumSize = new System.Drawing.Size(160, 190);
			this.Name = "frmViewPortEditor";
			this.Text = "View Port Editor";
			this.Controls.SetChildIndex(this.ctlViewPortEditor1, 0);
			this.ResumeLayout(false);

		}
		#endregion

		public RectangleF SelectedValue 
		{
			get 
			{
				return this.ctlViewPortEditor1.SelectedValue;
			}
			set 
			{
				this.ctlViewPortEditor1.SelectedValue = value;
			}
		}
	}
}
