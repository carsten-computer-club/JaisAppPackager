using JaisAppPackager.Commands;
using JaisAppPackager.Services.Shell;
using Microsoft.Extensions.DependencyInjection;

namespace JaisAppPackager;

public class Startup
{
    public static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        return services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<NewCommand>();
        services.AddSingleton<PackageCommand>();
        services.AddSingleton<InstallCommand>();
        services.AddSingleton<IShellService, ShellService>();
    }
}