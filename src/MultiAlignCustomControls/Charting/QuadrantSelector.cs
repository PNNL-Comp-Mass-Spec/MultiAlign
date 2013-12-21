using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MultiAlignCustomControls.Charting
{
    public class AnnotationPoint
    {
        public AnnotationPoint(float width, float height)
        {
            Annotations         = new List<AnnotationPoint>();            
            AnnotationSize      = new SizeF(width, height);
        }
        public AnnotationPoint():
            this (0,0)
        {
            
        }

        public void FindMaxSubSize()
        {
            var maxWidth  = Annotations.Max(xx => xx.AnnotationSize.Width);
            var maxHeight = Annotations.Max(xx => xx.AnnotationSize.Height);

            AnnotationSize = new SizeF(maxWidth, maxHeight);
        }

        public PointF Location { get; set; }
        public PointF Offset { get; set; }

        public List<AnnotationPoint> Annotations { get; set; }
        public string Annotation { get; set; }
        public Color Color { get; set; }
        public SizeF AnnotationSize { get; set; }
    }
}
