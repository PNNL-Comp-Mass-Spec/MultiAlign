#region

using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data.Features;
using OxyPlot;

#endregion

namespace MultiAlignCore.Drawing
{
    public static class ScatterPlotFactory
    {
        #region Cluster Scatter Plots

        /// <summary>
        /// Creates a scatter plot for clusters showing NET and Mono Mass.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static PlotBase CreateClusterMassScatterPlot<T>(IEnumerable<T> clusters)
            where T : UMCClusterLight
        {
            Func<T, double> mass = t => t.MassMonoisotopicAligned;
            Func<T, double> net = t => t.Net;

            return CreateClustersScatterPlot(clusters, net, mass, "NET", "Monoisotopic Mass", OxyColors.Crimson);
        }

        /// <summary>
        /// Creates a scatter plot for clusters showing NET and Mono Mass.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static PlotBase CreateClusterDriftTimeScatterPlot<T>(IEnumerable<T> clusters)
            where T : UMCClusterLight
        {
            Func<T, double> mass = t => t.MassMonoisotopicAligned;
            Func<T, double> driftTime = t => t.DriftTime;

            return CreateClustersScatterPlot(clusters, driftTime, mass, "Drift Time", "Monoisotopic Mass",
                OxyColors.Crimson);
        }

        public static PlotBase CreateClustersScatterPlot<T>(IEnumerable<T> features,
            Func<T, double> timeSelector,
            Func<T, double> massSelector,
            string timeLabel,
            string massLabel,
            OxyColor color
            )
            where T : FeatureLight
        {
            var x = new List<double>();
            var y = new List<double>();
            foreach (var feature in features)
            {
                x.Add(timeSelector(feature));
                y.Add(massSelector(feature));
            }
            var plot = new ScatterPlot("Clusters", timeLabel, massLabel);
            plot.AddSeries(x, y, "Clusters ", color);
            plot.Model.Axes[0].MajorGridlineStyle = LineStyle.Solid;
            plot.Model.Axes[1].MajorGridlineStyle = LineStyle.Solid;
            return plot;
        }

        #endregion

        #region Feature Scatter Plots

        /// <summary>
        /// Creates a feature scatter plot using mono mass aligned as a function of NET aligned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <returns></returns>
        public static PlotBase CreateFeatureMassScatterPlot<T>(IEnumerable<T> features)
            where T : FeatureLight
        {
            Func<T, double> mass = t => t.MassMonoisotopicAligned;
            Func<T, double> net = t => t.NetAligned;

            return CreateFeatureScatterPlot(features, net, mass, "NET Aligned", "Monoisotopic Mass");
        }

        /// <summary>
        /// Creates a feature scatter plot using mono mass aligned as a function of NET aligned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <returns></returns>
        public static PlotBase CreateFeatureDriftTimeScatterPlot<T>(IEnumerable<T> features)
            where T : FeatureLight
        {
            Func<T, double> mass = t => t.MassMonoisotopicAligned;
            Func<T, double> driftTime = t => t.DriftTime;

            return CreateFeatureScatterPlot(features, driftTime, mass, "Drift Time", "Monoisotopic Mass");
        }

        /// <summary>
        /// Creates a feature scatter plot using m/z as a function of NET aligned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <returns></returns>
        public static PlotBase CreateFeatureMzScatterPlot<T>(IEnumerable<T> features)
            where T : MSFeatureLight
        {
            Func<T, double> mass = t => t.Mz;
            Func<T, double> net = t => t.NetAligned;

            return CreateFeatureScatterPlot(features, net, mass, "NET Aligned", "Mz");
        }

        /// <summary>
        /// Creates a feature plot enumerating each charge state with a different color.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <param name="timeSelector"></param>
        /// <param name="massSelector"></param>
        /// <param name="timeLabel"></param>
        /// <param name="massLabel"></param>
        /// <returns></returns>
        public static PlotBase CreateFeatureScatterPlot<T>(IEnumerable<T> features,
            Func<T, double> timeSelector,
            Func<T, double> massSelector,
            string timeLabel,
            string massLabel
            )
            where T : FeatureLight
        {
            // Map charge states.
            var chargeMap = new Dictionary<int, IList<T>>();
            foreach (var feature in features)
            {
                var charge = feature.ChargeState;
                if (!chargeMap.ContainsKey(charge))
                {
                    chargeMap.Add(charge, new List<T>());
                }
                chargeMap[charge].Add(feature);
            }

            var colorIterator = new ColorTypeIterator();
            var plot = new ScatterPlot("Features", timeLabel, massLabel);
            foreach (var charge in chargeMap.Keys)
            {
                var x = new List<double>();
                var y = new List<double>();
                foreach (var feature in chargeMap[charge])
                {
                    x.Add(timeSelector(feature));
                    y.Add(massSelector(feature));
                }
                var color = colorIterator.GetColor(charge);
                plot.AddSeries(x, y, "Charge " + charge, color);
            }
            plot.Model.Axes[0].MajorGridlineStyle = LineStyle.Solid;
            plot.Model.Axes[1].MajorGridlineStyle = LineStyle.Solid;

            return plot;
        }

        #endregion

        #region Residual Plots

        public static PlotBase CreateNetResidualAlignmentPlot<T>(IEnumerable<T> x, IEnumerable<T> y)
            where T : FeatureLight
        {
            Func<T, double> net = t => t.Net;
            Func<T, T, double> netPre = (t, u) => t.Net - u.MassMonoisotopic;
            Func<T, T, double> netPost = (t, u) => t.Net - u.NetAligned;

            return CreateResidualAlignmentPlot(x,
                y,
                net,
                netPre,
                netPost,
                "NET",
                "NET Residual");
        }

        public static PlotBase CreateMassMzResidualAlignmentPlot<T>(IEnumerable<T> x, IEnumerable<T> y)
            where T : FeatureLight
        {
            Func<T, double> mz = t => t.Mz;
            Func<T, T, double> massPre =
                (t, u) => FeatureLight.ComputeMassPPMDifference(t.MassMonoisotopic, u.MassMonoisotopic);
            Func<T, T, double> massPost =
                (t, u) => FeatureLight.ComputeMassPPMDifference(t.MassMonoisotopic, u.MassMonoisotopicAligned);

            return CreateResidualAlignmentPlot(x,
                y,
                mz,
                massPre,
                massPost,
                "mz",
                "Mass Residual (ppm)");
        }

        public static PlotBase CreateMassScanResidualAlignmentPlot<T>(IEnumerable<T> x, IEnumerable<T> y)
            where T : FeatureLight
        {
            Func<T, double> scan = t => t.Scan;
            Func<T, T, double> massPre =
                (t, u) => FeatureLight.ComputeMassPPMDifference(t.MassMonoisotopic, u.MassMonoisotopic);
            Func<T, T, double> massPost =
                (t, u) => FeatureLight.ComputeMassPPMDifference(t.MassMonoisotopic, u.MassMonoisotopicAligned);

            var plot = CreateResidualAlignmentPlot(x,
                y,
                scan,
                massPre,
                massPost,
                "scan",
                "Mass Residual (ppm)");

            return plot;
        }

        /// <summary>
        /// Creates a residual alignment plot from the features provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="abscissaFunc"></param>
        /// <param name="ordinateFuncPre"></param>
        /// <param name="ordinateFuncPost"></param>
        /// <param name="xLabel"></param>
        /// <param name="yLabel"></param>
        /// <returns></returns>
        public static PlotBase CreateResidualAlignmentPlot<T>(IEnumerable<T> x,
            IEnumerable<T> y,
            Func<T, double> abscissaFunc,
            Func<T, T, double> ordinateFuncPre,
            Func<T, T, double> ordinateFuncPost,
            string xLabel,
            string yLabel)
            where T : FeatureLight
        {
            var xPoints = x as T[] ?? x.ToArray();
            var yPoints = y as T[] ?? y.ToArray();

            var colorIterator = new ColorTypeIterator();
            var plot = new ScatterPlot("", xLabel, yLabel);

            if (xPoints.Length != yPoints.Length)
                throw new Exception("The two data arrays must be of equal length.");

            var preColor = colorIterator.GetColor(1);
            var postColor = colorIterator.GetColor(2);

            var preX = new List<double>();
            var preY = new List<double>();
            var postY = new List<double>();

            for (var i = 0; i < xPoints.Length; i++)
            {
                var featureX = xPoints[i];
                var featureY = yPoints[i];

                preX.Add(abscissaFunc(featureX));
                preY.Add(ordinateFuncPre(featureX, featureY));
                postY.Add(ordinateFuncPost(featureX, featureY));
            }

            plot.AddSeries(preX, preY, "Pre-Alignment ", preColor);
            plot.AddSeries(preX, postY, "Post-Alignment ", postColor);

            return plot;
        }

        public static PlotBase CreateResidualPlot(IEnumerable<float> x,
            IEnumerable<float> yPre,
            IEnumerable<float> yPost,
            string title,
            string xLabel,
            string yLabel)
        {
            var xd = x.Select(Convert.ToDouble);
            var yPreD = yPre.Select(Convert.ToDouble);
            var yPostD = yPost.Select(Convert.ToDouble);

            return CreateResidualPlot(xd, yPreD, yPostD, title, xLabel, yLabel);
        }

        public static PlotBase CreateResidualPlot(IEnumerable<double> x,
            IEnumerable<double> yPre,
            IEnumerable<double> yPost,
            string title,
            string xLabel,
            string yLabel)
        {
            var colorIterator = new ColorTypeIterator();
            var plot = new ScatterPlot(title, xLabel, yLabel);

            var preColor = colorIterator.GetColor(1);
            var postColor = colorIterator.GetColor(2);

            plot.AddSeries(x, yPre, "Pre-Alignment ", preColor);
            plot.AddSeries(x, yPost, "Post-Alignment ", postColor);

            return plot;
        }

        #endregion
    }
}