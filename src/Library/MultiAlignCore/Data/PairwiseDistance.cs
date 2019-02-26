using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds the distance between two features and indices.
    /// </summary>
    public sealed class PairwiseDistance<T> : IComparer<PairwiseDistance<T>>
        where T : FeatureLight, new()
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PairwiseDistance()
        {
            Distance = double.NaN;
        }
        /// <summary>
        /// Constructor that builds a distance between two features.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="distance"></param>
        public PairwiseDistance(T x, T y, double distance)
        {
            FeatureX = x;
            FeatureY = y;
            Distance = distance;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the id of a pairwise distance.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the x feature.
        /// </summary>
        public T FeatureX { get; set; }
        /// <summary>
        /// Gets or sets the y feature.
        /// </summary>
        public T FeatureY { get; set; }
        /// <summary>
        /// Gets or sets the distance between the two features.
        /// </summary>
        public double Distance { get; set; }
        #endregion

        #region IComparer<PairwiseDistance> Members
        /// <summary>
        /// Compares the distance between x and y.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Returns an integer value determining if x is greater than, less than, or equal to y.</returns>
        public int Compare(PairwiseDistance<T> x, PairwiseDistance<T> y)
        {
            return x.Distance.CompareTo(y.Distance);
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0}  ({1}, {2}) - ({3}, {4})  = {5}", Id, FeatureX.Id, FeatureX.GroupId, FeatureY.Id, FeatureY.GroupId, Distance);
        }
    }
}
