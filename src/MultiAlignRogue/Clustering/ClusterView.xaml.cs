using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace MultiAlignRogue.Clustering
{
    using System.Windows;
    
    /// <summary>
    /// Interaction logic for ClusterView.xaml
    /// </summary>
    public partial class ClusterView : Window
    {
        /// <summary>
        /// The name of the default layout file to use.
        /// </summary>
        private const string StandardLayoutFileName = "StandardLayout.xml";

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
        /// The path to the layout file.
        /// </summary>
        private string layoutFilePath;

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
            Messenger.Default.Register<PropertyChangedMessage<string>>(
                this,
                args =>
            {
                if (args.Sender == this.viewModel && 
                    args.PropertyName == "LayoutFilePath" &&
                    this.layoutFilePath != args.NewValue &&
                    this.isLoaded)
                {
                    this.LoadLayout();
                }
            });

            // Scroll to selected item when SelectedItem is changed externally.
            ClusterDataGrid.SelectionChanged += (o, e) =>
            {
                object item = ClusterDataGrid.SelectedItem;
                if (ClusterDataGrid.SelectedItem == null && selectedItem != null)
                {
                    item = selectedItem;
                }

                selectedItem = item;
                ClusterDataGrid.ScrollIntoView(item);
                ClusterDataGrid.UpdateLayout();
            };

            // Serialize layout when window is closed.
            this.AvDock.Unloaded += (s, e) => this.SaveLayout();
        }

        /// <summary>
        /// Serialize the layout to layoutFilePath.
        /// </summary>
        private void SaveLayout()
        {
            using (var fs = File.Open(this.layoutFilePath, FileMode.Create))
            {
                var xmlLayout = new XmlLayoutSerializer(AvDock);
                xmlLayout.Serialize(fs);
            }
        }

        /// <summary>
        /// Deserialize layout from layoutFilePath.
        /// </summary>
        private void LoadLayout()
        {
            var path = this.layoutFilePath;

            // If the layout file doesn't exist, use standard layout file.
            if (!File.Exists(this.layoutFilePath) && File.Exists(StandardLayoutFileName))
            {
                path = StandardLayoutFileName;
                ////this.SaveLayout();
            }

            var serializer = new XmlLayoutSerializer(AvDock);
            using (var stream = new StreamReader(path))
            {
                serializer.LayoutSerializationCallback += (s, args) =>
                {
                    args.Content = this.FindName(args.Model.ContentId);
                };
                serializer.Deserialize(stream);
            }

            // If the layout file doesn't exist, create it.
            if (!File.Exists(this.layoutFilePath))
            {
                this.SaveLayout();
            }
        }

        /// <summary>
        /// Load layout for a view model.
        /// </summary>
        private void LoadNewViewModel()
        {
            this.viewModel = this.DataContext as ClusterViewModel;
            if (this.viewModel != null && this.layoutFilePath != this.viewModel.LayoutFilePath)
            {
                this.layoutFilePath = this.viewModel.LayoutFilePath;
                this.LoadLayout();
            }
        }
    }
}
