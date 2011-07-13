using System.Collections.Generic;
using MultiAlignEngine.MassTags;
using NHibernate.Criterion;
using PNNLOmics.Data;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate
{

	public class ProteinDAO : GenericDAOHibernate<Protein>, IProteinDAO
    {
        /// <summary>
        /// Searches for and returns a List of Protein Objects in the Database that have the exact Protein String given.
        /// </summary>
        /// <param name="proteinString">Protein String to be searched for</param>
        /// <returns>List of Protein Objects</returns>
		public ICollection<Protein> FindByProteinString(string proteinString)
        {
            ICriterion criterion            = Expression.Eq("ProteinString", proteinString);
            List<ICriterion> criterionList  = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

    }

}
