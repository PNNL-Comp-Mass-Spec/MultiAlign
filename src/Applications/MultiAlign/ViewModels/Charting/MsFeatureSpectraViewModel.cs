using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Drawing;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MultiAlign.ViewModels.Charting
{
    public sealed class MsFeatureSpectraViewModel : PlotModelBase
    {
        private readonly LinearAxis m_mzAxis;

        public MsFeatureSpectraViewModel(MSFeatureLight feature, IEnumerable<XYData> spectrum, string name)
            : base(name)
        {
            MsmsDistanceLower = 1.5;
            MsmsDistanceUpper = 1.5;

            var mzAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = true,
                MinorStep = 1,
                AbsoluteMinimum = 0
            };

            var intensityAxis = new LinearAxis
            {
                IsPanEnabled = false,
                Position = AxisPosition.Left,
                IsZoomEnabled = true,
                Minimum = 0,
                AbsoluteMinimum = 0,
                UseSuperExponentialFormat = true
            };
            Model.Axes.Add(mzAxis);
            Model.Axes.Add(intensityAxis);

            m_mzAxis = mzAxis;
            PlotSpectra(feature, spectrum);
        }

        public double MsmsDistanceLower { get; set; }

        public double MsmsDistanceUpper { get; set; }

        public void SetXExtrema(double minimumX, double maximumY)
        {
            m_mzAxis.Minimum = minimumX;
            m_mzAxis.Maximum = maximumY;
            Model.InvalidatePlot(true);
        }

        public void PlotSpectra(MSFeatureLight feature, IEnumerable<XYData> spectrum)
        {
            var series = new StemSeries
            {
                Color = OxyColors.Black
            };

            var minimumMz = double.MaxValue;
            var maximumMz = double.MinValue;
            var maxAbundance = double.MinValue;

            if (spectrum.Count() < 1)
                return;

            foreach (var peak in spectrum)
            {
                minimumMz = Math.Min(peak.X, minimumMz);
                maximumMz = Math.Max(peak.X, maximumMz);
                maxAbundance = Math.Max(maxAbundance, peak.Y);

                series.Points.Add(new DataPoint(peak.X, peak.Y));
            }

            var maxAbundanceTop = maxAbundance*.5;

            Model.Axes[0].AbsoluteMinimum = minimumMz;
            Model.Axes[0].AbsoluteMaximum = maximumMz;

            Model.Series.Add(series);

            // Add in the monoisotopic peak
            var colors = new ColorTypeIterator();
            var chargeColor = colors.GetColor(feature.ChargeState);
            var msFeature = new StemSeries
            {
                Color = chargeColor
            };
            msFeature.Points.Add(new DataPoint(feature.Mz, feature.Abundance));
            Model.Series.Add(msFeature);

            // Add in the rest of the isotopes
            var alphaColor = OxyColor.FromAColor(100, OxyColors.Red);
            var charge = feature.ChargeState;
            var mz = feature.Mz;
            var abundance = Convert.ToDouble(feature.Abundance);
            var monoPeakAnnotation = new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = mz,
                Color = alphaColor,
                TextColor = alphaColor,
                Text = string.Format("mono peak: {0} m/z",
                    mz.ToString("F3"))
            };
            Model.Annotations.Add(monoPeakAnnotation);

            var lastMz = mz;
            var spacing = 1.0/charge;
            while (mz < maximumMz && abundance > 1)
            {
                mz = mz + (1.0/charge);
                abundance *= .75;
                var peakAnnotation = new LineAnnotation
                {
                    Type = LineAnnotationType.Vertical,
                    X = mz,
                    Color = alphaColor,
                    TextColor = alphaColor,
                    Text = string.Format("{0} m/z",
                        mz.ToString("F3"))
                };

                var spaceAnnotation = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Color = alphaColor,
                    TextColor = alphaColor,
                    TextHorizontalAlignment = HorizontalAlignment.Center,
                    TextVerticalAlignment = VerticalAlignment.Top,
                    TextPosition = new DataPoint(.5, 0),
                    MinimumX = lastMz,
                    MaximumX = mz,
                    Text = string.Format("d={0}", spacing.ToString("F2")),
                    Y = maxAbundance*.75
                };
                maxAbundance *= .75;
                lastMz = mz;
                Model.Annotations.Add(spaceAnnotation);
                Model.Annotations.Add(peakAnnotation);
            }

            if (feature.GetParentFeature() != null)
            {
                var features = feature.GetParentFeature().Features;
                foreach (var subFeature in features)
                {
                    var msms =
                        subFeature.MSnSpectra.Where(x => x.PrecursorMz > minimumMz && x.PrecursorMz < maximumMz);
                    foreach (var fragmentation  in msms)
                    {
                        var spaceAnnotation = new LineAnnotation
                        {
                            Type = LineAnnotationType.Vertical,
                            Color = OxyColors.Gray,
                            TextColor = OxyColors.Gray,
                            FontWeight = 3,
                            TextVerticalAlignment = VerticalAlignment.Top,
                            TextPosition = new DataPoint(1, 0),
                            StrokeThickness = 2,
                            Text =
                                string.Format("msms {0} - scan {1}", fragmentation.PrecursorMz.ToString("F2"),
                                    fragmentation.Scan),
                            X = fragmentation.PrecursorMz
                        };
                        Model.Annotations.Add(spaceAnnotation);


                        var lowerMz = new LineAnnotation
                        {
                            Type = LineAnnotationType.Horizontal,
                            Color = OxyColors.LightGray,
                            TextColor = OxyColors.LightGray,
                            FontWeight = 3,
                            TextVerticalAlignment = VerticalAlignment.Top,
                            TextPosition = new DataPoint(1, 0),
                            StrokeThickness = 2,
                            Y = maxAbundanceTop,
                            Text = string.Format("{0} m/z", MsmsDistanceLower.ToString("F2")),
                            MinimumX = fragmentation.PrecursorMz - MsmsDistanceLower,
                            MaximumX = fragmentation.PrecursorMz
                        };

                        var upperMz = new LineAnnotation
                        {
                            Type = LineAnnotationType.Horizontal,
                            Color = OxyColors.LightGray,
                            TextColor = OxyColors.LightGray,
                            FontWeight = 3,
                            TextVerticalAlignment = VerticalAlignment.Top,
                            TextPosition = new DataPoint(1, 0),
                            StrokeThickness = 2,
                            Text = string.Format("{0} m/z", MsmsDistanceUpper.ToString("F2")),
                            Y = maxAbundanceTop,
                            MinimumX = fragmentation.PrecursorMz,
                            MaximumX = fragmentation.PrecursorMz + MsmsDistanceUpper
                        };

                        Model.Annotations.Add(upperMz);
                        Model.Annotations.Add(lowerMz);
                    }
                }
            }
        }
    }
}