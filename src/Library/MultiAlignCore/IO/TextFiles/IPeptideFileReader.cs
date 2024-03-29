﻿using System.Collections.Generic;
using FeatureAlignment.Data.MassTags;

namespace MultiAlignCore.IO.TextFiles
{
    /// <summary>
    /// Interface for files that contain peptides or identifications
    /// </summary>
    public interface ISequenceFileReader
    {
        IEnumerable<Peptide> Read(string path);
    }
}
