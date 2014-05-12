#region

using MultiAlignCore.Data.Factors;

#endregion

namespace MultiAlignCore.IO.Features.Hibernate
{
    public class DatasetToFactorDAOHibernate : GenericDAOHibernate<DatasetToExperimentalFactorMap>,
        IDatasetToFactorMapDAO
    {
    }
}