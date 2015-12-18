namespace MultiAlignRogue.Clustering
{
    using System.IO;
    using GalaSoft.MvvmLight.Messaging;
    using System.Windows;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// Interaction logic for ClusterView.xaml
    /// </summary>
    public partial class ClusterView : Window
    {
        /// <summary>
        /// Indicates whether the docking manager has loaded yet.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// The selected item in the ScanDataGrid.
        /// </summary>
        private object selectedItem;

        /// <summary>
        /// The view model.
        /// </summary>
        private ClusterViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterView"/> class.
        /// </summary>
        public ClusterView()
        {
            this.InitializeComponent();

            this.isLoaded = false;
            this.viewModel = this.DataContext as ClusterViewModel;
            this.XicPlotMenu.DataContext = this.DataContext;

            // Update layout when AvDock is loaded.
            this.AvDock.Loaded += (s, e) =>
            {
                this.LoadNewViewModel();
                this.isLoaded = true;
            };
            
            // Update layout when view model changes.
            this.DataContextChanged += (o, e) =>
            {
                this.XicPlotMenu.DataContext = this.DataContext;
                if (this.isLoaded)
                {
                    this.LoadNewViewModel();
                }
            };

            // Listen to changes in file path in view model.
            Messenger.Default.Register<PropertyChangedMessage<LayoutRoot>>(
                this,
                args =>
            {
                if (args.Sender == this.viewModel && 
                    args.PropertyName == "LayoutRoot" &&
                    this.AvDock.Layout != args.NewValue &&
                    this.isLoaded)
                {
                    this.LoadLayout(args.NewValue);
                }
            });

            // Scroll to selected item when SelectedItem is changed externally.
            this.ClusterDataGrid.SelectionChanged += (o, e) =>
            {
                object item = this.ClusterDataGrid.SelectedItem;
                if (this.ClusterDataGrid.SelectedItem == null && this.selectedItem != null)
                {
                    item = this.selectedItem;
                }

                this.selectedItem = item;
                this.ClusterDataGrid.ScrollIntoView(item);
                this.ClusterDataGrid.UpdateLayout();
            };

            // Serialize layout when window is closed.
            this.AvDock.Unloaded += (s, e) =>
            {
                this.SaveLayout();
            };
        }

        /// <summary>
        /// Initialize docking manager to layout.
        /// </summary>
        private void LoadLayout(LayoutRoot layout)
        {
            if (layout != null)
            {
                this.AvDock.Initialize(layout);
            }
        }

        /// <summary>
        /// Serialize the layout to layoutFilePath.
        /// </summary>
        private void SaveLayout()
        {
            if (this.viewModel != null)
            {
                this.viewModel.LayoutRoot = this.AvDock.Layout;
                this.viewModel.SaveLayoutFile();
            }
        }

        /// <summary>
        /// Load layout for a view model.
        /// </summary>
        private void LoadNewViewModel()
        {
            this.viewModel = this.DataContext as ClusterViewModel;
            if (this.viewModel != null)
            {
                this.LoadLayout(this.viewModel.LayoutRoot);
            }
        }
    }
}
