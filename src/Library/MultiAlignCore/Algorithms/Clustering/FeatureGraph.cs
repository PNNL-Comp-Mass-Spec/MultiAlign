using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{

    /// <summary>
    /// Manages a graph of connected features based on some similarity metric.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class FeatureGraph<T>
        where T : FeatureLight, new()
    {
        /// <summary>
        /// Holds information about the graph.
        /// </summary>
        Dictionary<T, UniqueEdgeList<T>> m_edges;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FeatureGraph()
        {
            m_edges = new Dictionary<T, UniqueEdgeList<T>>();
        }
        public void RemoveEdgeFromVertex(T vertex, Edge<T> edge)
        {
            m_edges[vertex].RemoveEdge(edge);
        }
        /// <summary>
        /// Removes the edge using the vertices
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge(Edge<T> edge)
        {
            if (m_edges.ContainsKey(edge.VertexA))
                m_edges[edge.VertexA].RemoveEdge(edge);

            if (m_edges.ContainsKey(edge.VertexB))
                m_edges[edge.VertexB].RemoveEdge(edge);
        }

        /// <summary>
        /// Adds a vertex to the graph ensuring uniqueness, and does not throw an exception if attempting to add multiple
        /// references to the same instance.
        /// </summary>
        /// <param name="vertex">Vertex to add.</param>
        public void AddVertex(T vertex)
        {
            if (!m_edges.ContainsKey(vertex))
            {
                m_edges.Add(vertex, new UniqueEdgeList<T>());
            }
        }
        /// <summary>
        /// Adds the edge to the vertex list.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edge"></param>
        public void AddEdgeToVertex(T vertex, Edge<T> edge)
        {
            if (!m_edges.ContainsKey(vertex))
            {
                throw new Exception("The vertex supplied does not exist in the graph.");
            }

            // Make sure the vertex is part of this edge.
            var isOneOf = edge.VertexB == vertex;
            isOneOf = (isOneOf == (vertex == edge.VertexB));

            if (!isOneOf)
            {
                throw new Exception("The vertex supplied is not connected on this edge.");
            }

            m_edges[vertex].AddEdge(edge);
        }
        /// <summary>
        /// Adds the edge to the vertex list.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edge"></param>
        public void AddEdgeToVertex(T vertex, List<Edge<T>> edges)
        {
            edges.ForEach(x => AddEdgeToVertex(vertex, x));
        }
        public void ClearGraph()
        {
            m_edges.Clear();
        }
        /// <summary>
        /// Creates a graph based
        /// </summary>
        /// <param name="edges"></param>
        public void CreateGraph(List<Edge<T>> edges)
        {
            // Sort out the distances so we dont have to recalculate distances.

            foreach (var edge in edges)
            {
                AddVertex(edge.VertexA);
                AddVertex(edge.VertexB);

                AddEdgeToVertex(edge.VertexA, edge);
                AddEdgeToVertex(edge.VertexB, edge);
            }
        }

        /// <summary>
        /// Gets the list of edges for a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public UniqueEdgeList<T> GetAdjacentEdgesFromEdgeVertices(Edge<T> edge)
        {
            var newEdges = new UniqueEdgeList<T>();

            newEdges.AddEdges(m_edges[edge.VertexA].Edges);
            newEdges.AddEdges(m_edges[edge.VertexB].Edges);

            return newEdges;
        }
    }
}
