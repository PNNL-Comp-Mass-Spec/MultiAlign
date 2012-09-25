using System;
using System.Collections.Generic;
using NHibernate.Criterion;

using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSnFeatureToMSFeatureDAOHibernate:
        GenericDAOHibernate<MSFeatureToMSnFeatureMap>, 
        IMsnFeatureToMSFeatureDAO
    {
        public List<MSFeatureToMSnFeatureMap> FindByMSFeatureDatasetId(int datasetId)
        {
            ICriterion criterion            = Expression.Eq("MSDatasetID", datasetId);
            List<ICriterion> criterionList  = new List<ICriterion>();

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
            ICriterion criterion            = Expression.Eq("LCMSFeatureID", featureId);
            ICriterion datasetCriterion     = Expression.Eq("MSDatasetID", datasetId);
            List<ICriterion> criterionList  = new List<ICriterion>();

            criterionList.Add(criterion);
            criterionList.Add(datasetCriterion);

            return FindByCriteria(criterionList);
        }

        #endregion
    }
}
