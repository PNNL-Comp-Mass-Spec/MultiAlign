using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Distance
{
    public class MahalanobisWrapper<T> where T : FeatureLight, new()
    {
        private DenseMatrix CreateMatrix(List<T> x)
        {
            //double [,] y = new double[x.cou

            return null;
        }

        public double Mahalanobis(List<T> x, T y)
        {

            
           // DenseMatrix featureY = new DenseMatrix(


           // return MahalanobisDistanceCalculator.CalculateMahalanobisDistance(featureX, featureY);            

           return 0;
        }
    }
}
