using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PNNLControls;

namespace MultiAlignCustomControls.Charting
{
    public class SeriesOptions
    {
        public SeriesOptions()
        {
            Color  = Color.Red;
            Shape  = new BubbleShape(1, false);
            Label  = "series";
            Points = new List<PointF>();
        }

        public clsShape Shape
        {
            get;
            set;
        }
        public Color Color
        {
            get;
            set;
        }
        public List<PointF> Points
        {
            get;
            set;
        }
        public string Label
        {
            get;
            set;
        }
    }
}
