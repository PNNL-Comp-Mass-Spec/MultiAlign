namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using GalaSoft.MvvmLight;

    using OxyPlot;

    using PNNLOmics.Data.Features;

    /// <summary>
    /// The view model for the ClusterView.
    /// </summary>
    public class ClusterViewModel : ViewModelBase
    {
        /// <summary>
        /// The selected cluster.
        /// </summary>
        private UMCClusterLight selectedCluster;

        /// <summary>
        /// The selected feature.
        /// </summary>
        private UMCLight selectedFeature;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterViewModel"/> class.
        /// </summary>
        /// <param name="clusters">
        /// The clusters.
        /// </param>
        public ClusterViewModel(List<UMCClusterLight> clusters)
        {
            this.Clusters = new ObservableCollection<UMCClusterLight>(clusters ?? new List<UMCClusterLight>());
            this.Features = new ObservableCollection<UMCLight>();

            this.XicPlotModel = new PlotModel();
            this.ClusterPlotViewModel = new ClusterPlotViewModel(clusters);
            this.ClusterPlotViewModel.ClusterSelected += (s, e) =>
            {
                if (!this.ClusterPlotViewModel.SelectedCluster.Equals(this.SelectedCluster))
                {
                    this.SelectedCluster = this.ClusterPlotViewModel.SelectedCluster;
                }
            };

            if (this.Clusters.Count > 0)
            {
                this.SelectedCluster = this.Clusters[0];
            }
        }

        /// <summary>
        /// Gets the list of clusters.
        /// </summary>
        public ObservableCollection<UMCClusterLight> Clusters { get; private set; }

        /// <summary>
        /// Gets the list of features for the selected cluster.
        /// </summary>
        public ObservableCollection<UMCLight> Features { get; private set; } 

        /// <summary>
        /// Gets the plot model for extracted ion chromatogram plots.
        /// </summary>
        public PlotModel XicPlotModel { get; private set; }

        /// <summary>
        /// Gets the view model for the cluster plot.
        /// </summary>
        public ClusterPlotViewModel ClusterPlotViewModel { get; private set; }

        /// <summary>
        /// Gets or sets the selected cluster.
        /// </summary>
        public UMCClusterLight SelectedCluster
        {
            get { return this.selectedCluster; }
            set
            {
                if (this.selectedCluster != value)
                {
                    this.selectedCluster = value;
                    this.Features.Clear();

                    // Set feature list for this cluster.
                    if (this.selectedCluster != null && this.selectedCluster.UmcList != null)
                    {
                        this.selectedCluster.UmcList.ForEach(cluster => this.Features.Add(cluster));
                        this.ClusterPlotViewModel.SelectedCluster = this.selectedCluster;
                    }

                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected feature.
        /// </summary>
        public UMCLight SelectedFeature
        {
            get { return this.selectedFeature; }
            set
            {
                if (!this.selectedFeature.Equals(value))
                {
                    this.selectedFeature = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
