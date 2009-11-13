/// <file>MassTag.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{

    public class MassTag
    {

        private int id;
        private double mass;
        private double netAverage;
        private double netPredicted;
        private double netStandardDeviation;
        private double xCorr;
        private double discriminantMax;
        private double charge1FScore;
        private double charge2FScore;
        private double charge3FScore;
        private ushort cleavageState;
        private ISet<UmcCluster> umcClusterSet;
        private ISet<Peptide> peptideSet;
        private ISet<Modification> modificationSet;
        private ISet<Protein> proteinSet;

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

        public virtual double NetAverage
        {
            get { return netAverage; }
            set { netAverage = value; }
        }

        public virtual double NetPredicted
        {
            get { return netPredicted; }
            set { netPredicted = value; }
        }

        public virtual double NetStandardDeviation
        {
            get { return netStandardDeviation; }
            set { netStandardDeviation = value; }
        }

        public virtual double XCorr
        {
            get { return xCorr; }
            set { xCorr = value; }
        }

        public virtual double DiscriminantMax
        {
            get { return discriminantMax; }
            set { discriminantMax = value; }
        }

        public virtual double Charge1FScore
        {
            get { return charge1FScore; }
            set { charge1FScore = value; }
        }

        public virtual double Charge2FScore
        {
            get { return charge2FScore; }
            set { charge2FScore = value; }
        }

        public virtual double Charge3FScore
        {
            get { return charge3FScore; }
            set { charge3FScore = value; }
        }

        public virtual ushort CleavageState
        {
            get { return cleavageState; }
            set { cleavageState = value; }
        }

        public virtual ISet<UmcCluster> UmcClusterSet
        {
            get { return umcClusterSet; }
            set { umcClusterSet = value; }
        }

        public virtual ISet<Peptide> PeptideSet
        {
            get { return peptideSet; }
            set { peptideSet = value; }
        }

        public virtual ISet<Modification> ModificationSet
        {
            get { return modificationSet; }
            set { modificationSet = value; }
        }

        public virtual ISet<Protein> ProteinSet
        {
            get { return proteinSet; }
            set { proteinSet = value; }
        }

    }

}
