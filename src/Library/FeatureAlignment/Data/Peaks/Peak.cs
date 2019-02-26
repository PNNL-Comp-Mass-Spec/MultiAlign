using System.Collections.Generic;

namespace FeatureAlignment.Data.Peaks
{
    //TODO: Change Peak to inherit from XYData

    /// <summary>
    /// Represents a peak.
    /// </summary>
    public class Peak
    {
        private const float DEFAULT_HEIGHT = 0;
        private const float DEFAULT_WIDTH  = 0;
        private const float DEFAULT_XVALUE = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Peak()
        {
            Height = DEFAULT_HEIGHT;
            Width = DEFAULT_WIDTH;
            XValue = DEFAULT_XVALUE;
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="height">Height of Peak</param>
        /// <param name="xValue">Xvalue of Peak</param>
        /// <param name="width">Width of peak (often full width at half maximum)</param>
        /// <param name="leftWidth">Distance to the left from the centroid </param>
        /// <param name="rightWidth">Distance to the right from the centroid</param>
        /// <param name="localSignalToNoise">Signal to noise at the local level</param>
        /// <param name="background">background contribution to the height</param>
        public Peak(double height, double xValue, float width, double leftWidth, double rightWidth, float localSignalToNoise, float background)
        {
            Height = height;
            XValue = xValue;
            Width = width;
            LeftWidth = leftWidth;
            RightWidth = rightWidth;
            LocalSignalToNoise = localSignalToNoise;
            Background = background;
        }


        public int Id { get; set; }
        /// <summary>
        /// Gets or the height of the peak.
        /// </summary>
        public double Height
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the X-Value of the peak (e.g. time, scan, m/z)
        /// </summary>
        public double XValue
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width of a peak.
        /// </summary>
        public float Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the data that define the peak or profile.
        /// </summary>
        public List<XYData> Points
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width to the left of the centroid (XValue) at half max.
        /// </summary>
        public double LeftWidth
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width to the right of the centroid (XValue) at half max.
        /// </summary>
        public double RightWidth
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets local signal to noise ratio.
        /// </summary>
        public float LocalSignalToNoise
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the background at a peak's x-value.
        /// </summary>
        public float Background
        {
            get;
            set;
        }
    }
}
