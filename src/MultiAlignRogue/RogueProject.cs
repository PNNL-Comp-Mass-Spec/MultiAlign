using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignRogue
{
    public class RogueProject
    {
        /// <summary>
        /// Gets or sets the MultiAlign analysis options.
        /// </summary>
        public MultiAlignAnalysisOptions MultiAlignAnalysisOptions { get; set; }

        /// <summary>
        /// Gets or sets the list of datasets.
        /// </summary>
        public List<DatasetInformation> Datasets { get; set; } 
    }
}
