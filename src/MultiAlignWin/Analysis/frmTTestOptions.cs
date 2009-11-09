using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data ; 

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmTTestOptions.
	/// </summary>
	public class frmTTestOptions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button mbtn_OK;
		private System.Windows.Forms.Button mbtn_cancel;
		private System.Windows.Forms.Panel panel2;
		private PNNL.Controls.ctlGroupMembership mctlGroupMembership;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmTTestOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public frmTTestOptions(DataGrid grid)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			int start_column = 0 ; 
			if (grid.GetType() == typeof(ctlClusterGrid))
			{
				start_column = 4 ; 
			}
			mctlGroupMembership.SetOptions(2, (DataTable) grid.DataSource, start_column) ;
			mctlGroupMembership.NumGroupsEditable = false ; 
		}

		public int [] GroupIndices
		{
			get
			{
				return mctlGroupMembership.GroupIndices ; 
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.mbtn_cancel = new System.Windows.Forms.Button();
			this.mbtn_OK = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.mctlGroupMembership = new PNNL.Controls.ctlGroupMembership();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.mbtn_cancel);
			this.panel1.Controls.Add(this.mbtn_OK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 182);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(376, 40);
			this.panel1.TabIndex = 0;
			// 
			// mbtn_cancel
			// 
			this.mbtn_cancel.Location = new System.Drawing.Point(200, 8);
			this.mbtn_cancel.Name = "mbtn_cancel";
			this.mbtn_cancel.Size = new System.Drawing.Size(80, 24);
			this.mbtn_cancel.TabIndex = 1;
			this.mbtn_cancel.Text = "Cancel";
			this.mbtn_cancel.Click += new System.EventHandler(this.mbtn_cancel_Click);
			// 
			// mbtn_OK
			// 
			this.mbtn_OK.Location = new System.Drawing.Point(72, 9);
			this.mbtn_OK.Name = "mbtn_OK";
			this.mbtn_OK.Size = new System.Drawing.Size(80, 24);
			this.mbtn_OK.TabIndex = 0;
			this.mbtn_OK.Text = "OK";
			this.mbtn_OK.Click += new System.EventHandler(this.mbtn_OK_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.mctlGroupMembership);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(376, 182);
			this.panel2.TabIndex = 1;
			// 
			// mctlGroupMembership
			// 
			this.mctlGroupMembership.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mctlGroupMembership.Location = new System.Drawing.Point(0, 0);
			this.mctlGroupMembership.Name = "mctlGroupMembership";
			this.mctlGroupMembership.Size = new System.Drawing.Size(376, 182);
			this.mctlGroupMembership.TabIndex = 0;
			// 
			// frmTTestOptions
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 222);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "frmTTestOptions";
			this.Text = "Specify t-test Options";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void mbtn_OK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK ; 
			this.Hide() ; 
		}

		private void mbtn_cancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel ; 
			this.Hide() ; 		
		}
	}
}
