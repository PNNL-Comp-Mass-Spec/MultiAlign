
using MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;

namespace MultiAlignCore.Algorithms.Solvers.LevenburgMarquadt
{
    public class BasisFunctionFactory
    {        
        public static BasisFunctionBase BasisFunctionSelector(BasisFunctionsEnum functionChoise)
        {
            //default
            BasisFunctionBase solver = null;
            
            switch (functionChoise)
            {
                case BasisFunctionsEnum.AsymmetricGaussian:
                    solver = new AsymmetricGaussian();
                    solver.Coefficients = new double[4];
                    solver.Coefficients[0] = .5;
                    solver.Coefficients[1] = .5;
                    solver.Coefficients[2] = .3;
                    solver.Coefficients[3] = 1;
                    break;
                case BasisFunctionsEnum.Linear:
                    solver = new Linear();
                    solver.Coefficients = new double[2];
                    solver.Coefficients[0] = 1; // m
                    solver.Coefficients[1] = 0; // b
                    break;
                case BasisFunctionsEnum.PolynomialQuadratic:
                    {
                        solver = new Quadratic();
                        solver.Coefficients = new double[3];
                        solver.Coefficients[0] = 1;//ax^2
                        solver.Coefficients[1] = 1;//bx
                        solver.Coefficients[2] = 1;//c
                    }
                    break;
                case BasisFunctionsEnum.PolynomialCubic:
                    {
                        solver = new Cubic();
                        solver.Coefficients = new double[7];
                        solver.Coefficients[0] = 1;//ax^3
                        solver.Coefficients[1] = 1;//bx^2
                        solver.Coefficients[2] = 1;//cx
                        solver.Coefficients[3] = 1;//d
                        solver.Coefficients[4] = 1;//d
                        solver.Coefficients[5] = 1;//d
                        solver.Coefficients[6] = 1;//d
                    }
                    break;
                case BasisFunctionsEnum.Lorentzian:
                    {
                        solver = new Lorentian();
                        solver.Coefficients = new double[3];
                        solver.Coefficients[0] = 6;//width
                        solver.Coefficients[1] = 50;//height
                        solver.Coefficients[2] = -1;//xoffset
                    }
                    break;
                case BasisFunctionsEnum.Gaussian:
                    {
                        solver = new Gaussian();
                        solver.Coefficients = new double[3];
                        solver.Coefficients[0] = 6;//sigma
                        solver.Coefficients[1] = 50;//height
                        solver.Coefficients[2] = -1;//xoffset
                    }
                    break;
                case BasisFunctionsEnum.Chebyshev:
                    {
                        solver = new Chebyshev();
                        solver.Coefficients = new double[6];
                        solver.Coefficients[0] = 0;//?
                        solver.Coefficients[1] = 1;//?
                        solver.Coefficients[2] = 1;//?
                        solver.Coefficients[3] = 0;//?
                        solver.Coefficients[4] = 0;//?
                        solver.Coefficients[5] = 0;//?
                    }
                    break;
                case BasisFunctionsEnum.Orbitrap:
                    {
                        solver = new OrbitrapFunction();
                        solver.Coefficients = new double[3];
                        solver.Coefficients[0] = 1;//?
                        solver.Coefficients[1] = 1;//?
                        solver.Coefficients[2] = 1;//?
                        //quadSolver.Coefficients[3] = 1;//?
                        //quadSolver.Coefficients[4] = 1;//?
                        //quadSolver.Coefficients[4] = 1;//?
                    }
                    break;
                case BasisFunctionsEnum.Hanning:
                    {
                        solver = new Hanning();
                        solver.Coefficients = new double[3];
                        solver.Coefficients[0] = 1;//?
                        solver.Coefficients[1] = 1;//?
                        solver.Coefficients[2] = 1;//?
                    }
                    break;
            }
            return solver;
        }
    }

    
}
