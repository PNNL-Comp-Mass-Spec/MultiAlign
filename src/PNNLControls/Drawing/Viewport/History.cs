using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Delegate signature for CurrentEntryChanged event.
	/// </summary>
	public delegate void CurrentEntryChangedHandler(object sender, CurrentEntryChangedEventArgs args);
	public class CurrentEntryChangedEventArgs 
	{
		private bool mFromHistoryList;
		public CurrentEntryChangedEventArgs(bool fromHistoryList) 
		{
			this.mFromHistoryList = fromHistoryList;
		}

		/// <summary>
		/// Tells whether the current entry that was just set was one from either the back 
		/// or forward lists, or whether it was a new setting.
		/// </summary>
		public bool FromHistoryList 
		{
			get 
			{
				return this.mFromHistoryList;
			}
		}
	}


	/// <summary>
	/// Maintains browser-type history, consisting of 
	/// a current setting; 
	/// a list of back settings;
	/// a list of forward settings.
	/// Provides methods to manipulate these lists in the common internet browser way.
	/// </summary>
	public class History
	{
		#region "Class Variables"
		/// <summary>
		/// The list of forward entries.  The ones closest to the current entry are
		/// at the lowest indices.
		/// </summary>
		private IList mForwardList = new ArrayList();

		/// <summary>
		/// The list of back entries. The ones closest to the current entry are
		/// at the lowest indices.
		/// </summary>
		private IList mBackList = new ArrayList();

		/// <summary>
		/// The current entry.
		/// </summary>
		private Object mCurrentEntry;

		#endregion

		#region "Events"
		/// <summary>
		/// Raised when the current entry is changed.
		/// </summary>
		public event CurrentEntryChangedHandler CurrentEntryChanged;

		/// <summary>
		/// Raised when the list of itema in the forward list is changed.
		/// </summary>
		public event EventHandler ForwardListChanged;
		
		/// <summary>
		/// Raised when the list of items in the back list is changed. 
		/// </summary>
		public event EventHandler BackListChanged;

		/// <summary>
		/// Raises the CurrentEntryChanged event.
		/// </summary>
		/// <param name="fromHistoryList"></param>
		protected virtual void OnCurrentEntryChanged(bool fromHistoryList) 
		{
			if (this.CurrentEntryChanged != null) 
			{
				this.CurrentEntryChanged(this, new CurrentEntryChangedEventArgs(fromHistoryList));
			}
		}

		/// <summary>
		/// Raises the ForwardListChanged event.
		/// </summary>
		protected virtual void OnForwardListChanged() 
		{
			if (this.ForwardListChanged != null) 
			{
				this.ForwardListChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Raises the BackListChanged event.
		/// </summary>
		protected virtual void OnBackListChanged() 
		{
			if (this.BackListChanged != null) 
			{
				this.BackListChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Allows subclasses to alter a proposed current entry before the history 
		/// is updated.  This implementation just returns the input.
		/// </summary>
		/// <param name="potentialValue"></param>
		/// <returns></returns>
		protected virtual Object OnPreChangeCurrentEntry(Object potentialValue) 
		{
			return potentialValue;
		}

		#endregion

		#region "Constructors"
		/// <summary>
		/// Create a new History class.  Intially the current entry is null and the 
		/// back and forward lists are empty.  If the current entry is set through the 
		/// CurrentEntry property, this null will be added to the back list.  To avoid this, 
		/// the SetCurrentEntry method should be called instead with false as the final parameter, 
		/// or the other constructor should be used.
		/// </summary>
		public History()
		{
		}

		/// <summary>
		/// Creates a new History instance.  The back and forward lists are initially empty.
		/// </summary>
		/// <param name="initialEntry"></param>
		public History(Object initialEntry) : this()
		{
			this.mCurrentEntry = initialEntry;
		}

		/// <summary>
		/// Creates a new History instance.  The back list, forward list, and current entry
		/// are set to the given values.
		/// </summary>
		/// <param name="initialEntry"></param>
		/// <param name="backList"></param>
		/// <param name="forwardList"></param>
		/// <exception cref="ArgumentNullException">Thrown if backList or forwardList is null.</exception>
		public History(Object initialEntry, 
			IList backList, IList forwardList) : this(initialEntry)
		{
			this.ForwardList = forwardList;
			this.BackList = backList;
		}
		#endregion

		#region "Movement"
		/// <summary>
		/// Moves to the given entry in the forward list.  The current state as well as any 
		/// other entries in the forward list prior to the given entry are moved onto 
		/// the back list.
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if there is no entry 
		/// in the forward list for this given index.</exception>
		public void MoveForward(int index) 
		{
			if (!this.CanMoveForward(index)) 
			{
				throw new ArgumentOutOfRangeException("index", index, "No forward entry at given index");
			}
			// put the current entry on the back list
			this.mBackList.Insert(0, this.mCurrentEntry);
			// pull entries off the forward list and add them to the back list
			for (int i = 0; i < index ;i++) 
			{
				this.mBackList.Insert(0, this.mForwardList[0]);
				this.mForwardList.RemoveAt(0);
			}
			Object currentEntry = this.mForwardList[0];
			this.mForwardList.RemoveAt(0);
			
			this.SetCurrentEntry(currentEntry, false, false);

			this.OnBackListChanged();
			this.OnCurrentEntryChanged(true);
			this.OnForwardListChanged();
		}

		/// <summary>
		/// Moves to the first entry in the forward list equal to the given entry.  
		/// The current entry as well as any other entries in the forward list prior 
		/// to this entry are moved onto the back list.
		/// </summary>
		/// <param name="entry"></param>
		/// <exception cref="IllegalArgumentException">Thrown if no matching entry 
		/// is found in the forward list.</exception>
		public void MoveForward(object entry) 
		{
			if (!this.CanMoveForward(entry)) 
			{
				throw new ArgumentException("Entry does not exist in forward list.", "entry");
			}
			this.MoveForward(this.mForwardList.IndexOf(entry));
		}

		/// <summary>
		/// Moves one entry forward, to the first entry in the forward list.  The current 
		/// entry is moved onto the back list, the first entry on the forward list 
		/// becomes current and is removed from the forward list.
		/// </summary>
		public void MoveForward() 
		{
			this.MoveForward(true);
		}

		/// <summary>
		/// Moves one entry forward, to the first entry in the forward list.  The current 
		/// entry is moved onto the back list, the first entry on the forward list 
		/// becomes current and is removed from the forward list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if there is no entry in 
		/// the forward list to move to.</exception>
		public void MoveForward(bool throwException) 
		{
			if (!this.CanMoveForward()) 
			{
				if (throwException) 
				{
					throw new InvalidOperationException("No entries to move forward to");
				} 
				else 
				{
					return;
				}
			}
			this.MoveForward(0);
		}

		/// <summary>
		/// Moves to the given entry in the back list.  The current state as well as any
		/// other entries in the back list prior to the given entry are moved onto the 
		/// forward list.
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if there is no entry 
		/// in the forward list for this given index.</exception>
		public void MoveBack(int index) 
		{
			if (!this.CanMoveBack(index)) 
			{
				throw new ArgumentOutOfRangeException("index", index, "No back entry at given index");
			}
			// put the current entry on the forward list
			this.mForwardList.Insert(0, this.mCurrentEntry);
			// pull entries off the back list and add them to the forward list
			for (int i = 0; i < index; i++) 
			{
				this.mForwardList.Insert(0, this.mBackList[0]);
				this.mBackList.RemoveAt(0);
			}
			Object currentEntry  = this.mBackList[0];
			this.mBackList.RemoveAt(0);

			this.SetCurrentEntry(currentEntry, false, false);

			this.OnBackListChanged();
			this.OnCurrentEntryChanged(true);
			this.OnForwardListChanged();
		}

		/// <summary>
		/// Moves to the first entry in the back list equal to the given entry.  
		/// The current entry as well as any other entries in the back list prior 
		/// to this entry are moved onto the forward list.
		/// </summary>
		/// <param name="entry"></param>
		/// <exception cref="IllegalArgumentException">Thrown if no matching entry 
		/// is found in the back list.</exception>
		public void MoveBack(object entry) 
		{
			if (!this.CanMoveBack(entry)) 
			{
				throw new ArgumentException("Entry does not exist in back list.", "entry");
			}
			this.MoveBack(this.mBackList.IndexOf(entry));
		}

		/// <summary>
		/// Moves one entry back, to the first entry in the back list.  The current 
		/// entry is moved onto the forward list, the first entry on the back list becomes
		/// current and is removed from the back list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if there is no entry in 
		/// the forward list to move to.</exception>
		public void MoveBack() 
		{
			MoveBack(true);
		}

		/// <summary>
		/// Moves one entry back, to the first entry in the back list.  The current 
		/// entry is moved onto the forward list, the first entry on the back list becomes
		/// current and is removed from the back list.  If throwException is true and the back
		/// list is empty, an InvalidOperationException is thrown.  If throwExcpetion is false, nothing
		/// is done.
		/// </summary>
		/// <param name="throwException"></param>
		/// <exception cref="InvalidOperationException">Thrown if there is no entry in 
		/// the back list to move back to and throwException is true.</exception>
		public void MoveBack(bool throwException) 
		{
			if (!this.CanMoveBack()) 
			{
				if (throwException) 
				{
					throw new InvalidOperationException("No entries to move back to");
				} 
				else 
				{
					return;
				}
			}
			this.MoveBack(0);
		}
		#endregion

		#region "MovementHelpers"
		/// <summary>
		/// Tells whether MoveForward can be called.
		/// </summary>
		/// <returns></returns>
		public bool CanMoveForward() 
		{
			return this.mForwardList.Count > 0;
		}

		/// <summary>
		/// Tells whether <code>MoveToForwardEntry(int)</code> can be called.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool CanMoveForward(int index) 
		{
			return index >= 0 && index < this.mForwardList.Count;
		}

		/// <summary>
		/// Tells whether <code>MoveToForwardEntry(object)</code> can be called.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public bool CanMoveForward(object entry) 
		{
			return this.mForwardList.IndexOf(entry) >= 0;
		}

		/// <summary>
		/// Tells whether MoveBack can be called.
		/// </summary>
		/// <returns></returns>
		public bool CanMoveBack() 
		{
			return this.mBackList.Count > 0;
		}

		/// <summary>
		/// Tells whether <code>MoveToBackEntry(int)</code> can be called.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool CanMoveBack(int index) 
		{
			return index >= 0 && index < this.mBackList.Count;
		}

		/// <summary>
		/// Tells whether <code>MoveToBackEntry(object)</code> can be called.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public bool CanMoveBack(object entry) 
		{
			return this.mBackList.IndexOf(entry) >= 0;
		}
		#endregion

		#region "Properties"
		/// <summary>
		/// Gets or sets the list of entries on the back list.
		/// </summary>
		public IList BackList 
		{
			get 
			{
				return new ArrayList(this.mBackList);
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("BackList");
				}
				ArrayList list = new ArrayList(value.Count);
				foreach(Object entry in value) 
				{
					Validate(entry);
					list.Add(entry);
				}
				this.mBackList = list;
				this.OnBackListChanged();
			}
		}

		/// <summary>
		/// Gets or sets the list of entries on the forward list.
		/// </summary>
		public IList ForwardList 
		{
			get 
			{
				return new ArrayList(this.mForwardList);
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("ForwardList");
				}
				ArrayList list = new ArrayList(value.Count);
				foreach(Object entry in value) 
				{
					Validate(entry);
					list.Add(entry);
				}
				this.mForwardList = list;
				this.OnForwardListChanged();
			}
		}

		/// <summary>
		/// Gets or sets the current entry to the given value.  When setting, moves the 
		/// existing current entry onto the top of the back list, and clears the forward list.
		/// Calling set is equivalent to <code>SetCurrentEntry(value)</code>.
		/// </summary>
		/// <param name="entry"></param>
		public object CurrentEntry 
		{
			get 
			{
				return this.mCurrentEntry;
			}
			set 
			{
				this.SetCurrentEntry(value);
			}
		}

		#endregion

		#region "Helpers"
		/// <summary>
		/// Determines whether the given entry is valid or invalid for this history instance.
		/// Overriden in subclasses, this implementation always assumes that the entry is 
		/// valid.
		/// </summary>
		/// <param name="entry"></param>
		/// <exception cref="ArgumentException">Throw if the value is not appropriate for 
		/// this history.</exception>
		protected virtual void Validate(Object entry) 
		{
			return;
		}

		/// <summary>
		/// Clears both the back and forward lists.
		/// </summary>
		public void Clear() 
		{
			this.ForwardList = new ArrayList();
			this.BackList = new ArrayList();
		}

		/// <summary>
		/// Sets the current entry.  Equivalend to <code>SetCurrentEntry(entry, true, true)</code>
		/// </summary>
		/// <param name="entry"></param>
		public void SetCurrentEntry(Object entry) 
		{
			this.SetCurrentEntry(entry, true, true);
		}

		/// <summary>
		/// Sets the current entry and may modify the back and forward lists.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="modifyForwardList">If true, the forward list is cleared, otherwise
		/// the exsting forward list remains in place.</param>
		/// <param name="pushCurrentOntoBack">If true, the current entry (prior to setting 
		/// to the given value) is added to the back list.  If false, the back list is 
		/// not modified.</param>
		public void SetCurrentEntry(Object entry, bool modifyForwardList, bool pushCurrentOntoBack) 
		{
			// Validate the entry
			this.Validate(entry);
			// Allow changes to be made to entry being set
			entry = this.OnPreChangeCurrentEntry(entry);

			// If the same as the current entry, return.  So multiple identical requests don't 
			// end up at the back of the queue.
			if (entry.Equals(CurrentEntry)) 
			{
				return;
			}

			if (modifyForwardList) 
			{
				this.mForwardList.Clear();
			}
			if (pushCurrentOntoBack) 
			{
				this.mBackList.Insert(0, this.mCurrentEntry);
			}

			this.mCurrentEntry = entry;

			if (pushCurrentOntoBack) 
			{
				this.OnBackListChanged();
			}
			this.OnCurrentEntryChanged(false);
			if (modifyForwardList) 
			{
				this.OnForwardListChanged();
			}
		}
		#endregion
	}



	/// <summary>
	/// Provides for storing the object needed to move forward or back 
	/// in a History with a menu item.
	/// </summary>
	class HistoryMenuItem : MenuItem 
	{
		private Object mHistoryItem;
		public HistoryMenuItem(String text, Object historyItem) : base(text) 
		{
			this.mHistoryItem = historyItem;
		}
		
		public Object HistoryItem
		{
			get 
			{
				return this.mHistoryItem;
			}
		}

		public override string ToString()
		{
			return "HMI (" + mHistoryItem.ToString() + ")";
		}

	}

	/// <summary>
	/// Provides back and forward history menu items and manages keeping 
	/// the menu items synchronized with the history.
	/// </summary>
	public class HistoryMenu : IDisposable
	{
		private MenuItem mBackMenu;
		private MenuItem mForwardMenu;
		protected History mHistory;

		public HistoryMenu(History history) 
		{
			if (history == null) 
			{
				throw new ArgumentNullException("history");
			}
			this.mHistory = history;

			this.mBackMenu = new MenuItem("Back");
			this.mBackMenu.MenuItems.Add(new MenuItem("None"));
			SetBackMenuEnable();

			this.mForwardMenu = new MenuItem("Forward");
			this.mForwardMenu.MenuItems.Add(new MenuItem("None"));
			SetForwardMenuEnable();

			this.mHistory.BackListChanged += new EventHandler(HistoryBackListChanged);
			this.mHistory.ForwardListChanged += new EventHandler(HistoryForwardListChanged);
			this.mBackMenu.Popup += new EventHandler(BackMenuPopup);
			this.mForwardMenu.Popup += new EventHandler(ForwardMenuPopup);
		}

		/// <summary>
		/// Respondes to HistoryBackListChanged, enabling or disabling the back menu item 
		/// as appropriate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HistoryBackListChanged(Object sender, EventArgs args) 
		{
			this.SetBackMenuEnable();
		}

		private void SetBackMenuEnable() 
		{
			this.mBackMenu.Enabled = (this.mHistory.BackList.Count > 0);
		}

		/// <summary>
		/// Called when the submenu of the back menu item is about to be expanded. 
		/// Allows submenu items to be created
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void BackMenuPopup(Object sender, EventArgs args) 
		{
			IList backEntries = this.mHistory.BackList;
			this.mBackMenu.Enabled = (backEntries.Count > 0);

			Console.WriteLine("Back List Popup, Items {0}", backEntries.Count);
			// First dispose of any current menu items - copy to an array list first to 
			// prevent iteration errors
			foreach (MenuItem mi in new ArrayList(this.mBackMenu.MenuItems)) 
			{
				mi.Dispose();
			}
			this.mBackMenu.MenuItems.Clear();
			
			// Create the new menu items
			if (backEntries.Count == 0) 
			{
				// Always keep at least one sub-menu item, so Popup event continues to take place
				MenuItem mi = new MenuItem("None");
				mi.Enabled = false;
				this.mBackMenu.MenuItems.Add(mi);
			}
			for (int i = 0; i < backEntries.Count; i++)
			{
				Object historyEntry = backEntries[i];
				MenuItem mi = new HistoryMenuItem(this.GetMenuString(i, historyEntry), historyEntry);
				mi.Click += new EventHandler(this.BackMenuItemSelected);
				this.mBackMenu.MenuItems.Add(mi);
			}
			
		}

		/// <summary>
		/// Moves back in history to selected item when a menu subitem is clicked by the user.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void BackMenuItemSelected(Object sender, EventArgs args) 
		{
			HistoryMenuItem hmi = (HistoryMenuItem) sender;
			try 
			{
				this.mHistory.MoveBack(hmi.HistoryItem);
			} 
			catch (System.InvalidOperationException ex) 
			{
				// History has been changed in some other manner, do nothing
				Console.WriteLine(ex.StackTrace + " " + ex.Message)  ;
			}
		}

		private void HistoryForwardListChanged(Object sender, EventArgs args) 
		{
			this.SetForwardMenuEnable();
		}

		private void SetForwardMenuEnable() 
		{
			this.mForwardMenu.Enabled = (this.mHistory.ForwardList.Count > 0);
		}

		/// <summary>
		/// Called when the submenu of the forward menu item is about to be expanded. 
		/// Create submenu items for all of the entries currently in the forward list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ForwardMenuPopup(Object sender, EventArgs args) 
		{
			IList forwardEntries = this.mHistory.ForwardList;
			this.mForwardMenu.Enabled = (forwardEntries.Count > 0);

			Console.WriteLine("Forward List Changed, Items {0}", forwardEntries.Count);
			// First dispose of any current menu items - copy to an array list first to 
			// prevent iteration errors
			foreach (MenuItem mi in new ArrayList(this.mForwardMenu.MenuItems)) 
			{
				mi.Dispose();
			}
			this.mForwardMenu.MenuItems.Clear();
			
			// Create the new menu items
			if (forwardEntries.Count == 0)
			{ 
				// Always keep at least one sub-menu item, so Popup event continues to take place
				// Note that we can disable the menu at this point, preventing the 
				// popup from actually taking place.
				MenuItem mi = new MenuItem("None");
				mi.Enabled = false;
				this.mForwardMenu.MenuItems.Add(mi);
			}

			for (int i = 0; i < forwardEntries.Count; i++)
			{
				Object historyEntry = forwardEntries[i];
				MenuItem mi = new HistoryMenuItem(this.GetMenuString(i, historyEntry), historyEntry);
				mi.Click += new EventHandler(this.ForwardMenuItemSelected);
				this.mForwardMenu.MenuItems.Add(mi);
			}
		}

		/// <summary>
		/// Moves forward in history to selected item when a menu subitem is clicked by the user.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ForwardMenuItemSelected(Object sender, EventArgs args) 
		{
			HistoryMenuItem hmi = (HistoryMenuItem) sender;
			try 
			{
				this.mHistory.MoveForward(hmi.HistoryItem);
			} 
			catch (System.InvalidOperationException ex) 
			{
				// Do nothing
				Console.WriteLine(ex.StackTrace + " " + ex.Message)  ;
			}
		}

		/// <summary>
		/// The MenuItem that expands into a listing of entries in the history's Back list.
		/// When an entry is clicked the history is moved forward or back to that 
		/// entry.
		/// </summary>
		public MenuItem BackMenuItem 
		{
			get
			{
				return this.mBackMenu;
			}
		}
		
		/// <summary>
		/// The MenuItem that expands into a listing of entries in the history's Forward list.
		/// When an entry is clicked the history is moved forward or back to that 
		/// entry.
		/// </summary>
		public MenuItem ForwardMenuItem 
		{
			get 
			{
				return this.mForwardMenu;
			}
		}
		
		/// <summary>
		/// Allows subclasses to control how entries in the history are displayed 
		/// as strings in the menu.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="historyEntry"></param>
		/// <returns></returns>
		protected virtual String GetMenuString(int position, Object historyEntry) 
		{
			//Console.WriteLine("History Entry to String: {0}", historyEntry);
			return historyEntry.ToString();
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.mBackMenu.Dispose();
			this.mForwardMenu.Dispose();
		}

		#endregion

	}
}
