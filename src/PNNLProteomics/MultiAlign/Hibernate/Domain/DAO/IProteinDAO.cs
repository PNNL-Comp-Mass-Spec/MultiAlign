using System;
using System.Collections.Generic;
using MultiAlignEngine.MassTags;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
	public interface IProteinDAO : IGenericDAO<clsProtein>
    {
		ICollection<clsProtein> FindByProteinString(string proteinString);
    }
}
