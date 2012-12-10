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
using PNNLOmics.Data;

namespace Manassa.Windows.Viewers.Proteins
{
    /// <summary>
    /// Interaction logic for MassTagDetail.xaml
    /// </summary>
    public partial class ProteinDetail : UserControl
    {
        public ProteinDetail()
        {
            InitializeComponent();
        }

        public Protein Protein
        {
            get { return (Protein)GetValue(ProteinProperty); }
            set { SetValue(ProteinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassTag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProteinProperty =
            DependencyProperty.Register("Protein", typeof(Protein), typeof(ProteinDetail));


    }
}
