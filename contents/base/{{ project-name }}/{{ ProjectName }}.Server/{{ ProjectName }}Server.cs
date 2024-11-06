using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OpenTelemetry.Logs;
using Serilog;

namespace {{ ProjectName }}.Server;

public class {{ ProjectName }}Server
{
    private string[] args = [];
    private WebApplication? app;

    public {{ ProjectName }}Server Start()
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((content, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(builder.Configuration)
        );
        
        builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);
        app = builder.Build();
        startup.Configure(app);
        app.Start();

        return this;
    }

    public {{ ProjectName }}Server Stop()
    {
        app?.StopAsync().GetAwaiter().GetResult();
        return this;
    }

    public {{ ProjectName }}Server WithArguments(string[] args)
    {
        this.args = args;
        return this;
    }
    

     public {{ ProjectName }}Server WithRandomPorts()
    {
        Environment.SetEnvironmentVariable("Kestrel__Endpoints__Server__Url", "http://127.0.0.1:0");
        Environment.SetEnvironmentVariable("Kestrel__Endpoints__Management__Url", "http://127.0.0.1:0");
        return this;
    }

    public string? GetGraphQlUrl()
    {
        var serverAddresses = app?.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
        return serverAddresses?.Addresses.FirstOrDefault();
    }
    
    public static async Task Main(string[] args)
    {
        var {{ ProjectName }}Server = new {{ ProjectName }}Server()
            .WithArguments(args);

        {{ ProjectName }}Server.Start();

        // Simulate waiting for shutdown signal or some other condition
        var cancellationTokenSource = new CancellationTokenSource();

        // Register an event to stop the app when Ctrl+C or another shutdown signal is received
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true; // Prevent the app from terminating immediately
            cancellationTokenSource.Cancel(); // Trigger the stop signal
        };

        try
        {
            // Wait indefinitely (or for a shutdown signal) by awaiting on the task
            await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            // The delay task is canceled when a shutdown signal is received
        }

        // Gracefully stop the application
        {{ ProjectName }}Server.Stop();
    }
}