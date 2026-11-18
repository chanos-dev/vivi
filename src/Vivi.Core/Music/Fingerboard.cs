namespace Vivi.Core.Music;

/// <summary>
/// 바이올린 지판의 음과 운지 관계를 계산한다.
/// </summary>
/// <remarks>
/// <para>
/// 1포지션 이상에서는 표준 규칙을 따른다. p포지션의 f번 손가락은 개방현에서 (p − 1) + f 글자 위의 음을 짚는다.
/// 예를 들어 A현 1포지션 1번은 B, 3포지션 1번은 D다. ♯과 ♭은 같은 손가락을 높게·낮게 짚는 것(high/low)으로 본다.
/// </para>
/// <para>
/// 반 포지션은 손 전체를 1포지션보다 반음 낮게 두는 자리라 글자가 아닌 반음 거리로 판정한다.
/// 개방현에서 1번 +1, 2번 +2~3, 3번 +4, 4번 +5~6 반음이다(A현: B♭ / B·C / C♯ / D·E♭).
/// </para>
/// </remarks>
public static class Fingerboard
{
    public const int FingerCount = 4;

    private static readonly (int Low, int High)[] HalfPositionSemitones =
    [
        (1, 1), // 1번
        (2, 3), // 2번
        (4, 4), // 3번
        (5, 6), // 4번
    ];

    /// <summary>
    /// 표기된 음을 낼 수 있는 모든 운지. 운지 퀴즈에서는 이 중 어느 것이든 정답이다.
    /// </summary>
    public static IReadOnlyList<Fingering> GetFingerings(Pitch pitch) => GetFingerings(pitch, Position.Half, Position.MaxNumber);

    /// <param name="pitch">표기된 음.</param>
    /// <param name="minPosition">포함할 가장 낮은 포지션. <see cref="Position.Half"/>이면 반 포지션을 포함한다.</param>
    /// <param name="maxPosition">포함할 가장 높은 포지션 번호.</param>
    public static IReadOnlyList<Fingering> GetFingerings(Pitch pitch, Position minPosition, int maxPosition)
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

            if (minPosition.IsHalf)
            {
                for (var finger = 1; finger <= FingerCount; finger++)
                {
                    var (low, high) = HalfPositionSemitones[finger - 1];
                    if (semitones >= low && semitones <= high)
                    {
                        result.Add(new Fingering(violinString, Position.Half, finger));
                    }
                }
            }

            var lowest = Math.Max(1, minPosition.Number);
            for (var finger = 1; finger <= FingerCount; finger++)
            {
                var positionNumber = letterSteps - finger + 1;
                if (positionNumber >= lowest && positionNumber <= maxPosition)
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
    /// 지판 지도용: 한 현·한 포지션에서 각 손가락(1~4)이 짚을 수 있는 음들. 낮은 음부터 정렬된다.
    /// 1포지션 이상은 기본 글자의 ♭·내추럴·♯, 반 포지션은 ♯·♭ 두 표기를 모두 담는다.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<Pitch>> GetFingerPitches(EViolinString violinString, Position position)
    {
        var open = violinString.OpenPitch();
        var fingers = new List<IReadOnlyList<Pitch>>(FingerCount);

        for (var finger = 1; finger <= FingerCount; finger++)
        {
            var pitches = new SortedSet<Pitch>();
            if (position.IsHalf)
            {
                var (low, high) = HalfPositionSemitones[finger - 1];
                for (var s = low; s <= high; s++)
                {
                    pitches.Add(Pitch.FromMidi(open.Midi + s));
                    pitches.Add(Pitch.FromMidi(open.Midi + s, preferFlat: true));
                }
            }
            else
            {
                var diatonic = open.DiatonicIndex + position.Number - 1 + finger;
                var letter = (ENoteLetter)(diatonic % 7);
                var octave = diatonic / 7;
                pitches.Add(new Pitch(letter, EAccidental.Flat, octave));
                pitches.Add(new Pitch(letter, EAccidental.Natural, octave));
                pitches.Add(new Pitch(letter, EAccidental.Sharp, octave));
            }

            fingers.Add(pitches.ToArray());
        }

        return fingers;
    }
}
