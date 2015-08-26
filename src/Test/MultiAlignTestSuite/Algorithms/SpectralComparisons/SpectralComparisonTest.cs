using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.TextFiles;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.SpectralComparisons
{

    [TestFixture]
    public class SpectralComparisonTest
    {

        private List<MSSpectra> GetSpectra(string path)
        {
            var spectrum        = new MSSpectra();
            spectrum.Peptides         = new List<Peptide>();
            IMsMsSpectraReader reader = new MgfFileReader();
            var spectra   =  reader.Read(path);         
   
            return spectra;
        }
               
    }
}
