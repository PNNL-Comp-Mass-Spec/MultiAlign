using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MultiAlignCore.Data;

namespace MultiAlign.Windows.Viewers.Clusters
{
    /// <summary>
    /// Interaction logic for 
    /// </summary>
    public partial class ClusterToMassTagGrid : UserControl
    {
        private List<ClusterToMassTagMap> m_massTags;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClusterToMassTagGrid()
        {
            InitializeComponent();
            MassTags = new List<ClusterToMassTagMap>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public List<ClusterToMassTagMap> MassTags
        {
            get
            {
                return m_massTags;
            }
            set
            {
                m_massTags = value;
                if (value != null)
                {
                    m_dataGrid.ItemsSource = value;
                }
            }
        }

        public ClusterToMassTagMap SelectedTag
        {
            get { return (ClusterToMassTagMap)GetValue(SelectedMassTagProperty); }
            set { SetValue(SelectedMassTagProperty, value); }
        }        
        public static readonly DependencyProperty SelectedMassTagProperty =
            DependencyProperty.Register("SelectedTag", typeof(ClusterToMassTagMap), typeof(ClusterToMassTagGrid)); 

       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = m_dataGrid.SelectedItem as ClusterToMassTagMap;
            SelectedTag = tag;
        }    
    }
}
