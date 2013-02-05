using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.Solvers;

namespace MultiAlignCore.Algorithms.Alignment
{
    public enum BasisFunctionType
    {
        ChebyshevSecondKind
    }

    public class LevenburgMarquadtSolverFactory
    {
        public static alglib.ndimensional_pfunc CreateSolver(BasisFunctionType type)
        {
            alglib.ndimensional_pfunc myDelegate = null;

            switch (type)
            {
                case BasisFunctionType.ChebyshevSecondKind:
                    ChebyshevSolver solver  = new ChebyshevSolver();
                    myDelegate              = solver.SecondOrderSolve;
                    break;
                default:
                    break;
            }
            return myDelegate;
        }
    }
}
