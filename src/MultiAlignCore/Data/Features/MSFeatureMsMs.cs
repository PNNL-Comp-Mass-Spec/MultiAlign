using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;

namespace MultiAlignCore.Data.Features
{
    public class MSFeatureMsMs
    {
        public MSSpectra Spectra
        {
            get;
            set;
        }
        public MSFeatureLight Feature
        {
            get;
            set;
        }
        public Peptide Peptide
        {
            get;
            set;
        }
        public MassTag MassTag
        {
            get;
            set;
        }
        public int ClusterId
        {
            get;
            set;
        }
    }
}
