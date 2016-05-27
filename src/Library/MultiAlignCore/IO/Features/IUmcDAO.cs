namespace MultiAlignCore.IO.Features
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This is an interface for defining the operations that can be
    /// performed on feature data from a data-access object.
    /// </summary>
    public interface IUmcDAO
    {
        /// <summary>
        /// Find features by mass.
        /// </summary>
        /// <param name="mass">The mass of the feature.</param>
        /// <returns>The features found for the provided mass.</returns>
        List<UMCLight> FindByMass(double mass);

        /// <summary>
        /// Find features within a mass range.
        /// </summary>
        /// <param name="mass1">The minimum mass (inclusive).</param>
        /// <param name="mass2">The maximum mass (inclusive).</param>
        /// <returns>The features within the provided mass range.</returns>
        List<UMCLight> FindByMassRange(double mass1, double mass2);

        /// <summary>
        /// Finds a feature based on a unique feature id.
        /// </summary>
        /// <param name="id">The unique identification number of the feature to find.</param>
        /// <returns>The feature found with the provided identification number.</returns>
        UMCLight FindByFeatureID(int id);

        /// <summary>
        /// Finds features that belong to a cluster with the provided unique identification number.
        /// </summary>
        /// <param name="id">The unique identification number of the cluster.</param>
        /// <returns>The features that are part of the cluster.</returns>
        List<UMCLight> FindByClusterID(int id);

        /// <summary>
        /// Finds features that belong to several clusters with the provided unique identification numbers.
        /// </summary>
        /// <param name="id">The unique identification number of the cluster.</param>
        /// <returns>The features that are part of the clusters.</returns>
        List<UMCLight> FindByClusterID(List<int> idList);

        /// <summary>
        /// Finds all features that are part of the given dataset.
        /// </summary>
        /// <param name="">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        List<UMCLight> FindByDatasetId(int datasetId);

        /// <summary>
        /// Save all features for the provided dataset.
        /// </summary>
        /// <param name="features">The features to save.</param>
        /// <param name="datasetId">The ID of the the dataset.</param>
        /// <param name="progress">The progress reporter to report percent complete and status message to.</param>
        void SaveFeaturesByDataset(List<UMCLight> features, int datasetId, IProgress<ProgressData> progress = null);

        /// <summary>
        /// Delete all features for a provided dataset.
        /// </summary>
        /// <param name="datasetId">The dataset to delete features for.</param>
        void DeleteByDataset(int datasetId);

        /// <summary>
        /// Find all features.
        /// </summary>
        /// <returns>The features.</returns>
        List<UMCLight> FindAll();

            /// <summary>
        /// Finds all features with the provided charge state.
        /// </summary>
        /// <param name="charge">The charge state.</param>
        /// <returns>The features found with the provided charge state.</returns>
        List<UMCLight> FindByCharge(int charge);

        /// <summary>
        /// Finds all features with provided charge that are part of the provided dataset.
        /// </summary>
        /// <param name="charge">The charge state.</param>
        /// <param name="dataset">The unique identification number of features to find.</param>
        /// <returns>The features.</returns>
        List<UMCLight> FindByChargeDataset(int charge, int dataset);

        /// <summary>
        /// Finds all features that are members of clusters.
        /// </summary>
        /// <returns>The features found.</returns>
        List<UMCLight> FindAllClustered();

        /// <summary>
        /// Find the highest charge state in all datasets.
        /// </summary>
        /// <returns>The charge state.</returns>
        int FindMaxCharge();

        /// <summary>
        /// Clears old alignment data from all of the features.
        /// </summary>
        void ClearAlignmentData();

        /// <summary>
        /// Retrieves the charge states for all features.
        /// </summary>
        /// <returns>Collection of charge states.</returns>
        IEnumerable<int> RetrieveChargeStates();
    }
}