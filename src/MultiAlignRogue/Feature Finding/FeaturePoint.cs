namespace MultiAlignRogue.Feature_Finding
{
    using System.Drawing;
    using System.Linq;

    using MultiAlignCore.Data.MetaData;

    using PNNLOmics.Data.Features;

    using QuadTreeLib;

    class FeaturePoint : IHasRect
    {
        public FeaturePoint(UMCLight umcLight, DatasetInformation dataset)
        {
            this.UMCLight = umcLight;

            var etStart = (float)this.GetNet(dataset, umcLight.ScanStart);
            var etEnd = (float)this.GetNet(dataset, umcLight.ScanEnd);

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

        private double GetNet(DatasetInformation dataset, int scan)
        {
            var minScan = dataset.ScanTimes.Keys.Min();
            var minEt = dataset.ScanTimes[minScan];

            var maxScan = dataset.ScanTimes.Keys.Max();
            var maxEt = dataset.ScanTimes[maxScan];

            var et = dataset.ScanTimes[scan];

            return (et - minEt) / (maxEt - minEt);
        }
    }
}
