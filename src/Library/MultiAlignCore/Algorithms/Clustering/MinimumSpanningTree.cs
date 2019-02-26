using System;
using System.Collections;
using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{

    /// <summary>
    /// Encapsulates data about a minimum spanning tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MinimumSpanningTree<T> : IEnumerable<Edge<T>>
        where T : FeatureLight, new()
    {
        private Dictionary<int, Edge<T>> m_visitedEdges;
        private Dictionary<T, bool> m_visitedVertices;
        /// <summary>
        /// Constructor.
        /// </summary>
        public MinimumSpanningTree()
        {
            m_visitedEdges = new Dictionary<int, Edge<T>>();
            m_visitedVertices = new Dictionary<T, bool>();
            LinearRelationship = new List<Edge<T>>();
        }
        /// <summary>
        /// Adds the edge to the MST.
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(Edge<T> edge)
        {
            var hasBeenSeen = HasEdgeBeenSeen(edge);
            if (hasBeenSeen)
                throw new Exception("The edge has already been seen.");

            if (!m_visitedVertices.ContainsKey(edge.VertexA))
                m_visitedVertices.Add(edge.VertexA, true);

            if (!m_visitedVertices.ContainsKey(edge.VertexB))
                m_visitedVertices.Add(edge.VertexB, true);

            m_visitedEdges.Add(edge.ID, edge);
            LinearRelationship.Add(edge);
        }

        /// <summary>
        /// Determines if the edge is part of the MST
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool HasEdgeBeenSeen(Edge<T> edge)
        {
            return m_visitedEdges.ContainsKey(edge.ID);
        }
        /// <summary>
        /// Determines if the edge vertices have been used as part of adjacent edges.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool HasEdgeVerticesBeenSeen(Edge<T> edge)
        {
            // See if the vertices have been seen too...if they both have
            // then the valid flag should be true.
            var beenSeen = m_visitedVertices.ContainsKey(edge.VertexA);
            beenSeen = (beenSeen && m_visitedVertices.ContainsKey(edge.VertexB));

            return beenSeen;
        }
        /// <summary>
        /// Determines the order in which a MST is constructed.
        /// </summary>
        public List<Edge<T>> LinearRelationship
        {
            get;
            private set;
        }

        public IEnumerator<Edge<T>> GetEnumerator()
        {
            return LinearRelationship.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
