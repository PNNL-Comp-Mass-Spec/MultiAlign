using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.Collections.ObjectModel;
using MultiAlignCustomControls.Charting;
using System.Windows.Forms.Integration;
using MultiAlign.IO;
using System.Drawing;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels
{

    public class MSSpectraViewModel : ViewModelBase
    {
        private MSSpectra m_spectra;
        private PeptideViewModel m_peptide;
        /// <summary>
        /// 
        /// </summary>
        WindowsFormsHost m_host;
        /// <summary>
        /// 
        /// </summary>
        SpectraChart m_chart;

        public MSSpectraViewModel(MSSpectra spectrum)
        {
            Peptides = new ObservableCollection<PeptideViewModel>();
            m_spectra = spectrum;
            m_spectra.Peptides.ForEach(x => Peptides.Add(new PeptideViewModel(x)));


            m_chart             = new SpectraChart();
            m_chart.Title       = "MS/MS";
            m_chart.XAxisLabel  = "m/z";
            m_chart.YAxisLabel  = "Intensity";
            m_chart.DrawSticks  = true;
            m_chart.AxisVisible = false;
            SpectraChart        = new WindowsFormsHost() { Child = m_chart };

            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(spectrum.GroupID);
            if (info != null)
            {
                Dataset = new DatasetViewModel(info);
            }

            UpdateSpectra(spectrum);
        }

        public ObservableCollection<PeptideViewModel> Peptides { get; set; }

        public WindowsFormsHost SpectraChart
        {
            get
            {
                return m_host;
            }
            set
            {
                if (value != null && m_host != value)
                {
                    m_host = value;
                    OnPropertyChanged("SpectraChart");
                }
            }

        }

        public MSSpectra Spectrum { get { return m_spectra; } }

        public DatasetViewModel Dataset { get; set; }

        private void UpdateSpectra(MSSpectra spectrum)
        {
            if (m_chart != null && spectrum != null)
            {
                
                if (spectrum.Peaks.Count < 1)
                {
                    spectrum.Peaks = SpectraLoader.LoadSpectrum(spectrum);
                }

                m_chart.SetSpectra(spectrum.Peaks);
                m_chart.Title = string.Format("scan {0} @ {1} m/z ", spectrum.Scan,
                                                                        spectrum.PrecursorMZ);
                m_chart.AutoViewPort();
                RectangleF viewport = m_chart.ViewPort;
                m_chart.ViewPort = new System.Drawing.RectangleF(0, viewport.Y, 2000, viewport.Height);
            }
        }
    }
}
