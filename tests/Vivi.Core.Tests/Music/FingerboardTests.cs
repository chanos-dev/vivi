using Vivi.Core.Music;

namespace Vivi.Core.Tests.Music;

public class FingerboardTests
{
    private static Fingering F(EViolinString s, int position, int finger) => new(s, new Position(position), finger);

    private static Fingering Half(EViolinString s, int finger) => new(s, Position.Half, finger);

    [Fact]
    public void LowestNote_G3_IsOnlyOpenGString()
    {
        Assert.Equal([Fingering.Open(EViolinString.G)], Fingerboard.GetFingerings(Pitch.Parse("G3")));
    }

    [Fact]
    public void BelowRange_HasNoFingerings()
    {
        Assert.Empty(Fingerboard.GetFingerings(Pitch.Parse("F#3")));
    }

    [Fact]
    public void A4_IncludesOpenStringAndFourthFingerOnD()
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse("A4"));

        Assert.Contains(Fingering.Open(EViolinString.A), fingerings);
        Assert.Contains(F(EViolinString.D, 1, 4), fingerings);  // 1포지션 4번
        Assert.Contains(F(EViolinString.D, 3, 2), fingerings);  // 3포지션 2번
        Assert.Contains(F(EViolinString.G, 5, 4), fingerings);  // G현 5포지션 4번
        Assert.Contains(F(EViolinString.G, 8, 1), fingerings);  // G현 8포지션 1번
        Assert.DoesNotContain(fingerings, f => f.String == EViolinString.E);
    }

    [Fact]
    public void FirstPosition_OnAString_FollowsStandardFingering()
    {
        Assert.Contains(F(EViolinString.A, 1, 1), Fingerboard.GetFingerings(Pitch.Parse("B4")));
        Assert.Contains(F(EViolinString.A, 1, 2), Fingerboard.GetFingerings(Pitch.Parse("C#5")));  // high 2
        Assert.Contains(F(EViolinString.A, 1, 2), Fingerboard.GetFingerings(Pitch.Parse("C5")));   // low 2
        Assert.Contains(F(EViolinString.A, 1, 3), Fingerboard.GetFingerings(Pitch.Parse("D5")));
        Assert.Contains(F(EViolinString.A, 1, 4), Fingerboard.GetFingerings(Pitch.Parse("E5")));
    }

    [Fact]
    public void ThirdPosition_FirstFinger_IsThirdLetterAboveOpen()
    {
        Assert.Contains(F(EViolinString.G, 3, 1), Fingerboard.GetFingerings(Pitch.Parse("C4")));
        Assert.Contains(F(EViolinString.A, 3, 1), Fingerboard.GetFingerings(Pitch.Parse("D5")));
        Assert.Contains(F(EViolinString.E, 3, 1), Fingerboard.GetFingerings(Pitch.Parse("A5")));
    }

    [Fact]
    public void Bflat4_IsLowFirstFingerAndHalfPositionFirstFinger()
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse("Bb4"));

        Assert.Contains(F(EViolinString.A, 1, 1), fingerings);  // 1포지션 low 1
        Assert.Contains(Half(EViolinString.A, 1), fingerings);  // 반 포지션 1번
    }

    [Theory]
    [InlineData("Bb4", 1)]
    [InlineData("A#4", 1)]
    [InlineData("B4", 2)]
    [InlineData("C5", 2)]
    [InlineData("C#5", 3)]
    [InlineData("Db5", 3)]
    [InlineData("D5", 4)]
    [InlineData("Eb5", 4)]
    public void HalfPosition_OnAString_UsesSemitoneDistance(string pitch, int expectedFinger)
    {
        var halfFingers = Fingerboard.GetFingerings(Pitch.Parse(pitch))
            .Where(f => f.String == EViolinString.A && f.Position is { IsHalf: true })
            .Select(f => f.Finger);

        Assert.Equal([expectedFinger], halfFingers);
    }

    [Fact]
    public void HalfPosition_CanBeExcluded()
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse("Bb4"), Position.First, Position.MaxNumber);

        Assert.DoesNotContain(fingerings, f => f.Position is { IsHalf: true });
        Assert.Contains(F(EViolinString.A, 1, 1), fingerings);
    }

    [Fact]
    public void PositionRange_LimitsResults()
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse("A4"), Position.First, maxPosition: 3);

        Assert.All(fingerings.Where(f => !f.IsOpenString), f => Assert.InRange(f.Position!.Value.Number, 1, 3));
        Assert.DoesNotContain(F(EViolinString.G, 5, 4), fingerings);
    }

    [Fact]
    public void HighestPosition_FourthFingerOnE_IsD7()
    {
        Assert.Contains(F(EViolinString.E, Position.MaxNumber, 4), Fingerboard.GetFingerings(Pitch.Parse("D7")));
    }

    [Fact]
    public void CanPlay_AcceptsEveryValidFingering()
    {
        var e5 = Pitch.Parse("E5");

        Assert.True(Fingerboard.CanPlay(Fingering.Open(EViolinString.E), e5));
        Assert.True(Fingerboard.CanPlay(F(EViolinString.A, 1, 4), e5));
        Assert.False(Fingerboard.CanPlay(F(EViolinString.A, 1, 3), e5));
    }

    [Fact]
    public void GetFingerPitches_FirstPositionOnA()
    {
        var fingers = Fingerboard.GetFingerPitches(EViolinString.A, Position.First);

        Assert.Equal(["B♭4", "B4", "B♯4"], fingers[0].Select(p => p.ToString()));
        Assert.Equal(["C♭5", "C5", "C♯5"], fingers[1].Select(p => p.ToString()));
        Assert.Equal(["E♭5", "E5", "E♯5"], fingers[3].Select(p => p.ToString()));
    }

    [Fact]
    public void GetFingerPitches_HalfPositionOnA()
    {
        var fingers = Fingerboard.GetFingerPitches(EViolinString.A, Position.Half);

        Assert.Equal(["A♯4", "B♭4"], fingers[0].Select(p => p.ToString()));
        Assert.Equal(["C♯5", "D♭5"], fingers[2].Select(p => p.ToString()));
    }

    [Fact]
    public void GetFingerPitches_AreAllPlayableWithThatFingering()
    {
        foreach (var violinString in EViolinStringExtensions.All)
        {
            foreach (var position in Position.All)
            {
                var fingers = Fingerboard.GetFingerPitches(violinString, position);
                for (var finger = 1; finger <= Fingerboard.FingerCount; finger++)
                {
                    foreach (var pitch in fingers[finger - 1])
                    {
                        Assert.True(
                            Fingerboard.CanPlay(new Fingering(violinString, position, finger), pitch),
                            $"{pitch} on {violinString} pos {position} finger {finger}");
                    }
                }
            }
        }
    }
}
