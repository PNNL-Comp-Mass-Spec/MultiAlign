using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Annotations;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    public interface IFeatureMatcher<TObserved, TTarget> 
        where TObserved : FeatureLight, new() 
        where TTarget : FeatureLight, new()
    {
        /// <summary>
        /// Gets or sets the initial parameters used for matching.
        /// </summary>
        [UsedImplicitly]
        FeatureMatcherParameters MatchParameters { get; set; }

        /// <summary>
        /// Gets the list of feature matches.
        /// </summary>
        [UsedImplicitly]
        List<FeatureMatch<TObserved, TTarget>> MatchList { get; }

        /// <summary>
        /// Gets the list of features matched with a shift.
        /// </summary>
        [UsedImplicitly]
        List<FeatureMatch<TObserved, TTarget>> ShiftedMatchList { get; }

        /// <summary>
        /// Gets the FDR calculated by using a fixed shift.  Calculated as (# shifted matches)/(# shifted matches + # non-shifted matches).
        /// </summary>
        [UsedImplicitly]
        double ShiftFdr { get; }

        /// <summary>
        /// Gets the FDR calculated by using a fixed shift.  Calculated as 2*(# shifted matches)/(# shifted matches + # non-shifted matches).
        /// </summary>
        [UsedImplicitly]
        double ShiftConservativeFdr { get; }

        /// <summary>
        /// Gets the FDR calculated by using a mass error histogram.
        /// </summary>
        [UsedImplicitly]
        double ErrorHistogramFdr { get; }

        /// <summary>
        /// Gets the list of parameters trained by STAC.  Each entry is a different charge state.
        /// </summary>
        List<STACInformation> StacParameterList { get; }

        /// <summary>
        /// Get the STAC FDR table.
        /// </summary>
        List<STACFDR> StacFdrTable { get; }

        /// <summary>
        /// Gets the list of refined tolerances used for SLiC and shift.  Each entry is a different charge state.
        /// </summary>
        [UsedImplicitly]
        List<FeatureMatcherTolerances> RefinedToleranceList { get; }

        /// <summary>
        /// Gets the parameters used in calculating SLiC.
        /// </summary>
        [UsedImplicitly]
        SLiCInformation SliCParameters { get; }
        
        /// <summary>
        /// Find a list of matches between two lists.
        /// </summary>
        /// <param name="shortObservedList">List of observed features.  Possibly a subset of the entire list corresponding to a particular charge state.</param>
        /// <param name="shortTargetList">List of target features.  Possibly a subset of the entire list corresponding to a particular charge state.</param>
        /// <param name="tolerances">Tolerances to be used for matching.</param>
        /// <param name="shiftAmount">A fixed shift amount to use for populating the shifted match list.</param>
        /// <returns>A list of type FeatureMatch containing matches within the defined region.</returns>
        List<FeatureMatch<TObserved, TTarget>> FindMatches(List<TObserved> shortObservedList, List<TTarget> shortTargetList, FeatureMatcherTolerances tolerances, double shiftAmount);

        /// <summary>
        /// Fills in the values for the STAC FDR table.
        /// </summary>
        List<STACFDR> PopulateStacfdrTable(List<FeatureMatch<TObserved,TTarget>> matchList);

        event EventHandler<ProgressNotifierArgs> MessageEvent;
        event EventHandler<ProgressNotifierArgs> ProcessingCompleteEvent;

        /// <summary>
        /// Function to call to re-calculate algorithm results. 
        /// </summary>
        void MatchFeatures();
    }
}