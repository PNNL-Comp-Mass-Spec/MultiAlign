using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Interpolation
{
    public class SplineInterpolation
    {
        // used to temporarily store the spline coefficients
        private readonly List<double> m_tempSpline = new List<double>();

        // vector to store the second derivatives at knot points of spline
        private readonly List<double> m_y2 = new List<double>();

        public void Spline(ref List<double> x, ref List<double> y, double yp1, double ypn)
        {
            m_tempSpline.Clear();
            var n = x.Count;
            int i, k;
            double qn;
            double un;

            m_y2.Capacity = n;

            if (yp1 > 0.99e30)
            {
                m_y2[0] = 0.0;
                m_tempSpline.Add(0);
            }
            else
            {
                m_y2[0] = -0.5f;
                m_tempSpline.Add((3.0f / (x[1] - x[0])) * ((y[1] - y[0]) / (x[1] - x[0]) - yp1));
            }
            //Generate second derivatives at internal points using recursive spline equations
            for (i = 1; i <= n - 2; i++)
            {
                var sig = (x[i] - x[i - 1]) / (x[i + 1] - x[i - 1]);
                var p = sig * m_y2[i - 1] + 2.0;
                m_y2[i] = (sig - 1.0) / p;
                m_tempSpline.Add((y[i + 1] - y[i]) / (x[i + 1] - x[i]) - (y[i] - y[i - 1]) / (x[i] - x[i - 1]));
                m_tempSpline[i] = (6.0 * m_tempSpline[i] / (x[i + 1] - x[i - 1]) - sig * m_tempSpline[i - 1]) / p;
            }
            if (ypn > 0.99e30)
            {
                qn = un = 0.0;
            }
            else
            {
                qn = 0.5;
                un = (3.0 / (x[n - 1] - x[n - 2])) * (ypn - (y[n - 1] - y[n - 2]) / (x[n - 1] - x[n - 2]));
            }
            m_y2[n - 1] = (un - qn * m_tempSpline[n - 2]) / (qn * m_y2[n - 2] + 1.0);
            for (k = n - 2; k >= 0; k--)
            {
                m_y2[k] = m_y2[k] * m_y2[k + 1] + m_tempSpline[k];
            }
        }

        public double Splint(ref List<double> xa, ref List<double> ya, double x)
        {
            var n = xa.Count;
            var klo = 0;
            var khi = n - 1;

            //Binary search for khi and klo
            while (khi - klo > 1)
            {
                var k = (khi + klo) >> 1;
                if (xa[k] > x)
                {
                    khi = k;
                }
                else
                {
                    klo = k;
                }
            }
            var h = xa[khi] - xa[klo];
            if (System.Math.Abs(h) < double.Epsilon)
            {
                return -1;
            }
            var a = (xa[khi] - x) / h;
            var b = (x - xa[klo]) / h;

            //cubic interpolation at x
            var y = a * ya[klo] + b * ya[khi] + ((a * a * a - a) * m_y2[klo] + (b * b * b - b) * m_y2[khi]) * (h * h) / 6.0;
            return y;
        }
    }
}
