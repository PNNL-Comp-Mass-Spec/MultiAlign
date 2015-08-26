using System;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Binary Search Tree Node
    /// </summary>
    public class TreeNode<T> where T : IComparable, IComparable<T>
    {
        public TreeNode(T value)
        {
            Left   = null;
            Right  = null;
            Parent = null;
            Value  = value;
        }

        public T Value { get; set; }

        public TreeNode<T> Left { get; set; }
        public TreeNode<T> Right { get; set; }
        public TreeNode<T> Parent { get; set; }
    }
    /// <summary>
    /// Binary Search Tree for quick searching.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BSTTree<T> where T : IComparable, IComparable<T>
    {
        public BSTTree()
        {
            Root = null;
        }

        public TreeNode<T> Root { get; set; }

        /// <summary>
        /// Inserts the item into the tree.  If two elements are the same on insert, the element
        /// is inserted left.
        /// </summary>
        /// <param name="value"></param>
        public virtual void Insert(T value)
        {
            var node = new TreeNode<T>(value);
            if (Root == null)
                Root = node;
            else
            {
                Insert(Root, node);
            }
        }

        /// <summary>
        /// Inserts the node into the parent node.  If two elements are equal
        /// then the node is inserted left.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="node"></param>
        protected virtual void Insert(TreeNode<T> parent, TreeNode<T> node)
        {
            // Greater Than the Parent - go right
            if (node.Value.CompareTo(parent.Value) > 0)
            {
                node.Parent = parent;
                if (parent.Right == null)
                {
                    parent.Right = node;
                    return;
                }
                Insert(parent.Right, node);  
            }
            else
            {
                node.Parent = parent;
                if (parent.Left == null)
                {
                    parent.Left = node;
                    return;
                }
                Insert(parent.Left, node);                
            }
        }
    }

    /// <summary>
    /// Minimum Spanning Tree Linear Relationship Tree
    /// 
    /// This class has a special insert function for creating the Linear Relationship's Binary Search Tree (LR-BST).
    /// This reduces the search complexity from O(N^2) for creating clusters to O(N*logN)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MstLrTree<T> : BSTTree<T>
        where T : IComparable, IComparable<T>
    {
        protected override void Insert(TreeNode<T> parent, TreeNode<T> node)
        {                        
            var compareValue             = node.Value.CompareTo(parent.Value);

            // Greater Than the Parent - replace parent...
            if (compareValue > 0)
            {
                node.Parent     = parent.Parent;
                parent.Parent   = node;

                // Everything goes left,,, that means the parent
                // came before during construction of the MST
                node.Left       = parent;                                
            }
            else 
            {
                // This would mean that the parent was equal...so we move everything right   
                // If the parent was bigger..we still move everything right...it's
                // only if the node is bigger than the child that we move anything left....
                // This tells us about the structure of the clusters...
                // some worst case sorting could happen here...O(N^2) but we are bounded by this number
                // and best case is O(N*logN)
                if (parent.Right == null)
                {
                    parent.Right = node;
                    return;
                }
                Insert(parent.Right, node);                        
            }            
        }
    }
}
