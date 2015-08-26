using MultiAlignCore.Data.Features;

namespace MultiAlignRogue.Feature_Finding
{
    using System;
    using System.Drawing;
    using System.Linq;

    using MultiAlignCore.Data.MetaData;

    
    using QuadTreeLib;

    class FeaturePoint : IHasRect
    {
        public delegate float GetNet(int scan);

        private GetNet getNet;

        public FeaturePoint(UMCLight umcLight, GetNet getNet, bool aligned)
        {
            this.UMCLight = umcLight;
            this.getNet = getNet;
            this.SetRectangle(aligned);
        }

        public UMCLight UMCLight { get; private set; }

        public RectangleF Rectangle { get; private set; }

        private void SetRectangle(bool aligned)
        {
            if (aligned)
            {
                this.Rectangle = new RectangleF
                {
                    X = (float)this.UMCLight.NetAligned,
                    Y = (float)this.UMCLight.MassMonoisotopicAligned,
                    Width = 0.01f,
                    Height = 1.0f
                };
            }
            else
            {
                var etStart = getNet(this.UMCLight.ScanStart);
                var etEnd = getNet(this.UMCLight.ScanEnd);

                this.Rectangle = new RectangleF
                {
                    X = etStart,
                    Y = (float)this.UMCLight.MassMonoisotopic,
                    Width = etEnd - etStart,
                    Height = 1
                };
            }
        }
    }
}
