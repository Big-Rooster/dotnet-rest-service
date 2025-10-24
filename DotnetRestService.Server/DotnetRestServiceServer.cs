using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OpenTelemetry.Logs;
using Serilog;
using DotnetRestService.Server.Services;

namespace DotnetRestService.Server;

public class DotnetRestServiceServer
{
    private string[] args = [];
    private WebApplication? app;

    public DotnetRestServiceServer Start()
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Allow ASPNETCORE_URLS environment variable to control binding
        // This supports both service-port and management-port from docker-compose.yml
        // Example: ASPNETCORE_URLS=http://+:8080;http://+:8081
        
        // Configure graceful shutdown
        builder.Host.ConfigureHostOptions(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
        });
        
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

    public DotnetRestServiceServer Stop()
    {
        if (app != null)
        {
            app.StopAsync().GetAwaiter().GetResult();
        }
        return this;
    }

    public DotnetRestServiceServer WithArguments(string[] args)
    {
        this.args = args;
        return this;
    }
    
    public DotnetRestServiceServer WithEphemeral()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Ephemeral");
        Environment.SetEnvironmentVariable("SPRING_PROFILES_ACTIVE", "ephemeral");
        return this;
    }

     public DotnetRestServiceServer WithRandomPorts()
    {
        // Use fixed test port for integration tests to avoid port discovery issues
        Environment.SetEnvironmentVariable("HTTP_PORT", "5031");
        return this;
    }

    
    public string? getHttpUrl()
    {
        if (app == null) return null;
        
        try
        {
            var serverAddresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
            if (serverAddresses?.Addresses == null || !serverAddresses.Addresses.Any())
            {
                return "http://localhost:5031";
            }
            
            var addresses = serverAddresses.Addresses.ToList();
            
            // Look for the HTTP endpoint - it should contain port 5031 or 15031 (test port)
            var httpAddress = addresses.FirstOrDefault(addr => addr.Contains("5031") || addr.Contains("15031"));
            
            // If not found by port, use the first address
            if (httpAddress == null && addresses.Count > 0)
            {
                httpAddress = addresses[0];
            }
            
            if (httpAddress != null)
            {
                // Replace 0.0.0.0 with localhost for client connections
                return httpAddress.Replace("0.0.0.0", "localhost").Replace("[::]", "localhost");
            }
            
            // Default fallback
            var testPort = Environment.GetEnvironmentVariable("HTTP_PORT");
            if (!string.IsNullOrEmpty(testPort) && testPort != "0")
            {
                return $"http://localhost:{testPort}";
            }
            
            return "http://localhost:5031";
        }
        catch
        {
            return "http://localhost:5031";
        }
    }
    
    public static async Task Main(string[] args)
    {
        var server = new DotnetRestServiceServer()
            .WithArguments(args);

        // Parse command-line arguments for special modes
        if (args.Contains("--ephemeral"))
        {
            server.WithEphemeral();
        }
        

        server.Start();

        // Simulate waiting for shutdown signal or some other condition
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
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
        }

        // Gracefully stop the application
        server.Stop();
    }
}