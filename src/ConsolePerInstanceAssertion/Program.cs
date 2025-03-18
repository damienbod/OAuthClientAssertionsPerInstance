using Duende.AccessTokenManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace ConsolePerInstanceAssertion;

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

                var rsa2048 = RSA.Create(2048);
                services.AddSingleton<KeySessionService>(t => new KeySessionService(rsa2048));

                services.AddScoped<IClientAssertionService, ClientAssertionService>();
                // https://docs.duendesoftware.com/foss/accesstokenmanagement/advanced/client_assertions/

                services.AddClientCredentialsTokenManagement()
                    .AddClient("mobile-dpop-client", client =>
                    {
                        client.TokenEndpoint = "https://localhost:5101/connect/token";

                        client.ClientId = "mobile-dpop-client";
                        // Using client assertion
                        //client.ClientSecret = "905e4892-7610-44cb-a122-6209b38c882f";

                        client.Scope = "DPoPApiDefaultScope";
                        client.DPoPJsonWebKey = CreateDPoPKey(rsa2048);
                    })
                    .AddClient("onboarding-user-client", client =>
                    {
                        client.TokenEndpoint = "https://localhost:5101/connect/token";

                        client.ClientId = "onboarding-user-client";
                        // Using client assertion
                        //client.ClientSecret = "905e4892-7610-44cb-a122-6209b38c882f";

                        client.Scope = "OnboardingUserScope";
                        client.DPoPJsonWebKey = CreateDPoPKey(rsa2048);
                    });

                services.AddClientCredentialsHttpClient("mobile-dpop-client", "mobile-dpop-client", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5105/");
                });
                services.AddClientCredentialsHttpClient("onboarding-user-client", "onboarding-user-client", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5101/");
                });

                services.AddHostedService<DPoPClient>();
            });

        return host;
    }

    private static string CreateDPoPKey(RSA rsa2048)
    {
        var key = new RsaSecurityKey(rsa2048);
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
        jwk.Alg = "PS256";
        var jwkJson = JsonSerializer.Serialize(jwk);
        return jwkJson;
    }
}