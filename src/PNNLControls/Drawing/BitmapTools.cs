using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;
using PNNLControls;


namespace Derek
{
	/// <summary>
	/// Summary description for BitmapTools.
	/// </summary>
	public unsafe class BitmapTools
	{
		public BitmapTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
		public struct PixelData
		{
			[System.Runtime.InteropServices.FieldOffset(0)]
			public byte blue;
			[System.Runtime.InteropServices.FieldOffset(1)]
			public byte green;
			[System.Runtime.InteropServices.FieldOffset(2)]
			public byte red;
			[System.Runtime.InteropServices.FieldOffset(3)]
			private byte none;		// Allows using of Format32bppRgb, which is faster for doing 
									// comparisons of bitmap contents.
		}

		public class BitmapInfo
		{
			public int width;
			public BitmapData bitmapData = null;
			public Byte* pBase = null;

			public BitmapInfo()
			{
			}
		}

		public Point PixelSize(Bitmap bitmap)
		{
			GraphicsUnit unit = GraphicsUnit.Pixel;
			RectangleF bounds = bitmap.GetBounds(ref unit);

			return new Point((int) bounds.Width, (int) bounds.Height);
		}

//			public Bitmap XzoomIn(Bitmap src, int factor)
//			{
//				Point srcSize = PixelSize(src);
//
//				Bitmap dest = new Bitmap(factor*src.Width, src.Height);
//
//				BitmapInfo srcInfo = LockBitmap(src);
//				BitmapInfo destInfo = LockBitmap(dest);
//
//				for (int y = 0; y < srcSize.Y; y++)
//				{
//					
//					PixelData* srcPixel = PixelAt(srcInfo, 0, y);
//					PixelData* destPixel = PixelAt(destInfo, 0, y);
//					for (int x = 0; x < srcSize.X; x++)
//					{
//						for (int j=0; j<factor; j++)
//						{
//							*(destPixel) = *(srcPixel);
//							destPixel++;
//						}
//						srcPixel++;
//					}
//				}
//				UnlockBitmap(src, srcInfo);
//				UnlockBitmap(dest, destInfo);
//
//				return (dest);
//			}
//
//			public Bitmap XzoomOut(Bitmap src, int factor)
//			{
//				Point srcSize = PixelSize(src);
//
//				Bitmap dest = new Bitmap(src.Width/factor, src.Height);
//
//				BitmapInfo srcInfo = LockBitmap(src);
//				BitmapInfo destInfo = LockBitmap(dest);
//
//				for (int y = 0; y < srcSize.Y; y++)
//				{
//					
//					PixelData* srcPixel = PixelAt(srcInfo, 0, y);
//					PixelData* destPixel = PixelAt(destInfo, 0, y);
//					for (int x = 0; x < srcSize.X; x+=factor)
//					{
//						*(destPixel) = *(srcPixel);
//						destPixel++;
//						srcPixel+=factor;
//					}
//				}
//				UnlockBitmap(src, srcInfo);
//				UnlockBitmap(dest, destInfo);
//
//				return (dest);
//			}


		public BitmapInfo LockBitmap(Bitmap bitmap)
		{
			BitmapInfo info = new BitmapInfo();

			GraphicsUnit unit = GraphicsUnit.Pixel;
			RectangleF boundsF = bitmap.GetBounds(ref unit);
			Rectangle bounds = new Rectangle((int) boundsF.X,
				(int) boundsF.Y,
				(int) boundsF.Width,
				(int) boundsF.Height);

			// Figure out the number of bytes in a row
			// This is rounded up to be a multiple of 4
			// bytes, since a scan line in an image must always be a multiple of 4 bytes
			// in length. 
			info.width = (int) boundsF.Width * sizeof(PixelData);
			if (info.width % 4 != 0)
			{
				info.width = 4 * (info.width / 4 + 1);
			}

			info.bitmapData = 
				bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

			info.pBase = (Byte*) info.bitmapData.Scan0.ToPointer();

			return (info);
		}

		public PixelData* PixelAt(BitmapInfo info, int x, int y)
		{
			return (PixelData*) (info.pBase + y * info.width + x * sizeof(PixelData));
		}

		public int* PixelAsIntAt(BitmapInfo info, int x, int y) 
		{
			return (int*) (info.pBase + y * info.width + x * sizeof(PixelData));
		}

//		public void DrawDots(System.Collections.ArrayList points, PNNLControls.clsShape shape, Bitmap map) 
//		{
//			int count = points.Count;
//			int[] xValues = new int[count];
//			int[] yValues = new int[count];
//			for(int i = 0; i < count; i++) 
//			{
//				Point p = (Point) points[i];
//				xValues[i] = p.X;
//				yValues[i] = p.Y;
//			}
//			DrawDots(xValues, yValues, shape, map);
//		}

		public void DrawDots(ArrayList dataPoints, 
			PNNLControls.clsShape shape, Bitmap map) 
		{
			int x1;
			int y1;
			BitmapInfo srcInfo = null;
			Rectangle rect = new Rectangle(new Point(0,0), map.Size);
			try
			{
				srcInfo = LockBitmap(map);
				int pBase = (int)srcInfo.pBase;
				int pWidth = srcInfo.width;
				PixelData pix = new PixelData(); 
				int pixSize = sizeof(PixelData);

				byte* srcBase = null;

				int [] x = shape.XOffsets; 
				int [] y = shape.YOffsets;
				int num_pts_fig = x.Length;
				int max_x = map.Width;
				int max_y = map.Height;

				int num_pts_per_fig = x.Length;

				int [] yMapOffset = new int[num_pts_per_fig];
				int [] xMapOffset = new int[num_pts_per_fig];

				//map the shape points into bitmap space once per function call
				//additions are fast, multiplications are slow.  Do multiplication
				//outside loop
				for (int i=0; i<num_pts_per_fig; i++)
				{
					xMapOffset[i] = x[i] * pixSize;
					yMapOffset[i] = y[i] * pWidth;
				}

				//array list random access faster than collection foreach
				int count = dataPoints.Count;
				for (int i=0; i < count; i++)
				{
					ChartDataPlotPoint point = (ChartDataPlotPoint) dataPoints[i];

					int pX = point.x;
					int pY = point.y;
					pix = point.pixelColor;

					//map the point offset into bitmap space one time
					//additions are fast, multiplications are slow.  Do multiplication
					//outside loop
					int pointOffset = pBase + pY * pWidth + pX * pixSize;

					for (int j = 0; j < num_pts_per_fig; j++)
					{
						x1 = pX + x[j];
						y1 = pY + y[j]; 

						//do these comparisons one at a time to kick out as soon as possible
						if (x1 >= max_x) continue; 
						if (x1 < 0)  continue; 
						if (y1 >= max_y) continue; 
						if (y1 < 0)  continue; 	
						
						PixelData* destPixel = (PixelData*) (pointOffset + yMapOffset[j] + xMapOffset[j]);
						*(destPixel) = pix;
					}
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace) ; 
			}
			finally 
			{
				UnlockBitmap(map, srcInfo);
			}
		}

		public void oldDrawDots(System.Collections.IEnumerable dataPoints, 
			PNNLControls.clsShape shape, Bitmap map) 
		{
			int x1;
			int y1;
			BitmapInfo srcInfo = null;
			try
			{
				srcInfo = LockBitmap(map);
				Point srcSize = PixelSize(map);

				PixelData pix = new PixelData() ; 

				int [] x = shape.XOffsets ; 
				int [] y = shape.YOffsets;
				int num_pts_fig = x.Length;
				int min_x = 0;
				int min_y = 0;
				int max_x = map.Width;
				int max_y = map.Height;

				int num_pts_per_fig = x.Length;

				foreach (ChartDataPlotPoint point in dataPoints)
				{
					pix = point.pixelColor;
					for (int j = 0; j < num_pts_per_fig ; j++)
					{
						int x_v = point.x;
						int y_v = point.y;
						x1 = x_v + x[j];
						y1 = y_v + y[j]; 

						if (x1 >= max_x || x1 < min_x)
						{
							continue ; 
						}
						if (y1 >= max_y || y1 < min_y)
						{
							continue ; 
						}

						PixelData* destPixel = PixelAt(srcInfo, x1, y1);
						*(destPixel) = pix ;
					}
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace) ; 
			}
			finally 
			{
				UnlockBitmap(map, srcInfo);
			}
		}

		public void DrawHorizontalLine(Bitmap map, Point p1, Point p2, PixelData linePixel) 
		{
			BitmapInfo srcInfo = null;
			try
			{
				srcInfo = LockBitmap(map);
			
				int pixSize = sizeof(PixelData);
				int yOffset = (int)srcInfo.pBase + p1.Y * (int)srcInfo.width;

				int x1 = p1.X * pixSize;
				int x2 = p2.X * pixSize;

				for (int i = x1; i <= x2; i+=pixSize)
				{
					PixelData* destPixel = (PixelData*) (yOffset + i);
						
					*(destPixel) = linePixel;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace) ; 
			}
			finally 
			{
				UnlockBitmap(map, srcInfo);
			}
		}

		public void DrawVerticalLine(Bitmap map, Point p1, Point p2, PixelData linePixel) 
		{
			BitmapInfo srcInfo = null;
			try
			{
				srcInfo = LockBitmap(map);

				int pWidth = (int)srcInfo.width;
				int xOffset = (int)srcInfo.pBase + p1.X * sizeof(PixelData);

				int y1 = p1.Y * pWidth;
				int y2 = p2.Y * pWidth;

				for (int j = y1; j <= y2; j+=pWidth)
				{
					PixelData* destPixel = (PixelData*) (xOffset + j);
					*(destPixel) = linePixel;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace) ; 
			}
			finally 
			{
				UnlockBitmap(map, srcInfo);
			}
		}

		public void Copy(Bitmap from, Bitmap to) 
		{
			if (from == null) 
			{
				throw new ArgumentNullException("from");
			}
			if (to == null) 
			{
				throw new ArgumentNullException("to");
			}
			if (from.Width != to.Width || from.Height != to.Height) 
			{
				throw new ArgumentException("from and to bitmaps must have the same dimensions");
			}
			BitmapInfo fromInfo = LockBitmap(from);
			BitmapInfo toInfo = LockBitmap(to);
			try 
			{
				PixelData* fromPtr = (PixelData*) fromInfo.pBase;
				PixelData* toPtr = (PixelData*) toInfo.pBase;
				PixelData* max = fromPtr + from.Width * from.Height;
				for (; fromPtr < max; fromPtr++, toPtr++) 
				{
					*toPtr = *fromPtr;
				}
			}
			finally 
			{
				UnlockBitmap(from, fromInfo);
				UnlockBitmap(to, toInfo);
			}
		}

		public Bitmap ClipBitmap(Bitmap src, Range r)
		{
			
			Bitmap dest = new Bitmap(r.NumColumns, r.NumRows, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			BitmapInfo fromInfo = LockBitmap(src);
			BitmapInfo toInfo = LockBitmap(dest);

			//for speed, make everything local outside of loop
			PixelData* srcPixel = null;
			PixelData* destPixel = null;
			int pixSize = sizeof(PixelData);
			byte* fromYbase = null;
			byte* toYbase = null;

			//create pixel map from source to destination
			int [] xSrcMap = new int[dest.Width];
			int [] xDestMap = new int[dest.Width];
			int [] ySrcMap = new int[dest.Height];

			for(int i=0; i<dest.Width; i++)
			{
				xSrcMap[i] =  (r.StartColumn + i) * pixSize;
				xDestMap[i] = i * pixSize;
			}

			for (int j=0; j<dest.Height; j++)
			{
				fromYbase = fromInfo.pBase + j * fromInfo.width;
				toYbase = toInfo.pBase + j * toInfo.width;
				for(int i=0; i<dest.Width; i++)
				{
					srcPixel = (PixelData*) (fromYbase + xSrcMap[i]);
					destPixel = (PixelData*) (toYbase + xDestMap[i]);
					*(destPixel) = *(srcPixel);
				}
			}

			UnlockBitmap(src, fromInfo);
			UnlockBitmap(dest, toInfo);

			return(dest);
		}

//		public Bitmap StretchAndAlignBitmap(Bitmap src, int destWidth, int destHeight, int[] hAlign, int[] vAlign)
//		{
//			if (destWidth * destHeight <= 0) return null;
//
//			Bitmap dest = new Bitmap(destWidth, destHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//
//			BitmapInfo fromInfo = LockBitmap(src);
//			BitmapInfo toInfo = LockBitmap(dest);
//
//			//for speed, make everything local outside of loop
//			PixelData* srcPixel = null;
//			PixelData* destPixel = null;
//			int pixSize = sizeof(PixelData);
//			byte* fromYbase = null;
//			byte* toYbase = null;
//
//			//create pixel map from source to destination
//			int [] xSrcMap = new int[destWidth];
//			int [] xDestMap = new int[destWidth];
//			int [] ySrcMap = new int[destHeight];
//
//			if (hAlign != null)
//			{
//				for (int i=1; i<hAlign.Length; i++)
//				{
//					for(int j=hAlign[i-1]; j<hAlign[i]; j++)
//					{
//						xSrcMap[j] =( i-1)* pixSize;
//						xDestMap[j] = j * pixSize;
//					}
//				}
//			}
//			else  // interpolate src pixel to use at dest position
//			{
//				double wFrac = (double) src.Width  / (double) dest.Width;
//
//				for(int i=0; i<dest.Width; i++)
//				{
//					xSrcMap[i] = ((int)(wFrac*i)) * pixSize;
//					xDestMap[i] = i * pixSize;
//				}
//			}
//
//			if (vAlign != null)
//			{
//				for (int i=1; i<vAlign.Length; i++)
//				{
//					for(int j=vAlign[i-1]; j<vAlign[i]; j++)
//					{
//						ySrcMap[j] =( i-1);
//					}
//				}
//			}
//			else  // interpolate src pixel to use at dest position
//			{
//				double hFrac = (double) src.Height / (double) dest.Height;
//
//				for(int j=0; j<dest.Height; j++)
//				{
//					ySrcMap[j] =  (int) (hFrac*(j));
//				}
//			}
//
//			for (int j=0; j<destHeight; j++)
//			{
//				fromYbase = fromInfo.pBase + ySrcMap[j] * fromInfo.width;
//				toYbase = toInfo.pBase + j * toInfo.width;
//				for(int i=0; i<destWidth; i++)
//				{
//					srcPixel = (PixelData*) (fromYbase + xSrcMap[i]);
//					destPixel = (PixelData*) (toYbase + xDestMap[i]);
//					*(destPixel) = *(srcPixel);
//					
//				}
//			}
//
//			UnlockBitmap(src, fromInfo);
//			UnlockBitmap(dest, toInfo);
//
//			return(dest);
//		}

				public Bitmap StretchAndAlignBitmap(Bitmap src, int destWidth, int destHeight, int[] hAlign, int[] vAlign)
				{
					if (destWidth * destHeight <= 0) return null;
		
					Bitmap dest = new Bitmap(destWidth, destHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		
					try
					{
						BitmapInfo fromInfo = LockBitmap(src);
						BitmapInfo toInfo = LockBitmap(dest);
		
						//for speed, make everything local outside of loop
						PixelData* srcPixel = null;
						PixelData* destPixel = null;
						int pixSize = sizeof(PixelData);
						byte* fromYbase = null;
						byte* toYbase = null;
		
						//create pixel map from source to destination
						int [] xSrcMap = new int[destWidth];
						int [] xDestMap = new int[destWidth];
						int [] ySrcMap = new int[destHeight];
		

						// interpolate src pixel to use at dest position
						double wFrac = (double) src.Width  / (double) dest.Width;
	
						for(int i=0; i<dest.Width; i++)
						{
							xSrcMap[i] = ((int)(wFrac*i)) * pixSize;
							xDestMap[i] = i * pixSize;
						}

						//fix crossover
						if (hAlign != null)
						{
							//only clean up the internal demarcations, the endpoints are fine
							for (int i=1; i<hAlign.Length-1; i++)
							{
								int aIndex = hAlign[i];

								if (aIndex < dest.Width-1 && aIndex > 0)
								{
									if( xSrcMap[aIndex]==xSrcMap[aIndex+1])
									{
										xSrcMap[aIndex]=xSrcMap[aIndex-1];
									}
								}
							}
						}
		
						double hFrac = (double) src.Height / (double) dest.Height;
	
						for(int j=0; j<dest.Height; j++)
						{
							ySrcMap[j] =  (int) (hFrac*(j));
						}

						//fix crossover
						if (vAlign != null)
						{
							//only clean up the internal demarcations, the endpoints are fine
							for (int i=1; i<vAlign.Length-1; i++)
							{
								int aIndex = vAlign[i];
								if (aIndex < dest.Height-1 && aIndex > 0)
								{
									if (ySrcMap[aIndex-1]==ySrcMap[aIndex-1])
									{
										ySrcMap[aIndex]=ySrcMap[aIndex+1];
									}
								}
							}
						}

						for (int j=0; j<destHeight; j++)
						{
							fromYbase = fromInfo.pBase + ySrcMap[j] * fromInfo.width;
							toYbase = toInfo.pBase + j * toInfo.width;
							for(int i=0; i<destWidth; i++)
							{
								srcPixel = (PixelData*) (fromYbase + xSrcMap[i]);
								destPixel = (PixelData*) (toYbase + xDestMap[i]);
								*(destPixel) = *(srcPixel);
							
							}
						}
		
						UnlockBitmap(src, fromInfo);
						UnlockBitmap(dest, toInfo);

					}
					catch(Exception ex) 
					{
						//System.Windows.Forms.MessageBox.Show(ex.Message);
					}

					return(dest);
				}


		public Bitmap StretchBitmap(Bitmap src, int width, int height)
		{
			if (width * height <= 0) return null;

			Bitmap dest = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			//used to interpolate src pixel to use at dest position
			double hFrac = (double) src.Height / (double) dest.Height;
			double wFrac = (double) src.Width  / (double) dest.Width;

			BitmapInfo fromInfo = LockBitmap(src);
			BitmapInfo toInfo = LockBitmap(dest);

			//for speed, make everything local outside of loop
			PixelData* srcPixel = null;
			PixelData* destPixel = null;
			int pixSize = sizeof(PixelData);
			byte* fromYbase = null;
			byte* toYbase = null;

			//create pixel map from source to destination
			int [] xSrcMap = new int[width];
			int [] xDestMap = new int[width];
			for(int i=0; i<dest.Width; i++)
			{
				xSrcMap[i] = ((int)(wFrac*i)) * pixSize;
				xDestMap[i] = i * pixSize;
			}

			for (int j=0; j<height; j++)
			{
				int yMap = (int) (hFrac*(j));
				fromYbase = fromInfo.pBase + yMap * fromInfo.width;
				toYbase = toInfo.pBase + j * toInfo.width;
				for(int i=0; i<width; i++)
				{
					srcPixel = (PixelData*) (fromYbase + xSrcMap[i]);
					destPixel = (PixelData*) (toYbase + xDestMap[i]);
					*(destPixel) = *(srcPixel);
					
				}
			}

			UnlockBitmap(src, fromInfo);
			UnlockBitmap(dest, toInfo);

			return(dest);
		}

//		public void ApplyLegend(Bitmap from, Bitmap to) 
//		{
//			if (from == null) 
//			{
//				throw new ArgumentNullException("from");
//			}
//			if (to == null) 
//			{
//				throw new ArgumentNullException("to");
//			}
//			
//			BitmapInfo fromInfo = LockBitmap(from);
//			BitmapInfo toInfo = LockBitmap(to);
//			try 
//			{
//				PixelData* fromPtr = (PixelData*) fromInfo.pBase;
//				PixelData* toPtr = (PixelData*) toInfo.pBase;
//
//				for(int i=0; i<to.Width; i++)
//					for (int j=0; j<to.Height; j++)
//					{
//						int k = i % 255;
//						PixelData* srcPixel = PixelAt(fromInfo, 0, k);
//						PixelData* destPixel = PixelAt(toInfo, i, j);
//						*(destPixel) = *(srcPixel);
//					}
//			}
//			finally 
//			{
//				UnlockBitmap(from, fromInfo);
//				UnlockBitmap(to, toInfo);
//			}
//		}

		public void MarkDifferences(Bitmap map, Bitmap map2, Bitmap diffMap, int markValue) 
		{
			BitmapTools.BitmapInfo info = LockBitmap(map);
			BitmapTools.BitmapInfo info2 = LockBitmap(map2);
			BitmapTools.BitmapInfo diffInfo = LockBitmap(diffMap);

			try 
			{
				int height = map.Height;
				int width = map.Width;
				int* max = (int*) info.pBase;
				max += width * height;
				int* ptr1 = (int*) info.pBase;
				int* ptr2 = (int*) info2.pBase;
				int* diffPtr = (int*) diffInfo.pBase;
				for (; ptr1 < max; ptr1++, ptr2++, diffPtr++) 
				{
					if (*ptr1 != *ptr2) 
					{
						*diffPtr = markValue;
					}
				}
			} 
			finally 
			{
				UnlockBitmap(map, info);
				UnlockBitmap(map2, info2);
				UnlockBitmap(diffMap, diffInfo);
			}
			//Console.WriteLine("Number of non-black pixels {0}", count);
		}

		/// <summary>
		/// Marks into with the markValue where from has whereValue.
		/// </summary>
		/// <param name="into"></param>
		/// <param name="markValue"></param>
		/// <param name="from"></param>
		/// <param name="whereValue"></param>
		public void MarkWhere(Bitmap into, int markValue, Bitmap from, int whereValue) 
		{
			BitmapTools.BitmapInfo info = LockBitmap(into);
			BitmapTools.BitmapInfo info2 = LockBitmap(from);

			try 
			{
				int height = into.Height;
				int width = into.Width;
				int* max = (int*) info.pBase;
				max += width * height;
				int* ptr1 = (int*) info.pBase;
				int* ptr2 = (int*) info2.pBase;
				for (; ptr1 < max; ptr1++, ptr2++) 
				{
					if (*ptr2 == whereValue) 
					{
						*ptr1 = markValue;
					}
				}
			} 
			finally 
			{
				UnlockBitmap(into, info);
				UnlockBitmap(from, info2);
			}
		}
		public void MarkRectangle(Bitmap map, int markValue, Rectangle bounds) 
		{
			BitmapInfo info = this.LockBitmap(map);
			try 
			{
				Rectangle toMark = Rectangle.Intersect(bounds, new Rectangle(0, 0, map.Width, map.Height));
				for (int i = toMark.Top; i < toMark.Bottom; i++) 
				{
					for (int j = toMark.Left; j < toMark.Right; j++) 
					{
						*this.PixelAsIntAt(info, j, i) = markValue;
					}
				}
			}
			finally 
			{
				this.UnlockBitmap(map, info);
			}
		}


//		public void DrawDots(PNNLControls.clsSeries series, Bitmap map, PNNLControls.clsMargins margins, PNNLControls.clsPlotRange range)
//		{
//			int x1 ; 
//			int y1 ; 
//			try
//			{
//				BitmapInfo srcInfo = LockBitmap(map);
//				Point srcSize = PixelSize(map);
//
//				PixelData   pix = new PixelData() ; 
//
//				PNNLControls.clsPlotParams series_param = series.PlotParams ; 
//				PNNLControls.clsShape shape = series_param.Shape ; 
//				int [] x = shape.mint_x_offsets ; 
//				int [] y = shape.mint_y_offsets ;
//				int num_pts_fig = x.Length ; 
//
////				float [] x_val = series.XAxisData ; 
////				float [] y_val = series.YAxisData ; 
//				int num_pts = series.DataSize; 
//
//				pix.red = shape.Color.R ; 
//				pix.green = shape.Color.G ; 
//				pix.blue = shape.Color.B ; 
//
//				int num_pts_per_fig = series_param.Shape.mint_x_offsets.Length ; 
//				int min_x = margins.mint_ymargin ; 
//				int max_x = margins.mint_width - margins.mint_ymargin ; 
//				int min_y = margins.mint_xmargin ; 
//				int max_y = margins.mint_height - margins.mint_xmargin ; 
//				System.Collections.IEnumerator xValues = series.XAxisData.GetEnumerator();
//				System.Collections.IEnumerator yValues = series.YAxisData.GetEnumerator();
//
//				//for (int i = 0 ; i < num_pts ; i++)
//				//{
//				while (xValues.MoveNext()) 
//				{
//					yValues.MoveNext();
////					int x_v = Convert.ToInt32(x_val[i]*margins.mint_plot_width/range.m_xDataRange) + margins.mint_xmargin ; 
////					int y_v = Convert.ToInt32(y_val[i]*margins.mint_plot_height/range.m_yDataRange) + margins.mint_ymargin ; 
//					int x_v = Convert.ToInt32((float) (xValues.Current)*margins.mint_plot_width/range.m_xDataRange) + margins.mint_xmargin ; 
//					int y_v = Convert.ToInt32((float) (yValues.Current)*margins.mint_plot_height/range.m_yDataRange) + margins.mint_ymargin ;
//
//					for (int j = 0; j < num_pts_per_fig ; j++)
//					{
//						x1 = x_v+x[j] ;
//						y1 = y_v + y[j] ; 
//						if (x1 >= max_x || x1 < min_x)
//						{
//							continue ; 
//						}
//						if (y1 >= max_y || y1 < min_y)
//						{
//							continue ; 
//						}
//						PixelData* destPixel = PixelAt(srcInfo, x1, y1);
//						*(destPixel) = pix ;
//					}
//				}
//				UnlockBitmap(map, srcInfo);
//			}
//			catch (Exception e)
//			{
//				Debug.WriteLine(e) ; 
//			}
//		}

//		//speed this up.  Derek 3/23/07
//		public void ClearBitmap(Bitmap map)
//		{
//			BitmapInfo srcInfo = LockBitmap(map);
//			Point srcSize = PixelSize(map);
//
//			PixelData   pix = new PixelData() ; 
//			pix.blue = Byte.MaxValue ; 
//			pix.red = Byte.MaxValue ; 
//			pix.green = Byte.MaxValue ; 
//
//			for (int y = 0; y < srcSize.Y; y++)
//			{
//				PixelData* destPixel = PixelAt(srcInfo, 0, y);
//				for (int x = 0; x < srcSize.X; x++)
//				{
//					*(destPixel) = pix ;
//					destPixel++;
//				}
//			}
//			UnlockBitmap(map, srcInfo);
//		}

		public void UnlockBitmap(Bitmap bmp, BitmapInfo info)
		{
			bmp.UnlockBits(info.bitmapData);
		}
	}
}