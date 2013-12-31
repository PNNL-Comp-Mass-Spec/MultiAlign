using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using MultiAlign.ViewModels;
using System.IO;
using MultiAlign.Windows.Plots;

namespace MultiAlign.Commands.Plotting
{

    public class PictureDisplayCommand : ICommand
    {
        private Window m_window;
        private string m_path;
        private string m_name;

        public PictureDisplayCommand(string path, string name)
        {
            m_path   = path;
            m_name   = name;
            m_window = null;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (m_path != null)
            {
                if (m_window == null)
                {                    
                    LargeImageView window = new LargeImageView();

                    if (!File.Exists(m_path))
                        return;

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(m_path);
                    bi.EndInit();

                    PictureViewModel viewModel = new PictureViewModel(bi, m_name);
                    window.DataContext = viewModel;
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Show();

                    m_window = window;
                    m_window.Closed += new EventHandler(m_window_Closed);
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
