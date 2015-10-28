using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    public class LcmsWarpNetAlignmentFunction
    {
        public LcmsWarpSectionInfo AligneeSections { get; set; }

        public List<LcmsWarpAlignmentMatch> AlignmentMatches { get; set; }

        public double WarpNet(double net)
        {
            var section = this.AligneeSections.GetSectionNumber(net);

            var netStart = this.AlignmentMatches[section].AligneeNetStart;
            var netEnd = this.AlignmentMatches[section].AligneeNetEnd;
            var netStartBaseline = this.AlignmentMatches[section].BaselineNetStart;
            var netEndBaseline = this.AlignmentMatches[section].BaselineNetEnd;

            return ((net - netStart) * (netEndBaseline - netStartBaseline)) / (netEnd - netStart) + netStartBaseline;
        }
    }
}
