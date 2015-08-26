using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Algorithms.Regression
{
    public class LoessInterpolator
    {
        public static double DEFAULT_BANDWIDTH = 0.50;
        public static int DEFAULT_ROBUSTNESS_ITERS = 2;

        /**
         * The bandwidth parameter: when computing the loess fit at
         * a particular point, this fraction of source points closest
         * to the current point is taken into account for computing
         * a least-squares regression.
         * 
         * A sensible value is usually 0.25 to 0.5.
         */
        private double bandwidth;

        /**
         * The number of robustness iterations parameter: this many
         * robustness iterations are done.
         * 
         * A sensible value is usually 0 (just the initial fit without any
         * robustness iterations) to 4.
         */
        private int robustnessIters;
        private  IList<double> m_xModel;
        private  IList<double> m_yModel;
        private double[] m_res;
        private double[] m_residuals;
        private double[] m_sortedResiduals;
        private double[] m_robustnessWeights;
        private int[] m_bandwidthInterval;
        private int m_modelSize;
        private int m_bandwidthInPoints;
        private Func<double, double> m_fitFunction;

        public LoessInterpolator()
            : this(DEFAULT_BANDWIDTH, DEFAULT_ROBUSTNESS_ITERS)
        {
        }

        public LoessInterpolator(double bandwidth, int robustnessIters)
        {
            if (bandwidth < 0 || bandwidth > 1)
            {
                throw new ApplicationException(string.Format("bandwidth must be in the interval [0,1], but got {0}", bandwidth));
            }
            this.bandwidth = bandwidth;
            if (robustnessIters < 0)
            {
                throw new ApplicationException(string.Format("the number of robustness iterations must be non-negative, but got {0}", robustnessIters));
            }
            this.robustnessIters = robustnessIters;

            MaxDistance = .15;
        }

        private void Predict(double x,  out double res, out double residuals, double[] robustnessWeights, int[] bandwidthInterval, int i)
        {
            

            var ileft  = bandwidthInterval[0];
            var iright = bandwidthInterval[1];

            // Compute the point of the bandwidth interval that is
            // farthest from x
            int edge;
            if (m_xModel[i] - m_xModel[ileft] > m_xModel[iright] - m_xModel[i])
            {
                edge = ileft;
            }
            else
            {
                edge = iright;
            }

            // Compute a least-squares linear fit weighted by
            // the product of robustness weights and the tricube
            // weight function.
            // See http://en.wikipedia.org/wiki/Linear_regression
            // (section "Univariate linear case")
            // and http://en.wikipedia.org/wiki/Weighted_least_squares
            // (section "Weighted least squares")
            double sumWeights   = 0;
            double sumX         = 0, sumXSquared = 0, sumY = 0, sumXy = 0;
            var denom        = Math.Abs(1.0 / (m_xModel[edge] - x));
            for (var k = ileft; k <= iright; ++k)
            {
                var xk = m_xModel[k];
                var yk = m_yModel[k];

                double dist;
                if (k < i)
                {
                    dist = (x - xk);
                }
                else
                {
                    dist = (xk - x);
                }

                var w     = m_fitFunction(dist * denom) * robustnessWeights[k];
                var xkw   = xk * w;

                sumWeights  += w;
                sumX        += xkw;
                sumXSquared += xk * xkw;
                sumY        += yk * w;
                sumXy       += yk * xkw;
            }

            var meanX        = sumX / sumWeights;
            var meanY        = sumY / sumWeights;
            var meanXy       = sumXy / sumWeights;
            var meanXSquared = sumXSquared / sumWeights;

            double beta;
            if (meanXSquared == meanX * meanX)
            {
                beta = 0;
            }
            else
            {
                beta = (meanXy - meanX * meanY) / (meanXSquared - meanX * meanX);
            }

            var alpha = meanY - beta * meanX;
            res          = beta * x + alpha;
            residuals    = Math.Abs(m_yModel[i] - res);            
        }

        /// <summary>
        /// Predicts a new value based on xValue
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns></returns>
        public double Predict(double xValue)
        {
            if (m_xModel == null || m_yModel == null)
                throw new Exception("The models have not been set yet.  Please set them");
            if (m_xModel.Count != m_yModel.Count)
                throw new Exception("The models do not match in size. They must be the same dimension");

            // Find index within the model...for the locally weighted regression
            var index = FindIndex(xValue, m_xModel);
            double res;
            double residuals;            
            
            
            m_bandwidthInterval = new[] { 0, m_bandwidthInPoints - 1 };

            // From the left 
            var points = m_bandwidthInPoints / 2;

            // We are at the left edge.
            if (index - points  < 0)
            {
                m_bandwidthInterval[0] = 0;
                m_bandwidthInterval[1] = m_bandwidthInPoints - 1;
            }
            else if (index + points > m_xModel.Count)
            {

                m_bandwidthInterval[1] = m_xModel.Count - 1;
                m_bandwidthInterval[0] = m_bandwidthInterval[1] - m_bandwidthInPoints;
            }
            else
            {
                m_bandwidthInterval[0] = index - points;
                m_bandwidthInterval[1] = index + points - 1;

            }            
            // At each x, compute a local weighted linear regression                
            Predict(xValue, out res, out residuals, m_robustnessWeights, m_bandwidthInterval, index);                                
            return res;
        }

        /// <summary>
        /// Compute a loess fit on the data at the original abscissae.
        /// </summary>
        /// <param name="xval">xval the arguments for the interpolation points</param>
        /// <param name="yval">the values for the interpolation points</param>
        /// <returns>values of the loess fit at corresponding original abscissae</returns>
        /// <exception>MathException if some of the following conditions are false:
        ///     <cref>MathException</cref>
        /// </exception>
        /// <remarks>
        ///  Arguments and values are of the same size that is greater than zero
        ///  The arguments are in a strictly increasing order
        ///  All arguments and values are finite real numbers
        /// </remarks>
        public IList<double> Smooth(IList<double> xval, IList<double> yval, Func<double, double> fitFunction)
        {
            m_xModel = xval;
            m_yModel = yval;

            m_fitFunction = fitFunction;

            if (m_xModel.Count() != m_yModel.Count())
            {
                throw new ApplicationException(string.Format("Loess expects the abscissa and ordinate arrays to be of the same size, but got {0} abscisssae and {1} ordinatae",
                        m_xModel.Count,
                        m_yModel.Count));
            }
            m_modelSize = m_xModel.Count();
            if (m_modelSize == 0)
            {
                throw new ApplicationException("Loess expects at least 1 point");
            }

            CheckAllFiniteReal(m_xModel, true);
            CheckAllFiniteReal(m_yModel, false);
            CheckStrictlyIncreasing(m_xModel);

            if (m_modelSize == 1)
                return new List<double> { m_yModel[0] };
            
            if (m_modelSize == 2)
                return new List<double> { m_yModel[0], m_yModel[1] };
            

            m_bandwidthInPoints = (int)(bandwidth * m_modelSize);

            if (m_bandwidthInPoints < 2)
            {
                throw new ApplicationException(string.Format("the bandwidth must be large enough to accomodate at least 2 points. There are {0} " +
                    " data points, and bandwidth must be at least {1} but it is only {2}",
                    m_modelSize, 2.0 / m_modelSize, bandwidth
                ));
            }

            m_res               = new double[m_modelSize];
            m_residuals         = new double[m_modelSize];
            m_sortedResiduals   = new double[m_modelSize];
            m_robustnessWeights = new double[m_modelSize];

            // Do an initial fit and 'robustnessIters' robustness iterations.
            // This is equivalent to doing 'robustnessIters+1' robustness iterations
            // starting with all robustness weights set to 1.
            for (var i = 0; i < m_robustnessWeights.Length; i++) 
                    m_robustnessWeights[i] = 1;

            for (var iter = 0; iter <= robustnessIters; ++iter)
            {
                m_bandwidthInterval = new[] { 0, m_bandwidthInPoints - 1 };

                // At each x, compute a local weighted linear regression
                for (var i = 0; i < m_modelSize; ++i)
                {
                    var res       = m_res[i];
                    var residuals = m_res[i];

                    // Find out the interval of source points on which
                    // a regression is to be made.
                    if (i > 0)
                    {
                        updateBandwidthInterval(m_xModel, i, m_bandwidthInterval);
                    }

                    Predict(m_xModel[i], out res, out residuals, m_robustnessWeights, m_bandwidthInterval, i);

                    m_res[i]        = res;
                    m_residuals[i]  = res;
                }

                // No need to recompute the robustness weights at the last
                // iteration, they won't be needed anymore
                if (iter == robustnessIters)
                {
                    break;
                }

                // Recompute the robustness weights.
                // Find the median residual.
                // An arraycopy and a sort are completely tractable here, 
                // because the preceding loop is a lot more expensive
                System.Array.Copy(m_residuals, m_sortedResiduals, m_modelSize);
                Array.Sort(m_sortedResiduals);
                var medianResidual = m_sortedResiduals[m_modelSize / 2];

                if (medianResidual == 0)
                {
                    break;
                }

                for (var i = 0; i < m_modelSize; ++i)
                {
                    var arg = m_residuals[i] / (6 * medianResidual);
                    m_robustnessWeights[i] = (arg >= 1) ? 0 : Math.Pow(1 - arg * arg, 2);
                }
            }

            return m_res;
        }        

        /// <summary>
        /// Determines which part of the model the query value exists in.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private int FindIndex(double x, IList<double> model)
        {
            var i           = 0;
            var tempDiff = double.MaxValue;
            var j           = 0;

            // throw new NotImplementedException();
            for(i = 0; i < model.Count - 1; i++)
            {
                var diff = Math.Abs(x - model[i]);

                if (tempDiff > diff)
                {
                    tempDiff = diff;
                    j        = i;
                }                
            }
            return j;
        }

        /**
         * Given an index interval into xval that embraces a certain number of
         * points closest to xval[i-1], update the interval so that it embraces
         * the same number of points closest to xval[i]
         *
         * @param xval arguments array
         * @param i the index around which the new interval should be computed
         * @param bandwidthInterval a two-element array {left, right} such that: <p/>
         * <tt>(left==0 or xval[i] - xval[left-1] > xval[right] - xval[i])</tt>
         * <p/> and also <p/>
         * <tt>(right==xval.length-1 or xval[right+1] - xval[i] > xval[i] - xval[left])</tt>.
         * The array will be updated.
         */
        private void updateBandwidthInterval(IList<double> xval, int i, int[] bandwidthInterval)
        {
            var left  = bandwidthInterval[0];
            var right = bandwidthInterval[1];
            // The right edge should be adjusted if the next point to the right
            // is closer to xval[i] than the leftmost point of the current interval
            if (right < xval.Count - 1 && xval[right + 1] - xval[i] < xval[i] - xval[left])
            {                
                   bandwidthInterval[0]++;
                   bandwidthInterval[1]++;
            }
        }

        //private void updateBandwidthInterval(IList<double> xval, int i, int[] bandwidthInterval)
        //{
        //    int left  = bandwidthInterval[0];
        //    int right = bandwidthInterval[1];
        //    // The right edge should be adjusted if the next point to the right
        //    // is closer to xval[i] than the leftmost point of the current interval
        //    //if (right < xval.Count - 1 && xval[right + 1] - xval[i] < xval[i] - xval[left])
        //    //{
        //    //    bandwidthInterval[0]++;
        //    //    bandwidthInterval[1]++;
        //    //}

        //    var value = xval[i];
        //    while (right < xval.Count - 1 && xval[right] - value < MaxDistance)
        //    {
        //        right++;
        //    }

        //    while (left >= 0 && (value - xval[left]) > MaxDistance && left < right)
        //    {
        //        left++;
        //    }

        //    bandwidthInterval[0] = left;
        //    bandwidthInterval[1] = right;
        //}

        public double MaxDistance { get; set; }

        /**
         * Compute the 
         * <a href="http://en.wikipedia.org/wiki/Local_regression#Weight_function">tricube</a>
         * weight function
         *
         * @param x the argument
         * @return (1-|x|^3)^3
         */
        private double tricube(double x)
        {
            var tmp = 1 - x * x * x;
            return tmp * tmp * tmp;
        }

        /**
         * Check that all elements of an array are finite real numbers.
         *
         * @param values the values array
         * @param isAbscissae if true, elements are abscissae otherwise they are ordinatae
         * @throws MathException if one of the values is not
         *         a finite real number
         */
        private void CheckAllFiniteReal(IList<double> values, bool isAbscissae)
        {
            for (var i = 0; i < values.Count; i++)
            {
                var x = values[i];
                if (Double.IsInfinity(x) || Double.IsNaN(x))
                {
                    var pattern = isAbscissae ?
                            "all abscissae must be finite real numbers, but {0}-th is {1}" :
                            "all ordinatae must be finite real numbers, but {0}-th is {1}";
                    throw new ApplicationException(string.Format(pattern, i, x));
                }
            }
        }

        /**
         * Check that elements of the abscissae array are in a strictly
         * increasing order.
         *
         * @param xval the abscissae array
         * @throws MathException if the abscissae array
         * is not in a strictly increasing order
         */
        private void CheckStrictlyIncreasing(IList<double> xval)
        {
            for (var i = 0; i < xval.Count; ++i)
            {
                //if (i >= 1 && xval[i - 1] >= xval[i])
                if (i >= 1 && xval[i - 1] > xval[i])
                {
                    throw new ApplicationException(string.Format(
                            "the abscissae array must be sorted in a strictly " +
                            "increasing order, but the {0}-th element is {1} " +
                            "whereas {2}-th is {3}",
                            i - 1, xval[i - 1], i, xval[i]));
                }
            }
        }
    }

}
