using System.Diagnostics.CodeAnalysis;

namespace Vivi.Core.Music;

/// <summary>
/// 표기된 음(글자 + 임시표 + 옥타브). 과학적 음높이 표기법을 따르며 C4가 가운데 도다.
/// 같은 소리라도 표기가 다르면(C♯4와 D♭4) 다른 값이다. 오선보에 그리는 위치가 다르기 때문이다.
/// </summary>
public readonly record struct Pitch(ENoteLetter Letter, EAccidental Accidental, int Octave)
    : IComparable<Pitch>
{
    public Pitch(ENoteLetter letter, int octave)
        : this(letter, EAccidental.Natural, octave)
    {
    }

    /// <summary>MIDI 노트 번호. C4 = 60, A4 = 69.</summary>
    public int Midi => (Octave + 1) * 12 + Letter.NaturalSemitone() + (int)Accidental;

    /// <summary>임시표를 무시한 온음계 위치. 글자 간 거리 계산에 쓴다.</summary>
    public int DiatonicIndex => Octave * 7 + (int)Letter;

    public bool IsEnharmonicWith(Pitch other) => Midi == other.Midi;

    public int CompareTo(Pitch other)
    {
        var byMidi = Midi.CompareTo(other.Midi);
        return byMidi != 0 ? byMidi : DiatonicIndex.CompareTo(other.DiatonicIndex);
    }

    /// <summary>예: "A4", "C♯5", "B♭4".</summary>
    public override string ToString() => $"{Letter}{Accidental.Symbol()}{Octave}";

    /// <summary>MIDI 번호를 ♯ 또는 ♭ 표기로 바꾼다.</summary>
    public static Pitch FromMidi(int midi, bool preferFlat = false)
    {
        var octave = midi / 12 - 1;
        var semitone = midi % 12;
        return semitone switch
        {
            0 => new(ENoteLetter.C, octave),
            1 => preferFlat ? new(ENoteLetter.D, EAccidental.Flat, octave) : new(ENoteLetter.C, EAccidental.Sharp, octave),
            2 => new(ENoteLetter.D, octave),
            3 => preferFlat ? new(ENoteLetter.E, EAccidental.Flat, octave) : new(ENoteLetter.D, EAccidental.Sharp, octave),
            4 => new(ENoteLetter.E, octave),
            5 => new(ENoteLetter.F, octave),
            6 => preferFlat ? new(ENoteLetter.G, EAccidental.Flat, octave) : new(ENoteLetter.F, EAccidental.Sharp, octave),
            7 => new(ENoteLetter.G, octave),
            8 => preferFlat ? new(ENoteLetter.A, EAccidental.Flat, octave) : new(ENoteLetter.G, EAccidental.Sharp, octave),
            9 => new(ENoteLetter.A, octave),
            10 => preferFlat ? new(ENoteLetter.B, EAccidental.Flat, octave) : new(ENoteLetter.A, EAccidental.Sharp, octave),
            _ => new(ENoteLetter.B, octave),
        };
    }

    /// <summary>"A4", "C#5", "C♯5", "Bb4", "B♭4" 형식을 읽는다.</summary>
    public static Pitch Parse(string text) =>
        TryParse(text, out var pitch) ? pitch : throw new FormatException($"Invalid pitch: '{text}'.");

    public static bool TryParse([NotNullWhen(true)] string? text, out Pitch pitch)
    {
        pitch = default;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var span = text.AsSpan().Trim();
        if (!Enum.TryParse(span[..1], ignoreCase: true, out ENoteLetter letter) || !char.IsLetter(span[0]))
        {
            return false;
        }

        var rest = span[1..];
        var accidental = EAccidental.Natural;
        if (rest.Length > 0 && rest[0] is '#' or '♯')
        {
            accidental = EAccidental.Sharp;
            rest = rest[1..];
        }
        else if (rest.Length > 0 && rest[0] is 'b' or '♭')
        {
            accidental = EAccidental.Flat;
            rest = rest[1..];
        }

        if (!int.TryParse(rest, out var octave) || octave is < 0 or > 9)
        {
            return false;
        }

        pitch = new Pitch(letter, accidental, octave);
        return true;
    }
}
