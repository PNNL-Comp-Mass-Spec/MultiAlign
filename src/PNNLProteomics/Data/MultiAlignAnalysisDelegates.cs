using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Data.Features;
using PNNLProteomics.Data;
using PNNLProteomics.Data.Alignment;

namespace PNNLProteomics.MultiAlign
{
    /// <summary>
    /// Event arguments when two datasets are aligned.
    /// </summary>
    public class FeaturesAlignedEventArgs : EventArgs
    {
        private DatasetInformation m_baselinDatasetInformation;
        private DatasetInformation m_datasetInformation;
        private classAlignmentData  m_alignmentData;

        /// <summary>
        /// Arguments that hold alignment information when a dataset is aligned.
        /// </summary>
        /// <param name="function">Alignment function for this part of the alignment.</param>
        /// <param name="info">Dataset information object</param>
        /// <param name="mScores">Heat scores</param>
        /// <param name="minX">Min. X - scan baseline</param>
        /// <param name="maxX">Max. X - scan baseline</param>
        /// <param name="minY">Min. Y - scan alignee</param>
        /// <param name="maxY">Max. Y - scan alignee</param>
        /// <param name="part">If > -1, The alignment was split over a given m/z range., if -1, then alignment was performed over full m/z range.</param>
        public FeaturesAlignedEventArgs(DatasetInformation                  baselineDatasetInfo,
                                        DatasetInformation                  datasetInfo,
                                        classAlignmentData                  alignmentData,
                                        DriftTimeAlignmentResults<UMC, UMC> driftAlignmentData,
                                        DriftTimeAlignmentResults<UMC, UMC> offsetDriftAlignmentData)
            
        {
            m_datasetInformation        = datasetInfo;
            m_baselinDatasetInformation = datasetInfo;
            m_alignmentData             = alignmentData;
            DriftTimeAlignmentData      = driftAlignmentData;
            OffsetDriftAlignmentData    = offsetDriftAlignmentData;
        }

        /// <summary>
        /// Gets the dataset information for the alignee dataset.
        /// </summary>
        public DatasetInformation AligneeDatasetInformation
        {
            get
            {
                return m_datasetInformation;
            }
        }
        /// <summary>
        /// Gets the dataset information for the baseline dataset.
        /// </summary>
        public DatasetInformation BaselineDatasetInformation
        {
            get
            {
                return m_baselinDatasetInformation;
            }
        }
        /// <summary>
        /// Gets the alignment data associated between baseline and alignee.
        /// </summary>
        public classAlignmentData AlignmentData
        {
            get
            {
                return m_alignmentData;
            }
        }
        /// <summary>
        /// Gets the drift time alignment data.
        /// </summary>
        public DriftTimeAlignmentResults<UMC, UMC> DriftTimeAlignmentData
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the drift time alignment data.
        /// </summary>
        public DriftTimeAlignmentResults<UMC, UMC> OffsetDriftAlignmentData
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Error class for alignment.
    /// </summary>
    public class AnalysisErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        public AnalysisErrorEventArgs(string error, Exception ex)
        {
            Exception    = ex;
            ErrorMessage = error;   
        }

        public string ErrorMessage
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
    }
    public class AnalysisCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        public AnalysisCompleteEventArgs(MultiAlignAnalysis analysis)
        {
            Analysis = analysis;
        }
        /// <summary>
        /// Gets or sets the analysis.
        /// </summary>
        public MultiAlignAnalysis Analysis
        { 
            get; 
            private set;
        }
    }
    public class FeaturesLoadedEventArgs : EventArgs
    {
        private DatasetInformation   m_datasetInformation;
        private List<clsUMC>         m_features;
        
        /// <summary>
        /// Arguments that hold dataset information when features are loaded.
        /// </summary>        
        /// <param name="info">Dataset information object</param>        
        public FeaturesLoadedEventArgs( DatasetInformation      info,
                                        List<clsUMC>            features)                                       
        {            
            m_datasetInformation    = info;
            m_features              = features;
        }
        /// <summary>
        /// Gets the dataset information.
        /// </summary>
        public DatasetInformation DatasetInformation
        {
            get
            {
                return m_datasetInformation;
            }
        }
        /// <summary>
        /// Gets the list of features found.
        /// </summary>
        public List<clsUMC> Features
        {
            get
            {
                return m_features;
            }
        }
    }
    public class FeaturesPeakMatchedEventArgs : EventArgs
    {
    }
    public class FeaturesClusteredEventArgs : EventArgs
    {
    }

    public class AnalysisStatusEventArgs : EventArgs
    {
        private string m_message;
        private int    m_percentComplete;

        public AnalysisStatusEventArgs(string message, int percent)
        {
            m_message           = message;
            m_percentComplete   = percent;
        }

        public int PercentComplete
        {
            get
            {
                return m_percentComplete;
            }
        }
        public string StatusMessage
        {
            get
            {
                return m_message;
            }
        }
    }
}
