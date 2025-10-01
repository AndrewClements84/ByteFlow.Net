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
        /// Converts a number of bytes into a human-readable string (e.g. "1.18 MB").
        /// </summary>
        /// <param name="bytes">The size in bytes.</param>
        /// <param name="decimalPlaces">Number of decimal places to display.</param>
        public static string ToHumanBytes(this long bytes, int decimalPlaces = 2)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Value must be non-negative.");

            if (bytes == 0)
                return $"0 {SizeSuffixes[0]}";

            int mag = (int)Math.Log(bytes, 1024);
            if (mag >= SizeSuffixes.Length)
            {
                mag = SizeSuffixes.Length - 1; // clamp to PB
            }

            double adjustedSize = bytes / Math.Pow(1024, mag);

            return string.Format(CultureInfo.InvariantCulture,
                $"{{0:F{decimalPlaces}}} {{1}}", adjustedSize, SizeSuffixes[mag]);
        }

        /// <summary>
        /// Parses a human-readable size string (e.g. "2.5 GB") back into bytes.
        /// </summary>
        /// <param name="input">The input string.</param>
        public static long ToBytes(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            input = input.Trim();

            // Order suffixes by length so "KB" matches before "B"
            foreach (var suffix in SizeSuffixes.OrderByDescending(s => s.Length))
            {
                if (input.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    string numberPart = input.Substring(0, input.Length - suffix.Length).TrimEnd();

                    if (double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    {
                        int index = Array.IndexOf(SizeSuffixes, suffix);
                        return (long)(value * Math.Pow(1024, index));
                    }

                    throw new FormatException($"Invalid number format: {numberPart}");
                }
            }

            throw new FormatException($"Invalid size suffix in: {input}");
        }

        /// <summary>
        /// Safely parses a human-readable string into bytes.
        /// Returns false if parsing fails.
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
    }
}
