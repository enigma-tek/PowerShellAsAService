using PowerShellService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Enable Windows Service support
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Truststore2";
});

// Register your background service
builder.Services.AddHostedService<WindowsBackgroundService>();

IHost host = builder.Build();
host.Run();