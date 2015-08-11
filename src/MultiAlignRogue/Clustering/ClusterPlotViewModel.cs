using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlign.ViewModels.Charting;
using NHibernate.Util;
using OxyPlot.Annotations;
using OxyPlot.Series;

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
        /// Number of divisions on mass axis.
        /// </summary>
        private readonly int massAxisDivisions;

        /// <summary>
        /// Number of divisions on net axis.
        /// </summary>
        private readonly int netAxisDivisions;

        /// <summary>
        /// Maximum number of clusters in a single division of the view.
        /// </summary>
        private readonly int maxClustersPerDivision;

        /// <summary>
        /// The list of clusters.
        /// </summary>
        private List<UMCClusterLight> clusters; 

        /// <summary>
        /// QuadTree of clusters.
        /// </summary>
        private QuadTree<ClusterPoint> clusterTree;

        /// <summary>
        /// The cluster selected and highlighted on the plot.
        /// </summary>
        private UMCClusterLight selectedCluster;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterPlotViewModel"/> class.
        /// </summary>
        /// <param name="clusters">The clusters to plot.</param>
        public ClusterPlotViewModel(List<UMCClusterLight> clusters)
        {
            this.clusters = clusters;
            this.massAxisDivisions = 20;
            this.netAxisDivisions = 20;
            this.maxClustersPerDivision = 10;

            this.throttler = new Throttler(TimeSpan.FromMilliseconds(100));
            this.BuildClusterTree();

            this.netAxis = new LinearAxis
            {
                Title = "NET",
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                Minimum = 0,
                AbsoluteMaximum = 1.0,
                Maximum = 1.0
            };

            var maxMass = this.clusters.Max(x => x.MassMonoisotopicAligned);
            this.massAxis = new LinearAxis
            {
                Title = "Mass",
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                Minimum = 0,
                AbsoluteMaximum = maxMass,
                Maximum = maxMass
            };

            this.netAxis.AxisChanged += (s, e) => this.throttler.Run(this.BuildClusterPlot);
            this.massAxis.AxisChanged += (s, e) => this.throttler.Run(this.BuildClusterPlot);

            this.ClusterPlotModel = new PlotModel();
            this.ClusterPlotModel.Axes.Add(this.netAxis);
            this.ClusterPlotModel.Axes.Add(this.massAxis);
            this.ClusterPlotModel.MouseDown += this.ClusterPlotMouseDown;

            this.throttler.Run(this.BuildClusterPlot);
        }

        /// <summary>
        /// Gets the plot model for cluster plot.
        /// </summary>
        public PlotModel ClusterPlotModel { get; private set; }

        /// <summary>
        /// Gets or sets the cluster selected and highlighted on the plot.
        /// </summary>
        public UMCClusterLight SelectedCluster
        {
            get { return this.selectedCluster; }
            set
            {
                if (this.selectedCluster != value)
                {
                    this.selectedCluster = value;
                    this.throttler.Run(this.BuildClusterPlot);
                    this.RaisePropertyChanged("SelectedCluster", null, value, true);
                }
            }
        }

        /// <summary>
        /// Build cluster quad tree from list of clusters.
        /// </summary>
        private void BuildClusterTree()
        {
            var maxMass = (float)this.clusters.Max(cluster => cluster.MassMonoisotopicAligned);
            var rectangle = new RectangleF
            {
                X = 0,
                Y = 0,
                Height = maxMass,
                Width = 1
            };

            this.clusterTree = new QuadTree<ClusterPoint>(rectangle);
            this.clusters.ForEach(cluster => this.clusterTree.Insert(new ClusterPoint(cluster)));
        }

        /// <summary>
        /// Build cluster plot based on points in range selected/zoom level.
        /// </summary>
        private void BuildClusterPlot()
        {
            this.SetClusterHighlight();

            var netRange = (this.netAxis.ActualMaximum - this.netAxis.ActualMinimum) / this.netAxisDivisions;
            var massRange = (this.massAxis.ActualMaximum - this.massAxis.ActualMinimum) / this.massAxisDivisions;

            // Select clusters to display
            var clusterHash = new HashSet<ClusterPoint>();
            if (this.SelectedCluster != null)
            {
                clusterHash.Add(new ClusterPoint(this.SelectedCluster));
            }

            for (int i = 0; i < this.netAxisDivisions; i++)
            {
                var minNet = (float)(this.netAxis.ActualMinimum + (i * netRange));
                var maxNet = (float)(this.netAxis.ActualMinimum + ((i + 1) * netRange));
                for (int j = 0; j < this.massAxisDivisions; j++)
                {
                    var minMass = (float)(this.massAxis.ActualMinimum + (j * massRange));
                    var maxMass = (float)(this.massAxis.ActualMinimum + ((j + 1) * massRange));
                    var rectangle = new RectangleF
                    {
                        X = minNet,
                        Y = minMass,
                        Width = maxNet - minNet,
                        Height = maxMass - minMass
                    };

                    var clusterPoints = this.clusterTree.Query(rectangle)
                                       .OrderByDescending(clusterPoint => clusterPoint.UMCClusterLight.Abundance)
                                       .Take(this.maxClustersPerDivision);
                    clusterHash.UnionWith(clusterPoints);
                }
            }

            // Create cluster series
            this.ClusterPlotModel.Series.Clear();
            var clusterSeries = new ScatterSeries
            {
                ItemsSource = clusterHash,
                MarkerFill = OxyColors.Red,
                MarkerType = MarkerType.Circle
            };

            this.ClusterPlotModel.Series.Add(clusterSeries);
            this.ClusterPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Sets the size of the cluster highlight rectangle annotation based on zoom level.
        /// </summary>
        private void SetClusterHighlight()
        {
            var netRange = (this.netAxis.ActualMaximum - this.netAxis.ActualMinimum) * 0.01;
            var massRange = (this.massAxis.ActualMaximum - this.netAxis.ActualMinimum) * 0.01;
            this.ClusterPlotModel.Annotations.Clear();
            if (this.SelectedCluster != null)
            {
                var clusterHighlight = new RectangleAnnotation
                {
                    StrokeThickness = 2,
                    Stroke = OxyColors.LimeGreen,
                    Fill = OxyColors.Transparent,
                    MinimumX = this.SelectedCluster.Net - netRange,
                    MaximumX = this.SelectedCluster.Net + netRange,
                    MinimumY = this.SelectedCluster.MassMonoisotopicAligned - massRange,
                    MaximumY = this.SelectedCluster.MassMonoisotopicAligned + massRange
                };

                this.ClusterPlotModel.Annotations.Add(clusterHighlight);
            }
        }

        /// <summary>
        /// Event handler for OxyPlot left mouse click.
        /// </summary>
        /// <param name="sender">The sending PlotView.</param>
        /// <param name="args">The event arguments.</param>
        private void ClusterPlotMouseDown(object sender, OxyMouseEventArgs args)
        {
            var series = this.ClusterPlotModel.GetSeriesFromPoint(args.Position, 10);
            if (series != null)
            {
                var result = series.GetNearestPoint(args.Position, false);
                if (result != null)
                {
                    var clusterPoint = result.Item as ClusterPoint;
                    if (clusterPoint != null)
                    {
                        this.SelectedCluster = clusterPoint.UMCClusterLight;
                    }
                }
            }
        }
    }
}
