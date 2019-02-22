#region

using System.Collections.Generic;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Proteins;
using NHibernate.Criterion;

#endregion

namespace MultiAlignCore.IO.Hibernate
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
            ICriterion criterion = Restrictions.Eq("ProteinString", proteinString);
            var criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
    }
}