using System.Collections.Generic;
using NHibernate.Criterion;
using PNNLOmics.Data.MassTags;


namespace MultiAlignCore.IO.Features.Hibernate
{
    /// <summary>
    /// Accesses mass tags from the underlying database.
    /// </summary>
	public class MassTagDAOHibernate : GenericDAOHibernate<MassTagLight>, IMassTagDAO
    {

        /// <summary>
        /// Finds nearby mass tags.
        /// </summary>
        /// <param name="massMin"></param>
        /// <param name="massMax"></param>
        /// <param name="netMin"></param>
        /// <param name="netMax"></param>
        /// <returns></returns>
        public List<MassTagLight> FindNearby(double massMin, double massMax, double netMin, double netMax)
        {
            List<ICriterion> criterionList  = new List<ICriterion>();
            ICriterion criterionMass        = Expression.Between("MassMonoisotopic",    massMin,    massMax);
            ICriterion criterionNet         = Expression.Between("NETAverage",          netMin,     netMax);

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
        /// <returns></returns>
        public List<MassTagLight> FindNearby(double massMin, double massMax, double netMin, double netMax, double driftMin, double driftMax)
        {
            List<ICriterion> criterionList  = new List<ICriterion>();
            ICriterion criterionMass        = Expression.Between("MassMonoisotopic",   massMin,    massMax);
            ICriterion criterionNet         = Expression.Between("NETAverage",          netMin,     netMax);
            ICriterion criterionDriftTime   = Expression.Between("DriftTime", driftMin, driftMax);

            criterionList.Add(criterionMass);
            criterionList.Add(criterionNet);
            criterionList.Add(criterionDriftTime);

            return FindByCriteria(criterionList);
        }
        /// <summary>
        /// Finds mass tags based on an ID list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<MassTagLight> FindMassTags(List<int> ids)
        {
            ICriterion criterion            = Expression.In("ID", ids);
            List<ICriterion> criterionList  = new List<ICriterion>();

            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
    }
}
