#region

using System;
using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;

#endregion

namespace MultiAlignCore.Data
{
    public class FeaturesLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Arguments that hold dataset information when features are loaded.
        /// </summary>
        /// <param name="info">Dataset information object</param>
        public FeaturesLoadedEventArgs(DatasetInformation info,
            IList<UMCLight> features)
        {
            DatasetInformation = info;
            Features = features;
        }

        /// <summary>
        /// Gets the dataset information.
        /// </summary>
        public DatasetInformation DatasetInformation { get; private set; }

        /// <summary>
        /// Gets the list of features found.
        /// </summary>
        public IList<UMCLight> Features { get; private set; }
    }
}