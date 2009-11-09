using System;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for clsMargins.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class clsMargins
	{
		public event EventHandler MarginsChanged;

		//public int leftMargin = 124;
		public float leftMarginFraction = .2F;
		public int leftMarginMin = 72;
		public int leftMarginMax = 150;
		
		//public int defaultMargin = 10;
		public float defaultMarginFraction = .05F;
		public int defaultMarginMin = 5;
		public int defaultMarginMax = 15;

		//public int bottomMargin = 48;
		public float bottomMarginFraction = .1F;
		public int bottomMarginMin = 30;
		public int bottomMarginMax = 72;

		//public int legendXMargin = 2;
		//public int legendYMargin = 2;

		public clsMargins()
		{
		}

		public int LeftMarginMin 
		{
			get 
			{
				return this.leftMarginMin;
			}
			set 
			{
				this.CheckMin("LeftMarginMin", value);
				this.leftMarginMin = value;
				this.RaiseMarginsChanged();
			}
		}

		public int LeftMarginMax 
		{
			get 
			{
				return this.leftMarginMax;
			}
			set 
			{
				this.CheckMax("LeftMarginMax", value);
				this.leftMarginMax = value;
				this.RaiseMarginsChanged();
			}
		}

		public float LeftMarginFraction 
		{
			get 
			{
				return this.leftMarginFraction;
			}
			set 
			{
				this.CheckFraction("LeftMarginFraction", value);
				this.leftMarginFraction = value;
				this.RaiseMarginsChanged();
			}
		}

		public int BottomMarginMin 
		{
			get 
			{
				return this.bottomMarginMin;
			}
			set 
			{
				this.CheckMin("BottomMarginMin", value);
				this.bottomMarginMin = value;
				this.RaiseMarginsChanged();
			}
		}

		public int BottomMarginMax 
		{
			get 
			{
				return this.bottomMarginMax;
			}
			set 
			{
				this.CheckMax("BottomMarginMax", value);
				this.bottomMarginMax = value;
				this.RaiseMarginsChanged();
			}
		}

		public float BottomMarginFraction 
		{
			get 
			{
				return this.bottomMarginFraction;
			}
			set 
			{
				this.CheckFraction("BottomMarginFraction", value);
				this.bottomMarginFraction = value;
				this.RaiseMarginsChanged();
			}
		}

		public int DefaultMarginMin 
		{
			get 
			{
				return this.defaultMarginMin;
			}
			set 
			{
				this.CheckMin("DefaultMarginMin", value);
				this.defaultMarginMin = value;
				this.RaiseMarginsChanged();
			}
		}

		public int DefaultMarginMax 
		{
			get 
			{
				return this.defaultMarginMax;
			}
			set 
			{
				this.CheckMax("DefaultMarginMax", value);
				this.defaultMarginMax = value;
				this.RaiseMarginsChanged();
			}
		}

		public float DefaultMarginFraction 
		{
			get 
			{
				return this.defaultMarginFraction;
			}
			set 
			{
				this.CheckFraction("DefaultMarginFraction", value);
				this.defaultMarginFraction = value;
				this.RaiseMarginsChanged();
			}
		}

		private void RaiseMarginsChanged() 
		{
			if (this.MarginsChanged != null) 
			{
				this.MarginsChanged(this, EventArgs.Empty);
			}
		}

		private void CheckMin(String argName, int value) 
		{
			CheckMinOrMax(argName, value, "Minimum margin");
		}

		private void CheckMax(String argName, int value) 
		{
			CheckMinOrMax(argName, value, "Maximum margin");
		}

		private void CheckMinOrMax(String argName, int value, String intro) 
		{
			if (value <= 0) 
			{
				throw new ArgumentOutOfRangeException(argName, value, intro + " must be > 0");
			}
		}

		private void CheckFraction(String argName, float value) 
		{
			if (value < 0 || value > 1) 
			{
				throw new ArgumentOutOfRangeException(argName, value, "Fraction must be between 0 and 1");
			}
		}

		private int GetMargin(int size, float fraction, int min, int max) 
		{
			int val = (int) (size * fraction);
			val = Math.Min(max, val);
			val = Math.Max(min, val);
			return val;
		}

		public int GetChartLeftMargin(int width) 
		{
			return GetMargin(width, this.leftMarginFraction, this.leftMarginMin, this.leftMarginMax);
		}

		public int GetDefaultMargin(int size) 
		{
			return GetMargin(size, this.defaultMarginFraction, this.defaultMarginMin, this.defaultMarginMax);
		}

		public int GetChartBottomMargin(int height) 
		{
			return GetMargin(height, this.bottomMarginFraction, this.bottomMarginMin, this.bottomMarginMax);
		}


//		public void SetMargins( int width, int height)
//		{
//			mint_width = width ; 
//			mint_height = height ; 
//
//			mint_xmargin = (mint_height * mint_xaxis_percent)/100 ; 
//			if (mint_xmargin < mint_min_x_margin)
//				mint_xmargin = mint_min_x_margin ; 
//			if (mint_xmargin > mint_max_x_margin)
//				mint_xmargin = mint_max_x_margin ; 
//
//			mint_ymargin = (mint_width * mint_yaxis_percent)/100 ; 
//			if (mint_ymargin < mint_min_y_margin)
//				mint_ymargin = mint_min_y_margin ; 
//			if (mint_ymargin > mint_max_y_margin)
//				mint_ymargin = mint_max_y_margin ; 
//
//			mint_plot_width = mint_width - 2 * mint_ymargin ; // because y margin is perpend. to xaxis.
//			mint_plot_height = mint_height - 2 * mint_xmargin ; // because x margin is perpend. to yaxis.
//		}
	}
}
