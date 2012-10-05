using System;
using System.Collections.Generic;
using System.Drawing;
using PNNLControls;
using PNNLControls.Drawing.Charting;

namespace MultiAlignCustomControls.Charting
{
    public partial class GenericHistogram: ctlHistogram        
    {

        public GenericHistogram()
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

            SeriesCollection.Clear();

            int maxCharge               = int.MinValue;       
            int minCharge               = int.MaxValue;
            Color color                 = Color.Red;
            clsShape shape              = new PNNLControls.DiamondShape(1, false); ;
            clsPlotParams plotParams    = new PNNLControls.clsPlotParams(shape, color);

            foreach (int charge in features.Keys)
            {
                maxCharge = Math.Max(charge, maxCharge);
                minCharge = Math.Min(charge, minCharge);
            }
            
            float[] bins = new float[maxCharge - minCharge + 1];
            float[] freq = new float[maxCharge - minCharge + 1];
            
            foreach (int charge in features.Keys)
            {
                
                
                bins[charge - minCharge] = charge;
                freq[charge - minCharge] = features[charge];
            }
            
            this.AddSeries(new clsSeries(ref bins, ref freq, plotParams));
        }
    }
}
