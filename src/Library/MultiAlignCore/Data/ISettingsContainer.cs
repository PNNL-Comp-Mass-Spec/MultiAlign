namespace MultiAlignCore.Data
{
    /// <summary>
    /// Interface that all objects that store settings should implement.
    /// </summary>
    public interface ISettingsContainer
    {
        /// <summary>
        /// Restore settings back to their default values.
        /// </summary>
        void RestoreDefaults();
    }
}
