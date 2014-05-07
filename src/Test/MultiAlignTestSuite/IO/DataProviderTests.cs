using System;
using System.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignTestSuite.IO
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
            var dao = new MSFeatureDAOHibernate();
            var start = DateTime.Now;
            var features = dao.FindByDatasetId(0);            
            var end = DateTime.Now;
            //msFeatureTest.LoadMSFeaturesFromCache(@"m:\data\proteomics\matest-gui\guitest.db3");
            var span = end.Subtract(start);
            System.Console.WriteLine("{0} total ms", span.TotalMilliseconds);

            var msFeatureTest = new MSFeatureDOATest();
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
