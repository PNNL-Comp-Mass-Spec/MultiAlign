using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MultiAlignCore.Data.Features;

namespace MultiAlign.Windows.Viewers.Spectra
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
    /// </summary>
    public partial class MsMsGrid : UserControl
    {
        private ObservableCollection<MSFeatureMsMs> m_clusters;

        public MsMsGrid()
        {
            InitializeComponent();
            MsMsSpectra = new ObservableCollection<MSFeatureMsMs>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public ObservableCollection<MSFeatureMsMs> MsMsSpectra
        {
            get
            {
                return m_clusters;
            }
            set
            {
                m_clusters = value;
                if (value != null)
                {
                    m_dataGrid.ItemsSource = value;
                }
            }
        }

        public MSFeatureMsMs SelectedSpectra
        {
            get { return (MSFeatureMsMs)GetValue(SelectedSpectraProperty); }
            set { SetValue(SelectedSpectraProperty, value); }
        }        
        public static readonly DependencyProperty SelectedSpectraProperty =
            DependencyProperty.Register("SelectedSpectra", typeof(MSFeatureMsMs), typeof(MsMsGrid)); 

       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSpectra = m_dataGrid.SelectedItem as MSFeatureMsMs;
        }
    }
}
