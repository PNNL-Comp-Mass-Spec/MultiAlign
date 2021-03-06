#region

using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.Features;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    public class MSFeatureDAOHibernate : GenericDAOHibernate<MSFeatureLight>, IMSFeatureDAO
    {
        /// <summary>
        /// Searches for and returns a List of MS feature Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <returns>List of Umc Objects</returns>
        public List<MSFeatureLight> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Restrictions.Eq("GroupId", datasetId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public void DeleteByDatasetId(int datasetId)
        {
            DeleteByCriteria("T_MSFeatures", "DATASET_ID", datasetId);
        }

        public List<MSFeatureLight> FindByFeatureId(int datasetId, int featureId)
        {
            ICriterion criterion = Restrictions.Eq("GroupId", datasetId);
            ICriterion featureCriterion = Restrictions.Eq("UmcId", featureId);
            var criterionList = new List<ICriterion>();

            criterionList.Add(featureCriterion);
            criterionList.Add(criterion);

            return FindByCriteria(criterionList);
        }
    }
}