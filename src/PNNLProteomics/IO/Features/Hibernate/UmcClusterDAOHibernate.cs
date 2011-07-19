using System.Collections.Generic;
using MultiAlignEngine.Features;
using NHibernate.Criterion;

namespace MultiAlignCore.IO.Features.Hibernate
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
