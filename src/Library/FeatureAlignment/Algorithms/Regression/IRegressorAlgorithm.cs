#region Namespaces

using System.Collections.Generic;

#endregion

namespace FeatureAlignment.Algorithms.Regression
{
    public interface IRegressorAlgorithm<T>
    {
        // ReSharper disable once UnusedMember.Global (Public API)
        T CalculateRegression(IEnumerable<double> observed, IEnumerable<double> predicted);
        // ReSharper disable once UnusedMember.Global (Public API)
        double Transform(T regressionFunction, double observed);
    }
}
