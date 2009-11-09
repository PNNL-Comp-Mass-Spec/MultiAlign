using System;

namespace MultiAlignWin
{
	/// <summary>
	/// Summary description for ConfigParms.
	/// </summary>
	public class clsConfigParms
	{
		// Initial directory when calling Open in frmMSNorm menu.
		private string mstr_open_dir ;
		public clsConfigParms()
		{
			//
			// TODO: Add constructor logic here
			//
			mstr_open_dir = "c:\\" ; 
		}

		#region Properties
		// Set the Initial Directory for the Open comman in frmMSNorm menu.
		public string OpenDir
		{
			get
			{
				return mstr_open_dir ; 
			}
			set
			{
				if (value != null)
				{
					mstr_open_dir = value ;
				}
			}
		}
		#endregion
	}
}
