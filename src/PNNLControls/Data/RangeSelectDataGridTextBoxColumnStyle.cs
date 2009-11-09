using System;
using System.Collections ; 
using System.Windows.Forms ; 
using System.Drawing ; 

namespace PNNLControls
{
	/// <summary>
	/// Summary description for RangeSelectDataGridTextBoxColumnStyle.
	/// </summary>
	public class RangeSelectDataGridTextBoxColumnStyle : System.Windows.Forms.DataGridTextBoxColumn
	{
		private int mint_min_height = 24 ; 
		private int mint_preferred_height = 24 ; 
		private int mint_preferred_width = 40 ; 
		private Size msz_preferred_size ;
		private Color mclr_selection_back_color ; 
		private Color mclr_selection_fore_color ; 
		private Brush mbr_selection_back_brush ;
		private Brush mbr_selection_fore_brush  ; 
		private Brush mbr_hover_back_brush ;
		private Pen mpen_border ; 
		private Color mclr_border ;
		private Color mclr_internal_border ; 
		private Pen mpen_internal_border ; 
		private SelectionRangeCollection mobj_parent_range_collection ; 
		private int mint_index ; 
		private Font mfnt_selected_font = null ; 
		private Font mfnt_font = null ; 
		private string mstr_font_name = "Microsoft Sans Serif" ; 

		public Color SelectionForeColor 
		{
			get
			{
				return this.mclr_selection_fore_color ;
			}
			set
			{
				this.mclr_selection_fore_color = value ; 
				mbr_selection_fore_brush = new SolidBrush(this.mclr_selection_fore_color) ; 
			}
		}

		public Color SelectionBorderColor
		{
			get
			{
				return this.mclr_border ; 
			}
			set
			{
				this.mclr_border = value ; 
				this.mpen_border.Color = this.mclr_border ; 
			}
		}
		public Color SelectionInternalBorderColor
		{
			get
			{
				return this.mclr_internal_border ; 
			}
			set
			{
				this.mclr_internal_border = value ; 
				this.mpen_internal_border.Color = this.mclr_internal_border ; 
			}
		}
    
		public float SelectionInternalBorderWidth
		{
			get
			{
				return this.mpen_internal_border.Width ; 
			}
			set
			{
				this.mpen_internal_border.Width = value ; 
			}
		}


		public float SelectionBorderWidth
		{
			get
			{
				return this.mpen_border.Width ; 
			}
			set
			{
				this.mpen_border.Width = value ; 
			}
		}

		public Color SelectionBackColor 
		{
			get
			{
				return this.mclr_selection_back_color ;
			}
			set
			{
				this.mclr_selection_back_color = value ; 
				mbr_selection_back_brush = new SolidBrush(this.mclr_selection_back_color) ; 
			}
		}
    

		public Font Font
		{
			get
			{
				return mfnt_font ; 
			}
			set
			{
				mfnt_font = value ; 
			}
		}


		public Font SelectionFont
		{
			get
			{
				return mfnt_selected_font ; 
			}
			set
			{
				mfnt_selected_font = value ; 
			}
		}

		public Pen BorderPen
		{
			get
			{
				return this.mpen_border ; 
			}
			set
			{
				this.mpen_border = value ; 
			}
		}
		public Pen BorderInternalPen
		{
			get
			{
				return this.mpen_internal_border ; 
			}
			set
			{
				this.mpen_internal_border = value ; 
			}
		}

		public RangeSelectDataGridTextBoxColumnStyle(int col_num)
		{
			msz_preferred_size = new Size(mint_preferred_width ,mint_preferred_height) ;
			mobj_parent_range_collection = null ; 
			//    SelectionBackColor = Color.LightSteelBlue ; 
			//    SelectionBackColor = Color.FromArgb(172, 182, 214) ; //excel color.
			//      SelectionBackColor = Color.FloralWhite ; 
			SelectionBackColor = Color.FromArgb(175, 185, 220) ; 
			SelectionForeColor = Color.Black ; 
      
			this.mpen_border = new Pen(Color.Black, 2) ; 
			this.mpen_internal_border = new Pen(Color.LightGray, 0.75F) ; 
			SelectionBorderColor = Color.Black ; 
			SelectionInternalBorderColor = Color.LightGray ; 

			mbr_hover_back_brush = Brushes.GhostWhite ; 
			mint_index = col_num ; 
			this.mfnt_selected_font = new Font(this.mstr_font_name, 10, FontStyle.Bold) ;
			this.mfnt_font = new Font(this.mstr_font_name, 10) ; 
			this.TextBox.Font = this.mfnt_selected_font ; 
			this.TextBox.BackColor = Color.White ; 
			this.TextBox.Capture = false ; 
		}

		public SelectionRangeCollection RangeCollection
		{
			get { return this.mobj_parent_range_collection ; }
			set { this.mobj_parent_range_collection = value ; }
		}

		protected override void Abort(int rowNum)
		{
			return ; 
		}
   
		public void HideTextBox()
		{
			TextBox.Visible = false ; 
		}
		protected override void Edit(
			CurrencyManager source,
			int rowNum,
			Rectangle bounds,
			bool readOnly,
			string instantText,
			bool cellIsVisible
			)
		{
			base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible) ; 
		}
   
		protected override void Paint( Graphics g, Rectangle bounds, 
			CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, 
			bool alignToRight )
		{
			Object val ; 
			try 
			{
				RectangleF rectF = new RectangleF(Convert.ToSingle(bounds.X), Convert.ToSingle(bounds.Y),
					Convert.ToSingle(bounds.Width),Convert.ToSingle(bounds.Height));
				// if you're at the last guy, ignore it cause there is no data here.
				if (source.Position == source.Count - 1 && rowNum == source.Count - 1)
					return ; 

				StringFormat format = new StringFormat() ; 
				if (alignToRight)
					format.Alignment = StringAlignment.Far ; 
				else 
					format.Alignment = StringAlignment.Near ; 

				val = this.GetColumnValueAtRow(source, rowNum) ; 
				string txt = val.ToString() ; 

				bool is_selected = this.mobj_parent_range_collection.IsSelected(rowNum, mint_index) ;
				bool is_first_selected = this.mobj_parent_range_collection.IsLastClickedElement(rowNum, mint_index) ;

				if (is_selected)
				{
					if(!is_first_selected)
						g.FillRectangle(this.mbr_selection_back_brush, bounds) ;
					else
						g.FillRectangle(backBrush, bounds) ;

					g.DrawString(txt, this.mfnt_selected_font, this.mbr_selection_fore_brush, rectF, format) ; 
					int orientation = this.mobj_parent_range_collection.SelectedCellOrientation(rowNum, mint_index) ;
					DrawBorder(orientation, g, bounds) ; 
					return ; 
				}

				g.FillRectangle(backBrush, bounds) ;
				g.DrawString(txt, this.mfnt_font, foreBrush, rectF, format) ; 
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.Message, "Error in CreateTableStyleCollectionForTable") ; 
			}
		}

		private void DrawBorder(int orientation, Graphics g, Rectangle bounds)
		{
			g.DrawRectangle(this.mpen_internal_border, bounds) ; 
			if(0 != (orientation & (int)OrientationType.BOTTOM))
				g.DrawLine(mpen_border, bounds.X, bounds.Bottom-mpen_border.Width/2, bounds.Right, bounds.Bottom-mpen_border.Width/2) ; 
			if(0 != (orientation & (int)OrientationType.TOP))
				g.DrawLine(mpen_border, bounds.X, bounds.Y, bounds.Right, bounds.Y) ; 
			if(0 != (orientation & (int) OrientationType.LEFT))
				g.DrawLine(mpen_border, bounds.X, bounds.Y, bounds.X, bounds.Bottom) ; 
			if(0 != (orientation & (int)OrientationType.RIGHT))
				g.DrawLine(mpen_border, bounds.Right-mpen_border.Width/2, bounds.Y, bounds.Right-mpen_border.Width/2, bounds.Bottom) ; 
  
		}
		protected override int GetMinimumHeight()
		{
			return  mint_min_height ; 
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}
	}
}
