using PNNLOmics.Data.MassTags;

namespace MultiAlignRogue.Alignment
{
    using System;
    using System.Collections.Generic;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.MetaData;

    using PNNLOmics.Data.Features;

    class LCMSFeatureAligner
    {

        public AlgorithmProvider m_algorithms { get; set; }

        public classAlignmentData AlignToDataset(
            ref IList<UMCLight> features,
            DatasetInformation datasetInfo,
            IEnumerable<UMCLight> baselineFeatures)
        {            
            // Align pairwise and cache results intermediately.           
            var aligner = this.m_algorithms.DatasetAligner;
            classAlignmentData alignmentData = aligner.Align(baselineFeatures, features);
            
            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }
           
            return alignmentData;
        }

        public classAlignmentData AlignToDatabase(
            ref IList<UMCLight> features,
            DatasetInformation datasetInfo,
            MassTagDatabase mtdb)
        {
            var aligner = this.m_algorithms.DatabaseAligner;
            classAlignmentData alignmentData = aligner.Align(mtdb, features);            

            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }

            return alignmentData;
        }

    }
}