namespace Vivi.Services;

/// <summary>
/// 앱 전역 설정을 보관하고 변경 시 구독자에게 알린다.
/// </summary>
public sealed class SettingsService(LocalStorage storage)
{
    private const string StorageKey = "settings";

    public AppSettings Current { get; private set; } = new();

    public event Action? Changed;

    public async Task LoadAsync()
    {
        Current = await storage.GetAsync<AppSettings>(StorageKey) ?? new AppSettings();
    }

    public async Task UpdateAsync(Func<AppSettings, AppSettings> update)
    {
        Current = update(Current);
        await storage.SetAsync(StorageKey, Current);
        Changed?.Invoke();
    }
}
