using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PNNLControls
{
	public enum ColorInterpolationMode 
	{
		Linear, Log
	}

	public enum PlotParamField 
	{
		InterpolationMode, UseInterpolation, SyncColor, LineColor, LineCap, LineDashStyle, 
		LineWidth, LineDashCap, Visible, MaxHeight, MinHeight, ShapeType, Color, 
		HeightColor, Size, Shape, Name
	}

	/// <summary>
	/// Argument for PlotParamsChanged event
	/// </summary>
	public class PlotParamsChangedEventArgs : EventArgs 
	{
		private PlotParamField mFieldChanged;
		public PlotParamsChangedEventArgs(PlotParamField field) 
		{
			this.mFieldChanged = field;
		}
		public PlotParamField Field 
		{
			get 
			{
				return this.mFieldChanged;
			}
		}
	}

	public delegate void PlotParamsChangedHandler(Object sender, PlotParamsChangedEventArgs args);
	/// <summary>
	/// Summary description for clsPlotParams.
	/// </summary>
	/// 
	public class clsPlotParams : System.ICloneable
	{
		#region Private members
		private ColorInterpolater mColorInterpolater = new SolidColorInterpolater(new Color[] {Color.Black});
		private clsShape mobj_shape = null ;
		private PenProvider mLinePenProvider = new PenProvider();
		private EventHandler mPenProviderChangedHandler;

		private bool mVisible = true;
		private String mName = "Chart Data";
		#endregion
		public event PlotParamsChangedHandler PlotParamsChanged;

		#region Public properties

		public ColorInterpolater Coloring 
		{
			get 
			{
				return (ColorInterpolater) this.mColorInterpolater.Clone();
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("Coloring");
				}
				this.mColorInterpolater = (ColorInterpolater) (value.Clone());
				this.RaisePlotParamsChanged(PlotParamField.Color);
			}
		}

		public PenProvider LinePen
		{
			get 
			{
				return this.mLinePenProvider;
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("Pen");
				}
				if (this.mLinePenProvider != null) 
				{
					this.mLinePenProvider.PenChanged -= this.mPenProviderChangedHandler;
				}
				this.mLinePenProvider = value;
				this.mLinePenProvider.PenChanged += this.mPenProviderChangedHandler;
				this.RaisePlotParamsChanged(PlotParamField.InterpolationMode);
			}
		}


		private void PenProviderChanged(object sender, EventArgs args)
		{
			this.RaisePlotParamsChanged(PlotParamField.InterpolationMode);
		}
		
		public bool Visible
		{
			get
			{
				return mVisible ; 
			}
			set
			{
				mVisible = value ;
				this.RaisePlotParamsChanged(PlotParamField.Visible);
			}
		}

		public clsShape Shape
		{
			get
			{
				return mobj_shape ; 
			}
			set
			{
				mobj_shape = value ;
				this.RaisePlotParamsChanged(PlotParamField.Shape);
			}
		}

		public String Name 
		{
			get 
			{
				return this.mName;
			}
			set 
			{
				this.mName = value;
				this.RaisePlotParamsChanged(PlotParamField.Name);
			}
		}

		#endregion

		public void DrawShape(Graphics g) 
		{
			this.Shape.Draw(g, this.Coloring.Colors[0]);
		}

		public void DrawShape(Graphics g, float zValue) 
		{
			Color c = this.Coloring.GetColor(zValue);
			this.Shape.Draw(g, c);
		}
		
		private void RaisePlotParamsChanged(PNNLControls.PlotParamField field) 
		{
			if (this.PlotParamsChanged != null) 
			{
				this.PlotParamsChanged(this, new PlotParamsChangedEventArgs(field));
			}
		}

		public Derek.BitmapTools.PixelData GetColor(float zValue) 
		{
			Color c = this.mColorInterpolater.GetColor(zValue);
			Derek.BitmapTools.PixelData data = new Derek.BitmapTools.PixelData();
			data.blue = c.B;
			data.red = c.R;
			data.green = c.G;
			return data;
		}

		public clsPlotParams(clsShape shape, Color clr)
		{
			//
			// TODO: Add constructor logic here
			//
			this.Shape = shape;
			Color[] colors = new Color[1];
			colors[0] = clr;
			this.mColorInterpolater.Colors = colors;
			this.mPenProviderChangedHandler = new EventHandler(this.PenProviderChanged);
			this.LinePen = new PenProvider();
			this.LinePen.Color = clr;
			this.LinePen.Width = 1;
		}
		#region ICloneable Members

	    public object Clone()
		{
			clsPlotParams newParams = (clsPlotParams) this.MemberwiseClone();
			newParams.mobj_shape = (clsShape) this.mobj_shape.Clone();
			newParams.mLinePenProvider = (PenProvider) this.mLinePenProvider.Clone();
			return newParams;
		}

		#endregion
	}
}
