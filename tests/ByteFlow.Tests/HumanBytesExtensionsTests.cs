namespace ByteFlow.Tests
{
    public class HumanBytesExtensionsTests
    {
        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(1023, "1023.00 B")]
        [InlineData(1024, "1.00 KB")]
        [InlineData(1048576, "1.00 MB")]
        public void ToHumanBytes_ShouldConvertCorrectly(long input, string expected)
        {
            string result = input.ToHumanBytes();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToHumanBytes_ShouldRespectDecimalPlaces()
        {
            long input = 1234567;
            string result = input.ToHumanBytes(3);

            Assert.Equal("1.177 MB", result);
        }

        [Theory]
        [InlineData("1 KB", 1024)]
        [InlineData("1 MB", 1048576)]
        [InlineData("2.5 GB", 2684354560)]
        [InlineData("1B", 1)]
        [InlineData("1KB", 1024)]
        [InlineData(" 1 MB", 1048576)]
        [InlineData("2 GB ", 2147483648)]
        public void ToBytes_ShouldParseCorrectly(string input, long expected)
        {
            long result = input.ToBytes();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToBytes_InvalidString_ShouldThrow()
        {
            Assert.Throws<FormatException>(() => "invalid".ToBytes());
        }

        [Theory]
        [InlineData("1 XB")]       // unsupported unit
        [InlineData("ten MB")]     // not a number
        [InlineData("1.2.3 GB")]   // malformed number
        [InlineData(" ")]          // empty after trim
        public void ToBytes_InvalidInputs_ShouldThrow(string input)
        {
            Assert.ThrowsAny<Exception>(() => input.ToBytes());
        }

        [Fact]
        public void ToBytes_Null_ShouldThrow()
        {
            string input = null;
            Assert.Throws<ArgumentNullException>(() => input.ToBytes());
        }

        [Fact]
        public void TryParseHumanBytes_ShouldReturnFalseOnInvalidInput()
        {
            bool success = "not a size".TryParseHumanBytes(out long result);

            Assert.False(success);
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData("1 XB")]
        [InlineData("ten MB")]
        [InlineData("1.2.3 GB")]
        [InlineData(" ")]
        [InlineData(null)]
        public void TryParseHumanBytes_InvalidInputs_ShouldReturnFalse(string input)
        {
            bool success = input.TryParseHumanBytes(out long result);

            Assert.False(success);
            Assert.Equal(0, result);
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
        public void ToBytes_Whitespace_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => "   ".ToBytes());
        }

        [Fact]
        public void ToBytes_ShouldSupportPetabytes()
        {
            string input = "1 PB";
            long result = input.ToBytes();

            // 1 PB = 1024^5 bytes
            Assert.Equal((long)Math.Pow(1024, 5), result);
        }

        [Fact]
        public void ToHumanBytes_ShouldHandleLongMaxValue()
        {
            string result = long.MaxValue.ToHumanBytes();
            Assert.Contains("PB", result); // should be expressed in petabytes
        }

        [Fact]
        public void ToBytes_ShouldHandleVeryLargePetabytes()
        {
            string input = "8192 PB";
            long result = input.ToBytes();
            double expected = 8192 * Math.Pow(1024, 5);
            Assert.Equal((long)expected, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1024)]           // 1 KB
        [InlineData(1048576)]        // 1 MB
        [InlineData(1073741824)]     // 1 GB
        [InlineData(1099511627776)]  // 1 TB
        public void RoundTrip_BytesToHumanAndBack_ShouldBeConsistent(long original)
        {
            string human = original.ToHumanBytes();
            long parsed = human.ToBytes();

            // Allow slight tolerance because ToHumanBytes() rounds
            double tolerance = original * 0.01; // 1% margin
            Assert.InRange(parsed, original - tolerance, original + tolerance);
        }

        [Theory]
        [InlineData("NotBytes")]
        [InlineData("123 QZ")]   // invalid suffix
        [InlineData("1.2.3 GB")] // malformed number
        public void RoundTrip_InvalidStrings_ShouldFailGracefully(string input)
        {

            bool success = input.TryParseHumanBytes(out long result);

            Assert.False(success);
            Assert.Equal(0, result);
        }
    }
}
