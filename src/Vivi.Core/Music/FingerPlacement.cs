namespace Vivi.Core.Music;

/// <summary>
/// 지판 위 한 자리: 어느 현에서 개방현으로부터 몇 반음 위를 어느 손가락으로(기본·낮게·높게) 짚어 어떤 음을 내는지.
/// </summary>
public readonly record struct FingerPlacement(
    EViolinString String, int Finger, Pitch Pitch, int Semitones, EFingerReach Reach)
{
    /// <summary>그 손가락의 기본 자리(내추럴)인가.</summary>
    public bool IsPrimary => Reach == EFingerReach.Natural;
}
