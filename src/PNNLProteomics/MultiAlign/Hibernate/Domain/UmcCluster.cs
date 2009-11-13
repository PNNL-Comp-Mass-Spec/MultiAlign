/// <file>UmcCluster.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{

    public class UmcCluster
    {

        private int id;
        private double mass;
        private double massCalibrated;
        private double net;
        private double netAligned;
        private ushort scan;
        private ushort charge;
        private ISet<Umc> umcSet;
        private ISet<MassTag> massTagSet;

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public virtual double MassCalibrated
        {
            get { return massCalibrated; }
            set { massCalibrated = value; }
        }

        public virtual double Net
        {
            get { return net; }
            set { net = value; }
        }

        public virtual double NetAligned
        {
            get { return netAligned; }
            set { netAligned = value; }
        }

        public virtual ushort Scan
        {
            get { return scan; }
            set { scan = value; }
        }

        public virtual ushort Charge
        {
            get { return charge; }
            set { charge = value; }
        }

        public virtual ISet<Umc> UmcSet
        {
            get { return umcSet; }
            set { umcSet = value; }
        }

        public virtual ISet<MassTag> MassTagSet
        {
            get { return massTagSet; }
            set { massTagSet = value; }
        }

    }

}
