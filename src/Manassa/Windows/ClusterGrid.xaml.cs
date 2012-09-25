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

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
    /// </summary>
    public partial class ClusterGrid : UserControl, INotifyPropertyChanged
    {
        private BindingList<UMCClusterLight> m_clusters;
        public event PropertyChangedEventHandler  PropertyChanged;

        public ClusterGrid()
        {
            InitializeComponent();
            Clusters = new BindingList<UMCClusterLight>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public BindingList<UMCClusterLight> Clusters
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

        public UMCClusterLight SelectedCluster
        {
            get { return (UMCClusterLight)GetValue(SelectedClusterProperty); }
            set { SetValue(SelectedClusterProperty, value); }
        }        
        public static readonly DependencyProperty SelectedClusterProperty =
            DependencyProperty.Register("SelectedCluster", typeof(UMCClusterLight), typeof(ClusterGrid)); 

       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCluster != null)
            {
                SelectedCluster.Features.Clear();
            }            
            SelectedCluster = m_dataGrid.SelectedItem as UMCClusterLight;
        }    
    }
}
