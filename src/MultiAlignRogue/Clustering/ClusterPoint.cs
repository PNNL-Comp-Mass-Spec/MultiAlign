using System.Windows;
using FeatureAlignment.Data.Features;
using OxyPlot.Series;
using QuadTreeLib;

namespace MultiAlignRogue.Clustering
{
    public class ClusterPoint : IHasRect, IScatterPointProvider
    {
        public ClusterPoint(UMCClusterLight umcClusterLight)
        {
            this.UMCClusterLight = umcClusterLight;
            this.Rectangle = new Rect
            {
                X = umcClusterLight.Net,
                Y = umcClusterLight.MassMonoisotopicAligned,
                Width = 0.00001f,
                Height = 0.01f
            };
        }

        public UMCClusterLight UMCClusterLight { get; }

        public Rect Rectangle { get; }

        public ScatterPoint GetScatterPoint()
        {
            return new ScatterPoint(this.UMCClusterLight.Net, this.UMCClusterLight.MassMonoisotopicAligned, 0.8);
        }
    }
}
