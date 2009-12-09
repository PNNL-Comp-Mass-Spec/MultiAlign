/// <file>UmcClusterDAOHibernate.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using NHibernate;
using Iesi.Collections.Generic;
using NHibernate.Criterion;
using MultiAlignEngine.Features;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate
{

    public class UmcClusterDAOHibernate : GenericDAOHibernate<clsCluster>, IUmcClusterDAO
    {
		
        /// <summary>
        /// Searches for and returns a List of UmcCluster Objects in the Database that have the exact Mass given.
        /// </summary>
        /// <param name="mass">Mass value to be searched for</param>
        /// <returns>List of UmcCluster Objects</returns>
		public ICollection<clsCluster> FindByMass(double mass)
        {
            ICriterion criterion = Expression.Eq("Mass", mass);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            return FindByCriteria(criterionList);
        }
		
    }

}
