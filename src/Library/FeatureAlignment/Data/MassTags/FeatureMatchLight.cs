
using FeatureAlignment.Data.Features;

namespace FeatureAlignment.Data.MassTags
{
    /// <summary>
    /// Holds matches between a feature and a mass tag.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FeatureMatchLight<T, U>
        where T : FeatureLight
        where U : FeatureLight
    {
        public T Observed
        {
            get;
            set;
        }
        public U Target
        {
            get;
            set;
        }
        #region Scores
        /// <summary>
        /// Gets or sets the confidence of this match. STAC Score.
        /// </summary>
        public double Confidence
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the uniqueness of this match.  STAC Up
        /// </summary>
        public double Uniqueness
        {
            get;
            set;
        }
        #endregion
    }
}
