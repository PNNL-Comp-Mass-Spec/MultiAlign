using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;
using PNNLControls;
using System.Drawing;

namespace MultiAlignCustomControls.Charting
{
    public class MsFeatureChart : SpectraChart
    {
        private MSFeatureLight m_feature;

        public MsFeatureChart()
	    {
            NumberOfIsotopes  = 3;
            IsotopeRatio      = .66;
            PenSize           = 3;
            MinIntensity      = 100;
            IsotopeAlphaValue = 128;
            AddPostProcessor(new ChartPostRenderingProcessor(RenderSpectra), PostProcessPriority.MidHigh);
        }

        public void SetFeature(MSFeatureLight feature, List<XYData> spectra)
        {
            this.ViewPortHistory.Clear();

            m_feature = feature;
            if (spectra != null)
            {
                base.SetSpectra(spectra);
            }
            base.AutoViewPort();
        }

        private void RenderSpectra(ctlChartBase chart, PostRenderEventArgs args)
        {
            if (m_feature == null)
                return;

            RenderSpectra(m_feature, chart, args);
        }
        private void RenderSpectra(MSFeatureLight feature, ctlChartBase chart, PostRenderEventArgs args)
        {
            Graphics graphics = args.Graphics;
            double mz         = feature.Mz;
            double abundance  = feature.Abundance;


            float bottom = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(0));   
            
            Color color = Color.FromArgb(IsotopeAlphaValue, Color.Red);
            using (Brush brush = new SolidBrush(color))
            {
                using (Pen pen = new Pen(brush, PenSize))
                {
                    while (abundance > MinIntensity)
                    {                
                        float mzScreen        = mobj_axis_plotter.XScreenPixel(Convert.ToSingle(mz));
                        float abundanceScreen = this.mobj_axis_plotter.YScreenPixel(Convert.ToSingle(abundance));

                        graphics.DrawLine(pen, mzScreen, bottom, mzScreen, abundanceScreen);

                        abundance *= IsotopeRatio;
                        mz += (1.0 / feature.ChargeState);                
                    }
                }
            }
        }

        public double MinIntensity { get; set; }
        public double IsotopeRatio  { get; set; }
        public int NumberOfIsotopes { get; set; }

        public float PenSize
        {
            get;
            set;
        }
        public int IsotopeAlphaValue
        {
            get;
            set;
        }
    }
}
