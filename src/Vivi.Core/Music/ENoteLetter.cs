namespace Vivi.Core.Music;

/// <summary>음 이름(글자). 값은 C부터의 온음계 순서다.</summary>
public enum ENoteLetter
{
    C = 0,
    D = 1,
    E = 2,
    F = 3,
    G = 4,
    A = 5,
    B = 6,
}

public static class ENoteLetterExtensions
{
    private static readonly int[] NaturalSemitones = [0, 2, 4, 5, 7, 9, 11];

    /// <summary>같은 옥타브의 C로부터의 반음 거리(내추럴 기준).</summary>
    public static int NaturalSemitone(this ENoteLetter letter) => NaturalSemitones[(int)letter];
}
