namespace Vivi.Core.Music;

/// <summary>
/// 왼손 포지션. 0은 반 포지션(half position), 1~<see cref="MaxNumber"/>는 해당 포지션이다.
/// </summary>
public readonly record struct Position : IComparable<Position>
{
    public const int HalfNumber = 0;
    public const int MaxNumber = 10;

    public Position(int number)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, MaxNumber);
        Number = number;
    }

    public int Number { get; }

    public bool IsHalf => Number == HalfNumber;

    public static Position Half { get; } = new(HalfNumber);

    public static Position First { get; } = new(1);

    /// <summary>반 포지션부터 최고 포지션까지 순서대로.</summary>
    public static IReadOnlyList<Position> All { get; } =
        Enumerable.Range(HalfNumber, MaxNumber + 1).Select(n => new Position(n)).ToArray();

    public int CompareTo(Position other) => Number.CompareTo(other.Number);

    /// <summary>"½" 또는 "1"~"10".</summary>
    public override string ToString() => IsHalf ? "½" : Number.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
