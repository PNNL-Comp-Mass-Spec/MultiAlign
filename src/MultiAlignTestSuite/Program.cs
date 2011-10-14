using System;
using System.Globalization;
using System.IO;
using MultiAlignTestSuite.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignTestSuite.Algorithms;

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
                return DataAccessFactory.CreateDataAccessProviders(path, createNew);                
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
            UMCFeatureFinding finding = new UMCFeatureFinding();
            finding.Test();


            return;

            try
            {                
                string data = "-2147483648";
                long abu    = Int64.Parse(data);

                UMCReaderFailure failTest = new UMCReaderFailure();
                failTest.Test();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            SetupDataProviders();
            MassTagDOATest   massTagTest    = new MassTagDOATest();
            MSFeatureDOATest msFeatureTest  = new MSFeatureDOATest();
            massTagTest.SaveMassTags();
            msFeatureTest.SaveMSFeatures();

            NHibernateUtil.CloseSession();
        }
    }
}
