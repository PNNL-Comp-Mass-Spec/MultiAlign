/*////////////////////////////////////////////////////////////////////////////////////////////////////////////
 *
 * Name:    IClusterer Interface
 * File:    IClusterer.cs
 * Author:  Brian LaMarche
 * Purpose: Interface for clustering UMC data.
 * Date:    5-19-2010
 * Revisions:
 *          5-19-2010 - BLL - Created interface.
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

using System;
using System.Collections.Generic;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Data.Features;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Interface describing how to cluster
    /// </summary>
    /// <typeparam name="T">Type to cluster data objects of.</typeparam>
    /// <typeparam name="U">Type of cluster output objects.</typeparam>
    public interface IClusterer<T, U>:  IProgressNotifer
        where T: FeatureLight, new ()
    {
        FeatureClusterParameters<T> Parameters { get; set; }

        /// <summary>
        /// Clusters the data objects provided in the list.
        /// </summary>
        /// <param name="data">Data to cluster.</param>
        /// <param name="clusters"></param>
        /// <param name="progress"></param>
        List<U> Cluster(List<T> data, List<U> clusters, IProgress<PRISM.ProgressData> progress = null);

        /// <summary>
        /// Progress information
        /// </summary>
        /// <param name="data"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        List<U> Cluster(List<T> data, IProgress<PRISM.ProgressData> progress = null);

        /// <summary>
        /// Clusters and writes data to teh stream writer provided.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="progress"></param>
        void ClusterAndProcess(List<T> data, IClusterWriter<U> writer, IProgress<PRISM.ProgressData> progress = null);
    }
}
