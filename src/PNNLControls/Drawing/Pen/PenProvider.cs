using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace PNNLControls
{
	/// <summary>
	/// Provides a wrapper around a pen.  This allows for design-time editing of a pen.
	/// </summary>
	// Having an editor it pointless, since any changes made in it (in the designer) are not 
	// written to code.  Probably needs another design attribute to make it work, but I can't 
	// figure it out.
	// 8/12/2005 Actually, the secret is to make the PenProviderConverter handle conversions 
	// to System.ComponentModel.Design.Serialization.InstanceDescriptor.  Then Visual Studio
	// handles the rest and the editor works.
	[System.ComponentModel.Editor(typeof(PenProviderEditor), typeof(System.Drawing.Design.UITypeEditor))]
	[System.ComponentModel.TypeConverter(typeof(PenProviderConverter))]
	public class PenProvider : ICloneable
	{
		private Pen mPen = new Pen(Color.Black, 1);
		private EventHandler mPenChangedHandler;

		/// <summary>
		/// Use custom add/remove code so that we can implement clone in an intelligent 
		/// manner.
		/// </summary>
		public event EventHandler PenChanged 
		{
			add 
			{
				mPenChangedHandler += value;
			}
			remove 
			{
				mPenChangedHandler -= value;
			}
		}


		/// <summary>
		/// Required designer variable.
		/// </summary>

		public PenProvider() 
		{
			this.InitializePen();
		}
	
		public PenProvider(Pen p) 
		{
			this.Pen = p;
		}

		private void InitializePen() 
		{
			this.DashCap = DashCap.Flat;
			this.DashStyle = DashStyle.Solid;
			this.EndCap = LineCap.Flat;
			this.StartCap = LineCap.Flat;
		}

		public override string ToString()
		{
			return "Pen Provider";
		}


		/// <summary>
		/// Gets a clone of the pen being configured by this provider.  Or sets the 
		/// pen being configured to a clone of the given value.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public Pen Pen 
		{
			get 
			{
				return (Pen) this.mPen.Clone();
			}
			set 
			{
				if (value == null) 
				{
					throw new ArgumentNullException("Pen");
				}
				Pen pen = (Pen) value.Clone();
				this.mPen.Dispose();
				this.mPen = pen;
				this.RaisePenChanged();
			}
		}

		/// <summary>
		/// Raises the PenChanged event.
		/// </summary>
		private void RaisePenChanged() 
		{
			if (this.mPenChangedHandler != null) 
			{
				this.mPenChangedHandler(this, EventArgs.Empty);
			}
		}

		#region "Pen Properties"
		[System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public Color Color 
		{
			get 
			{
				return this.mPen.Color;
			}
			set 
			{
				this.mPen.Color = value;
				this.RaisePenChanged();
			}
		}

		private void ResetColor() 
		{
			this.Color = Color.Black;
		}

		[System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[System.ComponentModel.DefaultValue(DashCap.Flat)]
		public DashCap DashCap 
		{
			get 
			{
				return this.mPen.DashCap;
			}
			set 
			{
				this.mPen.DashCap = value;
				this.RaisePenChanged();
			}
		}

		[System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[System.ComponentModel.DefaultValue(LineCap.Flat)]
		public LineCap EndCap 
		{
			get 
			{
				return this.mPen.EndCap;
			}
			set 
			{
				this.mPen.EndCap = value;
				this.RaisePenChanged();
			}
		}

		[System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[System.ComponentModel.DefaultValue(LineCap.Flat)]
		public LineCap StartCap 
		{
			get 
			{
				return this.mPen.StartCap;
			}
			set 
			{
				this.mPen.StartCap = value;
				this.RaisePenChanged();
			}
		}

		[System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[System.ComponentModel.DefaultValue(1)]
		public float Width 
		{
			get 
			{
				return this.mPen.Width;
			}
			set 
			{
				this.mPen.Width = value;
				this.RaisePenChanged();
			}
		}

		[System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[System.ComponentModel.DefaultValue(DashStyle.Solid)]
		public DashStyle DashStyle 
		{
			get 
			{
				return this.mPen.DashStyle;
			}
			set 
			{
				this.mPen.DashStyle = value;
				this.RaisePenChanged();
			}
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			PenProvider p = (PenProvider) this.MemberwiseClone();
			p.mPen = (Pen) this.mPen.Clone();
			p.mPenChangedHandler = null;
			foreach (EventHandler d in this.mPenChangedHandler.GetInvocationList()) 
			{
				p.PenChanged += d;
			}
			return p;
		}

		#endregion
	}
}
