using System;

namespace UMCManipulation
{
	/// <summary>
	/// Class used for sorting pairs of key, value pairs based on key.
	/// </summary>
	public class clsPair : IComparable
	{
		public int mint_key ; 
		public object mobj_val ;

		public clsPair()
		{
			mint_key = 0 ; 
			mobj_val = null ; 
		}

		public clsPair(int key, object val)
		{
			mint_key = key ; 
			mobj_val = val ; 
		}

		public void Set(int key, object val)
		{
			mint_key = key ; 
			mobj_val = val ; 
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			clsPair pt = (clsPair) obj ; 
			// TODO:  Add clsPair.CompareTo implementation
			return mint_key.CompareTo(pt.mint_key) ; 
		}

		#endregion

	}
}
