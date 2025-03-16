using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace IdpPreInstanceAssertion;

public class PerInstancePrivateKeyJwtSecretValidator : ISecretValidator
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IReplayCache _replayCache;
    private readonly ILogger _logger;
    private readonly PublicKeyService _publicKeyService;
    private const string Purpose = nameof(PrivateKeyJwtSecretValidator);

    /// <summary>
    /// Instantiates an instance of private_key_jwt secret validator
    /// </summary>
    public PerInstancePrivateKeyJwtSecretValidator(IHttpContextAccessor contextAccessor, 
        IReplayCache replayCache, 
        ILogger<PrivateKeyJwtSecretValidator> logger,
        PublicKeyService publicKeyService)
    {
        _contextAccessor = contextAccessor;
        _replayCache = replayCache;
        _logger = logger;
        _publicKeyService = publicKeyService;
    }

    /// <summary>
    /// Validates a secret
    /// </summary>
    /// <param name="secrets">The stored secrets.</param>
    /// <param name="parsedSecret">The received secret.</param>
    /// <returns>
    /// A validation result
    /// </returns>
    /// <exception cref="System.ArgumentException">ParsedSecret.Credential is not a JWT token</exception>
    public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
    {
        var fail = new SecretValidationResult { Success = false };
        var success = new SecretValidationResult { Success = true };

        if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.JwtBearer)
        {
            return fail;
        }

        if (!(parsedSecret.Credential is string jwtTokenString))
        {
            _logger.LogError("ParsedSecret.Credential is not a string.");
            return fail;
        }

        var validAudiences = new[]
        {
            "https://localhost:5101/connect/token"
        };

        var sessionId = await GetSessionId(jwtTokenString);
        List<SecurityKey> trustedKeys;
        try
        {
            trustedKeys = [_publicKeyService.GetPublicSecurityKey(sessionId)];
            //trustedKeys = await secrets.GetKeysAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not parse secrets");
            return fail;
        }

        if (!trustedKeys.Any())
        {
            _logger.LogError("There are no keys available to validate client assertion.");
            return fail;
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKeys = trustedKeys,
            ValidateIssuerSigningKey = true,

            ValidIssuer = parsedSecret.Id,
            ValidateIssuer = true,

            ValidAudiences = validAudiences,
            ValidateAudience = true,

            RequireSignedTokens = true,
            RequireExpirationTime = true,

            ClockSkew = TimeSpan.FromMinutes(5)
        };
        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(jwtTokenString, tokenValidationParameters, out var token);

            var jwtToken = (JwtSecurityToken)token;
            if (jwtToken.Subject != jwtToken.Issuer)
            {
                _logger.LogError("Both 'sub' and 'iss' in the client assertion token must have a value of client_id.");
                return fail;
            }

            var exp = jwtToken.Payload.Expiration;
            if (!exp.HasValue)
            {
                _logger.LogError("exp is missing.");
                return fail;
            }

            var jti = jwtToken.Payload.Jti;
            if (string.IsNullOrWhiteSpace(jti))
            {
                _logger.LogError("jti is missing.");
                return fail;
            }

            if (await _replayCache.ExistsAsync(Purpose, jti))
            {
                _logger.LogError("jti is found in replay cache. Possible replay attack.");
                return fail;
            }
            else
            {
                await _replayCache.AddAsync(Purpose, jti, DateTimeOffset.FromUnixTimeSeconds(exp.Value).AddMinutes(5));
            }

            return success;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "JWT token validation error");
            return fail;
        }
    }

    public static async Task<string> GetSessionId(string jwtTokenString)
    {
        // multi-tenant app, tenantId is not required in the first step
        // token is validated in second step.
        // TenantId is required for metaaddress to validate the signature
        // Explicit validation of allow tenant list is required
        var validationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = false,
            ValidateLifetime = false,
            RequireSignedTokens = false,
            ValidateIssuerSigningKey = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            SignatureValidator = (token, _) => new JsonWebToken(token)
        };

        var tokenValidator = new JsonWebTokenHandler
        {
            MapInboundClaims = false
        };

        var tokenValidationResult = await tokenValidator.ValidateTokenAsync(jwtTokenString, validationParameters);

        var sessionIdClaim = tokenValidationResult.Claims.FirstOrDefault(c => c.Key == "AppSessionId");

        return sessionIdClaim.Value.ToString();
    }
}