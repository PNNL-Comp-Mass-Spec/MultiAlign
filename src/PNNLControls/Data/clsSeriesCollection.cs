using System;
using System.Collections;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsSeriesCollection.
	/// </summary>
	public class clsSeriesCollection : CollectionBase
	{
		private ctlChartBase mControl;

		public clsSeriesCollection(ctlChartBase control) 
		{
			mControl = control;
		}

		public void Add(clsSeries series) 
		{
			this.List.Add(series);
		}

		public int IndexOf(clsSeries series) 
		{
			return this.List.IndexOf(series);
		}

		public void Remove(clsSeries series) 
		{
			this.List.Remove(series);
		}

		public void Insert(int index, clsSeries series) 
		{
			this.List.Insert(index, series);
		}

		public clsSeries this[int index] 
		{
			get 
			{
				return (clsSeries) this.List[index];
			}
			set 
			{
				this.List[index] = value;
			}
		}

		protected override void OnClear()
		{
			base.OnClear ();
			foreach (clsSeries series in this) 
			{
				mControl.SeriesRemoved(series);
			}
		}

		protected override void OnRemove(int index, object value)
		{
			base.OnRemove (index, value);
			CheckInput(value);
			mControl.SeriesRemoved((clsSeries) value);
		}


		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete (index, oldValue, newValue);
			CheckInput(oldValue);
			CheckInput(newValue);
			mControl.SeriesRemoved((clsSeries) oldValue);
			mControl.SeriesAdded((clsSeries) newValue);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete (index, value);
			CheckInput(value);
			mControl.SeriesAdded((clsSeries) value);
		}

		private void CheckInput(object value) 
		{
			if (value == null) 
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is clsSeries)) 
			{
				throw new ArgumentException("Value must be of clsSeries type.", "value");
			}
		}
	}
}
