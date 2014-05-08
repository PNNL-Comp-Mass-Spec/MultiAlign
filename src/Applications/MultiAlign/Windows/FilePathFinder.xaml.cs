using System.IO;
using System.Windows;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;

namespace MultiAlign.Windows
{
    /// <summary>
    ///     Interaction logic for FilePathFinder.xaml
    /// </summary>
    public partial class FilePathFinder : UserControl
    {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof (string), typeof (FilePathFinder),
                new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    if (sender == null)
                    {
                        sender = null;
                    }
                }));

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof (string), typeof (FilePathFinder), new UIPropertyMetadata(""));

        private readonly OpenFileDialog m_dialog;

        public FilePathFinder()
        {
            InitializeComponent();

            m_dialog = new OpenFileDialog();
        }


        public string LabelText
        {
            get { return (string) GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelText.  This enables animation, styling, binding, etc...


        public string FilePath
        {
            get { return (string) GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...

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
                        string dirName = Path.GetDirectoryName(FilePath);
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
                    m_dialog.InitialDirectory = Path.GetDirectoryName(FilePath);
                }
            }


            DialogResult result = m_dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                FilePath = m_dialog.FileName;
            }
        }
    }
}