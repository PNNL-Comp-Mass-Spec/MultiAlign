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

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for InputFileType.xaml
    /// </summary>
    public partial class InputFileType : UserControl
    {
        public InputFileType()
        {
            InitializeComponent();

            LabelName = "Raw File";
            Extension = ".raw";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public string LabelName
        {
            get { return (string)GetValue(LabelNameProperty); }
            set { SetValue(LabelNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelNameProperty =
            DependencyProperty.Register("LabelName", typeof(string), typeof(InputFileType));

        public string Extension
        {
            get { return (string)GetValue(ExtensionProperty); }
            set { SetValue(ExtensionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionProperty =
            DependencyProperty.Register("Extension", typeof(string), typeof(InputFileType));

        
    }
}
