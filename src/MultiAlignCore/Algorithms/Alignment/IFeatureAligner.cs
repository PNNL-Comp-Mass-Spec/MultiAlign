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
        /// <param name="massTagDatabase">Database to align to.</param>
        /// <param name="features">Features to align.</param>
        /// <param name="options">Options to use.</param>
        /// <returns>Synopsis of alignment information</returns>
        classAlignmentData AlignFeatures(   MassTagDatabase     database,      
                                            List<UMCLight>      features,  
                                            AlignmentOptions    options,
                                            bool                alignDriftTimes);
        /// <summary>
        /// Aligns a set of features to another set of features.
        /// </summary>
        /// <param name="baseline">Features to align to. (baseline)</param>
        /// <param name="features">Features to align.  (alignee)</param>
        /// <param name="options">Options to use.</param>
        /// <returns>Synopsis of alignment information</returns>
        classAlignmentData AlignFeatures(   List<UMCLight>      baseline,
                                            List<UMCLight>      features,  
                                            AlignmentOptions    options);
        /// <summary>
        /// Aligns clusters to a mass tag database.
        /// </summary>
        /// <param name="massTagDatabase">Database to align to.</param>
        /// <param name="clusters">Clusters to align.</param>
        /// <param name="options">Options to use.</param>
        /// <returns>Synopsis of alignment information</returns>
        classAlignmentData AlignFeatures(   MassTagDatabase     massTagDatabase,  
                                            List<clsCluster>    clusters,
                                            AlignmentOptions    options);        
    }
}