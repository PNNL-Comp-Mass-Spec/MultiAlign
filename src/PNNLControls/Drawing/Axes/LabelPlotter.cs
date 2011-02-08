using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PNNLControls
{
    public class clsLabelPlotter
    {
        float max_font_size = 50;
        float min_font_size = 6;
        private Rectangle mrect_bounds = new Rectangle(0, 0, 0, 0);
        private Font mfnt_font = new Font("Microsoft Sans Serif", 10);
        private bool mbln_horizontal_orientation = true;
        private bool mbln_autosize = false;
        private string mstr_label;
        private System.Drawing.StringAlignment malgn_lineAlignment = System.Drawing.StringAlignment.Center;
        private System.Drawing.StringAlignment malgn_alignment = System.Drawing.StringAlignment.Center;

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

        public clsLabelPlotter()
        {

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
                return this.mstr_label;
            }
            set
            {
                this.mstr_label = value;
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

        public float FontSize
        {
            get
            {
                return this.mfnt_font.Size;
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
                return;
            if (this.AutoSize)
            {
                float size = this.GetBestFontSize(g);
                this.Font = new Font(this.Font.FontFamily, size, this.Font.Style);
            }
        }

        public float GetBestFontSize(Graphics g)
        {
            float font_size = max_font_size;
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
                    font_size -= 1F;
                }
            }
            return min_font_size;
        }

        private SizeF GetTextSize(Graphics g, Font f, int width)
        {
            StringFormat strTextFormat = new System.Drawing.StringFormat();
            if (!mbln_horizontal_orientation)
                strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical;
            else
                strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;

            strTextFormat.Alignment = this.Alignment;
            strTextFormat.LineAlignment = this.LineAlignment;
            return g.MeasureString(Label, f, width, strTextFormat);
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
                return;
            // Y Axis label
            StringFormat strTextFormat = new System.Drawing.StringFormat();
            if (!mbln_horizontal_orientation)
                strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical;
            else
                strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;
            strTextFormat.Alignment = this.Alignment;
            strTextFormat.LineAlignment = this.LineAlignment;

            if (!mbln_horizontal_orientation)
            {
                RectangleF newBounds = new RectangleF(-mrect_bounds.Width / 2, -1 * mrect_bounds.Height / 2,
                    mrect_bounds.Width, mrect_bounds.Height);
                g.TranslateTransform(mrect_bounds.Left + mrect_bounds.Width / 2, mrect_bounds.Top + mrect_bounds.Height / 2);
                g.RotateTransform(180);
                g.DrawString(this.mstr_label, mfnt_font, brush, newBounds, strTextFormat);
                g.ResetTransform();
            }
            else
            {
                g.DrawString(this.mstr_label, mfnt_font, brush, mrect_bounds, strTextFormat);
            }
            brush.Dispose();
        }
    }
}
