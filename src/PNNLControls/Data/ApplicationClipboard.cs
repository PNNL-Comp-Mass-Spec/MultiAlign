using System;
using System.Windows.Forms;

namespace PNNLControls
{
	/// <summary>
	/// Implements an Application level clipboard, because items can't be copied by reference 
	/// through the system clipboard.  This allows items to be copied by reference within an 
	/// application (but not outside it), and makes copying objects like clsSeries within the 
	/// application easy (as opposed to successfully copying them to the system clipboard, 
	/// which is very difficult - or at least the documentation is very shoddy).
	/// </summary>
	public class ApplicationClipboard
	{
		/// <summary>
		/// The object copied to the clipboard (or not actually copied to the system 
		/// clipboard, as the case may be.)
		/// </summary>
		private static Object copiedObject = null;

		private ApplicationClipboard()
		{
		}

		/// <summary>
		/// Sets the data object in the application clipboard and the system clipboard.
		/// </summary>
		/// <param name="data"></param>
		public static void SetData(Object data) 
		{
			System.Windows.Forms.Clipboard.SetDataObject(data, false);
			copiedObject = data;
		}

		/// <summary>
		/// Gets a copied data object of the given type, preferring the system clipboard to 
		/// the application clipboard.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Object GetData(Type type) 
		{
			Object o = Clipboard.GetDataObject().GetData(type);
			if (o != null) 
			{
				return o;
			}
			if (copiedObject != null && type.IsAssignableFrom(copiedObject.GetType())) 
			{
				return copiedObject;
			}
			return null;
		}

		/// <summary>
		/// Clears the object in the application clipboard.
		/// </summary>
		public static void ClearApplicationClipboard() 
		{
			copiedObject = null;
		}

		/// <summary>
		/// Tells whether the data object on the application clipboard or the system 
		/// clipboard is of (or can be converted to) the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool HasData(Type type) 
		{
			return (copiedObject != null && type.IsAssignableFrom(copiedObject.GetType())) 
				|| Clipboard.GetDataObject().GetDataPresent(type);
		}
	}
}
