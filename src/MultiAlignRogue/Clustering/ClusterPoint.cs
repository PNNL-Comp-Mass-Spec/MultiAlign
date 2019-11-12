using FeatureAlignment.Data.Features;
using OxyPlot.Series;

namespace MultiAlignRogue.Clustering
{
    using System.Drawing;
    using QuadTreeLib;

    public class ClusterPoint : IHasRect, IScatterPointProvider
    {
        public ClusterPoint(UMCClusterLight umcClusterLight)
        {
            this.UMCClusterLight = umcClusterLight;
            this.Rectangle = new RectangleF
            {
                X = (float)umcClusterLight.Net,
                Y = (float)umcClusterLight.MassMonoisotopicAligned,
                Width = 0.00001f,
                Height = 0.01f
            };
        }

        public UMCClusterLight UMCClusterLight { get; }

        public RectangleF Rectangle { get; }

        public ScatterPoint GetScatterPoint()
        {
            return new ScatterPoint(this.UMCClusterLight.Net, this.UMCClusterLight.MassMonoisotopicAligned, 0.8);
        }
    }
}
