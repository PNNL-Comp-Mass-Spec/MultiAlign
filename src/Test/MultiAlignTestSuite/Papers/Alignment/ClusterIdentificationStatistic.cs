using System;
using System.Collections.Generic;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public class ClusterIdentificationStatistic
    {
        public ClusterIdentificationStatistic()
        {
            Peptides        = new Dictionary<string, int>();
            PeptideDatasets = new Dictionary<string, List<int>>();
        }

        public Dictionary<string, int> Peptides { get; set; }
        public Dictionary<string, List<int>> PeptideDatasets { get; set; }

        public int TotalDatasetsObserved
        {
            get
            {
                var total   = 0;                
                foreach (var peptide in PeptideDatasets.Keys)
                {
                    var counter  = new Dictionary<int, int>();
                    var subTotal = 0;
                    foreach (var dataset in PeptideDatasets[peptide])
                    {
                        if (!counter.ContainsKey(dataset))
                        {
                            counter.Add(dataset, 0);
                            subTotal++;
                        }
                        counter[dataset]++;
                    }

                    total = Math.Max(total, subTotal);
                }

                return total;
            }
        }
    }
}