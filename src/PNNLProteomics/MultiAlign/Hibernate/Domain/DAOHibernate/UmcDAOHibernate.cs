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

    public class UmcDAOHibernate : GenericDAOHibernate<clsUMC>, IUmcDAO
    {
		
        /// <summary>
        /// Searches for and returns a List of Umc Objects in the Database that have the exact Mass given.
        /// </summary>
        /// <param name="mass">Mass value to be searched for</param>
        /// <returns>List of Umc Objects</returns>
		public List<clsUMC> FindByMass(double mass)
        {
            ICriterion criterion = Expression.Eq("Mass", mass);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            
            return FindByCriteria(criterionList);
        }

        /// <summary>
        /// Searches for and returns a List of Umc Objects in the Database that have a Mass inside the given range.
        /// If the Lower Mass Value given is greater than the Upper Mass Value given, they are switched.
        /// </summary>
        /// <param name="mass1">Lower mass value</param>
        /// <param name="mass2">Upper mass value</param>
        /// <returns>List of Umc Objects</returns>
		public List<clsUMC> FindByMassRange(double mass1, double mass2)
        {
            ICriterion criterion;

            if (mass1 <= mass2)
            {
                criterion = Expression.Between("Mass", mass1, mass2);
            } else
            {
                criterion = Expression.Between("Mass", mass2, mass1);
            }

            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);

            return FindByCriteria(criterionList);
        }
        /// <summary>
        /// Finds a feature based on a feature id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public clsUMC FindByFeatureID(int id)
        {
            ICriterion criterion = Expression.Eq("Id", id);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            List<clsUMC> umcs = FindByCriteria(criterionList);
            if (umcs.Count < 1)
                return null;

            return umcs[0];
        }
        /// <summary>
        /// Finds a feature based on a feature id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public clsUMC FindByClusterID(int id)
        {
            ICriterion criterion = Expression.Eq("ClusterId", id);
            List<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(criterion);
            List<clsUMC> umcs = FindByCriteria(criterionList);
            if (umcs.Count < 1)
                return null;

            return umcs[0];
        }
		/// <summary>
		/// Searches for and returns a List of Umc Objects in the Database that have the exact Dataset Id given.
		/// </summary>
		/// <param name="mass">Dataset value to be searched for</param>
		/// <returns>List of Umc Objects</returns>
		public List<clsUMC> FindByDatasetId(int datasetId)
		{
			ICriterion criterion = Expression.Eq("DatasetId", datasetId);
			List<ICriterion> criterionList = new List<ICriterion>();
			criterionList.Add(criterion);
			return FindByCriteria(criterionList);
		}
    }

}
