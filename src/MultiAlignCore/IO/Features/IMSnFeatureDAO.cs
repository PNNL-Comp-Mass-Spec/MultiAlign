using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    public interface IMSnFeatureDAO : IGenericDAO<MSSpectra>
	{
        /// <summary>
        /// Finds MSMS Spectra stored in the database.
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        List<MSSpectra> FindByDatasetId(int datasetId);
        List<MSSpectra> FindBySpectraId(List<int> spectraId);
        /// <summary>
        /// Gets the number of MS/MS spectra found in the experiment.
        /// </summary>
        /// <returns></returns>
        int GetMsMsCount();
	}
}