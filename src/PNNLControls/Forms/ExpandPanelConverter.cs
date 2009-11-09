using System;
using System.ComponentModel.Design.Serialization;
using System.IO;

namespace PNNLControls
{
	/// <summary>
	/// Converts ExpandPanel's to various forms.  Primarily to InstanceDescriptor's for 
	/// use with Visual Studio's design time features.
	/// </summary>
	public class ExpandPanelConverter : System.ComponentModel.ExpandableObjectConverter
	{
		public ExpandPanelConverter()
		{
		}

		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			Log("CanConvertTo: {0}", destinationType);
			if (destinationType == typeof(InstanceDescriptor)) 
			{
				return true;
			}
			return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			Log("ConvertTo: {0}", destinationType);
			if ((destinationType == typeof(InstanceDescriptor)) && (value is ExpandPanel)) 
			{
				ExpandPanel expandPanel = value as ExpandPanel;
				// Get the (int) constructor
				System.Reflection.ConstructorInfo constructorInfo = typeof(ExpandPanel).GetConstructor(new Type [] {typeof(int)});
				// Get the expanded height
				int height = expandPanel.ExpandedContentHeight;
				Log("Height: {0}", height);
				// Create a new instance descriptor which will use the given constructor.
				return new InstanceDescriptor(constructorInfo, new object [] {height}, false) ;
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}

		private void Log(String format, params object[] args) 
		{
//			TextWriter writer = new StreamWriter(@"..\..\ExpandPanelConverterLog.txt", true);
//			writer.WriteLine(format, args);
//			writer.Close();
		}
	}
}
