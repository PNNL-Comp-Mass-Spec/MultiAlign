﻿#region

using MultiAlignCore.Algorithms.Clustering;

#endregion

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    public class FeatureFinderFactory
    {
        /// <summary>
        /// Creates a new feature finder based on a known type of algorithm.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IFeatureFinder CreateFeatureFinder(FeatureFinderType type)
        {
            IFeatureFinder finder;
            switch (type)
            {
                default:
                    finder = new UmcTreeFeatureFinder();
                    break;
            }

            return finder;
        }
    }
}