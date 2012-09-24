using System.Collections.Generic;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Adjusts the scans for the features.
    /// </summary>
    public interface ILcScanAdjuster : IProgressNotifer
    {
        List<UMCLight> AdjustScans(List<UMCLight> features);
    }
}
