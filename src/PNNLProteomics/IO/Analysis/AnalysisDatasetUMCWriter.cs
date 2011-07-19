//using System;
//using System.IO;
//using System.Collections.Generic;

//using MultiAlignEngine.Features;
//using MultiAlignEngine.MassTags;
//using MultiAlignCore.Data;
//using MultiAlignEngine.PeakMatching;

//using PNNLProteomics.SMART;
//using MultiAlignCore.Filters;
//using MultiAlignCore.MultiAlign.Hibernate;
//using MultiAlignCore.IO.Features.Hibernate;
//using MultiAlignCore.MultiAlign.Hibernate.Domain;


//namespace MultiAlignCore.IO
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class AnalysisDatasetUMCWriter: IAnalysisWriter 
//    {

//        #region Members and constants
//        /// <summary>
//        /// Default delimeter
//        /// </summary>
//        public const string CONST_DELIMITER = ",";

//        private const string CONST_INDEX            = "ID";
//        private const string CONST_CHARGE           = "Charge";
//        private const string CONST_MZ               = "MZ";
//        private const string CONST_MASS             = "Mass";
//        private const string CONST_MASS_ALIGNED     = "MassAligned";
//        private const string CONST_SCAN             = "Scan";
//        private const string CONST_SCAN_ALIGNED     = "ScanAligned";
//        private const string CONST_NET              = "NET";
//        private const string CONST_NET_ALIGNED      = "NETAligned";
        
//        /// <summary>
//        /// Delimeter used between columns.
//        /// </summary>
//        private string mstring_delimeter;
//        #endregion

//        public AnalysisDatasetUMCWriter()
//        {
//            mstring_delimeter = CONST_DELIMITER;
//        }

//        #region Properties
//        /// <summary>
//        /// Gets or sets the delimeter to use between data columns.
//        /// </summary>
//        public string Delimeter
//        {
//            get
//            {
//                return mstring_delimeter;
//            }
//            set
//            {
//                mstring_delimeter = value;
//            }
//        }
//        #endregion


//        #region Methods
//        /// <summary>
//        /// Resets the options to their default values. 
//        /// </summary>
//        public void Clear()
//        {
//            /// 
//            /// Delimeter for text output
//            /// 
//            Delimeter = CONST_DELIMITER;

//        }/// <summary>
//        /// Writes the header information to the file.
//        /// </summary>
//        /// <param name="writer"></param>
//        private void WriteHeader(TextWriter writer,
//                                 MultiAlignAnalysis analysis)
//        {
//            string data = CONST_INDEX;
//            data += Delimeter + CONST_CHARGE;
//            data += Delimeter + CONST_MZ;
//            data += Delimeter + CONST_MASS;
//            data += Delimeter + CONST_MASS_ALIGNED;            
//            data += Delimeter + CONST_SCAN;            
//            data += Delimeter + CONST_SCAN_ALIGNED;
//            data += Delimeter + CONST_NET;         
//            writer.Write(data);            
//            writer.Write("\n");
//        }
//        /// <summary>
//        /// Writes the analysis object to the file provided.
//        /// </summary>
//        /// <param name="analysis">Analysis object to write.</param>
//        /// <param name="pathname">File path to write to.</param>
//        public void WriteAnalysis(string pathname,
//                                    MultiAlignAnalysis analysis,
//                                    List<IFilter<clsUMC>> umcFilters,
//                                    List<IFilter<clsCluster>> clusterFilters)
//        {
//            for (int i = 0; i < analysis.Datasets.Count; i++)
//            {
//                string newPathname = Path.GetDirectoryName(pathname);
//                newPathname += Path.GetFileNameWithoutExtension(analysis.Datasets[i].mstrLocalPath) + Path.GetExtension(pathname);

//                using (TextWriter writer = File.CreateText(newPathname))
//                {
//                    WriteHeader(writer, analysis);
//                    clsUMC[] umcs = analysis.UMCData.GetUMCS(i);
//                    foreach (clsUMC umc in umcs)
//                    {
//                        if (FilterUtil<clsUMC>.PassesFilters(umc, umcFilters))
//                        {
//                            string data = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
//                                                Delimeter,
//                                                umc.Id,
//                                                umc.ChargeRepresentative,
//                                                umc.MZForCharge,
//                                                umc.Mass,
//                                                umc.MassCalibrated,
//                                                umc.Scan,
//                                                umc.ScanAligned,
//                                                umc.Net);
//                            writer.WriteLine(data);
//                        }
//                    }
                    
//                }
//            }
//        }
//        #endregion
//    }
//}
