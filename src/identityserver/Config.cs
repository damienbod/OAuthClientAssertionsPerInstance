using Duende.IdentityServer.Models;

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

    public static IEnumerable<Client> Clients =>
        new Client[]
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
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedScopes = { "mobile" }
            }
        };
}
