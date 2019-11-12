using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MultiAlignRogue.Clustering
{
    public class ClusterViewerSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// The number of divisions on the net axis.
        /// </summary>
        private int netDivisions;

        /// <summary>
        /// The number of divisions on the mass axis.
        /// </summary>
        private int massDivisions;

        /// <summary>
        /// The number of points to display per 2-dimensional
        /// division in mass in net.
        /// </summary>
        private int pointsPerDivision;

        /// <summary>
        /// A value indicating whether the points should be
        /// reduced on the cluster view plot.
        /// </summary>
        private bool shouldReducePoints;

        /// <summary>
        /// A value that indicates whether lines visualizing the
        /// mass and net divisions should be displayed on the cluster plot.
        /// </summary>
        private bool showDivisionLines;

        public ClusterViewerSettingsViewModel(ClusterViewerSettings clusterViewerSettings)
        {
            this.ShouldReducePoints = clusterViewerSettings.ShouldReducePoints;
            this.NetDivisions = clusterViewerSettings.NetDivisions;
            this.MassDivisions = clusterViewerSettings.MassDivisions;
            this.PointsPerDivision = clusterViewerSettings.PointsPerDivision;
            this.ShowDivisionLines = clusterViewerSettings.ShowDivisionLines;
            this.Status = false;

            this.SaveCommand = new RelayCommand(() =>
            {
                this.Status = true;
                ReadyToClose?.Invoke(this, EventArgs.Empty);
            });

            this.CancelCommand = new RelayCommand(() =>
            {
                this.Status = false;
                ReadyToClose?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// An event that is triggered when the settings are ready to be closed.
        /// </summary>
        public event EventHandler ReadyToClose;

        /// <summary>
        /// Gets a command that saves the settings.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets a command that cancels the settings editing.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Gets a value that indicates whether the settings were saved or cancelled.
        /// </summary>
        public bool Status { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClusterViewerSettings" /> model for this view model.
        /// </summary>
        public ClusterViewerSettings ClusterViewerSettings
        {
            get
            {
                return new ClusterViewerSettings
                {
                    ShouldReducePoints = this.ShouldReducePoints,
                    NetDivisions = this.NetDivisions,
                    MassDivisions = this.MassDivisions,
                    PointsPerDivision = this.PointsPerDivision,
                    ShowDivisionLines = this.ShowDivisionLines
                };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the points should be
        /// reduced on the cluster view plot.
        /// </summary>
        public bool ShouldReducePoints
        {
            get => this.shouldReducePoints;
            set
            {
                if (this.shouldReducePoints != value)
                {
                    this.shouldReducePoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of divisions on the net axis.
        /// </summary>
        public int NetDivisions
        {
            get => this.netDivisions;
            set
            {
                if (this.netDivisions != value)
                {
                    this.netDivisions = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of divisions on the mass axis.
        /// </summary>
        public int MassDivisions
        {
            get => this.massDivisions;
            set
            {
                if (this.massDivisions != value)
                {
                    this.massDivisions = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of points to display per 2-dimensional
        /// division in mass in net.
        /// </summary>
        public int PointsPerDivision
        {
            get => this.pointsPerDivision;
            set
            {
                if (this.pointsPerDivision != value)
                {
                    this.pointsPerDivision = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether lines visualizing the
        /// mass and net divisions should be displayed on the cluster plot.
        /// </summary>
        public bool ShowDivisionLines
        {
            get => this.showDivisionLines;
            set
            {
                if (this.showDivisionLines != value)
                {
                    this.showDivisionLines = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
