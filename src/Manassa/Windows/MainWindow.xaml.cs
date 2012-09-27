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

using MultiAlignCore.Algorithms;
using MultiAlignCore.IO;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.IO.Features;

namespace Manassa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.OpenFileDialog m_analysisLoadDialog;
        /// <summary>
        /// Object that controls the setup of an analysis and processing.
        /// </summary>
        private AnalysisController      m_controller;
        private AnalysisReportGenerator m_reporter;

        public MainWindow()
        {
            InitializeComponent();

            m_analysisLoadDialog = new System.Windows.Forms.OpenFileDialog();

            m_controller    = new AnalysisController();
            m_reporter      = new AnalysisReportGenerator();
        }

        private void LoadAnalysisMenuButton_Click(object sender, RoutedEventArgs e)
        {            
            System.Windows.Forms.DialogResult result = m_analysisLoadDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LoadMultiAlignFile(m_analysisLoadDialog.FileName);
            }
        }


        private void CleanupMultiAlignController()
        {
            if (m_controller != null)
            {
                m_controller = null;
            }
        }
        private void LoadMultiAlignFile(string filename)
        {
            // Cleanup old ties if necessary.
            CleanupMultiAlignController();
            
            // Create a new controller
            m_controller = new AnalysisController();                                    
            m_controller.LoadExistingAnalysis(filename, m_reporter);
            m_controller.Config.Analysis.MetaData.AnalysisPath = filename;

            m_mainControl.IsEnabled = true;
            m_mainControl.Analysis  = m_controller.Config.Analysis;            
        }
    }
}
