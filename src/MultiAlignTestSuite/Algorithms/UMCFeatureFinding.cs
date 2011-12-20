using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using PNNLOmics.Data.Features;
using MultiAlignEngine.Features;
using MultiAlignCore.Algorithms.FeatureFinding;

namespace MultiAlignTestSuite.Algorithms
{
    public class UMCFeatureFinding
    {
        public void Test()
        {
            string path     = @"m:\data\proteomics\chronicFatigue\crap.txt";
            string[] lines  = File.ReadAllLines(path);

            List<MSFeatureLight> features = new List<MSFeatureLight>();
            int i = 0;
            foreach (string line in lines)
            {
                i++;
                if (i == 1)
                {
                    continue;
                }
                string[] data   = line.Split('\t');
                int scan        = Convert.ToInt32(data[1]);
                double net      = Convert.ToDouble(data[2]);
                long abundance  = Convert.ToInt64(data[3]);
                double weight   = Convert.ToDouble(data[5]);
                double fit      = Convert.ToDouble(data[6]);

                MSFeatureLight feature = new MSFeatureLight();
                feature.ID   = i;
                feature.Scan = scan;
                feature.NET  = net;
                feature.MassMonoisotopic    = weight;
                feature.Score               = fit;
                feature.Abundance           = abundance;
                features.Add(feature);                
            }

            UMCFeatureFinder finder             = new UMCFeatureFinder();
            LCMSFeatureFindingOptions options   = new LCMSFeatureFindingOptions();
            options.AveMassWeight               = .01f;
            options.ConstraintAveMass           = 6;
            options.ConstraintMonoMass          = 6;
            options.FitWeight                   = .1f;
            options.IsIsotopicFitFilterInverted = false;
            options.IsotopicFitFilter           = .15;
            options.IsotopicIntensityFilter     = 0;
            options.LogAbundanceWeight          = .1f;
            options.MaxDistance                 = .1;
            options.MinUMCLength                = 3;
            options.MonoMassWeight              = .01f;
            options.NETWeight                   = .1f;
            options.ScanWeight                  = .01f;
            options.UMCAbundanceReportingType   = AbundanceReportingType.Max;
            options.UseIsotopicFitFilter        = true;
            options.UseIsotopicIntensityFilter  = false;
            options.UseNET                      = true; 
           
            List<UMCLight> umcs = finder.FindFeatures(features, options);            

        }
    }
}
