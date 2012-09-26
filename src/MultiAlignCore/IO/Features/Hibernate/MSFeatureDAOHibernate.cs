using System.Collections.Generic;
using NHibernate.Criterion;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features.Hibernate
{

	public class MSFeatureDAOHibernate : GenericDAOHibernate<MSFeatureLight>, IMSFeatureDAO
	{
        /// <summary>
        /// Searches for and returns a List of MS feature Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <param name="mass">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        public List<MSFeatureLight> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Expression.Eq("GroupID", datasetId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public List<MSFeatureLight> FindByFeatureId(int datasetId, int featureId)
        {
            ICriterion criterion                = Expression.Eq("GroupID",      datasetId);
            ICriterion featureCriterion         = Expression.Eq("UMCID",           featureId);
            List<ICriterion> criterionList      = new List<ICriterion>();

            criterionList.Add(featureCriterion);
            criterionList.Add(criterion);

            return FindByCriteria(criterionList);
        }
	}

}
