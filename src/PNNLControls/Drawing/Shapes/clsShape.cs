using System;
using System.Collections ; 
using System.Drawing ; 

namespace PNNLControls
{
	/// <summary>
	/// The base class for defining how markers on a chart are drawn.
	/// Drawing can be achieved through the Draw method (slow) or by using the 
	/// offset arrays and the BitmapTools class (fast for many markers).
	/// </summary>
	public abstract class clsShape : ICloneable
	{
		protected int [] mXoffsets ; 
		protected int [] mYoffsets ; 
		protected int mSize = 1; 
		protected bool mHollow = false;
		protected readonly Color mDrawColor = Color.Chocolate;
		protected Brush mFillBrush;
		private Pen mBorderPen;

		public clsShape(int size, bool hollow)
		{
			this.mBorderPen = new Pen(this.mDrawColor, 1);
			this.mFillBrush = new SolidBrush(this.mDrawColor);
			this.Size = size;
			this.Hollow = hollow;
			this.CreateShapeOffsetArrays();
		}

		public Bitmap Bitmap
		{
			get
			{
				// Create a bitmap and draw the shape onto it
				Bitmap b = new Bitmap((int) (this.Size * 2 + this.mBorderPen.Width * 2 + 2), 
					(int) (this.Size * 2 + 2 + this.mBorderPen.Width * 2 + 2));
				int numPts = mXoffsets.Length ; 
				int penWidth = Convert.ToInt32(this.mBorderPen.Width) ; 
				for (int ptNum = 0 ; ptNum < numPts; ptNum++)
				{
					int x = mXoffsets[ptNum] ; 
					int y = mYoffsets[ptNum] ; 
					b.SetPixel(x+mSize+penWidth, y+mSize+penWidth, mDrawColor) ; 
				}
				return b; 
			}
		}
		/// <summary>
		/// Creates the offset arrays.  Called whenever the size or 
		/// hollowness changes.
		/// </summary>
		protected virtual void CreateShapeOffsetArrays() 
		{
			// Create a bitmap and draw the shape onto it
			Bitmap b = new Bitmap((int) (this.Size * 2 + this.mBorderPen.Width * 2 + 2), 
				(int) (this.Size * 2 + 2 + this.mBorderPen.Width * 2 + 2));
			Graphics g = Graphics.FromImage(b);
			g.TranslateTransform(this.Size + (int) this.mBorderPen.Width, this.Size + (int) this.mBorderPen.Width);
			this.Draw(g, this.mDrawColor);

			//extract the offset arrays from the bitmap
			ArrayList nonzero_xs = new ArrayList() ; 
			ArrayList nonzero_ys = new ArrayList() ; 

			for (int x = 0 ; x < b.Width ; x++)
			{
				for (int y = 0 ; y < b.Height ; y++)
				{
					Color pt_clr = b.GetPixel(x, y) ; 
					if (pt_clr.R == this.mDrawColor.R && pt_clr.B == this.mDrawColor.B &&
						pt_clr.G == this.mDrawColor.G)
					{
						nonzero_xs.Add(x - mSize - (int) this.mBorderPen.Width) ; 
						nonzero_ys.Add(y - mSize - (int) this.mBorderPen.Width) ; 
					}
				}
			}

			// Copy into arrays
			this.mXoffsets = new int [nonzero_xs.Count] ; 
			this.mYoffsets = new int [nonzero_ys.Count] ;
			nonzero_xs.CopyTo(this.mXoffsets) ; 
			nonzero_ys.CopyTo(this.mYoffsets) ; 
		}

		public int[] XOffsets 
		{
			get 
			{
				return (int[]) this.mXoffsets.Clone();
			}
		}

		public int[] YOffsets 
		{
			get 
			{
				return (int[]) this.mYoffsets.Clone();
			}
		}


		#region Properties
		public Pen BorderPen 
		{
			get 
			{
				return (Pen) this.mBorderPen.Clone();
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("BorderPen");
				}
				this.mBorderPen = (Pen) value.Clone();
				this.mBorderPen.Brush = null;
				this.mBorderPen.Color = this.mDrawColor;
				this.CreateShapeOffsetArrays();
			}
		}
		
		public int Size 
		{
			get 
			{
				return this.mSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentOutOfRangeException("Size", value, "Size must be greater than 0.");
				}
				this.mSize = value;
				this.CreateShapeOffsetArrays();
			}
		}

		public bool Hollow 
		{
			get 
			{
				return mHollow;
			}
			set 
			{
				this.mHollow = value;
				this.CreateShapeOffsetArrays();
			}
		}
		#endregion

		/// <summary>
		/// Draws the given marker on the graphics.  The marker is drawn 
		/// centered at (0, 0) on the graphics coordinates.  The marker is
		/// drawn in the given color.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="c"></param>
		public abstract void Draw(Graphics g, Color c);

		#region ICloneable Members

		public object Clone()
		{
			clsShape clone = (clsShape) this.MemberwiseClone();
			clone.mBorderPen = (Pen) this.mBorderPen.Clone();
			clone.mFillBrush = (Brush) this.mFillBrush.Clone();
			return clone;
		}

		#endregion
	}

	public class DiamondShape : clsShape
	{
		public DiamondShape(int size, bool hollow) : base(size, hollow) 
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Brush b = new SolidBrush(c);
			Pen p = this.BorderPen;
			p.Color = c;

			Point p1 = new Point(0, -mSize);
			Point p2 = new Point(-mSize, 0);
			Point p3 = new Point(0, mSize);
			Point p4 = new Point(mSize, 0);
			Point[] points = new Point[] {p1, p2, p3, p4};
			
			if (!Hollow) 
			{
				g.FillPolygon(b, points);
			}
			g.DrawPolygon(p, points);

			p.Dispose();
			b.Dispose();
		}
	}
	public class SquareShape : clsShape
	{
		public SquareShape(int size, bool hollow) : base(size, hollow) 
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Brush b = new SolidBrush(c);
			Pen p = this.BorderPen;
			p.Color = c;

			Point p1 = new Point(-mSize, -mSize) ; 
			Point p2 = new Point(-mSize, mSize) ; 
			Point p3 = new Point(mSize, mSize) ; 
			Point p4 = new Point(mSize, -mSize) ; 
			Point[] points = new Point[] {p1, p2, p3, p4};

			if (!Hollow) 
			{
				g.FillPolygon(b, points);
			}
			g.DrawPolygon(p, points);

			p.Dispose();
			b.Dispose();
		}
	}
	public class TriangleShape : clsShape
	{
		public TriangleShape(int size, bool hollow) : base(size, hollow)  
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Brush b = new SolidBrush(c);
			Pen p = this.BorderPen;
			p.Color = c;

			float half_height = 1.732F / 2 * this.Size;
			PointF p1 = new PointF(0, -half_height) ; 
			PointF p2 = new PointF(-mSize, half_height) ; 
			PointF p3 = new PointF(mSize, half_height);
			PointF[] points = new PointF[] {p1, p2, p3};

			if (!Hollow) 
			{
				g.FillPolygon(b, points);
			}
			g.DrawPolygon(p, points);

			p.Dispose();
			b.Dispose();
		}
	}
	public class CrossShape : clsShape
	{
		public CrossShape(int size, bool hollow) : base(size, hollow) 
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Pen p = this.BorderPen;
			p.Color = c;

			g.DrawLine(p, -mSize, -mSize, mSize, mSize) ; 
			g.DrawLine(p, -mSize, mSize, mSize, -mSize) ;

			p.Dispose();
		}
	}
	public class PlusShape : clsShape
	{
		public PlusShape(int size, bool hollow) : base(size, hollow)  
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Pen p = this.BorderPen;
			p.Color = c;

			g.DrawLine(p, -mSize, 0, mSize, 0) ; 
			g.DrawLine(p, 0, -mSize, 0, mSize) ;

			p.Dispose();
		}
	}
	public class StarShape : clsShape
	{
		public int mintWidth = 2 ; 
		public StarShape(int size, bool hollow) : base(size, hollow) 
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Pen p = this.BorderPen;
			p.Width = mintWidth ; 
			p.Color = c;

			g.DrawLine(p, 0, -mSize, 0, mSize); 
			g.DrawLine(p, -mSize, 0, mSize, 0);
			g.DrawLine(p, -mSize, -mSize, mSize, mSize) ; 
			g.DrawLine(p, -mSize, mSize, mSize, -mSize) ;

			p.Dispose();
		}
	}
	public class BubbleShape : clsShape
	{
		public BubbleShape(int size, bool hollow) : base(size, hollow) 
		{
		}
		public override void Draw(Graphics g, Color c) 
		{
			Brush b = new SolidBrush(c);
			Pen p = this.BorderPen;
			p.Color = c;

			if (!Hollow) 
			{
				g.FillEllipse(b, -mSize, -mSize, 2 * mSize, 2 * mSize);
			}
			g.DrawEllipse(p, -mSize, -mSize, 2 * mSize, 2 * mSize);
			p.Dispose();
			b.Dispose();
		}
	}

	public class PointShape : clsShape 
	{
		public PointShape(int size, bool hollow) : base(size, hollow) 
		{
		}

		public override void Draw(Graphics g, Color c) 
		{
			Pen p = new Pen(c, 1);

			g.DrawLine(p, 0, 0, 0, 1);

			p.Dispose();
		}

		/// <summary>
		/// Simpler to create the offset arrays by hand instead of 
		/// drawing to a bitmap and then reading out the colors.
		/// </summary>
		protected override void CreateShapeOffsetArrays()
		{
			this.mXoffsets = new int[] {0};
			this.mYoffsets = new int[] {0};
		}
	}
}





