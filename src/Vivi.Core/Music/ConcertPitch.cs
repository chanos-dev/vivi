namespace Vivi.Core.Music;

/// <summary>
/// 기준음(A4)의 주파수. 모든 음높이 계산의 기준이며 평균율을 쓴다.
/// </summary>
public sealed record ConcertPitch
{
    public const double DefaultHz = 440.0;
    public const double MinHz = 415.0;
    public const double MaxHz = 466.0;

    private const int A4Midi = 69;

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

    /// <summary>MIDI 번호에 해당하는 평균율 주파수(Hz).</summary>
    public double FrequencyOf(int midi) => A4Hz * Math.Pow(2, (midi - A4Midi) / 12.0);

    public double FrequencyOf(Pitch pitch) => FrequencyOf(pitch.Midi);

    /// <summary>주파수에 가장 가까운 MIDI 번호.</summary>
    public int NearestMidi(double hz)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hz);
        return (int)Math.Round(A4Midi + 12 * Math.Log2(hz / A4Hz));
    }

    /// <summary>
    /// <paramref name="hz"/>가 <paramref name="targetHz"/>보다 몇 센트 높은지(음수면 낮음). 반음 = 100센트.
    /// </summary>
    public static double CentsBetween(double hz, double targetHz)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hz);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetHz);
        return 1200 * Math.Log2(hz / targetHz);
    }

    /// <summary>목표 음 대비 센트 편차.</summary>
    public double CentsFrom(double hz, Pitch target) => CentsBetween(hz, FrequencyOf(target));
}
