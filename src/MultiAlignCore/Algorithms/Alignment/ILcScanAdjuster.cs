using System.Collections.Generic;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;
using MultiAlignCore.IO;
using PNNLOmics.Data;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Adjusts the scans for the features.
    /// </summary>
    public interface ILcScanAdjuster : IProgressNotifer
    {
        List<UMCLight> AdjustScans(List<UMCLight> features, ISpectraProvider provider);
    }
}
