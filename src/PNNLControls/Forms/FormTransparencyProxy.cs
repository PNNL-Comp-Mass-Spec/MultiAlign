using System;
using System.Windows.Forms;
using System.Drawing;

namespace PNNLControls
{
	
	#region Form Transparency Proxy
	public class FormTransparencyProxy: IDisposable
	{								
		private const int    INTERVAL		 = 100;
		private const double TRANSPARENCY	 = 0.6;	
		private const double NO_TRANSPARENCY = 1.0;
		private const double TRANSPARENCY_SUB_AMT = .05;	
		protected Timer   m_timer  = null;
		private Form m_parent = null;
		

		public FormTransparencyProxy(Form parent)
		{						
			m_timer		  = new Timer();
			m_timer.Tick += new EventHandler(OnTick);
			m_timer.Interval = INTERVAL;

			if (parent != null)
			{
				m_parent = parent;
				SetTransparencyHandlers(m_parent);
			}
		}

		public int Interval
		{
			get
			{
				return m_timer.Interval;
			}
			set
			{	
				m_timer.Interval = value;
			}
		}
		
		protected void SetTransparencyHandlers(Control parentControl)
		{			
			foreach(Control childControl in parentControl.Controls)
			{
				childControl.GotFocus   += new EventHandler(MouseEnter);				
				childControl.MouseHover += new EventHandler(MouseEnter);
				childControl.MouseEnter += new EventHandler(MouseEnter);				
				childControl.MouseLeave += new EventHandler(MouseLeave);							
				SetTransparencyHandlers(childControl);
			}
		}

									
				
		protected void MouseEnter(object sender, EventArgs e)
		{
			m_parent.Opacity = NO_TRANSPARENCY;
		}

		protected void MouseLeave(object sender, EventArgs e)
		{
			m_timer.Enabled = true;
		}
		protected void OnTick(object sender, EventArgs e)
		{
			Rectangle Owner;
			Rectangle Bounds;
			
			/* Dont allow for the window to be transparent unless the form has an associated owner */
			if (m_parent.Owner == null)			
				m_timer.Enabled = false;			

			Owner  = m_parent.Owner.Bounds;
			Bounds = m_parent.Bounds;

			double subtractAmount = 0;
			Point p = Cursor.Position;						
			/* Find the position of the window over the parent window. */
			if (p.X < Bounds.Left || p.X > Bounds.Left + Bounds.Width || p.Y > Bounds.Top + Bounds.Height || p.Y < Bounds.Top)
			{
				if(Bounds.Left > Owner.Right || Bounds.Right < Owner.Left || Bounds.Top > Owner.Top + Owner.Height || Bounds.Bottom < Owner.Top)
				{					
					subtractAmount = 0;
				}
				else
				{
					subtractAmount = TRANSPARENCY_SUB_AMT;
				}
			}			
			m_parent.Opacity -= subtractAmount;
			if (m_parent.Opacity < TRANSPARENCY)
				m_timer.Enabled = false;
		}
		#region IDisposable Members
		private bool m_disposing = false;
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_disposing = disposing; 
				if (m_timer != null)
					m_timer.Dispose();
			}			
		}	
		void System.IDisposable.Dispose()
		{
			Dispose(m_disposing);
		}

		#endregion
	}
	#endregion
}
