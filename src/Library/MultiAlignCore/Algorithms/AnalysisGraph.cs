#region

using System.Collections.Generic;

#endregion

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Encapsulates the list of workflow items to execute.
    /// </summary>
    public class AnalysisGraph
    {
        private readonly List<AnalysisGraphNode> m_nodes;

        public AnalysisGraph()
        {
            m_nodes = new List<AnalysisGraphNode>();
        }

        /// <summary>
        /// Gets the list of nodes to execute.
        /// </summary>
        public List<AnalysisGraphNode> Nodes
        {
            get { return m_nodes; }
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
}