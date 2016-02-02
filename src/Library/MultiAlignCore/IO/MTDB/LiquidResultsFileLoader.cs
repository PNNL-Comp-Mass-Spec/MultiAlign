namespace MultiAlignCore.IO.MTDB
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    using Data.MassTags;

    using PNNLOmics.Data.Constants;

    using Constants = InformedProteomics.Backend.Data.Biology.Constants;

    /// <summary>
    /// Loads a Liquid lipd MS/MS search results file as a MultiAlign
    /// <see cref="MassTagDatabase" />.
    /// </summary>
    public class LiquidResultsFileLoader : IMtdbLoader
    {
        /// <summary>
        /// Full file path for Liquid results CSV file.
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// Headers required for parsing the file into <see cref="MassTagLight" />.
        /// </summary>
        public readonly string[] RequiredHeaders =
        {
            "Common Name",
            "Formula",
            "NET",
            "Exact m/z",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LiquidResultsFileLoader"/> class. 
        /// </summary>
        /// <param name="filePath">Full file path for Liquid results CSV file.</param>
        public LiquidResultsFileLoader(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Load a Liquid lipd MS/MS search results file as a MultiAlign
        /// <see cref="MassTagDatabase" />.
        /// </summary>
        /// <returns>The resulting mass tag database.</returns>
        /// <exception cref="FormatException">
        /// Throws a format exception when there are missing headers, or
        /// a non-numeric value was found in a column that was expected to be numeric.
        /// </exception>
        public MassTagDatabase LoadDatabase()
        {
            var headers = new Dictionary<string, int>();

            var massTags = new List<MassTagLight>();

            // Mapping between protein (actually lipid) name, to protein object (currently represents lipids).
            var commonNameToProtein = new Dictionary<string, Protein>();

            int lipidId = 0;

            // Parse lines of file into mass tags.
            int lineCount = 0;
            foreach (var line in File.ReadLines(this.filePath))
            {
                var parts = line.Split('\t');
                if (lineCount++ == 0)
                {   // Header line, store header indices
                    for (int i = 0; i < parts.Length; i++)
                    {
                        headers.Add(parts[i], i);
                    }

                    // Make sure all of the headers we are expecting are actually in the file.
                    this.DetectMissingHeaders(headers);
                    continue;
                }

                var net = Convert.ToDouble(parts[headers["NET"]]);
                var mz = Convert.ToDouble(parts[headers["Exact m/z"]]);
                var mass = mz - Constants.Proton;

                var name = parts[headers["Common Name"]];
                var formula = parts[headers["Formula"]];

                // Add lipid to mapping if it isn't already there
                if (!commonNameToProtein.ContainsKey(name))
                {
                    commonNameToProtein.Add(
                                            name,
                                            new Protein
                    {
                        Id = lipidId++,
                        ChargeState = 1,
                        ChemicalFormula = formula,
                        MassMonoisotopic = mass,
                        Mz = mz,
                        Name = name,
                        Sequence = formula,
                    });
                }

                // Data line, create mass tag
                massTags.Add(
                    new MassTagLight
                {   // TODO: We have lipid data now, we're not doing just proteomics anymore. We should have omic-independent names for this stuff.
                    Id = lineCount - 2, // Subtract 2 because i will be incremented once when it sees the headers, and once before it gets to this line
                    ProteinName = name,
                    PeptideSequence = formula,
                    Net = net,
                    NetAligned = net,
                    NetAverage = net,
                    NetPredicted = net,
                    MassMonoisotopic = mass,
                    MassMonoisotopicAligned = mass,
                    Mz = mz,
                });
            }

            // Mapping from masstags to proteins (actually lipids)
            var massTagToProtein = new Dictionary<int, List<Protein>>();

            // Create mappings.
            foreach (var massTag in massTags)
            {
                var protein = commonNameToProtein[massTag.ProteinName];

                massTagToProtein.Add(massTag.Id, new List<Protein>());
                massTagToProtein[massTag.Id].Add(protein);
            }

            // Finally, create the mass tag database.
            var massTagDatabase = new MassTagDatabase();
            massTagDatabase.AddMassTagsAndProteins(massTags, massTagToProtein);

            return massTagDatabase;
        }

        /// <summary>
        /// Determines if all required headers are present in the file.
        /// </summary>
        /// <param name="headers">The headers that are present in the file.</param>
        /// <exception cref="FormatException">Throws a format exception when there are missing headers.</exception>
        private void DetectMissingHeaders(Dictionary<string, int> headers)
        {
            var missingHeaders = this.RequiredHeaders.Where(header => !headers.Keys.Contains(header)).ToList();
            if (missingHeaders.Any())
            {
                var errorString = missingHeaders.Aggregate(
                                      "Liquid results file is missing expected headers:\n",
                                      (current, missingHeader) => 
                                            current + string.Format("\t{0}\n", missingHeader));

                throw new FormatException(errorString);
            }
        }
    }
}
