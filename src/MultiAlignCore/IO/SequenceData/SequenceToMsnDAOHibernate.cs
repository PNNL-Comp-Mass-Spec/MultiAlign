using System.Collections.Generic;
using NHibernate.Criterion;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.SequenceData;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignCore.IO.SequenceData
{

    public class SequenceToMsnDAOHibernate : GenericDAOHibernate<SequenceToMsnFeature>, ISequenceToMsnFeatureDAO 
	{        
        public List<SequenceToMsnFeature> FindByDatasetId(int datasetId, int lcmsFeatureId)
        {
            ICriterion criterionDataset = Expression.Eq("DatasetId", datasetId);
            ICriterion criterionFeature = Expression.Eq("UmcFeatureId", lcmsFeatureId);
            List<ICriterion> criterionList  = new List<ICriterion>();
            criterionList.Add(criterionDataset);
            criterionList.Add(criterionFeature);

            return FindByCriteria(criterionList);                
        } 
    }

}
