using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace PNNLControls
{
	public delegate void DelegateListViewExEvent(object sender, EventArgs e);
	
	#region ListViewItem Extended class
	/// <summary>
	/// Extended list view item class for drawing custom list view items.
	/// </summary>
	public class ListViewExItem: ListViewItem
	{			
		public ListViewExItem()
		{		
		
		}		

		public ListViewExItem(string text):
			base(text)
		{						
		}
				
		/// <summary>
		/// Forces a draw of this item and its sub items.
		/// </summary>
		/// <param name="rect"></param>
		public void Draw(Rectangle rect)
		{			
			for(int i = 0; i < SubItems.Count; i++)				
			{
				ListViewExSubItem s = SubItems[i] as ListViewExSubItem;
				if (s != null)
				{
					s.Draw(rect, false);
				}
			}			
		}
	}
	#endregion

	#region ListViewSubItem Abstract Base Class
	/// <summary>
	/// Summary description for ListViewItemEx.
	/// </summary>
	public abstract class ListViewExSubItem: ListViewItem.ListViewSubItem
	{			
		public		event DelegateListViewExEvent Action;		
		public		Rectangle Bounds;
		protected	ToolTip m_tip = new ToolTip();						
		private		Color mobj_highlightBackColor;
		private		Color mobj_highlightForeColor;
		private		bool  mbln_selected;
        private ListViewItem mobj_parent;

		/// <summary>
		/// Constructor
		/// </summary>
		public ListViewExSubItem()
		{		            
			m_tip.Active = true;
			m_tip.ReshowDelay = 1;	
			mbln_selected = false;

			mobj_highlightForeColor = Color.White;
			mobj_highlightBackColor = Color.Navy;
		}

        /// <summary>
        /// Gets or sets the parent ListViewItem of this sub item.
        /// </summary>
        public ListViewItem Parent
        {
            get
            {
                return mobj_parent;
            }
            set
            {
                mobj_parent = value;
            }
        }

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="text"></param>
		public ListViewExSubItem(string text)
		{					
			Text = text;
			m_tip.Active = true;
			m_tip.ReshowDelay = 1;			
			mbln_selected = false;
			
			mobj_highlightForeColor = Color.White;
			mobj_highlightBackColor = Color.Navy;
		}						
		
		/// <summary>
		/// Abstract method telling the derived class where to draw its underlying control.
		/// </summary>
		/// <param name="rect">Bounding box for control</param>
		/// <param name="onClick">Draw is a result of a user click.</param>
		public abstract void Draw(Rectangle rect, bool onClick);
		
		/// <summary>
		/// Propogrates an event up using sender and e as the parameters.
		/// </summary>
		/// <param name="sender">Derived control who triggered the event.</param>
		/// <param name="e">Arguments about the action to be processed.</param>
		protected void FireActionEvent(object sender, EventArgs e)
		{
			if (Action != null)
				Action(sender,e);
		}

		public bool Selected 
		{
			get
			{
				return mbln_selected;
			}
			set
			{
				mbln_selected = value;
			}
		}

		public Color HighlightForeColor
		{
			get
			{
				return mobj_highlightForeColor;
			}
			set
			{
				mobj_highlightForeColor = value;
			}
		}

		public Color HighlightBackColor
		{
			get
			{
				return mobj_highlightBackColor;
			}
			set
			{
				mobj_highlightBackColor = value;
			}
		}
	}

	#endregion

	#region ListViewExSubItem Extended Classes
	/// <summary>
	/// Base ListViewExSubItem that only has text.
	/// </summary>
	public class ListViewExTextItem: ListViewExSubItem
	{		
		public ListViewExTextItem()			
		{					
		}						
	
		public ListViewExTextItem(string text):
			base(text)
		{			
		}						

		public override  void Draw(Rectangle rect, bool onClick)
		{
			return;
		}
	}
	
	/// <summary>
	/// Link label List View Item.
	/// </summary>
	public class ListViewExLinkLabelItem: ListViewExSubItem
	{
		public LinkLabel m_label = new LinkLabel();
		/// <summary>
		/// Constructor for the listview link label
		/// </summary>
		/// <param name="parent"></param>
		public ListViewExLinkLabelItem(Control parent)
		{
			m_label.Text = "";
			m_label.Click += new EventHandler(m_label_Click);
		}

		/// <summary>
		/// Constructor for the list view link label.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="parent"></param>
		public ListViewExLinkLabelItem(string text, Control parent):
			base(text)
		{
			m_label.Text   = text;
			m_label.Click += new EventHandler(m_label_Click);			
			parent.Controls.Add(m_label);
		}

		/// <summary>
		/// Raises notification that this particular link label was selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_label_Click(object sender, EventArgs e)
		{
			ListViewExEventArgs args = new ListViewExEventArgs(base.Text);
			base.FireActionEvent(this,args);
		}

		/// <summary>
		/// Overrides the draw method.  Draws the label in the specified bounds.
		/// </summary>
		/// <param name="rect">Area to draw link label in.</param>
		/// <param name="onClick">Tells control if the user clicked on the control or not.</param>
		public override void Draw(Rectangle rect, bool onClick)
		{
			if (rect.Top < 0) 
			{
				m_label.Visible = false;
				return;
			}
			if (Selected == true)
			{
				m_label.BackColor = HighlightBackColor;
			}
			else
			{
				m_label.BackColor = BackColor;
			}

			if (Selected == true)
			{
				m_label.ForeColor = HighlightForeColor;
			}
			else
			{
				m_label.ForeColor = ForeColor;
			}

			m_label.LinkColor = m_label.ForeColor;
			

			m_label.Visible = true;
			m_label.Top = rect.Top;
			m_label.Left = rect.Left;
			m_label.Bounds = rect;
			m_label.Refresh();			
		}
	}

	/// <summary>
	/// List View Combo Box Item.  Draws a combo box in a list view.
	/// </summary>
	public class ListViewExComboBoxItem: ListViewExSubItem
	{
		private ComboBox m_combo;
		private int		 m_prevSelectedIndex;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Initial Text to Display</param>
		/// <param name="parent">Parent Control</param>
		/// <param name="list">List of items that should combo box should populate</param>
		public ListViewExComboBoxItem(string text, Control parent, object[] objectList):
			base(text)
		{			
			m_combo = new ComboBox();			
						
			m_combo.Items.AddRange(objectList);			
			m_combo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			m_combo.DrawItem				+= new DrawItemEventHandler(m_combo_DrawItem);
			m_combo.MouseLeave				+= new EventHandler(m_combo_MouseLeave);
			m_combo.MouseHover				+= new EventHandler(m_combo_MouseHover);
			m_combo.LostFocus				+= new EventHandler(m_combo_LostFocus);
			m_combo.SelectedIndexChanged	+= new EventHandler(m_combo_SelectedIndexChanged);			
			parent.Controls.Add(m_combo);
			
			m_combo.Hide();
			m_prevSelectedIndex = -1;
		}
		
		/// <summary>
		/// Override the base draw function
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="onClick">If the subitem was clicked or just needs to be updated;		
		public override void Draw(Rectangle rect, bool onClick)
		{	
			rect.Height    = Math.Abs(rect.Height - 100);			
			m_combo.Bounds = rect;			
			if (onClick == true)
				m_combo.Show();
			else			
				m_combo.Hide();	
		}
		
		/// <summary>
		/// Event handler when the combo box changes its index.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_combo_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListViewExComboEventArgs args = new ListViewExComboEventArgs (
														m_combo.Text,
														m_combo.SelectedIndex,
														m_prevSelectedIndex);			
			m_prevSelectedIndex = m_combo.SelectedIndex;
			Text = m_combo.Text;			
			m_combo.Hide();
			FireActionEvent(this, args);
		}

		/// <summary>
		/// Occurs when the mouse hovers over the combo box. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_combo_MouseHover(object sender, EventArgs e)
		{
			m_tip.SetToolTip(m_combo, m_combo.Text);
		}

		/// <summary>
		/// Custom draw the combo box to adjust for height in the listview...Code found http://www.csharphelp.com/archives/archive262.html
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_combo_DrawItem(object sender, DrawItemEventArgs e)
		{			
			int delta = 1;
			Rectangle rc = new Rectangle(e.Bounds.X + delta,
										 e.Bounds.Y + delta, 
										 e.Bounds.Width  - delta,
										 e.Bounds.Height - delta);
			
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center ;

			e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black) , 2) , rc);
			string text = (string)m_combo.Items[e.Index];	

			if ( e.State == ( DrawItemState.Selected | DrawItemState.NoAccelerator
				| DrawItemState.NoFocusRect) || 
				e.State == DrawItemState.Selected  ) 
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.Blue) , rc);
				e.Graphics.DrawString( text , new Font(FontFamily.GenericSansSerif , 8,  FontStyle.Italic | FontStyle.Bold) , new SolidBrush(Color.White), rc ,sf);				
			}
			else
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.White) , rc);
				e.Graphics.DrawString( text , new Font(FontFamily.GenericSansSerif , 8,  FontStyle.Regular) , new SolidBrush(Color.Black), rc ,sf);
			}
		}

		/// <summary>
		/// Occurs when the mouse leaves the control. Hides the combo box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_combo_MouseLeave(object sender, EventArgs e)
		{
			m_combo.Hide();
		}

		/// <summary>
		/// Hides the combo box when it loses focus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_combo_LostFocus(object sender, EventArgs e)
		{
			m_combo.Hide();
		}
	}

	/// <summary>
	/// List View Button Item : Draws a button in a list view control.
	/// </summary>
	public class ListViewExButtonItem: ListViewExSubItem
	{
		private Button m_button;

		/// <summary>
		/// Constructor for extending a listviewitem to have a button over the top of it.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="parent"></param>
		public ListViewExButtonItem(string text, Control parent):
			base(text)
		{			
			m_button			 = new Button();	
			m_button.Text		 = text;
			m_button.Click		+= new EventHandler(m_button_Click);			
			m_button.MouseHover += new EventHandler(m_button_MouseHover);
			parent.Controls.Add(m_button);
		}
		
		/// <summary>
		/// Override draw method to draw the  control onto of the parent control.
		/// </summary>
		/// <param name="rect">Bounding box</param>
		/// <param name="onClick">Whether the sub item area was clicked.</param>
		public override void Draw(Rectangle rect, bool onClick)
		{
			Bounds			= rect;
			m_button.Bounds = Bounds;						
		}

		/// <summary>
		/// Occurs when the button is clicked.  Notifies any subscribers of this event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_button_Click(object sender, EventArgs e)
		{			
			ListViewExEventArgs args = new ListViewExEventArgs(m_button.Text);
			FireActionEvent(this, args);
		}

		/// <summary>
		/// Occurs when the mouse hovers over the button control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_button_MouseHover(object sender, EventArgs e)
		{
			m_tip.SetToolTip(m_button, m_button.Text);
		}
	}

	#endregion


	#region Event Args
	/// <summary>
	/// Event arguments for a listview combo box item.
	/// </summary>
	public class ListViewExComboEventArgs: EventArgs
	{
		private string m_text				  = "";
		private int	  m_selectedIndex		  = 0;		
		private int	  m_previousSelectedIndex = 0;
		public ListViewExComboEventArgs(string s, int index, int prevIndex)
		{
			m_text  = s;
			m_selectedIndex = index;
			m_previousSelectedIndex = prevIndex;
		}
	
		/// <summary>
		/// Gets text for selected item.
		/// </summary>
		public string Text
		{
			get
			{
				return m_text;
			}
		}

		/// <summary>
		/// Gets selected index.
		/// </summary>
		public int SelectedIndex
		{
			get 
			{
				return m_selectedIndex;
			}

		}
		/// <summary>
		/// Gets the previously selected index.
		/// </summary>
		public int PreviousSelectedIndex
		{
			get
			{
				return m_previousSelectedIndex;
			}
		}
	}

	/// <summary>
	/// Event arguments for a listview combo box item.
	/// </summary>
	public class ListViewExEventArgs: EventArgs
	{
		public string Text;		
		public ListViewExEventArgs(string s)
		{
			Text  = s;			
		}
	}
	#endregion

}
