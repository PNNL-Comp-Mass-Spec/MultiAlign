using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PNNLControls
{
	public enum enumAxisType { enmxAxis = 0, enmyAxis};

	/// <summary>
	/// Summary description for clsPlotterAxis.
	/// </summary>
	/// 

	public class clsUnitPlotter
	{
		private Rectangle mrect_bounds = new Rectangle(0,0,1,1); 
		private Font mfnt_font = new Font("Microsoft Sans Serif", 10) ; 
		private float mflt_max_font_size = 8; 
		private float mflt_min_font_size = 8; 
		private bool mbln_horizontal_orientation = true; 
		private Pen mpen_unit = Pens.Black; 
		private Pen mpen_axis = Pens.Black; 
		//private Brush mbr_unit = Brushes.Black ; 
		private int mint_tick_percent = 20; 
		internal float mflt_min_val = 0; 
		internal float mflt_max_val = 1; 
		private float mflt_max_num_without_scientific = 10000; 
		private float mdbl_resolution =1f; 
		private float mflt_min_unit = 0.0001F; 
		private int mint_labeling_interval; 

		public clsUnitPlotter() 
		{

		}

		private bool mbln_is_inverted = false;
		public bool IsInverted
		{
			get
			{
				return this.mbln_is_inverted; 
			}
			set
			{
				this.mbln_is_inverted = value; 
			}
		}

		public bool IsVertical
		{
			get
			{
				return !this.mbln_horizontal_orientation ; 
			}
			set
			{
				this.mbln_horizontal_orientation = !value ; 
			}
		}

		public void SetRange(float min, float max)
		{
			//Console.WriteLine("Set range {0} {1}", min, max);
			mflt_min_val = min ; 
			mflt_max_val = max ; 
			//Console.WriteLine("Setting range: {0}, {1}", mflt_min_val, mflt_max_val);
		}

		/// <summary>
		/// Gets a list of chart coordinate values at which tick 
		/// marks are drawn.
		/// </summary>
		public System.Collections.IList TickMarks
		{
			get
			{
				IList list = new ArrayList();
				float val = this.mflt_min_unit;
				for (int i = 0; ; i++) 
				{
					list.Add(val);
					int mult = 1;
					float next_val;
					// make sure that the value actually increases
					do 
					{
						next_val = val + this.mdbl_resolution * this.mint_labeling_interval * mult;
						mult++;
					} while (next_val == val);
					val = next_val;
					if (next_val >= this.mflt_max_val) 
					{
						break;
					}
				}
//				for(float val = this.mflt_min_unit ; 
//					val < mflt_max_val ; 
//					val += Convert.ToSingle(mdbl_resolution) * mint_labeling_interval) 
//				{
//					list.Add(val);
//				}
				return list;
			}
		}

		public float MinRange 
		{
			get 
			{
				return this.mflt_min_val;
			}
		}

		public float MaxRange 
		{
			get 
			{
				return this.mflt_max_val;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return mrect_bounds ; 
			}
			set
			{
				this.mrect_bounds = value ; 
				// on setting the bounds set the font size.
			}
		}

		public Font Font 
		{
			get
			{
				return this.mfnt_font ; 
			}
			set
			{
				this.mfnt_font = value ; 
			}
		}
//
//		public float FontSize
//		{
//			get
//			{
//				return this.Font.Size;
//			}
//			set
//			{
//				this.mfnt_font = new Font(this.mfnt_font.FontFamily, value) ; 
//			}
//		}

		public bool ScientificNotationRequired
		{
			get
			{
				return (this.mflt_max_val > this.mflt_max_num_without_scientific) ; 
			}
		}

		#region "Private Helpers"
		private string GetStringRepresentation(float val)
		{
			int decimal_place_to_see_till ; 
			int index_of_point ; 
			if (val == 0) {
				return "0";
			}

			if (!this.ScientificNotationRequired)
			{
				//       decimal_place_to_see_till = Convert.ToInt32(Math.Log10(resolution + resolution/2)) ; 
				decimal_place_to_see_till = Convert.ToInt32(Math.Floor(Math.Log10(this.mdbl_resolution + this.mdbl_resolution/2))) ; 
				index_of_point = val.ToString().IndexOf('.') ;
				if (index_of_point >= 0 && decimal_place_to_see_till < 0)
				{
					return val.ToString("f" + (-1*decimal_place_to_see_till).ToString()) ; 
				}
				else
				{
					return val.ToString() ; 
				}
			}

			decimal_place_to_see_till = Convert.ToInt32(Math.Floor(Math.Log10(this.mdbl_resolution))) ; 
			index_of_point = val.ToString("f1").IndexOf('.') ;
			if (index_of_point >= 0)
				index_of_point = index_of_point - decimal_place_to_see_till ; 
			else
			{
				index_of_point = Convert.ToInt32(Math.Floor(Math.Log10(Math.Abs(val)))) ;
				index_of_point -= decimal_place_to_see_till ; 
			}

			string scientific_str = val.ToString("e" + index_of_point.ToString()) ; 
			// the scientific_str has extra 0's after the exponent sign that 
			// we want to remove. 
			int index = scientific_str.IndexOf('e') + 2 ; // for the + or - 
			if (index == -1)
				return scientific_str ; 

			int end_zero_index = index+1 ; 
			while(end_zero_index < scientific_str.Length)
			{
				if(scientific_str[end_zero_index] != '0')
					break ; 
				end_zero_index++ ; 
			}
			string scientific_str_formatted = scientific_str.Substring(0,index)  ;
			return scientific_str_formatted + scientific_str.Substring(end_zero_index) ; 
		}

		private float GetFontSizeForText(Graphics g, string txt, ref SizeF str_size)
		{
			float font_size = this.mflt_max_font_size ; 
			Font font;
			while(font_size >= this.mflt_min_font_size)
			{
				font = new Font(this.Font.FontFamily, font_size, this.Font.Style) ; 
				str_size = g.MeasureString(txt, font) ; 
				if (!this.IsVertical)
				{
					if (str_size.Height < (Bounds.Height * (100 - this.mint_tick_percent))/100)
						return font_size ; 
				}
				else
				{
					if (str_size.Width < (Bounds.Width * (100 - this.mint_tick_percent))/100)
						return font_size ; 
				}
				font_size -= 1F ; 
			}
			return font_size ;
			//return this.Font.Size;
		}


		private float SetFontForGraphics(Graphics g)
		{
			if (this.mflt_min_val >= this.mflt_max_val) 
				return this.Font.Size; 

			float min_font_size = float.MaxValue ; 

			SizeF val_size = new SizeF(0,0);
			//Debug.WriteLine("Min " + this.mflt_min_unit + "  Max " + this.mflt_max_val + "  Resolution" + Convert.ToSingle(this.mdbl_resolution));
			//Debug.WriteLine("Iterations " + (this.mflt_max_val - this.mflt_min_unit) / Convert.ToSingle(this.mdbl_resolution));
			//int iterations = (int) ((this.mflt_max_val - this.mflt_min_unit) / Convert.ToSingle(this.mdbl_resolution));
			//for(int i = 0; i <= iterations; i++)
			//{
			foreach (float val in this.TickMarks) 
			{
				//float val = this.mflt_min_unit + i * Convert.ToSingle(this.mdbl_resolution);
				//Debug.WriteLine("Trying loop " + val % .001);
				string val_str = GetStringRepresentation(val) ; 
				float fnt_size = this.GetFontSizeForText(g, val_str, ref val_size) ; 
				if (fnt_size < min_font_size)
					min_font_size = fnt_size ; 
			}
			//}
			//this.mflt = min_font_size ; 
			return min_font_size; 
		}

		private void SetLabelingInterval(Graphics g)
		{
			//Console.WriteLine("Resolution: {0}", Convert.ToSingle(this.mdbl_resolution));
			if (this.mflt_min_val == this.mflt_max_val)
				return ; 
			//this.Font = new Font(this.Font.FontFamily, this.mflt_font_size) ; 

			// now with min font size, set the labeling interval.
			string first_val_str = GetStringRepresentation(mflt_min_unit) + "12";

			SizeF first_val_size = new SizeF(); 
			this.GetFontSizeForText(g, first_val_str, ref first_val_size ) ; 
      
			mint_labeling_interval = 1 ; 

			float val = this.mflt_min_unit ;
			float next_val ;
			//Console.WriteLine("Setting interval {0} {1}", this.mdbl_resolution, this.mint_labeling_interval);
			for(int i = 0; val < mflt_max_val ; i += this.mint_labeling_interval)
			{
				val = Convert.ToSingle(this.mflt_min_val + i * this.mdbl_resolution * this.mint_labeling_interval);
				next_val = Convert.ToSingle(this.mflt_min_val + (i + 1) * this.mdbl_resolution * this.mint_labeling_interval);
				RectangleF rect = GetUnitBounds(val, next_val) ; 
				if (this.IsVertical)
				{
					if(rect.Height < first_val_size.Height)
					{
						mint_labeling_interval++ ;
					}
//					else
//					{
//						val = next_val ; 
//					}
				}
				else
				{
					if(rect.Width < first_val_size.Width)
					{
						mint_labeling_interval++  ; 
					}
//					else
//					{
//						val = next_val ; 
//					}
				}
//				if (next_val > mflt_max_val)
//					break ; 
			}
		}

		private void SetFirstTickVal()
		{
			//XWidth is this.Width - 2*mint_xmargin			
			if (this.mflt_max_val == this.mflt_min_val)
				return ; 

			Int32 log_unit_width = Convert.ToInt32(Math.Floor(Math.Log10(mflt_max_val - mflt_min_val))) ; 
			float unit_size = Convert.ToSingle(Math.Pow(10.0, Convert.ToDouble(log_unit_width))) ; 
			Int32 num_grades = Convert.ToInt32(Math.Floor((mflt_max_val - mflt_min_val) / unit_size)) ; 
			if (num_grades > 8)
			{
				unit_size = 2 * unit_size ; 
				num_grades = num_grades/2 ; 
			}
			else if (num_grades < 4 && num_grades >= 2)
			{
				unit_size = unit_size / 2 ; 
				num_grades = num_grades * 2 ; 
			} 
			else if (num_grades == 1)
			{
				unit_size = unit_size / 4 ; 
				num_grades = num_grades * 4 ; 
			}

			this.mflt_min_unit = Convert.ToSingle(unit_size) * Convert.ToInt32(Math.Floor(this.mflt_min_val/Convert.ToSingle(unit_size))) + Convert.ToSingle(unit_size) ; 
			this.mdbl_resolution = unit_size ; 
			if (this.mdbl_resolution == 0) 
			{
				this.mdbl_resolution = float.Epsilon;
			}
		}

		public float SetTickAndFont(Graphics g)
		{
			mint_labeling_interval = 1 ; 
			SetFirstTickVal() ; 
			float val = SetFontForGraphics(g) ;
			//SetLabelingInterval(g) ; 
			return val;
		}

		#endregion 

		#region "Drawing"

		public void Layout(Graphics g) 
		{
			// font is already decided, all we need is the labeling interval for this 
			// particular font and axis values.
			this.SetLabelingInterval(g);
		}

		public void Draw(Graphics g, Color c)
		{
			DrawAxis(g) ; 
			DrawUnits(g, c) ;
		}
		private void DrawAxis(Graphics g)
		{
			if (IsVertical)
			{
				g.DrawLine(this.mpen_axis, Bounds.Right, Bounds.Top, Bounds.Right, Bounds.Bottom) ; 
			}
			else
			{
				g.DrawLine(this.mpen_axis, Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Top) ; 
			}
			//Console.WriteLine("Bounds " + Bounds);
		}
//		public float GetBestFontSize(Graphics g)
//		{
//			float first_tick_val = 0 ;
//			double resolution = 0 ; 
//			int labeling_interval = 0 ;
//			if (this.mflt_max_val == this.mflt_min_val)
//				return 0; 
//			return this.Font.Size; 
//		}

		private void DrawUnits(Graphics g, Color c)
		{
			Brush unit_brush = new SolidBrush(c);
			StringFormat strTextFormat = new System.Drawing.StringFormat();
			strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip ; 
			strTextFormat.Alignment = System.Drawing.StringAlignment.Center;
			strTextFormat.LineAlignment = System.Drawing.StringAlignment.Center;				
      
			Rectangle bounds = Bounds ; 
			int iterations = (int) ((this.mflt_max_val - this.mflt_min_unit) / (Convert.ToSingle(this.mdbl_resolution)));
			//Console.WriteLine("Vals to draw: {0}", this.TickMarks.Count);



			foreach (float val in this.TickMarks)
			{
				//draw the ticks on the axis
				Point pt_Plot = GetPointCoordinate(val) ; 
				//draw units on the axis
				string val_str = GetStringRepresentation(val) ; 

				if (!this.IsVertical)
				{
					int tick_width = (bounds.Height * this.mint_tick_percent)/100 ; 
					g.DrawLine(mpen_unit, pt_Plot.X, pt_Plot.Y, pt_Plot.X, pt_Plot.Y + tick_width);
					RectangleF unit_bounds = GetUnitBounds(val, val + Convert.ToSingle(mdbl_resolution) * mint_labeling_interval) ; 
					g.DrawString(val_str, Font, unit_brush , unit_bounds, strTextFormat) ; 
				}
				else
				{
					int tick_width = (bounds.Width * this.mint_tick_percent)/100 ; 
					g.DrawLine(mpen_unit, pt_Plot.X, pt_Plot.Y, pt_Plot.X - tick_width, pt_Plot.Y);
					RectangleF unit_bounds = GetUnitBounds(val, val + Convert.ToSingle(mdbl_resolution) * mint_labeling_interval) ; 
					g.DrawString(val_str, Font, unit_brush, unit_bounds, strTextFormat);
				}      
			}
			unit_brush.Dispose();
		}


		#endregion

		#region "Positioning and bounds"
		private Point GetPointCoordinate(float val)
		{
			Rectangle bounds = Bounds ; 
			if (!this.IsVertical)
			{
				float range = this.mflt_max_val - this.mflt_min_val;
				float range_per_pixel = range / this.Bounds.Width;
				float xcoordinate = (val - this.mflt_min_val) / range_per_pixel + Bounds.Left;
				//float xcoordinate = (this.mflt_max_val-val) / range_per_pixel + Bounds.Left;
				return new Point((int) xcoordinate, bounds.Top);
			}
			else
			{
				float range = this.mflt_max_val - this.mflt_min_val;
				float range_per_pixel = range / this.Bounds.Height;

				float ycoordinate = 0;
				if (mbln_is_inverted)
					ycoordinate = this.Bounds.Height - 
						(this.mflt_max_val-val) / range_per_pixel + Bounds.Top;
				else
					ycoordinate = this.Bounds.Height - 
					(val - this.mflt_min_val) / range_per_pixel + Bounds.Top;
				return new Point(bounds.Right, (int) ycoordinate);
			}
		}
		private RectangleF GetUnitBounds(float val, float next_val)
		{
			Point pt_Plot = this.GetPointCoordinate(val) ; 
			Point pt_Plot_next = this.GetPointCoordinate(next_val) ; 
			if (this.IsVertical)
			{
				int tick_width = (Bounds.Width * this.mint_tick_percent)/100 ; 
				int y_width = Math.Abs(pt_Plot_next.Y - pt_Plot.Y) ; 
				return new RectangleF(Bounds.X, pt_Plot.Y - y_width/2, Bounds.Width - tick_width, y_width) ; 
			}
			else
			{
				int tick_width = (Bounds.Height * this.mint_tick_percent)/100 ; 
				int x_width = (pt_Plot_next.X - pt_Plot.X) ; 
				return new RectangleF(pt_Plot.X - x_width/2, Bounds.Top + tick_width, x_width, Bounds.Height - tick_width) ; 
			}
		}
		#endregion
	}

	public class clsLabelPlotter
	{
		float max_font_size = 50 ; 
		float min_font_size = 6 ; 
		private Rectangle mrect_bounds = new Rectangle(0,0,0,0) ; 
		private Font mfnt_font = new Font("Microsoft Sans Serif", 10) ; 
		private bool mbln_horizontal_orientation = true ; 
		private bool mbln_autosize = false;
		private string mstr_label ; 
		private System.Drawing.StringAlignment malgn_lineAlignment = System.Drawing.StringAlignment.Center;
		private System.Drawing.StringAlignment malgn_alignment = System.Drawing.StringAlignment.Center;

		public bool IsVertical
		{
			get
			{
				return !this.mbln_horizontal_orientation ; 
			}
			set
			{
				this.mbln_horizontal_orientation = !value ; 
			}
		}

		public clsLabelPlotter()
		{

		}

		public Rectangle Bounds
		{
			get
			{
				return mrect_bounds ; 
			}
			set
			{
				this.mrect_bounds = value ; 
			}
		}

		public float MaxFontSize 
		{
			get 
			{
				return this.max_font_size;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MaxFontSize");
				}
				this.max_font_size = value;
			}
		}

		public float MinFontSize 
		{
			get 
			{
				return this.min_font_size;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MinFontSize");
				}
				this.min_font_size = value;
			}
		}

		public bool AutoSize 
		{
			get 
			{
				return this.mbln_autosize;
			}
			set 
			{
				this.mbln_autosize = value;
			}
		}

		public System.Drawing.StringAlignment LineAlignment 
		{
			get 
			{
				return this.malgn_lineAlignment;
			}
			set 
			{
				this.malgn_lineAlignment = value;
			}
		}

		public System.Drawing.StringAlignment Alignment
		{
			get 
			{
				return this.malgn_alignment;
			}
			set 
			{
				this.malgn_alignment = value;
			}
		}

		public string Label
		{
			get
			{
				return this.mstr_label ; 
			}
			set
			{
				this.mstr_label = value ; 
			}
		}

		public Font Font 
		{
			get
			{
				return this.mfnt_font ; 
			}
			set
			{
				this.mfnt_font = value ; 
			}
		}

		public float FontSize
		{
			get
			{
				return this.mfnt_font.Size ; 
			}
			set
			{
				this.mfnt_font = new Font(mfnt_font.FontFamily, value, mfnt_font.Style);
			}
		}

		public void Layout(Graphics g)
		{
			// on setting the bounds set the font size.
			if (this.Label == null || this.Label == "")
				return ;
			if (this.AutoSize) 
			{
				float size = this.GetBestFontSize(g);
				this.Font = new Font(this.Font.FontFamily, size, this.Font.Style);
			}
		}

		public float GetBestFontSize(Graphics g) 
		{
			float font_size = max_font_size ; 
			while (true && font_size >= min_font_size)
			{
				Font font = new Font(this.mfnt_font.FontFamily, font_size, this.mfnt_font.Style);
				SizeF str_size = GetTextSize(g, font, this.Bounds.Width);
				if (str_size.Width < this.Bounds.Width && str_size.Height < this.Bounds.Height)
				{
					return font_size;
				}
				else
				{
					font_size -= 1F ; 
				}
			}
			return min_font_size;
		}

		private SizeF GetTextSize(Graphics g, Font f, int width) 
		{
			StringFormat strTextFormat = new System.Drawing.StringFormat();
			if (!mbln_horizontal_orientation)
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical ;
			else
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;

			strTextFormat.Alignment = this.Alignment;
			strTextFormat.LineAlignment = this.LineAlignment;
			return g.MeasureString(Label, f, width, strTextFormat) ; 
		}

		public SizeF GetTextSizeForWidth(Graphics g, int width) 
		{
			return GetTextSize(g, this.Font, width);
		}

		public void Draw(Graphics g, Color c)
		{
			Brush brush = new SolidBrush(c);
			// on setting the bounds set the font size.
			if (this.Label == null || this.Label == "")
				return ; 
			// Y Axis label
			StringFormat strTextFormat = new System.Drawing.StringFormat();
			if (!mbln_horizontal_orientation)
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical ;
			else
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip ; 
			strTextFormat.Alignment = this.Alignment;
			strTextFormat.LineAlignment = this.LineAlignment;

			if (!mbln_horizontal_orientation)
			{
				RectangleF newBounds = new RectangleF(- mrect_bounds.Width/2, -1* mrect_bounds.Height/2,
					mrect_bounds.Width, mrect_bounds.Height) ; 
				g.TranslateTransform(mrect_bounds.Left + mrect_bounds.Width/2, mrect_bounds.Top + mrect_bounds.Height/2) ; 
				g.RotateTransform(180) ; 
				g.DrawString(this.mstr_label, mfnt_font, brush, newBounds, strTextFormat);
				g.ResetTransform() ; 
			}
			else
			{
				g.DrawString(this.mstr_label, mfnt_font, brush, mrect_bounds, strTextFormat);
			}
			brush.Dispose();
		}				
	}
    /// <summary>
    /// Class that handles the axis bounds and plotting.
    /// </summary>
	public class clsPlotterAxis
	{
		private Rectangle m_chartBounds;
		private Int32 mint_XAxisMargin = 0;
		private Int32 mint_YAxisMargin = 0;
		// width from the left taken up by the ylabel.
		private Int32 mint_ylabel_percent = 40 ; 
		// width from the axis taken up by the y units
		private Int32 mint_yunit_percent = 60 ; 
		// height from the bottom tak up by the x label.
		private Int32 mint_xlabel_percent = 40 ; 
		// height from the axis taken up by the x units.
		private Int32 mint_xunit_percent = 60 ; 
        private Font m_font = new Font("Microsoft Sans Serif", 10);
		private int m_minFontSize = 8;
		private int m_maxFontSize = 15;

		private bool m_autoSizeFonts = true;

		private clsUnitPlotter mobj_yUnitPlotter = null; 
		private clsLabelPlotter mobj_yLabelPlotter = null ; 
		private clsUnitPlotter mobj_xUnitPlotter = null; 
		private clsLabelPlotter mobj_xLabelPlotter = null ;

		private Rectangle m_bounds;

		#region "Public Properties"

		public int MinFontSize 
		{
			get 
			{
				return this.m_minFontSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MinFontSize");
				}
				this.m_minFontSize = value;
			}
		}

		public int MaxFontSize 
		{
			get 
			{
				return this.m_maxFontSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MaxFontSize");
				}
				this.m_maxFontSize = value;
			}
		}
		
		public float FontSize
		{
			get
			{
				return this.m_font.Size ; 
			}
			set
			{
				if (value != this.m_font.Size)
				{
					this.m_font = new Font(m_font.FontFamily, value) ; 
				}
			}
		}
		public float XMinValue
		{
			get
			{
				return this.mobj_xUnitPlotter.MinRange;
			}
		}

		public float XMaxValue
		{
			get
			{
				return this.mobj_xUnitPlotter.MaxRange;
			}
		}

		public  float YMinValue
		{
			get
			{
				return this.mobj_yUnitPlotter.MinRange;
			}
		}

		public  float YMaxValue
		{
			get
			{
				return this.mobj_yUnitPlotter.MaxRange;
			}
		}

		public Font LabelAndUnitFont 
		{
			get
			{
				return this.m_font;
			}
			set 
			{
				this.m_font = value;
			}
		}

		public bool AutoSizeFonts 
		{
			get 
			{
				return m_autoSizeFonts;
			}
			set 
			{
				this.m_autoSizeFonts = value;
			}
		}


		public string XAxisLabel
		{			
			get
			{
				return this.mobj_xLabelPlotter.Label ; 
			}
			set
			{
				this.mobj_xLabelPlotter.Label = value;
			}
		}

		public string YAxisLabel
		{
			get
			{
				return this.mobj_yLabelPlotter.Label ;
			}
			set
			{
				this.mobj_yLabelPlotter.Label = value;
			}
		}

		public Rectangle ChartAreaBounds 
		{
			get 
			{
				return this.m_chartBounds;
			}
		}

		public  Int32 XPixelMargin
		{
			get
			{
				return mint_XAxisMargin;
			}
		}

		public  Int32 YPixelMargin
		{
			get
			{
				return mint_YAxisMargin;
			}
		}

		public Rectangle Bounds 
		{
			get 
			{
				return this.m_bounds;
			}
			set 
			{
				this.m_bounds = value;
			}
		}


		public Rectangle XLabelBounds
		{
			get
			{
				return this.mobj_xLabelPlotter.Bounds ; 
			}
			set
			{
				this.mobj_xLabelPlotter.Bounds = value ; 
			}
		}
		public Rectangle XUnitBounds
		{
			get
			{
				return this.mobj_xUnitPlotter.Bounds ; 
			}
			set
			{
				this.mobj_xUnitPlotter.Bounds = value ; 
			}
		}

		public Rectangle YLabelBounds
		{
			get
			{
				return this.mobj_yLabelPlotter.Bounds ; 
			}
			set
			{
				this.mobj_yLabelPlotter.Bounds = value ; 
			}
		}
		public Rectangle YUnitBounds
		{
			get
			{
				return this.mobj_yUnitPlotter.Bounds ; 
			}
			set
			{
				this.mobj_yUnitPlotter.Bounds = value ; 
			}
		}

		public IList XTickMarks
		{
			get 
			{
				return this.mobj_xUnitPlotter.TickMarks;
			}
		}

		public IList YTickMarks 
		{
			get 
			{
				return this.mobj_yUnitPlotter.TickMarks;
			}
		}

		#endregion

		#region Public Methods

		public void SetRanges(float xMinData, float xMaxData, float yMinData, float yMaxData)
		{      
			// By default first set the xminunit values and ymintunit values to these people.
			this.mobj_xUnitPlotter.SetRange(xMinData, xMaxData) ; 
			this.mobj_yUnitPlotter.SetRange(yMinData, yMaxData) ;
			//Console.WriteLine("Setting ranges {0} {1} {2} {3}", xMinData, xMaxData, yMinData, yMaxData);
		}

//		public void SetLabels(string xAxisLabel, string yAxisLabel, string Title)
//		{
//			this.mobj_xLabelPlotter.Label = xAxisLabel;
//			this.mobj_yLabelPlotter.Label = yAxisLabel;
//			this.mobj_TitlePlotter.Label = Title;
//		}

		/// <summary>
		/// Sets the boundaries of the axes and internal charting area.
		/// </summary>
		/// <param name="xaxis_margin">Pixels of margin to the bottom of the x axis.</param>
		/// <param name="yaxis_margin">Pixels of margin to the left of the y axis.</param>
		/// <param name="width">Width of area into which axes should fit.</param>
		/// <param name="height">Height of area into which axes should fit.</param>
		/// <returns>The rectangle that in which the actual data of the chart 
		/// should be drawn.</returns>
//		private System.Drawing.Rectangle SetPlotArea(int xaxis_margin, int yaxis_margin, int width, int height) 
//		{
//			this.mint_XAxisMargin = xaxis_margin; 
//			this.mint_YAxisMargin = yaxis_margin; 
//
//			this.mint_height = height;
//			this.mint_width = width;
//
//			Rectangle xlabel_bounds = new Rectangle(this.YPixelMargin + this.Bounds.Left, this.mint_height - (this.XPixelMargin * this.mint_xlabel_percent)/100 + this.Bounds.Top,
//				this.mint_width - 2* this.YPixelMargin, (this.XPixelMargin * this.mint_xlabel_percent)/100) ;   
//
//			Rectangle xunit_bounds = new Rectangle(this.YPixelMargin + this.Bounds.Left, this.mint_height - this.XPixelMargin + this.Bounds.Top,
//				this.mint_width - 2* this.YPixelMargin, (this.XPixelMargin * this.mint_xunit_percent)/100) ;   
//        
//			Rectangle ylabel_bounds = new Rectangle(0 + this.Bounds.Left, this.XPixelMargin + this.Bounds.Top, (this.YPixelMargin * this.mint_ylabel_percent)/100, 
//				this.mint_height - 2*this.XPixelMargin) ;   
//
//			Rectangle yunit_bounds = new Rectangle((this.YPixelMargin * (100 - this.mint_yunit_percent))/100  + this.Bounds.Left, this.XPixelMargin + this.Bounds.Top, 
//				(this.mint_yunit_percent * this.YPixelMargin)/100 , this.mint_height - 2* this.XPixelMargin) ;   
//
//			//Rectangle title_bounds = new Rectangle(0 + this.Bounds.Left,0,this.mint_width, this.XPixelMargin) ; 
//
//			this.mobj_xLabelPlotter.Bounds = xlabel_bounds ; 
//			this.mobj_yLabelPlotter.Bounds = ylabel_bounds ; 
////			this.mobj_TitlePlotter.Bounds = title_bounds ;
//
//			this.mobj_xUnitPlotter.Bounds = xunit_bounds ; 
//			this.mobj_yUnitPlotter.Bounds = yunit_bounds ;
//			this.screenAreaPixelHeight = xunit_bounds.Top - yunit_bounds.Top;
//			this.screenAreaPixelWidth = xunit_bounds.Right - yunit_bounds.Right;
//			this.m_chartBounds = new System.Drawing.Rectangle(yunit_bounds.Right, yunit_bounds.Top, 
//				screenAreaPixelWidth, screenAreaPixelHeight);
//			return m_chartBounds;
//		}

//		public void SetPlotArea(ref Bitmap PlotArea, int xaxis_margin, int yaxis_margin)
//		{
//			//mbmpDrawingArea = PlotArea ; 
//			this.SetPlotArea(xaxis_margin, yaxis_margin, PlotArea.Width, PlotArea.Height);	
//		}

		/// <summary>
		/// Gets the x screen coordinate for an x chart coordinate
		/// </summary>
		/// <param name="xValue"></param> 
		/// <returns></returns>
		public float XScreenPixel(float xValue) 
		{
			float range = this.mobj_xUnitPlotter.mflt_max_val - this.mobj_xUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Width;
			return (xValue - this.mobj_xUnitPlotter.mflt_min_val) / range_per_pixel;
		}

		/// <summary>
		/// Gets the y screen coordinate for a y chart coordinate.
		/// </summary>
		/// <param name="yValue"></param>
		/// <returns></returns>
		public float YScreenPixel(float yValue) 
		{
			float range = this.mobj_yUnitPlotter.mflt_max_val - this.mobj_yUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Height;
			return this.m_chartBounds.Height - 
				(yValue - this.mobj_yUnitPlotter.mflt_min_val) / range_per_pixel;
		}

		/// <summary>
		/// Gets the number of pixels used on the screen for drawing a give width. 
		/// </summary>
		/// <param name="width">The width of the drawing area</param>
		/// <returns></returns>
		public float PixelWidth(float width)
		{
			float range = this.mobj_xUnitPlotter.mflt_max_val - this.mobj_xUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Width;
			return width / range_per_pixel;
		}

		/// <summary>
		/// Gets the number of pixels used on the screen for drawing a give height. 
		/// </summary>
		/// <param name="height">The height of the drawing area</param>
		/// <returns></returns>
		public float PixelHeight(float height)
		{
			float range = this.mobj_yUnitPlotter.mflt_max_val - this.mobj_yUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Height;
			return  height / range_per_pixel;
		}

		/// <summary>
		/// Gets the chart coordinate for a screen coordinate relative to the upper-left 
		/// of the chart area.
		/// </summary>
		/// <param name="xPixel"></param>
		/// <returns></returns>
		public float XChartCoordinate(int xPixel) 
		{
			float range = this.mobj_xUnitPlotter.mflt_max_val - this.mobj_xUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Width;
			return xPixel * range_per_pixel + this.mobj_xUnitPlotter.mflt_min_val;
		}

		public float YChartCoordinate(int yPixel) 
		{
			float range = this.mobj_yUnitPlotter.mflt_max_val - this.mobj_yUnitPlotter.mflt_min_val;
			float range_per_pixel = range / this.m_chartBounds.Height;
			return -yPixel * range_per_pixel + this.mobj_yUnitPlotter.mflt_max_val;
		}


		public ChartLocation GetChartLocation(Point p) 
		{
			ChartLocation location = ChartLocation.None;
			if (this.mobj_xLabelPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.XLabel;
			} 
			else if (this.mobj_xUnitPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.XAxisAndUnits;
			} 
			else if (this.mobj_yLabelPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.YLabel;
			} 
			else if (this.mobj_yUnitPlotter.Bounds.Contains(p)) 
			{
				location |= ChartLocation.YAxisAndUnits;
			} 
			else if (this.m_chartBounds.Contains(p)) 
			{
				location |= ChartLocation.ChartArea;
			}
			return location;
		}

		/// <summary>
		/// Tells whether the given chart coordinate is in the viewport 
		/// on the screen
		/// </summary>
		/// <param name="xValue"></param>
		/// <returns></returns>
		public bool XOnScreen(float xValue) 
		{
			return xValue >= this.XMinValue && xValue <= this.XMaxValue;
		}

		/// <summary>
		/// Tells whether the given chart coordinate is in the viewport 
		/// on the screen
		/// </summary>
		/// <param name="yValue"></param>
		/// <returns></returns>
		public bool YOnScreen(float yValue) 
		{
			return yValue >= this.YMinValue && yValue <= this.YMaxValue;
		}

		public void Layout(Graphics g, clsMargins margins) 
		{
			int leftMargin = margins.GetChartLeftMargin(Bounds.Width);
			int rightMargin = margins.GetDefaultMargin(Bounds.Width);
			int topMargin = margins.GetDefaultMargin(Bounds.Height);
			int bottomMargin = margins.GetChartBottomMargin(Bounds.Height);
			int left = this.Bounds.Left;
			int right = this.Bounds.Right;
			int top = this.Bounds.Top;
			int bottom = this.Bounds.Bottom;
			int width = this.Bounds.Width;
			int height = this.Bounds.Height;

			// compute bounds of various pieces of charting surface and axes
			Rectangle yLabelBounds = new Rectangle(left, top + topMargin, leftMargin * this.mint_ylabel_percent / 100, height - topMargin - bottomMargin);
			Rectangle yUnitBounds = new Rectangle(left + (leftMargin * (100 - this.mint_yunit_percent)) / 100, 
				top + topMargin, (leftMargin * this.mint_yunit_percent) / 100, height - topMargin - bottomMargin);
			Rectangle xUnitBounds = new Rectangle(left + leftMargin, bottom - bottomMargin, width - leftMargin - rightMargin, 
				(bottomMargin * this.mint_xunit_percent) / 100);
			Rectangle xLabelBounds = new Rectangle(left + leftMargin, bottom - (bottomMargin * (this.mint_xlabel_percent)) / 100, 
				width - leftMargin - rightMargin, (bottomMargin * (this.mint_xlabel_percent)) / 100);
			//Console.WriteLine("XLabelBounds {0}", xLabelBounds);
			this.m_chartBounds = new Rectangle(left + leftMargin, top + topMargin, width - leftMargin - rightMargin, 
				height - topMargin - bottomMargin);
			//Console.WriteLine("m_chartBounds {0}", m_chartBounds);

			// Set the bounds of the various pieces
			this.mobj_xLabelPlotter.Bounds = xLabelBounds;
			this.mobj_yLabelPlotter.Bounds = yLabelBounds;
			this.mobj_xUnitPlotter.Bounds = xUnitBounds;
			this.mobj_yUnitPlotter.Bounds = yUnitBounds;

			// Set the fonts of all pieces to the main font
			this.mobj_xUnitPlotter.Font = this.LabelAndUnitFont;
			this.mobj_xLabelPlotter.Font = this.LabelAndUnitFont;
			this.mobj_yUnitPlotter.Font = this.LabelAndUnitFont;
			this.mobj_yLabelPlotter.Font = this.LabelAndUnitFont;

			// Determine the best font size to use - the largest font that works
			// for all units and labels
			float current_y_font_size = this.mobj_yUnitPlotter.SetTickAndFont(g) ; 
			float current_x_font_size = this.mobj_xUnitPlotter.SetTickAndFont(g) ; 

			// If autosizing fonts, create a new font using the best size
			if (this.AutoSizeFonts) 
			{
				float xlabel_font_size = this.mobj_xLabelPlotter.GetBestFontSize(g); 
				float ylabel_font_size = this.mobj_yLabelPlotter.GetBestFontSize(g); 

				// choose the largest fond that works for all units.
				float font_size = Math.Min(ylabel_font_size, xlabel_font_size) ; 
				font_size = Math.Min(this.MaxFontSize, font_size);
				font_size = Math.Max(this.MinFontSize, font_size);

				this.LabelAndUnitFont = new Font(this.LabelAndUnitFont.FontFamily, font_size, this.LabelAndUnitFont.Style);
			}
			// Set the font of all four label and unit parts to the same value -  
			// either the one that has been explicitly set, or the newly autosized one
			this.mobj_xUnitPlotter.Font = LabelAndUnitFont; 
			this.mobj_yUnitPlotter.Font = LabelAndUnitFont; 
			this.mobj_xLabelPlotter.Font = LabelAndUnitFont; 
			this.mobj_yLabelPlotter.Font = LabelAndUnitFont;

			//Console.WriteLine("Font Size {0}", this.LabelAndUnitFont.Size);
			// finish laying out the components now that the font size is fully known
			this.mobj_xUnitPlotter.Layout(g);
			this.mobj_yUnitPlotter.Layout(g);
			this.mobj_xLabelPlotter.Layout(g);
			this.mobj_yLabelPlotter.Layout(g);
		}

		public void Draw(Graphics g, Color foreColor)
		{  
			this.mobj_yUnitPlotter.Draw(g, foreColor) ; 
			this.mobj_xUnitPlotter.Draw(g, foreColor) ;
			this.mobj_xLabelPlotter.Draw(g, foreColor) ;
			this.mobj_yLabelPlotter.Draw(g, foreColor) ;
			//this.mobj_TitlePlotter.Draw(g) ; 
		}
		#endregion

		#region Private Helper Methods


//		private int GetCoordinate (float val, float min_val, float max_val, int start_coordinate, int width)
//		{
//			float float_width = Convert.ToSingle(width) ; 
//			float float_coordinate = float_width * (val - min_val) / (max_val - min_val) ;
//			Int32 int_coordinate = Convert.ToInt32(float_coordinate) ; 
//			return start_coordinate + int_coordinate ; 
//		}

		#endregion

		#region Constructor/Destructor

		public clsPlotterAxis()
		{
			this.mobj_xLabelPlotter = new clsLabelPlotter() ; 
			this.mobj_yLabelPlotter = new clsLabelPlotter() ; 
			this.mobj_xUnitPlotter = new clsUnitPlotter() ; 
			this.mobj_yUnitPlotter = new clsUnitPlotter() ;
//			this.mobj_TitlePlotter = new clsLabelPlotter() ;

			this.mobj_yLabelPlotter.IsVertical = true ; 
			this.mobj_yUnitPlotter.IsVertical = true ; 
		}
		#endregion
  
	}

	public class clsPlotSingleAxis
	{
		//private Int32 mint_YAxisMargin = 0;

		// height from the bottom tak up by the x label.
		// width from the left taken up by the ylabel.
		//private Int32 mint_ylabel_percent = 40 ; 

		// height from the axis taken up by the x units.
		//private Int32 mint_yunit_percent = 60 ; 

		private Font m_font = new Font("Microsoft Sans Serif", 10);
		private int m_minFontSize = 8;
		private int m_maxFontSize = 15;

		private bool m_autoSizeFonts = true;

		//private Bitmap mbmpDrawingArea = null;

		private clsUnitPlotter mobj_UnitPlotter = null; 
		private clsLabelPlotter mobj_LabelPlotter = null ; 

		private Rectangle m_bounds;

		#region "Public Properties"

		//private bool mbln_is_inverted = false;
		public bool IsInverted
		{
			get
			{
				return mobj_UnitPlotter.IsInverted; 
			}
			set
			{
				this.mobj_UnitPlotter.IsInverted = value; 

			}
		}

		public bool IsVertical
		{
			get
			{
				return mobj_LabelPlotter.IsVertical; 
			}
			set
			{
				mobj_LabelPlotter.IsVertical=value;
				mobj_UnitPlotter.IsVertical=value;
			}
		}


		public int MinFontSize 
		{
			get 
			{
				return this.m_minFontSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MinFontSize");
				}
				this.m_minFontSize = value;
			}
		}

		public int MaxFontSize 
		{
			get 
			{
				return this.m_maxFontSize;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Font size must be >= 0", "MaxFontSize");
				}
				this.m_maxFontSize = value;
			}
		}

		public  clsUnitPlotter UnitPlotter 
		{
			get	{return this.mobj_UnitPlotter;}
		}

		public  float YMaxValue
		{
			get
			{
				return this.mobj_UnitPlotter.MaxRange;
			}
		}

		public Font LabelAndUnitFont 
		{
			get
			{
				return this.m_font;
			}
			set 
			{
				this.m_font = value;
			}
		}

		public bool AutoSizeFonts 
		{
			get 
			{
				return m_autoSizeFonts;
			}
			set 
			{
				this.m_autoSizeFonts = value;
			}
		}

		public clsLabelPlotter LabelPlotter
		{
			get{return this.mobj_LabelPlotter;}
		}

		public Rectangle Bounds 
		{
			get{return this.m_bounds;}
			set{this.m_bounds = value;}
		}

		#endregion

		#region Public Methods
		
		public void Layout(Graphics g) 
		{
		
			Rectangle b = this.Bounds;
			
			// Set the bounds of the various pieces
			this.mobj_LabelPlotter.Bounds = this.Bounds;
			this.mobj_UnitPlotter.Bounds = this.Bounds;

			// Set the fonts of all pieces to the main font
			this.mobj_UnitPlotter.Font = this.LabelAndUnitFont;
			this.mobj_LabelPlotter.Font = this.LabelAndUnitFont;

			// Determine the best font size to use - the largest font that works
			// for all units and labels
			float min_font_size = this.mobj_UnitPlotter.SetTickAndFont(g) ;  
			float ylabel_font_size = this.mobj_LabelPlotter.GetBestFontSize(g); 

			if (ylabel_font_size < min_font_size)
				min_font_size = ylabel_font_size ;

			min_font_size = Math.Min(this.MaxFontSize, min_font_size);
			min_font_size = Math.Max(this.MinFontSize, min_font_size);

			// If autosizing fonts, create a new font using the best size
			if (this.AutoSizeFonts) 
			{
				this.LabelAndUnitFont = new Font(this.LabelAndUnitFont.FontFamily, min_font_size, this.LabelAndUnitFont.Style);
			}
			// Set the font of all label and unit parts to the same value -  
			// either the one that has been explicitly set, or the newly autosized one
			this.mobj_UnitPlotter.Font = LabelAndUnitFont; 
			this.mobj_LabelPlotter.Font = LabelAndUnitFont;

			// finish laying out the components now that the font size is fully known
			this.mobj_UnitPlotter.Layout(g);
			this.mobj_LabelPlotter.Layout(g);
		}

		public void Draw(Graphics g, Color foreColor)
		{  
			this.mobj_UnitPlotter.SetTickAndFont(g);
			this.mobj_UnitPlotter.Draw(g, foreColor) ; 
			this.mobj_LabelPlotter.Draw(g, foreColor) ;
			
		}
		#endregion


		
		#region Constructor/Destructor

		public clsPlotSingleAxis()
		{
			this.mobj_LabelPlotter = new clsLabelPlotter() ; 
			this.mobj_UnitPlotter = new clsUnitPlotter() ;
		}
		#endregion
  
	}
}
