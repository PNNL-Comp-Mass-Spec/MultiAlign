//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MultiAlignCore.Algorithms.Alignment;
//using MultiAlignCore.IO.Features;
//using MultiAlignCore.Data.Alignment;
//using PNNLOmics.Data.Features;
//using MultiAlignCore.Data.MassTags;
//using PNNLOmics.Algorithms;
//using MultiAlignCore.Data;
//using PNNLOmics.Algorithms.Alignment;

//namespace MultiAlignCore.Algorithms.Workflow
//{
//    public class AlignmentWorkflow : WorkflowBase
//    {

//        public List<UMCLight> AlignDataset(
//                                            List<UMCLight> features,
//                                            List<UMCLight> baselineFeatures,
//                                            MassTagDatabase database,
//                                            AlignmentOptions options,
//                                            DriftTimeAlignmentOptions driftTimeOptions,
//                                            IAlignmentDAO alignmentCache,
//                                            IFeatureAligner aligner,
//                                            DatasetInformation datasetInfo,
//                                            DatasetInformation baselineInfo)
//        {
//            classAlignmentData alignmentData = null;

//            // align the data.
//            if (baselineFeatures != null)
//            {
//                UpdateStatus("Aligning " + datasetInfo.DatasetName + " to baseline.");
//                alignmentData = AlignFeatures(features, baselineFeatures, aligner, options);
//            }
//            else
//            {
//                UpdateStatus("Aligning " + datasetInfo.DatasetName + " to mass tag database.");
//                alignmentData = AlignFeatures(features, database, aligner, options);
//            }

//            if (alignmentData != null)
//            {
//                alignmentData.aligneeDataset = datasetInfo.DatasetName;
//                alignmentData.DatasetID = datasetInfo.DatasetId;

//                FeaturesAlignedEventArgs args = new FeaturesAlignedEventArgs(baselineInfo,
//                                                                                datasetInfo,
//                                                                                alignmentData);

//                args.AlignedFeatures = features;

//                // Execute any post-processing corrections.
//                if (driftTimeOptions.ShouldAlignDriftTimes)
//                {
//                    // Drift time alignment.
//                    KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>,
//                                    DriftTimeAlignmentResults<UMC, UMC>> pair = new KeyValuePair<DriftTimeAlignmentResults<UMC, UMC>,
//                                                                                                    DriftTimeAlignmentResults<UMC, UMC>>();

//                    DriftTimeAligner driftTimeAligner = new DriftTimeAligner();
//                    RegisterProgressNotifier(driftTimeAligner);
//                    pair = CorrectDriftTimes(features, baselineFeatures, options, driftTimeOptions, database, driftTimeAligner);
//                    DeRegisterProgressNotifier(driftTimeAligner);

//                    args.DriftTimeAlignmentData = pair.Key;
//                    args.OffsetDriftAlignmentData = pair.Value;
//                }

//                // Notify
//                if (FeaturesAligned != null)
//                {
//                    FeaturesAligned(this, args);
//                }

//                if (options.ShouldStoreAlignmentFunction)
//                {
//                    // Store
//                    alignmentCache.Add(alignmentData);
//                }
//            }

//            UpdateStatus("Updating cache with aligned features.");
//            return features;
//        }
//        private classAlignmentData AlignFeatures(List<UMCLight> features,
//                                                 List<UMCLight> baselineFeatures,
//                                                 IFeatureAligner aligner,
//                                                 AlignmentOptions options)
//        {
//            classAlignmentData alignmentData = null;

//            alignmentData = aligner.AlignFeatures(baselineFeatures,
//                                                    features,
//                                                    options);

//            return alignmentData;
//        }
//        private classAlignmentData AlignFeatures(List<UMCLight> features,
//                                                MassTagDatabase database,
//                                                IFeatureAligner aligner,
//                                                AlignmentOptions options)
//        {
//            classAlignmentData alignmentData = null;
//            alignmentData = aligner.AlignFeatures(database,
//                                                    features,
//                                                    options,
//                                                    false);

//            return alignmentData;
//        }
//    }
//}

