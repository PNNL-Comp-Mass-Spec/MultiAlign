
using InformedProteomics.Backend.Utils;
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
            IEnumerable<UMCLight> baselineFeatures,
            IProgress<ProgressData> progress = null)
        {
            progress = progress ?? new Progress<ProgressData>();
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
            IProgress<ProgressData> progress = null)
        {
            progress = progress ?? new Progress<ProgressData>();
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
            if (Progress != null)
            {
                Progress(sender, e);
            }
        }

        #endregion

    }
}