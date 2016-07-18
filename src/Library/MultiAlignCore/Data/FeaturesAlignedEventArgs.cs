#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Data
{
    /// <summary>
    ///     Event arguments when two datasets are aligned.
    /// </summary>
    public sealed class FeaturesAlignedEventArgs : EventArgs
    {
        private readonly DatasetInformation m_datasetInformation;

        /// <summary>
        ///     Arguments that hold alignment information when a dataset is aligned.
        /// </summary>
        public FeaturesAlignedEventArgs(DatasetInformation datasetInfo,
            IEnumerable<UMCLight> baselineFeatures,
            IEnumerable<UMCLight> aligneeFeatures,
            AlignmentData alignmentData)
        {
            m_datasetInformation = datasetInfo;
            BaselineFeatures = baselineFeatures;
            AligneeFeatures = aligneeFeatures;
            AlignmentData = alignmentData;
        }

        /// <summary>
        ///     Gets the dataset information for the alignee dataset.
        /// </summary>
        public DatasetInformation AligneeDatasetInformation
        {
            get { return m_datasetInformation; }
        }

        /// <summary>
        ///     Gets the baseline features used in alignment.
        /// </summary>
        [UsedImplicitly]
        public IEnumerable<UMCLight> BaselineFeatures { get; private set; }

        /// <summary>
        ///     Gets the alignee features used in alignment.
        /// </summary>
        [UsedImplicitly]
        public IEnumerable<UMCLight> AligneeFeatures { get; private set; }

        /// <summary>
        ///     Gets the alignment data associated between baseline and alignee.
        /// </summary>
        public AlignmentData AlignmentData { get; private set; }
    }
}