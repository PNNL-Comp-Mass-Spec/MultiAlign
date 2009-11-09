using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Provides support for displaying an array of colors evenly spaced across a 
	/// panel.
	/// </summary>
	public class pnlColorInterpolationViewer : System.Windows.Forms.UserControl
	{

		private Color[] colors = new Color[0];

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private bool mVertical = true;

		public pnlColorInterpolationViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.UpdateStyles();
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

		public Color[] Colors 
		{
			get 
			{
				return (Color[]) this.colors.Clone();
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("Colors");
				}
				this.colors = (Color[]) value.Clone();
				this.Invalidate();
			}
		}

		public bool Vertical 
		{
			get 
			{
				return this.mVertical;
			}
			set 
			{
				this.mVertical = value;
				this.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.Vertical) 
			{
				int width = this.Width;
				int count = this.colors.Length;
				float height = ((float) this.Height) / count;
				for (int i = 0; i < count; i++) 
				{
					Color c = this.colors[i];
					e.Graphics.FillRectangle(new SolidBrush(c), 
						0, ((float) this.Height) / count * i, width, height);
				}
			}
			else 
			{
				int height = this.Width;
				int count = this.colors.Length;
				float width = ((float) this.Width) / count;
				for (int i = 0; i < count; i++) 
				{
					Color c = this.colors[i];
					e.Graphics.FillRectangle(new SolidBrush(c), 
						((float) this.Width) / count * i, 0, width, height);
				}
			}

		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// pnlColorInterpolationViewer
			// 
			this.Name = "pnlColorInterpolationViewer";
		}
		#endregion
	}
}
