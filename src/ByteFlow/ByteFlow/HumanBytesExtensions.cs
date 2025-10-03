using System;
using System.Globalization;
using System.Linq;

namespace ByteFlow
{
    /// <summary>
    /// Provides extension methods for converting between raw byte counts and human-readable sizes.
    /// Supports both IEC (binary: KiB, MiB, GiB) and SI (decimal: KB, MB, GB) unit standards,
    /// allows culture-aware formatting and parsing, and supports custom suffix sets.
    /// </summary>
    public static class HumanBytesExtensions
    {
        /// <summary>
        /// Converts a number of bytes into a human-readable string using either
        /// SI (decimal: KB, MB, GB) or IEC (binary: KiB, MiB, GiB) units,
        /// or a custom suffix set if provided.
        /// </summary>
        public static string ToHumanBytes(
            this long bytes,
            int decimalPlaces = 2,
            UnitStandard standard = UnitStandard.IEC,
            IFormatProvider formatProvider = null,
            (string Symbol, double Factor)[] customSuffixes = null)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Value must be non-negative.");

            var suffixes = customSuffixes ?? (standard == UnitStandard.SI ? SiSuffixes : IecSuffixes);

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
            string number = adjusted.ToString($"F{decimalPlaces}", formatProvider ?? CultureInfo.InvariantCulture);

            return $"{number} {suffixes[mag].Symbol}";
        }

        /// <summary>
        /// Converts a number of bytes into a human-readable string,
        /// padded to a given width for alignment.
        /// </summary>
        public static string ToHumanBytesAligned(
            this long bytes,
            int decimalPlaces = 2,
            UnitStandard standard = UnitStandard.IEC,
            int width = 10,
            char paddingChar = ' ',
            IFormatProvider formatProvider = null,
            (string Symbol, double Factor)[] customSuffixes = null)
        {
            var s = ToHumanBytes(bytes, decimalPlaces, standard, formatProvider, customSuffixes);
            return s.PadLeft(width, paddingChar);
        }

        /// <summary>
        /// Parses a human-readable size string back into bytes,
        /// using either SI (decimal), IEC (binary), or a custom suffix set if provided.
        /// </summary>
        public static long ToBytes(
            this string input,
            UnitStandard standard = UnitStandard.IEC,
            IFormatProvider formatProvider = null,
            (string Symbol, double Factor)[] customSuffixes = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            input = input.Trim();
            var suffixes = customSuffixes ?? (standard == UnitStandard.SI ? SiSuffixes : IecSuffixes);

            foreach (var (symbol, factor) in suffixes.OrderByDescending(s => s.Symbol.Length))
            {
                if (input.EndsWith(symbol, StringComparison.OrdinalIgnoreCase))
                {
                    var numberPart = input.Substring(0, input.Length - symbol.Length).Trim();
                    if (!double.TryParse(
                            numberPart,
                            NumberStyles.Float,
                            formatProvider ?? CultureInfo.InvariantCulture,
                            out var value))
                        throw new FormatException($"Invalid number format: {numberPart}");

                    return (long)(value * factor);
                }
            }

            throw new FormatException($"Invalid size suffix in: {input}");
        }

        /// <summary>
        /// Safely parses a human-readable string into bytes.
        /// </summary>
        public static bool TryParseHumanBytes(this string input, out long result)
        {
            try
            {
                result = input.ToBytes();
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        /// <summary>
        /// Safely parses a human-readable string into bytes,
        /// supporting SI, IEC, or custom suffix sets.
        /// </summary>
        public static bool TryParseHumanBytes(
            this string input,
            out long result,
            UnitStandard standard = UnitStandard.IEC,
            IFormatProvider formatProvider = null,
            (string Symbol, double Factor)[] customSuffixes = null)
        {
            try
            {
                result = input.ToBytes(standard, formatProvider, customSuffixes);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        // --- Default suffix definitions ---

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
