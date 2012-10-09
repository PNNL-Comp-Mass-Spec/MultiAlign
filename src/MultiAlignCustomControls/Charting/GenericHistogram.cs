using System;
using System.Collections.Generic;
using System.Drawing;
using PNNLControls;
using PNNLControls.Drawing.Charting;

namespace MultiAlignCustomControls.Charting
{
    public partial class GenericHistogram: ctlHistogram        
    {
        private Dictionary<int, int> m_histogram;

        public GenericHistogram()
        {
            InitializeComponent();
            IsClipboardCopyEnabled  = true;
            m_histogram             = new Dictionary<int, int>();
        }

        public override void CopyDataToClipboard()
        {
            ApplicationClipboard.ClearApplicationClipboard();
            string data = "Bin\tCount\n";
            foreach (int key in m_histogram.Keys)
            {
                data += string.Format("{0}\t{1}\n", key, m_histogram[key]);
            }
            ApplicationClipboard.SetData(data);
        }

        /// <summary>
        /// Creates a charge state histogram from the data provided.
        /// </summary>
        /// <param name="features"></param>
        public void ConstructHistogram(Dictionary<int, int> features)
        {
            if (features.Count < 1)
                return;

            m_histogram = features;

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
