using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MultiAlignCore.Data;

namespace MultiAlign.Windows.Viewers.Proteins
{
    /// <summary>
    /// Interaction logic for
    /// </summary>
    public partial class ProteinGrid : UserControl
    {
        public static readonly DependencyProperty SelectedMassTagProperty =
            DependencyProperty.Register("SelectedProtein", typeof (ProteinToMassTags), typeof (ProteinGrid));

        private List<ProteinToMassTags> m_proteins;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProteinGrid()
        {
            InitializeComponent();
            Proteins = new List<ProteinToMassTags>();
        }

        /// <summary>
        /// Gets or sets the clusters used in the analysis.
        /// </summary>
        public List<ProteinToMassTags> Proteins
        {
            get { return m_proteins; }
            set
            {
                m_proteins = value;
                if (value != null)
                {
                    m_dataGrid.ItemsSource = value;
                }
            }
        }

        public ProteinToMassTags SelectedProtein
        {
            get { return (ProteinToMassTags) GetValue(SelectedMassTagProperty); }
            set { SetValue(SelectedMassTagProperty, value); }
        }


        private void m_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProtein = m_dataGrid.SelectedItem as ProteinToMassTags;
        }
    }
}