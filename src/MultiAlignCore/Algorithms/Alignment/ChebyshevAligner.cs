using System;
using MultiAlignCore.Data.SequenceData;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using System.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignEngine.Features;


namespace MultiAlignCore.Algorithms.Alignment
{
    class ChebyshevAligner: IFeatureAligner
    {
        #region IFeatureAligner Members

        public Data.Alignment.classAlignmentData AlignFeatures(Data.MassTags.MassTagDatabase database, List<MultiAlignEngine.Features.clsUMC> features, AlignmentOptions options, bool alignDriftTimes)
        {
            throw new NotImplementedException();
        }

        public Data.Alignment.classAlignmentData AlignFeatures(List<MultiAlignEngine.Features.clsUMC> baseline, List<MultiAlignEngine.Features.clsUMC> features, AlignmentOptions options)
        {
            throw new NotImplementedException();
        }

        public Data.Alignment.classAlignmentData AlignFeatures(Data.MassTags.MassTagDatabase massTagDatabase, List<MultiAlignEngine.Features.clsCluster> clusters, AlignmentOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IProgressNotifer Members

        public event EventHandler<PNNLOmics.Algorithms.ProgressNotifierArgs> Progress;

        #endregion


        public void ChebyShev(double[] c, double[] x, ref double func, object obj)
        {
            double sum = 0;
            double t0 = c[0];
            double t1 = c[1] * x[0];
            double prev = t0;
            for (int i = 0; i < c.Length - 1; i++)
            {
                double value = 2 * x[0] * c[i] - prev;
                prev = value;
                sum += value;
            }
            func = sum;
        }

        private List<MSMSCluster> GetMSMSClusters(MultiAlignAnalysis analysis)
        {
            return new List<MSMSCluster>();
        }
                       

        public void Align(MultiAlignAnalysis analysis)
        {            
                                           
            List<MSMSCluster> clusters = GetMSMSClusters(analysis);            
            List<MSMSClusterMap> maps  = new List<MSMSClusterMap>();
            int id = 0;

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            
            List<KeyValuePair<double, double>> values = new List<KeyValuePair<double, double>>();
            for (int i = 0; i < xValues.Count(); i++)
            {
                KeyValuePair<double, double> value = new KeyValuePair<double, double>(xValues.ElementAt(i), yValues.ElementAt(i));
                values.Add(value);
            }
            values = values.OrderBy(i => i.Key).ToList();
            xValues = values.Select(i => i.Key).ToList();
            yValues = values.Select(i => i.Value).ToList();

            List<double> tX = xValues.ToList();
            List<double> tY = yValues.ToList();

            PNNLOmics.Algorithms.Solvers.LevenburgMarquadt warp = new PNNLOmics.Algorithms.Solvers.LevenburgMarquadt();
            warp.BasisFunction = ChebyShev;

            double[] coeffs = new double[20];
            coeffs[1] = 1;
            bool passed = false;
            passed = warp.Solve(tX, tY, ref coeffs);
                    
            // NEED TO DO ALIGNMENT
                            
        }
    }
}
