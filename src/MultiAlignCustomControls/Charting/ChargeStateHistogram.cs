using System;
using System.Collections.Generic;
using System.Drawing;
using PNNLControls;
using PNNLControls.Drawing.Charting;

namespace MultiAlignCustomControls.Charting
{
    public partial class ChargeStateHistogram: ctlHistogram        
    {

        public ChargeStateHistogram()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a charge state histogram from the data provided.
        /// </summary>
        /// <param name="features"></param>
        public void ConstructHistogram(Dictionary<int, int> features)
        {
            if (features.Count < 1)
                return;

            int maxCharge               = int.MinValue;            
            Color color                 = Color.Red;
            clsShape shape              = new PNNLControls.DiamondShape(1, false); ;
            clsPlotParams plotParams    = new PNNLControls.clsPlotParams(shape, color);

            foreach (int charge in features.Keys)
            {
                maxCharge = Math.Max(charge, maxCharge);
            }

            float[] bins = new float[maxCharge];
            float[] freq = new float[maxCharge];
            
            foreach (int charge in features.Keys)
            {
                bins[charge - 1] = charge;
                freq[charge - 1] = features[charge];
            }

            this.ViewPortHistory.Clear();
            this.AddSeries(new clsSeries(ref bins, ref freq, plotParams));
        }
    }
}
