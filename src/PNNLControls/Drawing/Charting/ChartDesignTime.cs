/// Provides design time classes for editing the properties of chart controls 
/// in a property grid.
using System;
using System.Text.RegularExpressions;
using System.Drawing;

namespace PNNLControls
{
	/// <summary>
	/// Provides string conversion for editing the ViewPort of a chart 
	/// in a PropertyGrid.  See the MSDN article 
	/// "Getting the Most Out of the .NET Framework PropertyGrid Control"
	/// </summary>
	public class ViewPortConverter : System.ComponentModel.TypeConverter
	{
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(String)) 
			{
				//System.Windows.Forms.MessageBox.Show("To String Query");
				return true;
			}
			//System.Windows.Forms.MessageBox.Show("Request to " + destinationType);
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (value is RectangleF && destinationType == typeof(System.String)) 
			{
				RectangleF rect = (RectangleF) value;
				return "X: " + rect.Left.ToString("f2") + " to " + rect.Right.ToString("f2")
					+ " Y: " + rect.Top.ToString("f2") + " to " + rect.Bottom.ToString("f2");
			}
			//System.Windows.Forms.MessageBox.Show("Convert to " + destinationType);
			if (value is RectangleF && destinationType == 
				typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)) 
			{
				System.Windows.Forms.MessageBox.Show("Requesting ID");
			}
			return ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(String)) 
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}


		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{

			//System.Windows.Forms.MessageBox.Show("Input " + value.ToString());
			if (value is string) 
			{
				System.Text.RegularExpressions.Regex regex = 
					new Regex(@"^\s*X:\s*(?<xLow>-?((\d+)|(\d*.\d+)))\s*to\s*(?<xHigh>-?((\d+)|(\d*.\d+)))\s*"  +
					@"Y:\s*(?<yLow>-?((\d+)|(\d*.\d+)))\s*to\s*(?<yHigh>-?((\d+)|(\d*.\d+)))\s*$");
				
				Match m = regex.Match((string) value);
				if (!m.Success) 
				{
					//System.Windows.Forms.MessageBox.Show("Can not parse " + value.ToString());
					throw new Exception("Can not parse input");
				}
				float xLow = float.Parse(m.Groups["xLow"].Value);
				float xHigh = float.Parse(m.Groups["xHigh"].Value);
				float yLow = float.Parse(m.Groups["yLow"].Value);
				float yHigh = float.Parse(m.Groups["yHigh"].Value);

				//System.Windows.Forms.MessageBox.Show("X: " + xLow + " " + (xHigh - xLow) + 
				//	 "  Y: " + yLow + " " + (yHigh - yLow));
																					
				return new RectangleF(xLow, yLow, xHigh - xLow, yHigh - yLow) ;
			}
			return base.ConvertFrom (context, culture, value);
		}
	}

	/// <summary>
	/// Provides a graphical editor for the ViewPort of a chart for use 
	/// in a PropertyGrid
	/// </summary>
	public class ViewPortEditor : System.Drawing.Design.UITypeEditor 
	{
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (value is RectangleF) 
			{
				ctlViewPortEditor control = new ctlViewPortEditor();
				control.SelectedValue = (RectangleF) value;
				System.Windows.Forms.Design.IWindowsFormsEditorService service = 
					(System.Windows.Forms.Design.IWindowsFormsEditorService)
					provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));
				service.DropDownControl(control);
				return control.SelectedValue;
			}
			return base.EditValue(context, provider, value);
		}
	}

	/// <summary>
	/// Provides a graphical editor for editing Pens.
	/// </summary>
	public class PenProviderEditor : System.Drawing.Design.UITypeEditor 
	{
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (value is PenProvider) 
			{
				ctlPenProviderEditor control = new ctlPenProviderEditor();
				control.PenProvider = (PenProvider) value;
				System.Windows.Forms.Design.IWindowsFormsEditorService service = 
					(System.Windows.Forms.Design.IWindowsFormsEditorService)
					provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));
				service.DropDownControl(control);
				return control.PenProvider;
			}
			return base.EditValue(context, provider, value);
		}
	}

	/// <summary>
	/// Supports design time/property grid showing of PenProvider.  Also handles code 
	/// generation for PenProvider via the InstanceDescriptor returned from the ConvertTo method.
	/// </summary>
	public class PenProviderConverter : System.ComponentModel.ExpandableObjectConverter
	{
		private void Log(String format, params Object[] filler) 
		{
			//System.IO.StreamWriter writer = System.IO.File.AppendText(@"C:\Documents and Settings\d3k914\My Documents\Projects\PenProviderConverter.txt");
			//writer.WriteLine(format, filler);
			//writer.Close();
		}

		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			Log("PPC: Requested {0}", destinationType);
			if (destinationType == typeof(String)) 
			{
				return true;
			}
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)) 
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			Log("PPC: Converting {0}", destinationType);
			if (value != null && destinationType == typeof(String))
			{
				return "System.Drawing.Pen";
			}
			if (value != null && destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)) 
			{
				// Return an instance descriptor that says how to generate code for the PenProvider.
				// In this case, we can always just use the empty constructor, and rely on Visual
				// Studio to set the rest of the properties.
				System.Reflection.ConstructorInfo ci = typeof(PenProvider).GetConstructor(new Type[] {});
				return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new Object[] {}, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
