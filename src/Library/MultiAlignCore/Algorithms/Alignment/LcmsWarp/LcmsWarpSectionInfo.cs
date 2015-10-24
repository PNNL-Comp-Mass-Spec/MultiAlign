using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Data.Features;

    public class LcmsWarpSectionInfo
    {
        private Dictionary<int, List<UMCLight>> sectionToFeatures; 

        public LcmsWarpSectionInfo(int numSections)
        {
            this.NumSections = numSections;
            this.sectionToFeatures = new Dictionary<int, List<UMCLight>>();
        }

        /// <summary>
        /// Gets the total number of sections.
        /// </summary>
        public int NumSections { get; private set; }

        /// <summary>
        /// Gets the width of the NET for each section.
        /// </summary>
        public double SectionWidth { get; private set; }

        /// <summary>
        /// The lowest NET in the feature set.
        /// </summary>
        public double MinNet { get; private set; }

        /// <summary>
        /// The highest NET in the feature set.
        /// </summary>
        public double MaxNet { get; private set; }

        /// <summary>
        /// Initialize the section info by calculate min, max, and features per section.
        /// </summary>
        /// <param name="features">The feature set to create sections for.</param>
        public void InitSections(List<UMCLight> features)
        {
            // Calculate min/max net for alignee
            this.MinNet = double.PositiveInfinity;
            this.MaxNet = 0;
            foreach (var feature in features)
            {
                this.MinNet = Math.Min(feature.Net, this.MinNet);
                this.MaxNet = Math.Max(feature.Net, this.MaxNet);
            }

            this.SectionWidth = (this.MaxNet - this.MinNet) / this.NumSections;

            // Assign features to sections.
            foreach (var feature in features)
            {
                var sectionNum = this.GetSectionNumber(feature.Net, this.MinNet, this.MaxNet);
                if (!this.sectionToFeatures.ContainsKey(sectionNum))
                {
                    this.sectionToFeatures.Add(sectionNum, new List<UMCLight>());
                }

                this.sectionToFeatures[sectionNum].Add(feature);
            }
        }

        /// <summary>
        /// Get the lowest NET for a given section.
        /// </summary>
        /// <param name="section">The section number.</param>
        /// <returns>The lowest start NET></returns>
        public double GetSectionStartNet(int section)
        {
            return this.MinNet + (section * (this.MaxNet - this.MinNet)) / this.NumSections;
        }

        /// <summary>
        /// Get the highest NET for a given section.
        /// </summary>
        /// <param name="section">The section number.</param>
        /// <returns>The highest start NET></returns>
        public double GetSectionEndNet(int section)
        {
            return this.MinNet + ((section + 1) * (this.MaxNet - this.MinNet)) / this.NumSections;
        }

        /// <summary>
        /// Get the section number that a given NET falls into.
        /// </summary>
        /// <param name="net">The given NET value.</param>
        /// <param name="minNet">The lowest NET in the feature set.</param>
        /// <param name="maxNet">The highest NET in the feature set.</param>
        /// <returns>The section number.</returns>
        private int GetSectionNumber(double net, double minNet, double maxNet)
        {
            var sectionNum = Convert.ToInt32(((net - minNet) * this.NumSections) / (maxNet - minNet));

            if (sectionNum >= NumSections)
            {
                sectionNum = NumSections - 1;
            }
            if (sectionNum < 0)
            {
                sectionNum = 0;
            }

            return sectionNum;
        }
    }
}
