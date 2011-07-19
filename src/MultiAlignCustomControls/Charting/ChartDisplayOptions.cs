
namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Encapsulates options for displaying chart monikers for thumbnail views of analysis plots.
    /// </summary>
    public class ChartDisplayOptions
    {
        private bool m_displayLegend;
        private bool m_displayAxis;
        private bool m_displayTitle;
        private bool m_displayGridlines;
        private int m_marginMin;
        private int m_marginMax;
        private string m_xAxisLabel;
        private string m_yAxisLabel;
        private string m_title;
        private int m_width;
        private int m_height;

        /// <summary>
        /// 
        /// </summary>
        public ChartDisplayOptions()
        {
            DisplayLegend = false;
            DisplayTitle = false;
            DisplayGridLines = false;
            DisplayAxis = false;
            MarginMax = 1;
            MarginMin = 1;
            XAxisLabel = "";
            YAxisLabel = "";
            Title = "";
            Height = 64;
            Width = 64;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayLegend"></param>
        /// <param name="displayAxis"></param>
        /// <param name="displayTitle"></param>
        /// <param name="displayGridlines"></param>
        public ChartDisplayOptions(bool displayLegend,
                                       bool displayAxis,
                                       bool displayTitle,
                                       bool displayGridlines)
        {

            DisplayLegend = displayLegend;
            DisplayTitle = displayAxis;
            DisplayGridLines = displayTitle;
            DisplayAxis = displayGridlines;
            MarginMax = 1;
            MarginMin = 1;
            XAxisLabel = "";
            YAxisLabel = "";
            Title = "";
            Height = 64;
            Width = 64;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayLegend"></param>
        /// <param name="displayAxis"></param>
        /// <param name="displayTitle"></param>
        /// <param name="displayGridlines"></param>
        /// <param name="marginMin"></param>
        /// <param name="marginMax"></param>
        /// <param name="title"></param>
        /// <param name="xAxisLabel"></param>
        /// <param name="yAxisLabel"></param>
        public ChartDisplayOptions(bool displayLegend,
                                        bool displayAxis,
                                        bool displayTitle,
                                        bool displayGridlines,
                                        int marginMin,
                                        int marginMax,
                                        string title,
                                        string xAxisLabel,
                                        string yAxisLabel,
                                        int width,
                                        int height)
        {

            DisplayLegend = displayLegend;
            DisplayTitle = displayAxis;
            DisplayGridLines = displayTitle;
            DisplayAxis = displayGridlines;
            MarginMax = marginMax;
            MarginMin = marginMin;

            XAxisLabel = "";
            YAxisLabel = "";
            Title = "";
            Height = height;
            Width = width;
        }
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public int MarginMin
        {
            get
            {
                return m_marginMin;
            }
            set
            {
                m_marginMin = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MarginMax
        {
            get
            {
                return m_marginMax;
            }
            set
            {
                m_marginMax = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>     
        public bool DisplayLegend
        {
            get
            {
                return m_displayLegend;
            }
            set
            {
                m_displayLegend = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool DisplayAxis
        {
            get
            {
                return m_displayAxis;
            }
            set
            {
                m_displayAxis = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool DisplayTitle
        {
            get
            {
                return m_displayTitle;
            }
            set
            {
                m_displayTitle = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool DisplayGridLines
        {
            get
            {
                return m_displayGridlines;
            }
            set
            {
                m_displayGridlines = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string XAxisLabel
        {
            get
            {
                return m_xAxisLabel;
            }
            set
            {
                m_xAxisLabel = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string YAxisLabel
        {
            get
            {
                return m_yAxisLabel;
            }
            set
            {
                m_yAxisLabel = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        #endregion
    }
}
