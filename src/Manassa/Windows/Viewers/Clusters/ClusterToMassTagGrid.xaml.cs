using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiAlignCore.Data;
using System.ComponentModel;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
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
            ClusterToMassTagMap tag = m_dataGrid.SelectedItem as ClusterToMassTagMap;
            SelectedTag = tag;
        }    
    }
}
