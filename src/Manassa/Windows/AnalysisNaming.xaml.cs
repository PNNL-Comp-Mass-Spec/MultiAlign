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
using System.Windows.Shapes;
using MultiAlignCore.Data;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for AnalysisNaming.xaml
    /// </summary>
    public partial class AnalysisNaming : UserControl
    {
        public AnalysisNaming()
        {
            InitializeComponent();

            DataContext = this;
        }

        public MultiAlignAnalysis Analysis
        {
            get { return (MultiAlignAnalysis)GetValue(AnalysisProperty); }
            set { SetValue(AnalysisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Analysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisProperty =
            DependencyProperty.Register("Analysis", typeof(MultiAlignAnalysis), typeof(AnalysisNaming));

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {

        }        
    }
}
