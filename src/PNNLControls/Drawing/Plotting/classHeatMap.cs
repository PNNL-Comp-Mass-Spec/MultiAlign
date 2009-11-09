using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D; 
using System.Collections.Generic;

using Derek;

namespace PNNLControls.Drawing.Plotting
{

    public enum HeatMapInterpolationMode
    {
        Average,
        Maximum
    }

    /// <summary>
    /// Class that renders heat map blocks.
    /// </summary>
    public class classHeatMap
    {
        private HeatMapInterpolationMode menum_averageMode;

        /// <summary>
        /// Gets or sets the mode of how data is interpolated.
        /// </summary>
        public HeatMapInterpolationMode InterpolationMode
        {
            get
            {
                return menum_averageMode;
            }
            set
            {
                menum_averageMode = value;
            }
        }

        /// <summary>
        /// Averages the data 
        /// </summary>
        /// <param name="srcData"></param>
        /// <param name="displayRange"></param>
        /// <param name="destWidth"></param>
        /// <param name="destHeight"></param>
        /// <returns></returns>
        public unsafe float[,] InterpolateData(float[,] srcData,
                                     int numDestinationRows,
                                     int numDestinationCols)                                     
        {
            float[,] destData = null;

            int numSourceRows = srcData.GetLength(1);
            int numSourceCols = srcData.GetLength(1);            

            /// 
            /// If we expanding the data.
            /// 
            if (numSourceRows <= numDestinationRows)
            {
                destData = new float[numDestinationRows, numDestinationCols];
                fixed (float* s = srcData)
                fixed (float* d = destData)

                for (int row = 0; row < numDestinationRows; row++)                        
                    for (int col = 0; col < numDestinationCols; col++)
                        *(d + (numDestinationCols * row) + col) = *(s + (numSourceCols * row) + col);
                
            }
            /// 
            /// If we are reducing the data.
            /// 
            else
            {                    
                /// 
                /// Create a new array to hold the binned data.
                ///        
                destData = new float[numDestinationRows, numDestinationCols];

                double ratioCol = Convert.ToDouble(numSourceCols) / Convert.ToDouble(numDestinationCols);
                double ratioRow = Convert.ToDouble(numSourceRows) / Convert.ToDouble(numDestinationRows);

                /// 
                /// This calculation says that we are going to sample only a small fraction 
                /// of the cells when we have uneven column size reductions e.g. Not 4->2, but 9->6
                /// 
                int modCols      = numSourceCols % numDestinationCols;
                float scaleCols = Convert.ToSingle(modCols) / Convert.ToSingle(numDestinationCols);
                
                int modRows      = numSourceRows % numDestinationRows;
                float scaleRows = Convert.ToSingle(modRows) / Convert.ToSingle(numDestinationRows);

                /// 
                /// Index values
                /// 
                int i       = 0;  // row
                int j       = 0;  // col   
                
                /// 
                /// Setup temporary variables
                /// 
                float sum   = 0.0F;
                float value = 0.0F;
                int   N     = 0;
                int index   = 0;

                /// 
                /// Used to clamp end indices.
                /// 
                int jIndex;  
                int iIndex;  

                /// 
                /// This determines many columns and row do we span while reducing data                
                ///                 
                int spanCol = Convert.ToInt32(Math.Ceiling(ratioCol)); 
                int spanRow = Convert.ToInt32(Math.Ceiling(ratioRow)); 

                /// 
                /// Use pointers for quick data access
                /// 
                fixed (float* pSource = srcData)
                fixed (float* pDest   = destData)
                
                /// 
                /// Row major ordering, attempting to access memory in contiguous blocks.
                /// 
                
                for (int row = 0; row < numDestinationRows; row++)                        
                {       
                    i = row*spanRow;                    
                    for (int col = 0; col < numDestinationCols; col++)
                    {        
                        j = col*spanCol;
                        if (menum_averageMode == HeatMapInterpolationMode.Average)
                        {
                            sum = 0.0F;
                            N   = 0;
                            /// 
                            /// From our base offset, calculate the sum/max a
                            /// span away.
                            /// 
                            for (int iOffset = 0; iOffset < spanRow; iOffset++)
                            {
                                for (int jOffset = 0; jOffset < spanCol; jOffset++)
                                {
                                    /// 
                                    /// Clamp the indices to stay within the bounds of the array.
                                    /// 
                                    /// This keeps summations from being incorrectly performed
                                    /// since the array is contiguous in memory.  Thus
                                    /// accessing column m where m > M, the width of an array
                                    /// and m lt N*M - 1 would wrap the data access to an 
                                    /// incorrect part of the array.
                                    /// 
                                    jIndex = (j + jOffset);
                                    if (jIndex >= numSourceCols)
                                        jIndex = numSourceCols - 1;

                                    iIndex = (i + iOffset);
                                    if (iIndex >= numSourceRows)                                    
                                        iIndex = numSourceRows - 1;
                                    
                                    index  =  (numSourceCols * iIndex) + jIndex;
                                    value = *(pSource + index);
                                    if (float.IsNaN(value) == false)
                                    {
                                        sum += value;
                                        N++;
                                    }
                                }
                            }
                            *(pDest + ((row * numDestinationCols) + col)) = sum / Convert.ToSingle(N);
                        }                            
                    }
                }
            }                    
            return destData;
        }
        
        /// <summary>
        /// Draws the data to the graphics object.
        /// </summary>
        /// <param name="g">Graphics to render data to.</param>
        /// <param name="data">Data to render.</param>
        public unsafe void Draw(Graphics g, Rectangle bounds, float [,] data)
        {
            int width  = bounds.Width;
            int height = bounds.Height;

            float[,] interpolatedData = InterpolateData(data,
                                                        width,
                                                        height);
                                                        
            /// 
            /// Draw...
            ///     
            Range r = new Range(0,
                                0,
                                interpolatedData.GetUpperBound(0),
                                interpolatedData.GetUpperBound(1));

            Bitmap bitmap = new Bitmap( width, 
                                        height, 
                                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);                
            BitmapTools bitmapTools = new BitmapTools();            // Tools to finding information about image
            BitmapTools.BitmapInfo info = bitmapTools.LockBitmap(bitmap);   // info.
            
            try
            {
                int bmpOffset = (int)info.pBase;
                int bmpWidth  = (int)info.width;
                
                byte  pixelByte  = 0;
                float pixelFloat = 0.0F;
                                
                fixed (float* pData = interpolatedData)
                {
                    for (int i = 0; i < height*width; i++)
                    {
                        try
                        {
                            BitmapTools.PixelData* pPixel = (BitmapTools.PixelData*)(bmpOffset + i*sizeof(BitmapTools.PixelData));
                            pixelFloat  = *(pData + i);                         // Get the continuous representation. 
                            pixelByte   = Convert.ToByte(pixelFloat * 255);     // Then convert to a byte.

                            /// 
                            /// Scale only on red
                            /// 
                            pPixel->red   = pixelByte;     
                            pPixel->green = 0;
                            pPixel->blue  = 0;


                        }
                        catch (Exception e)
                        { 
                            System.Windows.Forms.MessageBox.Show(e.Message);
                        }                        
                    }
                }
            }
            catch (Exception e) 
            {
                System.Windows.Forms.MessageBox.Show(e.Message); 
            }
            finally
            {
                bitmapTools.UnlockBitmap(bitmap, info);                
            }

            g.DrawImage(bitmap, 0, 0);
        }
    }
}
