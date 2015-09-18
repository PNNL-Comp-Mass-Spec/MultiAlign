#region

using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Utils;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode.Impl;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    /// <summary>
    ///     A Generic class that contains methods that will be used to manipulate Objects in the Database using Hibernate.
    /// </summary>
    /// <typeparam name="T">The Object Type. e.g. Umc, MassTag, etc.</typeparam>
    public class GenericDAOHibernate<T> : IGenericDAO<T>
    {
        private readonly Type m_persistentType;
        private ISession m_session;

        #region Constructor

        /// <summary>
        ///     Constructor class that will store the Object Type to allow the use of Generics.
        ///     e.g. Umc if UmcDAOHibernate
        /// </summary>
        public GenericDAOHibernate()
        {
            m_persistentType = typeof (T);
            SessionBatchSize = 2;
        }

        #endregion

        #region Member Methods

        /// <summary>
        ///     Uses NHibernateUtil to create and open a Hibernate Session if one does not already exist.
        /// </summary>
        /// <returns>A configured Hibernate ISession</returns>
        protected ISession GetSession()
        {
            if (m_session == null || !m_session.IsOpen)
            {
                m_session = NHibernateUtil.OpenSession();
            }
            return m_session;
        }

        /// <summary>
        ///     Uses NHibernateUtil to create and open a Hibernate Session if one does not already exist.
        /// </summary>
        /// <returns>A configured Hibernate ISession</returns>
        protected IStatelessSession GetStatelessSession()
        {
            return NHibernateUtil.OpenStatelessSession();
        }

        /// <summary>
        ///     A simple getter method to grab the Object Type.
        /// </summary>
        /// <returns>The Type of the Object. e.g. Umc if UmcDAOHibernate</returns>
        public Type GetPersistentType()
        {
            return m_persistentType;
        }

        #endregion

        #region Hibernate Methods

        /// These methods will be available to all of the DAOHibernate classes.
        /// They are generic methods so that T can be replaced with the class type (Umc, MassTag, etc.)
        /// e.g. If a Umc object is given to "Add(T t)", a Umc object will be stored in the Database
        /// <summary>
        ///     Adds an Object to the Database.
        /// </summary>
        /// <param name="t">Object to be added</param>
        public void Add(T t)
        {
            using (var session = GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(t);
                    transaction.Commit();
                }
            }
        }

        public int SessionBatchSize { get; set; }

        /// <summary>
        ///     Adds a Collection of Objects to the Database.
        ///     Not good for very large bulk inserts.
        /// </summary>
        /// <param name="tCollection">Collection of Objects to be added</param>
        public virtual void AddAll(ICollection<T> tCollection, IProgress<ProgressData> progress = null)
        {
            var progressStep = (int)Math.Ceiling(0.01 * tCollection.Count);
            using (var session = GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.CreateSQLQuery("PRAGMA defer_foreign_keys = ON").ExecuteUpdate();
                    session.CreateSQLQuery("PRAGMA ignore_check_constraints = ON").ExecuteUpdate();
                    var progressData = new ProgressData(progress) { IsPartialRange = true, MaxPercentage = 95 };
                    int i = 0;
                    foreach (var t in tCollection)
                    {
                        session.SaveOrUpdate(t); //If we don't want to keep the unaligned features
                        if ((i > 0 && i % progressStep == 0) || i == tCollection.Count - 1)
                        {
                            progressData.Report(i, tCollection.Count);
                        }

                        i++;
                    }

                    session.CreateSQLQuery("PRAGMA ignore_check_constraints = OFF").ExecuteUpdate();
                    progressData.StepRange(100);
                    transaction.Commit();
                    progressData.Report(100);
                }
            }
        }

        /// <summary>
        ///     Adds a Collection of Objects to the Database.
        /// </summary>
        /// <param name="tCollection">Collection of Objects to be added</param>
        public virtual void AddAllStateless(ICollection<T> tCollection, IProgress<ProgressData> progress = null)
        {
            var progressStep = (int)Math.Ceiling(0.01 * tCollection.Count);
            using (var session = GetStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var progressData = new ProgressData(progress) { IsPartialRange = true, MaxPercentage = 95 };
                    int i = 0;
                    session.CreateSQLQuery("PRAGMA defer_foreign_keys = ON").ExecuteUpdate();
                    session.CreateSQLQuery("PRAGMA ignore_check_constraints = ON").ExecuteUpdate();
                    foreach (var t in tCollection)
                    {
                        session.Insert(t); //If we don't want to keep the unaligned features

                        if ((i > 0 && i % progressStep == 0) || i == tCollection.Count - 1)
                        {
                            progressData.Report(i, tCollection.Count);
                        }

                        i++;
                    }
                    session.CreateSQLQuery("PRAGMA ignore_check_constraints = OFF").ExecuteUpdate();
                    progressData.StepRange(100);
                    transaction.Commit();
                    progressData.Report(100);
                }
            }
        }

        /// <summary>
        ///     Update method will not save a new Object; it will only update the Object if it already exists in the Database.
        /// </summary>
        /// <param name="t">Object to be updated</param>
        public void Update(T t)
        {
            using (var session = GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(t);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Update method will not save a new Object; it will only update the Object if it already exists in the Database.
        /// </summary>
        /// <param name="tCollection">Collection of Objects to be updated</param>
        public void UpdateAll(ICollection<T> tCollection, IProgress<ProgressData> progress = null)
        {
            var progressStep = (int)Math.Ceiling(0.01 * tCollection.Count);
            using (var session = GetStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var progressData = new ProgressData(progress) { IsPartialRange = true, MaxPercentage = 95 };
                    int i = 0;
                    foreach (var t in tCollection)
                    {
                        session.Update(t);

                        if ((i > 0 && i % progressStep == 0) || i == tCollection.Count - 1)
                        {
                            progressData.Report(i, tCollection.Count);
                        }

                        i++;
                    }

                    progressData.StepRange(100);
                    transaction.Commit();
                    progressData.Report(100);
                }
            }
        }

        /// <summary>
        ///     Deletes an Object from the Database.
        /// </summary>
        /// <param name="t">Object to be deleted</param>
        public void Delete(T t)
        {
            using (var session = GetSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(t);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Deletes a Collection of Objects from the Database.
        /// </summary>
        /// <param name="tCollection">Collection of Objects to be deleted</param>
        public void DeleteAll(ICollection<T> tCollection)
        {
            using (var session = GetStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach (var t in tCollection)
                    {
                        session.Delete(t);
                    }
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        ///     Finds an Object in the Database that matches the given ID value.
        /// </summary>
        /// <param name="id">The ID value to be searched for</param>
        /// <returns>The Object found</returns>
        public T FindById(int id)
        {
            return GetSession().Load<T>(id);
        }

        /// <summary>
        ///     Returns all Objects in the Database that are of Type t.
        /// </summary>
        /// <returns>A set of Objects found</returns>
        public List<T> FindAll()
        {
            return FindByCriteria(null);
        }

        /// <summary>
        ///     This method take in a List of Hibernate Criterion and executes all of them at once to return
        ///     a list of Objects. This is a generic method, so the Object type returned will depend on which
        ///     DAOHibernate class was used to call this method.
        ///     Criterion will limit what Objects will be returned from the Database. An example of a Criterion:
        ///     ICriterion criterion = Expression.Eq("Mass", mass);
        ///     This Criterion will limit the Objects returned to Objects that equal the given mass.
        ///     To return a List of Objects that will contain every single Object of that type in the Database,
        ///     simply pass in an empty list of Criterion.
        /// </summary>
        /// <param name="criterionList">A list of Hibernate ICriterion - refer to Hibernate docs</param>
        /// <returns>A List of T Objects</returns>
        protected List<T> FindByCriteria(List<ICriterion> criterionList)
        {   
            List<T> list = null;
            using (var session = GetSession())
            {
                var crit = session.CreateCriteria(GetPersistentType());
                if (criterionList != null)
                {
                    foreach (var c in criterionList)
                    {
                        crit.Add(c);
                    }
                }
                list = (List<T>) crit.List<T>();
            }
            return list;
        }

        protected void DeleteAllFromTable(string tableName)
        {
            using (var session = GetStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var query = session.CreateSQLQuery(string.Format("DELETE FROM {0}", tableName));
                    query.ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        protected void DeleteByCriteria(string tableName, string keyName, int value)
        {
            using (var session = GetStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var query = session.CreateSQLQuery(string.Format("DELETE FROM {0} WHERE {1} = {2}", tableName, keyName, value));
                    query.ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        #endregion
    }
}