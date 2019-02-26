
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Distance
{
    /// <summary>
    /// Function for calculating the distance between two features.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public delegate double DistanceFunction<T>(T x, T y) where T : FeatureLight, new();
    /// <summary>
    /// Delegate to determine if two features are within range of one another.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public delegate bool WithinTolerances<T>(T x, T y) where T : FeatureLight, new();
}