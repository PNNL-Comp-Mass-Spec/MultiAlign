using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms;
using PNNLControls;
using System.Drawing;
using PNNLOmics.Data.Features;

namespace MultiAlignCustomControls.Extensions
{
    public static class ChartExtensions
    {
        /// <summary>
        /// Adjusts a charts viewport based on the data and extends the view based on mass/net tolerances.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="tolerances"></param>
        /// <param name="useDriftTolerance"></param>
        public static void AdjustViewPortWithTolerances(this ctlChartBase chart, FeatureTolerances tolerances, bool useDriftTolerance)
        {
            double massPpm      = tolerances.Mass;
            double net          = tolerances.RetentionTime;
            if (useDriftTolerance)
                net = tolerances.DriftTime;

            // And make sure we re-adjust the viewport for the NET plot
            RectangleF viewport = chart.ViewPort;
            float netView       = Convert.ToSingle(net);
            float minMassView   = Convert.ToSingle(Feature.ComputeDaDifferenceFromPPM(viewport.Top, massPpm));

            float maxMassView   = viewport.Bottom + (viewport.Top - minMassView);
            float minNetView    = viewport.X - netView;
            float maxNetView    = viewport.X + netView + viewport.Width;

            RectangleF newViewport = new RectangleF(minNetView, minMassView, maxNetView - minNetView, maxMassView - minMassView);
            chart.ViewPortHistory.Clear();
            chart.ViewPort = newViewport;
        }
    }
}
