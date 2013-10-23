using MultiAlign.IO;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels
{

    /// <summary>
    /// Feature tree view model.
    /// </summary>
    public class UMCViewModel : TreeItemViewModel
    {
        private UMCLight m_feature;
        

        public UMCViewModel(UMCLight feature)
            : this(feature, null)
        {
        }


        public UMCViewModel(UMCLight feature, UMCCollectionTreeViewModel parent)
        {
            m_feature = feature;
            m_parent  = parent;            

            LoadChildren();

            foreach (MSFeatureLight msFeature in feature.Features)
            {
                msFeature.ReconstructMSFeature(SingletonDataProviders.Providers);

                foreach(MSSpectra spectrum in msFeature.MSnSpectra)                
                {
                    
                }
            }
        }


        public string Id
        {
            get
            {
                return m_feature.ID.ToString();
            }
        }

        public string Mass
        {
            get
            {
                return m_feature.MassMonoisotopic.ToString();
            }
        }
        public string Net
        {
            get
            {
                return m_feature.NET.ToString();
            }
        }
        public string ChargeState
        {
            get
            {
                return m_feature.ChargeState.ToString();
            }
        }

        public int MsMsCount
        {
            get
            {
                int count = 0;
                m_feature.MSFeatures.ForEach(x => count += x.MSnSpectra.Count);

                return count;
            }
        }

        public override void LoadChildren()
        {
            m_feature.ReconstructUMC(SingletonDataProviders.Providers, false);

          
        }
    }
}
