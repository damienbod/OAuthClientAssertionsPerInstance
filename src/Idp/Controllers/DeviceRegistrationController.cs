using AuthFlow;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Idp.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class DeviceRegistrationController : Controller
{
    private readonly PublicKeyService _publicKeyService;
    private readonly IKeyMaterialService _keys;

    public DeviceRegistrationController(PublicKeyService publicKeyService, IKeyMaterialService keys)
    {
        _publicKeyService = publicKeyService;
        _keys = keys;
    }

    /// <summary>
    /// Unsecure API which creates a session
    /// DDOS protection required
    /// Add nonce to prevent reply attacks
    /// </summary>
    /// <param name="publicKey">Public key which is used by the session creator</param>
    [HttpPost]
    public async Task<DeviceRegistrationResponse> CreateAuthSessionAsync(DeviceRegistrationRequest deviceRegistrationRequest)
    {
        // TODO
        // Validate client_id
        // Validate grant_type=fp_register
        // DDoS protection required...
        // Maybe as secret to authenticate, prevent simple bots

        var authSession = _publicKeyService.CreateSession(deviceRegistrationRequest.public_key);
        var signingCredential = await _keys.GetSigningCredentialsAsync();

        var scheme = HttpContext.Request.Scheme;
        var host = HttpContext.Request.Host.Value;
        var issuer = $"{scheme}://{host}";


        var deviceRegistrationResponse = new DeviceRegistrationResponse
        {
            FpToken = GenerateJwtTokenAsync(authSession, 
                deviceRegistrationRequest.nonce, 
                signingCredential, 
                issuer, 
                deviceRegistrationRequest.client_id),

            State = deviceRegistrationRequest.state
        };

        return deviceRegistrationResponse;
    }

    public static string GenerateJwtTokenAsync(string authSession, string nonce, SigningCredentials signingCredentials, string issuer, string clientId)
    {
        var alg = signingCredentials.Algorithm;

        //{
        //  "alg": "RS256",
        //  "kid": "....",
        //  "typ": "fp+jwt",
        //}
        //{
        //    "iss": "https://localhost:5101",
        //    "nbf": 1744120238,
        //    "iat": 1744120238,
        //    "aud": "<client_id>"
        //    "exp": 1744123838,
        //    "auth_session": "AC7E69B69D627CDDA61AF41518B046E1",
        //    "nonce": "<nonce>"
        //}

        var subject = new ClaimsIdentity([
            new Claim("nonce", nonce),
            new Claim("auth_session", authSession),
        ]);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = DateTime.UtcNow.AddMinutes(7),
            IssuedAt = DateTime.UtcNow,
            Audience = clientId,
            Issuer = issuer,
            SigningCredentials = signingCredentials,
            TokenType = "fp+jwt"
        };

        tokenDescriptor.AdditionalHeaderClaims ??= new Dictionary<string, object>();

        if (!tokenDescriptor.AdditionalHeaderClaims.ContainsKey("alg"))
        {
            tokenDescriptor.AdditionalHeaderClaims.Add("alg", alg);
        }

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
