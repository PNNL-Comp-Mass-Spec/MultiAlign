namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Enumerations of possible Alignment Types
    /// </summary>
    public enum LcmsWarpAlignmentType
    {
        /// <summary>
        /// Alignment type that uses a single NET warp
        /// </summary>
        NET_WARP = 0,

        /// <summary>
        /// Alignment type that performs a NET warp, recalibrates with regards to Mass
        /// and then performs warping again
        /// </summary>
        NET_MASS_WARP
    }
}
