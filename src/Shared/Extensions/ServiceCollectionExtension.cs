using BlazingChat.Client;
using BlazingChat.Client.Handlers;
using BlazingChat.Client.Logging;
using BlazingChat.ViewModels;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace BlazingChat.Shared.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBlazingChat(this IServiceCollection services, string baseAddress)
        {
            // blazored services
            services.AddBlazoredLocalStorage();
            services.AddBlazoredToast();

            // authetication & authorization
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddTransient<CustomAuthorizationHandler>();

            // configuring http clients
            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });
            services.ConfigureAll<HttpClientFactoryOptions>(options =>
                options.HttpMessageHandlerBuilderActions.Add(handlerBuilder =>
                    handlerBuilder.AdditionalHandlers.Add(
                        handlerBuilder.Services.GetRequiredService<CustomAuthorizationHandler>())));

            var clientConfigurator = void (HttpClient client) => client.BaseAddress = new Uri(baseAddress);

            // transactional named http clients
            services.AddHttpClient<IProfileViewModel, ProfileViewModel>("ProfileViewModelClient", clientConfigurator);
            services.AddHttpClient<IContactsViewModel, ContactsViewModel>("ContactsViewModelClient", clientConfigurator);
            services.AddHttpClient<ISettingsViewModel, SettingsViewModel>("SettingsViewModelClient", clientConfigurator);
            services.AddHttpClient<IAssignRolesViewModel, AssignRolesViewModel>("AssignRolesViewModel", clientConfigurator);

            // authentication http clients
            services.AddHttpClient<ILoginViewModel, LoginViewModel>("LoginViewModelClient", clientConfigurator);
            services.AddHttpClient<IRegisterViewModel, RegisterViewModel>("RegisterViewModelClient", clientConfigurator);

            // logging
            services.AddLogging(logging =>
            {
                var httpClient = services.BuildServiceProvider().GetRequiredService<HttpClient>();
                var authStateProvider = services.BuildServiceProvider().GetRequiredService<AuthenticationStateProvider>();
                logging.SetMinimumLevel(LogLevel.Error);
                logging.AddProvider(new ApplicationLoggerProvider(httpClient, authStateProvider));
            });

            return services;
        }
    }
}
