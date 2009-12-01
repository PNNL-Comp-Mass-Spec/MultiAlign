using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using Derek;

namespace PNNLControls
{
	/// <summary>
	/// Summary description for ctlHeatMapClient.
	/// </summary>
	public unsafe class ctlHeatMapClient : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.Container components = null;

		public delegate void BitmapPaintedDelegate (Graphics g);
		public event BitmapPaintedDelegate BitmapPainted = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void ZoomDelegate (Point start, Point stop);
		public event ZoomDelegate Zoom = null;

		/// <summary>
		/// 
		/// </summary>
		public delegate void UnZoomDelegate ();
		public event UnZoomDelegate UnZoom = null;


		public enum AverageMode
		{
			average,
			maximumValue
		}

		private AverageMode averageMode = AverageMode.average;

		#region fields

		/// <summary>
		/// rectangle associated with drag-zooming
		/// </summary>
		private Rectangle mZoomRectangle = new Rectangle(0,0,1,1);

		/// <summary>
		/// flag which indicates whether we are in the process of selecting a zoom area
		/// </summary>
		private bool mZooming = false;

		private int mMinSelectionRange = 10; //10 pixels

		/// <summary>
		/// 2D float array of data, MxN.  M is vertical data, N is horizontal
		/// </summary>
		private float [,] mData;

		/// <summary>
		/// 2D array of data after being averaged.  Averaging only happens vertically.
		/// </summary>
		private float [,] mAveragedData;

		/// <summary>
		/// when data is moved, these tagged arrays hold the location of the moved data
		/// in terms of the original data.  The original data is not changed, these arrays
		/// are used to map it when creating the viewed data.
		/// </summary>
		private int[] rowIndices = null;
		private int[] colIndices = null;

		/// <summary>
		/// legend used to generate heatmap color scheme
		/// </summary>
		private ctlHeatMapLegend mLegend = null;

		/// <summary>
		/// container for heatmap dispay
		/// </summary>
		private System.Windows.Forms.Panel pnlDisplay;

		/// <summary>
		/// bitmap generated from averaged data and sized to fit pnlDisplay
		/// </summary>
		private Bitmap mBitmap = null; 

		/// <summary>
		/// bitmap generated from mBitmap, has selection drawn on it, is then displayed
		/// </summary>
		private Bitmap mSelectedBitmap = null;

		/// <summary>		
		/// tags that hold the current pixel starting points of rows and columns of data.
		/// used to align data to external axes
		/// </summary>
		private int[] mVerticalAlignment = null;
		private int[] mHorizontalAlignment = null;

		/// <summary>
		/// holds the bounds for data to be displayed
		/// </summary>
		private Range mDisplayedData = new Range(int.MinValue,int.MinValue,int.MinValue,int.MinValue);
		private Range mSelected = new Range(int.MinValue,int.MinValue,int.MinValue,int.MinValue);

		private bool mblnDrawDemarcations ; 
		#endregion

		public ctlHeatMapLegend Legend 
		{
			get{return this.mLegend;}
			set{
				this.mLegend = value;
				if (value!=null)
				{
					this.mLegend.LegendChanged += new ctlHeatMapLegend.LegendChangedDelegate(this.LegendChanged);
					this.mLegend.AutoScaleRequest += new ctlHeatMapLegend.AutoScaleRequestDelegate(this.AutoScale);
				}
			}
		}

		private void AutoScale()
		{
			if (mData!=null)
				mLegend.AutoScale(mData, new Range(0,0, mData.GetUpperBound(0), mData.GetUpperBound(1)));
		}

		private void LegendChanged()
		{
			this.OnRefresh();
		}

		public int[] ColumnMap 
		{
			get{return (int[])this.colIndices.Clone();}
			set{this.colIndices = (int[])value.Clone();}	
		}

		public int[] RowMap 
		{
			get{return (int[])this.rowIndices.Clone();}
			set{this.rowIndices = (int[])value.Clone();}	
		}

		public float[,] Data 
		{
			get{return this.mData;}
			set 
			{
				this.mData = value;	

				//initialize row and column indices
				//these indices are tags holding the view locations of the data points
				if (mData!=null)
				{
					AutoScale();
				
					int rows = mData.GetLength(0);
					rowIndices= new int[rows];
					for (int j=0; j<rows; j++)
					{
						rowIndices[j]=j;
					}

					int cols = mData.GetLength(1);
					colIndices= new int[cols];
					for (int i=0; i<cols; i++)
					{
						colIndices[i]=i;
					}
				}
			}
		}

		//DJ May 3 2007.
		public bool DrawDemarcationLines
		{
			get
			{
				return mblnDrawDemarcations ; 
			}
			set
			{
				mblnDrawDemarcations = value ; 
			}
		}

		public Bitmap Thumbnail(Size size)
		{
			if (mBitmap!=null)
				return new Bitmap(mBitmap, size);
			else
				return null;
		}

		public Bitmap SelectedThumbnail(Size size)
		{
			if (mSelectedBitmap!=null)
				return new Bitmap(mSelectedBitmap, size);
			else
				return null;
		}

		public int[] AlignVertical 
		{
			get{return this.mVerticalAlignment;}
			set{this.mVerticalAlignment = value;}
		}

		public int[] AlignHorizontal 
		{
			get{return this.mHorizontalAlignment;}
			set{this.mHorizontalAlignment = value;}
		}

		public Range DisplayedData
		{
			get{return this.mDisplayedData;}
			set{this.mDisplayedData = value;}
		}

		private void ShowSelection()
		{
			//Clear previous selection 
			mSelectedBitmap = new Bitmap(mBitmap);

			Graphics g = Graphics.FromImage(mSelectedBitmap);

			SolidBrush b = new SolidBrush(Color.FromArgb(155,Color.Black));

			//has vertical selection
			if (mSelected.StartColumn>=0)
			{
				//has horizontal selection
				if (mSelected.StartRow>=0)
				{
					Rectangle r = new Rectangle(0,0, mSelectedBitmap.Width,mSelected.StartRow);
					g.FillRectangle (b, r);
					r = new Rectangle(0,mSelected.StartRow, mSelected.StartColumn+1,this.Height-mSelected.StartRow);
					g.FillRectangle (b, r);
					r = new Rectangle(mSelected.EndColumn+2,mSelected.StartRow, this.Width,this.Height-mSelected.StartRow);
					g.FillRectangle (b, r);
				    r = new Rectangle(mSelected.StartColumn+1,mSelected.EndRow+1,mSelected.EndColumn-mSelected.StartColumn+1,this.Height-mSelected.EndRow);
					g.FillRectangle (b, r);
				}
				else 
				{
					Rectangle r = new Rectangle(0,0,mSelected.StartColumn+1,this.Height);
					g.FillRectangle (b, r);
					r = new Rectangle(mSelected.EndColumn+2,0,this.Width,this.Height);
					g.FillRectangle (b, r);
				}
			}
			else if (mSelected.StartRow>=0)
			{
				Rectangle r = new Rectangle(0,0,this.Width,mSelected.StartRow);
				g.FillRectangle (b, r);
				r = new Rectangle(0,mSelected.EndRow+1,this.Width,this.Height);
		        g.FillRectangle (b, r);
			}
			g.Dispose();

			pnlDisplay.Refresh();
		}

		public void SelectedVertical(int lowPixel, int highPixel)
		{
			mSelected.StartRow = lowPixel;
			mSelected.EndRow = highPixel;

			//ShowSelection();
		}

		public void SelectedHorizontal(int lowPixel, int highPixel)
		{
			mSelected.StartColumn = lowPixel;
			mSelected.EndColumn = highPixel;

			//ShowSelection();
		}

		//need to upgrade this to block copies when I learn how to do it.
		private void MoveRange(ArrayList list, int low, int high, int pos)
		{

			ArrayList temp = new ArrayList();

			//remove range from previous location			
			for (int i=low; i<=high; i++)
			{
				temp.Add((int)list[low]);
				list.RemoveAt(low);
			}

			//insert range
			int offset = 0;
			if (pos>=low) //Insert position will have been decremented
			{
				offset = (high-low+1);
			}
			for (int i=temp.Count; i>0; i--)
			{
				list.Insert(pos-offset, temp[i-1]);
			}

			OnRefresh();
		}

		public void OnRefresh()
		{
			//if, for whatever reason, the control dimensions are not defined, bail
			if (this.Height<=0 || this.Width<=0) return;

			if (mData != null)
			{
				//define the boundries of the data set to be shown.
				Range displayRange = new Range(0,0,0,0);

				int numColumns = mData.GetUpperBound(1);

				displayRange.StartColumn = Math.Max(0, mDisplayedData.StartColumn);
				displayRange.StartRow = Math.Max(0, mDisplayedData.StartRow);

				displayRange.EndColumn = Math.Max(0, mDisplayedData.EndColumn);
				displayRange.EndColumn = Math.Min(numColumns,displayRange.EndColumn);

				if (mDisplayedData.EndRow>=0)
				{
					displayRange.EndRow = mDisplayedData.EndRow;
				}
				else
				{
					displayRange.EndRow = mData.GetUpperBound(0);
				}

				//note that data dimensions are the reverse of bitmap dimensions
				//assumes we are only going to average accross records, not within them
				//so the destination width is fixed, and only the destination height varies with the control size. 	
			
				//regardless of display range, we need to generate data across the full length of the data rows
				//in order to generated ZScore 
				
				Range r = new Range(displayRange);
				r.StartColumn = 0;
				r.EndColumn = numColumns;
				mAveragedData = AverageData(mData, r, numColumns, this.Height);

				r = new Range(0,0,mAveragedData.GetUpperBound(0),mAveragedData.GetUpperBound(1));
				mBitmap = mLegend.ApplyLegend(mAveragedData, r);

				BitmapTools bmt = new BitmapTools();

				//clip the bitmap to the display range
				if (displayRange.StartColumn != 0 || displayRange.EndColumn != mBitmap.Width-1)
				{
					r = new Range(displayRange);
					r.StartRow = 0;
					r.EndRow = mBitmap.Height-1;
					mBitmap = bmt.ClipBitmap(mBitmap, r);
				}

				if (mBitmap.Width != this.Width || mBitmap.Height != this.Height)
				{
					Console.WriteLine("Strecthing. Size of new bitmap = " + this.Size + " starting Bitmap size = " + mBitmap.Size) ; 
					mBitmap = bmt.StretchAndAlignBitmap(mBitmap, this.Width, this.Height, 
														this.mHorizontalAlignment, this.mVerticalAlignment);
				}

				//draws and refreshes client area
				ShowSelection();
			}
		}

		public ctlHeatMapClient()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			//this.SuspendLayout();

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

			pnlDisplay.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Pic_MouseWheel);
			

		}

        /// <summary>
/// AverageData
/// we are only averaging the height here.
/// assumption is that destWidth = srcWidth
/// </summary>
/// <param name="srcData"></param>
/// <param name="low"></param>
/// <param name="high"></param>
/// <param name="destWidth"></param>
/// <param name="destHeight"></param>
/// <returns></returns>
		private float [,] AverageData(float[,]srcData, Range displayRange, int destWidth, int destHeight)
		{
			float [,] destData = null;
			int nRecs = displayRange.NumRows;
			
			float frac = 1f;
			float currVal = float.NaN;

			try
			{
				if (nRecs<=destHeight) //no averaging needed, just map and copy
				{
					//note reversed dims
					destHeight = displayRange.NumRows;
					destWidth = displayRange.NumColumns;
					int srcWidth = srcData.GetUpperBound(1)+1;

					//note reversal of dimensions
					destData = new float [destHeight, destWidth];
					fixed (float* s = srcData)
						fixed (float* d = destData)
							for (int col = 0; col <destWidth; col++)
							{
								int j = colIndices[col+displayRange.StartColumn];
								for (int row=0; row<destHeight; row++)
								{
									int i = (int)rowIndices[row+displayRange.StartRow];
									//debug
//									int destOffset = (destWidth*row)+col;
//									int srcOffset = (srcWidth*(i))+j;
									*(d+(destWidth*row)+col) = *(s+(srcWidth*(i))+j);
								}
							}
					return(destData);
				}

				//note reversal of dimensions
				destData = new float [destHeight, destWidth];

				float srcToDestRatio =  (float)nRecs  / (float) destHeight;
				int startRow = displayRange.StartRow;
				int endRow = displayRange.EndRow;
				
				fixed (float* s = srcData)
				fixed (float* d = destData)
				for (int col = 0; col <destWidth; col++)
				{
					int c = colIndices[col];
					float* srcColOffset = s + c; 
					float* destColOffset = d + c; 

					for (int row=0; row<destHeight; row++)
					{
						//int i = (int)rowIndices[row];

						float fi = (float)row;
						float startBinF = (float) startRow + fi*srcToDestRatio;
						float stopBinF = startBinF + srcToDestRatio;
						int startBin = (int) startBinF;
						int stopBin = (int) stopBinF;
						if (stopBin>=endRow) stopBin = endRow-1;

						if (averageMode == AverageMode.average)
						{
							float val = 0f;
							float div = 0f;
							for (int j=startBin; j<=stopBin; j++)
							{
								if (j==stopBin)
									frac = stopBinF-startBinF;
								else
									frac = (float) j + 1.0f - startBinF;
								currVal = *(float*)(srcColOffset + ((destWidth+1) * rowIndices[j]));
								if (!float.IsNaN(currVal))
								{
									val+=currVal*frac;
									div+=frac;
								}
								startBinF+=frac;
							}
							startBinF=stopBinF;
							if (div>0f)
								*(destColOffset+(destWidth*row)) = val/div;
							else
								*(destColOffset+(destWidth*row)) = float.NaN;
						}
						else  //maximum value
						{
							float maxVal = float.MinValue;
							for (int j=startBin; j<stopBin; j++)
							{
								currVal = *(srcColOffset + (destWidth*rowIndices[j]));
								if (!float.IsNaN(currVal))
								{
									maxVal = Math.Max(maxVal,currVal);
								}
							}
							if (maxVal == float.MinValue)
								*(destColOffset+(destWidth*row)) = float.NaN;
							else
								*(destColOffset+(destWidth*row)) = maxVal;
						}
					}
				}
			} 
			catch (Exception e)
			{
				Console.WriteLine(e.ToString()) ; 
				//MessageBox.Show (e.ToString());
			}
			return (destData);
		}

//		private float [,] midAverageData(float[,]srcData, Range displayRange, int destWidth, int destHeight)
//		{
//			float [,] destData = null;
//			int nRecs = displayRange.NumRows;
//			
//			float frac = 1f;
//
//			try
//			{
//				if (nRecs<=destHeight) //no averaging needed, just map and copy
//				{
//					//note reversed dims
//					destHeight = displayRange.NumRows;
//					destWidth = displayRange.NumColumns;
//					int srcWidth = srcData.GetUpperBound(1)+1;
//
//					//note reversal of dimensions
//					destData = new float [destHeight, destWidth];
//					fixed (float* s = srcData)
//						fixed (float* d = destData)
//							for (int col = 0; col <destWidth; col++)
//							{
//								int j = colIndices[col+displayRange.StartColumn];
//								for (int row=0; row<destHeight; row++)
//								{
//									int i = (int)rowIndices[row+displayRange.StartRow];
//									*(d+(destWidth*row)+col) = *(s+(srcWidth*(i))+j);
//								}
//							}
//					return(destData);
//				}
//
//				//note reversal of dimensions
//				destData = new float [destHeight, destWidth];
//
//				float srcToDestRatio =  (float)nRecs  / (float) destHeight;
//				
//				fixed (float* s = srcData)
//					fixed (float* d = destData)
//						for (int col = 0; col <destWidth; col++)
//						{
//							int c = colIndices[col];
//
//							for (int row=0; row<destHeight; row++)
//							{
//								int i = (int)rowIndices[row];
//
//								float fi = (float)i;
//								float startBinF = (float) displayRange.StartRow + fi*srcToDestRatio;
//								float stopBinF = startBinF + srcToDestRatio;
//								int startBin = (int) startBinF;
//								int stopBin = (int) stopBinF;
//								if (stopBin>=displayRange.EndRow) stopBin = displayRange.EndRow-1;
//
//								if (averageMode == AverageMode.average)
//								{
//									float val = 0f;
//									float div = 0f;
//									for (int j=startBin; j<stopBin; j++)
//									{
//										frac = (float) j + 1.0f - startBinF;
//										if (!float.IsNaN(*(s+(destWidth*j)+c)))
//										{
//											val+=*(s+(destWidth*j)+c)*frac;
//											div+=frac;
//										}
//										startBinF+=frac;
//									}
//									frac = stopBinF - startBinF;
//									if (!float.IsNaN(*(s+(stopBin*destWidth)+c)))
//									{
//										val+=*(s+(stopBin*destWidth)+c)*frac;
//										div+=frac;
//									}
//									startBinF=stopBinF;
//									if (div>0f)
//										*(d+(destWidth*i)+c) = val/div;
//									else
//										*(d+(destWidth*i)+c) = float.NaN;
//								}
//								else  //maximum value
//								{
//									float maxVal = float.MinValue;
//									for (int j=startBin; j<stopBin; j++)
//									{
//										float val = *(s+(destWidth*j)+c); 
//										if (!float.IsNaN(val))
//										{
//											maxVal = Math.Max(maxVal,val);
//										}
//									}
//									if (maxVal == float.MinValue)
//										*(d+(destWidth*i)+c) = float.NaN;
//									else
//										*(d+(destWidth*i)+c) = maxVal;
//								}
//							}
//						}
//			} 
//			catch (Exception e){MessageBox.Show (e.ToString());}
//			return (destData);
//		}
//
//		private float [,] oldAverageData(float[,]srcData, Range displayRange, int destWidth, int destHeight)
//		{
//			float [,] destData = null;
//			int nRecs = displayRange.EndRow - displayRange.StartRow + 1;
//			float frac = 1f;
//
//			try
//			{
//				if (nRecs<=destHeight) //no averaging needed, just map and copy
//				{
//					destHeight = nRecs;
//					//note reversal of dimensions
//					destData = new float [destHeight, destWidth];
//					fixed (float* s = srcData)
//						fixed (float* d = destData)
//							for (int col = 0; col <destWidth; col++)
//							{
//								int j = colIndices[col];
//								for (int row=0; row<destHeight; row++)
//								{
//									int i = (int)rowIndices[row+displayRange.StartRow];
//									*(d+(destWidth*row)+col) = *(s+(destWidth*(i))+j);
//								}
//							}
//					return(destData);
//				}
//
//				//note reversal of dimensions
//				destData = new float [destHeight, destWidth];
//
//				float srcToDestRatio =  (float)nRecs  / (float) destHeight;
//				
//				fixed (float* s = srcData)
//					fixed (float* d = destData)
//						for (int col = 0; col <destWidth; col++)
//						{
//							int c = colIndices[col];
//
//							for (int row=0; row<destHeight; row++)
//							{
//								int i = (int)rowIndices[row];
//
//								float fi = (float)i;
//								float startBinF = (float) displayRange.StartRow + fi*srcToDestRatio;
//								float stopBinF = startBinF + srcToDestRatio;
//								int startBin = (int) startBinF;
//								int stopBin = (int) stopBinF;
//								if (stopBin>=displayRange.EndRow) stopBin = displayRange.EndRow-1;
//
//								if (averageMode == AverageMode.average)
//								{
//									float val = 0f;
//									float div = 0f;
//									for (int j=startBin; j<stopBin; j++)
//									{
//										frac = (float) j + 1.0f - startBinF;
//										if (!float.IsNaN(*(s+(destWidth*j)+c)))
//										{
//											val+=*(s+(destWidth*j)+c)*frac;
//											div+=frac;
//										}
//										startBinF+=frac;
//									}
//									frac = stopBinF - startBinF;
//									if (!float.IsNaN(*(s+(stopBin*destWidth)+c)))
//									{
//										val+=*(s+(stopBin*destWidth)+c)*frac;
//										div+=frac;
//									}
//									startBinF=stopBinF;
//									if (div>0f)
//										*(d+(destWidth*i)+c) = val/div;
//									else
//										*(d+(destWidth*i)+c) = float.NaN;
//								}
//								else  //maximum value
//								{
//									float maxVal = float.MinValue;
//									for (int j=startBin; j<stopBin; j++)
//									{
//										float val = *(s+(destWidth*j)+c); 
//										if (!float.IsNaN(val))
//										{
//											maxVal = Math.Max(maxVal,val);
//										}
//									}
//									if (maxVal == float.MinValue)
//										*(d+(destWidth*i)+c) = float.NaN;
//									else
//										*(d+(destWidth*i)+c) = maxVal;
//								}
//							}
//						}
//			} 
//			catch (Exception e){MessageBox.Show (e.ToString());}
//			return (destData);
//		}

//		public Bitmap xStretchBitmap(Bitmap src, int width, int height)
//		{
//			if (width * height <= 0) return null;
//
//			BitmapTools.PixelData linePixel = new Derek.BitmapTools.PixelData();
//
//			Bitmap dest = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//
//			//used to interpolate src pixel to use at dest position
//			double hFrac = (double) src.Height / (double) dest.Height;
//			double wFrac = (double) src.Width  / (double) dest.Width;
//
//			//used to interpolate src pixel to use at dest position
//			BitmapTools bmt = new BitmapTools();
//			BitmapTools.BitmapInfo fromInfo = bmt.LockBitmap(src);
//			BitmapTools.BitmapInfo toInfo = bmt.LockBitmap(dest);
//
//			for (int j=0; j<dest.Height; j++)
//			{
//				int y = (int) (hFrac*(j));
//				for(int i=0; i<dest.Width; i++)
//				{
//					int x = (int) (wFrac*(i));
//					BitmapTools.PixelData* srcPixel = bmt.PixelAt(fromInfo, x, y);
//					BitmapTools.PixelData* destPixel = bmt.PixelAt(toInfo, i, j);
//					*(destPixel) = *(srcPixel);
//				}
//			}
//
//			bmt.UnlockBitmap(src, fromInfo);
//			bmt.UnlockBitmap(dest, toInfo);
//
//			DrawDemarcations(dest);
//
//			return(dest);
//		}

//		public Bitmap xxStretchBitmap(Bitmap src, int width, int height)
//		{
//			if (width * height <= 0) return null;
//
//			BitmapTools.PixelData linePixel = new Derek.BitmapTools.PixelData();
//
//			Bitmap dest = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//
//			//used to interpolate src pixel to use at dest position
//			float hFrac = (float) src.Height / (float) dest.Height;
//			float wFrac = (float) src.Width  / (float) dest.Width;
//
//			//used to interpolate src pixel to use at dest position
//			BitmapTools bmt = new BitmapTools();
//			BitmapTools.BitmapInfo fromInfo = bmt.LockBitmap(src);
//			BitmapTools.BitmapInfo toInfo = bmt.LockBitmap(dest);
//
//			int y = 0;
//			//old inverted method
//			//for (int j=dest.Height-1; j>=0; j--)
//			for (int j=0; j<dest.Height; j++)
//				{
//				//mVerticalAlignment=null;
//				if (mVerticalAlignment!=null) //align bitmap to axis labels
//				{
//					//skip to the next demarcation
//					if (vAlignIndex<mVerticalAlignment.Length-2 &&
//							j==mVerticalAlignment[vAlignIndex+1]) 
//						vAlignIndex++;
//
//					//calculate the midpoint to select the appropriate representative y axis point
//					if (j==mVerticalAlignment[vAlignIndex])
//					{
//						y = mVerticalAlignment[vAlignIndex] + (mVerticalAlignment[vAlignIndex+1]-j)/2;
//						y = (int) (hFrac * (float) (y));
//					}
//				}
//				else
//				{
//					//y = (int) Math.Round(hFrac * (float) (j));
//					int jjj = (int) (hFrac * (float) (j));
//					y = (int) Math.Round(hFrac*j, 3);
//					y = (int) (hFrac*(j+1));
//					
//					int xxx = y;
//				}

//				if (mHorizontalAlignment!=null) //align bitmap to axis labels
//				{
//					int x = 0;
//					int hAlignIndex=0;
//					for(int i=0; i<dest.Width; i++)
//					{
//						if (hAlignIndex<mHorizontalAlignment.Length-2 &&
//							i==mHorizontalAlignment[hAlignIndex+1]) 
//							hAlignIndex++;
//
//						//calculate the midpoint to select the appropriate representative X axis point
//						if (i==mHorizontalAlignment[hAlignIndex])
//						{
//							x = mHorizontalAlignment[hAlignIndex] + (mHorizontalAlignment[hAlignIndex+1]-i)/2;
//							x = (int) (wFrac * (float) (x));
//						}
//
//						BitmapTools.PixelData* srcPixel = bmt.PixelAt(fromInfo, x, y);
//						BitmapTools.PixelData* destPixel = bmt.PixelAt(toInfo, i, j);
////						if (mVerticalAlignment!=null && j==mVerticalAlignment[vAlignIndex])
////							*(destPixel) = linePixel;
////						else if (mHorizontalAlignment!=null&&i==mHorizontalAlignment[hAlignIndex])
////							*(destPixel) = linePixel;
////						else
//							*(destPixel) = *(srcPixel);
//					}
//				}
//				else
//				{
//					for(int i=0; i<dest.Width; i++)
//					{
//						int x = (int) (wFrac * (float) (i));
//						BitmapTools.PixelData* srcPixel = bmt.PixelAt(fromInfo, x, y);
//						BitmapTools.PixelData* destPixel = bmt.PixelAt(toInfo, i, j);
//						*(destPixel) = *(srcPixel);
//					}
//				}
//			}
//
//			bmt.UnlockBitmap(src, fromInfo);
//			bmt.UnlockBitmap(dest, toInfo);
//
//			DrawDemarcations(dest);
//
//			return(dest);
//		}

		public void DrawDemarcations(Graphics g)
		{
			
			Pen greyPen = new Pen(Color.DarkGray, 1);
			Pen whitePen = new Pen(Color.White, 1);

			if (mVerticalAlignment!=null) 
			{
				int length = mVerticalAlignment.Length;
				
				for (int j=1; j<length-1; j++)
				{
					Point p1 = new Point(0, mVerticalAlignment[j]-1);
					Point p2 = new Point(pnlDisplay.Width, mVerticalAlignment[j]-1);
					g.DrawLine(whitePen, p1, p2);
					p1 = new Point(0, mVerticalAlignment[j]);
					p2 = new Point(pnlDisplay.Width, mVerticalAlignment[j]);
					g.DrawLine(greyPen, p1, p2);
				}
			}

			if (mHorizontalAlignment!=null) //align bitmap to axis labels
			{
				int length = mHorizontalAlignment.Length;

				for (int i=1; i<length-1; i++)
				{
					Point p1 = new Point(mHorizontalAlignment[i]-1, 0);
					Point p2 = new Point(mHorizontalAlignment[i]-1, pnlDisplay.Height);
					g.DrawLine(whitePen, p1, p2);
					p1 = new Point(mHorizontalAlignment[i], 0);
					p2 = new Point(mHorizontalAlignment[i], pnlDisplay.Height);
					g.DrawLine(greyPen, p1, p2);
				}
			}
		}

		public void DrawDemarcations(Bitmap dest)
		{
			BitmapTools bmt = new BitmapTools();
			BitmapTools.PixelData greyPixel = new Derek.BitmapTools.PixelData();
			BitmapTools.PixelData whitePixel = new Derek.BitmapTools.PixelData();
			Color c = Color.DarkGray;
			greyPixel.red = c.R;
			greyPixel.green = c.G;
			greyPixel.blue = c.B;

			c = Color.White;
			whitePixel.red = c.R;
			whitePixel.green = c.G;
			whitePixel.blue = c.B;

			if (mVerticalAlignment!=null) 
			{
				for (int j=1; j<mVerticalAlignment.Length-1; j++)
				{
					Point p1 = new Point(0, mVerticalAlignment[j]-1);
					Point p2 = new Point(dest.Width, mVerticalAlignment[j]-1);
					bmt.DrawHorizontalLine (dest, p1, p2, whitePixel);
					p1 = new Point(0, mVerticalAlignment[j]);
					p2 = new Point(dest.Width, mVerticalAlignment[j]);
					bmt.DrawHorizontalLine (dest, p1, p2, greyPixel);

				}
			}

			if (mHorizontalAlignment!=null) //align bitmap to axis labels
			{
				for (int i=1; i<mHorizontalAlignment.Length-1; i++)
				{
					Point p1 = new Point(mHorizontalAlignment[i]-1, 0);
					Point p2 = new Point(mHorizontalAlignment[i]-1, dest.Height);
					bmt.DrawVerticalLine (dest, p1, p2, whitePixel);
					p1 = new Point(mHorizontalAlignment[i], 0);
					p2 = new Point(mHorizontalAlignment[i], dest.Height);
					bmt.DrawVerticalLine (dest, p1, p2, greyPixel);
				}
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlDisplay = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// pnlDisplay
			// 
			this.pnlDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlDisplay.Location = new System.Drawing.Point(0, 0);
			this.pnlDisplay.Name = "pnlDisplay";
			this.pnlDisplay.Size = new System.Drawing.Size(352, 248);
			this.pnlDisplay.TabIndex = 0;
			this.pnlDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Pic_MouseUp);
			this.pnlDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlDisplay_Paint);
			this.pnlDisplay.MouseHover += new System.EventHandler(this.pnlDisplay_MouseHover);
			this.pnlDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Pic_MouseMove);
			this.pnlDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Pic_MouseDown);
			// 
			// ctlHeatMapClient
			// 
			this.Controls.Add(this.pnlDisplay);
			this.Name = "ctlHeatMapClient";
			this.Size = new System.Drawing.Size(352, 248);
			this.ResumeLayout(false);

		}
		#endregion


		private void PaintBitmap(Graphics g)
		{
			Console.WriteLine("ctlHeatMapClient PaintBitmap: Size = "+ this.Size) ; 
			if (mBitmap!=null)
			{
				g.DrawImage (mSelectedBitmap, 0, 0);
				if (mblnDrawDemarcations)
					DrawDemarcations(g);
			}
		}
		private void pnlDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (mBitmap != null && mBitmap.Size != this.Size)
				OnRefresh() ; 
			Console.WriteLine("ctlHeatMapClient: pnlDisplay_Paint: Size = "+ this.Size ) ; 
			PaintBitmap(e.Graphics);
			if (BitmapPainted!=null)
				BitmapPainted(e.Graphics);
		}
		#region Zoom
		/// <summary>
		/// mousedown event for numeric axis, starts zoom
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Pic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Right)
				{
					if (UnZoom!=null)
						UnZoom();
				}
				else
				{
					mZooming = true;
					mZoomRectangle = new Rectangle(((Control)sender).PointToScreen(new Point(e.X, e.Y)), new Size(1,1));
					ControlPaint.DrawReversibleFrame(mZoomRectangle, this.BackColor, FrameStyle.Dashed);
				}
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
		private void Pic_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Delta < 0)
			{
				if (Zoom!=null)
				{
					int delta = mMinSelectionRange/2;

					Point start =new Point (e.X-delta, e.Y-delta);
					Point stop = new Point(e.X+delta, 	e.Y+delta);
					Zoom(start, stop);
				}
			}
			else
			{
				if (UnZoom!=null)
					UnZoom();
			}
		}
		/// <summary>
		/// mousemove event for numeric axis, shows zoom rectangle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Pic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				ShowZoom((Control)sender, e.X, e.Y);
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
		/// <summary>
		/// mouseup event for numeric axis, stops zoom, applies zoom
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Pic_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if (!mZooming)  return;
				if (e.Button == MouseButtons.Right) return;

				mZooming = false;
				ShowZoom((Control)sender, e.X, e.Y);
				ControlPaint.DrawReversibleFrame(mZoomRectangle, ((Control)sender).BackColor, FrameStyle.Dashed);
				if (Zoom!=null)
				{
					Point start = this.PointToClient(new Point (mZoomRectangle.X, mZoomRectangle.Y));
					Point stop = this.PointToClient(new Point(mZoomRectangle.X + mZoomRectangle.Width, 
						mZoomRectangle.Y + mZoomRectangle.Height));

					//if (Math.Abs(start.X-stop.X) < mMinSelectionRange) return;
					if (Math.Abs(start.Y-stop.Y) < mMinSelectionRange) return;

					Zoom(start, stop);
				}
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
		/// <summary>
		/// shows zoom rectangle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void ShowZoom (Control sender, int x, int y)
		{
			try
			{
				if (mZooming)
				{
					//hide old rectangle
					ControlPaint.DrawReversibleFrame(mZoomRectangle, sender.BackColor, FrameStyle.Dashed);
					Point newPoint = sender.PointToScreen(new Point(x, y));
					mZoomRectangle.Width = newPoint.X - mZoomRectangle.X;
					mZoomRectangle.Height = newPoint.Y - mZoomRectangle.Y;
					
					ControlPaint.DrawReversibleFrame(mZoomRectangle, sender.BackColor, FrameStyle.Dashed);
				}
			}
			catch(Exception ex){System.Windows.Forms.MessageBox.Show(ex.Message);}
		}
		#endregion
		private void pnlDisplay_MouseHover(object sender, System.EventArgs e)
		{
			pnlDisplay.Focus();
		}
	}
}
