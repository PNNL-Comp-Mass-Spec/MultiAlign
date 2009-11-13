/// <file>Protein.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{

    public class Protein
    {

        private int id;
        private string proteinString;
        private int refId;
        private ISet<MassTag> massTagSet;

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string ProteinString
        {
            get { return proteinString; }
            set { proteinString = value; }
        }

        public virtual int RefId
        {
            get { return refId; }
            set { refId = value; }
        }

        public virtual ISet<MassTag> MassTagSet
        {
            get { return massTagSet; }
            set { massTagSet = value; }
        }

    }

}
