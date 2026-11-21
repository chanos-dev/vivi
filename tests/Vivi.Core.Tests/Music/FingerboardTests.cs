using Vivi.Core.Music;

namespace Vivi.Core.Tests.Music;

public class FingerboardTests
{
    private static Fingering F(EViolinString s, int position, int finger) => new(s, new Position(position), finger);

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
    public void Bflat4_IsLowFirstFinger()
    {
        Assert.Contains(F(EViolinString.A, 1, 1), Fingerboard.GetFingerings(Pitch.Parse("Bb4")));  // 1포지션 low 1
    }

    [Theory]
    [InlineData("A#4", EViolinString.A)]
    [InlineData("G#3", EViolinString.G)]
    [InlineData("D#4", EViolinString.D)]
    [InlineData("E#5", EViolinString.E)]
    public void SharpOfOpenString_IsFirstPositionLowFirstFinger(string pitch, EViolinString violinString)
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse(pitch));

        Assert.Contains(F(violinString, 1, 1), fingerings);
    }

    [Fact]
    public void SharpOfOpenString_RespectsPositionRange()
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse("A#4"), minPosition: 2);

        Assert.DoesNotContain(F(EViolinString.A, 1, 1), fingerings);
    }

    [Fact]
    public void PositionRange_LimitsResults()
    {
        var fingerings = Fingerboard.GetFingerings(Pitch.Parse("A4"), maxPosition: 3);

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

    [Fact]
    public void GetPlacements_FirstPositionOnA_MarksNaturalsAsPrimary()
    {
        var placements = Fingerboard.GetPlacements(EViolinString.A, Position.First);
        var primary = placements.Where(p => p.IsPrimary).Select(p => (p.Finger, p.Semitones, p.Pitch.ToString()));

        Assert.Equal([(1, 2, "B4"), (2, 3, "C5"), (3, 5, "D5"), (4, 7, "E5")], primary);
        Assert.DoesNotContain(placements, p => !p.Pitch.IsCommonSpelling);
    }

    [Fact]
    public void GetPlacements_VariantsCanShareASemitone()
    {
        // A현 1포지션: 높은 2번(C♯5)과 낮은 3번(D♭5)은 같은 자리(+4)
        var atFour = Fingerboard.GetPlacements(EViolinString.A, Position.First).Where(p => p.Semitones == 4).ToList();

        Assert.Equal(2, atFour.Count);
        Assert.Contains(atFour, p => p.Finger == 2 && p.Pitch.ToString() == "C♯5");
        Assert.Contains(atFour, p => p.Finger == 3 && p.Pitch.ToString() == "D♭5");
    }


    [Theory]
    [InlineData(EViolinString.A, "F5")]   // E♯5 → F5
    [InlineData(EViolinString.E, "C6")]   // B♯5 → C6
    public void GetPlacements_FillsHighFourthWithCommonSpelling(EViolinString violinString, string expected)
    {
        var highFourth = Fingerboard.GetPlacements(violinString, Position.First)
            .Single(p => p.Finger == 4 && p.Reach == EFingerReach.High);

        Assert.Equal(8, highFourth.Semitones);
        Assert.Equal(expected, highFourth.Pitch.ToString());
    }

    [Fact]
    public void GetPlacements_SkipsRespelledVariantOnOccupiedOrOpenSpot()
    {
        // E현 1포지션 낮은 1번(F♭5 = E5)은 개방현 자리라 넣지 않는다.
        Assert.DoesNotContain(Fingerboard.GetPlacements(EViolinString.E, Position.First), p => p.Semitones == 0);

        // G현 1포지션 높은 2번(B♯3 = C4)은 3번 기본 자리(C4)와 겹쳐 넣지 않는다.
        var atFive = Fingerboard.GetPlacements(EViolinString.G, Position.First).Where(p => p.Semitones == 5).ToList();
        Assert.Equal([3], atFive.Select(p => p.Finger));
    }

    [Fact]
    public void GetPlacements_EveryFingerHasLowAndHighWhereSpotIsFree()
    {
        foreach (var violinString in EViolinStringExtensions.All)
        {
            foreach (var position in Position.All)
            {
                var placements = Fingerboard.GetPlacements(violinString, position);
                var occupied = placements.Select(p => p.Semitones).ToHashSet();
                var naturals = placements.Where(p => p.IsPrimary).ToList();
                Assert.Equal(Fingerboard.FingerCount, naturals.Count);

                foreach (var natural in naturals)
                {
                    // 기본 자리 위아래 반음은 (개방현 자리가 아니면) 반드시 어떤 점이든 있다.
                    Assert.True(occupied.Contains(natural.Semitones + 1), $"{violinString} pos{position} f{natural.Finger} high");
                    if (natural.Semitones - 1 > 0)
                    {
                        Assert.True(occupied.Contains(natural.Semitones - 1), $"{violinString} pos{position} f{natural.Finger} low");
                    }
                }
            }
        }
    }
}
