using System.Collections.Generic;
using System.Linq;
using PNNLOmics.Data.Features;

namespace MultiAlignRogue.Clustering
{
    using System.Windows;
    
    /// <summary>
    /// Interaction logic for ClusterView.xaml
    /// </summary>
    public partial class ClusterView : Window
    {
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

            this.viewModel = this.DataContext as ClusterViewModel;
            this.DataContextChanged += (o, e) => this.viewModel = this.DataContext as ClusterViewModel;

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

            this.FeatureDataGrid.SelectionChanged += (s, e) =>
            {
                var selectedItems = this.FeatureDataGrid.SelectedItems;
                if (this.viewModel != null)
                {
                    this.viewModel.SelectedFeatures = new List<UMCLight>(selectedItems.Cast<UMCLight>());
                }
            };
        }
    }
}
