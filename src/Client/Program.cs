using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazingChat.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using BlazingChat.Client.Logging;
using Blazored.Toast;

namespace BlazingChat.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddLogging(logging => {
                var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
                logging.SetMinimumLevel(LogLevel.Error);
                logging.ClearProviders();
                logging.AddProvider(new ApplicationLoggerProvider(httpClient));
            });

            LoadHttpClients(builder);
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            
            builder.Services.AddBlazoredToast();

            await builder.Build().RunAsync();
        }

        public static void LoadHttpClients(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddHttpClient<IProfileViewModel, ProfileViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddHttpClient<IContactsViewModel, ContactsViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddHttpClient<ISettingsViewModel, SettingsViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
        }
    }
}
