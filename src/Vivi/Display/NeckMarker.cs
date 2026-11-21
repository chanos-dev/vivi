using Vivi.Core.Music;

namespace Vivi.Display;

/// <summary>넥 위의 한 점. 같은 현·같은 자리(반음 거리)를 짚는 운지들을 묶는다(예: 높은 2번 C♯ = 낮은 3번 D♭).</summary>
public sealed record NeckMarker(EViolinString String, int Semitones, IReadOnlyList<FingerPlacement> Entries)
{
    /// <summary>기본 자리(내추럴)면 채운 점, 변형만 있으면 빈 점으로 그린다.</summary>
    public bool IsPrimary => Entries.Any(e => e.IsPrimary);

    /// <summary>점에 표시할 대표 운지.</summary>
    public FingerPlacement Lead => Entries.FirstOrDefault(e => e.IsPrimary, Entries[0]);

    public static IReadOnlyList<NeckMarker> Build(Position position) =>
        EViolinStringExtensions.All
            .SelectMany(s => Fingerboard.GetPlacements(s, position)
                .GroupBy(p => p.Semitones)
                .Select(g => new NeckMarker(s, g.Key, g.ToArray())))
            .ToArray();
}
