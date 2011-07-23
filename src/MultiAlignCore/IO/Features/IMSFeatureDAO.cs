using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features
{
    public interface IMSFeatureDAO : IGenericDAO<MSFeatureLight>
	{
        /// <summary>
        /// Finds MSMS Spectra stored in the database.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        List<MSFeatureLight> FindByDatasetId(int datasetId);
	}
}