using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Data.Features;

namespace MultiAlignTestSuite.Papers.Alignment
{
    //public class FeatureLoader
    //{
    //    public Tuple<List<UMCLight>, List<MSFeatureLight>> LoadFeatures(string path)
    //    {
    //        DatasetInformation info = new DatasetInformation();
    //        info.Features       = new InputFile();
    //        info.Features.Path  = path;

    //        // Load the MS Feature Data
    //        List<MSFeatureLight> msFeatures = UMCLoaderFactory.LoadMsFeatureData(info, null);
    //        List<UMCLight> features         = new List<UMCLight>();

    //        // Find the features 
    //        IFeatureFinder finder               = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);
    //        var
    //        var options = new LcmsFeatureFindingOptions();
    //        features = finder.FindFeatures(msFeatures, options, null);

    //        return new Tuple<List<UMCLight>, List<MSFeatureLight>>(features, msFeatures);
    //    }
    //}
}
