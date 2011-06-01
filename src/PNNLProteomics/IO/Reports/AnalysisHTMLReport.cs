using System;
using System.Collections.Generic;
using System.IO;

namespace PNNLProteomics.IO.Reports
{
    /// <summary>
    /// Class that holds HTML tags and generates an analysis report in HTML format.
    /// </summary>
    public class AnalysisHTMLReport
    {
        #region HTML
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public  void PushImageColumn(string data)
        {
            PushStartTableColumn();
            PushData(string.Format("<a href={0}><img src={0} width={1} height={2} alt={0}/></a>", data, ImageWidth, ImageHeight));
            PushEndTableColumn();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public  void PushTextHeader(string data)
        {
            ContentTags.Add("<a href=\"#top\"><H2>" + data + "</H2></a>");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public  void PushData(string tag)
        {
            ContentTags.Add(tag);
        }
        /// <summary>
        /// Starts a table.
        /// </summary>
        public  void PushStartTable()
        {
            PushStartTable(false);
        }
        /// <summary>
        /// Starts a table with/without border.
        /// </summary>
        /// <param name="border">True for a border.  False for not.</param>
        public  void PushStartTable(bool border)
        {
            if (border)
            {
                ContentTags.Add("<table border = 1>");
            }
            else
            {
                ContentTags.Add("<table>");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushEndTable()
        {
            ContentTags.Add("</table>");
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushStartTableRow()
        {
            ContentTags.Add("<tr>");
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushEndTableRow()
        {
            ContentTags.Add("</tr>");
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushStartTableColumn()
        {
            ContentTags.Add("<td>");
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushEndTableColumn()
        {
            ContentTags.Add("</td>");
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushHeader()
        {
            ContentTags.Add("<html>");
            if (AnalysisName != null)
            {
                ContentTags.Add("<title>Analysis Name: " + AnalysisName + "</title>");
                ContentTags.Add("<h1>Analysis Name: " + AnalysisName + "</h1>");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public  void PushEndHeader()
        {
            ContentTags.Add("</html>");
        }
        #endregion


        /// <summary>
        /// Creates the HTML output file.
        /// </summary>
        public void CreateReport(string path)
        {

            ContentTags.Insert(0, "<a name=\"top\">MultiAlign Analysis Report</a>" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));

            if (!path.EndsWith(".html"))
            {
                path = path + ".html";
            }
            using (TextWriter htmlWriter = File.CreateText(path))
            {
                foreach (string tag in ContentTags)
                {
                    htmlWriter.WriteLine(tag);
                }
            }
        }

        #region Properties
        /// <summary>
        /// Gets the HTML page strings.
        /// </summary>
        public List<string> ContentTags
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the Image Width
        /// </summary>
        public int ImageWidth
        {
            get; 
            set; 
        }
        /// <summary>
        /// Gets or sets the Image Height
        /// </summary>
        public int ImageHeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the analysis to write.
        /// </summary>
        public string AnalysisName
        {
            get;
            set;
        }
        #endregion
    }
}
