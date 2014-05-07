using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    public class ReportFileUtility
    {
        static ReportFileUtility()
        {
            BasePath = "";
        }
        /// <summary>
        /// Gets or sets the base path to place this data.
        /// </summary>
        public static string BasePath { get; set; }

        /// <summary>
        /// Generates a new file name for reporting
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public static string GenerateName(string name)
        {
            name = string.Format("AlignmentPaper-{0}-{1}",
                                        name,
                                        DateTime.Now.ToString("YYYY-MM-DD-HH-MM-SS"));                            
            return name;
        }
    }
}
