namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;

    public interface IDatasetLoader
    {
        List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null);

        void ResetDefaults();
    }
}
