#region

using System.Collections.Generic;
using MultiAlignCore.Data.MassTags;

#endregion

namespace MultiAlignCore.IO.MassTags
{
    public interface IMassTagDAO : IGenericDAO<MassTagLight>
    {
        /// <summary>
        /// Finds nearby tags based on mass and net.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="netMin"></param>
        /// <param name="netMax"></param>
        /// <returns></returns>
        List<MassTagLight> FindNearby(double massMin, double massMax, double netMin, double netMax);

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
        List<MassTagLight> FindNearby(double massMin, double massMax, double netMin, double netMax, double driftMin,
            double driftMax);

        void DeleteAll();

        List<MassTagLight> FindMassTags(List<int> ids);
    }
}