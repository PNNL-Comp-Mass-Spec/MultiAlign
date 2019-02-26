using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Data.Features;
using GalaSoft.MvvmLight;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using OxyPlot;
using OxyPlot.Series;

namespace MultiAlignRogue.ViewModels
{
    public class UMCLightViewModel : ViewModelBase, IScatterPointProvider
    {
        private bool selected;

        private DatasetInformation datasetInformation;

        public UMCLightViewModel(UMCLight umcLight)
        {
            this.UMCLight = umcLight;
            this.selected = true;
        }

        public UMCLight UMCLight { get; private set; }

        public bool Selected
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    this.RaisePropertyChanged("Selected", !value, value, true);
                }
            }
        }

        public DatasetInformation DatasetInformation
        {
            get
            {
                if (this.datasetInformation == null && this.UMCLight != null)
                {
                    this.datasetInformation = SingletonDataProviders.GetDatasetInformation(this.UMCLight.GroupId);
                }

                return this.datasetInformation;
            }
        }

        public ScatterPoint GetScatterPoint()
        {
            return new ScatterPoint(this.UMCLight.NetAligned, this.UMCLight.MassMonoisotopicAligned, 3.0);
        }
    }
}
