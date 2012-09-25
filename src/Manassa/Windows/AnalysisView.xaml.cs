using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
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
using PNNLOmics.Data.Features;
using MultiAlignCore.IO;

using MultiAlignCore.Algorithms;
using MultiAlignCore.IO;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.IO.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for AllClustersControl.xaml
    /// </summary>
    public partial class AllClustersControl : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private MultiAlignAnalysis m_analysis;
        

        public AllClustersControl()
        {
            InitializeComponent();

            m_analysis = null;
        }

        /// <summary>
        /// Gets or sets the Analysis
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get
            {
                return m_analysis;
            }
            set
            {
                m_analysis = value;
                if (value != null)
                {                    
                    m_clusterGrid.Clusters = new BindingList<UMCClusterLight>(value.Clusters);                    
                    m_clusterPlot.SetClusters(value.Clusters);
                    m_clusterControl.Providers = m_analysis.DataProviders;
                }
            }
        }
    }
}
