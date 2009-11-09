using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Reflection;
using System.IO;

namespace PNNLControls
{
	/// <summary>
	/// Provides access to icons registered in windows for file types
	/// </summary>
	public class IconUtils
	{
		private IconUtils()
		{
		}

		/// <summary>
		/// The shell file info structure.  See 
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/shellcc/platform/shell/reference/structures/shfileinfo.asp
		/// for documentation on this type.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};


		/// <summary>
		/// API Constants passed to SHGetFileInfo to indicate the type of icon desired
		/// </summary>
		private const uint SHGFI_ICON				= 0x100;	// get icon, as opposed to getting the index of the icon in the system imagelist
		private const uint SHGFI_LINKOVERLAY		= 0x8000;	// put a link overlay on icon
		private const uint SHGFI_SELECTED			= 0x10000;	// show icon in selected state
		private const uint SHGFI_LARGEICON			= 0x0;		// get large icon
		private const uint SHGFI_SMALLICON			= 0x1;		// get small icon
		private const uint SHGFI_OPENICON			= 0x2;		// get open icon
		private const uint SHGFI_SHELLICONSIZE		= 0x4;		// get shell size icon
		private const uint SHGFI_USEFILEATTRIBUTES	= 0x10;		// use to indicate that the path of the file in question does not have to actually exist
		private const uint SHGFI_TYPENAME			= 0x400;	// get shell description of file type

		/// <summary>
		/// The windows function to call to get an icon for a file type.
		/// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/shellcc/platform/shell/reference/functions/shgetfileinfo.asp
		/// for documentation on this method.
		/// </summary>
		/// <param name="pszPath"></param>
		/// <param name="dwFileAttributes"></param>
		/// <param name="psfi"></param>
		/// <param name="cbSizeFileInfo"></param>
		/// <param name="uFlags"></param>
		/// <returns></returns>
		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath,
			uint dwFileAttributes,
			ref SHFILEINFO psfi,
			uint cbSizeFileInfo,
			uint uFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr DestroyIcon( IntPtr hIcon );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="?"></param
		public static Icon GetIconForFileType(String fileExt, IconSize size, bool selected, bool open, bool linkOverlay) 
		{
			return GetIconForFileName("." + fileExt, size, selected, open, linkOverlay, true);
		}

		public static Icon GetIconForFileName(String name, IconSize size, bool selected, bool open, 
			bool linkOverlay, bool useInfo) 
		{
			SHFILEINFO info = new SHFILEINFO();
			// We want to get the icon
			uint flags = SHGFI_ICON;
			flags |= useInfo ? SHGFI_USEFILEATTRIBUTES : 0;

			// Select the size of icon we want to retrieve.
			switch (size) 
			{
				case IconSize.Large :
					flags |= SHGFI_LARGEICON;
					break;
				case IconSize.Small : 
					flags |= SHGFI_SMALLICON;
					break;
				case IconSize.Shell : 
					flags |= SHGFI_SHELLICONSIZE;
					break;
					// default is none, which is same as large
			}
			
			flags |= selected ? SHGFI_SELECTED : 0;
			flags |= open ? SHGFI_OPENICON : 0;
			flags |= linkOverlay ? SHGFI_LINKOVERLAY : 0;
			// Call the windows function to get the icon
			IntPtr result = SHGetFileInfo(name, (uint) 0, ref info, (uint)Marshal.SizeOf(info), flags);
			//Console.WriteLine("In handle: {0}", info.hIcon);
			// Create a managed icon
			if(result.Equals(IntPtr.Zero) || info.hIcon.Equals(IntPtr.Zero)) 
			{
				return null;
			}
			return Icon.FromHandle(info.hIcon);
		}

		/// <summary>
		/// Looks up the system icon for a given file.  The file must exist, or null will be returned.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="size"></param>
		/// <param name="selected"></param>
		/// <param name="open"></param>
		/// <param name="linkOverlay"></param>
		/// <returns></returns>
		public static Icon GetIconForFile(String name, IconSize size, bool selected, bool open, bool linkOverlay) 
		{
			return GetIconForFileName(name, size, selected, open, linkOverlay, false);
		}

		/// <summary>
		/// Attempts to load icons for the given file names, returning the first name 
		/// for which an icon is found (the first existant file).  Returns null if no 
		/// image is found.
		/// </summary>
		/// <param name="names"></param>
		/// <param name="size"></param>
		/// <param name="selected"></param>
		/// <param name="open"></param>
		/// <param name="linkOverlay"></param>
		/// <returns></returns>
		public static Icon AttemptIconForFile(IEnumerable names, IconSize size, bool selected, 
			bool open, bool linkOverlay) 
		{
			foreach (String name in names) 
			{
				Icon icon = GetIconForFile(name, size, selected, open, linkOverlay);
				if (icon != null) return icon;
			}
			return null;
		}

		/// <summary>
		/// Loads an icon embedded in an assembly, or returns null if the 
		/// embedded resource can not be found.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static Icon LoadIconFromAssembly(Type type, String location, int width, int height) 
		{
			Stream stream = GetStreamFromAssembly(type, location);
			if (stream == null) return null;
			return new Icon(stream, width, height);
		}

		/// <summary>
		/// Gets the stream for an embedded resource
		/// </summary>
		/// <param name="type"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		private static Stream GetStreamFromAssembly(Type type, String location) 
		{
			Assembly assembly = type.Assembly;
			// I don't think this can ever happen
			if (assembly == null) 
			{
				return null;
			}
			Stream stream = assembly.GetManifestResourceStream(type, location);
			return stream;
		}

		/// <summary>
		/// Loads an icon embedded in an assembly, or returns null if the 
		/// embedded resource can not be found.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static Icon LoadIconFromAssembly(Type type, String location) 
		{
			
			Stream stream = GetStreamFromAssembly(type, location);
			if (stream == null) return null;
			return new Icon(stream);
		}

		/// <summary>
		/// Loads an image embedded in an assembly, or returns null if the 
		/// embedded resource can not be found.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static Image LoadImageFromAssemblyType(Type type, String location) 
		{
			Stream stream = GetStreamFromAssembly(type, location);
			if (stream == null) return null;
			return Image.FromStream(stream);
		}
	}

	public enum IconSize{ Large, Small, Shell };
}
