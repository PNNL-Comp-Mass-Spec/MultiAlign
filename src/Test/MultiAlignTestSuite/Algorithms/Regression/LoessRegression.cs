using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Regression;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.Regression
{
    [TestFixture]
    public class LoessRegression
    {
        /// <summary>
        /// This function tests the Loess interpolation code
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generatingFunction"></param>
        /// <param name="dt"></param>
        /// <param name="attenuation"></param>
        [Test]
        [TestCase(FitFunctionTypes.TriCubic,
            FitFunctionTypes.Sin,
            .4,
            .5)]
        public void TestLoess(FitFunctionTypes type,
            FitFunctionTypes generatingFunction,
            double dt,
            double attenuation)
        {
            var interpolator = new LoessInterpolator(.25, 0);

            var xValues = new List<double>();
            var yValues = new List<double>();


            double cv = 0;

            var fitFunction = FitFunctionFactory.Create(type);
            var genFunction = FitFunctionFactory.Create(generatingFunction);

            var random = new Random();

            // Take one period of the sine wave...
            while (cv < Math.PI * 3)
            {
                var value = genFunction(cv) + random.NextDouble() * attenuation;

                xValues.Add(cv);
                yValues.Add(value);
                cv += dt;
            }

            var newYValues = interpolator.Smooth(xValues, yValues, fitFunction);
            for (var i = 0; i < xValues.Count; i++)
            {
                Console.WriteLine(@"{0} {1} {2} {3}", i, xValues[i], yValues[i], newYValues[i]);
            }

            dt /= 2;
            cv = 0;

            Console.WriteLine();

            // Take one period of the sine wave...
            while (cv < Math.PI * 3)
            {
                var predicted = interpolator.Predict(cv);
                Console.WriteLine(@"{0} {1}", cv, predicted);
                cv += dt;
            }
        }
    }
}
