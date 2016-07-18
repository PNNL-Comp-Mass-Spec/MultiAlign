using System.Collections;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Holds only unique edges based on edge id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class UniqueEdgeList<T> : IEnumerable<Edge<T>>
        where T : FeatureLight, new()
    {
        /// <summary>
        /// Graph that encapsulates this
        /// </summary>
        private Dictionary<int, Edge<T>> m_edges;
        /// <summary>
        /// Gets the edges.
        /// </summary>
        //private readonly List<Edge<T>> m_edgeList;
        private readonly SortedSet<Edge<T>> m_edgeList;


        /// <summary>
        /// Constructor.
        /// </summary>
        public UniqueEdgeList()
        {
            //m_edgeList  = new List<Edge<T>>();
            m_edgeList  = new SortedSet<Edge<T>>();
            m_edges     = new Dictionary<int, Edge<T>>();
        }
        /// <summary>
        /// Gets
        /// </summary>
        public SortedSet<Edge<T>> Edges
        {
            get
            {
                return m_edgeList;
            }
        }
        /// <summary>
        /// Adds an edge between the feature
        /// </summary>
        /// <param name="featureY"></param>
        /// <param name="distance"></param>
        public void AddEdge(Edge<T> edge)
        {
            if (!m_edges.ContainsKey(edge.ID))
            {
                m_edges.Add(edge.ID, edge);
                m_edgeList.Add(edge);
            }
        }
        /// <summary>
        /// Adds an edge between the feature
        /// </summary>
        /// <param name="featureY"></param>
        /// <param name="distance"></param>
        public void AddEdges(SortedSet<Edge<T>> edges)
        {
            //edges.ForEach(x => AddEdge(x));
            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }
        /// <summary>
        /// Sorts the edges based on distance.
        /// </summary>
        public void Sort()
        {
            //m_edgeList.Sort(delegate(Edge<T> x,
            //                    Edge<T> y)
            //{
            //    return x.Length.CompareTo(y.Length);
            //});
        }
        /// <summary>
        /// Removes the edge from the list if it exists.
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge(Edge<T> edge)
        {
            if (m_edges.ContainsKey(edge.ID))
            {
                m_edges.Remove(edge.ID);
                m_edgeList.Remove(edge);
            }
        }
        /// <summary>
        /// Removes the edge from the list if it exists.
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdges(SortedSet<Edge<T>> edges)
        {
            foreach (var edge in edges)
            {
                //edges.ForEach(x => RemoveEdge(x));
                RemoveEdge(edge);
            }
        }
        /// <summary>
        /// Gets the number of items in the unique list.
        /// </summary>
        public int Count
        {
            get
            {
                return m_edges.Count;
            }
        }
        /// <summary>
        /// Clears the list of edges.
        /// </summary>
        public void Clear()
        {
            m_edges.Clear();
            m_edgeList.Clear();
        }

        public IEnumerator<Edge<T>> GetEnumerator()
        {
            return Edges.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
