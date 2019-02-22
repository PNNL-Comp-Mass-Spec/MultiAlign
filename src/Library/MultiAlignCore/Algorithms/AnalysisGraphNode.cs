#region

using System;

#endregion

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Class that encapsulates data about analysis steps.
    /// </summary>
    public class AnalysisGraphNode
    {
        /// <summary>
        /// Fired when the current flag is set or unset
        /// </summary>
        public event EventHandler StatusChanged;

        private bool m_isCurrent;

        public AnalysisGraphNode()
        {
            CurrentStep = AnalysisStep.None;
            Next = null;
            Method = null;
            IsCurrent = false;
        }

        /// <summary>
        /// Gets or sets the current step type.
        /// </summary>
        public AnalysisStep CurrentStep { get; set; }

        /// <summary>
        /// Gets or sets the next node to execute
        /// </summary>
        public AnalysisGraphNode Next { get; set; }

        /// <summary>
        /// Gets or sets the current method to call.
        /// </summary>
        public DelegateAnalysisMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the name of the analysis node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this node.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether the current node is being executed
        /// </summary>
        public bool IsCurrent
        {
            get { return m_isCurrent; }
            set
            {
                m_isCurrent = value;
                StatusChanged?.Invoke(this, null);
            }
        }
    }
}