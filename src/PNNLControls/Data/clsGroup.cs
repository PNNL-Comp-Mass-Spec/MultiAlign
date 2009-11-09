using System;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsGroup.
	/// </summary>
	public class clsGroup
	{
		private string mstr_name ; 
		public clsGroup [] marr_members ;
		public clsGroup()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public string Name
		{
			get
			{
				return mstr_name ;
			}
			set
			{
				mstr_name = value ; 
			}
		}
	}
}
