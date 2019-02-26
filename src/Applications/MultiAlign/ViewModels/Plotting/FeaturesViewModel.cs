using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Drawing;
using PNNLOmics.Annotations;

namespace MultiAlign.ViewModels.Plotting
{
    public class FeaturesViewModel : ViewModelBase
    {
        private int m_height;
        private int m_width;

        public FeaturesViewModel(IEnumerable<UMCLight> features, string text)
        {
            Text = text;
            Height = 800;
            Width = 800;

            FeaturesModel = ScatterPlotFactory.CreateFeatureMassScatterPlot(features);
        }

        [UsedImplicitly]
        public string Text { get; private set; }

        [UsedImplicitly]
        public int Height
        {
            get { return m_height; }
            set
            {
                if (value != m_height)
                {
                    m_height = value;
                    OnPropertyChanged("Height");
                }
            }
        }

        [UsedImplicitly]
        public int Width
        {
            get { return m_width; }
            set
            {
                if (value != m_width)
                {
                    m_width = value;
                    OnPropertyChanged("Width");
                }
            }
        }

        [UsedImplicitly]
        public PlotBase FeaturesModel { get; private set; }
    }
}