using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using IDLTools;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlLegendColor.
	/// </summary>
	public class ctlLegendColor : System.Windows.Forms.Panel
	{
		public enum DisplayMode
		{
			NorthPointer,
			NorthEastPointer,
			EastPointer,
			SouthEastPointer,
			SouthPointer,
			WestPointer,
			Block
		}

		public delegate void ColorChangedDelegate ();
		public event ColorChangedDelegate ColorChanged = null;

		private System.Windows.Forms.ColorDialog colorDialog1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ctlLegendColor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.BackColor = Color.Transparent;

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
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			// 
			// ctlLegendColor
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ctlLegendColor_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ctlLegendColor_MouseDown);

		}
		#endregion

		public int PointerPosition
		{
			get
			{
				int pos = 0;
				switch (mDisplayMode)
				{
					case DisplayMode.NorthPointer:	
						pos = this.ClientSize.Width/2;
						break;
					case DisplayMode.NorthEastPointer:	
						pos = 0;
						break;
					case DisplayMode.EastPointer:	
						pos = this.ClientSize.Height/2;
						break;

					case DisplayMode.SouthEastPointer:	
						pos = this.ClientSize.Height;
						break;

					case DisplayMode.SouthPointer:
						pos = this.ClientSize.Width/2;
						break;

					case DisplayMode.WestPointer:	
						pos = this.ClientSize.Height/2;
						break;
					
					case DisplayMode.Block:	
						pos = 0;
						break;
				}
				return pos;
			}
		}

		public void EditColor()
		{
			ColorDialog c = new ColorDialog();
			c.Color = this.LegendColor;
			c.FullOpen = true;
			c.AnyColor = true;
			if(c.ShowDialog(this) == DialogResult.OK)
			{
				this.LegendColor = c.Color;
				this.Refresh();
				if (this.ColorChanged!=null)
					ColorChanged();
			}
		}

		private void ctlLegendColor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				EditColor();
			}
		}

		private void PaintPoly(Graphics g, Point[] pts)
		{
			//this.BackColor = Color.Transparent;
			//this.BackColor = Color.Pink;

			Pen p = new Pen(Color.Black, 1);

			System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
			
			for (int i=0; i<pts.Length-1; i++)
                path.AddLine(pts[i], pts[i+1]);
			path.AddLine(pts[pts.Length-1], pts[0]);
			
			Region r = new Region(path);
			SolidBrush b = new SolidBrush(mLegendColor);
			g.FillRegion(b, r);
			g.DrawPolygon(p, pts);

		}

		private void ctlLegendColor_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Point[] pts = new Point[3];

			switch (mDisplayMode)
			{
				case DisplayMode.NorthPointer:	
					pts[0] = new Point (0, this.ClientSize.Height-1);
					pts[1] = new Point (this.ClientSize.Width-1, this.ClientSize.Height-1);
					pts[2] = new Point (this.ClientSize.Width/2, 0);
					break;

				case DisplayMode.NorthEastPointer:	
					pts[0] = new Point (0, 1);
					pts[1] = new Point (this.ClientSize.Width,1);
					pts[2] = new Point (0, this.ClientSize.Height/2);
					break;

				case DisplayMode.EastPointer:	
					pts[0] = new Point (0, 0);
					pts[1] = new Point (0,this.ClientSize.Height-1);
					pts[2] = new Point (this.ClientSize.Width-1, this.ClientSize.Height/2);
					break;

				case DisplayMode.SouthEastPointer:	
					pts[0] = new Point (0, this.ClientSize.Height/2);
					pts[1] = new Point (this.ClientSize.Width,this.ClientSize.Height-1);
					pts[2] = new Point (0, this.ClientSize.Height-1);
					break;

				case DisplayMode.SouthPointer:	
					pts[0] = new Point (0, 0);
					pts[1] = new Point (this.ClientSize.Width-1, 0);
					pts[2] = new Point (this.ClientSize.Width/2, this.ClientSize.Height);
					break;

				case DisplayMode.WestPointer:	
					pts[0] = new Point (this.ClientSize.Width-1, this.ClientSize.Height-1);
					pts[1] = new Point (this.ClientSize.Width-1, 0);
					pts[2] = new Point (0, this.ClientSize.Height/2);
					break;
					
				case DisplayMode.Block:	
					pts = new Point[4];
					pts[0] = new Point (0, 0);
					pts[1] = new Point (0, this.ClientSize.Height-1);
					pts[2] = new Point (this.ClientSize.Width-1, this.ClientSize.Height-1);
					pts[3] = new Point (this.ClientSize.Width-1, 0);
					break;
			}

			PaintPoly(e.Graphics, pts);

		}

		Color mLegendColor = Color.White;
		[Persist]
		public Color LegendColor
		{
			get{return this.mLegendColor;}
			set{this.mLegendColor=value;}
		}
	
		/// <summary>
		/// position of this control relative to other controls.
		/// 0.0 to 1.0
		/// </summary>
		double mPercent = 0;
		[Persist]
		public double Percent
		{
			get{return this.mPercent;}
			set{this.mPercent=value;}
		}

		ctlLegendColor.DisplayMode mDisplayMode = ctlLegendColor.DisplayMode.Block;
		[Persist]
		public DisplayMode Mode
		{
			get{return this.mDisplayMode;}
			set{this.mDisplayMode=value;}
		}
	}
}
