/// <file>ProteinDAOHibernate.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using NHibernate;
using NHibernate.Criterion;
using MultiAlignEngine.MassTags;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate
{

	public class ProteinDAOHibernate : GenericDAOHibernate<clsProtein>, IProteinDAO
    {

        /// <summary>
        /// Searches for and returns a List of Protein Objects in the Database that have the exact Protein String given.
        /// </summary>
        /// <param name="proteinString">Protein String to be searched for</param>
        /// <returns>List of Protein Objects</returns>
		public ICollection<clsProtein> FindByProteinString(string proteinString)
        {
            ICriterion criterion = Expression.Eq("ProteinString", proteinString);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }

    }

}
