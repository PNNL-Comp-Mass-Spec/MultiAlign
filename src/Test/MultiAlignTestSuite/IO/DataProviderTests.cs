using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using MultiAlignTestSuite.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignTestSuite.Algorithms;
using PNNLOmics.Data.Features;

namespace MultiAlignTestSuite
{
    /// <summary>
    /// Main application.
    /// </summary>
    public class DataProviderTest
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
        private static FeatureDataAccessProviders SetupDataProviders(string path)
        {
            return SetupDataProviders("test.db3", true);           
        }
        private static void CleanupDataProviders()
        {            
            NHibernateUtil.Dispose();
        }
        #endregion
                        
		static void Main(string[] args)
		{
            //MassTagDOATest massTagTest = new MassTagDOATest();
            SetupDataProviders(@"m:\data\proteomics\matest-gui\guitest.db3");
            MSFeatureDAOHibernate dao = new MSFeatureDAOHibernate();
            System.DateTime start = DateTime.Now;
            List<MSFeatureLight> features = dao.FindByDatasetId(0);            
            System.DateTime end = DateTime.Now;
            //msFeatureTest.LoadMSFeaturesFromCache(@"m:\data\proteomics\matest-gui\guitest.db3");
            TimeSpan span = end.Subtract(start);
            System.Console.WriteLine("{0} total ms", span.TotalMilliseconds);

            MSFeatureDOATest msFeatureTest = new MSFeatureDOATest();
            start = System.DateTime.Now;
            msFeatureTest.LoadMSFeaturesFromCache(@"m:\data\proteomics\matest-gui\guitest.db3");
            end = System.DateTime.Now;
            span = end.Subtract(start);
            System.Console.WriteLine("{0} total ms", span.TotalMilliseconds);
            //UMCFeatureFinding finding = new UMCFeatureFinding();
            //finding.Test();


            //return;

            //try
            //{
            //    string data = "-2147483648";
            //    long abu = Int64.Parse(data);

            //    UMCReaderFailure failTest = new UMCReaderFailure();
            //    failTest.Test();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            // SetupDataProviders();
            //MassTagDOATest massTagTest = new MassTagDOATest();
            //MSFeatureDOATest msFeatureTest = new MSFeatureDOATest();

            //msFeatureTest.LoadMSFeaturesFromCache(@"m:\data\proteomics\matest-gui\guitest.db3");

            //massTagTest.SaveMassTags();
            //msFeatureTest.SaveMSFeatures();

            //NHibernateUtil.CloseSession();
        }
    }
}
