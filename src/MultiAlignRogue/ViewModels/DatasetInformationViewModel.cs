﻿using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MultiAlignCore.Data.MetaData;
using MultiAlignRogue.Utils;

namespace MultiAlignRogue.ViewModels
{
    using System.Windows.Media;

    public class DatasetInformationViewModel : ViewModelBase
    {
        public enum DatasetStates
        {
            Waiting,
            LoadingRawData,
            Loaded,
            FindingFeatures,
            PersistingFeatures,
            FeaturesFound,
            Aligning,
            Baseline,
            PersistingAlignment,
            Aligned,
            Clustering,
            PersistingClusters,
            Clustered,
            Matching,
            PersistingMatches,
            Matched,
        };

        private bool m_expand;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="information"></param>
        public DatasetInformationViewModel(DatasetInformation information)
        {
            Dataset = information;
            // var data = information.PlotData;

            RequestRemovalCommand = new RelayCommand(
                () =>
                {
                    RemovalRequested?.Invoke(this, EventArgs.Empty);
                }, () => !this.DoingWork);

            this.SetDatasetState();
        }

        public event EventHandler RemovalRequested;

        public event EventHandler StateChanged;

        public RelayCommand RequestRemovalCommand { get; }

        private DatasetStates datasetState;
        public DatasetStates DatasetState
        {
            get => this.datasetState;
            set
            {
                if (this.datasetState != value)
                {
                    var prevValue = this.datasetState;
                    this.datasetState = value;

                    this.IsFindingFeatures = value == DatasetStates.FindingFeatures ||
                                             value == DatasetStates.PersistingFeatures;

                    this.IsAligning = value == DatasetStates.Aligning ||
                                      value == DatasetStates.PersistingAlignment;

                    this.IsClustering = value == DatasetStates.Clustering ||
                                        value == DatasetStates.PersistingClusters;
                    this.FeaturesFound = value >= DatasetStates.FeaturesFound;
                    this.IsAligned = value >= DatasetStates.Aligned;
                    this.IsClustered = value >= DatasetStates.Clustered;

                    this.IsLoadingRawData = value == DatasetStates.LoadingRawData;

                    this.ShouldShowProgress = (value == DatasetStates.LoadingRawData) ||
                          (value == DatasetStates.FindingFeatures) ||
                          (value == DatasetStates.PersistingFeatures) ||
                          ((value == DatasetStates.Aligning) ||
                          (value == DatasetStates.PersistingAlignment));

                    var isFinishedState = (value == DatasetStates.FeaturesFound || value == DatasetStates.Aligned ||
                                           value == DatasetStates.Clustered     || value == DatasetStates.Matched);
                    if (isFinishedState)
                    {
                        StateChanged?.Invoke(this, EventArgs.Empty);
                    }

                    this.RaisePropertyChanged(nameof(FindingFeatureLabelColor));
                    this.RaisePropertyChanged(nameof(AligningLabelColor));
                    this.RaisePropertyChanged(nameof(ClusterLabelColor));
                    this.RaisePropertyChanged(nameof(DatasetState), prevValue, value, true);
                }
            }
        }

        public bool IsBaseline
        {
            get => this.Dataset.IsBaseline;
            set
            {
                if (this.Dataset.IsBaseline != value)
                {
                    this.Dataset.IsBaseline = value;
                    this.DatasetState = DatasetStates.Baseline;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool FeaturesFound
        {
            get => this.Dataset.FeaturesFound;
            private set
            {
                if (this.Dataset.FeaturesFound != value)
                {
                    this.Dataset.FeaturesFound = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsAligned
        {
            get => this.Dataset.IsAligned;
            private set
            {
                if (this.Dataset.IsAligned != value)
                {
                    this.Dataset.IsAligned = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool isClustering;

        public bool IsClustering
        {
            get => this.isClustering;
            private set
            {
                if (this.isClustering != value)
                {
                    this.isClustering = value;
                    this.RaisePropertyChanged(nameof(ClusterLabelColor));
                    this.RaisePropertyChanged(nameof(DoingWork));
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsClustered
        {
            get => this.Dataset.IsClustered;
            private set
            {
                if (this.Dataset.IsClustered != value)
                {
                    this.Dataset.IsClustered = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Brush ClusterLabelColor
        {
            get
            {
                Brush brush;
                switch (this.DatasetState)
                {
                    case DatasetStates.Clustering:
                        brush = Brushes.Red;
                        break;
                    case DatasetStates.PersistingClusters:
                        brush = Brushes.Yellow;
                        break;
                    default:
                        brush = Brushes.Transparent;
                        break;
                }

                return brush;
            }
        }

        private bool isFindingFeatures;
        public bool IsFindingFeatures
        {
            get => this.isFindingFeatures;
            private set
            {
                if (this.isFindingFeatures != value)
                {
                    this.isFindingFeatures = value;
                    ThreadSafeDispatcher.Invoke(() => this.RequestRemovalCommand.RaiseCanExecuteChanged());
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(FindingFeatureLabelColor));
                    this.RaisePropertyChanged(nameof(DoingWork));
                }
            }
        }

        public Brush FindingFeatureLabelColor
        {
            get
            {
                Brush brush;
                switch (this.DatasetState)
                {
                    case DatasetStates.FindingFeatures:
                        brush = Brushes.Red;
                        break;
                    case DatasetStates.PersistingFeatures:
                        brush = Brushes.Yellow;
                        break;
                    default:
                        brush = Brushes.Transparent;
                        break;
                }

                return brush;
            }
        }

        private bool isAligning;
        public bool IsAligning
        {
            get => this.isAligning;
            private set
            {
                if (this.isAligning != value)
                {
                    this.isAligning = value;
                    ThreadSafeDispatcher.Invoke(() => this.RequestRemovalCommand.RaiseCanExecuteChanged());
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(AligningLabelColor));
                    this.RaisePropertyChanged(nameof(DoingWork));
                }
            }
        }

        public Brush AligningLabelColor
        {
            get
            {
                Brush brush;
                switch (this.DatasetState)
                {
                    case DatasetStates.Aligning:
                        brush = Brushes.Red;
                        break;
                    case DatasetStates.PersistingAlignment:
                        brush = Brushes.Yellow;
                        break;
                    default:
                        brush = Brushes.Transparent;
                        break;
                }

                return brush;
            }
        }

        private bool isLoadingRawData;
        public bool IsLoadingRawData
        {
            get => this.isLoadingRawData;
            private set
            {
                if (this.isLoadingRawData != value)
                {
                    this.isLoadingRawData = value;
                    ThreadSafeDispatcher.Invoke(() => this.RequestRemovalCommand.RaiseCanExecuteChanged());
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(DoingWork));
                }
            }
        }

        public bool DoingWork => this.IsLoadingRawData || this.IsAligning || this.IsFindingFeatures || this.IsClustering;

        private double progress;

        public double Progress
        {
            get => this.progress;
            set
            {
                if (Math.Abs(this.progress - value) > float.Epsilon)
                {
                    this.progress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool shouldShowProgress;
        public bool ShouldShowProgress
        {
            get => this.shouldShowProgress;
            set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.Progress = 0; // Reset the progress to zero
                    this.RaisePropertyChanged();
                }
            }
        }

        public DatasetInformation Dataset { get; }

        public int DatasetId
        {
            get => Dataset.DatasetId;
            set
            {
                if (Dataset != null)
                {
                    Dataset.DatasetId = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                var name = "";
                if (Dataset != null)
                {
                    name = Dataset.DatasetName;
                }
                return name;
            }
        }

        /// <summary>
        /// Replace a single underscore with a double underscore since WPF treats a single underscore as a keyboard shortcut
        /// http://stackoverflow.com/questions/7861699/can-not-see-underscore-in-wpf-content
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string DisplayName => Name.Replace("_", "__");

        public int Id
        {
            get
            {
                var id = 0;
                if (Dataset != null)
                {
                    id = Dataset.DatasetId;
                }
                return id;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public bool ShouldExpand
        {
            get => m_expand;
            set
            {
                if (value != m_expand)
                {
                    m_expand = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.RaisePropertyChanged(nameof(IsSelected), !value, value, true);
                }
            }
        }

#pragma warning disable 67
        public event EventHandler Selected;
#pragma warning restore 67

        private void SetDatasetState()
        {
            if (this.FeaturesFound && !(this.IsAligned || this.IsClustered))
            {
                this.DatasetState = DatasetStates.FeaturesFound;
            }

            if (this.IsAligned && ! this.IsClustered)
            {
                this.DatasetState = DatasetStates.Aligned;
            }

            if (this.IsClustered)
            {
                this.DatasetState = DatasetStates.Clustered;
            }
        }
    }
}