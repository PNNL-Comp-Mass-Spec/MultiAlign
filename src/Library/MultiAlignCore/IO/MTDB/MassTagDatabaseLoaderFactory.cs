namespace MultiAlignCore.IO.MTDB
{
    /// <summary>
    ///     Creates Database Loaders for a mass tag database system.
    /// </summary>
    public class MassTagDatabaseLoaderFactory
    {
        /// <summary>
        ///     Creates a database loader object to query a system for a set of server based
        ///     database loaders.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDatabaseServerLoader Create(MtdbDatabaseServerType type)
        {
            IDatabaseServerLoader loader = null;
            switch (type)
            {
                case MtdbDatabaseServerType.Dms:
                    loader = new MageDmsDatabaseLoader();
                    break;
                default:
                    break;
            }

            return loader;
        }
    }

    public enum MtdbDatabaseServerType
    {
        Dms
    }
}