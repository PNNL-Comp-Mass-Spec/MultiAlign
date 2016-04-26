namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;

    public interface IDatasetLoader : ISettingsContainer
    {
        List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null);
    }
}
