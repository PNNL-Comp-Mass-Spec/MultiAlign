#region

using System.Collections.Generic;
using MultiAlignCore.Data;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSnFeatureToMSFeatureDAOHibernate :
        GenericDAOHibernate<MSFeatureToMSnFeatureMap>,
        IMsnFeatureToMSFeatureDAO
    {
        public List<MSFeatureToMSnFeatureMap> FindByMSFeatureDatasetId(int datasetId)
        {
            ICriterion criterion = Restrictions.Eq("MSDatasetID", datasetId);
            var criterionList = new List<ICriterion>();

            criterionList.Add(criterion);

            return FindByCriteria(criterionList);
        }


        public List<MSFeatureToMSnFeatureMap> FindByDatasetId(int datasetId)
        {
            return FindByMSFeatureDatasetId(datasetId);
        }

        #region IMsnFeatureToMSFeatureDAO Members

        public List<MSFeatureToMSnFeatureMap> FindByUMCFeatureId(int datasetId, int featureId)
        {
            ICriterion criterion = Restrictions.Eq("LCMSFeatureID", featureId);
            ICriterion datasetCriterion = Restrictions.Eq("MSDatasetID", datasetId);
            var criterionList = new List<ICriterion>();

            criterionList.Add(criterion);
            criterionList.Add(datasetCriterion);

            return FindByCriteria(criterionList);
        }

        public List<MSFeatureToMSnFeatureMap> FindByMsFeatureId(int datasetId, int featureId)
        {
            ICriterion criterion = Restrictions.Eq("MSFeatureID", featureId);
            ICriterion datasetCriterion = Restrictions.Eq("MSDatasetID", datasetId);
            var criterionList = new List<ICriterion>();

            criterionList.Add(criterion);
            criterionList.Add(datasetCriterion);

            return FindByCriteria(criterionList);
        }

        #endregion
    }
}