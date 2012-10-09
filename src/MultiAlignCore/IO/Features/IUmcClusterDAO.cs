using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features
{
    public interface IUmcClusterDAO : IGenericDAO<UMCClusterLight>
    {
        List<UMCClusterLight> FindByMass(double mass);
        /// <summary>
        /// Find nearby clusters skipping the one specified.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="netMin"></param>
        /// <param name="netMax"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        List<UMCClusterLight> FindNearby(double massMin, double massMax, double netMin, double netMax);
        /// <summary>
        /// Finds nearby mass tags based on mass, net, and drift time.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="netMin"></param>
        /// <param name="netMax"></param>
        /// <param name="driftMin"></param>
        /// <param name="driftMax"></param>
        /// <returns></returns>
        List<UMCClusterLight> FindNearby(double massMin, double massMax, double netMin, double netMax, double driftMin, double driftMax);
        void ClearAllClusters();
    }
}
