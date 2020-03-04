using System.Windows;
using FeatureAlignment.Data.Features;
using QuadTreeLib;

namespace MultiAlignRogue.Feature_Finding
{
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

        public Rect Rectangle { get; private set; }

        private void SetRectangle(bool aligned)
        {
            if (aligned)
            {
                this.Rectangle = new Rect
                {
                    X = this.UMCLight.NetAligned,
                    Y = this.UMCLight.MassMonoisotopicAligned,
                    Width = 0.01f,
                    Height = 1.0f
                };
            }
            else
            {
                var etStart = getNet(this.UMCLight.ScanStart);
                var etEnd = getNet(this.UMCLight.ScanEnd);

                this.Rectangle = new Rect
                {
                    X = etStart,
                    Y = this.UMCLight.MassMonoisotopic,
                    Width = etEnd - etStart,
                    Height = 1
                };
            }
        }
    }
}
