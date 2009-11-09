using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PNNLControls
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

		public ctlDataFactorValueAssignment()
		{			
			InitializeComponent();
			InitControl();
		}

		private ContextMenu CreateContextMenu()
		{
			ContextMenu menu		= new ContextMenu();
			MenuItem item_detail	= new MenuItem("Detailed View", new System.EventHandler(Detail_Click));
			MenuItem item_large		= new MenuItem("Large Item View", new System.EventHandler(LargeView_Click));						
			
			menu.MenuItems.Add(item_detail);
			menu.MenuItems.Add(item_large);
			return menu;
		}

		public void InitControl()
		{
			m_factorBoxes = new ArrayList();
			m_dataHash = new Hashtable();
			
			Resize += new EventHandler(ctl_Resize);
			ContextMenu menu			= CreateContextMenu();
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
			list.Dock				= DockStyle.Top;	
			list.SmallImageList		= dataImages;
			list.LargeImageList		= dataImages;
			list.View				= View.Details;			
					
			list.Name		= factorValueName;						
			list.MouseDown += new MouseEventHandler(ListMouseDown);
			//list.DragDrop  += new DragEventHandler(ListDragDrop);	
			//list.DragEnter += new DragEventHandler(ListDragEnter);
			list.Resize    += new EventHandler(ListResize);						
			list.ContextMenu	= menu;	
			
			panelFactors.Controls.Add(list);		
			m_factorBoxes.Add(list);
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

		/// <summary>
		/// List Mouse Down event handler.  Starts drag and drop when list done.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListMouseDown(object sender, MouseEventArgs e)
		{
			ListView list = sender as ListView;
			m_sourceList = list;

			foreach(ListViewItem item in list.SelectedItems)
			{
				list.DoDragDrop(item.Text, DragDropEffects.Move);
			}
		}

		private void ListDragDrop(object sender, DragEventArgs e)
		{
			string itemText = e.Data.GetData(DataFormats.Text).ToString();			
			ListView list = sender as ListView;
			ListViewItem item = new ListViewItem();
			item.Text = itemText;
			item.ImageIndex = 0;
			list.Items.Add(item);	
			m_dataHash[itemText] = list.Name;
			list.Refresh();			

			if (m_sourceList != null)
			{
				int index = m_sourceList.SelectedItems[0].Index;
				m_sourceList.Items.RemoveAt(index);
			}
		}
		
		private void ListDragEnter(object sender, DragEventArgs e)
		{
			if (e.AllowedEffect == DragDropEffects.Move)
				e.Effect = DragDropEffects.Move;			
		}

		private void ListResize(object sender, EventArgs e)
		{
			ListView list = sender as ListView;
			list.Columns[0].Width = list.Width;
		}

		private void ctl_Resize(object sender, EventArgs e)
		{
			
			int height = panelFactors.Height;
			int count = 0;
			foreach(ListView list in m_factorBoxes)
			{
				list.Top    = height * count; 
				list.Height = height / m_factorBoxes.Count;
				count++; 
				list.Width = panelFactors.Width;
			}
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
	}
}
