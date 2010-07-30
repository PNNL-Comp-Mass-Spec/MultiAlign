using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlHierarchalLabel.
	/// </summary>
	public class ctlHierarchalLabel : System.Windows.Forms.UserControl, ICloneable
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
	
		private bool mbln_overrideResize = false;

		/// <summary>
		/// if the number of displayed labels is too large to be viewed, a numerical axis is drawn instead.
		/// AxisMode indicates which type of axis is displayed.
		/// </summary>
		public enum AxisMode
		{
			axisModeLabel,
			axisModeNumerical
		}

		private enum PopUpIndices
		{
			popup_Zoomout,
			popup_EditLevel,
			popup_EditLabel,
			popup_Properties,
			popup_Insert,
			popup_Delete,
			popup_Parent,
			popup_Child
		}

		public class AxisRangeF
		{
			public  double low = double.MinValue;
			public  double high =double.MinValue;

			public AxisRangeF (double lower, double upper)
			{
				low = lower;
				high = upper;
			}
		}

		public class AxisRangeI
		{
			public  int  low = int.MinValue;
			public   int  high = int.MinValue;

			public AxisRangeI (int lower, int upper)
			{
				low = lower;
				high = upper;
			}
		}

		/// <summary>
		/// constructor
		/// </summary>
		public ctlHierarchalLabel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call		
			labelMenu = new ContextMenu();
			
			labelMenu.MenuItems.Add(new MenuItem("Edit Label", new EventHandler(this.LabelEditHandler)));
			labelMenu.MenuItems.Add(new MenuItem("Edit Level", new EventHandler(this.LevelEditHandler)));	
			labelMenu.MenuItems.Add(new MenuItem("Properties", new EventHandler(this.LabelPropertiesHandler)));
			labelMenu.MenuItems.Add(new MenuItem("Sort", new EventHandler(this.LabelSortHandler)));
			labelMenu.MenuItems.Add(new MenuItem("Insert", new EventHandler(this.InsertLabelHandler)));
			labelMenu.MenuItems.Add(new MenuItem("Delete", new EventHandler(this.DeleteLabelHandler)));
			labelMenu.MenuItems.Add(new MenuItem("Add Parent", new EventHandler(this.AddParentHandler)));
			labelMenu.MenuItems.Add(new MenuItem("Add Child", new EventHandler(this.AddChildHandler)));
			AttachZoomOutMenus();
			
			axis.Zoom += new ctlSingleAxis.ZoomDelegate(this.AxisZoomIn);
			axis.UnZoom += new ctlSingleAxis.UnZoomDelegate(this.AxisZoomOut);
		}

		private void AttachZoomOutMenus()
		{
			labelMenu.MenuItems.Add(new MenuItem("Zoom Out", new EventHandler(this.HandleZoomOut)));
			labelMenu.MenuItems.Add(new MenuItem("Zoom In", new EventHandler(this.HandleZoomIn)));
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Event Handlers

		private void ZoomIn(int low, int high)
		{
			RaiseUnselect();
			mZoomHistory.Add(new Point(mLowLabel, mHighLabel));
			this.UpdateComplete = false;
			DrawControl(low, high);	
			this.UpdateComplete = true;
		}

		private void HandleZoomIn(object sender, System.EventArgs e)
		{
			if (mMenuLabel==null) return;

			try
			{
				int low = mMenuLabel.clsLabelAttributes.LowLeaf().leafIndex;
				int high = mMenuLabel.clsLabelAttributes.HighLeaf().leafIndex;

				//check to see if we are already zoomed to this point
				if (mLowLabel==low && mHighLabel==high) return;

				ZoomIn(low, high);	
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// applies the zoom rectangle to zoom in on a range of labels
		/// </summary>
		public  void AxisZoomIn(Point start, Point stop)
		{
			try
			{
				int low = 0;
				int high = 0;
				if (mVertical)
				{
					low = Math.Min(start.Y, stop.Y);
					low = CalcZoomBound(low, 0, axis.Height, mLowLabel, mHighLabel);
					high = Math.Max(start.Y, stop.Y);
					high = CalcZoomBound(high, 0, axis.Height, mLowLabel, mHighLabel);
				}
				else
				{
					low = Math.Min(start.X, stop.X);
					low = CalcZoomBound(low, 0, axis.Width, mLowLabel, mHighLabel);
					high = Math.Max(start.X, stop.X);
					high = CalcZoomBound(high, 0, axis.Width, mLowLabel, mHighLabel);
				}
				
				ZoomIn(low, high);
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message
                
            
		}

//		private void AxisZoomIn(Point start, Point stop)
//		{
//			try
//			{
//				int low = 0;
//				int high = 0;
//				if (mVertical)
//				{
//					low = Math.Min(start.Y, stop.Y);
//					low = CalcZoomBound(low, 0, axis.Height, mLowLabel, mHighLabel);
//					high = Math.Max(start.Y, stop.Y);
//					high = CalcZoomBound(high, 0, axis.Height, mLowLabel, mHighLabel);
//				}
//				else
//				{
//					low = Math.Min(start.X, stop.X);
//					low = CalcZoomBound(low, 0, axis.Width, mLowLabel, mHighLabel);
//					high = Math.Max(start.X, stop.X);
//					high = CalcZoomBound(high, 0, axis.Width, mLowLabel, mHighLabel);
//				}
//
//				ZoomIn(low, high);
//			}
//			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
//		}

		private void LabelEditHandler(object sender, System.EventArgs e)
		{
			if (mMenuLabel==null) return;

			clsLabelAttributes prev = (clsLabelAttributes) mMenuLabel.clsLabelAttributes.Clone();
			
			mMenuLabel.ShowEdit();

			if (this.LabelChanged!=null)
				LabelChanged(mMenuLabel.clsLabelAttributes, prev);
		}

		private void LevelEditHandler(object sender, System.EventArgs e)
		{
			try
			{
				if (mMenuLabel==null) return;
				clsLabelAttributes lbl = mMenuLabel.clsLabelAttributes;
				int level = lbl.level;

				if (mLabelLevels==null) return;
				if (mLabelLevels.Count<=level) return;

				clsLabelCollection levelLabels = mLabelLevels[level] as clsLabelCollection;
				levelLabels.Edit();
			}
			catch{}
		}


		private void LabelPropertiesHandler(object sender, System.EventArgs e)
		{
			try
			{
				if (mMenuLabel!=null)
				{
					clsLabelAttributes prev = (clsLabelAttributes) mMenuLabel.clsLabelAttributes.Clone();
					mMenuLabel.EditLabelProperties();
					if (this.LabelChanged!=null)
						LabelChanged(mMenuLabel.clsLabelAttributes, prev);
				}
			}
			catch{}
		}

		private class CompareLabel : IComparer  
		{
			int IComparer.Compare( Object x, Object y )  
			{
				try
				{
					clsLabelAttributes cX = x as clsLabelAttributes;
					clsLabelAttributes cY = y as clsLabelAttributes;
					return (cX.text.CompareTo(cY.text));
				}
				catch
				{
					throw (new ArgumentException("Arguments must be clsLabelAttributes"));
				}
			}
		}

		/// <summary>
		/// Sorts down hierarchy using the Sort delagate if it exists, or
		/// the default sort if not
		/// </summary>
		/// <param name="root"></param>
		private IComparer mDefaultCompare = new CompareLabel();
		public void SortLabel (clsLabelAttributes root)
		{
			if (root==null) return;
			if (root.branches==null) return;

			if (mExternalComparer!=null)
			{
				root.branches.Sort(mExternalComparer);

			}
			else
			{
				root.branches.Sort(mDefaultCompare);				
			}

			for (int i=0; i<root.branches.Count; i++)
			{
				SortLabel(root.branches[i] as clsLabelAttributes);
			}
			
		}

		private void LabelSortHandler(object sender, System.EventArgs e)
		{
			try
			{
				if (mMenuLabel!=null)
				{
					SortLabel(mMenuLabel.clsLabelAttributes);
					OnLabelsMoved();
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
		private void DeleteLabelHandler(object sender, System.EventArgs e)
		{
			try
			{
				if (mMenuLabel!=null)
					DeleteLabel(mMenuLabel.clsLabelAttributes);
			}
			catch{}
		}

		private void InsertLabelHandler(object sender, System.EventArgs e)
		{
			try
			{
				clsLabelAttributes newLbl = new clsLabelAttributes();
				newLbl.text = "new label";
				if (mMenuLabel!=null)
					InsertLabel(mMenuLabel.clsLabelAttributes, newLbl);
			}
			catch{}
		}

		private void AddParentHandler(object sender, System.EventArgs e)
		{
			try
			{
				if (mMenuLabel!=null)
					AddLevel(mMenuLabel.clsLabelAttributes);
			}
			catch{}
		}

		private void AddChildHandler(object sender, System.EventArgs e)
		{
			try
			{
				if (mMenuLabel!=null)
					AddChild (mMenuLabel.clsLabelAttributes, "new child");
			}
			catch{}
		}
		#endregion

		#region Events

		public delegate void LabelEventDelegate (clsLabelAttributes lbl);
		public event LabelEventDelegate  MenuHandler = null;
		public event LabelEventDelegate  LabelMouseUp = null;
		public event LabelEventDelegate  LabelMouseHover = null;

		/// <summary>
		/// 
		/// </summary>
		public event clsLabelAttributes.ChangedDelegate LabelChanged = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void ClientHeightDelegate (int height);
		public event ClientHeightDelegate ClientHeight = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void ClientWidthDelegate (int width);
		public event ClientWidthDelegate ClientWidth = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void ScrollPositionDelegate (int pos);
		public event ScrollPositionDelegate ScrollPosition = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void LabelUpdateDelegate ();
		public event LabelUpdateDelegate LabelUpdated = null;

		/// <summary>
		/// event handler for range changed event.  Range refers to the range of label leaves selected.
		/// Valid ranges are 0 to N-1 where N is the upper bound of the leaf label array
		/// </summary>
		public delegate void RangeDelegate (int lowRange, int highRange);
		public event RangeDelegate Range = null;

		/// <summary>
		/// event handler for the selected event.  Parameters refer to the low and high pixel encompassed by the 
		/// selected range of leaf labels.  Used to map a selected label region to a client region
		/// </summary>
		public delegate void SelectedDelegate (int lowPixel, int highPixel);
		public event SelectedDelegate Selected = null;

		/// <summary>
		/// event handler for the leaves moved event.  returns an array to data tags.
		/// </summary>
		public delegate void LeavesMovedDelegate (int [] tags);
		public event LeavesMovedDelegate LeavesMoved = null;

		/// <summary>
		/// event handler for the align event.  provides an array of pixel positions showing the alignment of the 
		/// displayed leaf labels.  Used to align data in a client area to the label positions.
		/// </summary>
		public delegate void AlignDelegate (int [] positions);
		public event AlignDelegate Align = null;

		#endregion

		#region Fields

		/// <summary>
		/// collection of ctlLabelItems
		/// Each item holds the overall label properties
		/// for a level
		/// </summary>
		private ArrayList mLabelLevels = new ArrayList();

		private int mLeafCount = 0;

		private ContextMenu labelMenu = null;

		//hasn't been drawn
		private bool mUpdateComplete = false;

		private bool mDragEnabled = true;

		/// <summary>
		/// show scroll bar
		/// </summary>
		private bool mShowScroll = false;

		/// <summary>
		/// Minimum size in pixels for a leaf
		/// if leaves are = or smaller, we use the scroll bar to show them.
		/// </summary>
		private int mMinLeafDim = int.MinValue;

		/// <summary>
		/// actual leaf height used for scrolling
		/// </summary>
		private float mMinFontSize = 6.0f;

		/// <summary>
		/// number of visible leaves used for scrolling
		/// </summary>
		private int mNViewLeaves = int.MinValue;

		/// <summary>
		/// holds the collection of leaf labels
		/// </summary>
		private clsLabelCollection mLeafLabels = null;

		/// <summary>
		/// the axis is laid out either horizontally or vertically based on the value of this flag
		/// </summary>
		bool mVertical = false;

		/// <summary>
		/// holds the number of label levels to be drawn.  labels are drawn at levels 1 - N.  Level 0 
		/// indicates the root level, which is not drawn.
		/// </summary>
		private int nLevels = 0;

		/// <summary>
		/// root of the label tree.  
		/// </summary>
		clsLabelAttributes mRoot = new clsLabelAttributes();

		/// <summary>
		/// references to the leaves of the label tree are held in this array list to give random access
		/// and a sequential ordering.
		/// </summary>
		ArrayList mLeaves = null;

		/// <summary>
		/// container on which labels are drawn
		/// </summary>
		private System.Windows.Forms.Panel pnlLabels; 
		
		/// <summary>
		/// these variables hold the range bounds of the selected labels
		/// </summary>
		private int mLowLabel  = int.MinValue;
		private int mHighLabel = int.MinValue;

		/// <summary>
		/// pointer to selectd label
		/// </summary>
		private ctlLabelItem mCurrentSelectedLabel = null; 

		/// <summary>
		/// pointer to label being dreagged over
		/// </summary>
		private ctlLabelItem mCurrentDragOverLabel = null; 

		/// <summary>
		/// color of labels not selected
		/// </summary>
		private Color mUnselectedColor = Color.White;

		/// <summary>
		/// color of labels selected
		/// </summary>
		private Color mSelectedColor = Color.Pink;

		/// <summary>
		/// color of labels being dragged over
		/// </summary>
		private Color mDragOverColor = Color.Pink;

		/// <summary>
		/// rectangle for drag/drop
		/// </summary>
		private Rectangle dragBoxFromMouseDown;

		/// <summary>
		/// flag indicating whether we are displaying labels or a numeric axis
		/// </summary>
		private AxisMode mMode = AxisMode.axisModeLabel;

		/// <summary>
		/// list to hold history of selections
		/// </summary>
		private ArrayList mZoomHistory = new ArrayList();

		/// <summary>
		/// holds the indices of the leaf labels which are highlighted.  Used to generate the Selected event
		/// </summary>
		private int mHighlightMin = 0;
		private int mHighlightMax = 0;

		/// <summary>
		/// width of a hierarchal level (not leaf)
		/// </summary>
		private int mLevelWidth = 24;

		private System.Windows.Forms.HScrollBar hScroll;
		private System.Windows.Forms.VScrollBar vScroll;

		/// <summary>
		/// flag that indicates that a label is selected, and is potentially unselectable by a second click
		/// </summary>
		//private bool unselectable = false;

		private ctlLabelItem mMenuLabel = null;

		private PNNLControls.ctlSingleAxis axis;

		#endregion

		#region Properties

		private AxisRangeF  mAxisRange = new AxisRangeF(double.MinValue, double.MinValue);
		public AxisRangeF  AxisRange
		{
			get{return this.mAxisRange;}
			set{this.mAxisRange = value;}
		}

		private AxisRangeI  mAxisDataRange = new AxisRangeI(int.MinValue, int.MinValue);
//		public AxisRangeI  AxisDataRange
//		{
//			get{return this.mAxisDataRange;}
//			set{this.mAxisDataRange = value;}
//		}

		/// <summary>
		/// 
		/// </summary>
		private IComparer mExternalComparer = null;
		public IComparer SortFunction
		{
			get{return this.mExternalComparer;}
			set{this.mExternalComparer = value;}
		}

		/// <summary>
		/// returns an integer array of the datatags for the leaf array
		/// </summary>
		public int[] DataTags
		{
			get
			{
				if (mLeaves==null) 
					return null;

				int [] tags = new int[mLeaves.Count];
				for(int i=0; i<mLeaves.Count; i++)
					tags[i] = (mLeaves[i] as clsLabelAttributes).dataTag;
				return(tags);
			}
		}

		public int[] Alignment
		{
			get
			{
				int [] positions = (int[]) mLeafLabels.alignment.Clone();
				return(positions);
			}
			set
			{
				int [] positions = (int[]) value.Clone();
				mLeafLabels.alignment = positions;
			}

		}

		public bool UpdateComplete
		{
			get{return this.mUpdateComplete;}
			set
			{
				this.mUpdateComplete = value;
				if(value && this.LabelUpdated!=null)
					LabelUpdated();
			}
		}

		/// <summary>
		/// Minimum size in pixels for a leaf
		/// if leaves are = or smaller, we use the scroll bar to show them.
		/// </summary>
		/// 
		[System.ComponentModel.DefaultValue(int.MinValue)]
		[System.ComponentModel.Description("Minimum height for a vertical leaf or width for a horizontal leaf")]
		public int MinLeafHeight 
		{
			get{return this.mMinLeafDim;}
			set{this.mMinLeafDim = value;}
		}

		/// <summary>
		/// Minimum size in pixels for a leaf
		/// if leaves are = or smaller, we use the scroll bar to show them.
		/// </summary>
		/// 
		[System.ComponentModel.DefaultValue(int.MinValue)]
		[System.ComponentModel.Description("Minimum height for a vertical leaf or width for a horizontal leaf")]
		public int MinLeafWidth 
		{
			get{return this.mMinLeafDim;}
			set{this.mMinLeafDim = value;}
		}

		/// <summary>
		/// Minimum size in pixels for a leaf
		/// if leaves are = or smaller, we use the scroll bar to show them.
		/// </summary>
		/// 
		[System.ComponentModel.DefaultValue(6.0f)]
		[System.ComponentModel.Description("Minimum Size in points that a label can display.  If the font must be smaller, the label is not drawn")]
		public float MinFontSize 
		{
			get{return this.mMinFontSize;}
			set{this.mMinFontSize = value;}
		}

		public int CurrentScrollPosition 
		{
			get
			{
				if (mVertical) 
					return (pnlLabels.Top);
				else
					return (pnlLabels.Left);
			
			}
		}

		/// <summary>
		/// show scroll bar
		/// </summary>
		public bool ShowScroll 
		{
			get{return this.mShowScroll;}
			set
			{
				this.mShowScroll = value;

				if (value)
					pnlLabels.Dock = DockStyle.None;
				else
					pnlLabels.Dock = DockStyle.Fill;

				if (mVertical)
				{
					vScroll.Visible = value;
					if (value)
					{
						pnlLabels.Left = vScroll.Width;
						pnlLabels.Width = this.Width - vScroll.Width;
						pnlLabels.Top = 0;
					}
				}
				else
				{
					hScroll.Visible = value;
					if (value)
					{
						pnlLabels.Top = 0;
						pnlLabels.Left = 0;
						pnlLabels.Height = this.Height - hScroll.Height;
					}
				}
			}
		}

		/// <summary>
		///  aligns the scrollbar to a collection of labels
		/// </summary>
		/// <param name="lbls"></param>
		clsLabelCollection mScrollLabels = null;
		public void AlignScroll(clsLabelCollection lbls)
		{
			mScrollLabels = lbls;

			if (mVertical)
			{
				vScroll.LargeChange = mLeafShortDim;
				vScroll.SmallChange = mLeafShortDim;
				vScroll.Maximum = (mLeafCount-mNViewLeaves+1) * mLeafShortDim;
			}
			else
			{
				hScroll.LargeChange = mLeafShortDim;
				hScroll.SmallChange = mLeafShortDim;
				hScroll.Maximum = pnlLabels.Width - this.Width + mLeafShortDim;	
			}
		}

		/// <summary>
		/// background color of selected labels
		/// </summary>
		public Color SelectedColor 
		{
			get{return this.mSelectedColor;}
			set
			{
				this.mSelectedColor = value;
				axis.SelectedColor = value;
			}
		}


		/// <summary>
		/// background color of label being dragged over
		/// </summary>
		public Color DragOverColor 
		{
			get{return this.mDragOverColor;}
			set{this.mDragOverColor = value;}
		}

		/// <summary>
		/// true if drag/drop of labels is allowed
		/// </summary>
		public bool DragEnabled 
		{
			get{return this.mDragEnabled;}
			set{this.mDragEnabled = value;}
		}


		/// <summary>
		/// true if axis is vertical, false if horizontal
		/// </summary>
		public bool Vertical 
		{
			get{return this.mVertical;}
			set
			{
				this.mVertical = value;
				axis.Plotter.IsVertical = value;
				//axis.Plotter.IsInverted = value;
			}
		}

		public bool ShowAxis 
		{
			get{return this.axis.Visible;}
			set
			{
				try
				{
					if(value)
					{
						axis.Visible = true;
						if (mVertical)
						{
							axis.Dock = DockStyle.Right;
						}
						else
						{
							axis.Dock = DockStyle.Top;
						}
					}
					else
					{
						axis.Visible = false;
					}
					pnlLabels.Invalidate();
					axis.Invalidate();
				}
				catch
                {
                }
			}
		}	

		/// <summary>
		/// mode may be label or numeric
		/// </summary>
		public AxisMode Mode 
		{
			get{return this.mMode;}
			set
			{
					this.mMode = value;
					ShowAxis=(value==AxisMode.axisModeNumerical);
			}
		}	
	
		/// <summary>
		/// width of a hierarchal level (not leaf)
		/// </summary>
		public int LevelWidth 
		{
			get{return this.mLevelWidth;}
			set{this.mLevelWidth = value;}
		}		

		#endregion

		#region Create Labels

		/// <summary>
		/// returns the root of the label tree.  Used to create the label tree.
		/// </summary>
		/// <returns></returns>
		public clsLabelAttributes GetRoot()
		{
			return (mRoot);
		}

		public clsLabelAttributes Root
		{
			get{return (mRoot);}
			set
			{
				mRoot=value;
				Init();
			}
		}

		/// <summary>
		/// returns a pointer the sequential array list of label leaves
		/// </summary>
		/// <returns></returns>
		public ArrayList GetLeaves()
		{
			return (mLeaves);
		}

		public override ContextMenu ContextMenu
		{
			get
			{
				return labelMenu;
			}
			set
			{
				labelMenu = value;
				if (value != null)
					AttachZoomOutMenus();
			}
		}

		/// <summary>
		/// creates a new ctlLabelItem Control
		/// </summary>
		/// <param name="la">
		/// label attributes for new label item
		/// </param>
		/// <param name="align">
		/// string alignment for label
		/// </param>
		/// <returns></returns>
		private ctlLabelItem CreateLabel(Panel panel, clsLabelAttributes la, StringAlignment align)
		{
			ctlLabelItem l = new ctlLabelItem ();
			try
			{			
				l.ContextMenu = labelMenu;

				l.Format.Alignment = align;
				l.Text = la.text;
				l.BackColor = la.backgroundColor;
				l.Vertical = la.IsLeaf() ^ mVertical; //exclusive or
				
				l.clsLabelAttributes = la;  //links this label to the label hierarchy
				la.label = l;				//link this hierarchy item to the label

				panel.Controls.Add (l);

				mUnselectedColor = l.BackColor;
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
			return (l);
		}

		private void label_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
			if (e.Delta < 0)
				HandleZoomIn(null, null);
			else
				HandleZoomOut(null, null);
		}

		/// <summary>
		/// recursive function to build array of all mLeaves
		/// </summary>
		/// <param name="root">
		/// root of the label tree
		/// </param>
		private void ParseLeaves(clsLabelAttributes root)
		{
			if (root.level > nLevels)
			{
				nLevels = root.level;
			}

			try
			{
				root.label = null; //clears previous label from previous loads

				if (root.branches == null) //is a leaf
				{
					root.leafIndex = mLeaves.Count;  //used to track index in overall dataset.
					mLeaves.Add (root);
					return;
				}
				else  //parse branches
				{
					for (int i=0; i<root.branches.Count; i++)
					{
						clsLabelAttributes branch = root.branches[i] as clsLabelAttributes;
						branch.level = root.level + 1;
						ParseLeaves(branch);
					}
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// recursive function to clear label references from tree
		/// </summary>
		/// <param name="root">
		/// root of the label tree
		/// </param>
		private void ClearLabelReferences(clsLabelAttributes root)
		{
			try
			{
				root.label = null; //clears label from previous loads

				if (root.branches != null) //is not a leaf
				{
					//parse branches
					for (int i=0; i<root.branches.Count; i++)
					{
						ClearLabelReferences(root.branches[i] as clsLabelAttributes);
					}
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private int LeafTop(clsLabelAttributes leaf)
		{
			double height = (double)pnlLabels.Height / (double)mLeafCount;
			//subtract a pixel to overlap labels
			//return (int)((double)(leaf.leafIndex-mLowLabel) * height) -1;
			return (int)((double)(leaf.leafIndex-mLowLabel) * height);
		}

		private int LeafBottom(clsLabelAttributes leaf)
		{
			double height = (double)pnlLabels.Height / (double)mLeafCount;
			return (int)((double)(leaf.leafIndex-mLowLabel+1) * height);
		}

		private int LeafLeft(clsLabelAttributes leaf)
		{
			double width = (double)pnlLabels.Width / (double)mLeafCount;
			//subtract a pixel to overlap labels
			//return (int)((double)(leaf.leafIndex-mLowLabel) * width) -1;
			
			return (int)((double)(leaf.leafIndex-mLowLabel) * width);
		}

		private int LeafRight(clsLabelAttributes leaf)
		{
			double width = (double)pnlLabels.Width / (double)mLeafCount;
			//return (int)((double)(leaf.leafIndex-mLowLabel+1) * width);
			return (int)((double)(leaf.leafIndex-mLowLabel+1) * width);
		}

		private int LabelWidth(clsLabelAttributes attr)
		{
			if (attr.IsLeaf())
			{
				return( pnlLabels.Width - (attr.level-1) * mLevelWidth + 1);
			}
			else
			{
				return(mLevelWidth);
			}
		}

		private int LabelHeight(clsLabelAttributes attr)
		{
			if (attr.IsLeaf())
			{
				return(pnlLabels.Height - (attr.level-1) * mLevelWidth);
			}
			else
			{
				return(mLevelWidth);
			}
		}

		private int HorizontalLabelTop(clsLabelAttributes attr)
		{
			if (attr.IsLeaf())
			{
				return(0);
			}
			else
			{
				return(pnlLabels.Height - (attr.level) * mLevelWidth);
			}
		}


		/// <summary>
		/// Sets the position and size for drawing a label at a hierarchal level
		/// </summary>
		/// <param name="lbl">label to be positioned</param>
		/// <param name="level">hierarchal level, starting at 1</param>
		/// <param name="start">leaf that defines the start of the label</param>
		/// <param name="end">leaf that defines the end of the lable</param>
		private void PositionLabel(ctlLabelItem lbl)
		{
			try
			{
				clsLabelAttributes la = lbl.clsLabelAttributes;

				if (mVertical)
				{
					lbl.Width = LabelWidth(la);
					lbl.Left = (la.level-1) * mLevelWidth;

					lbl.Top =  Math.Max(LeafTop(la.LowLeaf()), 0);
					int bottom = Math.Min(LeafBottom(la.HighLeaf()), pnlLabels.Height);
					lbl.Height = bottom - lbl.Top;
				}
				else
				{
					lbl.Height = LabelHeight(la);
					lbl.Top = HorizontalLabelTop(la);

					lbl.Left = LeafLeft(la.LowLeaf());
					lbl.Width = LeafRight(la.HighLeaf()) - lbl.Left;
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		//creates a collection of label pointers that represents labels
		//to be drawn at the lowest drawn level.  labels may be at level or above
		public clsLabelCollection BuildLowestLevel (int level)
		{
			int index = 0;

			clsLabelCollection newLevel = null;

			try
			{
				newLevel = new clsLabelCollection(pnlLabels, mVertical);

				newLevel.Clear();
				
				index = mLowLabel;
				
				while (index<=mHighLabel)
				{
					clsLabelAttributes lblPtr = mLeaves[index] as clsLabelAttributes;

					while (lblPtr!=mRoot && lblPtr.level > level)
					{
						lblPtr = (clsLabelAttributes) lblPtr.root;
					}

					if (lblPtr!=mRoot)
					{
						if (mVertical) 
							lblPtr.label.Width++;
						newLevel.Add(lblPtr);

						lblPtr = lblPtr.HighLeaf();
						index = lblPtr.leafIndex+1;
					}
					else
					{
						index++;
					}
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}	
	
			return newLevel;
		}

		//levels are collections of pointers to labels
		//actual labels are created for the first time when they are drawn
		public clsLabelCollection BuildLabelLevel (int level)
		{
			int index = 0;

			try
			{
				while (mLabelLevels.Count<=level)
				{
					mLabelLevels.Add(new clsLabelCollection(pnlLabels, mVertical));
				}

				clsLabelCollection newLevel = mLabelLevels[level] as clsLabelCollection;

				newLevel.Clear();
				
				index = mLowLabel;
				
				while (index<=mHighLabel)
				{
					clsLabelAttributes lblPtr = mLeaves[index] as clsLabelAttributes;

					while (lblPtr!=mRoot && lblPtr.level != level)
					{
						lblPtr = (clsLabelAttributes) lblPtr.root;
					}

					if (lblPtr.level == level)
					{
						newLevel.Add(lblPtr);

						lblPtr = lblPtr.HighLeaf();
						index = lblPtr.leafIndex+1;
					}
					else
					{
						index++;
					}
				}

				return newLevel;
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}	
			
			return null;
		}
		
//		//the leaves may be associated with more than one level
//		//so, we can't just us a level to address them.
//		public void BuildLeafCollection()
//		{
//			//gather and align leaf labels
//			if (mLeafLabels==null)
//			{
//				mLeafLabels = new clsLabelCollection(pnlLabels, mVertical);
//			}
//			mLeafLabels.Clear();
//			for (int i=mLowLabel; i<=mHighLabel; i++)
//			{
//				mLeafLabels.Add(mLeaves[i]);
//			}
//		}


		/// <summary>
		/// draw the labels at a selected level onto the label container
		/// </summary>
		/// <param name="panel">
		/// labels container
		/// </param>
		/// <param name="level">
		/// level to draw.  root is level 0, and is not drawn.
		/// </param>
		public void DrawLevel (Panel panel, clsLabelCollection c)
		{
			//draw parents
			int startIndex = mLowLabel;

			try
			{
				c.alignment = new int[c.Count+1];

				for (int i=0; i<c.Count; i++)
				{
					clsLabelAttributes lblPtr = c[i] as clsLabelAttributes;

					if (lblPtr.label == null)
					{
						if (lblPtr.IsLeaf())
							lblPtr.label = CreateLabel(pnlLabels, lblPtr, StringAlignment.Far);
						else
							lblPtr.label = CreateLabel(pnlLabels, lblPtr, StringAlignment.Center);
					}

					PositionLabel(lblPtr.label);
				}
				c.BuildAlignment();
				
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}		
		}

		private void RaiseSize(int height, int width)
		{
			if(ClientHeight!=null)
				ClientHeight(height);
			if(ClientWidth!=null)
				ClientWidth(width);
		}

		private void RaiseAlign()
		{
			if (Align!=null) 
			{
				axis.Alignment = null;
				if (mLeafLabels==null)
				{
					Align(null);
				}
				else if (mLeafLabels.alignment==null)
				{
					Align(null);
				}
				else
				{
					int [] positions = (int[]) mLeafLabels.alignment.Clone();
					Align(positions);
					axis.Alignment = positions;
				}
			}
		}

		/// <summary>
		/// checks to see if the labels would be large enough to be drawn
		/// </summary>
		/// <param name="g">
		/// graphics object for label container
		/// </param>
		/// <param name="minFontPoints">
		/// minimum size in points for the labels to be valid
		/// </param>
		/// <param name="labelHeight">
		/// height of the proposed labels
		/// </param>
		/// <returns>
		/// true if the label can be drawn greater than the min size, false if not
		/// </returns>
		private bool Drawable (float minFontPoints, double labelHeight)
		{
			Graphics g = pnlLabels.CreateGraphics();
			double dpiY = (double) g.DpiY;
			double points = labelHeight * 72.0 / dpiY;
			return (points >= minFontPoints);
		}

		private int LongDim 
		{
			get
			{
				if (mVertical)
					return (this.Height);
				else
					return (this.Width);
			}
		}

		private int ShortDim 
		{
			get
			{
				if (mVertical)
					return (this.Width);
				else
					return (this.Height);
			}
		}

		private int mLeafShortDim = 1;
		public bool SetLabelField (Panel panel)
		{
			bool drawLeaves = false;
			try
			{
				if (mLeafCount<=0) return drawLeaves;

				double longDim = (double)LongDim;
				double leafShortDim = 0;
		
				leafShortDim = longDim / (double)mLeafCount;

				ShowScroll = false;
				panel.Dock = DockStyle.Fill;

				//if minimum leaf dim is defined
				if (mMinLeafDim > 0)
				{
					if (leafShortDim<mMinLeafDim && mLeafCount<50) 
					{
						mNViewLeaves = (int) Math.Floor(longDim/mMinLeafDim);
						if (mNViewLeaves<=0) return drawLeaves;

						leafShortDim = longDim / mNViewLeaves;
						mLeafShortDim = (int) leafShortDim;

						ShowScroll = true;  //sets docking style and positions everything but long dimension

						int pnlDim = (int) (leafShortDim * mLeafCount);

						if (mVertical)
						{
							pnlLabels.Height = pnlDim;
						}
						else
						{
							pnlLabels.Width = pnlDim;
						}

						drawLeaves = true;
					}
					else if (leafShortDim>=mMinLeafDim)
					{
						drawLeaves = true;
					}
				}
				else if (leafShortDim>10.0)
				{
					drawLeaves = true;
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}

			return drawLeaves;
		}

		private double  InterpAxisBound (double  dataIndex)
		{
			try
			{
				double  denominator = (double) (mAxisDataRange.high - mAxisDataRange.low);
				if (denominator==0) return (mAxisRange.low);
				double frac =dataIndex / denominator;
				if (frac<=0.0)
					return (mAxisRange.low);
				if (frac>=1.0)
					return (mAxisRange.high);
				double range = (float)(mAxisRange.high-mAxisRange.low);

				double val = 0.0;
				if (axis.Plotter.IsInverted)
				{
					val = (frac*range)+mAxisRange.low;
				}
				else
				{
					val = mAxisRange.high - (frac*range);
				}
				return val;
			}
			catch(Exception e)
			{
				//System.Windows.Forms.MessageBox.Show(e.Message);
				return 0;
			}
		}

		public void DrawAxis()
		{
			if (mAxisRange!=null)
			{
				float low = (float) InterpAxisBound((double)mLowLabel-.5);
				float high=(float)InterpAxisBound((double)mHighLabel+.5);
				if (low>high)
				{
					axis.Plotter.UnitPlotter.SetRange(high,low);
				}
				else
				{
					axis.Plotter.UnitPlotter.SetRange(low,high);
				}
			}
			else
			{
				axis.Plotter.UnitPlotter.SetRange(0f, 1f);
			}
		}

		private bool LabelsExist()
		{
			return (mLeaves != null &&  mLeaves.Count > 0);
		}

		/// <summary>
		/// draws the leaf labels, overlapping them to form a single pixel border
		/// hierarchal label levels are then drawn by calling a second routine
		/// </summary>
		/// <param name="panel">
		/// label container
		/// </param>
		public void DrawLabels (Panel panel)
		{
			//if, for whatever reason, the control dimensions are not defined, bail
			if (this.Height<=0 || this.Width<=0) return;
			//if bounds are not initialized, bail
			if (mLowLabel==int.MinValue ||  mHighLabel==int.MinValue) return;

			//if no labels, then just draw the axis
			if (!LabelsExist()) 
			{
				DrawAxisOnly();			
				return;
			}

			panel.BorderStyle = BorderStyle.None;

			try
			{
				bool drawLeaves = SetLabelField(pnlLabels);

				if (drawLeaves)
				{
					ShowAxis=false;

					//draw labels
					for (int lev = 1; lev <= nLevels; lev++)
					{
						clsLabelCollection c = BuildLabelLevel(lev);
						DrawLevel(pnlLabels, c);
					}
					//have to wait until labels are drawn to build the alignment
					//note that leaves are not necessarily all on the same level
					mLeafLabels = BuildLowestLevel(nLevels);
					mLeafLabels.BuildAlignment();
					AlignScroll(mLeafLabels);

					//order is important here, need to size target control before aligning data
					RaiseSize(pnlLabels.Height, pnlLabels.Width);

					//if we are scrolling, raise the height and position
					if (ShowScroll)
					{
						if(mVertical)
							vScroll_Scroll(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0));
						else						
							hScroll_Scroll(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0));
					}
				}
				else //not drawing leaves
				{
					ShowAxis=true;

					if (mVertical)
					{
						axis.Width = this.Width - ((nLevels-1) * mLevelWidth);
					}
					else
					{
						axis.Height = this.Height - ((nLevels-1) * mLevelWidth);
					}

					//draw non-leaf levels
					int lowest = int.MinValue;
					for (int lev = nLevels-1; lev > 0; lev--)
					{
						clsLabelCollection c = BuildLabelLevel(lev);
						if (c.AverageShortDim > Math.Max(mMinLeafDim, 10))
						{
							DrawLevel(pnlLabels, c);
							if (lowest == int.MinValue)
								lowest = lev;
						}
						else
						{
							int pnlDim = (lev - 1) * mLevelWidth;
							if (mVertical)
							{
								axis.Width = this.Width - pnlDim;
							}
							else
							{
								axis.Height = this.Height - pnlDim;
							}
						}
					}

					DrawAxis();

					//null out leaf alignment
					if (mLeafLabels!=null)
						mLeafLabels.alignment = null;

					if (lowest>int.MinValue)
					{
						mLeafLabels = BuildLowestLevel(lowest);
						mLeafLabels.BuildAlignment();
						if (mCurrentSelectedLabel!=null)
							if(mCurrentSelectedLabel.clsLabelAttributes.level>lowest)
								mCurrentSelectedLabel=null;
					}
					else
					{
						mCurrentSelectedLabel=null;
					}

					RaiseSize(this.Height, this.Width);
				}

				if (mCurrentSelectedLabel!=null)
				{
					HighLightLabel(mCurrentSelectedLabel.clsLabelAttributes);
					RaiseSelect();
				}

				RaiseAlign();

			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		public void Clear()
		{
			pnlLabels.Controls.Clear();
			if (this.mRoot!=null)
				if (this.mRoot.branches!=null)
					this.mRoot.branches.Clear();
		}


//		public void LoadLabels (int lowIndex, int highIndex)
//		{
//
//			//stor
//			mAxisDataRange.low = lowIndex;
//			mAxisDataRange.high=highIndex;
//
//			if (mLeaves == null || mLeaves.Count == 0)
//			{
//				LoadAxis(lowIndex, highIndex);
//				return;
//			}
//
//			this.UpdateComplete = false;
//
//			mLowLabel = lowIndex;
//			if (mLowLabel >= mLeaves.Count)
//			{
//				mLowLabel = mLeaves.Count-1;
//				App.Error("Low label index above supported threshold");
//			}
//
//			mHighLabel = highIndex;
//			if (mHighLabel >= mLeaves.Count)
//			{
//				mHighLabel = mLeaves.Count-1;
//				App.Error("High label index above supported threshold");
//			}
//
//			//set axis ranges for data and visible range
//			mAxisDataRange.low =  mLowLabel;
//			mAxisDataRange.high = mHighLabel;
//			mAxisRange.low = (double)mLowLabel-.5;
//			mAxisRange.high = (double)mHighLabel+.5;
//
//			mLeafCount = mHighLabel-mLowLabel+1;
//
//			axis.Plotter.UnitPlotter.SetRange((float)mAxisRange.low, (float)mAxisRange.high);
//
//			pnlLabels.Controls.Clear();
//			ClearLabelReferences(mRoot);
//			DrawLabels(pnlLabels);
//
//			if (Range!=null)
//				Range(lowIndex, highIndex);
//
//			this.UpdateComplete = true;
//
//			return;
//		}

		private void DrawAxisOnly()
		{
			ShowAxis=true;
			axis.Width = this.Width;
			axis.Height = this.Height;

			//clear the previous selection
			RaiseUnselect();

			DrawAxis();
			
		}


		private void DrawControl (int lowIndex, int highIndex)
		{
			//this.UpdateComplete = false;

			mLowLabel = lowIndex;
			mHighLabel = highIndex;

			mLeafCount = mHighLabel-mLowLabel+1;
			
			foreach(ctlLabelItem item in pnlLabels.Controls)
			{
				if (item != null)
				{
					item.Dispose();
				}
			}
			pnlLabels.Controls.Clear();
			ClearLabelReferences(mRoot);

			DrawLabels(pnlLabels);

			if (Range!=null)
				Range(lowIndex, highIndex);

			//this.UpdateComplete = true;
		}

		public void LoadLabels (int lowIndex, int highIndex)
		{
			this.UpdateComplete = false;

			mLowLabel = lowIndex;
			mHighLabel = highIndex;
			
			//set axis ranges for data and visible range
			mAxisDataRange.low =  lowIndex;
			mAxisDataRange.high = highIndex;
			if (mAxisRange.low == double.MinValue || mAxisRange.high==double.MinValue)
			{
				mAxisRange.low = (double)mLowLabel-.5;
				mAxisRange.high = (double)mHighLabel+.5;

				//we draw the labels from the top down, so show the numerical axis from the top down
				if (this.mVertical)
				{
					axis.Plotter.IsInverted = true;
				}
			}

			DrawControl(lowIndex, highIndex);

			this.UpdateComplete = true;
		}

		public void LoadLabels (int lowIndex, int highIndex, double lowAxis, double highAxis)
		{
			mAxisRange.low =lowAxis;
			mAxisRange.high =highAxis;
			LoadLabels(lowIndex, highIndex);
		}


		/// <summary>
		/// loads the selected range of labels and displays them
		/// </summary>
		/// <param name="lowIndex"></param>
		/// <param name="highIndex"></param>
//		public void LoadAxis (int lowIndex, int highIndex)
//		{
//			this.UpdateComplete = false;
//			
//			mLowLabel = lowIndex;
//			mHighLabel = highIndex;
//
//			mLeafCount = 0;
//
//			pnlLabels.Controls.Clear();
//			ClearLabelReferences(mRoot);
//			DrawAxis();
//
//			if (Range!=null)
//				Range(lowIndex, highIndex);
//
//			this.UpdateComplete = true;
//
//			return;
//		}

		/// <summary>
		/// adds a new label as a branch on the root with the label string lbl
		/// </summary>
		/// <param name="root"></param>
		/// <param name="lbl"></param>
		/// <returns></returns>
		public clsLabelAttributes AddBranch(clsLabelAttributes root, string lbl)
		{
			return root.AddBranch(lbl);
		}

		#endregion

		#region Manipulate Labels

		private void OnLabelsMoved()
		{
			//parse new tree and build new leaf label array
			Init();

			//if we can no longer show the previous range, reflect this
			mHighLabel = Math.Min(mHighLabel, mLeaves.Count-1);

			//let the container know about the new leaf arrangement
			if (LeavesMoved!=null)
				LeavesMoved(DataTags);

			//redraw labels
			this.UpdateComplete = false;
			DrawControl(mLowLabel, mHighLabel);
			this.UpdateComplete = true;
		}

		private void InsertLabel(clsLabelAttributes dest, clsLabelAttributes src)
		{
			try
			{
				clsLabelAttributes root = (clsLabelAttributes) dest.root;
				int index = root.branches.IndexOf(dest);
				root.branches.Insert(index, src);
				src.root = root;
				
				OnLabelsMoved();
			}
			catch{}
		}
		private void DeleteLabel(clsLabelAttributes la)
		{
			try
			{
				clsLabelAttributes root = la.root as clsLabelAttributes;

				int index = root.branches.IndexOf(la);

				//move branches to this label's root
				if (la.branches!=null)
				{
					for (int i=la.branches.Count-1; i>=0; i--)
					{
						clsLabelAttributes child = la.branches[i] as clsLabelAttributes;
						root.branches.Insert(index, child);
						child.root = root;
					}
				}

				//remove label from tree
				root.branches.Remove(la);

				if (root.branches.Count==0) 
					root.branches = null;	//mark as a leaf
				
				OnLabelsMoved();
			}
			catch{}
		}

		/// <summary>
		/// sets the background color of the label pointed to by la and its branches to color
		/// </summary>
		/// <param name="la"></param>
		/// <param name="color"></param>
		private void HighLightLabelRecursive (clsLabelAttributes la, Color color)
		{
			try
			{
				if (la.label!=null)
				{
					la.label.BackColor  = color;
					la.label.Invalidate();
				}

				if (la.branches != null)
				{
					for (int i=0; i<la.branches.Count; i++)
					{
						HighLightLabelRecursive(la.branches[i] as clsLabelAttributes, color);
					}
				}
				else //this is a leaf so calc highlighted bounds
				{
					int val = 0;

					if (mVertical)
					{
						val = LeafTop(la);
						if (val<mHighlightMin)	mHighlightMin = val;
						val = LeafBottom(la)-1;
						if (val>mHighlightMax)	mHighlightMax = val;
					}
					else
					{
						val = LeafLeft(la);
						if (val<mHighlightMin)	mHighlightMin = val;
						val = LeafRight(la)-2;
						if (val>mHighlightMax)	mHighlightMax = val;
					}

					if (mHighlightMin>int.MinValue && mHighlightMin<0)
						mHighlightMin=0;
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// sets the label and branches to highlighted, determines the selected range
		/// </summary>
		/// <param name="target"></param>
		private void HighLightLabel(clsLabelAttributes target)
		{
			mHighlightMin = int.MaxValue;
			mHighlightMax = int.MinValue;

			HighLightLabelRecursive(target, mSelectedColor);
		}

		/// <summary>
		/// sets the label and branches to unselected color, unsets the selected range
		/// </summary>
		/// <param name="l"></param>
		private void UnselectLabel(ctlLabelItem l)
		{
			if (l==null) return;

			try
			{
				//can't unselect a label if it isn't selected
				if (l.BackColor == mUnselectedColor) return;

				//can't unselect child of selected parent
				clsLabelAttributes r = l.clsLabelAttributes.root as clsLabelAttributes;
				if (r.label!=null && r.label.BackColor == mSelectedColor) return;

				HighLightLabelRecursive(l.clsLabelAttributes, mUnselectedColor);

				mHighlightMin = int.MinValue;
				mHighlightMax = int.MinValue;
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// popup menu for label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_PopUp(ctlLabelItem l, System.Windows.Forms.MouseEventArgs e)
		{           
			try
			{
				mMenuLabel = l;
				clsLabelAttributes la = l.clsLabelAttributes;
				//can't zoom out if we haven't zoomed in
				l.ContextMenu.MenuItems[(int)PopUpIndices.popup_Zoomout].Visible = (mZoomHistory.Count>0);
				//can't create new leaf nodes
				l.ContextMenu.MenuItems[(int)PopUpIndices.popup_Child].Visible = false;
				l.ContextMenu.MenuItems[(int)PopUpIndices.popup_Insert].Visible = false;

				//can't delete leaf nodes
				l.ContextMenu.MenuItems[(int)PopUpIndices.popup_Delete].Visible = 
					!l.clsLabelAttributes.IsLeaf();


				l.ContextMenu.Show(l, new Point(e.X, e.Y));
				return;
			}
			catch{}
		}

		private void label_MouseHover(object sender, System.EventArgs e)
		{           
			try
			{
				ctlLabelItem l = sender as ctlLabelItem;
				l.Focus();
				mMenuLabel = l;

				if (LabelMouseHover!=null)
				{
					LabelMouseHover(l.clsLabelAttributes);
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// mousedown event for label, selects and starts drag/drop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) 
		{           
			try
			{
				ctlLabelItem l = sender as ctlLabelItem;

				if (e.Button == MouseButtons.Right)
				{
					if (MenuHandler!=null)
					{
						MenuHandler(l.clsLabelAttributes);
					}
					else
					{
						label_PopUp(l,e);
					}
					return;
				}

				UnselectLabel(mCurrentSelectedLabel);

				if (l==mCurrentSelectedLabel)
				{
					mCurrentSelectedLabel = null;
				}
				else
				{
					HighLightLabel(l.clsLabelAttributes);
					mCurrentSelectedLabel = l;
				}

				ForceSelect();

				if (mDragEnabled)
				{
					// Remember the point where the mouse down occurred. The DragSize indicates
					// the size that the mouse can move before a drag event should be started.                
					Size dragSize = SystemInformation.DragSize;

					// Create a rectangle using the DragSize, with the mouse position being
					// at the center of the rectangle.
					dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width /2),
						e.Y - (dragSize.Height /2)), dragSize);
				}

			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// mouseup event for label, ends drag/drop, raises selected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			try
			{
			
				//only handle left button up
				if (e.Button != MouseButtons.Right)
				{

					if (mDragEnabled)
					{
						// Reset the drag rectangle when the mouse button is raised.
						dragBoxFromMouseDown = Rectangle.Empty;
					}
				}

				if (LabelMouseUp!=null)
				{
					ctlLabelItem lbl = sender as ctlLabelItem;
					LabelMouseUp(lbl.clsLabelAttributes);
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		/// <summary>
		/// mousemove event for label, continues drag/drop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			if (mDragEnabled)
			{
				try
				{
					if ((e.Button & MouseButtons.Left) == MouseButtons.Left) 
					{
						ctlLabelItem l = sender as ctlLabelItem;

						// If the mouse moves outside the rectangle, start the drag.
						if (dragBoxFromMouseDown != Rectangle.Empty && 
							!dragBoxFromMouseDown.Contains(e.X, e.Y)) 
						{
							// Proceed with the drag and drop, passing in the dragged label.                    
							DragDropEffects dropEffect = l.DoDragDrop(sender, DragDropEffects.Move);
						}
					}
				}
				catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
			}
		}

		/// <summary>
		/// dragover event for label, shows dragover label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_DragOver(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			if (mDragEnabled)
			{
				try
				{
					e.Effect = DragDropEffects.Move;

					ctlLabelItem target = sender as ctlLabelItem;
					ctlLabelItem draggedLabel =e.Data.GetData(typeof(ctlLabelItem))as ctlLabelItem;

					if (ValidLabelDropTarget(draggedLabel, target))
					{
						HighLightLabelRecursive(target.clsLabelAttributes, mDragOverColor);
						mCurrentDragOverLabel = target;
					}
				}
				catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
			}
		}

		/// <summary>
		/// dragleave event for label, unselects dragover label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_DragLeave(object sender, System.EventArgs e)
		{
			if (mDragEnabled)
			{
				ctlLabelItem l = sender as ctlLabelItem;
				if (l.BackColor==mDragOverColor)
					HighLightLabelRecursive(l.clsLabelAttributes, mUnselectedColor);
			}
		
		}

		private bool ValidLabelDropTarget(ctlLabelItem draggedLabel, ctlLabelItem target)
		{
			bool retVal = true;
		
			//can't drop on itself
			if (draggedLabel==target) 
				retVal = false;
			//we can't drop a root label on a branch label
			else if (IsMyRoot(draggedLabel.clsLabelAttributes, target.clsLabelAttributes)) 
				retVal = false;

			return retVal;
		}

		/// <summary>
		/// dragdrop event for label, raises Moved event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (mDragEnabled)
			{
				try
				{
					// Reset the drag rectangle
					dragBoxFromMouseDown = Rectangle.Empty;

					ctlLabelItem draggedLabel =e.Data.GetData(typeof(ctlLabelItem))as ctlLabelItem;
					if (draggedLabel!=null) 
					{

						mCurrentDragOverLabel = null;
						//ctlLabelItem draggedLabel =e.Data.GetData(typeof(ctlLabelItem))as ctlLabelItem;
						ctlLabelItem target = sender as ctlLabelItem;

						if (ValidLabelDropTarget(draggedLabel, target))
						{	
							MoveLabel(draggedLabel, target);
							mCurrentSelectedLabel = draggedLabel;
						}				
					}
				}
				catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
			}
		}

		/// <summary>
		/// insert label in front of dest label
		/// </summary>
		/// <param name="lbl"></param>
		/// <param name="dest"></param>
		private void MoveLabel(ctlLabelItem lbl, ctlLabelItem dest)
		{
			try
			{
				//RaiseMove(lbl, dest);

				//remove from original root
				clsLabelAttributes root = (clsLabelAttributes) lbl.clsLabelAttributes.root;
				root.branches.Remove(lbl.clsLabelAttributes);
				if (root.branches.Count==0)
				{
					root.branches = null;
				}

				InsertLabel(dest.clsLabelAttributes, lbl.clsLabelAttributes);
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

//		/// <summary>
//		/// parses the source and destination labels for a move and calculates the moved
//		/// range and the destination index.  Raises the Moved event
//		/// </summary>
//		/// <param name="lbl"></param>
//		/// <param name="dest"></param>
//		private void RaiseMove(ctlLabelItem lbl, ctlLabelItem dest)
//		{
//			int lowIndex = 0;
//			int highIndex=0;
//			int pos = 0;
//
//			try
//			{
//				//find the range to be moved
//				if (lbl.clsLabelAttributes.branches == null)
//				{
//					lowIndex=lbl.clsLabelAttributes.leafIndex;
//					highIndex=lowIndex;
//				}
//				else  //low index is leftmost leaf, high index is rightmost leaf
//				{
//					clsLabelAttributes root = lbl.clsLabelAttributes;
//					lowIndex = root.LowLeaf().leafIndex;
//					highIndex = root.HighLeaf().leafIndex;
//				}
//
//				//find the destination index to move the range to
//				if (dest.clsLabelAttributes.branches == null)
//				{
//					pos=dest.clsLabelAttributes.leafIndex;
//				}
//				else  //pos index is leftmost leaf
//				{
//					clsLabelAttributes root = dest.clsLabelAttributes;
//					while (root.branches!=null)
//					{
//						root = root.branches[0] as clsLabelAttributes;
//					}
//					pos = root.leafIndex;
//				}
//
//				if (MovedRange!=null)
//					MovedRange(lowIndex, highIndex, pos);
//			}
//			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
//		}

		/// <summary>
		/// determines if a label is a parent of a label.  
		/// don't want to move a root to one of it's branches
		/// </summary>
		/// <param name="root"></param>
		/// <param name="branch"></param>
		/// <returns></returns>
		private bool IsMyRoot (clsLabelAttributes root, clsLabelAttributes branch)
		{
			try
			{
				clsLabelAttributes r = branch.root as clsLabelAttributes;

				while (r!=null)
				{
					if (r==root)
					{
						return true;
					}
					r=r.root as clsLabelAttributes;
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}

			return false;
		}

		/// <summary>
		/// initializes control after labels are loaded.  Must be called before LoadLabels.
		/// </summary>
		public void Init()
		{
			mAxisRange = new AxisRangeF(double.MinValue, double.MinValue);
			nLevels = 0;
			mLeaves = new clsLabelCollection(pnlLabels, mVertical);
			mRoot.level = 0;
			ParseLeaves(mRoot);
		}
		#endregion

		#region Manipulate Numeric Axis

		/// <summary>
		/// calculates the label index for a zoom position pos
		/// </summary>
		/// <param name="pos"> pixel position to be mapped </param>
		/// <param name="posMin"> low pixel </param>
		/// <param name="posMax">high pixel</param>
		/// <param name="boundMin"> low label index </param>
		/// <param name="boundMax">high label index </param>
		/// <returns> label index </returns>
		private int CalcZoomBound (int pos, int posMin, int posMax, int boundMin, int boundMax)
		{
			try
			{
				float denominator = (float) (posMax - posMin + 1);
				if (denominator==0f) return 0;
				float frac = (float) pos / denominator;
				if (frac<=0.0)
					return (boundMin);
				if (frac>=1.0)
					return (boundMax);
				float range = (float)(boundMax-boundMin+1);
				return ((int)(frac*range)+boundMin);
			}
			catch(Exception e)
			{
				//System.Windows.Forms.MessageBox.Show(e.Message);
				return 0;
			}
		}

		/// <summary>
		/// wraps the unzoom handler
		/// </summary>
		public void AxisZoomOut()
		{
			HandleZoomOut(null, null);
		}

		private void AddParent (clsLabelAttributes currentParent, string txt, ArrayList children, Range r)
		{
			try
			{
				clsLabelAttributes newParent = AddBranch(currentParent, txt);
				newParent.AddChildren(children, r);
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void AddChild(clsLabelAttributes parent, string txt)
		{
			try
			{
				AddBranch(parent, txt);
				OnLabelsMoved();
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void AddLevel(clsLabelAttributes la)
		{
			try
			{
				ctlLabelItem l = mMenuLabel;

				clsLabelAttributes currentParent = la.root as clsLabelAttributes;
				ArrayList branches = currentParent.branches;
				currentParent.branches = new ArrayList();

				int thisIndex = branches.IndexOf(la);
				
				if (currentParent.level==0) //is root
				{
					AddParent(currentParent, "1", branches, new Range(0,0,branches.Count-1,0));
				}
				else
				{
					if (thisIndex>0)
						AddParent(currentParent, "1", branches, new Range(0,0,thisIndex-1,0));
					AddParent(currentParent, "2", branches, new Range(thisIndex,0,thisIndex,0));
					if (thisIndex+1<branches.Count)
						AddParent(currentParent, "3", branches, new Range(thisIndex+1,0,branches.Count-1,0));
				}

				OnLabelsMoved();
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void HandleSelect(int low, int high)
		{
			if (Selected!=null)
				Selected(low, high);
			if (mVertical)
			{
				axis.SelectedVertical(low, high);
			}
			else
			{
				axis.SelectedHorizontal(low, high);
			}

			if (ShowAxis)
			{
				axis.UpdateAxis();
			}
		}

		private void RaiseUnselect()
		{
			HandleSelect(int.MinValue, int.MinValue);
		}

		private void RaiseSelect()
		{
			HandleSelect(mHighlightMin, mHighlightMax);
		}

		private void ForceSelect()
		{
			HandleSelect(mHighlightMin, mHighlightMax);
			if (Selected!=null)
			{				
				this.UpdateComplete = true;
			}
		}

		private void HandleZoomOut(object sender, System.EventArgs e)
		{
			try
			{
				if (mZoomHistory.Count>0)
				{
					Point p = (Point) mZoomHistory[mZoomHistory.Count-1];
					mZoomHistory.Remove(p);

					RaiseUnselect();

					this.UpdateComplete = false;
					DrawControl(p.X, p.Y);
					this.UpdateComplete = true;
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlLabels = new System.Windows.Forms.Panel();
			this.vScroll = new System.Windows.Forms.VScrollBar();
			this.hScroll = new System.Windows.Forms.HScrollBar();
			this.axis = new PNNLControls.ctlSingleAxis();
			this.SuspendLayout();
			// 
			// pnlLabels
			// 
			this.pnlLabels.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlLabels.Location = new System.Drawing.Point(16, 0);
			this.pnlLabels.Name = "pnlLabels";
			this.pnlLabels.Size = new System.Drawing.Size(192, 328);
			this.pnlLabels.TabIndex = 0;
			// 
			// vScroll
			// 
			this.vScroll.Dock = System.Windows.Forms.DockStyle.Left;
			this.vScroll.Location = new System.Drawing.Point(0, 0);
			this.vScroll.Name = "vScroll";
			this.vScroll.Size = new System.Drawing.Size(16, 344);
			this.vScroll.TabIndex = 2;
			this.vScroll.Visible = false;
			this.vScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScroll_Scroll);
			// 
			// hScroll
			// 
			this.hScroll.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScroll.Location = new System.Drawing.Point(16, 328);
			this.hScroll.Name = "hScroll";
			this.hScroll.Size = new System.Drawing.Size(440, 16);
			this.hScroll.TabIndex = 3;
			this.hScroll.Visible = false;
			this.hScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScroll_Scroll);
			// 
			// axis
			// 
			this.axis.Alignment = null;			
			this.axis.Dock = System.Windows.Forms.DockStyle.Right;
			this.axis.Location = new System.Drawing.Point(384, 0);
			this.axis.Name = "axis";
			this.axis.Size = new System.Drawing.Size(72, 328);
			this.axis.TabIndex = 4;
			// 
			// ctlHierarchalLabel
			// 
			this.Controls.Add(this.axis);
			this.Controls.Add(this.pnlLabels);
			this.Controls.Add(this.hScroll);
			this.Controls.Add(this.vScroll);
			this.Name = "ctlHierarchalLabel";
			this.Size = new System.Drawing.Size(456, 344);
			this.Resize += new System.EventHandler(this.ctlHierarchalLabel_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		private void ctlHierarchalLabel_Resize(object sender, System.EventArgs e)
		{
			if (mbln_overrideResize == false)
			{
				RedrawLabel();
			}
		}

		public bool OverrideResize
		{
			get
			{
				return mbln_overrideResize;
			}
			set
			{
				mbln_overrideResize = value;
			}
		}

		public void RedrawLabel()
		{
			try
			{
				//UpdateComplete = false;
				//DrawLabels(pnlLabels);
				DrawControl(this.mLowLabel, this.mHighLabel);
				Invalidate();
				//UpdateComplete = true;
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		private void hScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.NewValue<0) 
				return;

			//snap the scroll value to the closest label edge
			pnlLabels.Left = -mScrollLabels.SnapToAlignment(e.NewValue);

			if(ScrollPosition!=null)
				ScrollPosition(pnlLabels.Left);
		}

		private void vScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.NewValue<0) 
				return;

			//snap the scroll value to the closest label edge
			pnlLabels.Top = -mScrollLabels.SnapToAlignment(e.NewValue);

			if(ScrollPosition!=null)
				ScrollPosition(pnlLabels.Top);
		}
		#region ICloneable Members

		private void CloneRecursive(clsLabelAttributes dest, clsLabelAttributes src)
		{
			try
			{
				if (src.branches == null) //is a leaf
				{
					return;
				}
				else  //parse branches
				{
					dest.branches = new ArrayList();
					for (int i=0; i<src.branches.Count; i++)
					{
						clsLabelAttributes srcBranch = src.branches[i] as clsLabelAttributes;
						clsLabelAttributes destBranch = srcBranch.Clone() as clsLabelAttributes;
						dest.branches.Add(destBranch);
						destBranch.root = dest;
						CloneRecursive(destBranch, srcBranch);
					}
				}
			}
			catch(Exception ex){}//System.Windows.Forms.MessageBox.Show(ex.Message);}
		}

		public object Clone()
		{
			// TODO:  Add ctlHierarchalLabel.Clone implementation

			clsLabelAttributes newRoot = mRoot.Clone() as clsLabelAttributes;

			CloneRecursive(newRoot, mRoot);

			return newRoot;
		}

		#endregion
	}
}