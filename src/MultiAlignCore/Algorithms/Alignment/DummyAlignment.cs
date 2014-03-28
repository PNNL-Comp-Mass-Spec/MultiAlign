using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Dummy alignment class 
    /// </summary>
    public class DummyAlignment:
                IFeatureAligner<IEnumerable<UMCLight>,  IEnumerable<UMCLight>, classAlignmentData>,
                IFeatureAligner<MassTagDatabase,        IEnumerable<UMCLight>, classAlignmentData>
    {

        /// <summary>
        /// Gets or sets the baseline spectra provider
        /// </summary>
        public ISpectraProvider BaselineSpectraProvider { get; set; }
        /// <summary>
        /// Gets or sets the alignee spectra provider.
        /// </summary>
        public ISpectraProvider AligneeSpectraProvider { get; set; }

        private void OnStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }

        public classAlignmentData Align(MassTagDatabase         database,
                                        IEnumerable<UMCLight>   features)
        {
            return null;
        }

        public classAlignmentData Align(IEnumerable<UMCLight>  baseline, 
                                        IEnumerable<UMCLight>  features)
        {
            return null;
        }

        public event EventHandler<ProgressNotifierArgs> Progress;
    }
}
