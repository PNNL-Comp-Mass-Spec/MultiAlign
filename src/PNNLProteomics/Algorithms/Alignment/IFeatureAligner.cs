using System;
using System.Collections.Generic;
using System.Text;

using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.Alignment;

namespace PNNLProteomics.Algorithms
{
    public interface IFeatureAligner
    {
        classAlignmentData AlignFeatures(clsMassTagDB database, List<clsUMC> features, clsAlignmentOptions options);
        classAlignmentData AlignFeatures(List<clsUMC> baseline,  List<clsUMC> features, clsAlignmentOptions options);
        classAlignmentData AlignFeatures(clsMassTagDB massTagDatabase, clsClusterData clusters, clsAlignmentOptions options);
    }
}
