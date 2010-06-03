using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Collections.Generic;

using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.Analysis;
using MultiAlignEngine.PeakMatching;

using MultiAlignWin.Forms.Filters;
using PNNLProteomics.Filters;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain;

namespace MultiAlignWin.IO
{
    public class AnalysisSQLiteDBWriter: IAnalysisWriter
    {
        /// <summary>
        /// List of UMC filters for screening data.
        /// </summary>
        private List<IFilter<clsUMC>> mlist_umcFilters;
        /// <summary>
        /// List of filters related to UMC Clusters
        /// </summary>
        private List<IFilter<clsCluster>> mlist_clusterFilters;

        public AnalysisSQLiteDBWriter()
        {
            Clear();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the list of UMC Filters
        /// </summary>
        public List<IFilter<clsCluster>> FiltersClusters
        {
            get
            {
                return mlist_clusterFilters;
            }
            set
            {
                mlist_clusterFilters = value;
            }
        }
        /// <summary>
        /// Gets or sets the list of UMC Filters
        /// </summary>
        public List<IFilter<clsUMC>> FiltersUMC
        {
            get
            {
                return mlist_umcFilters;
            }
            set
            {
                mlist_umcFilters = value;
            }
        }
        #endregion

        public void Clear()
        {
            /// 
            /// Create the filter lists
            /// 
            if (mlist_umcFilters == null)
                mlist_umcFilters = new List<IFilter<clsUMC>>();
            mlist_umcFilters.Clear();

            if (mlist_clusterFilters == null)
                mlist_clusterFilters = new List<IFilter<clsCluster>>();
            mlist_clusterFilters.Clear();
        }
        public void WriteAnalysis(string dbPath, clsMultiAlignAnalysis analysis)
        {

            // Set DB Location
            NHibernateUtil.SetDbLocationForWrite(dbPath);

            // Setup DAOHibernates
            UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
            UmcClusterDAOHibernate umcClusterDAOHibernate = new UmcClusterDAOHibernate();
            MassTagDAOHibernate massTagDAOHibernate = new MassTagDAOHibernate();
            ProteinDAOHibernate proteinDAOHibernate = new ProteinDAOHibernate();
            GenericDAOHibernate<ClusterToMassTagMap> clusterToMassTagMapDAOHibernate = new GenericDAOHibernate<ClusterToMassTagMap>();
            GenericDAOHibernate<MassTagToProteinMap> massTagToProteinMapDAOHibernate = new GenericDAOHibernate<MassTagToProteinMap>();

            // Lists to hold the objects to be saved to SQLite
            List<clsUMC> umcList = new List<clsUMC>();
            List<clsCluster> clusterList = new List<clsCluster>();
            List<clsMassTag> massTagList = new List<clsMassTag>();
            List<clsProtein> proteinList = new List<clsProtein>();
            List<ClusterToMassTagMap> clusterToMassTagMapList = new List<ClusterToMassTagMap>();
            List<MassTagToProteinMap> massTagToProteinMapList = new List<MassTagToProteinMap>();

            // Grab all of the data from the analysis
            clsUMCData umcData = analysis.UMCData;
            clsClusterData clusterData = umcData.mobjClusterData;
            ArrayList clusterArrayList = umcData.mobjClusterData.marrClusters;
            clsMassTag[] massTagArray = analysis.PeakMatchingResults.marrMasstags;
            clsProtein[] proteinArray = analysis.PeakMatchingResults.marrProteins;

            // Used to store which Cluster IDs are saved to SQLite
            List<int> clusterIDs = new List<int>();

            // Populate List of UMC Clusters
            for (int i = 0; i < clusterData.NumClusters; i++)
            {
                clsCluster cluster = clusterData.GetCluster(i);

                // If the Cluster does not pass the filters, go to the next Cluster
                if (!FilterUtil<clsCluster>.PassesFilters(cluster, mlist_clusterFilters))
                {
                    continue;
                }

                clusterIDs.Add(i);
                clusterList.Add(cluster);
            }

            // Populate List of UMCs
            for (int i = 0; i < umcData.NumDatasets; i++)
            {
                clsUMC[] umcArray = umcData.GetUMCS(i);

                foreach (clsUMC umc in umcArray)
                {
                    // If the UMC does not pass the filters, go to the next UMC
                    if (!FilterUtil<clsUMC>.PassesFilters(umc, mlist_umcFilters))
                    {
                        continue;
                    }

                    umcList.Add(umc);
                }
            }

            // Populate Mass Tags, Proteins, and Mapping Lists
            if (analysis.PeakMatchedToMassTagDB)
            {
                foreach (clsPeakMatchingResults.clsPeakMatchingTriplet triplet in analysis.PeakMatchingResults.marrPeakMatchingTriplet)
                {
                    if (clusterIDs.Contains(triplet.mintFeatureIndex))
                    {
                        clsMassTag massTag = massTagArray[triplet.mintMassTagIndex];
                        clsProtein protein = proteinArray[triplet.mintProteinIndex];

                        ClusterToMassTagMap clusterToMassTagMap = new ClusterToMassTagMap(triplet.mintFeatureIndex, massTag.Id);
                        MassTagToProteinMap massTagToProteinMap = new MassTagToProteinMap(massTag.Id, protein.Id);

                        if (!clusterToMassTagMapList.Contains(clusterToMassTagMap))
                        {
                            clusterToMassTagMapList.Add(clusterToMassTagMap);
                        }

                        if (!massTagToProteinMapList.Contains(massTagToProteinMap))
                        {
                            massTagToProteinMapList.Add(massTagToProteinMap);
                        }

                        if (!massTagList.Contains(massTag))
                        {
                            massTagList.Add(massTag);
                        }

                        if (!proteinList.Contains(protein))
                        {
                            proteinList.Add(protein);
                        }
                    }
                }
            }

            // Save the Lists to SQLite
            umcDAOHibernate.AddAll(umcList);
            umcClusterDAOHibernate.AddAll(clusterList);
            massTagDAOHibernate.AddAll(massTagList);
            proteinDAOHibernate.AddAll(proteinList);
            clusterToMassTagMapDAOHibernate.AddAll(clusterToMassTagMapList);
            massTagToProteinMapDAOHibernate.AddAll(massTagToProteinMapList);
        }
    }
}
