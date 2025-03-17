using Duende.AccessTokenManagement;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleDPoPClientAssertions;

public class ClientAssertionService : IClientAssertionService
{
    private readonly IOptionsSnapshot<ClientCredentialsClient> _options;
    private readonly KeySessionService _keySessionService;

    public ClientAssertionService(IOptionsSnapshot<ClientCredentialsClient> options,
        KeySessionService keySessionService)
    {
        _options = options;
        _keySessionService = keySessionService;
    }

    public async Task<ClientAssertion?> GetClientAssertionAsync(
      string? clientName = null, TokenRequestParameters? parameters = null)
    {
        if (clientName == "mobile-dpop-client")
        {
            var key = await _keySessionService.CreateGetSessionAsync();
            var options = _options.Get(clientName);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = options.ClientId,
                Audience = options.TokenEndpoint,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = key.SigningCredentials,

                Claims = new Dictionary<string, object>
                {
                    { JwtClaimTypes.JwtId, Guid.NewGuid().ToString() },
                    { JwtClaimTypes.Subject, options.ClientId! },
                    { JwtClaimTypes.IssuedAt, DateTime.UtcNow.ToEpochTime() },
                    { "AppSessionId", key.SessionId! }
                }
            };

            var handler = new JsonWebTokenHandler();
            var jwt = handler.CreateToken(descriptor);

            return await Task.FromResult<ClientAssertion?>(new ClientAssertion
            {
                Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                Value = jwt
            });
        }

        return await Task.FromResult<ClientAssertion?>(null);
    }
}

