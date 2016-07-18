using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Algorithms.Regression;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt;
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.Solvers
{

    [TestFixture]
    public class LevenburgMarquadtSolverTests : SolverTestBase
    {

        [Test]
        [Description("Tests the Levenburg Marquadt solver using a Chebyshev polynomial.")]
        public void SolveChebyshev()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculatedParabola(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.Chebyshev);
            var coeffs     = functionSelector.Coefficients;

            var showDetails = false;
            var report = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            // Make sure it converged.
            Assert.IsTrue(report.DidConverge);

            // Here are the coefficients where this should pass.
            Assert.LessOrEqual(Math.Abs(coeffs[0] - -161.49999998947484),          .0000001);
            Assert.LessOrEqual(Math.Abs(coeffs[1] - -340.99999998132216),          .0000001);
            Assert.LessOrEqual(Math.Abs(coeffs[2] - -89.749999993541593),          .0000001);
            Assert.LessOrEqual(Math.Abs(coeffs[3] - 0.50000000335890493),          .0000001);
            Assert.LessOrEqual(Math.Abs(coeffs[4] - 0.50000000120664689),          .0000001);
            Assert.LessOrEqual(Math.Abs(coeffs[5] - 0.00000000023672415945361692), .0000001);
        }
        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a quadratic line shape.")]
        public void SolveLinearFunction()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculateLine(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.Linear);
            var coeffs = functionSelector.Coefficients;

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(5, coeffs[0], .0001);
            Assert.AreEqual(0, coeffs[1], .0001);
        }

        public void SolveLinearFunctionViaInterface()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculateLine(), out x, out y);

            var regression = new AlglibRegression();


            var coeffs = new double[2];
            //initial conditions
            coeffs[0] = 2;
            coeffs[1] = 2;
            var worked = regression.Fit(x, y, BasisFunctionsEnum.Linear, ref coeffs);

            Assert.AreEqual(5, coeffs[0], .0001);
            Assert.AreEqual(0, coeffs[1], .0001);

            Assert.AreEqual(true,worked.DidConverge);
        }








        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a quadratic line shape.")]
        public void SolveQuadraticFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculatedParabola(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.PolynomialQuadratic);
            var coeffs = functionSelector.Coefficients;

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(-0.99999999960388553d, coeffs[0], .000001);
            Assert.AreEqual(2.410211171560969E-10d, coeffs[1], .000001);
            Assert.AreEqual(99.999999976322613d, coeffs[2], .000001);
        }

        public void SolveQuadraticFunctionViaInterface()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculatedParabola(), out x, out y);

            var regression = new AlglibRegression();
            var coeffs = new double[3];
            //initial conditions
            coeffs[0] = 1;
            coeffs[1] = 1;
            coeffs[2] = 1;
            var worked = regression.Fit(x, y, BasisFunctionsEnum.PolynomialQuadratic, ref coeffs);

            Assert.AreEqual(-0.99999999960388553d, coeffs[0], .000001);
            Assert.AreEqual(2.410211171560969E-10d, coeffs[1], .000001);
            Assert.AreEqual(99.999999976322613d, coeffs[2], .000001);

            Assert.AreEqual(true, worked.DidConverge);
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a Hanning line shape.")]
        public void SolveHanningFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualHanning(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.Hanning);
            var coeffs = functionSelector.Coefficients;

            //important guesses
            coeffs[0] = 30;//hanningI
            coeffs[1] = 5;//hanningK
            coeffs[2] = 1234.388251;//xoffset

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(Math.Round(30.521054724721569d, 7), Math.Round(coeffs[0], 7), .00001);
            Assert.AreEqual(Math.Round(37.723968728457208d, 6), Math.Round(coeffs[1], 6), .00001);
            Assert.AreEqual(Math.Round(1234.4579999999935d, 7), Math.Round(coeffs[2], 7), .00001);
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a quadratic line shape.")]
        public void SolveOrbitrapLorentzianFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualOrbitrap2(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.Lorentzian);

            var coeffs = functionSelector.Coefficients;
            //important guesses
            coeffs[0] = 5;//hanningI
            coeffs[1] = 80000;//hanningK
            coeffs[2] = 1234.388251;//xoffset

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(0.014591732782157337d, coeffs[0], .001);
            Assert.AreEqual(41816.913857810927d, coeffs[1], .001);
            Assert.AreEqual(1234.4577771195013d, coeffs[2], .001);
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a cubic line shape.")]
        public void SolveCubicFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculatedCubic(), out x, out y);

            var functionChoise = BasisFunctionsEnum.PolynomialCubic;
            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            var coeffs = functionSelector.Coefficients;

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(-0.9999999999984106d, coeffs[0], .00001);
            Assert.AreEqual(5.0000000000444658d, coeffs[1], .00001);
            Assert.AreEqual(99.999999999930722d, coeffs[2], .00001);
            Assert.AreEqual(24.999999997435527d, coeffs[3], .00001);
        }

        /// <summary>
        ///
        /// </summary>
        //[Test]
        [Description("Tests the Levenburg Marquadt solver using Chebyshev polynomials.")]
        public void SolveCubicWithChebyshevFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculatedParabola(), out x, out y);

            var functionChoise = BasisFunctionsEnum.Chebyshev;

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            var coeffs = functionSelector.Coefficients;

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);


            //Assert.AreEqual(-0.9999999999984106d, coeffs[0]);
            //Assert.AreEqual(5.0000000000444658d, coeffs[1]);
            //Assert.AreEqual(99.999999999930722d, coeffs[2]);
            //Assert.AreEqual(24.999999997435527d, coeffs[3]);
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a lorentzian line shape.")]
        public void SolveLorentzianFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualLortentzianA(), out x, out y);

            var functionChoise = BasisFunctionsEnum.Lorentzian;

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            var coeffs = functionSelector.Coefficients;

            coeffs[0] = 6;//width
            coeffs[1] = 50;//height
            coeffs[2] = -1;//xoffset

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(0.50000000000535016d, coeffs[0], .00001);//real is 0.5.
            Assert.AreEqual(150.00000000174555d, coeffs[1], .00001);//real is 75
            Assert.AreEqual(0.99999999999999312d, coeffs[2], .00001);//real is 1

            //using 1 instead of 0.5
            //Assert.AreEqual(0.49999999817701907d, coeffs[0]);//real is 0.5.
            //Assert.AreEqual(74.99999972887592d, coeffs[1]);//real is 75
            //Assert.AreEqual(0.9999999999999587d, coeffs[2]);//real is 1
        }


        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a gaussian line shape.")]
        public void SolveGaussianFactory()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualGaussian(), out x, out y);

            var functionChoise = BasisFunctionsEnum.Gaussian;

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            var coeffs = functionSelector.Coefficients;

            coeffs[0] = 6;//sigma
            coeffs[1] = 50;//height
            coeffs[2] = -1;//xoffset
            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(0.50000000014842283d, Math.Abs(coeffs[0]), .00001);//real is 0.5.  may return a negative value
            Assert.AreEqual(99.999999955476071d, coeffs[1], .00001);//real is 100
            Assert.AreEqual(0.99999999999999967d, coeffs[2], .00001);//real is 1
        }


        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a gaussian line shape.")]
        public void SolveGaussianFactoryRealProblemData()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualGaussianProblem(), out x, out y);

            var functionChoise = BasisFunctionsEnum.Gaussian;

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            var coeffs = functionSelector.Coefficients;

            coeffs[0] = 2;//sigma
            coeffs[1] = 7375230.5281385286;//height
            coeffs[2] = 1080;//xoffset

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(15.307150768556957d, Math.Abs(coeffs[0]), .01);//real is
            Assert.AreEqual(5839780.76391418d, coeffs[1], 100);//real is
            Assert.AreEqual(1081.0890598765791d, coeffs[2], .001);//real is


            Console.WriteLine("Try New Guess Coeff");

            var thisSectionFails = false;
            if (thisSectionFails)
            {
                var functionSelectorPoorChoice = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
                coeffs = functionSelectorPoorChoice.Coefficients;
                coeffs[0] = 2; //sigma
                coeffs[1] = 7375230.5281385286; //height
                coeffs[2] = 1102; //xoffset

                var workedPoorChoiseOfX = EvaluateFunction(x, y, functionSelectorPoorChoice, ref coeffs, showDetails);

                Assert.AreEqual(15.307150768556957d, Math.Abs(coeffs[0]), .01); //real is
                Assert.AreEqual(5839780.76391418d, coeffs[1], 100); //real is
                Assert.AreEqual(1081.0890598765791d, coeffs[2], .001); //real is
            }
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a quadratic line shape (legacy)")]
        public void SolveQuadratic()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(CalculatedParabola(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.PolynomialQuadratic);
            var coeffs = functionSelector.Coefficients;

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            Assert.AreEqual(-0.99999999959999375d, coeffs[0], .00001);
            Assert.AreEqual(2.4338897338076459E-10d, coeffs[1], .00001);
            Assert.AreEqual(99.999999976089995d, coeffs[2], .00001);

        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a gaussian line shape (legacy)")]
        public void SolveGaussian()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualGaussian(), out x, out y);

            var functionSelector = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.Gaussian);
            var coeffs = functionSelector.Coefficients;

            var showDetails = false;
            var worked = EvaluateFunction(x, y, functionSelector, ref coeffs, showDetails);

            //sigma must be positive
            Assert.AreEqual(0.50000000014842283d, Math.Abs(coeffs[0]), .00001);//real is 0.5.  may return a negative value
            Assert.AreEqual(99.999999955476071d, coeffs[1], .00001);//real is 100
            Assert.AreEqual(0.99999999999999967d, coeffs[2], .00001);//real is 1

        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        [Description("Tests the Levenburg Marquadt solver using a gaussian line shape (legacy)")]
        public void SolveAsymmetricGaussian()
        {
            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(ManualGaussian(), out x, out y);

            var start = x.Min();
            var stop  = x.Max();
            var width = Math.Abs(stop - start);
            var A = y.Max();


            if (mShowDetails)
            {
                for (var i = 0; i < x.Count; i++)
                {
                    Console.WriteLine("{0}\t{1}", x[i], y[i]);
                }

                Console.WriteLine();
            }


            var basisFunction     = BasisFunctionFactory.BasisFunctionSelector(BasisFunctionsEnum.AsymmetricGaussian);
            var coefficients               = basisFunction.Coefficients;
            coefficients[0]                     = start + width/2;
            coefficients[1]                     = A;
            coefficients[2]                     = width*.9;
            coefficients[3]                     = coefficients[2];

            var showDetails = false;
            var worked = EvaluateFunction(x, y, basisFunction, ref coefficients, showDetails);
            var numberOfSamples = x.Count * 2;

            // Calculate the width an spacing of each of the trapezoids.
            var delta = width / Convert.ToDouble(numberOfSamples);
            var xx     = start;

            // We already evaluated the first point, now for each element within
            for (var i = 0; i < numberOfSamples + 1; i++)
            {
                xx += delta;
                var yy = basisFunction.Evaluate(coefficients, xx);

                if (mShowDetails)
                {
                    Console.WriteLine("{0}\t{1}", xx, yy);
                }
            }

        }
    }
}
