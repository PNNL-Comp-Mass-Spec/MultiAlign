using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue
{
    using System.Drawing;

    using PNNLOmics.Data.Features;

    using QuadTreeLib;

    class FeaturePoint : IHasRect
    {
        public FeaturePoint(UMCLight umcLight)
        {
            this.UMCLight = umcLight;
            this.Rectangle = new RectangleF
            {
                X = umcLight.ScanStart,
                Y = (float)umcLight.MassMonoisotopic,
                Width = umcLight.ScanEnd - umcLight.ScanStart,
                Height = 1
            };
        }

        public UMCLight UMCLight { get; private set; }

        public RectangleF Rectangle { get; private set; }
    }
}
