using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.Drawing;
using PNNLControls;

namespace MultiAlignCustomControls.Charting
{
    public class FeatureScatterPlot: ctlScatterChart
    {
        /// <summary>
        /// Gets or sets the x-axis shorthand value.
        /// </summary>
        public string XAxisShortHand
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the x-axis shorthand value.
        /// </summary>
        public string YAxisShortHand
        {
            get;
            set;
        }
        /// <summary>
        /// Draws the measurement from the start point to the end point on the given chart using the rectangle provided.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="rect"></param>
        /// <param name="bounds"></param>
        /// <param name="graphics"></param>
        protected override void DrawZoomValues(ctlChartBase chart,
                                                RectangleF rect,
                                                RectangleF bounds,
                                                Graphics graphics)
        {

            double ppmDiff = Math.Abs(Feature.ComputeMassPPMDifference(bounds.Top, bounds.Bottom));
            double netDiff = Convert.ToDouble(bounds.Width);

            DrawZoomValues(chart,
                            rect,
                            bounds,
                            string.Format("{0:.00} {1}", netDiff, XAxisShortHand),
                            string.Format("{0:.00} {1}", ppmDiff, YAxisShortHand),
                            graphics);
        }
        /// <summary>
        /// Draws the measurement from the start point to the end point on the given chart using the rectangle provided.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="rect"></param>
        /// <param name="bounds"></param>
        /// <param name="graphics"></param>
        protected void DrawZoomValues( ctlChartBase chart, 
                                                RectangleF rect,
                                                RectangleF bounds,
                                                string xString,
                                                string yString,
                                                Graphics graphics)
        {    
            if (graphics != null)
            {
                float x  = rect.X + rect.Width;
                float y  = rect.Y + rect.Height;
                float mx = rect.X + (rect.Width / 2);
                float my = rect.Y + (rect.Height / 2);

                SizeF ppmSize       = graphics.MeasureString(yString, Font);
                SizeF netSize       = graphics.MeasureString(xString, Font);
                float ppmX          = rect.X + rect.Width + 10;
                float ppmY          = my;
                float netX          = rect.X + (Math.Abs(rect.Width - netSize.Width) / 2);
                float netY          = rect.Y + rect.Height;

                using (Brush fillBrush = new SolidBrush(Color.FromArgb(216, BackColor)))
                {
                    graphics.FillRectangle(fillBrush,
                                            ppmX - 2,
                                            ppmY - 2,
                                            ppmSize.Width  + 4,
                                            ppmSize.Height + 4);

                    graphics.FillRectangle(fillBrush,
                                            netX - 2,
                                            netY - 2,
                                            netSize.Width  + 4,
                                            netSize.Height + 4);
                }

                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    graphics.DrawString(xString, Font, brush, netX, netY);
                    graphics.DrawString(yString, Font, brush, ppmX, ppmY);
                }
            }
        }
    }
}
