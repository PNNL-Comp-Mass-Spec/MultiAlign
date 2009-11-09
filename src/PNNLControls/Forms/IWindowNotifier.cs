using System;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Provides events for window opening and closing.
	/// </summary>
	public interface ICategorizedItemNotifier
	{
		/// <summary>
		/// Fired when a significant window is opened.
		/// </summary>
		event ItemChangedHandler ItemOpen;

		/// <summary>
		/// Fired when a window that was previously sent to WindowOpen is closed.
		/// </summary>
		event ItemChangedHandler ItemClose;
	}

	public delegate void ItemChangedHandler(object sender, ItemChangedEventArgs args);

	/// <summary>
	/// Event args for WindowOpened and WindowClosed events.
	/// </summary>
	public class ItemChangedEventArgs : EventArgs 
	{
		private ICategorizedItem mItem;

		public ItemChangedEventArgs(ICategorizedItem item) 
		{
			if (item == null) 
			{
				throw new ArgumentNullException("item");
			}
			mItem = item;
		}

		public ICategorizedItem Item 
		{
			get 
			{
				return mItem;
			}
		}
	}
}
