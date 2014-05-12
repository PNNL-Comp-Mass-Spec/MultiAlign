#region

using System.Collections.Generic;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features.Hibernate;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.SequenceData
{
    public class DatabaseSearchSequenceDAOHiberate : GenericDAOHibernate<DatabaseSearchSequence>,
        IDatabaseSearchSequenceDAO
    {
        List<DatabaseSearchSequence> IDatabaseSearchSequenceDAO.FindByDatasetId(int datasetId, int lcmsFeatureId)
        {
            ICriterion criterionDataset = Restrictions.Eq("GroupId", datasetId);
            ICriterion criterionFeature = Restrictions.Eq("UmcFeatureId", lcmsFeatureId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterionDataset);
            criterionList.Add(criterionFeature);

            return FindByCriteria(criterionList);
        }


        List<DatabaseSearchSequence> IDatabaseSearchSequenceDAO.FindByDatasetId(int datasetId)
        {
            ICriterion criterionDataset = Restrictions.Eq("GroupId", datasetId);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterionDataset);

            return FindByCriteria(criterionList);
        }
    }
}