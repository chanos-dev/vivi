using System.Text.Json;
using Microsoft.JSInterop;

namespace Vivi.Services;

/// <summary>
/// 브라우저 localStorage에 JSON 값을 읽고 쓰는 얇은 래퍼.
/// </summary>
public sealed class LocalStorage(IJSRuntime js)
{
    private const string KeyPrefix = "vivi:";

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", KeyPrefix + key);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            // 저장 형식이 바뀌었거나 손상된 값은 무시하고 기본값을 쓴다.
            return default;
        }
    }

    public ValueTask SetAsync<T>(string key, T value) =>
        js.InvokeVoidAsync("localStorage.setItem", KeyPrefix + key, JsonSerializer.Serialize(value));
}
