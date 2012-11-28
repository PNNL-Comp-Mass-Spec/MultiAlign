using System;
using System.IO;
using System.Collections.Generic;
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
using MultiAlignCore.IO.Features;
using System.ComponentModel;
using System.Windows.Shapes;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using Manassa.Data;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for MsMsViewer.xaml
    /// </summary>
    public partial class MsMsViewer : UserControl
    {
        /// <summary>
        /// Worker for loading MS/MS data
        /// </summary>
        private BackgroundWorker m_background;
        private int m_datasetIdToLoad = 0;

        /// <summary>
        /// providers to use
        /// </summary>
        private FeatureDataAccessProviders m_providers;

        public MsMsViewer()
        {
            InitializeComponent();
            m_providers = null;
            
            Binding binding         = new Binding("SelectedSpectra");
            binding.Source          = m_msmsGrid;
            SetBinding(SelectedSpectraProperty, binding);

        }



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MsMsViewer), new UIPropertyMetadata(Orientation.Vertical));



        public DatasetInformation SelectedDataset
        {
            get { return (DatasetInformation)GetValue(SelectedDatasetProperty); }
            set { SetValue(SelectedDatasetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDataset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDatasetProperty =
            DependencyProperty.Register("SelectedDataset", typeof(DatasetInformation), typeof(MsMsViewer),
            new PropertyMetadata(delegate (DependencyObject sender, DependencyPropertyChangedEventArgs e)
                {
                    var x = sender as MsMsViewer;
                    if (x == null)
                        return;

                    if (x.SelectedDataset == null)
                    {
                        return;
                    }

                    ApplicationStatusMediator.SetStatus(string.Format("Loading MS/MS spectra for {0}", x.SelectedDataset.DatasetName));
                    x.m_datasetIdToLoad = x.SelectedDataset.DatasetId;
                    x.m_providers       = x.Analysis.DataProviders;
                    x.StartLoadingData();
                })
            );



        public void ExtractMsMsData(FeatureDataAccessProviders providers)
        {
            m_providers         = providers;
            m_datasetIdToLoad   = -1;
            StartLoadingData();
        }

        public void StartLoadingData()
        {
            if (m_background != null)
            {
                try
                {
                    // Make sure that we are not doing an operation yet...so we get deadlocked
                    // when we try to report progress and we are 
                    lock (m_providers.Synch)
                    {
                        m_background.CancelAsync();
                    }
                }
                catch (InvalidOperationException)
                {
                    // who cares..?
                }
                finally
                {
                    m_background.Dispose();
                    m_background = null;
                }
            }

            m_msmsGrid.MsMsSpectra.Clear();
            m_background                        = new BackgroundWorker();
            m_background.WorkerReportsProgress  = true;
            m_background.DoWork             += new DoWorkEventHandler(m_background_DoWork);
            m_background.ProgressChanged    += new ProgressChangedEventHandler(m_background_ProgressChanged);
            m_background.RunWorkerAsync();
        }
        public void SetMsMsFeatureSpectra(List<MSFeatureMsMs> features)
        {
            //m_msmsScatterPlot.ClearData();
            m_msmsGrid.MsMsSpectra.Clear();

            foreach (MSFeatureMsMs feature in features)
            {
                m_msmsGrid.MsMsSpectra.Add(feature);
            }
            //m_msmsScatterPlot.AddFeatures(features);
        }

        void m_background_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<MSFeatureMsMs> features = e.UserState as List<MSFeatureMsMs>;
            if (features == null) return;

            Action workAction = delegate
            {
                foreach (MSFeatureMsMs feature in features)
                {
                    m_msmsGrid.MsMsSpectra.Add(feature);
                }
                //m_msmsScatterPlot.AddFeatures(features);
            };

            m_msmsGrid.Dispatcher.BeginInvoke(workAction, System.Windows.Threading.DispatcherPriority.Normal);                       
        }

        void m_background_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<int, List<MSFeatureToMSnFeatureMap>> datasetMap = new Dictionary<int,List<MSFeatureToMSnFeatureMap>>();
            List<MSFeatureToMSnFeatureMap> maps                        = new List<MSFeatureToMSnFeatureMap>();
            lock(m_providers.Synch)
            {
                // Get all of the maps for all spectra
                if (m_datasetIdToLoad < 0)
                {
                    maps = m_providers.MSFeatureToMSnFeatureCache.FindAll();
                }
                else
                {
                    maps = m_providers.MSFeatureToMSnFeatureCache.FindByDatasetId(m_datasetIdToLoad);
                }
            }            

            // Then we map to each dataset, so that we can load one dataset at a time.
            // We break between locks, so that we dont hang access to the data providers...
            foreach (MSFeatureToMSnFeatureMap map in maps)
            {
                bool doesExist = datasetMap.ContainsKey(map.MSDatasetID);
                if (!doesExist)
                {
                    datasetMap.Add(map.MSDatasetID, new List<MSFeatureToMSnFeatureMap>());
                }
                datasetMap[map.MSDatasetID].Add(map);
            }

            // Now we load per dataset.
            foreach (int dataset in datasetMap.Keys)
            {
                List<MSFeatureToMSnFeatureMap> singleMap = datasetMap[dataset];
                List<MSFeatureLight> msFeatures          = new List<MSFeatureLight>();
                List<MSSpectra> spectra                  = new List<MSSpectra>();
                lock (m_providers.Synch)
                {
                    DateTime start = DateTime.Now;
                    // Grab all of the ms features
                    msFeatures = m_providers.MSFeatureCache.FindByDatasetId(dataset);
                    DateTime end = DateTime.Now;
                    TimeSpan span = end.Subtract(start);
                    // Then grab all of the ms/ms spectra
                    spectra = m_providers.MSnFeatureCache.FindByDatasetId(dataset);
                }

                Dictionary<int, MSFeatureLight> msFeatureMap = new Dictionary<int, MSFeatureLight>();
                Dictionary<int, MSSpectra> spectraMap        = new Dictionary<int,MSSpectra>();

                // Make a map so that we can iterate through our table and match the guys up
                spectra.ForEach(x => spectraMap.Add(x.ID, x));
                msFeatures.ForEach(x => msFeatureMap.Add(x.ID, x));

                // Here is where we join the datas structures for the UI elements.
                List<MSFeatureMsMs> msFeatureStructures = new List<MSFeatureMsMs>();
                foreach (MSFeatureToMSnFeatureMap map in singleMap)
                {
                    MSFeatureMsMs msms = new MSFeatureMsMs();                    
                    MSSpectra spectrum          = spectraMap[map.MSMSFeatureID];
                    MSFeatureLight msFeature    = msFeatureMap[map.MSFeatureID];
                                        
                    msms.FeatureID               = msFeature.ID;
                    msms.FeatureGroupID          = msFeature.GroupID;
                    msms.Mz                      = msFeature.Mz;
                    msms.PrecursorMZ             = spectrum.PrecursorMZ;                     
                    msms.FeatureScan             = msFeature.Scan;
                    msms.MsMsScan                = spectrum.Scan;
                    msms.MassMonoisotopicAligned = msFeature.MassMonoisotopicAligned;
                    msms.ChargeState             = msFeature.ChargeState;
                    msms.Sequence                = "";
                    msms.PeptideSequence         = "";

                    msFeatureStructures.Add(msms);
                }
                
                m_background.ReportProgress(0, msFeatureStructures);                
            }
        }

        private static void LoadSpectra(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = sender as MsMsViewer;
            if (thisSender == null)
                return;

            if (e.NewValue != null)
            {
                MSFeatureMsMs item      = e.NewValue as MSFeatureMsMs;
                if (item == null)
                    return;

                DatasetInformation info = null;
                foreach(DatasetInformation datasetInfo in thisSender.Analysis.MetaData.Datasets)
                {
                    if (datasetInfo.DatasetId == item.FeatureGroupID)
                    {
                        info = datasetInfo;
                        break;
                    }
                }

                if (info == null)
                    return;

                if (info.Raw != null)
                {
                    if (info.Raw.Path == null)
                        return;

                    string path = info.Raw.Path;

                    bool doesExist = File.Exists(path);
                    if (!doesExist)
                        return;

                    ISpectraProvider reader = MultiAlignCore.IO.Features.RawLoaderFactory.CreateFileReader(path);
                    reader.AddDataFile(path, item.FeatureGroupID);
                    List<XYData> data = reader.GetRawSpectra(item.MsMsScan, item.FeatureGroupID);

                    thisSender.m_spectraChart.Title = string.Format("Scan {0}  Precursor m/z {1} Precursor Charge {2}",
                        item.MsMsScan,
                        item.PrecursorMZ, 
                        item.ChargeState);
                    thisSender.m_spectraChart.SetSpectra(data);
                    thisSender.m_spectraChart.AutoViewPort();
                }
            }
        }
        public MSFeatureMsMs CurrentSpectra
        {
            get
            {
                return (MSFeatureMsMs)GetValue(SelectedSpectraProperty);
            }
            set
            {
                SetValue(SelectedSpectraProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedSpectraProperty =
            DependencyProperty.Register("CurrentSpectra",
                                        typeof(MSFeatureMsMs),
                                        typeof(MsMsViewer),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(LoadSpectra)));

        /// <summary>
        /// Gets or sets the analysis used.
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get;
            set;
        }
    }
}
