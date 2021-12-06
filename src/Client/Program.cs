using BlazingChat.Client;
using BlazingChat.Client.Handlers;
using BlazingChat.Client.Logging;
using BlazingChat.ViewModels;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

#region ConfigureServices

var baseAddress = builder.Configuration["BaseAddress"] ??
                     throw new NullReferenceException("BaseAddress is missing in configuration");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });

builder.Services.ConfigureAll<HttpClientFactoryOptions>(options =>
    options.HttpMessageHandlerBuilderActions.Add(handlerBuilder =>
        handlerBuilder.AdditionalHandlers.Add(
            handlerBuilder.Services.GetRequiredService<CustomAuthorizationHandler>())));

var clientConfigurator = void (HttpClient client) => client.BaseAddress = new Uri(baseAddress);

//transactional named http clients
builder.Services.AddHttpClient<IProfileViewModel, ProfileViewModel>("ProfileViewModelClient", clientConfigurator);
builder.Services.AddHttpClient<IContactsViewModel, ContactsViewModel>("ContactsViewModelClient", clientConfigurator);
builder.Services.AddHttpClient<ISettingsViewModel, SettingsViewModel>("SettingsViewModelClient", clientConfigurator);
builder.Services.AddHttpClient<IAssignRolesViewModel, AssignRolesViewModel>("AssignRolesViewModel", clientConfigurator);

//authentication http clients
builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>("LoginViewModelClient", clientConfigurator);
builder.Services.AddHttpClient<IRegisterViewModel, RegisterViewModel>("RegisterViewModelClient", clientConfigurator);

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<CustomAuthorizationHandler>();
builder.Services.AddLogging(logging =>
{
    var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
    var authStateProvider = builder.Services.BuildServiceProvider().GetRequiredService<AuthenticationStateProvider>();
    logging.SetMinimumLevel(LogLevel.Error);
    logging.AddProvider(new ApplicationLoggerProvider(httpClient, authStateProvider));
});
builder.Services.AddBlazoredToast();

#endregion

await builder.Build().RunAsync();
