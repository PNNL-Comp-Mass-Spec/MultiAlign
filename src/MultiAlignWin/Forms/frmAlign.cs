using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for frmAlign.
	/// </summary>
	public class frmAlign : System.Windows.Forms.Form
	{
		
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
		
		
		private System.Windows.Forms.Splitter splitter3;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmAlign()
		{
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmAlign));
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this.SuspendLayout();
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(1050, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(6, 725);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(20, 295);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(338, 6);
			this.splitter2.TabIndex = 3;
			this.splitter2.TabStop = false;
			// 
			// splitter3
			// 
			this.splitter3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.splitter3.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter3.Location = new System.Drawing.Point(0, 0);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(1050, 3);
			this.splitter3.TabIndex = 3;
			this.splitter3.TabStop = false;
			// 
			// frmAlign
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1056, 725);
			this.Controls.Add(this.splitter3);
			this.Controls.Add(this.splitter1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmAlign";
			this.Text = "Alignment";
			this.Controls.SetChildIndex(this.splitter1, 0);
			this.Controls.SetChildIndex(this.splitter3, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
