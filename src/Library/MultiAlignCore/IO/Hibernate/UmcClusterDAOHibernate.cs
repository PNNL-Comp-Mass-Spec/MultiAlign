#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using FeatureAlignment.Data.Features;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.Features;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    public class UmcClusterDAOHibernate : GenericDAOHibernate<UMCClusterLight>, IUmcClusterDAO
    {
        public List<UMCClusterLight> FindByCharge(int charge)
        {
            ICriterion criterion = Restrictions.Eq("ChargeState", charge);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Searches for and returns a List of UmcCluster Objects in the Database that have the exact Mass given.
        /// </summary>
        /// <param name="mass">Mass value to be searched for</param>
        /// <returns>List of UmcCluster Objects</returns>
        public List<UMCClusterLight> FindByMass(double mass)
        {
            ICriterion criterion = Restrictions.Eq("MassMonoisotopic", mass);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Adds a Collection of Objects to the Database.
        /// </summary>
        /// <param name="tCollection">Collection of Objects to be added</param>
        public new virtual void AddAllStateless(ICollection<UMCClusterLight> tCollection, IProgress<PRISM.ProgressData> progress = null)
        {
            DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);
            base.AddAllStateless(tCollection, progress);
            DatabaseIndexer.IndexClusters(NHibernateUtil.Path);
        }

        /// <summary>
        /// Finds nearby clusters.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="netMin"></param>
        /// <param name="netMax"></param>
        /// <returns></returns>
        public List<UMCClusterLight> FindNearby(double massMin, double massMax, double netMin, double netMax)
        {
            var criterionList = new List<ICriterion>();
            ICriterion criterionMass = Restrictions.Between("MassMonoisotopic", massMin, massMax);
            ICriterion criterionNet = Restrictions.Between("Net", netMin, netMax);

            criterionList.Add(criterionMass);
            criterionList.Add(criterionNet);

            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Finds nearby mass tags.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="netMin"></param>
        /// <param name="netMax"></param>
        /// <param name="driftMin"></param>
        /// <param name="driftMax"></param>
        /// <returns></returns>
        public List<UMCClusterLight> FindNearby(double massMin, double massMax, double netMin, double netMax,
            double driftMin, double driftMax)
        {
            var criterionList = new List<ICriterion>();
            ICriterion criterionMass = Restrictions.Between("MassMonoisotopic", massMin, massMax);
            ICriterion criterionNet = Restrictions.Between("Net", netMin, netMax);
            ICriterion criterionDriftTime = Restrictions.Between("DriftTime", driftMin, driftMax);

            criterionList.Add(criterionMass);
            criterionList.Add(criterionNet);
            criterionList.Add(criterionDriftTime);

            return FindByCriteria(criterionList);
        }

        public void ClearAllClusters()
        {
            using (var connection = new SQLiteConnection("Data Source=" + NHibernateUtil.Path, true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM T_Clusters";
                    command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE T_LCMS_Features  SET Cluster_ID = -1";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}