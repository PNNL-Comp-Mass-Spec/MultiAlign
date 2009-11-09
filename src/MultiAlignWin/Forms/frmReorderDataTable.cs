using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmReorderDataTable : PNNLControls.frmDialogBase
	{
		private PNNLControls.ctlDataGridColumnReorder mobj_ReorderControl;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmReorderDataTable()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Gets and sets the data grid associated with the column reordering.
		/// </summary>
		public DataGrid DataGrid
		{
			get { return mobj_ReorderControl.DataGrid;  }
			set { mobj_ReorderControl.DataGrid = value; }
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
			this.mobj_ReorderControl = new PNNLControls.ctlDataGridColumnReorder();
			this.SuspendLayout();
			// 
			// mobj_ReorderControl
			// 
			this.mobj_ReorderControl.DataGrid = null;
			this.mobj_ReorderControl.Location = new System.Drawing.Point(0, 0);
			this.mobj_ReorderControl.Name = "mobj_ReorderControl";
			this.mobj_ReorderControl.Size = new System.Drawing.Size(648, 520);
			this.mobj_ReorderControl.TabIndex = 1;
			// 
			// frmReorderDataTable
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 581);
			this.Controls.Add(this.mobj_ReorderControl);
			this.Name = "frmReorderDataTable";
			this.Text = "Re-Order Table Columns";
			this.Controls.SetChildIndex(this.mobj_ReorderControl, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
