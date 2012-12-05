using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;

namespace MultiAlignCore.Data.Imaging
{
    public class AlignmentImageData 
    {

        public Image FeaturePlotImage
        {
            get;
            set;
        }  
        public Image HeatmapImage
        {
            get;
            set;
        }
        public Image NetHistogramImage   
        {
            get;
            set;
        }
        public Image NetResidualsHistogramImage 
        {
            get;
            set;
        }
        public Image MassHistogramImage 
        {
            get;
            set;
        }
        public Image MassScanImage 
        {
            get;
            set;
        }
        public Image MassMzImage
        {
            get;
            set;
        }
        
        public Image DriftTimeScatterImage
        {
            get;
            set;
        }

        public Image DriftTimeHistogramImage
        {
            get;
            set;
        }

        public Image DriftTimeAlignedHistogramImage
        {
            get;
            set;
        }

        public Image DriftTimeErrorImage
        {
            get;
            set;
        }
        public Image DriftTimeAlignedErrorImage
        {
            get;
            set;
        }
    }
}
