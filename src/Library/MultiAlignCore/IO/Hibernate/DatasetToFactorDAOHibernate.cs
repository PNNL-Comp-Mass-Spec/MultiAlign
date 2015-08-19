#region

using MultiAlignCore.Data.Factors;
using MultiAlignCore.IO.Datasets;

#endregion

namespace MultiAlignCore.IO.Hibernate
{
    public class DatasetToFactorDAOHibernate : GenericDAOHibernate<DatasetToExperimentalFactorMap>,
        IDatasetToFactorMapDAO
    {
    }
}