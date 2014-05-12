using MultiAlign.IO;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels.TreeView
{
    /// <summary>
    ///     Feature tree view model.
    /// </summary>
    public class UMCTreeViewModel : GenericCollectionTreeViewModel
    {
        private readonly UMCLight m_feature;


        public UMCTreeViewModel(UMCLight feature)
            : this(feature, null)
        {
        }

        public UMCTreeViewModel(UMCLight feature, UMCCollectionTreeViewModel parent)
        {
            m_feature = feature;
            m_parent = parent;

            var information = SingletonDataProviders.GetDatasetInformation(m_feature.GroupId);

            if (information != null)
            {
                Name = information.DatasetName;
            }
            else
            {
                Name = string.Format("Dataset {0}", m_feature.GroupId);
            }

            AddStatistic("Id", m_feature.Id);
            AddStatistic("Dataset Id", m_feature.GroupId);
            AddStatistic("Mass", m_feature.MassMonoisotopicAligned, "N2");
            AddStatistic("NET", m_feature.RetentionTime, "N2");
            if (m_feature.DriftTime > 0)
            {
                AddStatistic("Drift Time", m_feature.DriftTime, "N2");
            }
            AddStatistic("Charge", m_feature.ChargeState);
        }


        public UMCLight Feature
        {
            get { return m_feature; }
        }

        public override bool IsSelected
        {
            get { return base.IsSelected; }
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