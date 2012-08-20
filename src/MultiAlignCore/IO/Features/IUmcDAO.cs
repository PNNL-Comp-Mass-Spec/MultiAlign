using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUmcDAO : IGenericDAO<clsUMC>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        List<clsUMC> FindByMass(double mass);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mass1"></param>
        /// <param name="mass2"></param>
        /// <returns></returns>
        List<clsUMC> FindByMassRange(double mass1, double mass2);
        /// <summary>
        /// Finds a feature based on a feature id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        clsUMC FindByFeatureID(int id);            
        /// <summary>
        /// Finds a feature based on a List of cluster IDs.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<clsUMC> FindByClusterID(List<int> idList);
        /// <summary>
        /// Searches for and returns a List of Umc Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <param name="mass">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        List<clsUMC> FindByDatasetId(int datasetId);

        /// <summary>
        /// Clears old alignment data from all of the features.
        /// </summary>
        void ClearAlignmentData();
    }
}
