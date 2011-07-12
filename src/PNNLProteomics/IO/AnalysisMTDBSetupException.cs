using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNNLProteomics.IO
{
    /// <summary>
    /// Class thrown when there is a problem loading or specifying a mass tag database.
    /// </summary>
    public class AnalysisMTDBSetupException: Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public AnalysisMTDBSetupException(string message):
            base(message)
        {
        }
    }
}
