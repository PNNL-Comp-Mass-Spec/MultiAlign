
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    public class SteinDotProduct: ISpectralComparer
    {
        public double CompareSpectra(MSSpectra spectraX, MSSpectra spectraY)
        {
            var a = spectraX.Peaks;
            var b = spectraY.Peaks;


            // Then compute the magnitudes of the spectra
            double sum = 0;
            double sumOne = 0;
            double sumTwo = 0;
            for (var i = 0; i < spectraX.Peaks.Count; i++)
            {
                var x = a[i].Y;
                var y = b[i].Y;

                sum += x*y;
                sumOne += (x*x);
                sumTwo += (y*y);
            }

            
            return sum / (sumOne * sumTwo);
        }
    }
}
