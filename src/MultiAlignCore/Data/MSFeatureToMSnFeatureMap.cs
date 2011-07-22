using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Data
{
    
    /// <summary>
    /// Maps a MS Feature to a MSMS feature 
    /// </summary>
    public class MSFeatureToMSnFeatureMap
    {
        /// <summary>
        /// The raw dataset ID link.
        /// </summary>
        public int RawDatasetID
        {
            get;
            set;
        }
        /// <summary>
        /// The MS dataset ID link
        /// </summary>
        public int MSDatasetID
        {
            get;
            set;
        }
        /// <summary>
        /// The MS Feature ID
        /// </summary>
        public int MSFeatureID
        {
            get;
            set;
        }
        /// <summary>
        /// The MSMS Feature ID
        /// </summary>
        public int MSMSFeatureID
        {
            get;
            set;
        }
    }
}
