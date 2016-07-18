using System;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Unbalanced BST
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <typeparam name="TNodeType">Node Type for (Left and Right)</typeparam>
    public abstract class BinarySearchTree<T, TNodeType>
        where TNodeType: BinarySearchTree<T, TNodeType>, new ()
    {
        /// <summary>
        /// Comparison function between two features
        /// </summary>
        protected Func<T, T, int> m_func;
        /// <summary>
        /// Right features
        /// </summary>
        protected TNodeType m_right;
        /// <summary>
        /// Left features
        /// </summary>
        protected TNodeType m_left;

        protected BinarySearchTree()
            : this(null)
        {

        }

        protected BinarySearchTree(Func<T, T, int> comparison)
        {
            m_left          = null;
            m_right         = null;
            m_func          = comparison;
        }

        public Func<T, T, int> CompareFunction
        {
            get { return m_func; }
            set { m_func = value; }
        }

        /// <summary>
        /// Adds the feature to the tree.
        /// </summary>
        /// <param name="feature"></param>
        protected abstract void Add(T feature);

        /// <summary>
        /// Adds the feature to the tree.
        /// </summary>
        /// <param name="feature"></param>
        protected abstract int Compare(T feature);

        /// <summary>
        /// Inserts a feature into the tree
        /// </summary>
        /// <param name="feature"></param>
        public virtual void Insert(T feature)
        {
            // If this node is empty...it belongs here
            if (m_left == null && m_right == null)
            {
                Add(feature);
                return;
            }

            // Use the first item as a feature
            var compare = Compare(feature);

            if (compare == 0)
                Add(feature);
            else if (compare > 0)
                Insert(m_right, feature);
            else
                Insert(m_left, feature);
        }

        protected virtual void Insert(TNodeType node, T feature)
        {
            if (node == null)
            {
                node = new TNodeType {CompareFunction = m_func};
            }

            node.Insert(feature);
        }
    }
}