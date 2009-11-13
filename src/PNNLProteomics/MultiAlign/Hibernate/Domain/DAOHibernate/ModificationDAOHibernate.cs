/// <file>ModificationDAOHibernate.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using NHibernate;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate
{

    public class ModificationDAOHibernate : GenericDAOHibernate<Modification>, IModificationDAO
    {

    }

}
