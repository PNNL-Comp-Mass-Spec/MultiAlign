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
            IEnumerable<UMCLight> baselineFeatures,
            DatasetInformation datasetInfo,
            DatasetInformation baselineInfo)
        {
            classAlignmentData alignmentData;
            if (baselineInfo == null)
            {
                throw new NullReferenceException("No reference was set for LC-MS alignment.");
            }
            // Align pairwise and cache results intermediately.
            var aligner = this.m_algorithms.DatasetAligner;
            alignmentData = aligner.Align(baselineFeatures, features);
            
            if (alignmentData != null)
            {
                alignmentData.aligneeDataset = datasetInfo.DatasetName;
                alignmentData.DatasetID = datasetInfo.DatasetId;
            }
            
            //var args = new FeaturesAlignedEventArgs(datasetInfo, baselineFeatures, features, alignmentData);
            return alignmentData;
        }

    }
}