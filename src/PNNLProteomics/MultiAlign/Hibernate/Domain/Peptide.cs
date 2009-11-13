/// <file>Peptide.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{

    public class Peptide
    {

        private int id;
        private string peptideString;
        private MassTag massTag;

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string PeptideString
        {
            get { return peptideString; }
            set { peptideString = value; }
        }

        public virtual MassTag MassTag
        {
            get { return massTag; }
            set { massTag = value; }
        }

    }

}
