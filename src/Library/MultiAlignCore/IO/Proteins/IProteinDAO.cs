#region

using System.Collections.Generic;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.IO.Features
{
    public interface IProteinDAO : IGenericDAO<Protein>
    {
        ICollection<Protein> FindByProteinString(string proteinString);
    }
}