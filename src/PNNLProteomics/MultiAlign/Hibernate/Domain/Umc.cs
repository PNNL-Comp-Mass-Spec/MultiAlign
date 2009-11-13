/// <file>Umc.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{

    public class Umc
    {

        private int id;
        private int datasetId;
        private UmcCluster umcCluster;
        private double mass;
        private double massCalibrated;
        private double net;
        private ushort scan;
        private ushort scanStart;
        private ushort scanEnd;
        private ushort scanAligned;
        private ushort chargeRepresentative;
        private ushort chargeMax;
        private ushort spectralCount;
        private double abundanceMax;
        private double abundanceSum;

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual int DatasetId
        {
            get { return datasetId; }
            set { datasetId = value; }
        }

        public virtual UmcCluster UmcCluster
        {
            get { return umcCluster; }
            set { umcCluster = value; }
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

        public virtual ushort Scan
        {
            get { return scan; }
            set { scan = value; }
        }

        public virtual ushort ScanStart
        {
            get { return scanStart; }
            set { scanStart = value; }
        }

        public virtual ushort ScanEnd
        {
            get { return scanEnd; }
            set { scanEnd = value; }
        }

        public virtual ushort ScanAligned
        {
            get { return scanAligned; }
            set { scanAligned = value; }
        }

        public virtual ushort ChargeRepresentative
        {
            get { return chargeRepresentative; }
            set { chargeRepresentative = value; }
        }

        public virtual ushort ChargeMax
        {
            get { return chargeMax; }
            set { chargeMax = value; }
        }

        public virtual ushort SpectralCount
        {
            get { return spectralCount; }
            set { spectralCount = value; }
        }

        public virtual double AbundanceMax
        {
            get { return abundanceMax; }
            set { abundanceMax = value; }
        }

        public virtual double AbundanceSum
        {
            get { return abundanceSum; }
            set { abundanceSum = value; }
        }

    }

}
