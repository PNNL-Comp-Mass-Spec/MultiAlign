#region

using System.Collections.Generic;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.IO.Features
{
    public interface IMSFeatureDAO : IGenericDAO<MSFeatureLight>
    {
        /// <summary>
        ///     Finds MSMS Spectra stored in the database.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        List<MSFeatureLight> FindByDatasetId(int datasetId);

        /// <summary>
        ///     Finds a feature based on its dataset id and feature id.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        List<MSFeatureLight> FindByFeatureId(int datasetId, int featureId);
    }
}