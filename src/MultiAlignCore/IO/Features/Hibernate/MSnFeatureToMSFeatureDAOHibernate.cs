using System;
using System.Collections.Generic;
using NHibernate.Criterion;

using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSnFeatureToMSFeatureDAOHibernate: GenericDAOHibernate<MSFeatureToMSnFeatureMap>, IMsnFeatureToMSFeatureDAO
    {
        public List<MSFeatureToMSnFeatureMap> FindByMSFeatureDatasetId(int datasetId)
        {
            ICriterion criterion = Expression.Eq("MSDatasetID", datasetId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        #region IMsnFeatureToMSFeatureDAO Members

        public List<MSFeatureToMSnFeatureMap> FindByDatasetId(int datasetId)
        {
            return FindByMSFeatureDatasetId(datasetId);
        }

        #endregion
    }
}
