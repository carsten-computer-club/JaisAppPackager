using System.Reflection;
using CliFx;
using JaisAppPackager;
using Microsoft.Extensions.DependencyInjection;

ServiceProvider serviceProvider = Startup.BuildServiceProvider();

string? version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

await new CliApplicationBuilder()
    .SetTitle("JAIS App Packager CLI")
    .SetDescription("Create and package JAIS apps.")
    .UseTypeActivator(serviceProvider.GetService)
    .AddCommandsFromThisAssembly()
    .SetVersion(version ?? "version unknown")
    .SetExecutableName("jap")
    .Build()
    .RunAsync();