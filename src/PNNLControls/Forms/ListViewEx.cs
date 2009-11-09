using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PNNLControls
{
	/// <summary>
	/// Extended list view control.
	/// </summary>
	public class ListViewEx : ListView
	{				

		public event System.EventHandler Scroll;
		private System.ComponentModel.IContainer components;
		private ListViewHeaderControl m_headerControl;


		/// <summary>
		/// Default constructor for an extended list view control.
		/// </summary>
		public ListViewEx()
		{			
			InitializeComponent();			
			MouseUp += new MouseEventHandler(ctlListView_MouseUp);			
		
			this.SelectedIndexChanged += new EventHandler(ListViewEx_SelectedIndexChanged);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )			{
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
			this.SuspendLayout();
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ListViewEx));
			
			// 
			// ctlListView
			// 
			this.BackColor = System.Drawing.SystemColors.Window;			
			this.FullRowSelect = true;
			this.GridLines = true;
			this.MultiSelect = false;
			this.Size = new System.Drawing.Size(424, 248);
			this.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.View = System.Windows.Forms.View.Details;
			this.ResumeLayout(false);
		}
		#endregion									

		#region WIN32 Overrides - Event Classes and Methods

		/* Win32 Sections of this code were derived from http://www.codeproject.com/cs/miscctrl/myListViewNoSize.asp */

		/// <summary>
		/// Notify message header structure.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			private struct NMHDR
		{
			public IntPtr hwndFrom;
			public int idFrom;
			public int code;
		} 

		/// <summary>
		/// Class used to capture window messages for the header of the list view
		/// control.  
		/// </summary>
		internal class ListViewHeaderControl : NativeWindow
		{
			private ListViewEx m_parent = null;
			[DllImport("User32.dll",CharSet = CharSet.Auto,SetLastError=true)]
			public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

			/// <summary>
			/// Constructor for the header control
			/// </summary>
			/// <param name="m">parent listview item</param>
			public ListViewHeaderControl(ListViewEx m)
			{
				m_parent = m;
				//Get the header control handle
				IntPtr header = SendMessage(m.Handle, 
					(0x1000+31), IntPtr.Zero, IntPtr.Zero);
				
				AssignHandle(header);                
			} 

			/// <summary>
			/// Taps into the windows message map to control the behaivor of the listview
			/// </summary>
			/// <param name="message">Windows message</param>
			protected override void WndProc(ref Message message)
			{
				const int WM_LBUTTONDBLCLK = 0x0203;
				const int WM_SETCURSOR     = 0x0020;
				
				/* Figure out what message was sent to the control, then ravage it  */
				switch ( message.Msg )
				{
					case WM_LBUTTONDBLCLK:                    
					case WM_SETCURSOR:
						break;
					default:
						break;
				} 
				
				// pass messages on to the base control for processing
				base.WndProc( ref message ) ;				 
			} 
		} 


		/// <summary>
		/// When the control is created capture the messages for the header. 
		/// </summary>
		protected override void OnCreateControl()
		{
			//First actually create the control.
			base.OnCreateControl();
			//Now create the HeaderControl class to handle the customization of the header messages.
			m_headerControl = new ListViewHeaderControl(this);
		} 
		
		/// <summary>
		/// Capture messages for the list view control to synchronize column sizing.
		/// </summary>
		protected override void WndProc(ref Message message )
		{
			const int WM_NOTIFY			 = 0x004E;			
			const int WM_COMPLETE		 = -12;
			const int WM_LBUTTONUP		 = 0x0202;
			const int WM_LBUTTONDOWN	 = 0x0201;
				
			/* 
			 * Dont blindly pass the message to the base class.  We need to first 
			 * track the type of message and specifically handle resizing of the list updates.
			 */
			const int WM_HSCROLL = 0x114;
			const int WM_VSCROLL = 0x115;

			switch ( message.Msg )
			{
				case WM_VSCROLL:
				case WM_HSCROLL:
					if (this.Scroll != null)
					{
						Scroll(this, null);	
					}
					UpdateListViewWidths();
					base.WndProc( ref message );							
					break;
				case WM_NOTIFY:
					NMHDR nmhdr = (NMHDR)message.GetLParam(typeof(NMHDR));												
					switch(nmhdr.code)
					{
						/* When the column header is clicked or the column header is dragged */
						case WM_LBUTTONUP:							
						case WM_LBUTTONDOWN:		
						case WM_COMPLETE:
							base.WndProc( ref message );
							UpdateListViewWidths();							
							break;
						default:
							Console.WriteLine(nmhdr.code.ToString() + " " + message.LParam.ToString());
							base.WndProc( ref message );
							break;
					} 
					break;
				
				default:
					base.WndProc( ref message );
					//Console.WriteLine(message.Msg.ToString() + " " +  message.LParam.ToString());
					break;
			} 										
		} 
		#endregion

		/// <summary>
		/// Updates all of the underlying listviewitem's sub plots.  
		/// </summary>
		/// <param name="col">Column to adjust for.</param>
		/// <param name="left">Left pixel range of subplot item.</param>
		private void UpdateSubItemWidths(ColumnHeader col, int left)
		{
			/* Traverse the item list to update the specific subplot associated with this column. */		
			foreach(ListViewExItem item in Items)
			{									
				ListViewExSubItem subItem = item.SubItems[col.Index] as ListViewExSubItem;
				if (subItem != null)
				{					
					subItem.Bounds			= item.Bounds;
					subItem.Bounds.Width	= col.Width;
					subItem.Bounds.X		= left + item.Bounds.X;       
					subItem.Bounds.Height   = item.Bounds.Height;						
					subItem.Draw(subItem.Bounds, false);
				}
			}			
		}

		/// <summary>
		/// Handles width change for all of the list view items.
		/// </summary>
		public void UpdateListViewWidths()
		{			
			int left = Left;
			foreach(ColumnHeader col in Columns)
			{
				UpdateSubItemWidths(col, left);				
				left += col.Width;	
			}		
		}
		
		/// <summary>
		/// Figures out what subitem was selected.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		private ListViewExSubItem FindSubItem(ListViewExItem item, int x)
		{			
			int left, right;
			left  = 0;
			right = 0;
			foreach(ColumnHeader col in Columns)
			{
				right = left + col.Width;
				if (left < x && x < right)
				{
					ListViewExSubItem subItem = item.SubItems[col.Index] as ListViewExSubItem;
					if (subItem != null)
					{
						Rectangle rect = new Rectangle(left + item.Bounds.Left, item.Bounds.Y, col.Width, item.Bounds.Height);
						subItem.Bounds = rect;
					}
					return  subItem;
				}
				left += col.Width;
			}
			return null;
		}	

		/// <summary>
		/// Gets the item where the mouse has clicked and draws that current list item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ctlListView_MouseUp(object sender, MouseEventArgs e)
		{				
			/*Get the current item */
			ListViewExItem item	= GetItemAt(e.X, e.Y) as ListViewExItem;
			if (item == null) 
				return;
						
			ListViewExSubItem subItem = null; 
			/* Find the subitem */
			subItem = FindSubItem(item, e.X);								
			if (subItem == null) 
				return;
			subItem.Selected = true;
			subItem.Draw(subItem.Bounds, true);							
		}

		private void ListViewEx_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach(ListViewExItem item in Items)
			{
				foreach(ListViewItem.ListViewSubItem subitem in item.SubItems)
				{
					ListViewExSubItem subExItem = subitem as ListViewExSubItem;
					if (subExItem != null)
					{
						subExItem.Selected = item.Selected;
						subExItem.Draw(subExItem.Bounds, false);
					}
				}
			}			
		}
	}    
}
