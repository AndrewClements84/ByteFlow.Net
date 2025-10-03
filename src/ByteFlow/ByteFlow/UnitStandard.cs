namespace ByteFlow
{
    /// <summary>
    /// Defines whether to use decimal (SI: KB, MB, ...) or binary (IEC: KiB, MiB, ...) units.
    /// </summary>
    public enum UnitStandard
    {
        /// <summary>
        /// Binary (base 1024): KiB, MiB, GiB...
        /// </summary>
        IEC, // 1024-based: KiB, MiB, …
        /// <summary>
        /// Decimal (base 1000): KB, MB, GB...
        /// </summary>
        SI   // 1000-based: KB, MB, …
    }

}
