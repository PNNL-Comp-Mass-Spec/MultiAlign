using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    public interface IMSFeatureToLCMSFeatureDAO : IGenericDAO<MSFeatureToLCMSFeatureMap>
	{
        /// <summary>
        /// Finds MSMS Spectra stored in the database.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        List<MSFeatureToLCMSFeatureMap> FindByDatasetId(int datasetId);
	}
}