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

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for PerformAnalysisWindow.xaml
    /// </summary>
    public partial class PerformAnalysisWindow : Window
    {
        public PerformAnalysisWindow()
        {
            InitializeComponent();

            DatasetCount        = 0;
            ParameterFileName   = "No Parameter File Chosen";
        }

        public int DatasetCount
        {
            get { return (int)GetValue(DatasetCountProperty); }
            set { SetValue(DatasetCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatasetCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatasetCountProperty =
            DependencyProperty.Register("DatasetCount", typeof(int), typeof(PerformAnalysisWindow), new UIPropertyMetadata(0));

        public string ParameterFileName
        {
            get { return (string)GetValue(ParameterFileNameProperty); }
            set { SetValue(ParameterFileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParameterFileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParameterFileNameProperty =
            DependencyProperty.Register("ParameterFileName", typeof(string), typeof(PerformAnalysisWindow));

        
        #region Event Handlers
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadFromPreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
