using System;
using System.Collections.Generic;
using System.Windows;
using MultiAlign.IO;
using MultiAlign.ViewModels.Plotting;
using MultiAlign.Windows.Plots;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;

namespace MultiAlign.Commands.Plotting
{
    public class FeatureDisplayCommand : BaseCommand
    {
        private readonly DatasetInformation m_information;
        private readonly string m_name;
        private Window m_window;

        public FeatureDisplayCommand(DatasetInformation information)
            : base(null, AlwaysPass)
        {
            m_information = information;
            m_window = null;
            m_name = "Features " + information.DatasetName;
        }

        public override void Execute(object parameter)
        {
            if (m_information != null)
            {
                if (m_window == null)
                {
                    List<UMCLight> features =
                        SingletonDataProviders.Providers.FeatureCache.FindByDatasetId(m_information.DatasetId);
                    if (features == null || features.Count < 1)
                        return;

                    var window = new LargeFeatureView();
                    var viewModel = new FeaturesViewModel(features, m_name);
                    window.DataContext = viewModel;
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Show();

                    m_window = window;
                    m_window.Closed += m_window_Closed;
                }

                m_window.BringIntoView();
            }
        }


        private void m_window_Closed(object sender, EventArgs e)
        {
            m_window = null;
        }
    }
}