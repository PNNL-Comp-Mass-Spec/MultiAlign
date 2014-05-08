using System.Collections.Generic;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Class for baselined loaded features.
    /// </summary>
    public sealed class BaselineFeaturesLoadedEventArgs : FeaturesLoadedEventArgs
    {
        public BaselineFeaturesLoadedEventArgs(DatasetInformation info, List<UMCLight> features, MassTagDatabase database) :
            base(info, features)
        {
            Database = database;
        }
        public BaselineFeaturesLoadedEventArgs(DatasetInformation info, List<UMCLight> features) :
            base(info, features)
        {
            Database = null;
        }
        public MassTagDatabase Database
        {
            get;
            set;
        }
    }
}