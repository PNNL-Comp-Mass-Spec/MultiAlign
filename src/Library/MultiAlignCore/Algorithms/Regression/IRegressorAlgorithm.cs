#region Namespaces

using System.Collections.Generic;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Algorithms.Regression
{
    public interface IRegressorAlgorithm<T>
    {
        [UsedImplicitly]
        T CalculateRegression(IEnumerable<double> observed, IEnumerable<double> predicted);
        [UsedImplicitly]
        double Transform(T regressionFunction, double observed);        
    }
}
