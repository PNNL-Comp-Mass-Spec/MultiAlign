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

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for ClusterGrid.xaml
    /// </summary>
    public partial class MassTagGrid : UserControl
    {
        private BindingList<MassTagLight> m_massTags;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MassTagGrid()
        {
            InitializeComponent();
            MassTags = new BindingList<MassTagLight>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public BindingList<MassTagLight> MassTags
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

        public MassTagLight SelectedTag
        {
            get { return (MassTagLight)GetValue(SelectedMassTagProperty); }
            set { SetValue(SelectedMassTagProperty, value); }
        }        
        public static readonly DependencyProperty SelectedMassTagProperty =
            DependencyProperty.Register("SelectedTag", typeof(MassTagLight), typeof(MassTagGrid)); 

       
        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTag = m_dataGrid.SelectedItem as MassTagLight;
        }    
    }
}
