using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Data.Features;
using PNNLOmics.Utilities;

namespace MultiAlignCore.Algorithms.FeatureMatcher.Utilities
{
    static public class MatrixUtilities
    {
        #region Remove rows/columns
        /// <summary>
        /// Removes the row and column in which a 0 occurs along the diagonal of a square matrix.
        /// </summary>
        /// <param name="matrix">A square matrix, possibly with a 0 on the diagonal.</param>
        /// <returns>A square matrix with no 0's on the diagonal.</returns>
        static public DenseMatrix ReduceMatrix(DenseMatrix  matrix)
        {
            var rows = matrix.RowCount;

            if (rows != matrix.ColumnCount)
            {
                throw new InvalidOperationException("Matrix is not square in function ReduceMatrix.");
            }

            if (rows == matrix.Rank())
            {
                return matrix;
            }
            
            var reducedMatrix = matrix.Clone() as DenseMatrix;
                
            for (var rIndex = 0; rIndex < rows; rIndex++)
            {
                if (matrix[rIndex, rIndex] == 0)
                {
                    reducedMatrix = ReduceMatrix(reducedMatrix, rIndex);
                }
            }

            return reducedMatrix;
        }
        /// <summary>
        /// Remove the given row and column from a matrix.
        /// </summary>
        /// <param name="matrix">The matrix from with the row and column are to be removed.</param>
        /// <param name="rowColumnIndex">The index of the row and column which are to be removed.</param>
        /// <returns>A copy of matrix without the row and column given by rowColumnIndex.</returns>
        static public DenseMatrix ReduceMatrix(DenseMatrix  matrix, int rowColumnIndex)
        {
            var rows = matrix.RowCount;

            if (rowColumnIndex >= rows)
            {
                throw new InvalidOperationException("Given rowColumnIndex is out of range of matrix in function ReduceMatrix.");
            }
            if (rows != matrix.ColumnCount)
            {
                throw new InvalidOperationException("Matrix is not square in function ReduceMatrix.");
            }
            var reducedMatrix = new DenseMatrix(rows - 1, rows - 1);
            var rowIndex = 0;

            for (var rIndex = 0; rIndex < rows; rIndex++)
            {
                if (rIndex != rowColumnIndex)
                {
                    var colIndex = 0;
                    for (var cIndex = 0; cIndex < rows; cIndex++)
                    {
                        if (cIndex != rowColumnIndex)
                        {
                            reducedMatrix[rowIndex, colIndex] = matrix[rIndex, cIndex];
                            colIndex++;
                        }
                    }
                    rowIndex++;
                }
            }

            return reducedMatrix;
        }
        /// <summary>
        /// Remove a single row from an [n x 1] matix.
        /// </summary>
        /// <param name="matrix">The original matrix.</param>
        /// <param name="rowIndex">The index of the row to be removed.</param>
        /// <returns>The matrix without the given row.</returns>
        static public DenseMatrix RemoveRow(DenseMatrix  matrix, int rowIndex)
        {
            var rows = matrix.RowCount;

            if (rowIndex >= rows)
            {
                throw new InvalidOperationException("Given rowIndex is out of range of matrix in function RemoveRow.");
            }

            if (matrix.ColumnCount > 1)
            {
                throw new InvalidOperationException("Given matrix in function RemoveRow must have no more than 1 column.");
            }

            var reducedMatrix= new DenseMatrix(rows-1,1);
            var rowCount = 0;

            for (var rIndex = 0; rIndex < rows; rIndex++)
            {
                if (rIndex != rowIndex)
                {
                    reducedMatrix[rowCount, 0] = matrix[rIndex, 0];
                    rowCount++;
                }
            }

            return reducedMatrix;
        }
        #endregion

        #region Difference vectors

        /// <summary>
        /// Find the differences between any two features.
        /// </summary>
        /// <typeparam name="T">Feature or derived class.</typeparam>
        /// <typeparam name="TU">Feature or derived class.</typeparam>
        /// <param name="feature1">Observed feature to be compared to other feature.</param>
        /// <param name="feature2">Feature (MassTag) to be compared to.</param>
        /// <param name="driftTime">true/false:  Whether or not to include the drift time difference.</param>
        /// <returns>An [n x 1] DenseMatrix containing the differences between the two features.</returns>
        public static DenseMatrix Differences<T, TU>(T feature1, TU feature2, bool driftTime)
            where T : FeatureLight
            where TU : FeatureLight
        {
            var dimension = 2;
            if (driftTime)
                dimension++;
            var differences = new DenseMatrix(dimension, 1);

			if (!double.IsNaN(feature1.MassMonoisotopicAligned) && feature1.MassMonoisotopicAligned > 0.0)
			{
				differences[0, 0] = MathUtilities.MassDifferenceInPpm(feature1.MassMonoisotopicAligned, feature2.MassMonoisotopic);
			}
			else
			{
				differences[0, 0] = MathUtilities.MassDifferenceInPpm(feature1.MassMonoisotopic, feature2.MassMonoisotopic);
			}

			if (!double.IsNaN(feature1.NetAligned) && feature1.NetAligned > 0.0)
			{
				differences[1, 0] = feature1.NetAligned - feature2.Net;
			}
			else
			{
				differences[1, 0] = feature1.Net - feature2.Net;
			}

			if (driftTime)
			{
				double feature1DriftTime;
				double feature2DriftTime;

				if (!double.IsNaN(feature1.DriftTimeAligned) && feature1.DriftTimeAligned > 0.0)
				{
					feature1DriftTime = feature1.DriftTimeAligned;
				}
				else
				{
					feature1DriftTime = feature1.DriftTime;
				}
				if (feature2.DriftTimeAligned != double.NaN && feature2.DriftTimeAligned > 0.0)
				{
					feature2DriftTime = feature2.DriftTimeAligned;
				}
				else
				{
					feature2DriftTime = feature2.DriftTime;
				}

				differences[2, 0] = feature1DriftTime - feature2DriftTime;
			}

            return differences;
        }
        
        
        #endregion
    }
}
