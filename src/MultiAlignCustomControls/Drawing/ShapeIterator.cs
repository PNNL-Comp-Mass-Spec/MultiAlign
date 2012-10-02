using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLControls;

namespace MultiAlignCustomControls.Drawing
{
    /// <summary>
    /// Iterates shapes
    /// </summary>
    public static class ShapeIterator
    {
        /// <summary>
        /// Creates a list of shapes
        /// </summary>
        /// <param name="hollow"></param>
        /// <returns></returns>
        public static List<clsShape> CreateShapeList(int size, bool hollow)
        {
            List<clsShape> shapes = new List<clsShape>();
            shapes.Add(new BubbleShape(size,    hollow));
            shapes.Add(new SquareShape(size,    hollow));
            shapes.Add(new TriangleShape(size,  hollow));
            shapes.Add(new PlusShape(size,      hollow));
            shapes.Add(new CrossShape(size,     hollow));
            return shapes;
        }
    }
}
