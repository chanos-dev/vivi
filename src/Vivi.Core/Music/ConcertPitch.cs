namespace Vivi.Core.Music;

/// <summary>
/// 기준음(A4)의 주파수. 모든 음 높이 계산의 기준이 된다.
/// </summary>
public sealed record ConcertPitch
{
    public const double DefaultHz = 440.0;
    public const double MinHz = 415.0;
    public const double MaxHz = 466.0;

    public static ConcertPitch Default { get; } = new(DefaultHz);

    public ConcertPitch(double a4Hz)
    {
        if (double.IsNaN(a4Hz) || a4Hz < MinHz || a4Hz > MaxHz)
        {
            throw new ArgumentOutOfRangeException(nameof(a4Hz), a4Hz, $"A4 must be between {MinHz} and {MaxHz} Hz.");
        }

        A4Hz = a4Hz;
    }

    public double A4Hz { get; }

    public static bool IsValid(double a4Hz) => a4Hz is >= MinHz and <= MaxHz;
}
