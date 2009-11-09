using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using PNNLControls;
using System.Reflection;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlFileView.
	/// </summary>
	public class ctlFileView : System.Windows.Forms.TreeView 
	{
		private static readonly int DEFAULT_CATEGORY_ICON_INDEX = 0;
		private static readonly int DEFAULT_FORM_ICON_INDEX = 1;
		private static readonly int DEFAULT_DETAIL_ICON_INDEX = 2;

		/// <summary>
		/// The mediator that this file view is attached to, from which the 
		/// WindowOpened and WindowClosed events are recieved.
		/// </summary>
		private ICategorizedItemNotifier mNotifier;

		/// <summary>
		/// Handler for WindowOpen events.
		/// </summary>
		private PNNLControls.ItemChangedHandler mItemOpenHandler;

		/// <summary>
		/// Handler for WindowClose events.
		/// </summary>
		private PNNLControls.ItemChangedHandler mItemCloseHandler;

		/// <summary>
		/// Handler for CategorizedInfoChanged event of windows being managed.
		/// </summary>
		private EventHandler mCategorizedInfoChangedHandler;

		/// <summary>
		/// Maps forms to the current TreeNode associated with them.  Consists of 
		/// &lt;ICategorizedForm, TreeNode&gt; pairs.
		/// </summary>
		private Hashtable mManagedForms = new Hashtable();

		/// <summary>
		/// Maps icons for items to an index in the iconList.  Since images can not be set for 
		/// individual TreeNodes, we have to use the ImageList and ImageIndex properties.  But the 
		/// ImageList always returns -1 if you try to find the index of image, so we have to keep 
		/// track of the indexes ourselves.  Consists of &lt;Icon, int&gt; pairs.
		/// </summary>
		private Hashtable mIconIndexes = new Hashtable();

		/// <summary>
		/// Counts of the number of times a category is used (can include # of subitems 
		/// + special stuff).  When the number reaches 0, the category is removed from the 
		/// view.  Consists of &lt;TreeNode, int&gt; pairs.
		/// </summary>
		private Hashtable mCategoryCounts = new Hashtable();

		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.ImageList mIconList;
		private System.Windows.Forms.ContextMenu mDefaultCategoryContextMenu;
		private System.Windows.Forms.ContextMenu mDefaultFormContextMenu;
		private System.Windows.Forms.ContextMenu mDefaultDetailContextMenu;
		private System.ComponentModel.IContainer components;

		public ctlFileView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			mItemOpenHandler = new ItemChangedHandler(this.NotifierItemOpened);
			mItemCloseHandler = new ItemChangedHandler(this.NotifierItemClosed);
			mCategorizedInfoChangedHandler = new EventHandler(this.WindowCategorizedInfoChanged);
			this.contextMenu1.MenuItems.Add("Context Menu");
			this.ContextMenu = contextMenu1;

			InitializeDefaultMenus();

			// Load up default icons
			Assembly assembly = Assembly.GetExecutingAssembly();
			this.mIconList.Images.Clear();
			// Get the shell's icon for a folder (or more specifically, the folder this program 
			// was launched from
			this.mIconList.Images.Add(IconUtils.GetIconForFile(@".\", IconSize.Small, false, true, false));
			// Load default form and detail icons from embedded icons
			this.mIconList.Images.Add(new Icon(assembly.GetManifestResourceStream(typeof(ctlFileView), "Icons.Form.ico")));
			this.mIconList.Images.Add(new Icon(assembly.GetManifestResourceStream(typeof(ctlFileView), "Icons.Detail.ico")));
		}

		private void InitializeDefaultMenus() 
		{
			this.mDefaultCategoryContextMenu.MenuItems.Add(new MenuItem("Close All"));
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
				if (mNotifier != null) 
				{
					mNotifier.ItemOpen -= mItemOpenHandler;
					mNotifier.ItemClose -= mItemCloseHandler;
					mIconIndexes.Clear();
					mManagedForms.Clear();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ctlFileView));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.mIconList = new System.Windows.Forms.ImageList(this.components);
			this.mDefaultCategoryContextMenu = new System.Windows.Forms.ContextMenu();
			this.mDefaultFormContextMenu = new System.Windows.Forms.ContextMenu();
			this.mDefaultDetailContextMenu = new System.Windows.Forms.ContextMenu();
			// 
			// contextMenu1
			// 
			this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
			// 
			// mIconList
			// 
			this.mIconList.ImageSize = new System.Drawing.Size(16, 16);
			this.mIconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mIconList.ImageStream")));
			this.mIconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// mDefaultFormContextMenu
			// 
			this.mDefaultFormContextMenu.Popup += new System.EventHandler(this.mDefaultFormContextMenu_Popup);
			// 
			// ctlFileView
			// 
			this.ImageIndex = 0;
			this.ImageList = this.mIconList;
			this.SelectedImageIndex = 0;

		}
		#endregion

		#region Properties
		// These properties are a pain to have visible in the Visual Studio designer, because it will 
		// always attempt to assign them values, never allowing the defaults to come through.
		// So just hide them from the designer, if someone wants to use them, they can be used 
		// programmatically
		[System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.ComponentModel.Browsable(false)]
		public Image CategoryImage
		{
			get 
			{
				return this.mIconList.Images[DEFAULT_CATEGORY_ICON_INDEX];
			}
			set 
			{
				this.mIconList.Images[DEFAULT_CATEGORY_ICON_INDEX] = value;
			}
		}

		[System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.ComponentModel.Browsable(false)]
		public Image FormImage 
		{
			get 
			{
				return this.mIconList.Images[DEFAULT_FORM_ICON_INDEX];
			}
			set 
			{
				this.mIconList.Images[DEFAULT_FORM_ICON_INDEX] = value;
			}
		}

		[System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.ComponentModel.Browsable(false)]
		public Image DetailImage 
		{
			get 
			{
				return this.mIconList.Images[DEFAULT_DETAIL_ICON_INDEX];
			}
			set 
			{
				this.mIconList.Images[DEFAULT_DETAIL_ICON_INDEX] = value;
			}
		}

		#endregion

		#region Event Handlers
		/// <summary>
		/// Attaches this open file viewer to the given notifier.  If the viewer is 
		/// already attached to a notifier, an InvalidOperationException will be thrown.
		/// This is because it's easier to support only one attachment operation, and 
		/// that's all that Decon2LS needs.
		/// </summary>
		/// <param name="mediator"></param>
		public void AttachTo(ICategorizedItemNotifier notifier) 
		{
			if (notifier == null) 
			{
				throw new ArgumentNullException("notifier");
			}
			if (mNotifier != null) 
			{
				throw new InvalidOperationException("ctlFileView is already attached.");	
			}
			this.mNotifier = notifier;

			// attach handlers to mediator events
			mNotifier.ItemOpen += mItemOpenHandler;
			mNotifier.ItemClose += mItemCloseHandler;
		}

		/// <summary>
		/// Handles WindowOpened signal
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void NotifierItemClosed(object sender, ItemChangedEventArgs args)
		{
			//Console.WriteLine("Form Closed {0}", args.Form);
			// If we're showing info for the form, remove it
			if (this.mManagedForms.Contains(args.Item)) 
			{
				this.RemoveFormFromDisplay(args.Item, true);
			}
		}

		/// <summary>
		/// Handles WindowClosed signal
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void NotifierItemOpened(object sender, ItemChangedEventArgs args)
		{
			// Console.WriteLine("Form Opened {0}", args.Form);
			// If it implements ICategorizedForm, pay attention to it, 
			// otherwise ignore it
//			if (args.Form != null && args.Form is PNNLControls.ICategorizedForm) 
//			{
				// Check if we're already showing this form, the open event could be bogus
				if (this.mManagedForms.Contains(args.Item)) 
				{
					return;
				}
				// otherwise add it
				this.AddFormToDisplay(args.Item, true);
//			}
		}

		/// <summary>
		/// Handles changing of categorized info signal for any managed form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void WindowCategorizedInfoChanged(object sender, EventArgs args) 
		{
			this.UpdateFormInfo((ICategorizedItem) sender, true);
		}
		#endregion

		#region Tree Modifiers

		public void AddCategory(CategoryInfo[] category) 
		{
			if (category == null) 
			{
				throw new ArgumentNullException("category");
			}
			AddCategory(category, true);
		}

		/// <summary>
		/// Adds the category to the view, (adding any nodes if needed)
		/// </summary>
		/// <param name="category"></param>
		/// <param name="beginEndUpdate"></param>
		/// <returns>The node in the view associated with the final entry in the 
		/// category array.</returns>
		private TreeNode AddCategory(CategoryInfo[] category, bool beginEndUpdate) 
		{
			TreeNode currentNode = null;
			try 
			{
				if (beginEndUpdate) 
				{
					this.BeginUpdate();
				}
				TreeNodeCollection nodes = this.Nodes;
				// Loop through category, adding treenodes if necessary.
				for (int i = 0; i < category.Length; i++) 
				{
					CategoryInfo currentCategory = category[i];
					bool found = false;
					// Loop through nodes in the current collection.  If the node is found, 
					// break and start looking for the next entry in the category array.
					foreach(TreeNode node in nodes) 
					{
						if (node.Tag != null && node.Tag.Equals(currentCategory)) 
						{
							currentNode = node;
							nodes = node.Nodes;
							found = true;
							// update count of how many times this category is used
							mCategoryCounts[node] = ((int) mCategoryCounts[node]) + 1;
							break;
						}
					}
					if (!found) 
					{
						Console.WriteLine("Add Category: " + currentCategory.Text);
						// If we get here, a node for the category entry has not been found, 
						// so we need to create one.  Once we start doing this, every loop 
						// will get to this point.
						currentNode = SetCategoryNodeFeatures(new TreeNode(), currentCategory);
						currentNode.Tag = currentCategory;
						nodes.Add(currentNode);
						nodes = currentNode.Nodes;
						mCategoryCounts.Add(currentNode, 1);
					}
				}
			} 
			finally 
			{
				if (beginEndUpdate) 
				{
					this.EndUpdate();
				}
			}
			return currentNode;
		}

		/// <summary>
		/// Adds the form into the display, creating new nodes for the category (if necessary), 
		/// a new node for the form, and nodes for any details.
		/// </summary>
		/// <param name="form"></param>
		private void AddFormToDisplay(ICategorizedItem item, bool beginEndUpdate) 
		{
			try 
			{
				if (beginEndUpdate) 
				{
					this.BeginUpdate();
				}
				TreeNode categoryEndNode = this.AddCategory(item.CategorizedInfo.Category, false);

				// Create a new node for the form
				TreeNode formNode = SetFormNodeFeatures(new TreeNode(), item.CategorizedInfo.Info);
				formNode.Tag = item.CategorizedInfo.Info;
				categoryEndNode.Nodes.Add(formNode);
				mCategoryCounts.Add(formNode, 1);

				// And add the newly created node into the mapping
				this.mManagedForms.Add(item, formNode);

				// Create nodes for the details
				this.SetDetails(item, formNode);

				// Add change handler to form
				item.CategorizedInfoChanged += this.mCategorizedInfoChangedHandler;
			}
			finally 
			{
				if (beginEndUpdate) 
				{
					this.EndUpdate();
				}
			}
		}

		private void UpdateFormInfo(ICategorizedItem item, bool beginEndUpdate) 
		{
			try 
			{
				if (beginEndUpdate) 
				{
					this.BeginUpdate();
				}

				// Get the current Node for the form
				TreeNode currentNode = (TreeNode) this.mManagedForms[item];
				bool sameCategory = true;
				TreeNode node = currentNode.Parent;

				// Check if we have the corrent category for the form, if not 
				// do the easiest thing, which is to remove the form from the list, then
				// add it back in again.
				if (Depth(node) != item.CategorizedInfo.Category.Length) 
				{
					sameCategory = false;
				} 
				else 
				{
					for (int i = item.CategorizedInfo.Category.Length - 1; i >= 0; i--) 
					{
						CategoryInfo proposedCategory = item.CategorizedInfo.Category[i];
						CategoryInfo currentCategory = (CategoryInfo) node.Tag;
						//Console.WriteLine("Comparing categories {0} {1}", proposedCategory.Text, currentCategory.Text);
						if (!proposedCategory.Equals(currentCategory)) 
						{
							sameCategory = false;
							break;
						}
						node = node.Parent;
					}
				}

				if (!sameCategory) 
				{
					//Console.WriteLine("Changing category");
					this.RemoveFormFromDisplay(item, false);
					this.AddFormToDisplay(item, false);
				} 
				else 
				{
					SetFormNodeFeatures(currentNode, item.CategorizedInfo.Info);
					bool expanded = currentNode.IsExpanded;
					currentNode.Nodes.Clear();
					SetDetails(item, currentNode);
					// Reexpand the node if was expanded before.  Clearing it always sets 
					// it back to the unexpanded state.
					if (expanded) 
					{
						currentNode.Expand();
					}
				}
			}
			finally 
			{
				if (beginEndUpdate) 
				{
					this.EndUpdate();
				}
			}
		}

		/// <summary>
		/// Creates the detail nodes for a form in the tree.
		/// </summary>
		/// <param name="form"></param>
		/// <param name="node"></param>
		private void SetDetails(ICategorizedItem item, TreeNode node) 
		{
			foreach (DetailInfo detail in item.CategorizedInfo.Details) 
			{
				TreeNode detailNode = SetDetailNodeFeatures(new TreeNode(), detail);
				detailNode.Tag = detail;
				node.Nodes.Add(detailNode);
			}
		}

		/// <summary>
		/// Helper function.
		/// Starting at the given node, removes the node if it has no children and 
		/// progresses up the tree.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="level"></param>
		private void RemoveNodeLevels(TreeNode node, int level) 
		{
			for (int i = 0; i < level; i++) 
			{
				if (((int) mCategoryCounts[node]) > 0) 
				{
					break;
				}
				mCategoryCounts.Remove(node);
				TreeNode parentNode = node.Parent;
				// remove the node;
				node.Remove();	
				node = parentNode;
				if (node != null) 
				{
					mCategoryCounts[node] = ((int) mCategoryCounts[node]) - 1;
				}
			}
		}
		/// <summary>
		/// Removes the form from the display, removing parent nodes if they become 
		/// unneeded.
		/// </summary>
		/// <param name="form"></param>
		private void RemoveFormFromDisplay(ICategorizedItem item, bool beginEndUpdate) 
		{
			try 
			{
				if (beginEndUpdate) 
				{
					this.BeginUpdate();
				}
				// Remove change handler
				item.CategorizedInfoChanged -= this.mCategorizedInfoChangedHandler;

				// Get the TreeNode that's currently associated with the form
				TreeNode node = (TreeNode) this.mManagedForms[item];
				mCategoryCounts[node] = 0;

				// Remove all detail nodes from it
				node.Nodes.Clear();
			
				// Remove parent nodes if necessary.
				RemoveNodeLevels(node, Depth(node));
			
				this.mManagedForms.Remove(item);
			} 
			finally 
			{
				if (beginEndUpdate) 
				{
					this.EndUpdate();
				}
			}
		}
		#endregion

		#region Helpers
		/// <summary>
		/// Gets the index of an icon in the icons list.  If the icon is not in the list, 
		/// the icon is added before returning its index.
		/// </summary>
		/// <param name="icon"></param>
		/// <returns></returns>
		private int GetIconIndex(Icon icon, int defaultIndex) 
		{
			// return the default if null
			if (icon == null) 
			{
				return defaultIndex;
			}
			if (mIconIndexes.Contains(icon)) 
			{
				// If we've previously added this icon to the list, retrieve the 
				// index from the mapping we have to keep on the side
				return (int) mIconIndexes[icon];
			}
			lock (this) 
			{
				// Otherwise, add it to the icon list, put the returned index in the mapping and return it
				mIconList.Images.Add(icon);
				// Since the index isn't returned by the call (and using the IList version of Add fails), 
				// just assume that the index will be the last item in the list.  Synchronized to make 
				// sure no other thread will attempt to add an icon at the same time.
				int index = mIconList.Images.Count - 1;
				mIconIndexes.Add(icon, index);
				return index;
			}
		}

		/// <summary>
		/// Sets the image, selected image and text associated with a tree node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="info"></param>
		/// <param name="defaultIconIndex"></param>
		/// <returns>The input node.</returns>
		private TreeNode SetNodeFeatures(TreeNode node, CategorizedInfo info, int defaultIconIndex) 
		{
			node.Text = info.Text;
			node.ImageIndex = GetIconIndex(info.Icon, defaultIconIndex);
			node.SelectedImageIndex = GetIconIndex(info.Icon, defaultIconIndex);
			return node;
		}

		private TreeNode SetFormNodeFeatures(TreeNode node, CategorizedInfo info) 
		{
			return SetNodeFeatures(node, info, DEFAULT_FORM_ICON_INDEX);
		}

		private TreeNode SetCategoryNodeFeatures(TreeNode node, CategoryInfo info) 
		{
			return SetNodeFeatures(node, info, DEFAULT_CATEGORY_ICON_INDEX);
		}

		private TreeNode SetDetailNodeFeatures(TreeNode node, DetailInfo info) 
		{
			return SetNodeFeatures(node, info, DEFAULT_DETAIL_ICON_INDEX);
		}

		private int Depth(TreeNode node) 
		{
			if (node != null) 
			{
				return 1 + Depth(node.Parent);
			}
			return 0;
		}
		#endregion

		private void contextMenu1_Popup(object sender, System.EventArgs e)
		{
			Console.WriteLine("Context Menu Shown Node: " + this.SelectedNode);
		}

		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			base.OnBeforeSelect (e);
			Console.WriteLine("Before Select {0}", e.Node.Text);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			Console.WriteLine("After Select {0}", e.Node.Text);
		}

		private const int WM_EXITMENULOOP = 0x0212;

		protected override void WndProc(ref Message m)
		{
			//Console.WriteLine("Message {0} {1} {2} {3}", m.HWnd, m.Msg.ToString("x4"), m.LParam, m.WParam);
			if (m.Msg == WM_EXITMENULOOP) 
			{
				Console.WriteLine("Exited Context Menu");
				m.Result = IntPtr.Zero;
			}
			base.WndProc (ref m);
		}

		private void mDefaultFormContextMenu_Popup(object sender, System.EventArgs e)
		{
		
		}

	}
}