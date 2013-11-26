using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignTestSuite.Papers.Alignment
{
    /// <summary>
    /// Encapsulates the pre and post alignment error for a given metric
    /// </summary>
    public class AlignmentMeasurement<T>
    {
        /// <summary>
        /// Gets or sets the pre alignment value
        /// </summary>
        public List<T> PreAlignment { get; set; }
        /// <summary>
        /// Gets or sets the post alignment value
        /// </summary>
        public List<T> PostAlignment { get; set; }
    }
}
