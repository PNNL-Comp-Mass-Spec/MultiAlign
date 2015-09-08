using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignRogue.Clustering
{
    public class ClusterViewerSettings
    {
        public ClusterViewerSettings()
        {
            this.ShouldReducePoints = true;
            this.NetDivisions = 100;
            this.MassDivisions = 100;
            this.PointsPerDivision = 1;
            this.ShowDivisionLines = false;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the points on the cluster plot
        /// should be reduced to improve performance.
        /// </summary>
        public bool ShouldReducePoints { get; set; }

        /// <summary>
        /// Gets or sets the number of divisions on the net axis.
        /// </summary>
        public int NetDivisions { get; set; }

        /// <summary>
        /// Gets or sets the number of divisions on the mass axis.
        /// </summary>
        public int MassDivisions { get; set; }

        /// <summary>
        /// Gets or sets the number of points to display per 2-dimensional
        /// division in mass in net.
        /// </summary>
        public int PointsPerDivision { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether lines visualizing the
        /// mass and net divisions should be displayed on the cluster plot.
        /// </summary>
        public bool ShowDivisionLines { get; set; }
    }
}
