using System;
using System.Collections;

namespace PNNLControls
{
	public delegate void IntChangedHandler(object sender, IntChangedEventArgs args);
	public class IntChangedEventArgs : EventArgs 
	{
		int mVal;
		public IntChangedEventArgs(int val) 
		{
			this.mVal = val;
		}

		public int Value 
		{
			get 
			{
				return mVal;
			}
		}
	}

	/// <summary>
	/// Stores ranges of ints (for example page ranges), for example 1-4,5,9,11-36,97.
	/// </summary>
	public class IntRangeSelector
	{
		// The list of currently selected indexes.
		private SortedList ranges = new SortedList();

		public IntRangeSelector()
		{
		}

		public IntRangeSelector(IntRangeSelector copyFrom) 
		{
			ranges = new SortedList(copyFrom.ranges);
		}

		/// <summary>
		/// Writes out the selected ranges as comma seperated.  Ranges of 2 or more 
		/// are written out with their endpoints seperated by dashes.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			String s = "";
			IList keyList = ranges.GetKeyList();
			for (int i = 0; i < keyList.Count;) 
			{
				int min = (int) keyList[i];
				int j = i + 1;
				while (j < keyList.Count && ((int) keyList[j] == (min + (j - i))))
				{
					j++;
				}
				if (j > i + 1) 
				{
					s += min + "-" + keyList[j - 1] + ",";
				} 
				else 
				{
					s += min + ",";
				}
				i = j;
			}
			// remove final comma
			if (s.Length > 0) 
			{
				s = s.Substring(0, s.Length - 1);
			}
			return s;
		}


		/// <summary>
		/// Removes all ranges.
		/// </summary>
		public void Clear() 
		{
			while (ranges.Count > 0) 
			{
				Remove((int) ranges.GetKey(0));
			}
		}

		/// <summary>
		/// Adds a value.
		/// </summary>
		/// <param name="val"></param>
		public void Add(int val) 
		{
			if (ranges.ContainsKey(val)) 
			{
				return;
			}
			ranges.Add(val, null);
			OnIntAdded(val);
		}

		/// <summary>
		/// Adds a range
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void AddRange(int start, int end) 
		{
			CheckRange(start, end);
			for (int i = start; i <= end; i++) 
			{
				Add(i);
			}
		}

		/// <summary>
		/// Removes the value if its in the ranges of this object, Adds it if it isn't.
		/// </summary>
		/// <param name="val"></param>
		public void Alternate(int val) 
		{
			if (Contains(val)) 
			{
				Remove(val);
			}
			else 
			{
				Add(val);
			}
		}

		public void AlternateRange(int start, int end) 
		{
			CheckRange(start, end);
			for (int i = start; i <= end; i++) 
			{
				Alternate(i);
			}
		}

		/// <summary>
		/// Tells whether the given val is in the included ranges.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public bool Contains(int val) 
		{
			return ranges.Contains(val);
		}

		/// <summary>
		/// Tells whether all values in the given range are included in the ranges of this object.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public bool ContainsRange(int start, int end) 
		{
			CheckRange(start, end);
			bool has = true;
			for (int i = start; i <= end; i++) 
			{
				has = has & Contains(i);
			}
			return has;
		}

		private void CheckRange(int start, int end) 
		{
			if (start > end) 
			{
				throw new ArgumentException("Start must be less than or equal to end.");
			}
		}

		/// <summary>
		/// Removes a given range.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void RemoveRange(int start, int end) 
		{
			CheckRange(start, end);
			for (int i = start; i <= end; i++) 
			{
				Remove(i);
			}
		}

		/// <summary>
		/// Remove a given value.
		/// </summary>
		/// <param name="val"></param>
		public void Remove(int val) 
		{
			if (!ranges.ContainsKey(val)) 
			{
				return;
			}
			ranges.Remove(val);
			OnIntRemoved(val);
		}

		/// <summary>
		/// Gets the collection of integers stored.
		/// </summary>
		public ICollection Values 
		{
			get 
			{
				return ranges.Keys;
			}
		}

		public event IntChangedHandler IntRemoved;
		public event IntChangedHandler IntAdded;

		private void OnIntRemoved(int i) 
		{
			if (this.IntAdded != null) 
			{
				this.IntAdded(this, new IntChangedEventArgs(i));
			}
		}

		private void OnIntAdded(int i) 
		{
			if (this.IntRemoved != null) 
			{
				this.IntRemoved(this, new IntChangedEventArgs(i));
			}
		}
	}
}
