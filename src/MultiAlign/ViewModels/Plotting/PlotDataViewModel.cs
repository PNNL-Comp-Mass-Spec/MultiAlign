using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.MetaData;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows;
using MultiAlign.Commands.Plotting;

namespace MultiAlign.ViewModels.Plotting
{
    /// <summary>
    /// Displays a plot information 
    /// </summary>
    public class PlotViewModel: ViewModelBase 
    {
        private string m_name;

        public PlotViewModel(string path):
            this(path, "")
        {
        }

        public PlotViewModel(string path, string name):
            this(path, name, new PictureDisplayCommand(path, name) as ICommand)
        {         
        }

        public PlotViewModel(string path, string name, ICommand command)
        {
            Path    = path;
            m_name  = name;
            Command = command;
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (m_name != value)
                {
                    m_name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string Path
        {
            get;
            private set;
        }

        public ICommand Command
        {
            get;
            private set;
        }

    }
    
}
