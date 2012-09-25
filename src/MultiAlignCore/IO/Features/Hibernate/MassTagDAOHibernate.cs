using System.Collections.Generic;
using NHibernate.Criterion;
using PNNLOmics.Data.MassTags;


namespace MultiAlignCore.IO.Features.Hibernate
{
    /// <summary>
    /// Accesses mass tags from the underlying database.
    /// </summary>
	public class MassTagDAOHibernate : GenericDAOHibernate<MassTagLight>, IMassTagDAO
    {

        public List<MassTagLight> FindMassTags(List<int> ids)
        {
            ICriterion criterion            = Expression.In("ID", ids);
            List<ICriterion> criterionList  = new List<ICriterion>();

            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
    }
}
