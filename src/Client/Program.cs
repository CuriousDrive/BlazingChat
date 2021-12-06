using BlazingChat.Client;
using BlazingChat.Shared.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

#region ConfigureServices

var baseAddress = builder.Configuration["BaseAddress"] ??
                     throw new NullReferenceException("BaseAddress is missing in configuration");

builder.Services.AddBlazingChat(baseAddress);

#endregion

await builder.Build().RunAsync();
