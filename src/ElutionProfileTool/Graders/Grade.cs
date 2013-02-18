using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    /// <summary>
    /// Class that holds rating levels for users.
    /// </summary>
    public class ExpertLevel
    {
        /// <summary>
        /// Gets or sets the name of the grade.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the next grade.
        /// </summary>
        public ExpertLevel NextLevel
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets minimum rating that must be achieved for this grade
        /// </summary>
        public int MinimumRating
        {
            get;
            set;
        }
    }
}
