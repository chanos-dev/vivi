using Vivi.Core.Music;

namespace Vivi.Core.Tests.Music;

public class PitchTests
{
    [Theory]
    [InlineData("C4", 60)]
    [InlineData("A4", 69)]
    [InlineData("G3", 55)]
    [InlineData("E5", 76)]
    [InlineData("B♭4", 70)]
    [InlineData("Bb4", 70)]
    [InlineData("C#5", 73)]
    [InlineData("C♯5", 73)]
    [InlineData("B#3", 60)]
    [InlineData("Cb4", 59)]
    public void Parse_ComputesMidi(string text, int expectedMidi)
    {
        Assert.Equal(expectedMidi, Pitch.Parse(text).Midi);
    }

    [Theory]
    [InlineData("A4", "A4")]
    [InlineData("C#5", "C♯5")]
    [InlineData("bb4", "B♭4")]
    public void ToString_UsesMusicSymbols(string text, string expected)
    {
        Assert.Equal(expected, Pitch.Parse(text).ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("H4")]
    [InlineData("A")]
    [InlineData("A#")]
    [InlineData("4A")]
    [InlineData("A10")]
    public void TryParse_RejectsInvalidText(string text)
    {
        Assert.False(Pitch.TryParse(text, out _));
    }

    [Fact]
    public void EnharmonicSpellings_AreDifferentButSoundTheSame()
    {
        var cSharp = Pitch.Parse("C#5");
        var dFlat = Pitch.Parse("Db5");

        Assert.NotEqual(cSharp, dFlat);
        Assert.True(cSharp.IsEnharmonicWith(dFlat));
    }

    [Theory]
    [InlineData(70, false, "A♯4")]
    [InlineData(70, true, "B♭4")]
    [InlineData(60, false, "C4")]
    [InlineData(78, true, "G♭5")]
    public void FromMidi_SpellsWithSharpsOrFlats(int midi, bool preferFlat, string expected)
    {
        Assert.Equal(expected, Pitch.FromMidi(midi, preferFlat).ToString());
    }

    [Theory]
    [InlineData("C#5", true)]
    [InlineData("Bb4", true)]
    [InlineData("E5", true)]
    [InlineData("E#5", false)]
    [InlineData("B#4", false)]
    [InlineData("Fb5", false)]
    [InlineData("Cb5", false)]
    public void IsCommonSpelling_ExcludesRareEnharmonics(string text, bool expected)
    {
        Assert.Equal(expected, Pitch.Parse(text).IsCommonSpelling);
    }
}
