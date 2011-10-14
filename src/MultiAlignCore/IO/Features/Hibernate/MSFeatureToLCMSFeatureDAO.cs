using System;
using System.Collections.Generic;
using NHibernate.Criterion;

using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSFeatureToLCMSFeatureDAO: GenericDAOHibernate<MSFeatureToLCMSFeatureMap>, IMSFeatureToLCMSFeatureDAO
    {
        /// <summary>
        /// Searches for and returns a List of Umc Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <param name="mass">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        public List<MSFeatureToLCMSFeatureMap> FindByMSFeatureDatasetId(int datasetId)
        {
            ICriterion criterion = Expression.Eq("DatasetID", datasetId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        #region IMSFeatureToLCMSFeatureDAO Members

        public List<MSFeatureToLCMSFeatureMap> FindByDatasetId(int datasetId)
        {
            return FindByMSFeatureDatasetId(datasetId);
        }

        #endregion
    }
}
