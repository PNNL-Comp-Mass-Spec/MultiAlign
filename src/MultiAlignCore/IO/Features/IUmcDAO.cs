using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUmcDAO : IGenericDAO<UMCLight>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        List<UMCLight> FindByMass(double mass);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mass1"></param>
        /// <param name="mass2"></param>
        /// <returns></returns>
        List<UMCLight> FindByMassRange(double mass1, double mass2);
        /// <summary>
        /// Finds a feature based on a feature id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UMCLight FindByFeatureID(int id);            
        /// <summary>
        /// Finds a feature based on a List of cluster IDs.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<UMCLight> FindByClusterID(List<int> idList);
        /// <summary>
        /// Searches for and returns a List of Umc Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <param name="mass">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        List<UMCLight> FindByDatasetId(int datasetId);
        /// <summary>
        /// Finds the set of UMC's based on a specific charge state.
        /// </summary>
        /// <param name="charge"></param>
        /// <returns></returns>
        List<UMCLight> FindByCharge(int charge);
        /// <summary>
        /// Finds all features that are clustered.
        /// </summary>
        /// <returns></returns>
        List<UMCLight> FindAllClustered();
        /// <summary>
        /// Clears old alignment data from all of the features.
        /// </summary>
        void ClearAlignmentData();
    }
}
