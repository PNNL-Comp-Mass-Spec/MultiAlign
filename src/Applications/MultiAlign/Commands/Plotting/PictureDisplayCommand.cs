using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using MultiAlign.ViewModels;
using MultiAlign.Windows.Plots;

namespace MultiAlign.Commands.Plotting
{

    public sealed class PictureDisplayCommand : BaseCommand
    {
        private Window m_window;
        private readonly string m_path;
        private readonly string m_name;

        public PictureDisplayCommand(string path, string name)
            : base(null, AlwaysPass)
        {
            m_path   = path;
            m_name   = name;
            m_window = null;
        }

        public override void Execute(object parameter)
        {
            if (m_path != null)
            {
                if (m_window == null)
                {                    
                    var window = new LargeImageView();

                    if (!File.Exists(m_path))
                        return;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(m_path);
                    bi.EndInit();

                    var viewModel = new PictureViewModel(bi, m_name);
                    window.DataContext = viewModel;
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Show();

                    m_window = window;
                    m_window.Closed += m_window_Closed;
                }
                m_window.BringIntoView();
            }
        }

        void m_window_Closed(object sender, EventArgs e)
        {
            m_window = null;
        }
    }
}
