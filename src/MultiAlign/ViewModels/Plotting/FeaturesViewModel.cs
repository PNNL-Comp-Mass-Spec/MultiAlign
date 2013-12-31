using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.Windows.Forms.Integration;
using MultiAlignCustomControls.Charting;
using System.Drawing;
using MultiAlignCustomControls.Drawing;
using PNNLControls;

namespace MultiAlign.ViewModels.Plotting
{
    public class FeaturesViewModel: ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        WindowsFormsHost m_host;
        private int m_height;
        private int m_width;
        private IEnumerable<UMCLight> m_features;
        /// <summary>
        /// Scatter plot for features 
        /// </summary>
        ctlChartBase m_chart;

        public FeaturesViewModel(IEnumerable<UMCLight> features, string text)
        {
            m_features = new List<UMCLight> ();
            Features   = m_features;
            Text       = text;
            Height     = 800;
            Width      = 800;

            m_chart             = new UMCScatterChart();
            
            ChartDisplayOptions options = new ChartDisplayOptions(true, true, true, true);            
            options.MarginMin   = 1;
            options.MarginMax   = 100;
            options.Title       = text;
            options.XAxisLabel  = "Scan";
            options.YAxisLabel  = "Monoisotopic Mass";

            m_chart             = RenderDatasetInfo.FeaturesScatterPlot_Chart(features.ToList(), options);
            m_chart.Height      = Height;
            m_chart.Width       = Width;

            m_chart.AutoViewPort();            
            RectangleF viewport = m_chart.ViewPort;
            m_chart.ViewPort    = new System.Drawing.RectangleF(Math.Min(0, viewport.X),
                                                             viewport.Y,
                                                             Math.Max(1.0F, viewport.Width),
                                                             viewport.Height);

            FeaturesHost        = new WindowsFormsHost() { Child = m_chart };
        }
        public IEnumerable<UMCLight> Features
        {
            get
            {
                return m_features;
            }
            set
            {
                if (value != m_features)
                {
                    m_features = value;
                    OnPropertyChanged("Features");
                }
            }
        }
        public string Text
        { get; private set; }
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


        public WindowsFormsHost FeaturesHost
        {
            get
            {
                return m_host;
            }
            set
            {
                if (value != null && m_host != value)
                {
                    m_host = value;
                    OnPropertyChanged("FeaturesHost");
                }
            }

        }
    }
}
