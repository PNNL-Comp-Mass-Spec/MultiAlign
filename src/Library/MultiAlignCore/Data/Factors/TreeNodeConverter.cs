namespace MultiAlignCore.Data.Factors
{
    /// <summary>
    ///     Summary description for clsTreeNodeConverter.
    /// </summary>
    public sealed class classTreeNodeConverter
    {
        ///// <summary>
        ///// Converts a tree node heirachy to the data label heirachy for visualization.
        ///// </summary>
        ///// <param name="root">current root of the tree node structure.</param>
        ///// <returns>converted clsLabelAttributes class representing the root of the clsTreeNode Tree.</returns>
        //public void BuildLabels(clsTreeNode root, ref clsLabelAttributes lblRoot)
        //{
        //    if (root == null)
        //        return;
        //    if (lblRoot == null)
        //    {
        //        lblRoot = new clsLabelAttributes();
        //        lblRoot.text = root.Name;				
        //        lblRoot.root = null;
        //    }			

        //    foreach(clsTreeNode child in root.Children)
        //    {
        //        if (child != null)
        //        {
        //            clsLabelAttributes lblChild = new clsLabelAttributes();
        //            lblChild.text = child.Name;					
        //            lblRoot.AddBranch(lblChild);
        //            BuildLabels(child, ref lblChild);
        //        }
        //    }			
        //}
    }
}