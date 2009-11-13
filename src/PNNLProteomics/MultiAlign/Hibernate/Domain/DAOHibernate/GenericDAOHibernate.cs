/// <file>GenericDAOHibernate.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using NHibernate;
using Iesi.Collections.Generic;
using System.Runtime.Serialization;
using NHibernate.Criterion;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate
{

    /// <summary>
    /// A Generic class that contains methods that will be used to manipulate Objects in the Database using Hibernate.
    /// </summary>
    /// <typeparam name="T">The Object Type. e.g. Umc, MassTag, etc.</typeparam>
    public class GenericDAOHibernate<T> : IGenericDAO<T>
    {

        private Type m_persistentType;
        private ISession m_session = null;

        #region Constructor

        /// <summary>
        /// Constructor class that will store the Object Type to allow the use of Generics.
        ///     e.g. Umc if UmcDAOHibernate
        /// </summary>
        public GenericDAOHibernate()
        {
            m_persistentType = typeof(T);
        }

        #endregion

        #region Getter Methods

        /// <summary>
        /// Uses NHibernateHelper to create and open a Hibernate Session if one does not already exist.
        /// </summary>
        /// <returns>A configured Hibernate ISession</returns>
        protected ISession GetSession()
        {
            if (m_session == null)
            {
                m_session = NHibernateUtil.OpenSession();
            }
            return m_session;
        }

        /// <summary>
        /// A simple getter method to grab the Object Type.
        /// </summary>
        /// <returns>The Type of the Object. e.g. Umc if UmcDAOHibernate</returns>
        public Type GetPersistentType()
        {
            return m_persistentType;
        }

        #endregion

        /// These methods will be available to all of the DAOHibernate classes.
        /// They are generic methods so that T can be replaced with the class type (Umc, MassTag, etc.)
        ///     e.g. If a Umc object is given to "Add(T t)", a Umc object will be stored in the Database
        #region Hibernate Methods

        /// <summary>
        /// Adds an Object to the Database.
        /// </summary>
        /// <param name="t">Object to be added</param>
        public void Add(T t)
        {
            using (ISession session = GetSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(t);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Adds a Set of Objects to the Database.
        /// </summary>
        /// <param name="tSet">Set of Objects to be added</param>
        public void AddAll(Set<T> tSet)
        {
            using (ISession session = GetSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (T t in tSet)
                    {
                        session.Save(t);
                    }
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Update method will not save a new Object; it will only update the Object if it already exists in the Database.
        /// </summary>
        /// <param name="t">Object to be updated</param>
        public void Update(T t)
        {
            using (ISession session = GetSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(t);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Update method will not save a new Object; it will only update the Object if it already exists in the Database.
        /// </summary>
        /// <param name="tSet">Set of Objects to be updated</param>
        public void UpdateAll(Set<T> tSet)
        {
            using (ISession session = GetSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (T t in tSet)
                    {
                        session.Update(t);
                    }
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes an Object from the Database.
        /// </summary>
        /// <param name="t">Object to be deleted</param>
        public void Delete(T t)
        {
            using (ISession session = GetSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(t);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes a Set of Obejcts from the Database.
        /// </summary>
        /// <param name="tSet">Set of Objects to be deleted</param>
        public void DeleteAll(Set<T> tSet)
        {
            using (ISession session = GetSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (T t in tSet)
                    {
                        session.Delete(t);
                    }
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Finds an Object in the Database that matches the given ID value.
        /// </summary>
        /// <param name="id">The ID value to be searched for</param>
        /// <returns>The Object found</returns>
        public T FindById(int id)
        {
            return GetSession().Load<T>(id);
        }

        /// <summary>
        /// Returns all Objects in the Database that are of Type t.
        /// </summary>
        /// <returns>A set of Objects found</returns>
        public List<T> FindAll()
        {
            return FindByCriteria(null);
        }

        /// <summary>
        /// This method take in a List of Hibernate Criterion and executes all of them at once to return
        /// a list of Objects. This is a generic method, so the Object type returned will depend on which
        /// DAOHibernate class was used to call this method.
        /// 
        /// Criterion will limit what Objects will be returned from the Database. An example of a Criterion:
        ///     ICriterion criterion = Expression.Eq("Mass", mass);
        /// This Criterion will limit the Objects returned to Objects that equal the given mass.
        /// 
        /// To return a List of Objects that will contain every single Object of that type in the Database,
        /// simply pass in an empty list of Criterion.
        /// </summary>
        /// <param name="criterionList">A list of Hibernate ICriterion - refer to Hibernate docs</param>
        /// <returns>A List of T Objects</returns>
        protected List<T> FindByCriteria(List<ICriterion> criterionList)
        {
            ICriteria crit = GetSession().CreateCriteria(GetPersistentType());
            if (criterionList != null)
            {
                foreach (ICriterion c in criterionList)
                {
                    crit.Add(c);
                }
            }
            return (List<T>)crit.List<T>();
        }

        #endregion

    }

}
