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
using System.IO;

namespace MultiAlign.Windows
{
    /// <summary>
    /// Interaction logic for FilePathFinder.xaml
    /// </summary>
    public partial class FilePathFinder : UserControl
    {
        System.Windows.Forms.OpenFileDialog m_dialog;
        public FilePathFinder()
        {
            InitializeComponent();

            m_dialog = new System.Windows.Forms.OpenFileDialog();
        }



        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(FilePathFinder),
            new PropertyMetadata(delegate (DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    if (sender == null)
                    {
                        sender = null;
                    }
                }));




        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FilePathFinder), new UIPropertyMetadata(""));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            bool isEmpty = string.IsNullOrWhiteSpace(FilePath);

            if (!isEmpty)
            {
                bool doesExist = File.Exists(FilePath);
                if (!doesExist)
                {
                    try
                    {
                        string dirName = System.IO.Path.GetDirectoryName(FilePath);
                        doesExist = Directory.Exists(dirName);
                        if (doesExist)
                        {
                            m_dialog.InitialDirectory = dirName;
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    m_dialog.InitialDirectory = System.IO.Path.GetDirectoryName(FilePath);
                }
            }


            System.Windows.Forms.DialogResult result = m_dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = m_dialog.FileName;
            }
        }
    }
}
