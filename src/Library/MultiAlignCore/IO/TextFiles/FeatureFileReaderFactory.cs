using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.TextFiles
{
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.DatasetLoaders;

    public class FeatureFileReaderFactory
    {
        public static ITextFileReader<MSFeatureLight> GetMsFeatureFileReader(string filePath)
        {   // We only support one feature file type that returns MSFeatures
            return new MsFeatureLightFileReader();
        }

        public static ITextFileReader<UMCLight> GetLcmsFeatureFileReader(string filePath)
        {
            ITextFileReader<UMCLight> featureFileReader = null;
            if (filePath.EndsWith(".ms1ft"))
            {
                //featureFileReader = new PromexFileReader();
            }
            else if (filePath.EndsWith("_LcmsFeatures.txt"))
            {
                featureFileReader = new LcImsFeatureFileReader();
            }
            else
            {
                throw new ArgumentException("No feature file reader for given file type.");
            }

            return featureFileReader;
        }
    }
}
