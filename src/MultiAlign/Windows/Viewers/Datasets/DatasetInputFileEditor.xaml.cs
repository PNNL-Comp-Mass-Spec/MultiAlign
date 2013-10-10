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

namespace MultiAlign.Windows.Viewers.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetInputFileEditor.xaml
    /// </summary>
    public partial class DatasetInputFileEditor : UserControl
    {
        public DatasetInputFileEditor()
        {
            InitializeComponent();
        }

        public bool  IsReadOnly
        {
            get { return (bool )GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DatasetInputFileEditor), new UIPropertyMetadata(false));

        public DatasetInformation Dataset
        {
            get { return (DatasetInformation)GetValue(DatasetProperty); }
            set { SetValue(DatasetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dataset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatasetProperty =
            DependencyProperty.Register("Dataset", typeof(DatasetInformation), typeof(DatasetInputFileEditor));
    }
}
