using System;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ColorInterpolater.
	/// </summary>
	public abstract class ColorInterpolater : ICloneable
	{
		/// <summary>
		/// The list of colors to interpolate between.  
		/// This list is set through the Colors property.
		/// </summary>
		private Color[] userColors;
		
		/// <summary>
		/// The list of interpolated colors that are used to look up the color for a given zValue.
		/// This list is created in the InitializeColorArray method.
		/// </summary>
		private Color[] interpolatedColors;

		/// <summary>
		/// The number of colors in the interpolatedColors array.  
		/// This is set through the Gradations property.
		/// </summary>
		private int gradations = 100;

		public ColorInterpolater(Color[] colors, int gradations)
		{
			if (gradations <= 0) 
			{
				throw new ArgumentException("Must be > 0", "Gradations");
			}
			this.gradations = gradations;
			this.Colors = colors;
		}

		public ColorInterpolater() 
		{
			this.userColors = new Color[1];
			this.userColors[0] = Color.Black;
			this.InitializeColorArray();
		}

		public Color GetColor(float zValue) 
		{
			float arrayFraction = GetColorFraction(zValue);
			if (arrayFraction <= 0) 
			{
				return this.interpolatedColors[0];
			}
			if (arrayFraction >= 1) 
			{
				return this.interpolatedColors[interpolatedColors.Length - 1];
			}
			return this.interpolatedColors[(int) (interpolatedColors.Length * arrayFraction)];
		}

		/// <summary>
		/// Gets the fraction of the way into the color array that the color for the given zValue is found at.
		/// &gt;=1 causes the last value in the array to be used.  &lt;=0 causes the 
		/// first value in the array to be used.
		/// </summary>
		/// <param name="zValue"></param>
		/// <returns></returns>
		protected abstract float GetColorFraction(float zValue);

		#region "Properties"

		/// <summary>
		/// The total number of interpolated colors.  If this number is equal to the 
		/// number of entries in the Colors collection, then only the colors in that collection
		/// will be used.  Otherwise the colors in that collection will be interpolated to get the 
		/// specified number of gradations.
		/// </summary>
		public int Gradations 
		{
			get 
			{
				return this.gradations;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentException("Number of gradations must be positive.", "Gradations");
				}
				if (value >= 1000) 
				{
					throw new ArgumentException("Number of gradations can not be greater than 1000.", "Gradations");
				}
				if (value < this.userColors.Length) 
				{
					value = this.userColors.Length;
				}
				this.gradations = value;
				this.InitializeColorArray();
			}
		}

		public Color[] InterpolatedColors 
		{
			get 
			{
				return (Color[]) this.interpolatedColors.Clone();
			}
		}

		public virtual Color[] Colors 
		{
			get 
			{
				return (Color[]) this.userColors.Clone();
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("Colors");
				}
				if (value.Length < 1) 
				{
					throw new ArgumentException("User colors must have at least one element.", "Colors");
				}
				userColors = (Color[]) value.Clone();
				// Sets the number of gradations at least equal to the number of elements 
				// in the collection.
				this.Gradations = this.Gradations;
				this.InitializeColorArray();
			}
		}
		#endregion

		#region "Color Interpolation"
		/// <summary>
		/// Computes the Value part of a color in HSV space.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private float Value(Color c) 
		{
			return ((float) (Math.Max(Math.Max(c.R, c.G), c.B))) / 255;
		}

		/// <summary>
		/// Computes the Saturation part of a color in HSV space.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private float Saturation(Color c) 
		{
			int min = Math.Min(Math.Min(c.R, c.G), c.B);
			int max = Math.Max(Math.Max(c.R, c.G), c.B);
			if (max == 0) 
			{
				return 0;
			}
			int diff = max - min;
			float sat = ((float) diff) / max;
			return sat;
		}

		/// <summary>
		/// Computes the Hue of a color in HSV space.  If the color c is a gray tone, the 
		/// hue returned is the hue of the towards color.  This makes for good interpolation (instead
		/// of assuming the hue of a gray is that of red.)
		/// </summary>
		/// <param name="c"></param>
		/// <param name="towards"></param>
		/// <returns></returns>
		private float Hue(Color c, Color towards) 
		{
			if (c.R == c.B && c.R == c.G) 
			{
				return towards.GetHue();
			}
			return c.GetHue();
		}

		
		/// <summary>
		/// Interpolates between two colors by the given amount.
		/// </summary>
		/// <param name="fromColor"></param>
		/// <param name="toColor"></param>
		/// <param name="factor">The fraction to interpolate between the two colors, on a 
		/// 0 to 1 scale.  0 corresponds to fromColor, 1 to toColor.</param>
		/// <returns></returns>
		public Color Interpolate(Color fromColor, Color toColor, float factor) 
		{
			if (factor <= 0) 
			{
				return fromColor;
			}
			if (factor >= 1) 
			{
				return toColor;
			}
			
			float hueDist = Hue(toColor, fromColor) - Hue(fromColor, toColor);
			float hueSign = Math.Sign(hueDist);
			if (Math.Abs(hueDist) >= 180) 
			{
				if (hueSign > 0) 
				{
					hueDist = hueDist - 360;
				} 
				else 
				{
					hueDist = hueDist + 360;
				}
			}
			float hue = hueDist * factor + Hue(fromColor, toColor);
			float satur =  (Saturation(toColor) - Saturation(fromColor)) * factor + Saturation(fromColor);
			float value = (Value(toColor) - Value(fromColor)) * factor + Value(fromColor);

			// for ( this code to work, HSV.Hue needs
			// to be scaled from 0 to 360 (it//s the angle of the selected
			// point within the circle). HSV.Saturation and HSV.value must be 
			// scaled to be between 0 and 1.

			// Scale Hue to be between 0 and 360. Saturation
			// and value scale to be between 0 and 1.
			double h = ((hue % 360) + 360) % 360;
			double s = satur;
			double v = value;
			double r = 0;
			double b = 0;
			double g = 0;

			if ( s == 0 ) 
			{
				// If s is 0, all colors are the same.
				// This is some flavor of gray.
				r = v;
				g = v;
				b = v;
			} 
			else 
			{
				double p;
				double q;
				double t;

				double fractionalSector;
				int sectorNumber;
				double sectorPos;

				// The color wheel consists of 6 sectors.
				// Figure out which sector you're in.
				sectorPos = h / 60;
				sectorNumber = (int)(Math.Floor(sectorPos));

				// get the fractional part of the sector.
				// That is, how many degrees into the sector
				// are you?
				fractionalSector = sectorPos - sectorNumber;

				// Calculate values for the three axes
				// of the color. 
				p = v * (1 - s);
				q = v * (1 - (s * fractionalSector));
				t = v * (1 - (s * (1 - fractionalSector)));

				// Assign the fractional colors to r, g, and b
				// based on the sector the angle is in.
				switch (sectorNumber) 
				{
					case 0:
						r = v;
						g = t;
						b = p;
						break;

					case 1:
						r = q;
						g = v;
						b = p;
						break;

					case 2:
						r = p;
						g = v;
						b = t;
						break;

					case 3:
						r = p;
						g = q;
						b = v;
						break;

					case 4:
						r = t;
						g = p;
						b = v;
						break;

					case 5:
						r = v;
						g = p;
						b = q;
						break;
				}
			}
			return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
		}

		/// <summary>
		/// Interpolates the colors in the Colors array into as many colors as are 
		/// required by the Gradations setting.
		/// </summary>
		protected virtual void InitializeColorArray() 
		{
			int count = this.Gradations;
			this.interpolatedColors = new Color[count];
			if (count == 1) 
			{
				this.interpolatedColors[0] = this.userColors[0];
				return;
			}
			for (int i = 0; i < count; i++) 
			{
				// find the index of the color to interpolate from in the Colors array
				float userColorAtFloat = ((float) i) / (count - 1) * (this.Colors.Length - 1);
				int userColorAt = (int) userColorAtFloat;
				Color interpolatedColor;
				Color fromColor = this.userColors[userColorAt];

				if (userColorAt + 1 >= this.userColors.Length) 
				{
					interpolatedColor = fromColor;
				} 
				else 
				{
					Color toColor = this.userColors[userColorAt + 1];
					interpolatedColor = this.Interpolate(fromColor, toColor, userColorAtFloat % 1);
				}
				this.interpolatedColors[i] = interpolatedColor;				
			}
		}
		#endregion

		#region ICloneable Members

		public Object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}

	/// <summary>
	/// Provides high and low bounds for color interpretation, but leaves the 
	/// determination of what color to use (where in the color array to index) up 
	/// to subclasses.
	/// </summary>
	public abstract class BoundedColorInterpolater : ColorInterpolater 
	{
		protected float lowValue = 0;
		protected float highValue = 1;

		public BoundedColorInterpolater(Color[] colors, int gradations, float low, float high) 
			: base(colors, gradations)
		{
			this.SetBounds(low, high);
		}

		public BoundedColorInterpolater() : base() 
		{
		}

		public virtual void SetBounds(float low, float high) 
		{
			this.CheckValues(low, high);
			this.highValue = high;
			this.lowValue = low;
		}

		protected virtual void CheckValues(float low, float high) 
		{
			if (low >= high) 
			{
				throw new ArgumentException("Bounded values must obey low < hi");
			}
		}

		public virtual float LowValue 
		{
			get 
			{
				return this.lowValue;
			}
			set 
			{
				CheckValues(value, this.highValue);
				this.lowValue = value;
			}
		}

		public virtual float HighValue 
		{
			get 
			{
				return this.highValue;
			}
			set 
			{
				this.CheckValues(this.lowValue, value);
				this.highValue = value;
			}
		}
	}

	/// <summary>
	/// Provides linear z-Axis gradient shading.
	/// </summary>
	public class LinearColorInterpolater : BoundedColorInterpolater 
	{
		public LinearColorInterpolater() 
		{
			this.LowValue = .01F;
			this.HighValue = 1F;
		}

		public LinearColorInterpolater(Color[] colors, int gradations, float low, float high) :
			base(colors, gradations, low, high) 
		{

		}

		protected override float GetColorFraction(float zValue)
		{
			if (zValue <= this.lowValue) 
			{
				return 0;
			}
			if (zValue >= this.highValue) 
			{
				return 1;
			}
			return (zValue - this.lowValue) / (this.highValue - this.lowValue);
		}

	}

	/// <summary>
	/// Provides logarithmic z-Axis gradient shading.  Stores high and low values
	/// after taking the logarithm.
	/// </summary>
	public class LogColorInterpolater : BoundedColorInterpolater 
	{
		public LogColorInterpolater() : base()
		{
		}

		public LogColorInterpolater(Color[] colors, int gradations, float low, float high) :
			base(colors, gradations, low, high) 
		{
			this.lowValue = (float) Math.Log(low);
			this.highValue = (float) Math.Log(high);
		}

		protected override void CheckValues(float low, float high)
		{
			base.CheckValues (low, high);
			if (low <= 0 || high <= 0) 
			{
				throw new ArgumentException("High and low must be positive for log value");
			}
		}

		public override void SetBounds(float low, float high)
		{
			base.SetBounds (low, high);
			this.highValue = (float) Math.Log(high);
			this.lowValue = (float) Math.Log(low);
		}

		public override float HighValue
		{
			get
			{
				return (float) Math.Exp(base.HighValue);
			}
			set
			{
				base.HighValue = (float) Math.Log(value);
			}
		}

		public override float LowValue
		{
			get
			{
				return (float) Math.Exp(base.LowValue);
			}
			set
			{
				base.LowValue = (float) Math.Log(value);
			}
		}

		protected override float GetColorFraction(float zValue)
		{
			if (zValue < 0) 
			{
				return 0;
			}
			float logValue = (float) Math.Log(zValue);
			if (logValue < this.lowValue) 
			{
				return 0;
			}
			if (logValue > this.highValue) 
			{
				return 1;
			}
			return (logValue - lowValue) / (highValue - lowValue);
		}
	}

	/// <summary>
	/// Provides for quick interpolation when using only a single color.
	/// </summary>
	public class SolidColorInterpolater : PNNLControls.ColorInterpolater
	{
		public SolidColorInterpolater(Color[] colors) : base(colors, 1)
		{
		}

		protected override float GetColorFraction(float zValue)
		{
			return 0;
		}

		/// <summary>
		/// Overrides the base property.  Only set the first color of the given array.
		/// </summary>
		public override Color[] Colors
		{
			get
			{
				return base.Colors;
			}
			set
			{
				base.Colors = value;
				if (Colors.Length > 1) 
				{
					base.Colors = new Color[] {this.Colors[0]};
				}
				this.Gradations = 1;
			}
		}

	}
}
