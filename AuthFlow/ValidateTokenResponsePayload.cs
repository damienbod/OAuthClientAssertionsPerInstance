using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthFlow;

public static class ValidateTokenResponsePayload
{
    public static (bool Valid, string Reason, string Error) IsValid(DeviceRegistrationResponse deviceRegistrationResponse, AuthFlowConfiguration oauthTokenExchangeConfiguration)
    {
        if (!deviceRegistrationResponse.TokenType.Equals("fp+jwt"))
        {
            return (false, "token_type parameter has an incorrect value, expected: fp+jwt",
                OAuthConsts.ERROR_UNSUPPORTED_GRANT_TYPE);
        };

        return (true, string.Empty, string.Empty);
    }

    public async static Task<(bool Valid, string Reason, ClaimsIdentity? ClaimsIdentity)> ValidateTokenAndSignature(
        string jwtToken,
        AuthFlowConfiguration authFlowConfiguration,
        ICollection<SecurityKey> signingKeys)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateIssuer = true,
                ValidIssuer = authFlowConfiguration.TokenAuthority,
                ValidateAudience = true,
                ValidAudience = authFlowConfiguration.ClientId
            };

            var tokenValidator = new JsonWebTokenHandler
            {
                MapInboundClaims = false
            };

            var tokenValidationResult = await tokenValidator.ValidateTokenAsync(jwtToken, validationParameters);

            return (true, string.Empty, tokenValidationResult.ClaimsIdentity);
        }
        catch (Exception ex)
        {
            return (false, $"FP Token Authorization failed {ex.Message}", null);
        }
    }

    public static string GetAuthSession(ClaimsIdentity claimsIdentity)
    {
        var auth_session = claimsIdentity.Claims.FirstOrDefault(t => t.Type == "auth_session");
        return auth_session?.Value ?? string.Empty;
    }

    public static string GetNonce(ClaimsIdentity claimsIdentity)
    {
        var nonce = claimsIdentity.Claims.FirstOrDefault(t => t.Type == "nonce");
        return nonce?.Value ?? string.Empty;
    }
}
