using System;
using System.Windows.Media.Imaging;

namespace MultiAlign.ViewModels
{
    public class PictureViewModel:ViewModelBase 
    {
        private int m_height;
        private int m_width;

        public PictureViewModel(BitmapImage image, string text)
        {
            Image  = image;
            Text   = text;
            Height = Math.Max(128, Convert.ToInt32(image.PixelHeight));
            Width  = Math.Max(128, Convert.ToInt32(image.PixelWidth));
        }
        public string Text { get; private set; }
        public BitmapImage Image { get; private set; }
        public int Height 
        {
            get
            {
                return m_height;
            }
            set
            {
                if (value != m_height)
                {
                    m_height = value;
                    OnPropertyChanged("Height");
                }
            }
        }
        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                if (value != m_width)
                {
                    m_width = value;
                    OnPropertyChanged("Width");
                }
            }
        }        
    }
}
