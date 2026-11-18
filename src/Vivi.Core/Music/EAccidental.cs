namespace Vivi.Core.Music;

/// <summary>임시표. 값은 반음 변화량이다.</summary>
public enum EAccidental
{
    Flat = -1,
    Natural = 0,
    Sharp = 1,
}

public static class EAccidentalExtensions
{
    public static string Symbol(this EAccidental accidental) => accidental switch
    {
        EAccidental.Flat => "♭",
        EAccidental.Sharp => "♯",
        _ => string.Empty,
    };
}
