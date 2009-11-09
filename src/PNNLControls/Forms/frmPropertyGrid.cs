using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for frmPropertyGrid.
	/// </summary>
	public class frmPropertyGrid : PNNLControls.frmDialogBase
	{
		private System.Windows.Forms.PropertyGrid mPropertyGrid;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPropertyGrid()
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
			this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// mPropertyGrid
			// 
			this.mPropertyGrid.CommandsVisibleIfAvailable = true;
			this.mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mPropertyGrid.LargeButtons = false;
			this.mPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.mPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.mPropertyGrid.Name = "mPropertyGrid";
			this.mPropertyGrid.Size = new System.Drawing.Size(370, 296);
			this.mPropertyGrid.TabIndex = 1;
			this.mPropertyGrid.Text = "propertyGrid1";
			this.mPropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.mPropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// frmPropertyGrid
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelEnabled = false;
			this.ClientSize = new System.Drawing.Size(370, 344);
			this.Controls.Add(this.mPropertyGrid);
			this.Name = "frmPropertyGrid";
			this.Text = "Property Editor";
			this.Controls.SetChildIndex(this.mPropertyGrid, 0);
			this.ResumeLayout(false);

		}
		#endregion

		public Object SelectedObject 
		{
			get 
			{
				return this.mPropertyGrid.SelectedObject;
			}
			set 
			{
				this.mPropertyGrid.SelectedObject = value;
			}
		}
	}
}
