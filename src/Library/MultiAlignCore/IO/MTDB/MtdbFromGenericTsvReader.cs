namespace MultiAlignCore.IO.MTDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using System.IO;

    using InformedProteomics.Backend.Data.Biology;

    using MultiAlignCore.Data.MassTags;

    /// <summary>
    /// This class is is a reader for 
    /// </summary>
    public class MtdbFromGenericTsvReader : IMtdbLoader
    {
        /// <summary>
        /// Full file path for Liquid results CSV file.
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// The column separator to use.
        /// </summary>
        private readonly char delimeter;

        /// <summary>
        /// Headers required for parsing the file into <see cref="MassTagLight" />.
        /// </summary>
        public static readonly string[] RequiredHeaders =
        {
            "Name",         // The name of the protein
            "Sequence",     // The protein/peptide sequence or equivalent
            "NET",          // The normalized elution time
            "Mass",         // Monoisotopic Mass
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LiquidResultsFileLoader"/> class. 
        /// </summary>
        /// <param name="filePath">Full file path for the TSV file.</param>
        public MtdbFromGenericTsvReader(string filePath)
        {
            this.filePath = filePath;

            var ext = Path.GetExtension(filePath);
            if (ext != ".tsv" && ext != ".csv")
            {
                throw new FormatException(string.Format("Invalid file type: {0}.", ext));
            }
            this.delimeter = ext == ".tsv" ? '\t' : ',';
        }

        /// <summary>
        /// Load a generic TSV file as a MultiAlign
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
                var parts = line.Split(this.delimeter);
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
                var mz = Convert.ToDouble(parts[headers["Mass"]]);
                var mass = mz - Constants.Proton;

                var name = parts[headers["Name"]];
                var formula = parts[headers["Sequence"]];

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
                {
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

            // Mapping from masstags to proteins
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
            var missingHeaders = RequiredHeaders.Where(header => !headers.Keys.Contains(header)).ToList();
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
