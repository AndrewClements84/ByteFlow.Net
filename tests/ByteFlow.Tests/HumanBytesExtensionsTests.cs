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
        [InlineData("1B", 1)]           // no space, raw bytes
        [InlineData("1KB", 1024)]       // no space before suffix
        [InlineData(" 1 MB", 1048576)]  // leading space
        [InlineData("2 GB ", 2147483648)] // trailing space
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

        [Fact]
        public void TryParseHumanBytes_ShouldReturnFalseOnInvalidInput()
        {
            bool success = "not a size".TryParseHumanBytes(out long result);

            Assert.False(success);
            Assert.Equal(0, result);
        }
    }
}
