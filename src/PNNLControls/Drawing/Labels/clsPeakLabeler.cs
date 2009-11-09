using System;
using System.IO ; 
using System.Drawing ; 
using System.Collections ; 

namespace PNNLControls
{
	/// <summary>
	/// clsPeakLabeler takes on the task of labeling peaks in a ctlLineChart.
	/// It does so in two different modes. In the first mode, it labels the 
	/// peaks in a chart with x or y coordinate. In the second, peaks in the chart
	/// might be labelled with additional information. 
	/// In order to add peak information, AddPeak function is called with index of the peak
	/// in the original data, and the peak coordinates. In order to add alternative peak labels
	/// AddPeakLabel is called with the index of the peak to which a label is to be added.
	/// A PeakLabel is only plotted for Peaks that would be drawn otherwise.
	/// </summary>
	public class clsPeakLabeler
	{
		private int mint_num_peaks_per_bin_stored = 5; 
		/// <summary>
		/// Stores all the candidate peak tops to be drawn: mint_num_peaks_per_bin_stored
		/// most intense peaks in each bin. Once all peaks are entered into bins
		/// these are sorted in descending intensity bins and plotted
		/// preferentially based on intensity.
		/// </summary>
		private Point [][] marr_peak_tops ; 
		private int [][] marr_peak_top_indices ; 
		private bool [][] marr_draw_boxes ; 
		private int [] marr_min_heights_per_bin ; 
		private object [][] marr_peak_labels ; 

		private int mint_num_peak_top_bins ; 
		private Font mfnt_font = new Font("Microsoft Sans Serif", 10) ; 

		private Rectangle mrect_drawing_area ; 
		private Pen mpen_box = new Pen(Color.Black, 2) ; 

		private ArrayList marr_drawing_boxes = new ArrayList() ; 
		/// <summary>
		/// Keeps only the peaks that will be drawn. 
		/// </summary>
		private ArrayList marr_drawing_pts = new ArrayList() ;
		/// <summary>
		/// keeps Rectangular coordinates for labels of the points which will be drawn.
		/// </summary>
		private ArrayList marr_drawing_rectangles = new ArrayList() ; 
		/// <summary>
		/// marr_drawing_labels stores labels corresponding to the above points and rectangles 
		/// which will be draw 
		/// </summary>
		private ArrayList marr_drawing_labels = new ArrayList() ;

		private bool mbln_horizontal_orientation = true ; 

		private float mfltLabelOffset = 8 ; 

		private int mint_decimal_place = 2 ; 
		public clsPeakLabeler()
		{
			//
			// TODO: Add constructor logic here
			//
			mint_num_peak_top_bins = 20 ; 
			AllocateData() ; 
		}

		private void AllocateData()
		{
			marr_peak_tops = new Point[mint_num_peak_top_bins][] ; 
			marr_min_heights_per_bin = new int[mint_num_peak_top_bins] ; 
			marr_peak_top_indices = new int[mint_num_peak_top_bins][] ; 
			marr_peak_labels = new object [mint_num_peak_top_bins][] ; 
			marr_draw_boxes = new bool[mint_num_peak_top_bins][] ; 
			
			for (int i = 0 ; i < mint_num_peak_top_bins ; i++)
			{
				marr_peak_tops[i] = new Point [mint_num_peaks_per_bin_stored] ; 
				marr_peak_top_indices[i] = new int [mint_num_peaks_per_bin_stored] ; 
				marr_peak_labels[i] = new object [mint_num_peaks_per_bin_stored] ; 
				marr_draw_boxes[i] = new bool [mint_num_peaks_per_bin_stored] ; 

				for (int j = 0 ; j < mint_num_peaks_per_bin_stored ; j++)
				{
					marr_peak_tops[i][j] = new Point(int.MinValue, int.MaxValue) ; 
					marr_peak_top_indices[i][j] = -1 ; 
					marr_peak_labels[i][j] = null ; 
					marr_draw_boxes[i][j] = false ; 
				}
				marr_min_heights_per_bin[i] = int.MaxValue; 
			}
		}
		public bool VerticalLabels
		{
			get
			{
				return !mbln_horizontal_orientation ;
			}
			set
			{
				mbln_horizontal_orientation = !value ; 
			}
		}

		public int Precision
		{
			get
			{
				return mint_decimal_place ; 
			}
			set
			{
				mint_decimal_place = value ; 
			}
		}

		public int NumBins
		{
			get
			{
				return mint_num_peak_top_bins ; 
			}
			set
			{
				mint_num_peak_top_bins = value ; 
				AllocateData() ; 
			}
		}

		public int NumPeaksPerBins
		{
			get
			{
				return mint_num_peaks_per_bin_stored ; 
			}
			set
			{
				mint_num_peaks_per_bin_stored = value ; 
				AllocateData() ; 
			}
		}
		
		public void Clear()
		{
			for (int i = 0 ; i < mint_num_peak_top_bins ; i++)
			{
				for (int j = 0 ; j < mint_num_peaks_per_bin_stored ; j++)
				{
					marr_peak_tops[i][j].X = int.MinValue ; 
					marr_peak_tops[i][j].Y = int.MaxValue; 
					marr_peak_top_indices[i][j] = -1 ; 
					marr_peak_labels[i][j] = null ; 
					marr_draw_boxes[i][j] = false ; 
				}
				marr_min_heights_per_bin[i] = int.MaxValue ; 
			}
		}

		public Rectangle DrawingArea
		{
			set
			{
				Clear() ; 
				mrect_drawing_area = value ; 
			}
			get
			{
				return mrect_drawing_area ; 
			}
		}


		public float LabelOffset
		{
			get
			{
				return mfltLabelOffset ; 
			}
			set
			{
				mfltLabelOffset = value ; 
			}
		}

		/// <summary>
		/// Add the point with original index of peak_index to the list of points needing 
		/// to be labeled. The label to be used is provided. In order for this peak to be added
		/// to the list of points being drawn, it has to be in the mint_num_peaks_per_bin_stored
		/// most intense points in its bin.
		/// </summary>
		/// <param name="peak_index"></param>
		/// <param name="pt"></param>
		/// <param name="label"></param>
		/// <param name="draw_box">describes whether or not to draw a box around this label</param>
		public void AddPeak(int peak_index, Point pt, object label, bool draw_box)
		{
			int xbin = ((pt.X - mrect_drawing_area.Left) * mint_num_peak_top_bins) / mrect_drawing_area.Width ; 
			if (xbin < 0 || xbin >= mint_num_peak_top_bins || pt.Y < mrect_drawing_area.Top || pt.Y > mrect_drawing_area.Bottom)
				return ; 

			try
			{
				// Do remember that the higher the pixel, the lower its value.. so we are trying to keep track only of 
				// y Pixels with values >0 but the mimimum ones..
				if (pt.Y >= marr_min_heights_per_bin[xbin])
					return ; 

				for (int pk_num = 0 ; pk_num < mint_num_peaks_per_bin_stored ; pk_num++)
				{
					if (marr_peak_tops[xbin][pk_num].Y == marr_min_heights_per_bin[xbin])
					{
						marr_peak_tops[xbin][pk_num].X = pt.X ;
						marr_peak_tops[xbin][pk_num].Y = pt.Y ;

						marr_peak_labels[xbin][pk_num] = label ; 
						marr_peak_top_indices[xbin][pk_num] = peak_index ; 
						marr_draw_boxes[xbin][pk_num] = draw_box ; 
						marr_min_heights_per_bin[xbin] = int.MinValue ; 
						for (int j = 0 ; j < mint_num_peaks_per_bin_stored ; j++)
						{
							if (marr_peak_tops[xbin][j].Y > marr_min_heights_per_bin[xbin])
								marr_min_heights_per_bin[xbin] = marr_peak_tops[xbin][j].Y ; 
						}
						return ; 
					}
				}			
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace + " " + ex.Message) ; 
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="peak_index"></param>
		/// <param name="label"></param>
		public void AddPeakLabel(int peak_index, string label)
		{
		}

		public bool ScientificNotationRequired
		{
			get
			{
				return false ; 
			}
		}

		private string GetStringRepresentation(object obj)
		{
			try
			{
				if (obj.GetType() != typeof(float))
					return "" ; 

				float val = (float) obj ; 

				int index_of_point ; 
				if (val == 0) 
				{
					return "0";
				}

				if (!this.ScientificNotationRequired)
				{
					index_of_point = val.ToString().IndexOf('.') ;
					int decimal_place_to_see_till = Convert.ToInt32(Math.Floor(Math.Log10(val))) ; 
					if (index_of_point >= 0 && decimal_place_to_see_till < 0)
					{
						return val.ToString("f" + (-1*decimal_place_to_see_till+mint_decimal_place-1).ToString()) ; 
					}
					else
					{
						string val_str = val.ToString() ; 
						if (index_of_point == -1 || val_str.Length < index_of_point+1+mint_decimal_place )
							return val_str ; 
						return val_str.Substring(0, index_of_point+1+mint_decimal_place) ; 
					}
				}
				return "" ; 
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message + " at " + e.StackTrace) ; 
				return "" ; 
			}
		}

		public SizeF GetStringSize(object val, Graphics g)
		{
			try
			{
				string obj_str = GetStringRepresentation(val) ; 

				StringFormat strTextFormat = new System.Drawing.StringFormat();
				if (!mbln_horizontal_orientation)
					strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical ;
				else
					strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;

				strTextFormat.Alignment = StringAlignment.Center ;
				strTextFormat.LineAlignment = StringAlignment.Center ;

				PointF ptF = new PointF(0,0) ; 
				SizeF str_size = g.MeasureString(obj_str, mfnt_font, ptF, strTextFormat) ;
				return str_size ; 
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace + " " + e.Message)  ;
				return new SizeF(0,0) ; 
			}
		}

		/// <summary>
		/// This method finalizes the strings to be drawn the positions at which to draw them.
		/// </summary>
		private void FinalizePositions(Graphics g)
		{
			marr_drawing_labels.Clear() ; 
			marr_drawing_rectangles.Clear() ; 
			marr_drawing_pts.Clear() ;
			marr_drawing_boxes.Clear() ; 

			StringFormat strTextFormat = new System.Drawing.StringFormat();
			if (!mbln_horizontal_orientation)
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical ;
			else
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;

			strTextFormat.Alignment = StringAlignment.Center ;
			strTextFormat.LineAlignment = StringAlignment.Center ;


			Pen pen = Pens.Black ;

			ArrayList temp = new ArrayList() ; 

			for (int bin_num = 0 ; bin_num < mint_num_peak_top_bins ; bin_num++)
			{
				for (int pk_num = 0 ; pk_num < mint_num_peaks_per_bin_stored ; pk_num++)
				{
					if (marr_peak_top_indices[bin_num][pk_num] != -1)
					{
						clsPair x_orig_index = new clsPair(marr_peak_tops[bin_num][pk_num].X, bin_num*mint_num_peaks_per_bin_stored + pk_num) ;
						temp.Add(x_orig_index) ; 
					}
				}
			}
			temp.Sort() ; 
			
			int num_pks = temp.Count ; 
			ArrayList arr_draw_peaks = new ArrayList(num_pks) ; 
			ArrayList arr_orig_rectangles = new ArrayList(num_pks) ; 
			bool [] arr_draw_boxes = new bool[num_pks] ; 
			ArrayList arr_orig_labels = new ArrayList(num_pks) ; 
			ArrayList arr_peaks = new ArrayList(num_pks) ; 
			ArrayList arr_peak_indices = new ArrayList(num_pks) ; 

			for (int pk_num = 0 ; pk_num < num_pks ; pk_num++)
			{
				clsPair x_orign_index = (clsPair) temp[pk_num] ; 
				int bin_num = ((int) x_orign_index.mobj_val) / mint_num_peaks_per_bin_stored ; 
				int bin_pk_num = ((int) x_orign_index.mobj_val) % mint_num_peaks_per_bin_stored ; 
				
				Point pt = marr_peak_tops[bin_num][bin_pk_num] ; 
				bool draw_box = marr_draw_boxes[bin_num][bin_pk_num] ; 

				arr_peaks.Add(pt) ; 
				arr_draw_peaks.Add(true) ; 

				int x = pt.X ; 
				int y = pt.Y ; 
				string label = GetStringRepresentation(marr_peak_labels[bin_num][bin_pk_num]) ; 		
				arr_orig_labels.Add(label) ; 

				PointF ptF = new PointF ((float)x,(float)y)  ; 
				SizeF str_size = g.MeasureString(label, mfnt_font, ptF, strTextFormat) ;
				RectangleF rectF = new RectangleF(ptF.X - str_size.Width/2, ptF.Y - str_size.Height - mfltLabelOffset, str_size.Width, str_size.Height); 
				arr_orig_rectangles.Add(rectF) ; 
				arr_draw_boxes[pk_num] = draw_box ; 

				clsPair intensity_index_pair = new clsPair(y, pk_num) ; 
				arr_peak_indices.Add(intensity_index_pair) ; 

			}


			// now sort the peaks by intensity. 
			arr_peak_indices.Sort() ; 

			// now go through each one and removing others that might intersect with it..
			num_pks = arr_peak_indices.Count ; 
			for (int sort_pk_num = 0 ; sort_pk_num < num_pks ; sort_pk_num++)
			{
				int original_index = (int)(((clsPair) arr_peak_indices[sort_pk_num]).mobj_val) ; 
				RectangleF current_rect = (RectangleF) arr_orig_rectangles[original_index] ; 
				bool draw_box = arr_draw_boxes[original_index] ; 

				if (((bool)arr_draw_peaks[original_index]) && current_rect.Left >= mrect_drawing_area.Left 
						&& current_rect.Right <= mrect_drawing_area.Right)
				{
					// this one will be drawn.
					marr_drawing_rectangles.Add(current_rect) ; 
					marr_drawing_labels.Add(arr_orig_labels[original_index]) ; 
					marr_drawing_pts.Add(arr_peaks[original_index]) ;
					marr_drawing_boxes.Add(draw_box) ; 
				}

				// Move left in the original list, canceling out those that are within the rectangles boundary.
				for (int left_index = original_index - 1 ; left_index >=0 ; left_index--)
				{
					RectangleF prev_rect = (RectangleF) arr_orig_rectangles[left_index] ; 
					if (prev_rect.Right < current_rect.Left)
						break ; 
					arr_draw_peaks[left_index] = false ; 
				}

				// Move right in the original list, canceling out those that are within the rectangles boundary.
				for (int right_index = original_index + 1 ; right_index < num_pks ; right_index++)
				{
					RectangleF next_rect = (RectangleF) arr_orig_rectangles[right_index] ; 
					if (next_rect.Left > current_rect.Right)
						break ; 
					arr_draw_peaks[right_index] = false ; 
				}
			}

			// copy the peaks that will be drawn to the right structure.
		}

		/// <summary>
		/// When drawing the peaks, we must first sort all the peaks in order of decreasing, intensity..
		/// Here decreasing intensity is increasing Y values. So, we first so by the y-values.
		/// Next for each point we draw, we delete all the other peaks that might intersect with this peak.
		/// Only draw peaks that don't intersect others. 
		/// </summary>
		/// <param name="g"></param>
		public void Draw(Graphics g)
		{  
			FinalizePositions(g) ; 
			StringFormat strTextFormat = new System.Drawing.StringFormat();
			if (!mbln_horizontal_orientation)
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.DirectionVertical ;
			else
				strTextFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip;

			strTextFormat.Alignment = StringAlignment.Center ;
			strTextFormat.LineAlignment = StringAlignment.Center ;

			int num_pts = marr_drawing_labels.Count ; 
			for (int pt_num = 0 ; pt_num < num_pts ; pt_num++)
			{
				bool draw_box = (bool) marr_drawing_boxes[pt_num] ; 
				RectangleF rectF = (RectangleF) marr_drawing_rectangles[pt_num] ; 
				string label = (string) marr_drawing_labels[pt_num] ; 
				g.DrawString(label, mfnt_font, Brushes.Black, rectF, strTextFormat) ;  
				if (draw_box)
				{
					g.DrawRectangle(mpen_box, rectF.Left, rectF.Top, rectF.Width, rectF.Height) ; 
				}
			}
		}

	}
}
