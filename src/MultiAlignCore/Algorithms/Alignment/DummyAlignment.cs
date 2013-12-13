using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.Alignment;
using PNNLOmics.Algorithms;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Dummy alignment class 
    /// </summary>
    public class DummyAlignment: IFeatureAligner
    {
        #region IFeatureAligner Members

        public classAlignmentData AlignFeatures(MassTagDatabase database, List<PNNLOmics.Data.Features.UMCLight> features, AlignmentOptions options, bool alignDriftTimes)
        {
            return null;
        }

        public classAlignmentData AlignFeatures(List<PNNLOmics.Data.Features.UMCLight> baseline, List<PNNLOmics.Data.Features.UMCLight> features, AlignmentOptions options)
        {
            return null;
        }

        #endregion

        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        #endregion
    }
}
