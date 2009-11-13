/// <file>NHibernateUtil.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using PNNLProteomics.MultiAlign.Hibernate.Domain;
using NHibernate;
using NHibernate.Cfg;
using System.Data.SQLite;

namespace PNNLProteomics.MultiAlign.Hibernate
{

    /// <summary>
    /// Hibernate Session Factory that will create a session if one does not already exist.
    /// To create a session, Hibernate will need to be configured.
    /// In the case of this class, we are using a Hibernate Configuration file.
    /// </summary>
    public class NHibernateUtil
    {

        //Make sure that your hibernate.cfg.xml lives in this location
        private const string m_pathToHibernateConfig = "../../../PNNLProteomics/MultiAlign/Hibernate/hibernate.cfg.xml";

        private static ISessionFactory m_sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (m_sessionFactory == null)
                {
                    Configuration configuration = new Configuration();
                    configuration.Configure(m_pathToHibernateConfig);
                    m_sessionFactory = configuration.BuildSessionFactory();
                }
                return m_sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

    }

}