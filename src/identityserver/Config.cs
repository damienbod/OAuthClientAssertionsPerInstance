using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("DPoPApiDefaultScope"),
            new ApiScope("mobile")
        ];

    public static IEnumerable<Client> Clients(IWebHostEnvironment environment)
    {
        var publicPem = File.ReadAllText(Path.Combine(environment.ContentRootPath, "rsa256-public.pem"));
        var rsaCertificate = X509Certificate2.CreateFromPem(publicPem);

        return
        [
            new Client
            {
                ClientId = "mobile-dpop-client",
                ClientName = "Mobile dpop client",
                RequireDPoP = true,

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                [
                    new Secret
                    {
                        // X509 cert base64-encoded
                        Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                        Value = Convert.ToBase64String(rsaCertificate.GetRawCertData())
                    }
                ],

                AllowedScopes = { "DPoPApiDefaultScope" }
            }
        ];
    }
}
