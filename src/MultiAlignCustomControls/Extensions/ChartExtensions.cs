using System;
using System.Drawing;
using MultiAlignCustomControls.Charting;
using PNNLControls;
using PNNLOmics.Algorithms;
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

        /// <summary>
        /// Adjusts a charts viewport based on the data and extends the view based on mass/net tolerances.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="tolerances"></param>
        /// <param name="useDriftTolerance"></param>
        public static void AdjustViewPortWithTolerances(this ctlLineChart chart, double tolerance)
        {            
            // And make sure we re-adjust the viewport for the NET plot
            RectangleF viewport     = chart.ViewPort;
            float net               = Convert.ToSingle(tolerance);            
            float xview             = Math.Max(Math.Abs(viewport.X), Math.Abs(viewport.Right)) + net;
            RectangleF newViewport  = new RectangleF(-xview, viewport.Y, xview  * 2, viewport.Height);
            chart.ViewPortHistory.Clear();
            chart.ViewPort = newViewport;
        }
        
        /// <summary>
        /// Adjusts a charts viewport based on the data and extends the view based on mass/net tolerances.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="tolerances"></param>
        /// <param name="useDriftTolerance"></param>
        public static void AdjustViewPortWithTolerances(this ClusterErrorScatterPlot chart, FeatureTolerances tolerances, bool useDriftTolerance)
        {
            float massPpm       = Convert.ToSingle(tolerances.Mass);
            float net           = Convert.ToSingle(tolerances.RetentionTime);
            if (useDriftTolerance)
            {
                net = Convert.ToSingle(tolerances.DriftTime);
            }

            // And make sure we re-adjust the viewport for the NET plot
            RectangleF viewport  = chart.ViewPort;
            float maxNetError    = Math.Max(Math.Abs(viewport.X),   Math.Abs(viewport.Right))  + net;
            float maxMassError   = Math.Max(Math.Abs(viewport.Top), Math.Abs(viewport.Bottom)) + massPpm;
            
            RectangleF newViewport = new RectangleF(-maxNetError, -maxMassError, maxNetError * 2, maxMassError * 2 ); 
            chart.ViewPortHistory.Clear();
            chart.ViewPort = newViewport;
        }
    }
}
