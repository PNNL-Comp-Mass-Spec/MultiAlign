using FeatureAlignment.Data;
using MultiAlignCore.Drawing;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MultiAlign.ViewModels.Charting
{
    public class MsMsSpectraViewModel : PlotModelBase
    {
        private readonly LinearAxis m_intensityAxis;
        private readonly LinearAxis m_mzAxis;
        private MSSpectra m_spectrum;

        public MsMsSpectraViewModel(MSSpectra spectrum, string name) :
            base(name)
        {
            var mzAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = true,
                Minimum = 0,
                MaximumPadding = .01,
                AbsoluteMinimum = 0
            };

            var intensityAxis = new LinearAxis
            {
                IsPanEnabled = false,
                Position = AxisPosition.Left,
                IsZoomEnabled = true,
                Minimum = 0,
                AbsoluteMinimum = 0,
                MaximumPadding = .01,
                UseSuperExponentialFormat = true
            };
            Model.Axes.Add(mzAxis);
            Model.Axes.Add(intensityAxis);

            m_mzAxis = mzAxis;
            m_intensityAxis = intensityAxis;

            m_spectrum = spectrum;
            PlotSpectra(spectrum);
        }

        public void PlotSpectra(MSSpectra spectrum)
        {
            Model.Series.Clear();

            var colorIterator = new ColorTypeIterator();
            var charge = 0;
            if (spectrum.ParentFeature != null)
            {
                charge = spectrum.ParentFeature.ChargeState;
            }
            var series = new StemSeries
            {
                Title = string.Format("{0} m/z charge {1}",
                    spectrum.PrecursorMz,
                    charge),
                Color = colorIterator.GetColor(charge)
            };

            foreach (var peak in spectrum.Peaks)
            {
                series.Points.Add(new DataPoint(peak.X, peak.Y));
            }
            Model.Series.Add(series);
        }

        public void SetMax(double maxMz, double maxIntensity)
        {
            m_mzAxis.AbsoluteMaximum = maxMz;
            m_intensityAxis.AbsoluteMaximum = maxIntensity;
            m_mzAxis.Maximum = maxMz;
            m_intensityAxis.Maximum = maxIntensity;
        }
    }
}