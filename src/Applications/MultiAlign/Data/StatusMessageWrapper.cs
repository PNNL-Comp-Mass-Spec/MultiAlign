using System.Windows;
using System.Windows.Controls;

namespace MultiAlign.Data
{
    public class StatusMessageWrapper : DependencyObject
    {
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof (string), typeof (StatusMessageWrapper));

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Name", typeof (string), typeof (StatusMessageWrapper));

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof (Image), typeof (StatusMessageWrapper));

        public string Description
        {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public string Name
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public Image Image
        {
            get { return (Image) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }
}