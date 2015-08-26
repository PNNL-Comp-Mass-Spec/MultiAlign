using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Algorithms.Distance;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.Distance
{
	/// <summary>
	/// Test class for methods located in MahalanobisDistanceCalculator that contains various methods for calculating the mahalanobis distance.
	/// </summary>
	[TestFixture]
	public class DistanceCalculatorTests
	{
		private DenseMatrix m_matrixA;
		private DenseMatrix m_matrixB;
		private DenseMatrix m_covarianceMatrixA;
		private DenseMatrix m_covarianceMatrixB;

		private double[] m_meanArrayA;
		private double[] m_meanArrayB;

		// This value is used as the delta for comparing double values for unit tests to deal with floating-point issues
		private const double DELTA = 0.0000001;

		[SetUp]
		public void CreateTestMatrices()
		{
			double[,] arrayA = { { 2, 2 }, { 2, 5 }, { 6, 5 }, { 7, 3 }, { 4, 7 }, { 6, 4 }, { 5, 3 }, { 4, 6 }, { 2, 5 }, { 1, 3 } };
			double[,] arrayB = { { 6, 5 }, { 7, 4 }, { 8, 7 }, { 5, 6 }, { 5, 4 } };

			m_matrixA = DenseMatrix.OfArray(arrayA);
			m_matrixB = DenseMatrix.OfArray(arrayB);

			double[,] covarianceArrayA = { { 3.89, 0.13 }, { 0.13, 2.21 } };
			double[,] covarianceArrayB = { { 1.36, 0.56 }, { 0.56, 1.36 } };

            
			m_covarianceMatrixA = DenseMatrix.OfArray(covarianceArrayA);
			m_covarianceMatrixB = DenseMatrix.OfArray(covarianceArrayB);

			m_meanArrayA = new[] { 3.9, 4.3 };
			m_meanArrayB = new[] { 6.2, 5.2 };
		}

		[Test]
		public void TestCalculateArithmeticMean()
		{
			var meanA = MahalanobisDistanceCalculator.CalculateArithmeticMean(m_matrixA);
			var meanB = MahalanobisDistanceCalculator.CalculateArithmeticMean(m_matrixB);

			Assert.AreEqual(3.9, meanA[0], DELTA);
			Assert.AreEqual(4.3, meanA[1], DELTA);
			Assert.AreEqual(6.2, meanB[0], DELTA);
			Assert.AreEqual(5.2, meanB[1], DELTA);
		}

		[Test]
		public void TestCenterMatrixOnArithmeticMean()
		{
			var centeredMatrixA = MahalanobisDistanceCalculator.CenterMatrixOnArithmeticMean(m_matrixA, m_meanArrayA);
			var centeredMatrixB = MahalanobisDistanceCalculator.CenterMatrixOnArithmeticMean(m_matrixB, m_meanArrayB);

			double[,] testArrayA = { { -1.9, -2.3 }, { -1.9, 0.7 }, { 2.1, 0.7 }, { 3.1, -1.3 }, { 0.1, 2.7 }, { 2.1, -0.3 }, { 1.1, -1.3 }, { 0.1, 1.7 }, { -1.9, 0.7 }, { -2.9, -1.3 } };
			double[,] testArrayB = { { -0.2, -0.2 }, { 0.8, -1.2 }, { 1.8, 1.8 }, { -1.2, 0.8 }, { -1.2, -1.2 } };

			var testMatrixA = DenseMatrix.OfArray(testArrayA);
			var testMatrixB = DenseMatrix.OfArray(testArrayB);

			for (var i = 0; i < centeredMatrixA.RowCount; i++)
			{
				for (var j = 0; j < centeredMatrixA.ColumnCount; j++)
				{
					Assert.AreEqual(testMatrixA[i, j], centeredMatrixA[i, j], DELTA);
				}
			}

			for (var i = 0; i < centeredMatrixB.RowCount; i++)
			{
				for (var j = 0; j < centeredMatrixB.ColumnCount; j++)
				{
					Assert.AreEqual(testMatrixB[i, j], centeredMatrixB[i, j], DELTA);
				}
			}
		}

		[Test]
		public void TestCreateCovarianceMatrix()
		{
			var covarianceMatrixA = MahalanobisDistanceCalculator.CreateCovarianceMatrix(m_matrixA, m_meanArrayA);
			var covarianceMatrixB = MahalanobisDistanceCalculator.CreateCovarianceMatrix(m_matrixB, m_meanArrayB);

			var testMatrixA = m_covarianceMatrixA;
			var testMatrixB = m_covarianceMatrixB;

			for (var i = 0; i < covarianceMatrixA.RowCount; i++)
			{
				for (var j = 0; j < covarianceMatrixA.ColumnCount; j++)
				{
					Assert.AreEqual(testMatrixA[i, j], covarianceMatrixA[i, j], DELTA);
					Assert.AreEqual(testMatrixB[i, j], covarianceMatrixB[i, j], DELTA);
				}
			}
		}

		[Test]
		public void TestCreatePooledCovarianceMatrix()
		{
			var pooledCovarianceMatrix = MahalanobisDistanceCalculator.CreatePooledCovarianceMatrix(m_covarianceMatrixA, m_covarianceMatrixB, m_matrixA, m_matrixB);

			double[,] testArray = { { 3.04667, 0.27333 }, { 0.27333, 1.92667 } };
			var testMatrix = DenseMatrix.OfArray(testArray);

			for (var i = 0; i < pooledCovarianceMatrix.RowCount; i++)
			{
				for (var j = 0; j < pooledCovarianceMatrix.ColumnCount; j++)
				{
					Assert.AreEqual(testMatrix[i, j], Math.Round(pooledCovarianceMatrix[i, j], 5), DELTA);
				}
			}
		}

		[Test]
		public void TestCreateMeanDifferenceMatrix()
		{
			var meanDifferenceMatrix = MahalanobisDistanceCalculator.CreateMeanDifferenceMatrix(m_meanArrayA, m_meanArrayB);

			double[,] testArray = { { -2.3 }, { -0.9 } };
			var testMatrix = DenseMatrix.OfArray(testArray);

			for (var i = 0; i < meanDifferenceMatrix.RowCount; i++)
			{
				for (var j = 0; j < meanDifferenceMatrix.ColumnCount; j++)
				{
					Assert.AreEqual(testMatrix[i, j], meanDifferenceMatrix[i, j], DELTA);
				}
			}
		}

		[Test]
		public void CalculateMahalanobisDistance()
		{
			var mahalanobisDistance = MahalanobisDistanceCalculator.CalculateMahalanobisDistance(m_matrixA, m_matrixB);
			Assert.AreEqual(1.4104178399830798, mahalanobisDistance);
		}
	}
}
