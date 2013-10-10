using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using System.Drawing;

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel : ViewModelBase
    {
        private MultiAlignAnalysis  m_analysis;
        private bool                m_usesDriftTime;
        private bool                m_hasMsMs;
        private bool                m_usesAmt;
        private RectangleF          m_viewport;

        public AnalysisViewModel()
        {
            m_usesDriftTime      = false;
            m_hasMsMs            = false;
            m_usesAmt            = false;

            m_viewport           = new RectangleF(0, 0, 1, 2000); 
        }

        public bool UsesDriftTime
        {
            get { return m_usesDriftTime; }
            set
            {
                if (value != m_usesDriftTime)
                {
                    m_usesDriftTime = value;
                    OnPropertyChanged("UsesDriftTime");
                }
            }
        }

        public bool HasMsMs
        {
            get
            {
                return m_hasMsMs;
            }
            set
            {
                if (value != m_hasMsMs)
                {
                    m_hasMsMs = value;
                    OnPropertyChanged("HasMsMs");
                }
            }
        }


        public bool UsesAmt
        {
            get
            {
                return m_usesAmt;
            }
            set
            {
                if (value != m_usesAmt)
                {
                    m_usesAmt = value;
                    OnPropertyChanged("UsesAmt");
                }
            }
        }

        public RectangleF Viewport
        {
            get
            {
                return m_viewport;
            }
            set
            {
                if (value != m_viewport)
                {
                    m_viewport = value;
                    OnPropertyChanged("Viewport");
                }
            }
        }

    }
}
