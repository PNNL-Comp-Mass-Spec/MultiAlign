using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            
            var isEmpty = string.IsNullOrWhiteSpace(FilePath);

            if (!isEmpty)
            {
                var doesExist = File.Exists(FilePath);
                if (!doesExist)
                {
                    try
                    {
                        var dirName = System.IO.Path.GetDirectoryName(FilePath);
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


            var result = m_dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = m_dialog.FileName;
            }
        }
    }
}
