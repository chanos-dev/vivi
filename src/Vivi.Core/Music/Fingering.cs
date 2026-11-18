namespace Vivi.Core.Music;

/// <summary>
/// 한 음을 내는 운지: 현 + 포지션 + 손가락 번호.
/// 개방현은 <see cref="Finger"/> = 0이고 <see cref="Position"/>은 null이다(어느 포지션에서든 같다).
/// </summary>
public readonly record struct Fingering(EViolinString String, Position? Position, int Finger)
{
    public const int OpenFinger = 0;

    public bool IsOpenString => Finger == OpenFinger;

    public static Fingering Open(EViolinString violinString) => new(violinString, null, OpenFinger);

    /// <summary>디버그용 표기. 예: "A open", "D pos1 f4", "A pos½ f1". 화면 표기는 UI에서 만든다.</summary>
    public override string ToString() =>
        IsOpenString ? $"{String} open" : $"{String} pos{Position} f{Finger}";
}
