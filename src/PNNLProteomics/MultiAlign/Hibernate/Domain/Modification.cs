/// <file>Modification.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{

    public class Modification
    {

        private int id;
        private string description;
        private MassTag massTag;

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        public virtual MassTag MassTag
        {
            get { return massTag; }
            set { massTag = value; }
        }

    }

}
