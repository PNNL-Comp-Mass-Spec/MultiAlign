using System;
using System.Collections.Generic;
using PNNLOmics.Data;
namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
	public interface IProteinDAO : IGenericDAO<Protein>
    {
		ICollection<Protein> FindByProteinString(string proteinString);
    }
}
