﻿using System;

namespace FeatureAlignment.Algorithms.Alignment
{
    public interface IFeatureAligner<in TBaselineFeature, in TAligneeFeature, out TMatchData>: IProgressNotifer
    {
        /// <summary>
        /// Aligns the alignee features to the baseline features
        /// </summary>
        /// <param name="baseline">Typically an IEnumerable of a data type T (UMCLight) or an instance of a MassTagDatabase used as the alignment reference.</param>
        /// <param name="alignee">Typically an IEnumerable of a data type T that are aligned to a reference.</param>
        /// <param name="progress">Progress object</param>
        /// <returns></returns>
        TMatchData Align(TBaselineFeature baseline, TAligneeFeature alignee, IProgress<PRISM.ProgressData> progress = null);
    }
}
