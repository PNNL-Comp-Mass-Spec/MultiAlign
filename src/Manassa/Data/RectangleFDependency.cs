using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;
using System.ComponentModel;

namespace Manassa.Data
{
    public class RectangleFDependency: DependencyObject
    {
        public RectangleF Rectangle
        {
            get;
            set;
        }
    }
}
