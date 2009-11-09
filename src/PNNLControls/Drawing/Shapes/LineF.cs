using System;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// A rectangle structure for use with SegmentF.  Otherwise basically the same 
	/// as a RectangleF.
	/// </summary>
	public struct GeomRectangleF 
	{
		internal SegmentF left;
		internal SegmentF right;
		internal SegmentF top;
		internal SegmentF bottom;

		RectangleF rect;

		public GeomRectangleF(System.Drawing.RectangleF rect) 
		{
			left = new SegmentF(rect.Left, rect.Top, rect.Left, rect.Bottom);
			right = new SegmentF(rect.Right, rect.Top, rect.Right, rect.Bottom);
			top = new SegmentF(rect.Left, rect.Top, rect.Right, rect.Top);
			bottom = new SegmentF(rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			this.rect = rect;
		}

		public float Right 
		{
			get 
			{
				return rect.Right;
			}
		}

		public float Bottom 
		{
			get 
			{
				return rect.Bottom;
			}
		}

		public float Top 
		{
			get 
			{
				return rect.Top;
			}
		}

		public float Left 
		{
			get 
			{
				return rect.Left;
			}
		}

		public bool Contains(PointF point) 
		{
			return rect.Contains(point);
		}
	}

	public struct SegmentF
	{
		float xStart;
		float yStart;
		float xEnd;
		float yEnd;

		public SegmentF(float xStart, float yStart, float xEnd, float yEnd) 
		{
			this.xStart = xStart;
			this.yStart = yStart;
			this.xEnd = xEnd;
			this.yEnd = yEnd;
		}

		public SegmentF(PointF start, PointF end) : this(start.X, start.Y, end.X, end.Y)
		{
		}

		#region "Properties"

		public float StartX 
		{
			get 
			{
				return this.xStart;
			}
		}

		public float StartY 
		{
			get 
			{
				return this.yStart;
			}
		}

		public float EndX 
		{
			get 
			{
				return this.xEnd;
			}
		}

		public float EndY 
		{
			get 
			{
				return this.yEnd;
			}
		}
		#endregion

		public override string ToString()
		{
			return "{" + this.StartX + "," + this.StartY + " to "
				+ this.EndX + "," + this.EndY  + "}";
		}


		public System.Drawing.PointF Intersection(SegmentF line) 
		{
			// Want to solve equations
			// a_x * (1 - s) + b_x * s = c_x * (1 - t) + d_x * t
			// and same with x replaced with y, for s and t
			// a and b are end points of this line, c and d are endpoints of the other line
			float a_x = this.xStart;
			float a_y = this.yStart;
			float b_x = this.xEnd;
			float b_y = this.yEnd;
			float c_x = line.xStart;
			float c_y = line.yStart;
			float d_x = line.xEnd;
			float d_y = line.yEnd;

			// matrix form of above equations
			// A s + B t = C
			// D s + E t = F
			float A = b_x - a_x;
			float B = c_x - d_x;
			float C = c_x - a_x;

			float D = b_y - a_y;
			float E = c_y - d_y;
			float F = c_y - a_y;

			// matrix goes to 
			// AD s + BD t = CD
			// DA s + EA t = FA
			float s, t;
			if (A == 0) 
			{
				t = C / B;
				s = (F - E * t) / D;
			} 
			else if (D == 0) 
			{
				t = E / F;
				s = (C - B * t) / A;
			} 
			else 
			{
				float AD = A * D;
				float BD = B * D;
				float CD = C * D;
				float EA = E * A;
				float FA = F * A;
				// then subtracting gives the equation
				// t (BD - EA) = CD - FA
				t = (CD - FA) / (BD - EA);
				s = (CD - BD * t) / AD;
			}
			
			if (float.IsNaN(s)) 
			{
				// use t to compute intersection
				return new PointF(c_x * (1 - t) + d_x * t, c_y * (1 - t) + d_y * t);
			}
			// otherwise use s to compute intersection
			return new PointF(a_x * (1 - s) + b_x * s, a_y * (1 - s) + b_y * s);
		}

		/// <summary>
		/// Computes the intersection of this segment with the given rectangle
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public SegmentF Intersection(GeomRectangleF rect) 
		{
			// If fully within the rectangle, then return this segment
			if (rect.Contains(new PointF(this.xStart, this.yStart)) &&
				rect.Contains(new PointF(this.xEnd, this.yEnd)))
			{
				return this;
			}

			bool intersectsLeft = Intersects(rect.left);
			bool intersectsRight = Intersects(rect.right);
			bool intersectsBottom = Intersects(rect.bottom);
			bool intersectsTop = Intersects(rect.top);

			int intersections = 0;
			if (intersectsLeft) intersections++;
			if (intersectsRight) intersections++;
			if (intersectsBottom) intersections++;
			if (intersectsTop) intersections++;


			// If intersects two sides, then cut down this segment on both ends.
			// Make sure the returned segment has the same vector direction (don't 
			// accidently return the reverse) of this one.
			if (intersections >= 2) 
			{
				if (intersectsLeft && intersectsRight) 
				{
					if (xStart < rect.left.xStart) 
					{
						return new SegmentF(this.Intersection(rect.left), this.Intersection(rect.right));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.right), this.Intersection(rect.left));
					}
				}
				if (intersectsLeft && intersectsTop) 
				{
					if (xStart < rect.left.xStart) 
					{
						return new SegmentF(this.Intersection(rect.left), this.Intersection(rect.top));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.top), this.Intersection(rect.left));
					}
				}
				if (intersectsLeft && intersectsBottom) 
				{
					if (xStart < rect.left.xStart) 
					{
						return new SegmentF(this.Intersection(rect.left), this.Intersection(rect.bottom));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.bottom), this.Intersection(rect.left));
					}
				}
				if (intersectsRight && intersectsTop) 
				{
					if (xStart > rect.right.xStart) 
					{
						return new SegmentF(this.Intersection(rect.right), this.Intersection(rect.top));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.top), this.Intersection(rect.right));
					}
				}
				if (intersectsRight && intersectsBottom) 
				{
					if (xStart > rect.right.xStart) 
					{
						return new SegmentF(this.Intersection(rect.right), this.Intersection(rect.bottom));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.bottom), this.Intersection(rect.right));
					}
				}
				if (intersectsTop && intersectsBottom) 
				{
					if (this.yStart < rect.top.yStart) 
					{
						return new SegmentF(this.Intersection(rect.top), this.Intersection(rect.bottom));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.bottom), this.Intersection(rect.top));
					}
				}
			} 
			// One intersection, cut one end down.  Again, make sure to return 
			// a result in the same vector direction.
			else if (intersections == 1) 
			{
				PointF inside;
				bool startInside;
				if (rect.Contains(new PointF(this.xStart, this.yStart))) 
				{
					inside = new PointF(this.xStart, this.yStart);
					startInside = true;
				}
				else if (rect.Contains(new PointF(this.xEnd, this.yEnd))) 
				{
					inside = new PointF(this.xEnd, this.yEnd);
					startInside = false;
				} 
				else 
				{
					this.Intersection(rect.left);
					this.Intersection(rect.right);
					this.Intersection(rect.top);
					this.Intersection(rect.bottom);
					throw new Exception("Intersects only one side, but no points inside?");
				}

				if (intersectsLeft) 
				{
					if (startInside) 
					{
						return new SegmentF(inside, this.Intersection(rect.left));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.left), inside);
					}
				} 
				else if (intersectsRight) 
				{
					if (startInside) 
					{
						return new SegmentF(inside, this.Intersection(rect.right));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.right), inside);
					}
				}
				else if (intersectsTop) 
				{
					if (startInside) 
					{
						return new SegmentF(inside, this.Intersection(rect.top));
					} 
					else 
					{
						return new SegmentF(this.Intersection(rect.top), inside);
					}
				}
				else if (intersectsBottom) 
				{
					if (startInside) 
					{
						return new SegmentF(inside, this.Intersection(rect.bottom));
					} 
					else  
					{
						return new SegmentF(this.Intersection(rect.bottom), inside);
					}
				}
			}

			// None of the above conditions hold - throw an error.
//			Console.WriteLine("Line {0} {1} to {2} {3}", xStart, yStart, xEnd, yEnd);
//			Console.WriteLine("Rectangle {0} {1} to {2} {3}", rect.left.StartX, rect.left.StartY, 
//				rect.right.EndX, rect.right.EndY);
//			Console.WriteLine("Left? {0} Top? {1} Right? {2} Bottom? {3}", intersectsLeft, intersectsTop, 
//				intersectsBottom, intersectsBottom);
//			Console.WriteLine("Contains? {0}, {1}", rect.Contains(new PointF(this.xStart, this.yStart)),
//				rect.Contains(new PointF(this.xEnd, this.yEnd)));
			throw new ApplicationException("Intersection error");
		}

		/// <summary>
		/// Tells whether any part of this line falls within the given
		/// rectangle.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public bool Intersects(GeomRectangleF rect) 
		{	
			return (rect.Contains(new PointF(this.xStart, this.yStart)) ||
				rect.Contains(new PointF(this.xEnd, this.yEnd)) ||
				Intersects(rect.left) || 
				Intersects(rect.right) || 
				Intersects(rect.top) || 
				Intersects(rect.bottom));
		}
		
		/// <summary>
		/// Tells whether this segment intersects the given segment
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public bool Intersects(SegmentF line) 
		{
			// Want to solve equations
			// a_x * (1 - s) + b_x * s = c_x * (1 - t) + d_x * t
			// and same with x replaced with y
			// a and b are end points of this line, c and d are endpoints of the other line
			float a_x = this.xStart;
			float a_y = this.yStart;
			float b_x = this.xEnd;
			float b_y = this.yEnd;
			float c_x = line.xStart;
			float c_y = line.yStart;
			float d_x = line.xEnd;
			float d_y = line.yEnd;

			// matrix form of above equations
			// A s + B t = C
			// D s + E t = F
			float A = b_x - a_x;
			float B = c_x - d_x;
			float C = c_x - a_x;

			float D = b_y - a_y;
			float E = c_y - d_y;
			float F = c_y - a_y;

			// matrix goes to 
			// AD s + BD t = CD
			// DA s + EA t = FA
			float s, t;
			if (A == 0) 
			{
				t = C / B;
				s = (F - E * t) / D;
			} 
			else if (D == 0) 
			{
				t = E / F;
				s = (C - B * t) / A;
			} 
			else 
			{
				float AD = A * D;
				float BD = B * D;
				float CD = C * D;
				float EA = E * A;
				float FA = F * A;
				// then subtracting gives the equation
				// t (BD - EA) = CD - FA
				t = (CD - FA) / (BD - EA);
				s = (CD - BD * t) / AD;
			}

			if (float.IsNaN(s)) 
			{
				return t >= 0 && t <= 1;
			}
			if (float.IsNaN(t)) 
			{
				return s >= 0 && s <= 1;
			}
			return t >= 0 && t <= 1 && s >= 0 && s <= 1;
		}
	}
}
