using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope-dpop"),
            new ApiScope("IdentityServer.Configuration"),
            new ApiScope("mobile")
        };

    public static IEnumerable<Client> Clients(IWebHostEnvironment environment)
    {
        //var privatePem = File.ReadAllText(Path.Combine(environment.ContentRootPath, 
        //    "rsa256-private.pem"));
        var publicPem = File.ReadAllText(Path.Combine(environment.ContentRootPath, "rsa256-public.pem"));
        var rsaCertificate = X509Certificate2.CreateFromPem(publicPem);
        // var rsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
        //var rsaCertificateKey = new RsaSecurityKey(rsaCertificate.GetRSAPrivateKey());

        var key = new X509SecurityKey(rsaCertificate);
        var jwk = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, true);
       
        return new Client[]
            {
                new Client
                {
                    ClientId = "cc-dpop",
                    ClientSecrets = { new Secret("de".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope-dpop" }
                },
                new Client
                {
                    ClientId = "mobile-client",
                    ClientName = "Mobile client", 

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = 
                    [
                        new Secret
                        {
                            // base64 encoded X.509 certificate
                            Type = IdentityServerConstants.SecretTypes.JsonWebKey,

                            Value = JsonSerializer.Serialize(jwk)
                        }
                    ],

                    AllowedScopes = { "mobile", "scope-dpop" }
                }
        };
    }

    public static string ConvertPemToBase64(string pemString)
    {
        // Remove PEM headers and footers
        var base64String = pemString
            .Replace("-----BEGIN CERTIFICATE-----", string.Empty)
            .Replace("-----END CERTIFICATE-----", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty);

        return base64String;
    }
}
