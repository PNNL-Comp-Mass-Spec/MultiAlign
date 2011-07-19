using System;
using System.Collections.Generic;
using System.Text;

namespace MultiAlignCore.Filters
{
	public static class FilterUtil<T> where T : class
	{
		public static List<T> FilterList(List<T> tList, List<IFilter<T>> filterList)
		{
			List<T> newFilteredList = new List<T>();
			bool passed = true;

			foreach (T t in tList)
			{
				passed = true;

				foreach (IFilter<T> filter in filterList)
				{
					if (!filter.DoesPassFilter(t))
					{
						passed = false;
						break;
					}
				}

				if (passed)
				{
					newFilteredList.Add(t);
				}
			}

			return newFilteredList;
		}

		public static bool PassesFilters(T t, IList<IFilter<T>> filterList)
		{
			foreach (IFilter<T> filter in filterList)
			{
				if (filter.DoesPassFilter(t) == false)
				{
					return false;
				}
			}

			return true;
		}

		public static bool PassesFilters(IList<T> tList, IList<IFilter<T>> filterList)
		{
			foreach (T t in tList)
			{
				if (!PassesFilters(t, filterList))
				{
					return false;
				}
			}

			return true;
		}
	}
}
