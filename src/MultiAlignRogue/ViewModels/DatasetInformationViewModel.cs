using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using MultiAlign.Commands.Datasets;
using MultiAlign.Commands.Plotting;
using MultiAlign.ViewModels.Plotting;
using MultiAlignCore.Data.MetaData;
using NHibernate.Mapping;

namespace MultiAlignRogue.ViewModels
{
    using System.Windows.Input;
    using System.Windows.Media;

    using MultiAlign.Commands;
    using MultiAlign.Data;

    public class DatasetInformationViewModel : ViewModelBase
    {
        public enum DatasetStates
        {
            None,
            FindingFeatures,
            PersistingFeatures,
            FeaturesFound,
            Aligning,
            PersistingAlignment,
            Aligned,
            Clustering,
            PersistingClusters,
            Clustered
        };

        private readonly DatasetInformation m_information;
        private bool m_expand;
        private bool m_isSelected;

        public DatasetInformationViewModel(DatasetInformation information)
        {
            m_information = information;
            var data = information.PlotData;
            PlotData = new ObservableCollection<PlotViewModel>();

            RequestRemovalCommand = new BaseCommand(
                () =>
                {
                    if (RemovalRequested != null)
                    {
                        RemovalRequested(this, EventArgs.Empty);
                    }
                }, s => !this.DoingWork);

            if (data != null)
            {
                PlotData.Add(new PlotViewModel(data.Alignment,
                    "Alignment",
                    new PictureDisplayCommand(data.Alignment, "Alignment" + information.DatasetName)));
                PlotData.Add(new PlotViewModel(data.Features,
                    "Features",
                    new FeatureDisplayCommand(information)));

                PlotData.Add(new PlotViewModel(data.MassErrorHistogram, "Mass Error Histogram"));
                PlotData.Add(new PlotViewModel(data.NetErrorHistogram, "NET Error Histogram"));
                PlotData.Add(new PlotViewModel(data.MassScanResidual, "Mass vs Scan Residuals"));
                PlotData.Add(new PlotViewModel(data.MassMzResidual, "Mass vs m/z Residuals"));
                PlotData.Add(new PlotViewModel(data.NetResiduals, "NET Residuals"));
            }


            ModifyDatasetCommand = new ShowDatasetDetailCommand();
        }

        public event EventHandler RemovalRequested;

        public event EventHandler StateChanged;

        public BaseCommand RequestRemovalCommand { get; private set; }

        public bool ScansBool
        {
            get { return this.Dataset.ScansBool; }
            set
            {
                if (this.Dataset.ScansBool != value)
                {
                    this.Dataset.ScansBool = value;
                    this.RaisePropertyChanged("ScansBool");
                }
            }
        }

        public bool RawBool
        {
            get { return this.Dataset.RawBool; }
            set
            {
                if (this.Dataset.RawBool != value)
                {
                    this.Dataset.RawBool = value;
                    this.RaisePropertyChanged("RawBool");
                }
            }
        }

        private DatasetStates datasetState;
        public DatasetStates DatasetState
        {
            get { return this.datasetState; }
            set
            {
                if (this.datasetState != value)
                {
                    var prevValue = this.datasetState;
                    this.datasetState = value;

                    this.IsFindingFeatures = value == DatasetInformationViewModel.DatasetStates.FindingFeatures ||
                                             value == DatasetInformationViewModel.DatasetStates.PersistingFeatures;

                    this.IsAligning = value == DatasetInformationViewModel.DatasetStates.Aligning ||
                                      value == DatasetInformationViewModel.DatasetStates.PersistingAlignment;

                    this.IsClustering = value == DatasetInformationViewModel.DatasetStates.Clustering ||
                                        value == DatasetInformationViewModel.DatasetStates.PersistingClusters;
                    this.FeaturesFound = value >= DatasetInformationViewModel.DatasetStates.FeaturesFound;
                    this.IsAligned = value >= DatasetInformationViewModel.DatasetStates.Aligned;
                    this.IsClustered = value >= DatasetInformationViewModel.DatasetStates.Clustered;

                    if (this.StateChanged != null)
                    {
                        this.StateChanged(this, EventArgs.Empty);
                    }

                    this.ShouldShowProgress = (value == DatasetStates.FindingFeatures) ||
                                              (value == DatasetStates.PersistingFeatures) ||
                                              (value == DatasetStates.Aligning) ||
                                              (value == DatasetStates.PersistingAlignment);

                    this.RaisePropertyChanged("FindingFeatureLabelColor");
                    this.RaisePropertyChanged("AligningLabelColor");
                    this.RaisePropertyChanged("ClusterLabelColor");
                    this.RaisePropertyChanged("DatasetState", prevValue, value, true);
                }
            }
        }

        public bool FeaturesFound
        {
            get { return this.Dataset.FeaturesFound; }
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
            get { return this.Dataset.IsAligned; }
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
            get { return this.isClustering; }
            private set
            {
                if (this.isClustering != value)
                {
                    this.isClustering = value;
                    this.RaisePropertyChanged("ClusterLabelColor");
                    this.RaisePropertyChanged("DoingWork");
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsClustered
        {
            get { return this.Dataset.IsClustered; }
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
                    case DatasetInformationViewModel.DatasetStates.Clustering:
                        brush = Brushes.Red;
                        break;
                    case DatasetInformationViewModel.DatasetStates.PersistingClusters:
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
            get { return this.isFindingFeatures; }
            private set
            {
                if (this.isFindingFeatures != value)
                {
                    this.isFindingFeatures = value;
                    ThreadSafeDispatcher.Invoke(() => this.RequestRemovalCommand.InvokeCanExecuteChanged());
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged("FindingFeatureLabelColor");
                    this.RaisePropertyChanged("DoingWork");
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
                    case DatasetInformationViewModel.DatasetStates.FindingFeatures:
                        brush = Brushes.Red;
                        break;
                    case DatasetInformationViewModel.DatasetStates.PersistingFeatures:
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
            get { return this.isAligning; }
            private set
            {
                if (this.isAligning != value)
                {
                    this.isAligning = value;
                    ThreadSafeDispatcher.Invoke(() => this.RequestRemovalCommand.InvokeCanExecuteChanged());
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged("AligningLabelColor");
                    this.RaisePropertyChanged("DoingWork");
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
                    case DatasetInformationViewModel.DatasetStates.Aligning:
                        brush = Brushes.Red;
                        break;
                    case DatasetInformationViewModel.DatasetStates.PersistingAlignment:
                        brush = Brushes.Yellow;
                        break;
                    default:
                        brush = Brushes.Transparent;
                        break;
                }

                return brush;
            }
        }

        public bool DoingWork
        {
            get { return this.IsAligning || this.IsFindingFeatures || this.IsClustering; }
        }

        private double progress;

        public double Progress
        {
            get { return this.progress; }
            set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    this.RaisePropertyChanged("Progress");
                }
            }
        }

        private bool shouldShowProgress;
        public bool ShouldShowProgress
        {
            get { return this.shouldShowProgress; }
            set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public DatasetInformation Dataset
        {
            get { return m_information; }
        }

        public int DatasetId
        {
            get { return m_information.DatasetId; }
            set
            {
                if (m_information != null)
                {
                    m_information.DatasetId = value;
                    RaisePropertyChanged("DatasetId");
                }
            }
        }

        public string Name
        {
            get
            {
                var name = "";
                if (m_information != null)
                {
                    name = m_information.DatasetName;
                }
                return name;
            }
        }

        public string DisplayName
        {
            get
            {
                ///stupid WPF content __ http://stackoverflow.com/questions/7861699/can-not-see-underscore-in-wpf-content
                return Name.Replace("_", "__");
            }
        }

        public int Id
        {
            get
            {
                var id = 0;
                if (m_information != null)
                {
                    id = m_information.DatasetId;
                }
                return id;
            }
        }

        public ObservableCollection<PlotViewModel> PlotData { get; private set; }

        public bool ShouldExpand
        {
            get { return m_expand; }
            set
            {
                if (value != m_expand)
                {
                    m_expand = value;
                    RaisePropertyChanged("ShouldExpand");
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.RaisePropertyChanged("IsSelected", !value, value, true);
                }
            }
        }

        public ICommand ModifyDatasetCommand { get; set; }
        public event EventHandler Selected;
    }
}