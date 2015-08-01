using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue.Clustering
{
    using System.Drawing;

    using GalaSoft.MvvmLight;

    using MultiAlignRogue.Utils;

    using OxyPlot;
    using OxyPlot.Axes;

    using PNNLOmics.Data.Features;

    using QuadTreeLib;

    public class ClusterPlotViewModel : ViewModelBase
    {
        /// <summary>
        /// For reducing plot updates when zooming.
        /// </summary>
        private readonly Throttler throttler;

        /// <summary>
        /// Net axis (x axis).
        /// </summary>
        private readonly LinearAxis netAxis;

        /// <summary>
        /// Mass axis (y axis).
        /// </summary>
        private readonly LinearAxis massAxis;

        /// <summary>
        /// QuadTree of clusters.
        /// </summary>
        private QuadTree<ClusterPoint> clusterTree;

        /// <summary>
        /// Number of divisions on mass axis.
        /// </summary>
        private int massAxisDivisions;

        /// <summary>
        /// Number of divisions on net axis.
        /// </summary>
        private int netAxisDivisions;

        /// <summary>
        /// Maximum number of clusters in a single division of the view.
        /// </summary>
        private int maxClustersPerDivision;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterPlotViewModel"/> class.
        /// </summary>
        /// <param name="clusters">The clusters to plot.</param>
        public ClusterPlotViewModel(List<UMCClusterLight> clusters)
        {
            this.massAxisDivisions = 100;
            this.netAxisDivisions = 20;
            this.maxClustersPerDivision = 50;

            this.throttler = new Throttler(TimeSpan.FromMilliseconds(100));
            this.BuildClusterTree(clusters);

            this.netAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                Minimum = 0,
                AbsoluteMaximum = 1.0,
                Maximum = 1.0
            };

            this.massAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                Minimum = 0
            };

            this.netAxis.AxisChanged += (s, e) => this.throttler.Run(this.BuildClusterPlot);
            this.massAxis.AxisChanged += (s, e) => this.throttler.Run(this.BuildClusterPlot);

            this.ClusterPlotModel = new PlotModel();
            this.ClusterPlotModel.Axes.Add(this.netAxis);
            this.ClusterPlotModel.Axes.Add(this.massAxis);

            this.throttler.Run(this.BuildClusterPlot);
        }

        /// <summary>
        /// Gets the plot model for cluster plot.
        /// </summary>
        public PlotModel ClusterPlotModel { get; private set; }

        /// <summary>
        /// Build cluster quad tree from list of clusters.
        /// </summary>
        /// <param name="clusters">The list of clusters.</param>
        private void BuildClusterTree(List<UMCClusterLight> clusters)
        {
            var maxMass = (float)clusters.Max(cluster => cluster.MassMonoisotopicAligned);
            var rectangle = new RectangleF
            {
                X = 0,
                Y = 0,
                Height = maxMass,
                Width = 1
            };

            this.clusterTree = new QuadTree<ClusterPoint>(rectangle);
            clusters.ForEach(cluster => this.clusterTree.Insert(new ClusterPoint(cluster)));
        }

        /// <summary>
        /// Build cluster plot based on points in range selected/zoom level.
        /// </summary>
        private void BuildClusterPlot()
        {

        }
    }
}
