#region

using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Dummy alignment class
    /// </summary>
    public class DummyAlignment :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<MassTagDatabase, IEnumerable<UMCLight>, AlignmentData>
    {
        /// <summary>
        /// Gets or sets the baseline spectra provider
        /// </summary>
        public IScanSummaryProvider BaselineSpectraProvider { get; set; }

        /// <summary>
        /// Gets or sets the alignee spectra provider.
        /// </summary>
        public IScanSummaryProvider AligneeSpectraProvider { get; set; }

        private void OnStatus(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }

        public AlignmentData Align(MassTagDatabase database,
            IEnumerable<UMCLight> features, IProgress<PRISM.ProgressData> progress = null)
        {
            return null;
        }

        public AlignmentData Align(IEnumerable<UMCLight> baseline,
            IEnumerable<UMCLight> features, IProgress<PRISM.ProgressData> progress = null)
        {
            return null;
        }

        public event EventHandler<ProgressNotifierArgs> Progress;
    }
}