namespace PowerShellService;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Management;

public class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    private Process? _scriptProcess;

    public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            StartPowerShellScript();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void StartPowerShellScript()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "pwsh.exe",
            WorkingDirectory = "C:\\Program Files\\TS2",
            Arguments = "-ExecutionPolicy Bypass -File \"C:\\Program Files\\TS2\\ts2.ps1\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _scriptProcess = Process.Start(psi);
        if (_scriptProcess == null)
        {
            _logger.LogError("Failed to start PowerShell process.");
            return;
        }

        _scriptProcess.EnableRaisingEvents = true;
        _scriptProcess.Exited += (sender, args) =>
        {
            _logger.LogInformation("PowerShell script exited.");
        };
    }

public override async Task StopAsync(CancellationToken cancellationToken)
{
    try
    {
        var searcher = new ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'pwsh.exe'");
        foreach (ManagementObject obj in searcher.Get())
        {
            var commandLine = obj["CommandLine"]?.ToString() ?? "";
            var processId = Convert.ToInt32(obj["ProcessId"]);

            if (commandLine.Contains("ts2.ps1", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var proc = Process.GetProcessById(processId);
                    proc.Kill();
                    _logger.LogInformation("Killed PowerShell process running ts2.ps1 with PID {pid}", processId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to kill PowerShell process with PID {pid}", processId);
                }
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error while querying PowerShell processes via WMI.");
    }

    await base.StopAsync(cancellationToken);
    }

}