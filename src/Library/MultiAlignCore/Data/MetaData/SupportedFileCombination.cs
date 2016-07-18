using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Data.MetaData
{
    using MultiAlignCore.IO.InputFiles;

    public class SupportedFileCombination
    {
        public SupportedFileCombination(SupportedFileType baseType)
        {
            this.BaseType = baseType;
            this.RequiredTypes = new List<InputFileType>();
            this.AtLeastOneOf = new List<InputFileType>();
            this.OptionalTypes = new List<InputFileType>();
        }

        /// <summary>
        /// Gets or sets the dataset that this is associated with.
        /// </summary>
        public DatasetLoader.SupportedDatasetTypes DatasetType { get; set; }

        /// <summary>
        /// Gets the type of dataset that for this dataset combination.
        /// </summary>
        public SupportedFileType BaseType { get; private set; }

        /// <summary>
        /// Gets the list of required file types for this combination.
        /// (All required)
        /// </summary>
        public List<InputFileType> RequiredTypes { get; private set; }

        /// <summary>
        /// Gets the list of file types that we need at least one of.
        /// (1 or more required)
        /// </summary>
        public List<InputFileType> AtLeastOneOf { get; private set; }

        /// <summary>
        /// Gets the list of file types that are completely optional.
        /// (0 or more required)
        /// </summary>
        public List<InputFileType> OptionalTypes { get; private set; }

        /// <summary>
        /// Gets a message that lists the required files for this combination.
        /// </summary>
        public string RequiredMessage()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Files required for a {0} dataset:", this.BaseType.Name);
            builder.AppendLine();

            // Add required types
            if (this.RequiredTypes.Any())
            {
                builder.Append("Required: ");
                foreach (var fileType in this.RequiredTypes)
                {
                    builder.AppendFormat("{0} file; ", fileType);
                }
                builder.AppendLine();
            }

            // Add AtLeastOneof types
            if (this.AtLeastOneOf.Any())
            {
                builder.Append("At least one of the following: ");
                foreach (var fileType in this.AtLeastOneOf)
                {
                    builder.AppendFormat("{0} file; ", fileType);
                }
                builder.AppendLine();
            }

            // Add optional types
            if (this.OptionalTypes.Any())
            {
                builder.Append("Optional: ");
                foreach (var fileType in this.OptionalTypes)
                {
                    builder.AppendFormat("{0} file; ", fileType);
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Determine whether a set of files is a valid file type combination.
        /// </summary>
        /// <param name="files">The list of files to check.</param>
        /// <returns>A value indicating whether the set of files is valid.</returns>
        public bool IsValid(List<InputFile> files)
        {
            bool isValidCombo = true;

            // Files should contain at least one file for each of the required file types.
            if (this.RequiredTypes.Any())
            {
                isValidCombo &= this.RequiredTypes.Aggregate(true, (current, requiredType) => current & files.Any(file => file.FileType == requiredType));
            }

            // Files should contain at least one file for any of the AtLeastOneOf types.
            if (isValidCombo && this.AtLeastOneOf.Any())
            {
                isValidCombo &= this.AtLeastOneOf.Any(fileType => files.Any(file => file.FileType == fileType));
            }

            return isValidCombo;
        }
    }
}
