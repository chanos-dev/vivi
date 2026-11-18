using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vivi;
using Vivi.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<LocalStorage>();
builder.Services.AddSingleton<SettingsService>();

var host = builder.Build();

// 첫 렌더링 전에 저장된 설정을 불러와 모든 화면이 같은 기준음으로 시작하게 한다.
await host.Services.GetRequiredService<SettingsService>().LoadAsync();

await host.RunAsync();
