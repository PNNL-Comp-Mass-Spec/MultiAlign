namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.Features;

    /// <summary>
    /// This is an interface that defines how an object that loads feature data from
    /// a feature results file should be used.
    /// </summary>
    public interface IDatasetLoader
    {
        /// <summary>
        /// Loads features from a dataset and persists the features to a data access object.
        /// </summary>
        /// <param name="dataset">The data set to load.</param>
        /// <param name="dataAccessProvider">The data access object to persist features to.</param>
        /// <param name="progress">The progress reporter to report progress and status messages to.</param>
        void Load(DatasetInformation dataset, IUmcDAO dataAccessProvider, IProgress<ProgressData> progress = null);

        /// <summary>
        /// Loads features from a dataset.
        /// </summary>
        /// <param name="dataset">The data set to load.</param>
        /// <param name="progress">The progress reporter to report progress and status messages to.</param>
        /// <returns>The features that were loaded.</returns>
        List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null);
    }
}
