using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;

using MultiAlignCore.Data;
using MultiAlignEngine;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for ctlSummaryPages.
	/// </summary>
	public class ctlSummaryPages : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TabControl tabs;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// Hash table of the lists for describing the summary.
		/// </summary>
		private Hashtable m_lists;

		public ctlSummaryPages()
		{
			InitializeComponent();
			m_lists = new Hashtable();
			Resize += new EventHandler(ctlSummaryPages_Resize);
		}

		/// <summary>
		/// Add a cluster to the summary.  Reads the appropiate attribute from the property of the cluster objects.
		/// </summary>
		/// <param name="o"></param>
		private void UpdateSummary(ListView list, object o)
		{
			if (o != null)
			{
				foreach(PropertyInfo prop in o.GetType().GetProperties())
				{
					try
					{
						object[] customAttributes = prop.GetCustomAttributes(typeof(DataSummaryAttribute),true);
						if (customAttributes.Length > 0 && prop.CanRead)
						{
							DataSummaryAttribute attr = customAttributes[0] as DataSummaryAttribute ;						
							object objectValue = prop.GetValue(o,System.Reflection.BindingFlags.GetProperty,
								null,null,null);
							if (objectValue != null && attr != null)
							{
								AddToList(list, attr.Name, objectValue.ToString(), Color.Black, Color.White);
							}
						}
					}
					catch
					{
					}
				}	
				
				foreach(FieldInfo field in o.GetType().GetFields())
				{
					try
					{
						object[] customAttributes = field.GetCustomAttributes(typeof(DataSummaryAttribute),true);
						if (customAttributes.Length > 0)
						{
							DataSummaryAttribute attr = customAttributes[0] as DataSummaryAttribute ;						
							object objectValue = field.GetValue(o);
							if (objectValue != null && attr != null)
							{
								AddToList(list, attr.Name, objectValue.ToString(), Color.Black, Color.White);
							}
						}
					}
					catch
					{
					}
				}	
			}
		}

		/// <summary>
		/// Adds a text and value to the global summary list view
		/// </summary>
		/// <param name="description">Description to be drawn</param>
		/// <param name="val">Value to be displayed</param>
		/// <param name="foreColor">Forecolor of value text</param>
		/// <param name="backColor">Backcolor of value text</param>
		private void AddToList(ListView list, string description, string val, Color foreColor, Color backColor)
		{
			/* Create a new list view item */
			ListViewItem summaryItem = new ListViewItem();
			
			/* Make the item and enable dynamic formatting */
			summaryItem.Text = description;
			summaryItem.SubItems.Add(val);	
			summaryItem.UseItemStyleForSubItems = false;           
			/* Make it look pretty */
			summaryItem.SubItems[1].ForeColor   = foreColor;
			summaryItem.SubItems[1].BackColor   = backColor;						
			list.Items.Add(summaryItem);
		}



		/// <summary>
		///  Create a summary tab for this particular object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="o"></param>
		public void CreateSummary(string name, object o)
		{
			TabPage newPage = new TabPage(name);
			tabs.TabPages.Add(newPage);
				
			ListView list = new ListView();
			list.View = View.Details;
			list.GridLines = true;
			list.Dock = DockStyle.Fill;            
			newPage.Controls.Add(list);
			m_lists.Add(name, list);
			UpdateSummary(list, o);
			
			ContextMenu menu                = new ContextMenu();
			MenuItem copyClipboardMenuItem  = new MenuItem("Copy Selected Items");	
			copyClipboardMenuItem.Click     += new EventHandler(copyClipboardMenuItem_Click);
			menu.MenuItems.Add (copyClipboardMenuItem);

			list.ContextMenu                = menu;
			list.FullRowSelect              = true;
			list.MultiSelect                = true;
			list.KeyUp                      += new KeyEventHandler(list_KeyUp);

			list.Columns.Add("Description", -1, System.Windows.Forms.HorizontalAlignment.Left);
			list.Columns.Add("Value", -1, System.Windows.Forms.HorizontalAlignment.Left);

			UpdateListColumnWidths(list);
		}

        public void AddCustomSummary(string name, ListView listview)
        {
            TabPage newPage     = new TabPage(name);
            tabs.TabPages.Add(newPage);

            listview.Dock        = DockStyle.Fill;
            newPage.Controls.Add(listview);


            ContextMenu menu                = new ContextMenu();
            MenuItem copyClipboardMenuItem  = new MenuItem("Copy Selected Items");
            copyClipboardMenuItem.Click     += new EventHandler(copyClipboardMenuItem_Click);
            menu.MenuItems.Add(copyClipboardMenuItem);

            listview.ContextMenu = menu;
            listview.FullRowSelect = true;
            listview.MultiSelect = true;
            listview.KeyUp += new KeyEventHandler(list_KeyUp);
        }

		public void UpdateColumnWidths()
		{
			foreach(ListView list in m_lists.Values)
			{
				UpdateListColumnWidths(list);
			}
		}
		private void UpdateListColumnWidths(ListView list)
		{
			if (list.Columns.Count <= 0)
				return;
			foreach(ColumnHeader col in list.Columns)
			{
				col.Width = Width / list.Columns.Count;
			}
		}

		/// <summary>
		/// Save the specified list view contents to file.
		/// </summary>
		/// <param name="name">Name of header string to write</param>
		/// <param name="list">List view to save</param>
		/// <param name="delimiter">Delimiter to use</param>
		private void SaveList(string name, string path, ListView list, char delimiter, int skiplines)
		{
			PNNLControls.clsTextDelimitedFileWriter writer = new PNNLControls.clsTextDelimitedFileWriter();
			writer.Delimiter = delimiter;
			writer.LinesBeforeHeader = skiplines;
			writer.HeaderString = name;

			/// 
			/// Write the list in column format - this involves iterating over the data
			/// in column order.
			/// 
			for(int i = 0; i < list.Columns.Count; i++)
			{
				ArrayList data = new ArrayList();
				writer.Headers.Add(list.Columns[i].Text);
				for(int j = 0; j < list.Items.Count; j++)
				{
					ListViewItem item = list.Items[j];
					data.Add(item.SubItems[i].Text);
				}
				writer.Data.Add(list.Columns[i].Text, data);
			}

			/// 
			/// Append because we are opening and closing the file repeatedly.
			/// 
			writer.Write(path, true, true);
			writer.Dispose();
		}

		public void AddData(string name, string description, string val)
		{
			ListView list = m_lists[name] as ListView;
			
			if (list == null)
				return;

			ListViewItem item = new ListViewItem();
			item.Text = description;
			item.SubItems.Add(val);
			list.Items.Add(item);
		}

		/// <summary>
		/// Saves the contents of the summary pages to the specified path in column format.
		/// </summary>
		/// <param name="path"></param>
		public void Save(string path)
		{
			Save(path, ',', 2);
		}

		
		/// <summary>
		/// Saves the contents of the summary pages to the specified path in column format.
		/// </summary>
		/// <param name="path"></param>
		public void Save(string path, char delimiter)
		{
			/// 
			/// Iterate over all the keys to save all the summary pages to file.
			/// 
			Save(path, delimiter, 2);
		}
		
		/// <summary>
		/// Saves the contents of the summary pages to the specified path in column format.
		/// </summary>
		/// <param name="path"></param>
		public void Save(string path, char delimiter, int skiplines)
		{
			/// 
			/// Iterate over all the keys to save all the summary pages to file.
			/// 
			foreach(TabPage tab in tabs.TabPages)
			{
				string key = tab.Text;	
				ListView list = m_lists[key] as ListView;
				SaveList(key, path, list, delimiter, skiplines);
			}
		}

		/// <summary>
		/// Clears the summary pages' content.
		/// </summary>
		public void Clear()
		{
			m_lists.Clear();
			tabs.TabPages.Clear();
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
			this.tabs = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(624, 656);
			this.tabs.TabIndex = 1;
			// 
			// ctlSummaryPages
			// 
			this.Controls.Add(this.tabs);
			this.Name = "ctlSummaryPages";
			this.Size = new System.Drawing.Size(624, 656);
			this.ResumeLayout(false);

		}
		#endregion

		private void ctlSummaryPages_Resize(object sender, EventArgs e)
		{
			UpdateColumnWidths();
		}

		private void CopySelectedItemsToClipboard(ListView list)
		{			
			string data = "";
            int i       = 0;
            /// 
            /// We also want to grab the column headers 
            /// 
            foreach (ColumnHeader header in list.Columns)
            {
                data += header.Text + ",";
            }
            data  = data.TrimEnd(',');
            data += "\n";

            /// 
            /// Then grab the data 
            /// 
			foreach (ListViewItem item in list.SelectedItems)
			{				
				foreach(ListViewItem.ListViewSubItem subitem in item.SubItems)
				{
					data += subitem.Text + ",";
				}
				data = data.TrimEnd(',');
				data += "\n";
			}
			data = data.TrimEnd('\n');
			Clipboard.SetDataObject(data, true);
		}

		private void list_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.C && e.Control == true)
			{
				ListView list = sender as ListView;
				if (list == null)
					return;
				CopySelectedItemsToClipboard(list);
			}
		}

		/// <summary>
		/// Handles the user mouse click on the context menu for copying the data 
		/// currently selected in the list view to the system clipboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void copyClipboardMenuItem_Click(object sender, EventArgs e)
		{
			MenuItem item    = sender as MenuItem;
			if (item == null)
				return;
			ContextMenu menu = item.Parent as ContextMenu;
			if (menu == null)
				return ;		
			ListView list    = menu.SourceControl as ListView;
			if (list == null)
				return;			
			CopySelectedItemsToClipboard(list);
		}
	}
}
