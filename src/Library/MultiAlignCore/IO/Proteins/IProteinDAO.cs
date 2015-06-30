#region

using System.Collections.Generic;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.IO.Proteins
{
    public interface IProteinDAO : IGenericDAO<Protein>
    {
        ICollection<Protein> FindByProteinString(string proteinString);
    }
}