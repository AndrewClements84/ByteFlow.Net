using System.Globalization;

namespace ByteFlow.Tests
{
    public class HumanBytesExtensionsTests
    {
        // --- Formatting (ToHumanBytes) ---

        [Theory]
        [InlineData(0, "0 B", UnitStandard.IEC)]
        [InlineData(1023, "1023.00 B", UnitStandard.IEC)]
        [InlineData(1024, "1.00 KiB", UnitStandard.IEC)]
        [InlineData(1048576, "1.00 MiB", UnitStandard.IEC)]
        [InlineData(1000, "1.00 KB", UnitStandard.SI)]
        [InlineData(1500, "1.50 KB", UnitStandard.SI)]
        [InlineData(1_000_000, "1.00 MB", UnitStandard.SI)]
        public void ToHumanBytes_ShouldFormatCorrectly(long input, string expected, UnitStandard standard)
        {
            string result = input.ToHumanBytes(2, standard);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToHumanBytes_ShouldRespectDecimalPlaces()
        {
            long input = 1234567;
            string result = input.ToHumanBytes(3, UnitStandard.SI);
            Assert.Equal("1.235 MB", result); // 1234567 / 1e6 = 1.234567
        }

        [Fact]
        public void ToHumanBytes_ShouldRespectCulture()
        {
            var de = new CultureInfo("de-DE"); // comma decimal separator
            string result = 1234567L.ToHumanBytes(2, UnitStandard.SI, de);
            Assert.Equal("1,23 MB", result);
        }

        [Fact]
        public void ToHumanBytes_ShouldThrowOnNegativeInput()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (-1L).ToHumanBytes());
        }

        [Fact]
        public void ToHumanBytes_ShouldHandleZero()
        {
            string result = 0L.ToHumanBytes();
            Assert.Equal("0 B", result);
        }

        [Fact]
        public void ToHumanBytes_ShouldHandleLongMaxValue()
        {
            string result = long.MaxValue.ToHumanBytes();
            Assert.Contains("PiB", result); // expressed in largest IEC unit
        }

        // --- Parsing (ToBytes) ---

        [Theory]
        [InlineData("1 KiB", 1024, UnitStandard.IEC)]
        [InlineData("1 MiB", 1048576, UnitStandard.IEC)]
        [InlineData("2.5 GiB", 2684354560, UnitStandard.IEC)]
        [InlineData("1 KB", 1000, UnitStandard.SI)]
        [InlineData("1 MB", 1000000, UnitStandard.SI)]
        [InlineData("2 GB", 2000000000, UnitStandard.SI)]
        [InlineData("1B", 1, UnitStandard.IEC)]
        [InlineData(" 1 MB", 1000000, UnitStandard.SI)]
        [InlineData("2 GB ", 2000000000, UnitStandard.SI)]
        public void ToBytes_ShouldParseCorrectly(string input, long expected, UnitStandard standard)
        {
            long result = input.ToBytes(standard);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1,5 MB", 1500000, "de-DE", UnitStandard.SI)]
        [InlineData("1.5 MB", 1500000, "en-US", UnitStandard.SI)]
        [InlineData("2,5 GiB", 2684354560, "de-DE", UnitStandard.IEC)]
        public void ToBytes_ShouldParseAccordingToCulture(string input, long expected, string cultureName, UnitStandard standard)
        {
            var culture = new CultureInfo(cultureName);
            long result = input.ToBytes(standard, culture);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToBytes_InvalidString_ShouldThrow()
        {
            Assert.Throws<FormatException>(() => "invalid".ToBytes());
        }

        [Theory]
        [InlineData("1 XB")]
        [InlineData("ten MB")]
        [InlineData("1.2.3 GB")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ToBytes_InvalidInputs_ShouldThrow(string input)
        {
            if (input is null)
                Assert.Throws<ArgumentNullException>(() => input.ToBytes());
            else
                Assert.ThrowsAny<Exception>(() => input.ToBytes());
        }

        [Fact]
        public void ToBytes_Whitespace_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => "   ".ToBytes());
        }

        [Fact]
        public void ToBytes_ShouldSupportPetabytes_IEC()
        {
            string input = "1 PiB";
            long result = input.ToBytes(UnitStandard.IEC);
            Assert.Equal((long)Math.Pow(1024, 5), result);
        }

        [Fact]
        public void ToBytes_ShouldSupportPetabytes_SI()
        {
            string input = "1 PB";
            long result = input.ToBytes(UnitStandard.SI);
            Assert.Equal((long)Math.Pow(10, 15), result);
        }

        [Fact]
        public void ToBytes_ShouldHandleVeryLargePetabytes()
        {
            string input = "8192 PiB";
            long result = input.ToBytes(UnitStandard.IEC);
            double expected = 8192 * Math.Pow(1024, 5);
            Assert.Equal((long)expected, result);
        }

        [Fact]
        public void ToBytes_InputWithoutSuffix_ShouldThrow()
        {
            Assert.Throws<FormatException>(() => "123".ToBytes());
        }

        // --- TryParseHumanBytes ---

        [Theory]
        [InlineData("1 MB", 1000000, UnitStandard.SI)]
        [InlineData("1 MiB", 1048576, UnitStandard.IEC)]
        public void TryParseHumanBytes_ValidInput_ShouldReturnTrue(string input, long expected, UnitStandard standard)
        {
            bool success = input.TryParseHumanBytes(out long result, standard);
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryParseHumanBytes_ShouldRespectCulture()
        {
            var de = new CultureInfo("de-DE");
            bool success = "2,5 MiB".TryParseHumanBytes(out long result, UnitStandard.IEC, de);

            Assert.True(success);
            Assert.Equal((long)(2.5 * 1024 * 1024), result);
        }

        [Theory]
        [InlineData("1 XB", UnitStandard.SI)]
        [InlineData("ten MB", UnitStandard.SI)]
        [InlineData("1.2.3 GB", UnitStandard.IEC)]
        [InlineData(" ", UnitStandard.SI)]
        [InlineData(null, UnitStandard.IEC)]
        public void TryParseHumanBytes_InvalidInputs_ShouldReturnFalse(string input, UnitStandard standard)
        {
            bool success = input.TryParseHumanBytes(out long result, standard);
            Assert.False(success);
            Assert.Equal(0, result);
        }

        // --- Round-trip consistency ---

        [Theory]
        [InlineData(1, UnitStandard.IEC)]
        [InlineData(1024, UnitStandard.IEC)]          // 1 KiB
        [InlineData(1048576, UnitStandard.IEC)]       // 1 MiB
        [InlineData(1000, UnitStandard.SI)]           // 1 KB
        [InlineData(1_000_000, UnitStandard.SI)]      // 1 MB
        public void RoundTrip_BytesToHumanAndBack_ShouldBeConsistent(long original, UnitStandard standard)
        {
            string human = original.ToHumanBytes(2, standard);
            long parsed = human.ToBytes(standard);
            Assert.Equal(original, parsed);
        }
    }
}
