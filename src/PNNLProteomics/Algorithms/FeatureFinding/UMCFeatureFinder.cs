using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;

namespace PNNLProteomics.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds features using old C++ feature finding algorithm.
    /// </summary>
    public class UMCFeatureFinder: IFeatureFinder 
    {
        #region IFeatureFinder Members
        /// <summary>
        /// Finds features from the path of MS features file.
        /// </summary>
        /// <param name="path">Path to MS Features file.</param>
        /// <returns>List of UMC's.</returns>
        public List<UMCLight> FindFeatures(string path, clsUMCFindingOptions options)
        {
        //    clsUMCCreator creator       = new clsUMCCreator();
        //    clsUMC[] loadedFeatures     = null;
        //    creator.UMCFindingOptions   = options;            
        //    creator.FileName            = path;
        //    creator.LoadUMCs(false);
        //    creator.FindUMCs();
        //    loadedFeatures              = creator.GetUmcs();

            List<UMCLight> features     = new List<UMCLight>();
            //foreach (clsUMC umc in loadedFeatures)
            //{
            //    UMCLight light      = new UMCLight();
            //    light.Abundance     = umc.AbundanceMax;
            //    light.AbundanceSum  = umc.AbundanceSum;
            //    light.ChargeState   = umc.ChargeRepresentative;
            //    light.DriftTime     = umc.DriftTime;
            //    light.UMCCluster    = null;
            //    light.ScanEnd       = umc.ScanEnd;
            //    light.ScanStart     = umc.ScanStart;
            //    light.Score 
            //}
            return features;
        }

        #endregion
    }
}
