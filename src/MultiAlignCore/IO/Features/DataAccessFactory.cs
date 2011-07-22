using System;
using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignCore.IO.Features
{
    public class DataAccessFactory
    {

        public static FeatureDataAccessProviders CreateDataAccessProviders(MultiAlignAnalysis analysis, bool deleteIfExists)
        {
            string path = AnalysisPathUtils.BuildAnalysisName(analysis.MetaData.AnalysisPath, analysis.MetaData.AnalysisName);
            return CreateDataAccessProviders(path, deleteIfExists);
        }

        public static FeatureDataAccessProviders CreateDataAccessProviders(string path, bool deleteIfExists)
        {
            if (deleteIfExists)
            {
                bool exists = File.Exists(path);
                if (exists)
                {
                    File.Delete(path);
                }
            }
            try
            {
                NHibernateUtil.ConnectToDatabase(path, true);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            IUmcDAO featureCache                                = new UmcDAOHibernate();
            IUmcClusterDAO clusterCache                         = new UmcClusterDAOHibernate();
            IMSFeatureDAO msFeatureCache                        = new MSFeatureDAOHibernate();
            IMSnFeatureDAO msnFeatureCache                      = new MSnFeatureDAOHibernate();
            IGenericDAO<MSFeatureToLCMSFeatureMap> msFeatureMap = new GenericDAOHibernate<MSFeatureToLCMSFeatureMap>();
            IGenericDAO<MSFeatureToMSnFeatureMap> msnFeatureMap = new GenericDAOHibernate<MSFeatureToMSnFeatureMap>();

            FeatureDataAccessProviders providers =
                                            new FeatureDataAccessProviders( featureCache, 
                                                                            clusterCache,
                                                                            msFeatureCache,
                                                                            msnFeatureCache,
                                                                            msFeatureMap,
                                                                            msnFeatureMap);

            return providers;
        }

        public static void CleanupDataProviders()
        {
            NHibernateUtil.Dispose();
        }
    }
}
