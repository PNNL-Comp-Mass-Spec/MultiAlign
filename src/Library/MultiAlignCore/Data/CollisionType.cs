namespace MultiAlignCore.Data
{
    /// <summary>
    /// Type of MS/MS fragmentation techniques.
    /// </summary>
    public enum CollisionType
    {
        Cid = 0,
        Ecd,
        Etd,
        Hcd,
        Hid,        
        None,
        Other        
    }
}