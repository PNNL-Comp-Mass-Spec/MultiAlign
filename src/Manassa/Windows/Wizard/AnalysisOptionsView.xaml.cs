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
using MultiAlignCore.IO;
using MultiAlignCore.IO.Parameters;

namespace MultiAlign.Windows.Wizard
{
    /// <summary>
    /// Interaction logic for AnalysisOptions.xaml
    /// </summary>
    public partial class AnalysisOptionsView : UserControl
    { 
        /// <summary>
        /// Open file dialog for opening an existing parameter file.
        /// </summary>
        private System.Windows.Forms.OpenFileDialog m_dialog;

        public AnalysisOptionsView()
        {
            InitializeComponent();
            m_editor.HideCloseButton();
            m_editor.HideSaveButton();

            m_dialog = new System.Windows.Forms.OpenFileDialog();
            m_dialog.Filter = "MultiAlign Parameters (*.xml)| *.xml|All Files (*.*)|*.*";
        }

        private void OpenExistingParameterFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.DialogResult result = m_dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    XMLParamterFileReader reader = new XMLParamterFileReader();
                    MultiAlignCore.Data.MultiAlignAnalysis analysis = new MultiAlignCore.Data.MultiAlignAnalysis();
                    reader.ReadParameterFile(m_dialog.FileName, ref analysis);
                    Options = analysis.Options;
                    m_editor.SetOptions(Options, m_dialog.FileName);               
                }
                else
                {                    
                }
            }
            catch (Exception ex)
            {                
            }
        }



        public AnalysisOptions Options
        {
            get { return (AnalysisOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Options.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(AnalysisOptions), typeof(AnalysisOptionsView),
            new PropertyMetadata(delegate (DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    var x = sender as AnalysisOptionsView;

                    if (x == null)
                        return;

                    x.m_editor.SetOptions(x.Options, "");
                }));        
    }
}
