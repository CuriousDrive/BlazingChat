using System;
using System.Net.Http;
using BlazingChat.Client;
using BlazingChat.Shared.Logging;
using BlazingChat.Shared.Services;
using BlazingChat.ViewModels;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Shared.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBlazingChat(this IServiceCollection services, 
            ApplicationSettings applicationSettings)
        {
            // blazored services
            services.AddBlazoredToast();
            services.AddBlazoredLocalStorage();

            // authetication & authorization
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<IAccessTokenService, WebAppAccessTokenService>();

            // configuring http clients
            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(applicationSettings.BaseAddress) });

            // transactional named http clients
            var clientConfigurator = void (HttpClient client) => client.BaseAddress = new Uri(applicationSettings.BaseAddress);
            services.AddHttpClient<IProfileViewModel, ProfileViewModel>("ProfileViewModelClient", clientConfigurator);
            services.AddHttpClient<IContactsViewModel, ContactsViewModel>("ContactsViewModelClient", clientConfigurator);
            services.AddHttpClient<ISettingsViewModel, SettingsViewModel>("SettingsViewModelClient", clientConfigurator);
            services.AddHttpClient<IAssignRolesViewModel, AssignRolesViewModel>("AssignRolesViewModel", clientConfigurator);

            // authentication http clients
            services.AddHttpClient<ILoginViewModel, LoginViewModel>("LoginViewModelClient", clientConfigurator);
            services.AddHttpClient<IRegisterViewModel, RegisterViewModel>("RegisterViewModelClient", clientConfigurator);

            // logging
            services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Error));
            services.AddSingleton<LogQueue>();
            services.AddSingleton<LogReader>();
            services.AddSingleton<LogWriter>();
            services.AddSingleton<ILoggerProvider, ApplicationLoggerProvider>();
            services.AddHttpClient("LoggerJob", c => c.BaseAddress = new Uri(applicationSettings.BaseAddress) );
            services.AddSingleton<LoggerJob>();

            return services;
        }
    }
}
