using FeatureAlignment.Algorithms;
using FeatureAlignment.Data.Alignment;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue.Alignment
{
    using System;
    using System.Collections.Generic;


    class LCMSFeatureAligner
    {

        public AlgorithmProvider m_algorithms { get; set; }

        public AlignmentData AlignToDataset(
            ref IList<UMCLight> features,
            DatasetInformation datasetInfo,
            IEnumerable<UMCLight> baselineFeatures,
            IProgress<PRISM.ProgressData> progress = null)
        {
            progress = progress ?? new Progress<PRISM.ProgressData>();
            // Align pairwise and cache results intermediately.
            var aligner = this.m_algorithms.DatasetAligner;
            aligner.Progress += aligner_Progress;

            var alignmentData = aligner.Align(baselineFeatures, features, progress);

            if (alignmentData != null)
            {
                alignmentData.AligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }

            aligner.Progress -= aligner_Progress;

            return alignmentData;
        }

        public AlignmentData AlignToDatabase(
            ref IList<UMCLight> features,
            DatasetInformation datasetInfo,
            MassTagDatabase mtdb,
            IProgress<PRISM.ProgressData> progress = null)
        {
            progress = progress ?? new Progress<PRISM.ProgressData>();
            var aligner = this.m_algorithms.DatabaseAligner;
            var alignmentData = aligner.Align(mtdb, features, progress);
            aligner.Progress += aligner_Progress;

            if (alignmentData != null)
            {
                alignmentData.AligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }

            aligner.Progress -= aligner_Progress;

            return alignmentData;
        }

        #region ProgressReporting

        public event EventHandler<ProgressNotifierArgs> Progress;

        void aligner_Progress(object sender, ProgressNotifierArgs e)
        {
            Progress?.Invoke(sender, e);
        }

        #endregion

    }
}