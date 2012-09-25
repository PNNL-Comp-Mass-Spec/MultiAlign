using System;
using System.Collections.Generic;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    public interface IMassTagDAO : IGenericDAO<MassTagLight>
    {
        List<MassTagLight> FindMassTags(List<int> ids);
    }

}
