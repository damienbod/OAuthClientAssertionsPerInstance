using Duende.AccessTokenManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace ConsoleDPoPClientAssertions;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Title = "DPoP client with client assertions";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()

            .ConfigureServices((services) =>
            {
                services.AddDistributedMemoryCache();

                services.AddScoped<IClientAssertionService, ClientAssertionService>();
                // https://docs.duendesoftware.com/foss/accesstokenmanagement/advanced/client_assertions/

                services.AddClientCredentialsTokenManagement()
                    .AddClient("mobile-dpop-client", client =>
                    {
                        client.TokenEndpoint = "https://localhost:5001/connect/token";

                        client.ClientId = "mobile-dpop-client";
                        // Using client assertion
                        //client.ClientSecret = "905e4892-7610-44cb-a122-6209b38c882f";

                        client.Scope = "scope-dpop";
                        client.DPoPJsonWebKey = CreateDPoPKey();
                    });

                services.AddClientCredentialsHttpClient("mobile-dpop-client", "mobile-dpop-client", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5005/");
                });

                services.AddHostedService<DPoPClient>();
            });

        return host;
    }

    private static string CreateDPoPKey()
    {
        var key = new RsaSecurityKey(RSA.Create(2048));
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
        jwk.Alg = "PS256";
        var jwkJson = JsonSerializer.Serialize(jwk);
        return jwkJson;
    }
}