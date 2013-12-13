using MultiAlign.IO;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.TreeView
{

    /// <summary>
    /// Feature tree view model.
    /// </summary>
    public class UMCTreeViewModel : GenericCollectionTreeViewModel
    {
        private UMCLight m_feature;
        

        public UMCTreeViewModel(UMCLight feature)
            : this(feature, null)
        {
        }

        public UMCTreeViewModel(UMCLight feature, UMCCollectionTreeViewModel parent):
            base()
        {
            m_feature   = feature;
            m_parent    = parent;

            DatasetInformation information = SingletonDataProviders.GetDatasetInformation(m_feature.GroupID);

            if (information != null)
            {
                Name = information.DatasetName;
            }else
            {
                Name = string.Format("Dataset {0}", m_feature.GroupID);
            }

            AddStatistic("Id",          m_feature.ID);
            AddStatistic("Dataset Id",  m_feature.GroupID);
            AddStatistic("Mass",        m_feature.MassMonoisotopicAligned);
            AddStatistic("NET",         m_feature.RetentionTime);
            if (m_feature.DriftTime > 0)
            {
                AddStatistic("Drift Time", m_feature.DriftTime);
            }
            AddStatistic("Charge",      m_feature.ChargeState);            
        }


        public UMCLight Feature
        {
            get
            {
                return m_feature;
            }
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                base.IsSelected = value;
                if (m_feature != null)
                {                   
                    OnFeatureSelected(m_feature);
                }
            }
        }

        public override void LoadChildren()
        {        
        }
    }
}
