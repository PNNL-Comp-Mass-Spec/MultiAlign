using System;

namespace PNNLControls
{
	/// <summary>
	/// Provides a few useful functions not provided by the .NET framework.
	/// </summary>
	public class MenuUtils
	{
		private MenuUtils() {}

		// The external function that can set the maximum height of a menu.
		// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/resources/menus/menureference/menufunctions/setmenuinfo.asp
		// and http://www.thecodeproject.com/cs/menu/MenuEx.asp for information.
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int SetMenuInfo(IntPtr hmenu, ref MENUINFO mi);

		// See same places as for SetMenuInfo
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		struct MENUINFO 
		{
			internal int cbSize;
			internal int fMask;
			internal int dwStyle;
			internal int cyMax;
			internal IntPtr hbrBack;
			internal int dwContextHelpID;
			internal int dwMenuData;

			/// <summary>
			/// Creates a MENUINFO that can be used to set the maximum height of 
			/// a menu.
			/// </summary>
			/// <param name="sizeInPixels"></param>
			public MENUINFO(int sizeInPixels) 
			{
				if (sizeInPixels < 0) 
				{
					throw new System.ArgumentOutOfRangeException("sizeInPixels", sizeInPixels, "Must be >= 0");
				}
				// The size field must be set to the size of the object
				cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MENUINFO));
				// The field mask must be set to indicate that only the size is being 
				// set.  
				fMask = 1;
				dwStyle = 0;
				// The height information is set to the desired size, since the 
				// mask specifies only this field, the other values are unimportant.
				// So let them be 0.
				cyMax = sizeInPixels;
				hbrBack = IntPtr.Zero;
				dwContextHelpID = 0;
				dwMenuData = 0;
			}
		}

		/// <summary>
		/// Sets the maximum height of a menu or menu item.  If there are enough entries in the menu 
		/// to make it more than the given height, then Windows will provide scrolling functionality.
		/// </summary>
		/// <param name="menu"></param>
		/// <param name="sizeInPixels"></param>
		public static void SetMaximumMenuHeight(System.Windows.Forms.Menu menu, int sizeInPixels) 
		{
			if (menu == null) 
			{
				throw new ArgumentNullException("menu");
			}
			
			MENUINFO mi = new MENUINFO(sizeInPixels);
			MenuUtils.SetMenuInfo(menu.Handle, ref mi);
		}
	}
}
