#region

using System.Collections.Generic;
using PNNLOmics.Algorithms.FeatureMatcher.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

#endregion

namespace MultiAlignCore.Data
{
    /// <summary>
    ///     Encapsulates peak matched result data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class PeakMatchingResults<T, U>
        where T : FeatureLight
        where U : MassTagLight
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public PeakMatchingResults()
        {
            ShiftedMatches = new List<FeatureMatchLight<UMCClusterLight, MassTagLight>>();
            Matches = new List<FeatureMatchLight<UMCClusterLight, MassTagLight>>();
            FdrTable = new List<STACFDR>();
            PeakMatchedToMassTagDB = false;
        }

        /// <summary>
        ///     Gets or sets the list of available matches.
        /// </summary>
        public List<FeatureMatchLight<UMCClusterLight, MassTagLight>> Matches { get; set; }

        /// <summary>
        ///     Gets or sets the list of available matches shifted by the dalton correction.
        /// </summary>
        public List<FeatureMatchLight<UMCClusterLight, MassTagLight>> ShiftedMatches { get; set; }

        /// <summary>
        ///     Gets or sets the FDR Table from STAC
        /// </summary>
        public List<STACFDR> FdrTable { get; set; }

        /// <summary>
        ///     Gets the flag whether the results were peaked matched against the Mass Tag Database.
        /// </summary>
        [DataSummary("Peaks Matched to MTDB")]
        public bool PeakMatchedToMassTagDB { get; private set; }
    }
}