using System;
using System.Collections.Generic;
using System.Text;

using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Utilities;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Defines interface for alignment algorithms.
    /// </summary>
    public interface IFeatureAligner : IProgressNotifer
    {
        /// <summary>
        /// Aligns a set of features (from a dataset) to a mass tag database.
        /// </summary>
        classAlignmentData AlignFeatures(   MassTagDatabase     database,      
                                            List<UMCLight>      features,  
                                            AlignmentOptions    options,
                                            bool                alignDriftTimes);
        /// <summary>
        /// Aligns a set of features to another set of features.
        /// </summary>
        classAlignmentData AlignFeatures(   List<UMCLight>      baseline,
                                            List<UMCLight>      features,  
                                            AlignmentOptions    options);               
    }

}
