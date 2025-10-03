using System;
using System.Globalization;
using System.Linq;

namespace ByteFlow
{
    /// <summary>
    /// Provides extension methods for converting between raw byte counts and human-readable sizes.
    /// </summary>
    public static class HumanBytesExtensions
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB" };

        /// <summary>
        /// Converts a number of bytes into a human-readable string
        /// using either SI (decimal: KB, MB, GB) or IEC (binary: KiB, MiB, GiB) units.
        /// </summary>
        /// <param name="bytes">The size in bytes.</param>
        /// <param name="decimalPlaces">Number of decimal places to display.</param>
        /// <param name="standard">Whether to use SI (base 1000) or IEC (base 1024) units.</param>
        /// <returns>A formatted string such as "1.23 MB" (SI) or "1.18 MiB" (IEC).</returns>
        public static string ToHumanBytes(this long bytes, int decimalPlaces = 2, UnitStandard standard = UnitStandard.IEC)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Value must be non-negative.");

            var suffixes = standard == UnitStandard.SI ? SiSuffixes : IecSuffixes;

            if (bytes == 0)
                return $"0 {suffixes[0].Symbol}";

            var mag = suffixes.Length - 1;
            for (int i = 1; i < suffixes.Length; i++)
            {
                if (bytes < suffixes[i].Factor)
                {
                    mag = i - 1;
                    break;
                }
            }

            double adjusted = bytes / suffixes[mag].Factor;
            return string.Format(CultureInfo.InvariantCulture,
                $"{{0:F{decimalPlaces}}} {{1}}", adjusted, suffixes[mag].Symbol);
        }

        /// <summary>
        /// Parses a human-readable size string (e.g. "2.5 GB" or "2.5 GiB")
        /// back into bytes, using either SI (decimal) or IEC (binary) interpretation.
        /// </summary>
        /// <param name="input">The input string, e.g. "1 KB", "1 KiB", "1 MB".</param>
        /// <param name="standard">Whether to interpret units as SI (base 1000) or IEC (base 1024).</param>
        /// <returns>The size in bytes.</returns>
        public static long ToBytes(this string input, UnitStandard standard = UnitStandard.IEC)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            input = input.Trim();
            var suffixes = standard == UnitStandard.SI ? SiSuffixes : IecSuffixes;

            foreach (var (symbol, factor) in suffixes.OrderByDescending(s => s.Symbol.Length))
            {
                if (input.EndsWith(symbol, StringComparison.OrdinalIgnoreCase))
                {
                    var numberPart = input.Substring(0, input.Length - symbol.Length).Trim();
                    if (!double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                        throw new FormatException($"Invalid number format: {numberPart}");
                    return (long)(value * factor);
                }
            }

            throw new FormatException($"Invalid size suffix in: {input}");
        }

        /// <summary>
        /// Safely parses a human-readable string into bytes, using either SI (decimal) or IEC (binary) units.
        /// </summary>
        /// <param name="input">The input string (e.g. "1 KB", "1 KiB", "2.5 MB").</param>
        /// <param name="result">The parsed byte value if successful, or 0 if parsing fails.</param>
        /// <param name="standard">Whether to interpret units as SI (base 1000) or IEC (base 1024).</param>
        /// <returns>
        /// <c>true</c> if parsing was successful; otherwise <c>false</c>.
        /// </returns>
        public static bool TryParseHumanBytes(this string input, out long result, UnitStandard standard = UnitStandard.IEC)
        {
            try
            {
                result = input.ToBytes(standard);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        private static readonly (string Symbol, double Factor)[] SiSuffixes =
        {
            ("B", 1d),
            ("KB", 1e3),
            ("MB", 1e6),
            ("GB", 1e9),
            ("TB", 1e12),
            ("PB", 1e15)
        };

        private static readonly (string Symbol, double Factor)[] IecSuffixes =
        {
            ("B", 1d),
            ("KiB", 1024d),
            ("MiB", Math.Pow(1024, 2)),
            ("GiB", Math.Pow(1024, 3)),
            ("TiB", Math.Pow(1024, 4)),
            ("PiB", Math.Pow(1024, 5))
        };
    }
}
