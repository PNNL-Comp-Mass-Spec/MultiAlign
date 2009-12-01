using System;
using System.Drawing;
using System.Collections;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace PNNLControls
{

	public delegate void SeriesChangedHandler(Object sender, SeriesChangedEventArgs args);

	public enum SeriesChangedCause 
	{
		Data,
		Style
	}

	public class SeriesChangedEventArgs : EventArgs 
	{
		private SeriesChangedCause mStyle;
		public SeriesChangedEventArgs(SeriesChangedCause cause) 
		{
			mStyle = cause;
		}

		public SeriesChangedCause Cause
		{
			get 
			{
				return mStyle;
			}
		}
	}

	/// <summary>
	/// Provides the x and y and z data points needed for charting a series.
	/// </summary>
	public interface IChartDataProvider 
	{
		ArrayList Values 
		{
			get;
		}

		event EventHandler DataChanged;
	}

	public class ArrayChartDataProvider : IChartDataProvider
	{
		private ArrayList dataPoints;

		public ArrayChartDataProvider(PointF[] points) 
		{
			SetData(points);
		}

		public ArrayChartDataProvider(ArrayList points) 
		{
			dataPoints = new ArrayList();
			for (int i = 0; i < points.Count; i++) 
			{
				if (!(points[i] is ChartDataPoint)) 
				{
					throw new ArgumentException("All points must be of ChartDataPoint type.");
				}
				ChartDataPoint point = (ChartDataPoint) (points[i]);
				dataPoints.Add(new ChartDataPoint(point.x, point.y, point.z, point.color));
			}
		}

		public ArrayChartDataProvider() 
		{
			SetData(new PointF[] {});
		}

		public ArrayChartDataProvider(float[] x, float[] y, float[] z) 
		{
			SetData(x, y, z);
		}

		public ArrayChartDataProvider(float[] x, float[] y) 
		{
			SetData(x, y);
		}

		public void Initialize(float[] x, float[] y, float[] z) 
		{
			PreProcess(ref x, ref y, ref z);
			this.dataPoints = new ArrayList(x.Length);
			for (int i = 0; i < x.Length; i++) 
			{
				this.dataPoints.Add(new ChartDataPoint(x[i], y[i], z[i]));
			}
			if (this.DataChanged != null) 
			{
				this.DataChanged(this, EventArgs.Empty);
			}
		}


		/// <summary>
		/// Allows subclasses to perform custom operations on the points of the 
		/// data provider before they are set as the data provided.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		protected virtual void PreProcess(ref float[] x, ref float[] y, ref float[] z) 
		{
		}

		public event EventHandler DataChanged;

		public ArrayList Values 
		{
			get 
			{
				return (ArrayList) this.dataPoints.Clone();
			}
		}

		public void SetData(float[] x, float[] y) 
		{
			float[] zVals = new float[x.Length];
			for (int i = 0; i < x.Length; i++) 
			{
				zVals[i] = 0;
			}
			Initialize(x, y, zVals);
		}

		public void SetData(float[] x, float[] y, float[] z) 
		{
			Initialize(x, y, z);
		}

		public void SetData(System.Drawing.PointF[] points) 
		{
			float[] x = new float[points.Length];
			float[] y = new float[points.Length];
			float[] z = new float[points.Length];
			
			for (int i = 0; i < points.Length; i++) 
			{
				x[i] = points[i].X;
				y[i] = points[i].Y;
				z[i] = 0;
			}
			Initialize(x, y, z);
		}
	}

	/// <summary>
	/// Summary description for clsSeries.
	/// </summary>
	public class clsSeries
	{
		private IChartDataProvider dataProvider;
		public event SeriesChangedHandler SeriesChanged;

		//values to plot
		//private float[] mdbl_xpoints ;
		//private float[] mdbl_ypoints ;
		//private float mflt_threshold_intensity = float.MinValue ; 
		private ChartDataPoint[] mPlotData;
		// Tells whether the mPlotData array is up to date with the values 
		// of the dataProvider and parameters in mobjPlotParams
		private bool mPlotDataCurrent = false;
		
		//plot parameters
		private clsPlotParams mobjPlotParams = null;
		private PNNLControls.PlotParamsChangedHandler plotParamsChangedHandler;

		protected IChartDataProvider DataProvider 
		{
			get 
			{
				return dataProvider;
			}
		}

		public int DataSize 
		{
			get 
			{
				return this.dataProvider.Values.Count;
			}
		}

		/// <summary>
		/// Used for drag and drop and copy/pasting of series when a series needs 
		/// to be inserted in another chart.
		/// </summary>
		/// <returns></returns>
		public virtual clsSeries CopySeries() 
		{
			clsSeries newSeries = new clsSeries(this.CloneChartDataProvider(), 
				(clsPlotParams) this.PlotParams.Clone());
			return newSeries;
		}

		/// <summary>
		/// Tells whether the given series can be deleted from the given chart.
		/// </summary>
		public virtual bool Deletable(ctlChartBase chartIn)
		{
			return true;
		}

		/// <summary>
		/// Get any series specific menu options that should be available 
		/// in the given chart.  These are displayed in the "Series Special" menu items
		/// </summary>
		/// <param name="chartFor"></param>
		public virtual MenuItem[] GetCustomMenuItems(ctlChartBase chartFor) 
		{
			return new MenuItem[0];
		}

		public virtual bool CopyByReferenceAllowed
		{
			get 
			{
				return true;
			}
		}

		protected virtual IChartDataProvider CloneChartDataProvider() 
		{
			return new ArrayChartDataProvider(this.dataProvider.Values);
		}

		public ChartDataPoint[] PlotData
		{
			get 
			{
				if (!this.mPlotDataCurrent) 
				{
					this.CreatePlotData();
				}
				return this.mPlotData;
			}
		}
		
		public clsPlotParams PlotParams
		{
			get
			{
				if (this.mobjPlotParams == null) 
				{
					return null;
				}
				return mobjPlotParams;
			}
			set
			{
				if (PlotParams != null) 
				{
					PlotParams.PlotParamsChanged -= this.plotParamsChangedHandler;
				}
				mobjPlotParams = value;
				if (value != null) 
				{
					PlotParams.PlotParamsChanged += this.plotParamsChangedHandler;
				}
				this.mPlotDataCurrent = false;
				this.RaiseSeriesChanged(SeriesChangedCause.Style);
			}
		}

		private void RaiseSeriesChanged(SeriesChangedCause cause) 
		{
			if (this.SeriesChanged != null) 
			{
				this.SeriesChanged(this, new SeriesChangedEventArgs(cause));
			}
		}

		private void CreatePlotData() 
		{
			DateTime start = DateTime.Now;
			ArrayList dataPoints = this.dataProvider.Values;
			this.mPlotData = new ChartDataPoint[dataPoints.Count];

			for (int i = 0; i < dataPoints.Count; i++) 
			{
				ChartDataPoint point = (ChartDataPoint) dataPoints[i];
				this.mPlotData[i] = new ChartDataPoint(point.x, point.y, point.z, this.mobjPlotParams.GetColor(point.z));
			}
			this.mPlotDataCurrent = true;
		}

		private void PlotParamsChanged(Object sender, PlotParamsChangedEventArgs args) 
		{
			this.mPlotDataCurrent = false;
			this.RaiseSeriesChanged(SeriesChangedCause.Style);
		}

		private void DataChanged(object sender, EventArgs args) 
		{
			this.mPlotDataCurrent = false;
			this.RaiseSeriesChanged(SeriesChangedCause.Data);
		}

		public clsSeries(ref float[] xdata, ref float[] ydata, ref float[] zdata, clsPlotParams plotParams) :
			this(new ArrayChartDataProvider(xdata, ydata, zdata), plotParams)
		{
		}

		public clsSeries(ref float[] xdata, ref float[] ydata, clsPlotParams plotParams) : 
			this(new ArrayChartDataProvider(xdata, ydata), plotParams)
		{
		}

		public clsSeries(IChartDataProvider data, clsPlotParams plotParams)
		{
			this.dataProvider = data;
			this.dataProvider.DataChanged += new EventHandler(this.DataChanged);
			this.plotParamsChangedHandler = new PlotParamsChangedHandler(PlotParamsChanged);
			this.PlotParams = plotParams;
		}
	}

	public struct ChartDataPlotPoint
	{
		internal Derek.BitmapTools.PixelData pixelColor;
		internal int x;
		internal int y;

		public ChartDataPlotPoint(int x, int y, Derek.BitmapTools.PixelData data) 
		{
			pixelColor = data;
			this.x = x;
			this.y = y;
		}
	}

	public struct ChartDataPoint
	{
		internal float x;
		internal float y;
		internal float z;
		internal Derek.BitmapTools.PixelData color;

		public ChartDataPoint(float x, float y, float z, Derek.BitmapTools.PixelData pixelData) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.color = pixelData;
		}

		public ChartDataPoint(float x, float y, float z) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
			color = new Derek.BitmapTools.PixelData();
		}
	}
}