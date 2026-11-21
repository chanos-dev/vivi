namespace Vivi.Core.Music;

/// <summary>손가락을 기본 자리에서 어떻게 짚는가.</summary>
public enum EFingerReach
{
    /// <summary>낮게(♭ 쪽, 너트 방향으로 반음).</summary>
    Low = -1,

    /// <summary>기본 자리(내추럴).</summary>
    Natural = 0,

    /// <summary>높게(♯ 쪽, 브리지 방향으로 반음).</summary>
    High = 1,
}
