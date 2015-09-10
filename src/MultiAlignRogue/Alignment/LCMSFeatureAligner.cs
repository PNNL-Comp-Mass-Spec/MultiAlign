
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignRogue.Alignment
{
    using System;
    using System.Collections.Generic;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.MetaData;

    
    class LCMSFeatureAligner
    {

        public AlgorithmProvider m_algorithms { get; set; }

        public AlignmentData AlignToDataset(
            ref IList<UMCLight> features,
            DatasetInformation datasetInfo,
            IEnumerable<UMCLight> baselineFeatures)
        {            
            // Align pairwise and cache results intermediately.           
            var aligner = this.m_algorithms.DatasetAligner;
            aligner.Progress += aligner_Progress;

            var alignmentData = aligner.Align(baselineFeatures, features);
            
            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }

            aligner.Progress -= aligner_Progress;

            return alignmentData;
        }

        public AlignmentData AlignToDatabase(
            ref IList<UMCLight> features,
            DatasetInformation datasetInfo,
            MassTagDatabase mtdb)
        {
            var aligner = this.m_algorithms.DatabaseAligner;
            var alignmentData = aligner.Align(mtdb, features);
            aligner.Progress += aligner_Progress;

            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }

            aligner.Progress -= aligner_Progress;

            return alignmentData;
        }

        #region ProgressReporting

        public event EventHandler<ProgressNotifierArgs> Progress;

        void aligner_Progress(object sender, ProgressNotifierArgs e)
        {
            if (Progress != null)
            {
                Progress(sender, e);
            }
        }

        #endregion

    }
}