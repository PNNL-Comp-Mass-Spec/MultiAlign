namespace MultiAlignRogue.Clustering
{
    using System.Drawing;

    using PNNLOmics.Data.Features;

    using QuadTreeLib;

    public class ClusterPoint : IHasRect
    {
        public ClusterPoint(UMCClusterLight umcClusterLight)
        {
            this.UMCClusterLight = umcClusterLight;
            this.Rectangle = new RectangleF
            {
                X = (float)umcClusterLight.NetAligned,
                Y = (float)umcClusterLight.MassMonoisotopicAligned
            };
        }

        public UMCClusterLight UMCClusterLight { get; private set; }

        public RectangleF Rectangle { get; private set; }
    }
}
