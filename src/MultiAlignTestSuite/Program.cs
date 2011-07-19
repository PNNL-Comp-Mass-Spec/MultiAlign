using System.IO;
using PNNLProteomics.IO;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

namespace MultiAlignTestSuite
{
    /// <summary>
    /// Main application.
    /// </summary>
    class Program
    {                
        #region Data Provider Setup
        /// <summary>
        /// Sets up the NHibernate caches for storing and retrieving data.
        /// </summary>
        /// <param name="analysisPath"></param>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders(string path, bool createNew)
        {
            try
            {
                bool exists = File.Exists(path);
                if (exists && createNew)
                {
                    File.Delete(path);
                }

                NHibernateUtil.ConnectToDatabase(path, createNew);
                IUmcDAO featureCache            = new UmcDAOHibernate();
                IUmcClusterDAO clusterCache     = new UmcClusterDAOHibernate();
                IMSFeatureDAO msFeatureCache    = new MSFeatureDAOHibernate();

                FeatureDataAccessProviders providers = new FeatureDataAccessProviders(  featureCache, 
                                                                                        clusterCache, 
                                                                                        msFeatureCache);
                
                return providers;
            }
            catch (IOException ex)
            {                
                throw ex;
            }
        }
        /// <summary>
        /// Creates data providers to the database of the analysis name and path provided.
        /// </summary>
        /// <returns></returns>
        private static FeatureDataAccessProviders SetupDataProviders()
        {
            return SetupDataProviders("test.db3", true);           
        }
        private static void CleanupDataProviders()
        {            
            NHibernateUtil.Dispose();
        }
        #endregion
                        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
		static void Main(string[] args)
		{
            SetupDataProviders();
            MassTagDOATest   massTagTest    = new MassTagDOATest();
            MSFeatureDOATest msFeatureTest  = new MSFeatureDOATest();
            massTagTest.SaveMassTags();
            msFeatureTest.SaveMSFeatures();

            NHibernateUtil.CloseSession();
        }
    }
}
