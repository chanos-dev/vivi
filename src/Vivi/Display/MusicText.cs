using Vivi.Core.Music;

namespace Vivi.Display;

/// <summary>음악 도메인 값을 화면용 한국어 문구로 바꾼다.</summary>
public static class MusicText
{
    public static string PositionName(Position position) => $"{position.Number}포지션";

    public static string StringName(EViolinString violinString) => $"{violinString}현";

    public static string FingerName(int finger) => $"{finger}번";

    /// <summary>예: "A현 개방", "D현 · 3포지션 · 4번".</summary>
    public static string FingeringText(Fingering fingering) =>
        fingering.IsOpenString
            ? $"{StringName(fingering.String)} 개방"
            : $"{StringName(fingering.String)} · {PositionName(fingering.Position!.Value)} · {FingerName(fingering.Finger)}";

    /// <summary>CSS 톤 키: "g", "d", "a", "e".</summary>
    public static string ToneKey(EViolinString violinString) => violinString.ToString().ToLowerInvariant();
}
