using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    public class LcmsWarpStatistics
    {
        public double Mu { get; set; }

        private double massStdDev;
        public double MassStdDev
        {
            get { return this.massStdDev; }
            set
            {
                this.massStdDev = value;
                this.Log2PiMassStdDevSq = Math.Log(2 * Math.PI * this.MassVariance);
            }
        }

        public double MassVariance
        {
            get { return this.MassStdDev * this.MassStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiMassStdDevSq { get; private set; }

        private double netStdDev;
        public double NetStdDev
        {
            get { return this.netStdDev; }
            set
            {
                this.netStdDev = value;
                this.Log2PiNetStdDevSq = Math.Log(2 * Math.PI * this.NetVariance);
            }
        }

        public double NetVariance
        {
            get { return this.NetStdDev * this.NetStdDev; }
        }

        /// <summary>
        /// Gets the calculations of log[2pi * (StdDev)^2]
        /// </summary>
        public double Log2PiNetStdDevSq { get; private set; }

        public double NormalProbability { get; set; }
    }
}
