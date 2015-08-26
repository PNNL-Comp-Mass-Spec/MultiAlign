namespace MultiAlignCore.IO.TextFiles
{
    public class MsMsFileWriterFactory
    {
        /// <summary>
        /// Creates a spectra writer based on the file type.
        /// </summary>
        /// <param name="writerType"></param>
        /// <returns></returns>
        public static IMsMsSpectraWriter CreateSpectraWriter(MsMsWriterType writerType)
        {
            IMsMsSpectraWriter writer = null;
            switch (writerType)
            {
                case MsMsWriterType.DTA:
                    writer = new DtaFileWriter();
                    break;
                case MsMsWriterType.MGF:
                    writer = new MgfFileWriter();
                    break;
                default:
                    break;
            }
            return writer;
        }
        /// <summary>
        /// Creates a spectra writer based on the file type.
        /// </summary>
        /// <param name="writerType"></param>
        /// <returns></returns>
        public static IMsMsSpectraWriter CreateSpectraWriter(string  extension)
        {
            IMsMsSpectraWriter writer = null;
            switch (extension)
            {
                case ".dta":
                    writer =  CreateSpectraWriter(MsMsWriterType.DTA);
                    break;
                case ".mgf":
                    writer = CreateSpectraWriter(MsMsWriterType.MGF);
                    break;
            }
            return writer;
        }
    }

    public enum MsMsWriterType
    {
        DTA,
        MGF
    }
}
