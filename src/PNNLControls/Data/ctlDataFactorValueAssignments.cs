using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNL.Controls
{
	/// <summary>
	/// Summary description for ctlDataFactorValueAssignment.
	/// </summary>
	public class ctlDataFactorValueAssignment : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel panelFactors;
		private System.Windows.Forms.ImageList dataImages;
		private System.ComponentModel.IContainer components;
		private System.Collections.ArrayList m_factorBoxes;
		private System.Windows.Forms.ListView m_sourceList;
		private System.Collections.Hashtable m_dataHash;
		private ListView  m_focusList;
		private ArrayList m_factorPanels;
		private const int MAX_BOXES_PER_PANEL = 5;
		
		public ctlDataFactorValueAssignment()
		{			
			InitializeComponent();
			InitControl();
		}

		private ContextMenu CreateContextMenu()
		{
			/*ContextMenu menu		= new ContextMenu();
			MenuItem item_detail	= new MenuItem("Detailed View", new System.EventHandler(Detail_Click));
			MenuItem item_large		= new MenuItem("Large Item View", new System.EventHandler(LargeView_Click));
			menu.MenuItems.Add(item_detail);
			menu.MenuItems.Add(item_large);
			return menu;*/
			return null;
		}

		public void InitControl()
		{
			m_factorBoxes = new ArrayList();
			m_dataHash = new Hashtable();
			m_factorPanels = new ArrayList();
			Resize += new EventHandler(ctl_Resize);
			//ContextMenu menu			= CreateContextMenu();
			m_factorPanels.Add(panelFactors);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ctlDataFactorValueAssignment));
			this.panelFactors = new System.Windows.Forms.Panel();
			this.dataImages = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// panelFactors
			// 
			this.panelFactors.AutoScroll = true;
			this.panelFactors.AutoScrollMinSize = new System.Drawing.Size(16, 58);
			this.panelFactors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelFactors.Location = new System.Drawing.Point(0, 0);
			this.panelFactors.Name = "panelFactors";
			this.panelFactors.Size = new System.Drawing.Size(672, 680);
			this.panelFactors.TabIndex = 6;
			// 
			// dataImages
			// 
			this.dataImages.ImageSize = new System.Drawing.Size(16, 16);
			this.dataImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dataImages.ImageStream")));
			this.dataImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ctlDataFactorValueAssignment
			// 
			this.Controls.Add(this.panelFactors);
			this.Name = "ctlDataFactorValueAssignment";
			this.Size = new System.Drawing.Size(672, 680);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Add a factor value to the list of boxes
		/// </summary>
		/// <param name="factorValue">values for factors to add</param>
		public void AddFactorValue(string factorValueName)
		{
			ListView list	= new ListView();			
			ContextMenu menu	= CreateContextMenu();
			list.Columns.Add(factorValueName,-2,System.Windows.Forms.HorizontalAlignment.Left);
			list.Columns[0].Width = list.Width;
			list.Dock				= DockStyle.None;	
			list.View				= View.Details;			
			list.Name		= factorValueName;			
			list.AllowDrop  = true;
			list.MouseEnter+= new EventHandler(ListMouseEnter);
			list.MouseDown += new MouseEventHandler(ListMouseDown);
			list.MouseMove += new MouseEventHandler(ListMouseMove);
			list.DragDrop  += new DragEventHandler(ListDragDrop);	
			list.DragEnter += new DragEventHandler(ListDragEnter);
			list.Resize    += new EventHandler(ListResize);
			list.MouseUp   += new MouseEventHandler(ListMouseUp);
			list.ContextMenu	= menu;			
			panelFactors.Controls.Add(list);
			m_factorBoxes.Add(list);
			m_focusList = list;
			ctl_Resize(null, null);
		}

		public string GetDatasetFactorValue(string datasetName)
		{
			return m_dataHash[datasetName] as string;
		}

		/// <summary>
		/// Add a data set to the factor value assignment.
		/// </summary>
		/// <param name="datasetName"></param>
		/// <param name="factorName"></param>
		public void AddDataset(string datasetName, string factorName)
		{
			if (m_factorBoxes.Count <= 0)
				return;

			ListViewItem item = new ListViewItem();
			m_dataHash[datasetName] = string.Empty;
			item.Text		  = datasetName; 
			item.ImageIndex   = 0; 

			ListView list = m_factorBoxes[0] as ListView; 
			foreach(Control c in this.panelFactors.Controls)
			{
				ListView listSet = c as ListView;
				if (listSet != null)
				{
					if (listSet.Name == factorName)
					{
						m_dataHash[datasetName] = factorName;
						list = listSet;
						break;
					}
				}
			}
			list.Items.Add(item);				
		}
		bool m_dragging  = false;

		/// <summary>
		/// List Mouse Down event handler.  Starts drag and drop when list done.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
				return;
			m_dragging = true;
		}

		/// <summary>
		/// List Mouse Down event handler.  Starts drag and drop when list done.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListMouseMove(object sender, MouseEventArgs e)
		{
			if (m_dragging == false)
				return;

			if (e.Button != System.Windows.Forms.MouseButtons.Left)
				return;

			if (sender == null)
				return;
		

			ListView list = sender as ListView;
			if (list.SelectedItems == null || list.SelectedItems.Count <= 0)
				return;

			m_sourceList = list;
			foreach(ListViewItem item in list.SelectedItems)
			{
				list.DoDragDrop(item, DragDropEffects.Move);
			}			
			MouseEventArgs newArgs = new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, e.X, e.Y, 0);
			ListMouseDown(list, newArgs);
		}

		private void ListDragDrop(object sender, DragEventArgs e)
		{
			ListView list = sender as ListView;			
			if (list != m_sourceList && m_sourceList != null)
			{

				ListViewItem sourceItem = new ListViewItem();
				sourceItem = e.Data.GetData(sourceItem.GetType()) as ListViewItem;
				if (sourceItem == null)
					return;

				ListViewItem destItem;
				destItem = sourceItem.Clone() as ListViewItem;
				list.Items.Add(destItem);	
				sourceItem.Remove();
				m_sourceList.HideSelection = true;
				m_dataHash[destItem.Text] = list.Name;
			}
		}
		
		/// <summary>
		/// Handles the list drag enter event when an item is dragged over a list view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListDragEnter(object sender, DragEventArgs e)
		{
			if (e.AllowedEffect == DragDropEffects.Move)
				e.Effect = DragDropEffects.Move | DragDropEffects.Scroll;
			m_focusList = sender as ListView;
			Panel_Resize(sender, null);
		}

		private void ListResize(object sender, EventArgs e)
		{
			ListView list = sender as ListView;
			list.Columns[0].Width = list.Width;
		}

		private void ctl_Resize(object sender, EventArgs e)
		{
			this.panelFactors.Left	 = 0;
			this.panelFactors.Height = Height;
			this.panelFactors.Width  = Width;
			this.panelFactors.Top	 = 0;
			this.Panel_Resize(panelFactors, e);
		}

		private void Detail_Click(object sender, EventArgs e)
		{			
			MenuItem item		= sender as MenuItem;
			ContextMenu menu	= item.Parent as ContextMenu;
			ListView list		= menu.SourceControl as ListView;
			
			if (list != null)
			{
				list.View = View.Details;
			}
		}

		private void LargeView_Click(object sender, EventArgs e)
		{
			MenuItem item		= sender as MenuItem;
			ContextMenu menu	= item.Parent as ContextMenu;
			ListView list		= menu.SourceControl as ListView;

			if (list != null)
			{
				list.View = View.LargeIcon;
			}
		}

		private void ListMouseUp(object sender, MouseEventArgs e)
		{
			ListView list = sender as ListView;
			m_dragging = false;
		}

		private void Panel_Resize(object sender, EventArgs e)
		{
			int boxes  = m_factorBoxes.Count;
			if (boxes <= 0)
				return;

			int minBoxSize		= 100;
			int focusBoxSize	= 200;
			int height		 = Math.Max(panelFactors.Height / boxes, minBoxSize);
			int shrinkHeight = this.panelFactors.Height / boxes;
 
			int count  = 0;
			bool useShrink = false;
			/// 
			/// Use the shrink height if we have more than one box and 
			/// we the number of boxes times the minimum height is greater than the controls height
			/// 
			if (height * boxes > Height && boxes > 1)
			{	
				useShrink = true;				
				shrinkHeight = (panelFactors.Height - focusBoxSize) / (boxes - 1);
			}

			int top = 0;
			int useHeight = 0;
			foreach(ListView list in m_factorBoxes)
			{
				list.Dock	= DockStyle.None;
				list.Top    = top; 
				if (useShrink == true)
				{
					if (list == m_focusList)
					{
						useHeight = focusBoxSize;
					}
					else
					{
						useHeight = shrinkHeight;
					}
				}
				else
				{
					useHeight = height;
				}
				top			+= useHeight;
				list.Height  = useHeight;
		
				list.Width	= this.panelFactors.Width;
				ListResize(list, null);
				count++; 
			}
		}

		private void ListMouseEnter(object sender, EventArgs e)
		{
			m_focusList = sender as ListView;
			Panel_Resize(null,null);
		}
	}
}
