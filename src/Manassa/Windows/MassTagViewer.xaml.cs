using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data;
using MultiAlignCustomControls.Charting;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.MassTags;
using PNNLOmics.Data.Features;
using MultiAlignCore.IO.Features;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for MassTagControl.xaml
    /// </summary>
    public partial class MassTagViewer : UserControl
    {
        private MassTagDatabase m_database;
        private bool m_areTagsMatched;

        public MassTagViewer()
        {
            InitializeComponent();
            m_areTagsMatched = false;
        }

        public MassTagDatabase Database
        {
            get
            {
                return m_database; 
            }
            set
            {
                m_database = value;

                if (value != null)
                {                    
                    m_proteinGrid.Proteins      = value.AllProteins;
                    m_matchedProteins.Proteins  = value.MatchedProteins;                    
                    m_massTagPlot.AddMassTags(value.MassTags);
                    m_massTagPlot.AutoViewPort();
                }                
            }
        }
        public bool AreTagsMatched
        {
            get
            {
                return m_areTagsMatched;
            }
            set
            {
                m_areTagsMatched = value;
            }
        }



        public ObservableCollection<MassTagToCluster> MatchedTags
        {
            get { return (ObservableCollection<MassTagToCluster>)GetValue(MatchedTagsProperty); }
            set { SetValue(MatchedTagsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MatchedTags.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MatchedTagsProperty =
            DependencyProperty.Register("MatchedTags", typeof(ObservableCollection<MassTagToCluster>), typeof(MassTagViewer),
            new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    MassTagViewer viewer = sender as MassTagViewer;
                    if (viewer != null)
                    {
                        viewer.SetTags();
                    }                    
                }));


        private void SetTags()
        {
            if (MatchedTags != null || MatchedTags.Count < 1)
            {
                m_massTagGrid.MassTags          = MatchedTags.ToList();
                Dictionary<int, int> massTagMap = MatchedTags.ToList().CreateMassTagMatchedClusterSizeHistogram();

                m_massTagHistogram.ConstructHistogram(massTagMap);
                m_massTagHistogram.AutoViewPort();
            }
        }
        
        /// <summary>
        /// Gets or sets the feature data access providers for retrieving extra data for display.
        /// </summary>
        public FeatureDataAccessProviders Providers
        {
            get
            {
                return m_massTagDisplay.Providers;
            }
            set
            {
                m_massTagDisplay.Providers = value;
            }
        }
    }
}
