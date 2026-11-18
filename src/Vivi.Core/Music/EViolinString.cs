namespace Vivi.Core.Music;

/// <summary>바이올린의 네 현. 낮은 현부터.</summary>
public enum EViolinString
{
    G = 0,
    D = 1,
    A = 2,
    E = 3,
}

public static class EViolinStringExtensions
{
    public static IReadOnlyList<EViolinString> All { get; } =
        [EViolinString.G, EViolinString.D, EViolinString.A, EViolinString.E];

    /// <summary>개방현 음. G3, D4, A4, E5.</summary>
    public static Pitch OpenPitch(this EViolinString violinString) => violinString switch
    {
        EViolinString.G => new(ENoteLetter.G, 3),
        EViolinString.D => new(ENoteLetter.D, 4),
        EViolinString.A => new(ENoteLetter.A, 4),
        EViolinString.E => new(ENoteLetter.E, 5),
        _ => throw new ArgumentOutOfRangeException(nameof(violinString), violinString, null),
    };
}
