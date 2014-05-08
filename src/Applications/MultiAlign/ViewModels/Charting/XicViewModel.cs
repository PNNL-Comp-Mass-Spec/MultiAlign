using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;
using PNNLOmicsViz.Drawing;

namespace MultiAlign.ViewModels.Charting
{
    public class XicViewModel : PlotModelBase
    {
        private ColorTypeIterator m_colorIterator;
        private bool m_movingMouse;
        private LineAnnotation m_scanAnnotation;
        private int m_selectedCharge;
        private double m_selectedScan;

        public XicViewModel(IEnumerable<UMCLight> features, string name) :
            base(name)
        {
            var intensityAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                AbsoluteMinimum = 0,
                UseSuperExponentialFormat = true,
                Title = "Intensity"
            };


            var scanAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = true,
                Title = "Scan"
            };
            Model.Axes.Add(scanAxis);
            Model.Axes.Add(intensityAxis);

            Model.MouseDown += Model_MouseDown;
            Model.MouseMove += Model_MouseMove;
            Model.MouseUp += Model_MouseUp;
            PlotFeatures(features);
        }

        public double ScanAnnotationX
        {
            get { return m_selectedScan; }

            set
            {
                m_selectedScan = value;

                if (m_scanAnnotation == null) return;
                m_scanAnnotation.X = value;
                m_scanAnnotation.Text = value.ToString("F0");
                Model.Update(false);
            }
        }

        public int SelectedCharge
        {
            get { return m_selectedCharge; }
            set
            {
                m_selectedCharge = value;
                if (m_scanAnnotation == null) return;

                OxyColor color = m_colorIterator.GetColor(value);
                OxyColor newColor = OxyColor.FromAColor(120, color);

                m_scanAnnotation.Color = newColor;
                m_scanAnnotation.TextColor = newColor;
                Model.Update(false);
            }
        }

        public event EventHandler<PositionArgs> PointClicked;

        private void Model_MouseUp(object sender, OxyMouseEventArgs e)
        {
            //if (e == OxyMouseButton.Left)
            {
                m_movingMouse = false;
            }
        }

        private void Model_MouseMove(object sender, OxyMouseEventArgs e)
        {
            //if (e.ChangedButton == OxyMouseButton.Left)
            {
                if (!m_movingMouse) return;

                DataPoint point = FindPoint(e.Position);
                if (PointClicked != null)
                    PointClicked(this, new PositionArgs(point.X, point.Y));
                ScanAnnotationX = point.X;
            }
        }

        private void Model_MouseDown(object sender, OxyMouseEventArgs e)
        {
            // if (e.ChangedButton == OxyMouseButton.Left)
            {
                m_movingMouse = true;

                DataPoint point = FindPoint(e.Position);

                if (PointClicked != null)
                    PointClicked(this, new PositionArgs(point.X, point.Y));
                ScanAnnotationX = point.X;
            }
        }

        private DataPoint FindPoint(ScreenPoint point)
        {
            foreach (Series series in Model.Series)
            {
                var charge = (int) series.Tag;
                if (charge == m_selectedCharge)
                {
                    TrackerHitResult dataPoint = series.GetNearestPoint(point, false);
                    return dataPoint.DataPoint;
                }
            }
            return new DataPoint();
        }

        /// <summary>
        ///     Plots the UMC's
        /// </summary>
        /// <param name="features"></param>
        private void PlotFeatures(IEnumerable<UMCLight> features)
        {
            var markerIterator = new MarkerTypeIterator();
            m_colorIterator = new ColorTypeIterator();

            int i = 0;

            m_scanAnnotation = new LineAnnotation
            {
                X = 0,
                TextColor = OxyColors.Gray,
                Text = "0",
                TextOrientation = AnnotationTextOrientation.Vertical,
                LineStyle = LineStyle.Dash,
                Type = LineAnnotationType.Vertical,
            };


            Model.Annotations.Add(m_scanAnnotation);

            foreach (UMCLight feature in features)
            {
                Dictionary<int, List<MSFeatureLight>> chargeMap = feature.CreateChargeMap();

                foreach (int charge in chargeMap.Keys)
                {
                    List<MSFeatureLight> msFeatures = chargeMap[charge];
                    msFeatures = msFeatures.OrderBy(x => x.Scan).ToList();
                    double mz = msFeatures[0].Mz;

                    var newSeries = new LineSeries
                    {
                        Color = m_colorIterator.GetColor(charge),
                        MarkerFill = m_colorIterator.GetColor(charge),
                        MarkerSize = 3,
                        MarkerStroke = OxyColors.White,
                        MarkerStrokeThickness = 1.5,
                        MarkerType = markerIterator.GetMarker(i++),
                        Title = string.Format("{0} m/z  - Charge {1}",
                            mz.ToString("F3"),
                            charge)
                    };

                    long abundance = 0;
                    MSFeatureLight bestFeature = null;

                    foreach (MSFeatureLight msFeature in msFeatures)
                    {
                        if (abundance < msFeature.Abundance)
                        {
                            bestFeature = msFeature;
                            abundance = msFeature.Abundance;
                        }

                        foreach (MSSpectra msms in msFeature.MSnSpectra)
                        {
                            string peptideSequence = "";
                            if (msms.Peptides.Count > 0)
                                peptideSequence = msms.Peptides[0].Sequence;

                            var msmsAnnotation = new LineAnnotation
                            {
                                Type = LineAnnotationType.Vertical,
                                X = msms.Scan,
                                Y = msFeature.Abundance,
                                StrokeThickness = 2,
                                Color = m_colorIterator.GetColor(msFeature.ChargeState),
                                TextColor = m_colorIterator.GetColor(msFeature.ChargeState),
                                Text = string.Format("{2} - {0} m/z {1}",
                                    msms.PrecursorMz.ToString("F3"),
                                    peptideSequence,
                                    msms.CollisionType)
                            };
                            Model.Annotations.Add(msmsAnnotation);
                        }
                        newSeries.Points.Add(new DataPoint(msFeature.Scan, msFeature.Abundance));
                    }

                    newSeries.Tag = charge;

                    if (bestFeature != null)
                    {
                        ScanAnnotationX = bestFeature.Scan;
                        SelectedCharge = charge;
                    }
                    Model.Series.Add(newSeries);
                }
            }
        }
    }
}