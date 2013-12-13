﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Papers.Alignment.SSM;
using MultiAlignTestSuite.Papers.Alignment.Data;
using MultiAlignCore.Data;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Alignment;
using PNNLOmics.Data.Features;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class FeatureAligner
    {        
        public AlignedFeatureData AlignFeatures(   Tuple<List<UMCLight>, List<MSFeatureLight>> baseline ,
                                                   Tuple<List<UMCLight>, List<MSFeatureLight>> alignee ,
                                                   SpectralOptions  options)
        {        
            
            // Use the default settings for now
            AnalysisOptions analysisOptions = new AnalysisOptions();
            AlgorithmBuilder builder        = new AlgorithmBuilder();
            AlgorithmProvider provider      = builder.GetAlgorithmProvider(analysisOptions);

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
