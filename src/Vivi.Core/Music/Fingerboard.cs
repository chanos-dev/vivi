namespace Vivi.Core.Music;

/// <summary>
/// 바이올린 지판의 음과 운지 관계를 계산한다.
/// </summary>
/// <remarks>
/// p포지션의 f번 손가락은 개방현에서 (p − 1) + f 글자 위의 음을 짚는다.
/// 예를 들어 A현 1포지션 1번은 B, 3포지션 1번은 D다. ♯과 ♭은 같은 손가락을 높게·낮게 짚는 것(high/low)으로 본다.
/// 개방현 글자에 ♯이 붙은 음(A현의 A♯)은 예외로 1포지션 1번(낮은 1번)으로 짚는다.
/// </remarks>
public static class Fingerboard
{
    public const int FingerCount = 4;

    /// <summary>
    /// 표기된 음을 낼 수 있는 모든 운지. 운지 퀴즈에서는 이 중 어느 것이든 정답이다.
    /// </summary>
    /// <param name="pitch">표기된 음.</param>
    /// <param name="minPosition">포함할 가장 낮은 포지션 번호.</param>
    /// <param name="maxPosition">포함할 가장 높은 포지션 번호.</param>
    public static IReadOnlyList<Fingering> GetFingerings(
        Pitch pitch, int minPosition = Position.MinNumber, int maxPosition = Position.MaxNumber)
    {
        var result = new List<Fingering>();
        foreach (var violinString in EViolinStringExtensions.All)
        {
            var open = violinString.OpenPitch();
            var semitones = pitch.Midi - open.Midi;
            var letterSteps = pitch.DiatonicIndex - open.DiatonicIndex;

            if (semitones < 0)
            {
                continue;
            }

            if (semitones == 0 && letterSteps == 0)
            {
                result.Add(Fingering.Open(violinString));
                continue;
            }

            if (letterSteps == 0 && semitones == 1 && minPosition <= 1 && maxPosition >= 1)
            {
                result.Add(new Fingering(violinString, Position.First, 1));
            }

            for (var finger = 1; finger <= FingerCount; finger++)
            {
                var positionNumber = letterSteps - finger + 1;
                if (positionNumber >= Math.Max(minPosition, Position.MinNumber) &&
                    positionNumber <= Math.Min(maxPosition, Position.MaxNumber))
                {
                    result.Add(new Fingering(violinString, new Position(positionNumber), finger));
                }
            }
        }

        return result;
    }

    /// <summary>운지 퀴즈 판정: 이 운지로 해당 음을 낼 수 있는가.</summary>
    public static bool CanPlay(Fingering fingering, Pitch pitch) => GetFingerings(pitch).Contains(fingering);

    /// <summary>
    /// 한 현·한 포지션에서 각 손가락(1~4)이 짚을 수 있는 음들: 기본 글자의 ♭·내추럴·♯. 낮은 음부터 정렬된다.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<Pitch>> GetFingerPitches(EViolinString violinString, Position position)
    {
        var open = violinString.OpenPitch();
        var fingers = new List<IReadOnlyList<Pitch>>(FingerCount);

        for (var finger = 1; finger <= FingerCount; finger++)
        {
            var diatonic = open.DiatonicIndex + position.Number - 1 + finger;
            var letter = (ENoteLetter)(diatonic % 7);
            var octave = diatonic / 7;
            fingers.Add(
            [
                new Pitch(letter, EAccidental.Flat, octave),
                new Pitch(letter, EAccidental.Natural, octave),
                new Pitch(letter, EAccidental.Sharp, octave),
            ]);
        }

        return fingers;
    }

    /// <summary>
    /// 넥 그림용: 한 현·한 포지션에서 손가락들이 짚는 자리(기본·낮게·높게). 반음 거리 → 손가락 순으로 정렬된다.
    /// </summary>
    /// <remarks>
    /// E♯·B♯·C♭·F♭처럼 드문 표기는 같은 소리의 흔한 표기(F·C·B·E)로 바꿔 쓴다.
    /// 다만 그 자리에 이미 다른 손가락이 있거나 개방현 자리면 넣지 않는다.
    /// </remarks>
    public static IReadOnlyList<FingerPlacement> GetPlacements(EViolinString violinString, Position position)
    {
        var openMidi = violinString.OpenPitch().Midi;
        var fingers = GetFingerPitches(violinString, position);
        var result = new List<FingerPlacement>();
        var rare = new List<FingerPlacement>();

        for (var finger = 1; finger <= FingerCount; finger++)
        {
            foreach (var pitch in fingers[finger - 1])
            {
                var reach = (EFingerReach)(int)pitch.Accidental;
                var placement = new FingerPlacement(violinString, finger, pitch, pitch.Midi - openMidi, reach);
                (pitch.IsCommonSpelling ? result : rare).Add(placement);
            }
        }

        foreach (var placement in rare)
        {
            if (placement.Semitones > 0 && result.All(p => p.Semitones != placement.Semitones))
            {
                var respelled = Pitch.FromMidi(placement.Pitch.Midi, preferFlat: placement.Reach == EFingerReach.Low);
                result.Add(placement with { Pitch = respelled });
            }
        }

        return result.OrderBy(p => p.Semitones).ThenBy(p => p.Finger).ToArray();
    }
}
