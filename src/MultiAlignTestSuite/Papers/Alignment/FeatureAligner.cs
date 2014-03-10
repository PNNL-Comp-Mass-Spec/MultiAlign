using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Options;
using MultiAlignTestSuite.Papers.Alignment.Data;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class FeatureAligner
    {        
        public AlignedFeatureData AlignFeatures(   Tuple<List<UMCLight>, List<MSFeatureLight>> baseline ,
                                                   Tuple<List<UMCLight>, List<MSFeatureLight>> alignee ,
                                                   SpectralOptions  options)
        {        
            
            // Use the default settings for now
            var analysisOptions             = new MultiAlignAnalysisOptions();
            var builder        = new AlgorithmBuilder();
            var provider      = builder.GetAlgorithmProvider(analysisOptions);

            // Then align the samples
            builder.BuildAligner(analysisOptions.AlignmentOptions);

            IFeatureAligner aligner         = provider.Aligner;
            aligner.AlignFeatures(  baseline.Item1,
                                    alignee.Item1,
                                    analysisOptions.AlignmentOptions);

            AlignedFeatureData data = new AlignedFeatureData();
            data.Baseline           = baseline;
            data.Alignee            = alignee;
            return data;
        }
    }
}
