using System;
using System.Collections.Generic;
using PNNLOmics.Data;
namespace MultiAlignCore.IO.Features
{
	public interface IProteinDAO : IGenericDAO<Protein>
    {
		ICollection<Protein> FindByProteinString(string proteinString);
    }
}
