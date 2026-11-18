using Vivi.Core.Music;

namespace Vivi.Core.Tests.Music;

public class ConcertPitchTests
{
    private const double Tolerance = 0.001;

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

    [Theory]
    [InlineData(440.0, "A4", 440.0)]
    [InlineData(443.0, "A4", 443.0)]
    [InlineData(440.0, "A5", 880.0)]
    [InlineData(440.0, "G3", 195.998)]
    [InlineData(440.0, "D4", 293.665)]
    [InlineData(440.0, "E5", 659.255)]
    [InlineData(443.0, "E5", 663.750)]
    public void FrequencyOf_UsesEqualTemperament(double a4, string pitch, double expectedHz)
    {
        Assert.Equal(expectedHz, new ConcertPitch(a4).FrequencyOf(Pitch.Parse(pitch)), Tolerance);
    }

    [Theory]
    [InlineData(440.0, 440.0, 69)]
    [InlineData(440.0, 452.0, 69)]  // +46센트 → 여전히 A4
    [InlineData(440.0, 455.0, 70)]  // +58센트 → B♭4
    [InlineData(443.0, 443.0, 69)]
    [InlineData(443.0, 440.0, 69)]
    public void NearestMidi_RoundsToClosestSemitone(double a4, double hz, int expectedMidi)
    {
        Assert.Equal(expectedMidi, new ConcertPitch(a4).NearestMidi(hz));
    }

    [Theory]
    [InlineData(440.0, 440.0, 0.0)]
    [InlineData(880.0, 440.0, 1200.0)]
    [InlineData(466.164, 440.0, 100.0)]
    [InlineData(440.0, 466.164, -100.0)]
    public void CentsBetween_MeasuresInterval(double hz, double target, double expectedCents)
    {
        Assert.Equal(expectedCents, ConcertPitch.CentsBetween(hz, target), 0.01);
    }

    [Fact]
    public void CentsFrom_ReflectsReferencePitch()
    {
        // 443 Hz 기준으로 443.77 Hz는 A4보다 약 +3센트
        var cents = new ConcertPitch(443).CentsFrom(443.77, Pitch.Parse("A4"));
        Assert.Equal(3.0, cents, 0.1);
    }
}
