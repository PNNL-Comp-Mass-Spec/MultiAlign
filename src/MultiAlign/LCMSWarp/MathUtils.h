#pragma once
#include <math.h> 
#include <vector>
#include <fstream>
#using <mscorlib.dll>

const double M_GAMMAl = 0.5772156649015328606065120900824024; 
const double M_PI = 3.1415926535897932384626433832795029; 
const double TWOPI = 2 * 3.1415926535897932384626433832795029; 
const double el = 0.5772156649015329; 

namespace Utilities
{
	class MathUtils
	{
	public:
		static double psi(double x); 
		static double invPsi(double y); 
		static double gamma(double x); 

		inline static double PValGamma(double x, double alpha, double beta) 
		{
			if (x < 0)
				x = 0; 
			double gammaAlpha = MathUtils::gamma(alpha); 
			double PvalGamma = pow(x, alpha-1); 
			double expPortion = exp(-1*x/beta); 
			PvalGamma *= expPortion; 
			double denom = (pow(beta, alpha) * gammaAlpha); 
			PvalGamma /= denom; 
			return PvalGamma; 
		}

		static void CalculateGammaParametersByMethodOfMoments(std::vector<double> &vals, 
			double &alpha, double &beta, double &gamma)
		{
			std::vector<double> valsB; 

			// gamma is the minimum observed value. 
			int numPts = (int) vals.size(); 
			if (numPts == 0)
				throw "No data was passed to CalculateGammaParametersByMethodOfMoments"; 

			gamma = vals[0]; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				if (vals[ptNum] < gamma)
					gamma = vals[ptNum]; 
			}
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				valsB.push_back(vals[ptNum] - gamma); 
			}
			double meanGamma = 0, stdGamma = 0; 
			CalcMeanAndStd(valsB, meanGamma, stdGamma); 

			alpha = (meanGamma * meanGamma) / (stdGamma * stdGamma); // mean squared over variance. 
			beta = (stdGamma * stdGamma ) / meanGamma; 
		}
		
		static void TwoDEM(const std::vector<double> &x, std::vector<double> &y, double &p, double &u, 
			double &muX, double &muY, double &stdX, double &stdY)
		{


			const int NUM_ITERATIONS = 40; 
			int numPts = (int) x.size();
			double *pVals[2]; 
			pVals[0] = new double [numPts]; 
			pVals[1] = new double [numPts]; 

			double minX=x[0], maxX=x[0]; 
			double minY=y[0], maxY=y[0]; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				if (x[ptNum] < minX)
					minX = x[ptNum]; 
				if (x[ptNum] > maxX)
					maxX = x[ptNum]; 
				if (y[ptNum] < minY)
					minY = y[ptNum]; 
				if (y[ptNum] > maxY)
					maxY = y[ptNum]; 
			}

			u = 1.0/((maxX-minX) * (maxY - minY)); 
			p = 0.5;

			CalcMeanAndStd(x, muX, stdX); 
			stdX /= 3; 
			CalcMeanAndStd(y, muY, stdY); 
			stdY /= 3; 

			for (int iterNum = 0; iterNum < NUM_ITERATIONS; iterNum++)
			{
				// Calculate current probability assignments. 
				// i.e. p(y |x, theta)
				for (int ptNum = 0; ptNum < numPts; ptNum++)
				{
					double xDiff = (x[ptNum] - muX) / stdX; 
					double yDiff = (y[ptNum] - muY) / stdY; 
					pVals[0][ptNum] = p * exp(-0.5 * (xDiff * xDiff + yDiff * yDiff)) / (TWOPI * stdX * stdY); 
					pVals[1][ptNum] = (1-p) * u; 
					double sum = pVals[0][ptNum] + pVals[1][ptNum]; 
					pVals[0][ptNum] /= sum; 
					pVals[1][ptNum] /= sum; 
				}

				// Calculate new estimates from maximization step. 
				double pNumerator = 0; 
				double muXNumerator = 0; 
				double muYNumerator = 0; 
				double sigmaXNumerator = 0; 
				double sigmaYNumerator = 0; 

				double pDenominator = 0; 
				double denominator = 0; 

				for (int ptNum = 0; ptNum < numPts; ptNum++)
				{
					pNumerator += pVals[0][ptNum]; 
					pDenominator += (pVals[0][ptNum] + pVals[1][ptNum]); 

					double xDiff = (x[ptNum] - muX); 
					muXNumerator += pVals[0][ptNum] * x[ptNum]; 
					sigmaXNumerator += pVals[0][ptNum] * xDiff * xDiff; 

					double yDiff = (y[ptNum] - muY); 
					muYNumerator += pVals[0][ptNum] * y[ptNum]; 
					sigmaYNumerator += pVals[0][ptNum] * yDiff * yDiff; 

					denominator += pVals[0][ptNum]; 
				}

				muX = muXNumerator / denominator; 
				muY = muYNumerator / denominator; 
				stdX = sqrt(sigmaXNumerator / denominator); 
				stdY = sqrt(sigmaYNumerator / denominator); 
				p = pNumerator / pDenominator; 
			}
		}

		static void CalcMeanAndStd(const std::vector<double> &values, double &mean, double &stdev)
		{
			double numPts = (double) values.size(); 
			double sumSquares = 0; 
			double sum = 0; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				double val = values[ptNum]; 
				sum += val; 
				sumSquares += val * val; 
			}
			mean = sum/numPts; 
			stdev = sqrt((numPts*sumSquares - sum * sum)) / (sqrt(numPts) *sqrt(numPts-1)); 
		}

		static void CalcTruncatedMeanAndStd(const std::vector<double> &values, double minVal, 
			double &mean, double &stdev)
		{
			double numPts = values.size(); 
			double sumSquares = 0; 
			double sum = 0; 
			double numPtsUsed = 0; 
			for (int ptNum = 0; ptNum < numPts; ptNum++)
			{
				double val = values[ptNum]; 
				if (val > minVal)
				{
					sum += val; 
					sumSquares += val * val; 
					numPtsUsed++; 
				}
			}
			if (numPtsUsed < 3)
			{
				throw "Not enough points in range"; 
			}
			mean = sum/numPtsUsed; 
			stdev = sqrt(numPtsUsed*sumSquares - sum * sum)/ (sqrt(numPtsUsed) * sqrt(numPtsUsed-1)); 
		}

		MathUtils(void);
		~MathUtils(void);
	};
}