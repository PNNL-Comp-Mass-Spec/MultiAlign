#region

using System.Collections.Generic;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features.Hibernate;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.SequenceData
{
    public sealed class SequenceToMsnDaoHibernate : GenericDAOHibernate<SequenceToMsnFeature>, ISequenceToMsnFeatureDao
    {
        public List<SequenceToMsnFeature> FindByDatasetId(int datasetId, int lcmsFeatureId)
        {
            ICriterion criterionDataset = Restrictions.Eq("DatasetId", datasetId);
            ICriterion criterionFeature = Restrictions.Eq("UmcFeatureId", lcmsFeatureId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterionDataset);
            criterionList.Add(criterionFeature);

            return FindByCriteria(criterionList);
        }

        public List<SequenceToMsnFeature> FindByDatasetId(int datasetId)
        {
            ICriterion criterionDataset = Restrictions.Eq("DatasetId", datasetId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterionDataset);

            return FindByCriteria(criterionList);
        }
    }
}