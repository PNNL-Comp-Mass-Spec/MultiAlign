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
        /// Initializes a new instance of the <see cref="ClusterView"/> class.
        /// </summary>
        public ClusterView()
        {
            this.InitializeComponent();

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
        }
    }
}
