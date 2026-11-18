using Vivi.Core.Music;

namespace Vivi.Services;

/// <summary>
/// 사용자 설정. localStorage에 JSON으로 저장된다.
/// </summary>
public sealed record AppSettings
{
    public double A4Hz { get; init; } = ConcertPitch.DefaultHz;

    public ConcertPitch ConcertPitch =>
        ConcertPitch.IsValid(A4Hz) ? new ConcertPitch(A4Hz) : ConcertPitch.Default;
}
