using System.Collections.Generic;
using MultiAlignEngine.Features;
using NHibernate.Criterion;
using PNNLOmics.Data;


namespace MultiAlignCore.IO.Features.Hibernate
{
    public class MSnFeatureDAOHibernate : GenericDAOHibernate<MSSpectra>, IMSnFeatureDAO
    {

        /// <summary>
        /// Searches for and returns a List of MS Spectra Objects in the Database that have the exact Dataset Id given.
        /// </summary>
        /// <param name="mass">Dataset value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
        public List<MSSpectra> FindByDatasetId(int datasetId)
        {
            ICriterion criterion = Expression.Eq("GroupID", datasetId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

        public List<MSSpectra> FindBySpectraId(List<int> spectraId)
        {            
            ICriterion criterion = Expression.In("ID", spectraId);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
    }
}
