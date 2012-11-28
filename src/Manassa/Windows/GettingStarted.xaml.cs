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
using Manassa.Workspace;
using Manassa.Data;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for GettingStarted.xaml
    /// </summary>
    public partial class GettingStarted : UserControl
    {
        public event EventHandler<OpenAnalysisArgs> RecentAnalysisSelected;

        public GettingStarted()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// Gets or sets the current work space item
        /// </summary>        
        public ManassaWorkspace CurrentWorkspace
        {
            get { return (ManassaWorkspace)GetValue(CurrentWorkSpaceProperty); }
            set { SetValue(CurrentWorkSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentWorkSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentWorkSpaceProperty =
            DependencyProperty.Register("CurrentWorkspace", typeof(ManassaWorkspace), typeof(GettingStarted));

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RecentAnalysis analysis = e.Parameter as RecentAnalysis;
            if (analysis == null)
            {
                return;
            }

            if (RecentAnalysisSelected != null)
            {
                RecentAnalysisSelected(this, new OpenAnalysisArgs(analysis));
            }
        }
    }

    public class OpenAnalysisArgs : EventArgs
    {
        public OpenAnalysisArgs(RecentAnalysis analysis)
        {
            AnalysisData = analysis;
        }

        public RecentAnalysis AnalysisData
        {
            get;
            private set;
        }
    }
}
