using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MultiAlign.Data;
using MultiAlign.ViewModels.Charting;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;
using NHibernate.Mapping;

namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using GalaSoft.MvvmLight;

    using PNNLOmics.Data.Features;

    /// <summary>
    /// The view model for the ClusterView.
    /// </summary>
    public class ClusterViewModel : ViewModelBase
    {
        /// <summary>
        /// Data access providers for reconstructing clusters.
        /// </summary>
        private readonly FeatureDataAccessProviders providers;

        /// <summary>
        /// Lock for database access.
        /// </summary>
        private readonly object dbLock;

        /// <summary>
        /// The throttler.
        /// </summary>
        private readonly Throttler throttler;

        /// <summary>
        /// Factory for creating child windows.
        /// </summary>
        private readonly IClusterViewFactory viewFactory;

        /// <summary>
        /// The selected cluster.
        /// </summary>
        private UMCClusterLight selectedCluster;

        /// <summary>
        /// The selected feature.
        /// </summary>
        private IEnumerable<UMCLightViewModel> selectedFeatures;

        /// <summary>
        /// Path to layout file.
        /// </summary>
        private string layoutFilePath;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterViewModel"/> class.
        /// </summary>
        /// <param name="viewFactory">Factory for creating child windows.</param>
        /// <param name="clusters">The clusters.</param>
        /// <param name="layoutFilePath">Path to layout file.</param>
        public ClusterViewModel(IClusterViewFactory viewFactory, List<UMCClusterLight> clusters, FeatureDataAccessProviders providers, string layoutFilePath)
        {
            this.viewFactory = viewFactory;
            this.providers = providers;
            this.dbLock = new object();
            this.throttler = new Throttler(TimeSpan.FromMilliseconds(500));
            this.XicPlotViewModel = new XicPlotViewModel();
            this.Clusters = new ObservableCollection<UMCClusterLight>(clusters ?? new List<UMCClusterLight>());
            this.Features = new ObservableCollection<UMCLightViewModel>();
            this.LayoutFilePath = layoutFilePath;

            this.ClusterPlotViewModel = new ClusterPlotViewModel(clusters);

            this.ShowChargeStateDistributionCommand = new RelayCommand(
                () =>
                {
                    this.viewFactory.CreateChargeStateDistributionWindow(clusters, "Charge State Distribution");
                });

            // Listen for changes in selected cluster in ClusterViewModel.
            Messenger.Default.Register<PropertyChangedMessage<UMCClusterLight>>(
                this,
                args =>
            {
                if (args.Sender == this.ClusterPlotViewModel && !this.ClusterPlotViewModel.SelectedCluster.Equals(this.SelectedCluster))
                {
                    this.SelectedCluster = this.ClusterPlotViewModel.SelectedCluster;
                } 
            });

            // Listen for changes in selected cluster internally.
            Messenger.Default.Register<PropertyChangedMessage<UMCClusterLight>>(
                this,
                arg =>
            {
                if (arg.Sender == this)
                {
                    this.throttler.Run(() => this.ClusterSelected(arg));
                }
            });

            if (this.Clusters.Count > 0)
            {
                this.SelectedCluster = this.Clusters[0];
            }
        }

        /// <summary>
        /// Gets a command that displays a charge state distribution plot.
        /// </summary>
        public RelayCommand ShowChargeStateDistributionCommand { get; private set; }

        /// <summary>
        /// Gets the list of clusters.
        /// </summary>
        public ObservableCollection<UMCClusterLight> Clusters { get; private set; }

        /// <summary>
        /// Gets the list of features for the selected cluster.
        /// </summary>
        public ObservableCollection<UMCLightViewModel> Features { get; private set; } 

        /// <summary>
        /// Gets the view model for extracted ion chromatogram plots.
        /// </summary>
        public XicPlotViewModel XicPlotViewModel { get; private set; }

        /// <summary>
        /// Gets the view model for the cluster plot.
        /// </summary>
        public ClusterPlotViewModel ClusterPlotViewModel { get; private set; }

        /// <summary>
        /// Gets the path to the layout file.
        /// </summary>
        public string LayoutFilePath
        {
            get { return this.layoutFilePath; }
            private set
            {
                if (this.layoutFilePath != value)
                {
                    this.layoutFilePath = value;
                    this.RaisePropertyChanged("LayoutFilePath", string.Empty, this.layoutFilePath, true);
                }
            }
        }

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
                    this.RaisePropertyChanged("SelectedCluster", null, value, true);
                }
            }
        }

        /// <summary>
        /// Event handler for SelectedCluster changed.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        private async void ClusterSelected(PropertyChangedMessage<UMCClusterLight> args)
        {
            var cluster = args.NewValue;

            this.Features.Clear();

            // Set feature list for this cluster.
            if (this.selectedCluster != null)
            {
                this.ClusterPlotViewModel.SelectedCluster = cluster;
                await Task.Run(() =>
                {
                    lock (this.dbLock)
                    {
                        cluster.ReconstructUMCCluster(this.providers);
                    }
                });
                cluster.UmcList.ForEach(c => this.Features.Add(new UMCLightViewModel(c)));
                this.XicPlotViewModel.Features = new List<UMCLightViewModel>(this.Features);
            }
        }
    }
}
