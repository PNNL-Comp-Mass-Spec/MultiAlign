using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for SingleAxis.
	/// </summary>
	public class SingleAxis : System.Windows.Forms.UserControl 
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.PictureBox pictureBox1;

		private clsPlotSingleAxis plotter = new clsPlotSingleAxis();

		public clsPlotSingleAxis Plotter 
		{
			get{return plotter;}
			set{plotter=value;}
		}

		public bool IsVertical
		{
			get{return plotter.IsVertical;}
			set{plotter.IsVertical=value;}
		}

		public SingleAxis()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(144, 288);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
			// 
			// SingleAxis
			// 
			this.Controls.Add(this.pictureBox1);
			this.Name = "SingleAxis";
			this.Size = new System.Drawing.Size(144, 288);
			this.Load += new System.EventHandler(this.SingleAxis_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void SingleAxis_Load(object sender, System.EventArgs e)
		{
		
		}

		private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			plotter.Bounds = this.Bounds;
			//e.Graphics.FillRectangle(new SolidBrush(this.BackColor), pnlPlot.Bounds);
			plotter.Layout(e.Graphics);
			plotter.Draw(e.Graphics, Color.Black);
		}
	}
}
