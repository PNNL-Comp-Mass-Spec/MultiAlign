using System.Collections.Generic;
using NHibernate.Criterion;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.SequenceData;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignCore.IO.SequenceData
{

	public class DatabaseSearchSequenceDAOHiberate : GenericDAOHibernate<DatabaseSearchSequence>, IDatabaseSearchSequenceDAO 
	{

        #region IDatabaseSearchSequenceDAO Members
        List<DatabaseSearchSequence> IDatabaseSearchSequenceDAO.FindByDatasetId(int datasetId)
        {
            ICriterion criterion                = Expression.Eq("GroupID", datasetId);
            List<ICriterion> criterionList      = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
        #endregion
    }

}
