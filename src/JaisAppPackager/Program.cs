using CliFx;
using JaisAppPackager;
using Microsoft.Extensions.DependencyInjection;

ServiceProvider serviceProvider = Startup.BuildServiceProvider();

await new CliApplicationBuilder()
    .SetTitle("JAIS App Packager CLI")
    .SetDescription("Create and package JAIS apps.")
    .UseTypeActivator(serviceProvider.GetService)
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();