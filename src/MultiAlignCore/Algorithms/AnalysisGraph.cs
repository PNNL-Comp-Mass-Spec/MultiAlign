using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms
{
    public class AnalysisGraph
    {
        List<AnalysisGraphNode> m_nodes;

        public AnalysisGraph()
        {
            m_nodes = new List<AnalysisGraphNode>();
        }

        /// <summary>
        /// Gets the list of nodes to execute.
        /// </summary>
        public List<AnalysisGraphNode> Nodes
        {
            get
            {
                return m_nodes;
            }
        }
        /// <summary>
        /// Add a new node to the list of those to execute.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(AnalysisGraphNode node)
        {
            if (m_nodes.Count < 1)
                m_nodes.Add(node);
            else
            {
                m_nodes[m_nodes.Count - 1].Next = node;
                m_nodes.Add(node);
            }
        }
        public void Clear()
        {
            m_nodes.Clear();
        }
    }

    /// <summary>
    /// Next analysis method step to run.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public delegate void DelegateAnalysisMethod(AnalysisConfig config);

    /// <summary>
    /// Class that encapsulates data about analysis steps.
    /// </summary>
    public class AnalysisGraphNode
    {
        public AnalysisGraphNode()
        {
            CurrentStep = AnalysisStep.None;
            Next    = null;
            Method  = null;
        }
        /// <summary>
        /// Gets or sets the current step type.
        /// </summary>
        public AnalysisStep CurrentStep
        {
            get;
            set;
        }
        /// <summary>
        /// Next Method to call.
        /// </summary>
        public AnalysisGraphNode Next
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the current method to call.
        /// </summary>
        public DelegateAnalysisMethod Method
        {
            get;
            set;
        }
    }
}
