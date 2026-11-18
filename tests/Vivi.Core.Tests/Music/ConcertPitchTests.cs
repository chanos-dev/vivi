using Vivi.Core.Music;

namespace Vivi.Core.Tests.Music;

public class ConcertPitchTests
{
    [Fact]
    public void Default_Is440Hz()
    {
        Assert.Equal(440.0, ConcertPitch.Default.A4Hz);
    }

    [Theory]
    [InlineData(415.0)]
    [InlineData(440.0)]
    [InlineData(443.0)]
    [InlineData(466.0)]
    public void Constructor_AcceptsValuesInRange(double hz)
    {
        Assert.Equal(hz, new ConcertPitch(hz).A4Hz);
    }

    [Theory]
    [InlineData(414.9)]
    [InlineData(466.1)]
    [InlineData(double.NaN)]
    public void Constructor_RejectsValuesOutOfRange(double hz)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ConcertPitch(hz));
    }
}
