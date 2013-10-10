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

namespace MultiAlign.Windows.Viewers.MassTags
{
    /// <summary>
    /// Interaction logic for MassTagDetail.xaml
    /// </summary>
    public partial class MassTagDetail : UserControl
    {
        public MassTagDetail()
        {
            InitializeComponent();
            DataContext = this; 
        }

        public MassTagToCluster MassTag
        {
            get { return (MassTagToCluster)GetValue(MassTagProperty); }
            set { SetValue(MassTagProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassTag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MassTagProperty =
            DependencyProperty.Register("MassTag", typeof(MassTagToCluster), typeof(MassTagDetail));
    }
}
