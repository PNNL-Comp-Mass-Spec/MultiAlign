using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MultiAlignCore.Data;
using MultiAlign.IO;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;
using MultiAlign.Windows.Plots;
using System.Windows;
using MultiAlign.ViewModels.Plotting;

namespace MultiAlign.Commands.Plotting
{
    public class FeatureDisplayCommand: ICommand
    {
        private Window m_window;
        private DatasetInformation m_information;
        private string m_name;

        public FeatureDisplayCommand(DatasetInformation information)
        {
            m_information   = information;
            m_window        = null;
            m_name          = "Features " + information.DatasetName;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            
            if (m_information != null)
            {
                if (m_window == null)
                {
                    List<UMCLight> features = SingletonDataProviders.Providers.FeatureCache.FindByDatasetId(m_information.DatasetId);
                    if (features == null || features.Count < 1)
                        return;

                    LargeFeatureView window         = new LargeFeatureView();
                    FeaturesViewModel viewModel     = new FeaturesViewModel(features, m_name);
                    window.DataContext              = viewModel;
                    window.WindowStartupLocation    = WindowStartupLocation.CenterScreen;
                    window.Show();
                    
                    m_window         = window;
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
