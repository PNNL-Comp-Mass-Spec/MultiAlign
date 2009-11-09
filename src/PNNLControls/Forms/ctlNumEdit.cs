using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlNumEdit.
	/// </summary>
	public class ctlNumEdit : System.Windows.Forms.TextBox
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public delegate void ValueChangedDelegate (double val, double previous);
		public event ValueChangedDelegate ValueChanged = null;

		private double mCurrentValue = 0.0;
		public double Value 
		{
			get
			{
				//ValidateNumeric();
				//this.Text = mCurrentValue.ToString();
				return mCurrentValue;
			}
			set
			{
				mCurrentValue=value;
				this.Text = value.ToString();
			}
		}

		private double mMinValue = double.MinValue;
		public double MinValue 
		{
			get{return mMinValue;}
			set{mMinValue=value;}
		}

		private double mMaxValue = double.MaxValue;
		public double MaxValue 
		{
			get{return mMaxValue;}
			set{mMaxValue=value;}
		}
		public ctlNumEdit()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ctlNumEdit
			// 
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ctlNumEdit_KeyPress);
			this.TextChanged += new System.EventHandler(this.ctlNumEdit_TextChanged);
			this.Leave += new System.EventHandler(this.ctlNumEdit_Leave);

		}
		#endregion

		private void ctlNumEdit_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			string ls_AllowedKeyChars = "1234567890+-eE.\b";

			if(e.KeyChar == (char)13)
				ValidateNumeric();
			else if (ls_AllowedKeyChars.IndexOf(e.KeyChar) < 0)  
			{
				e.Handled = true;
			}
		}

		private void ValidateNumeric()
		{
			double prev = mCurrentValue;

			try
			{
				double result = 0.0;

				bool success = double.TryParse(this.Text, NumberStyles.Any, null, out result);
				if (success)
				{
					mCurrentValue = result;
				}
				if (mCurrentValue<mMinValue)
					mCurrentValue = mMinValue;
				if (mCurrentValue>mMaxValue)
					mCurrentValue = mMaxValue;
			}
			catch
			{
			}

			if (ValueChanged != null)
				ValueChanged(mCurrentValue, prev);
		}
		private void ctlNumEdit_TextChanged(object sender, System.EventArgs e)
		{
			ValidateNumeric();			
		}

		private void ctlNumEdit_Leave(object sender, System.EventArgs e)
		{
			ValidateNumeric();
			this.Text = mCurrentValue.ToString();
		}
	}
}
