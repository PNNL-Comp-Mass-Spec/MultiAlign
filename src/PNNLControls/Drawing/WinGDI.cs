using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;


namespace ECsoft.Windows.Forms
{
	/// <summary>
	/// Windows Graphics Device Interface
	/// </summary>
	public sealed class WinGDI
	{
		//	For more information, refer to WinGDI.h
		public const int SRCCOPY	= 0xcc0020;
		public const int SRCERASE	= 0x0440328;
		public const int SRCINVERT	= 0x660046;
		public const int DSTINVERT	= 0x550009;
		public const int SRCAND		= 0x8800C6;
		public const int SRCPAINT	= 0xEE0086;
		
		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(
			IntPtr hdcDest,     // handle to destination DC (device context)
			int nXDest,         // x-coord of destination upper-left corner
			int nYDest,         // y-coord of destination upper-left corner
			int nWidth,         // width of destination rectangle
			int nHeight,        // height of destination rectangle
			IntPtr hdcSrc,      // handle to source DC
			int nXSrc,          // x-coordinate of source upper-left corner
			int nYSrc,          // y-coordinate of source upper-left corner
			System.Int32 dwRop  // raster operation code
			);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDC
			(
				String driverName,
				String deviceName,
				String output,
				IntPtr lpInitData
			);

		[System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
		public static extern bool DeleteDC
			(
				IntPtr dc
			);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern unsafe bool ClientToScreen
			(
				IntPtr hWnd,       // handle to window
				Point* lpPoint     // screen coordinates
			);
	}
}