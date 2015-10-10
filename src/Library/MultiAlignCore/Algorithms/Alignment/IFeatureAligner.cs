
using System;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Alignment
{
    public interface IFeatureAligner<in TBaselineFeature, in TAligneeFeature, out TMatchData>: IProgressNotifer
    {
        /// <summary>
        /// Aligns the alignee features to the baseline features
        /// </summary>
        /// <param name="baseline">Typically an IEnumerable of a datatype T (UMCLight) or an instance of a MassTagDatabase used as the alignment reference.</param>
        /// <param name="alignee">Typically an IEnumerable of a datatype T that are aligned to a reference.</param>        
        /// <returns></returns>
        TMatchData Align(TBaselineFeature baseline, TAligneeFeature alignee, IProgress<ProgressData> progress = null);
        /// <summary>
        /// Gets or sets the baseline spectra provider
        /// </summary>
        IScanSummaryProvider BaselineSpectraProvider { get; set; }
        /// <summary>
        /// Gets or sets the alignee spectra provider.
        /// </summary>
        IScanSummaryProvider AligneeSpectraProvider { get; set; }
    }
}
