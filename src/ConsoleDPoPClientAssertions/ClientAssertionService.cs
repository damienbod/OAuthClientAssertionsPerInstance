using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Duende.IdentityModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleDPoPClientAssertions;

public class ClientAssertionService : IClientAssertionService
{
    private readonly IOptionsSnapshot<ClientCredentialsClient> _options;

    public ClientAssertionService(IOptionsSnapshot<ClientCredentialsClient> options)
    {
        _options = options;
    }

    public Task<ClientAssertion?> GetClientAssertionAsync(
      string? clientName = null, TokenRequestParameters? parameters = null)
    {
        if (clientName == "mobile-dpop-client")
        {
            // client assertion
            var privatePem = File.ReadAllText(Path.Combine("", "rsa256-private.pem"));
            var publicPem = File.ReadAllText(Path.Combine("", "rsa256-public.pem"));
            var rsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
            var rsaCertificateKey = new RsaSecurityKey(rsaCertificate.GetRSAPrivateKey());
            var signingCredentials = new SigningCredentials(new X509SecurityKey(rsaCertificate), "RS256");

            var options = _options.Get(clientName);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = options.ClientId,
                Audience = options.TokenEndpoint,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = signingCredentials,

                Claims = new Dictionary<string, object>
                {
                    { JwtClaimTypes.JwtId, Guid.NewGuid().ToString() },
                    { JwtClaimTypes.Subject, options.ClientId! },
                    { JwtClaimTypes.IssuedAt, DateTime.UtcNow.ToEpochTime() }
                }
            };

            var handler = new JsonWebTokenHandler();
            var jwt = handler.CreateToken(descriptor);

            return Task.FromResult<ClientAssertion?>(new ClientAssertion
            {
                Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                Value = jwt
            });
        }

        return Task.FromResult<ClientAssertion?>(null);
    }
}

