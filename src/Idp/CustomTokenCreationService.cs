using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Services;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Security.Claims;

namespace Idp;

public class CustomTokenCreationService : DefaultTokenCreationService
{
    public CustomTokenCreationService(IClock clock,
        IKeyMaterialService keys,
        IdentityServerOptions options,
        ILogger<DefaultTokenCreationService> logger)
        : base(clock, keys, options, logger)
    {
    }

    protected override Task<string> CreatePayloadAsync(Token token)
    {
        var clientName = token.ClientId;
        if ((clientName == "mobile-dpop-client") || (clientName == "onboarding-user-client"))
        {
            token.Claims.Add(new Claim("custom1", "custom1Value"));
        }

        return base.CreatePayloadAsync(token);
    }
}
