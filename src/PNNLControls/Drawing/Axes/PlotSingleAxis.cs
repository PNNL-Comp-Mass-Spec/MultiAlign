using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PNNLControls
{   
    public class clsPlotSingleAxis
    {
        private Font m_font;
        private int m_minFontSize;
        private int m_maxFontSize;
        private bool m_autoSizeFonts;
        private clsUnitPlotter mobj_UnitPlotter;
        private clsLabelPlotter mobj_LabelPlotter;
        private Rectangle m_bounds;

        #region Constructor/Destructor

        public clsPlotSingleAxis()
        {
            mobj_LabelPlotter = new clsLabelPlotter();
            mobj_UnitPlotter = new clsUnitPlotter();
            m_font = new Font("Microsoft Sans Serif", 10);
            m_minFontSize = 8;
            m_maxFontSize = 15;
            m_autoSizeFonts = true;
            //mobj_UnitPlotter = null;
            //mobj_LabelPlotter = null;
        }
        #endregion

        #region "Public Properties"
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
        /// <summary>
        /// Gets or sets whether to center the units between tick marks.
        /// </summary>
        public bool ShouldCenterUnits
        {
            get
            {
                return mobj_UnitPlotter.ShouldCenterUnitBetweenTick;
            }
            set 
            {
                mobj_UnitPlotter.ShouldCenterUnitBetweenTick = value;
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
                mobj_LabelPlotter.IsVertical = value;
                mobj_UnitPlotter.IsVertical = value;
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
        public clsUnitPlotter UnitPlotter
        {
            get { return this.mobj_UnitPlotter; }
        }
        public float YMaxValue
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
            get
            {
                return this.mobj_LabelPlotter;
            }
        }
        public Rectangle Bounds
        {
            get { return this.m_bounds; }
            set { this.m_bounds = value; }
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
            float min_font_size = this.mobj_UnitPlotter.SetTickAndFont(g);
            float ylabel_font_size = this.mobj_LabelPlotter.GetBestFontSize(g);

            if (ylabel_font_size < min_font_size)
                min_font_size = ylabel_font_size;

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
            this.mobj_UnitPlotter.Draw(g, foreColor);
            this.mobj_LabelPlotter.Draw(g, foreColor);
        }
        #endregion


    }
}
