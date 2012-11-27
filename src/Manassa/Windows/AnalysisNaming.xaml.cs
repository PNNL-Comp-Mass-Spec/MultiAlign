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

using System.IO;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for AnalysisNaming.xaml
    /// </summary>
    public partial class AnalysisNaming : UserControl
    {
        System.Windows.Forms.FolderBrowserDialog m_folderBrowser;

        public AnalysisNaming()
        {
            InitializeComponent();

            DataContext     = this;
            m_folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
        }

        public AnalysisConfig AnalysisConfiguration
        {
            get { return (AnalysisConfig)GetValue(AnalysisProperty); }
            set { SetValue(AnalysisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Analysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisProperty =
            DependencyProperty.Register("AnalysisConfiguration", typeof(AnalysisConfig), typeof(AnalysisNaming));

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string path = AnalysisConfiguration.AnalysisPath;
            if (Directory.Exists(path))
            {
                m_folderBrowser.SelectedPath = path;
            }

            System.Windows.Forms.DialogResult result = m_folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                AnalysisConfiguration.AnalysisPath = m_folderBrowser.SelectedPath;
            }
        }        
    }
}
