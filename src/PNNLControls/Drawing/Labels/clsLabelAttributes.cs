using System;
using System.Collections;

using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsLabelAttributes.
	/// </summary>
	public class clsLabelAttributes: ICloneable		
	{
		public string text;
		public ArrayList branches;
		public clsLabelAttributes root;
		public int level;
		public int leafIndex; //if the label is a leaf, shows the current position relative to other leaves
		public int dataTag;    //tags this label to a data item		
		public int groupTag; //tags this label to a group
		public string groupText;
		public Color backgroundColor;
		public ctlLabelItem label;

		public delegate void ChangedDelegate (clsLabelAttributes newLbl, clsLabelAttributes prevLbl);

		public clsLabelAttributes()
		{
			//
			// TODO: Add constructor logic here
			//
			this.text = "";
			this.branches = null;
			this.root = null;
			this.level = int.MinValue;
			this.leafIndex = int.MinValue;
			this.dataTag = int.MinValue;
			this.groupTag = int.MinValue;
			this.groupText = string.Empty;
			backgroundColor = SystemColors.Control;
		}

		public clsLabelAttributes AddBranch(clsLabelAttributes branch)
		{
			try
			{
				if (this.branches == null)
				{
					this.branches = new ArrayList();
				}
				branch.root = this;
				branch.level = this.level + 1;
				this.branches.Add(branch);
				return branch;

			}
			catch(Exception e)
			{
				//System.Windows.Forms.MessageBox.Show(e.Message);
				return null;
			}
		}

		public clsLabelAttributes HighLeaf()
		{
			try
			{
				clsLabelAttributes child = this;
				while (child.branches!=null)
				{
					child = child.branches[child.branches.Count-1] as clsLabelAttributes;
				}
				return child;

			}
			catch(Exception e)
			{
				//System.Windows.Forms.MessageBox.Show(e.Message);
				return null;
			}
		}

		public clsLabelAttributes LowLeaf()
		{
			try
			{
				clsLabelAttributes child = this;
				while (child.branches!=null)
				{
					child = child.branches[0] as clsLabelAttributes;
				}
				return child;

			}
			catch(Exception e)
			{
				//System.Windows.Forms.MessageBox.Show(e.Message);
				return null;
			}
		}

		public bool IsLeaf()
		{
			return(this.branches==null);
		}

		public clsLabelAttributes AddBranch(string lbl)
		{
			clsLabelAttributes branch = new clsLabelAttributes();
			branch.text = lbl;
			return (AddBranch(branch));
		}

		public void AddChildren(ArrayList children, Range r)
		{
			try
			{
				for (int i=r.StartRow; i<=r.EndRow; i++)
				{
					this.AddBranch(children[i] as clsLabelAttributes);
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		#region ICloneable Members

		public object Clone()
		{
			// TODO:  Add clsLabelAttributes.Clone implementation

			clsLabelAttributes clone = new clsLabelAttributes();

			clone.text = this.text;
			clone.branches = null;	//don't clone connections!
			clone.root = null;	//don't clone connections!
			clone.level = this.level;
			clone.leafIndex = this.leafIndex;
			clone.dataTag = this.dataTag;
			clone.groupTag = this.groupTag;
			clone.groupText = this.groupText;
			return clone;
		}

		#endregion

		public class SortGroupTag : IComparer  
		{
			int IComparer.Compare( Object x, Object y )  
			{
				try
				{
					clsLabelAttributes cX = x as clsLabelAttributes;
					clsLabelAttributes cY = y as clsLabelAttributes;
					return (cX.groupTag.CompareTo(cY.groupTag));
				}
				catch
				{
					throw (new ArgumentException("Arguments must be clsLabelAttribute"));
				}
			}
		}

		public class SortDataTag : IComparer  
		{
			int IComparer.Compare( Object x, Object y )  
			{
				try
				{
					clsLabelAttributes cX = x as clsLabelAttributes;
					clsLabelAttributes cY = y as clsLabelAttributes;
					return (cX.dataTag.CompareTo(cY.dataTag));
				}
				catch
				{
					throw (new ArgumentException("Arguments must be clsLabelAttribute"));
				}
			}
		}
	}
}
