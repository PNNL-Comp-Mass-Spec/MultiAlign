
using PNNLProteomics.Data;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;

namespace PNNLProteomics.IO
{
    public class DataAccessFactory
    {

        public static FeatureDataAccessProviders CreateDataAccessProviders(MultiAlignAnalysis analysis)
        {
            string path = AnalysisPathUtils.BuildAnalysisName(analysis.AnalysisPath, analysis.AnalysisName);

            NHibernateUtil.SetDbLocationForWrite(path, true);
            NHibernateUtil.SetDbLocationForRead(path);

            IUmcDAO featureCache        = new UmcDAOHibernate();
            IUmcClusterDAO clusterCache = new UmcClusterDAOHibernate();

            FeatureDataAccessProviders providers =
                new FeatureDataAccessProviders(featureCache, clusterCache);

            return providers;
        }

        public static void CleanupDataProviders()
        {
            NHibernateUtil.Dispose();
        }
    }
}
