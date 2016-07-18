using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.IO.SequenceData
{
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;

    public interface IIdentificationProvider
    {
        List<Peptide> GetIdentification(int scanNum);

        Dictionary<int, List<Peptide>> GetIdentifications(string filePath);

        Dictionary<int, List<Peptide>> GetAllIdentifications();

        int GroupId { get; set; }
    }
}
