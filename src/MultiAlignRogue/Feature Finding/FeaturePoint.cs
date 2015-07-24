namespace MultiAlignRogue.Feature_Finding
{
    using System;
    using System.Drawing;
    using System.Linq;

    using MultiAlignCore.Data.MetaData;

    using PNNLOmics.Data.Features;

    using QuadTreeLib;

    class FeaturePoint : IHasRect
    {
        public delegate float GetNet(int scan);

        public FeaturePoint(UMCLight umcLight, GetNet getNet)
        {
            this.UMCLight = umcLight;

            var etStart = getNet(umcLight.ScanStart);
            var etEnd = getNet(umcLight.ScanEnd);

            this.Rectangle = new RectangleF
            {
                X = etStart,
                Y = (float)umcLight.MassMonoisotopicAligned,
                Width = etEnd - etStart,
                Height = 1
            };
        }

        public UMCLight UMCLight { get; private set; }

        public RectangleF Rectangle { get; private set; }
    }
}
