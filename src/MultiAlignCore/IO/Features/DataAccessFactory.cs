using System.IO;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignCore.IO.Features
{
    public class DataAccessFactory
    {

        public static FeatureDataAccessProviders CreateDataAccessProviders(MultiAlignAnalysis analysis)
        {
            string path = AnalysisPathUtils.BuildAnalysisName(analysis.MetaData.AnalysisPath, analysis.MetaData.AnalysisName);

            bool exists = File.Exists(path);
            if (exists)
            {
                File.Delete(path);
            }
            NHibernateUtil.ConnectToDatabase(path, true);
            IUmcDAO featureCache            = new UmcDAOHibernate();
            IUmcClusterDAO clusterCache     = new UmcClusterDAOHibernate();
            IMSFeatureDAO msFeatureCache    = new MSFeatureDAOHibernate();

            FeatureDataAccessProviders providers =
                new FeatureDataAccessProviders( featureCache, 
                                                clusterCache,
                                                msFeatureCache);

            return providers;
        }

        public static void CleanupDataProviders()
        {
            NHibernateUtil.Dispose();
        }
    }
}
