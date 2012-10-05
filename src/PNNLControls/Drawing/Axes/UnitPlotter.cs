using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PNNLControls
{
    /// <summary>
    /// Summary description for clsPlotterAxis.
    /// </summary>
    public class clsUnitPlotter
    {
        private Rectangle mrect_bounds = new Rectangle(0, 0, 1, 1);
        private Font mfnt_font = new Font("Microsoft Sans Serif", 10);
        private float mflt_max_font_size = 8;
        private float mflt_min_font_size = 8;
        private bool mbln_horizontal_orientation = true;
        private Pen mpen_unit = Pens.Black;
        private Pen mpen_axis = Pens.Black;
        private int mint_tick_percent = 20;
        internal float mflt_min_val = 0;
        internal float mflt_max_val = 1;
        private float mflt_max_num_without_scientific = 10000;
        private float mdbl_resolution = 1f;
        private float mflt_min_unit = 0.0001F;
        private int mint_labeling_interval;
        private bool mbln_is_inverted = false;
        private bool m_centerUnitBetweenTick;
        /// <summary>
        /// Flag indicating whether the units are discrete or not.
        /// </summary>
        private bool m_isDiscreteUnits;

        public clsUnitPlotter()
        {

        }

        public void SetRange(float min, float max)
        {            
            mflt_min_val = min;
            mflt_max_val = max;

            m_isDiscreteUnits       = false;
            m_centerUnitBetweenTick = false;
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
        /// <summary>
        /// Gets or sets whether the units are discrete or not.
        /// </summary>
        public bool IsDiscrete
        {
            get
            {
                return m_isDiscreteUnits;
            }
            set
            {
                m_isDiscreteUnits = value;
            }
        }
        /// <summary>
        /// Gets or sets whether the units are centered between ticks.
        /// </summary>
        public bool ShouldCenterUnitBetweenTick
        {
            get
            {
                return m_centerUnitBetweenTick;
            }
            set
            {
                m_centerUnitBetweenTick = value;
            }
        }
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
                return !this.mbln_horizontal_orientation;
            }
            set
            {
                this.mbln_horizontal_orientation = !value;
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
                return mrect_bounds;
            }
            set
            {
                this.mrect_bounds = value;
                // on setting the bounds set the font size.
            }
        }
        public Font Font
        {
            get
            {
                return this.mfnt_font;
            }
            set
            {
                this.mfnt_font = value;
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
                return (this.mflt_max_val > this.mflt_max_num_without_scientific);
            }
        }

        #region "Private Helpers"
        private string GetStringRepresentation(float val)
        {
            int decimal_place_to_see_till;
            int index_of_point;
            if (val == 0)
            {
                return "0";
            }

            if (!this.ScientificNotationRequired)
            {
                //       decimal_place_to_see_till = Convert.ToInt32(Math.Log10(resolution + resolution/2)) ; 
                decimal_place_to_see_till = Convert.ToInt32(Math.Floor(Math.Log10(this.mdbl_resolution + this.mdbl_resolution / 2)));
                index_of_point = val.ToString().IndexOf('.');
                if (index_of_point >= 0 && decimal_place_to_see_till < 0)
                {
                    return val.ToString("f" + (-1 * decimal_place_to_see_till).ToString());
                }
                else
                {
                    return val.ToString();
                }
            }

            decimal_place_to_see_till = Convert.ToInt32(Math.Floor(Math.Log10(this.mdbl_resolution)));
            index_of_point = val.ToString("f1").IndexOf('.');
            if (index_of_point >= 0)
                index_of_point = index_of_point - decimal_place_to_see_till;
            else
            {
                index_of_point = Convert.ToInt32(Math.Floor(Math.Log10(Math.Abs(val))));
                index_of_point -= decimal_place_to_see_till;
            }

            string scientific_str = val.ToString("e" + index_of_point.ToString());
            // the scientific_str has extra 0's after the exponent sign that 
            // we want to remove. 
            int index = scientific_str.IndexOf('e') + 2; // for the + or - 
            if (index == -1)
                return scientific_str;

            int end_zero_index = index + 1;
            while (end_zero_index < scientific_str.Length)
            {
                if (scientific_str[end_zero_index] != '0')
                    break;
                end_zero_index++;
            }
            string scientific_str_formatted = scientific_str.Substring(0, index);
            return scientific_str_formatted + scientific_str.Substring(end_zero_index);
        }

        private float GetFontSizeForText(Graphics g, string txt, ref SizeF str_size)
        {
            float font_size = this.mflt_max_font_size;
            Font font;
            while (font_size >= this.mflt_min_font_size)
            {
                font = new Font(this.Font.FontFamily, font_size, this.Font.Style);
                str_size = g.MeasureString(txt, font);
                if (!this.IsVertical)
                {
                    if (str_size.Height < (Bounds.Height * (100 - this.mint_tick_percent)) / 100)
                        return font_size;
                }
                else
                {
                    if (str_size.Width < (Bounds.Width * (100 - this.mint_tick_percent)) / 100)
                        return font_size;
                }
                font_size -= 1F;
            }
            return font_size;
            //return this.Font.Size;
        }
        private float SetFontForGraphics(Graphics g)
        {
            if (this.mflt_min_val >= this.mflt_max_val)
                return this.Font.Size;

            float min_font_size = float.MaxValue;

            SizeF val_size = new SizeF(0, 0);
            //Debug.WriteLine("Min " + this.mflt_min_unit + "  Max " + this.mflt_max_val + "  Resolution" + Convert.ToSingle(this.mdbl_resolution));
            //Debug.WriteLine("Iterations " + (this.mflt_max_val - this.mflt_min_unit) / Convert.ToSingle(this.mdbl_resolution));
            //int iterations = (int) ((this.mflt_max_val - this.mflt_min_unit) / Convert.ToSingle(this.mdbl_resolution));
            //for(int i = 0; i <= iterations; i++)
            //{
            foreach (float val in this.TickMarks)
            {
                //float val = this.mflt_min_unit + i * Convert.ToSingle(this.mdbl_resolution);
                //Debug.WriteLine("Trying loop " + val % .001);
                string val_str = GetStringRepresentation(val);
                float fnt_size = this.GetFontSizeForText(g, val_str, ref val_size);
                if (fnt_size < min_font_size)
                    min_font_size = fnt_size;
            }
            //}
            //this.mflt = min_font_size ; 
            return min_font_size;
        }
        private void SetLabelingInterval(Graphics g)
        {
            //Console.WriteLine("Resolution: {0}", Convert.ToSingle(this.mdbl_resolution));
            if (this.mflt_min_val == this.mflt_max_val)
                return;
            //this.Font = new Font(this.Font.FontFamily, this.mflt_font_size) ; 

            // now with min font size, set the labeling interval.
            string first_val_str = GetStringRepresentation(mflt_min_unit) + "12";

            SizeF first_val_size = new SizeF();
            this.GetFontSizeForText(g, first_val_str, ref first_val_size);

            mint_labeling_interval = 1;

            float val = this.mflt_min_unit;
            float next_val;
            //Console.WriteLine("Setting interval {0} {1}", this.mdbl_resolution, this.mint_labeling_interval);
            for (int i = 0; val < mflt_max_val; i += this.mint_labeling_interval)
            {
                val = Convert.ToSingle(this.mflt_min_val + i * this.mdbl_resolution * this.mint_labeling_interval);
                next_val = Convert.ToSingle(this.mflt_min_val + (i + 1) * this.mdbl_resolution * this.mint_labeling_interval);
                RectangleF rect = GetUnitBounds(val, next_val);
                if (this.IsVertical)
                {
                    if (rect.Height < first_val_size.Height)
                    {
                        mint_labeling_interval++;
                    }
                    //					else
                    //					{
                    //						val = next_val ; 
                    //					}
                }
                else
                {
                    if (rect.Width < first_val_size.Width)
                    {
                        mint_labeling_interval++;
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
                return;


            Int32 log_unit_width = Convert.ToInt32(Math.Floor(Math.Log10(mflt_max_val - mflt_min_val)));
            float unit_size = Convert.ToSingle(Math.Pow(10.0, Convert.ToDouble(log_unit_width)));
            Int32 num_grades = Convert.ToInt32(Math.Floor((mflt_max_val - mflt_min_val) / unit_size));

            // Force Discrete!
            //if (m_isDiscreteUnits)
            //{
            //    unit_size = Math.Round(unit_size, 0);
            //}
            
            if (num_grades > 8)
            {
                unit_size = 2 * unit_size;
                num_grades = num_grades / 2;
            }
            else if (num_grades < 4 && num_grades >= 2)
            {
                unit_size = unit_size / 2;
                num_grades = num_grades * 2;
            }
            else if (num_grades == 1)
            {
                unit_size = unit_size / 4;
                num_grades = num_grades * 4;
            }

            this.mflt_min_unit = Convert.ToSingle(unit_size) * Convert.ToInt32(Math.Floor(this.mflt_min_val / Convert.ToSingle(unit_size))) + Convert.ToSingle(unit_size);
            this.mdbl_resolution = unit_size;
            if (this.mdbl_resolution == 0)
            {
                this.mdbl_resolution = float.Epsilon;
            }
        }
        public float SetTickAndFont(Graphics g)
        {
            mint_labeling_interval = 1;
            SetFirstTickVal();
            float val = SetFontForGraphics(g);
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
            DrawAxis(g);
            DrawUnits(g, c);
        }
        private void DrawAxis(Graphics g)
        {
            if (IsVertical)
            {
                g.DrawLine(this.mpen_axis, Bounds.Right, Bounds.Top, Bounds.Right, Bounds.Bottom);
            }
            else
            {
                g.DrawLine(this.mpen_axis, Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Top);
            }            
        }    
        private void DrawUnits(Graphics g, Color c)
        {
            Brush unit_brush = new SolidBrush(c);
            StringFormat strTextFormat = new System.Drawing.StringFormat();
            strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;
            strTextFormat.Alignment = System.Drawing.StringAlignment.Center;
            strTextFormat.LineAlignment = System.Drawing.StringAlignment.Center;

            Rectangle bounds = Bounds;
            int iterations = (int)((this.mflt_max_val - this.mflt_min_unit) / (Convert.ToSingle(this.mdbl_resolution)));
            
            foreach (float val in this.TickMarks)
            {
                //draw the ticks on the axis
                Point pt_Plot = GetPointCoordinate(val);
                //draw units on the axis
                string val_str = GetStringRepresentation(val);

                if (!this.IsVertical)
                {
                    int tick_width = (bounds.Height * this.mint_tick_percent) / 100;
                    g.DrawLine(mpen_unit, pt_Plot.X, pt_Plot.Y, pt_Plot.X, pt_Plot.Y + tick_width);
                    RectangleF unit_bounds = GetUnitBounds(val, val + Convert.ToSingle(mdbl_resolution) * mint_labeling_interval);
                    g.DrawString(val_str, Font, unit_brush, unit_bounds, strTextFormat);
                }
                else
                {
                    int tick_width = (bounds.Width * this.mint_tick_percent) / 100;
                    g.DrawLine(mpen_unit, pt_Plot.X, pt_Plot.Y, pt_Plot.X - tick_width, pt_Plot.Y);
                    RectangleF unit_bounds = GetUnitBounds(val, val + Convert.ToSingle(mdbl_resolution) * mint_labeling_interval);
                    g.DrawString(val_str, Font, unit_brush, unit_bounds, strTextFormat);
                }
            }
            unit_brush.Dispose();
        }


        #endregion

        #region "Positioning and bounds"
        private Point GetPointCoordinate(float val)
        {
            Rectangle bounds = Bounds;
            if (!this.IsVertical)
            {
                float range = this.mflt_max_val - this.mflt_min_val;
                float range_per_pixel = range / this.Bounds.Width;
                float xcoordinate = (val - this.mflt_min_val) / range_per_pixel + Bounds.Left;
                //float xcoordinate = (this.mflt_max_val-val) / range_per_pixel + Bounds.Left;
                return new Point((int)xcoordinate, bounds.Top);
            }
            else
            {
                float range = this.mflt_max_val - this.mflt_min_val;
                float range_per_pixel = range / this.Bounds.Height;

                float ycoordinate = 0;
                if (mbln_is_inverted)
                    ycoordinate = this.Bounds.Height -
                        (this.mflt_max_val - val) / range_per_pixel + Bounds.Top;
                else
                    ycoordinate = this.Bounds.Height -
                    (val - this.mflt_min_val) / range_per_pixel + Bounds.Top;
                return new Point(bounds.Right, (int)ycoordinate);
            }
        }
        private RectangleF GetUnitBounds(float val, float next_val)
        {
            Point pt_Plot = this.GetPointCoordinate(val);
            Point pt_Plot_next = this.GetPointCoordinate(next_val);
            if (this.IsVertical)
            {
                int tick_width = (Bounds.Width * this.mint_tick_percent) / 100;
                int y_width = Math.Abs(pt_Plot_next.Y - pt_Plot.Y);

                if (m_centerUnitBetweenTick)
                {
                    y_width -= (y_width / 2);
                }
                return new RectangleF(Bounds.X, pt_Plot.Y - y_width / 2, Bounds.Width - tick_width, y_width);
            }
            else
            {
                int tick_width = (Bounds.Height * this.mint_tick_percent) / 100;
                int x_width = (pt_Plot_next.X - pt_Plot.X);
                if (m_centerUnitBetweenTick)
                {
                    x_width -= (x_width / 2);
                }
                return new RectangleF(pt_Plot.X - x_width / 2, Bounds.Top + tick_width, x_width, Bounds.Height - tick_width);
            }
        }
        #endregion
    }
}
