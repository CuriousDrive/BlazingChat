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
using Blazored.LocalStorage;
using BlazingChat.Client.Handlers;
using Microsoft.Extensions.Configuration;

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

            AddHttpClients(builder, builder.Configuration);

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddTransient<CustomAuthorizationHandler>();

            builder.Services.AddLogging(logging =>
            {
                var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
                var authenticationStateProvider = builder.Services.BuildServiceProvider().GetRequiredService<AuthenticationStateProvider>();
                logging.SetMinimumLevel(LogLevel.Error);
                logging.AddProvider(new ApplicationLoggerProvider(httpClient, authenticationStateProvider));
            });

            builder.Services.AddBlazoredToast();

            await builder.Build().RunAsync();
        }

        public static void AddHttpClients(WebAssemblyHostBuilder builder, IConfiguration configuration)
        {
            string baseAddress = configuration["BaseAddress"];

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

            //transactional named http clients
            builder.Services.AddHttpClient<IProfileViewModel, ProfileViewModel>
                ("ProfileViewModelClient", client => client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<CustomAuthorizationHandler>();

            builder.Services.AddHttpClient<IContactsViewModel, ContactsViewModel>
                ("ContactsViewModelClient", client => client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<CustomAuthorizationHandler>();

            builder.Services.AddHttpClient<ISettingsViewModel, SettingsViewModel>
                ("SettingsViewModelClient", client => client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<CustomAuthorizationHandler>();

            builder.Services.AddHttpClient<IAssignRolesViewModel, AssignRolesViewModel>
                ("AssignRolesViewModel", client => client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<CustomAuthorizationHandler>();

            //authentication http clients
            builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>
                ("LoginViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

            builder.Services.AddHttpClient<IRegisterViewModel, RegisterViewModel>
                ("RegisterViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

        }
    }
}
