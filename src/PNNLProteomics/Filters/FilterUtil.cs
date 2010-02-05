using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLProteomics.Filters
{
	public class FilterUtil<T> where T : class
	{
		public List<T> FilterList(List<T> tList, List<IFilter<T>> filterList)
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
	}
}
