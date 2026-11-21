using System.Globalization;

namespace Vivi.Core.Music;

/// <summary>왼손 포지션. 1~<see cref="MaxNumber"/>.</summary>
public readonly record struct Position : IComparable<Position>
{
    public const int MinNumber = 1;
    public const int MaxNumber = 10;

    public Position(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, MinNumber);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, MaxNumber);
        Number = number;
    }

    public int Number { get; }

    public static Position First { get; } = new(MinNumber);

    /// <summary>1포지션부터 최고 포지션까지 순서대로.</summary>
    public static IReadOnlyList<Position> All { get; } =
        Enumerable.Range(MinNumber, MaxNumber).Select(n => new Position(n)).ToArray();

    public int CompareTo(Position other) => Number.CompareTo(other.Number);

    public override string ToString() => Number.ToString(CultureInfo.InvariantCulture);
}
