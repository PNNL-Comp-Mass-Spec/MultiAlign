using System.Collections.Generic;
using MTDBFramework.IO;
using PNNLOmics.Data;
using PNNLOmics.Data.MassTags;


namespace MultiAlignCore.IO.MTDB
{
    public sealed class MtdbCreatorDatabaseLoader : IMtdbLoader
    {
        private readonly string m_path;

        public MtdbCreatorDatabaseLoader(string path)
        {
            m_path = path;
        }

        public MassTagDatabase LoadDatabase()
        {
            var database = new MassTagDatabase();

            //Implement the loading / reading of the MTDB Framework objects.

            var reader          = new SqLiteTargetDatabaseReader();
            var mtdbDatabase    = reader.ReadDb(m_path);
            var massTags        = new List<MassTagLight>();

            // Mapping objects to create unique proteins and for the mass tag database to load only those proteins for
            // the given consensus targets/mass tags.
            var proteinMap          = new Dictionary<int, Protein>();
            var massTagProteinMap   = new Dictionary<int, List<Protein>>();

            if (mtdbDatabase == null)
                return database;

            foreach (var target in mtdbDatabase.ConsensusTargets)
            {
                // Copy the consensus data into a mass tag light.
                var tag = new MassTagLight
                {
                    Id = target.Id,
                    MassMonoisotopic = target.TheoreticalMonoIsotopicMass,
                    NetAverage = target.AverageNet,
                    NetPredicted = target.PredictedNet,
                    PeptideSequence = target.Sequence,
                    NetStandardDeviation = target.StdevNet
                };


                // Here we create unique proteins for the mass tag copying information from the consensus target proteins.
                var proteinsForMassTag = new List<Protein>();
                foreach (var targetProtein in target.Proteins)
                {
                    if (!proteinMap.ContainsKey(targetProtein.Id))
                    {
                        var newProtein = new Protein
                        {
                            Id = targetProtein.Id,
                            Name = targetProtein.ProteinName,
                            ResidueStartPosition = targetProtein.ResidueStart,
                            ResidueEndPosition = targetProtein.ResidueEnd
                        };
                        //TODO: Do something about the cleavage state and terminus state of a protein loaded from MTDBCreator database.
                        //protein.CleavageState = ??
                        //protein.TerminusState = ??            

                        proteinMap.Add(newProtein.Id, newProtein);
                    }

                    var protein = proteinMap[targetProtein.Id];
                    proteinsForMassTag.Add(protein);
                }

                massTagProteinMap.Add(tag.Id, proteinsForMassTag);
                massTags.Add(tag);
            }

            database.AddMassTagsAndProteins(massTags, massTagProteinMap);
            return database;
        }
    }
}

