using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Node / tree structure for creating UMC's
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FeatureTree<T, U>:
        BinarySearchTree<T, FeatureTree<T, U>>
        where T: FeatureLight, new()
        where U: FeatureLight, Data.Features.IFeatureCluster<T>, new ()
    {
        public static int id = 0;

        /// <summary>
        /// Features that belong in this node
        /// </summary>
        readonly List<T> m_features;

        public FeatureTree()
            : this(null)
        {
            
        }
        public FeatureTree(Func<T, T, int> comparison)
            : base(comparison)
        {            
            m_left          = null;
            m_right         = null;            
            m_features      = new List<T>();
        }
        public IEnumerable<U> Build()
        {
            var features = new List<U>();
            if (m_features.Count > 0)
            {
                var feature = new U {Id = id++};
                m_features.ForEach(feature.AddChildFeature);
                feature.CalculateStatistics();
                features.Add(feature);
            }

            
            if (m_left != null)
                features.AddRange(m_left.Build());
            if (m_right != null)
                features.AddRange(m_right.Build());

            return features;
        }
        
        /// <summary>
        /// Inserts a new feature tree into the mix...
        /// </summary>
        protected override void Insert(FeatureTree<T, U> node, T feature)
        {
            if (node == null) node = new FeatureTree<T, U>(m_func);
            node.Insert(feature);
        }

        protected override void Add(T feature)
        {
            m_features.Add(feature);
            m_left  = new FeatureTree<T, U>(m_func);
            m_right = new FeatureTree<T, U>(m_func);
        }
        protected override int Compare(T feature)
        {
            return m_func(m_features[0], feature);
        }
    }
}