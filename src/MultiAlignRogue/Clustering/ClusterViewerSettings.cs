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

        /// <summary>
        /// Override of the equals object for comparing by value.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the values of the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            var rightSettings = obj as ClusterViewerSettings;
            return rightSettings != null &&
                   this.ShouldReducePoints == rightSettings.ShouldReducePoints &&
                   this.NetDivisions == rightSettings.NetDivisions &&
                   this.MassDivisions == rightSettings.MassDivisions &&
                   this.PointsPerDivision == rightSettings.PointsPerDivision &&
                   this.ShowDivisionLines == rightSettings.ShowDivisionLines;
        }

        /// <summary>
        /// Override GetHashCode for value comparison.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.ShouldReducePoints.GetHashCode() * 23 ^ this.NetDivisions.GetHashCode() * 17
                   ^ this.MassDivisions.GetHashCode() * 31 ^ this.PointsPerDivision.GetHashCode() * 47
                   ^ this.ShowDivisionLines.GetHashCode() * 59;
        }
    }
}
