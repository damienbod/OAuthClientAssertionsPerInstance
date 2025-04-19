using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AuthFlow;
using System.Text.Json;

namespace ConsolePerInstanceAssertion;

/// <summary>
/// Creates a new key for the application client assertion.
/// The public key is exchanged with the IDP and connected with a session.
/// The application should validate more identity data like an email, sms and connect this to the session.
/// </summary>
public class KeySessionService
{
    /// <summary>
    /// One signing key per application instance
    /// </summary>
    private static (string? AuthSession, SigningCredentials? SigningCredentials) _inMemoryCache = (null, null);

    private AuthFlowConfiguration _authFlowConfiguration = new AuthFlowConfiguration
    {
        ClientId = "cid-fp-device",
        TokenMetadataAddress = "https://localhost:5101/.well-known/openid-configuration",
        TokenAuthority = "https://localhost:5101"
    };

    public async Task<(string? AuthSession, SigningCredentials? SigningCredentials)> CreateGetSessionAsync()
    {
        if (_inMemoryCache.AuthSession != null)
        {
            return _inMemoryCache;
        }
        var rsa2048 = RSA.Create(2048);
        var rsaCertificateKey = new RsaSecurityKey(rsa2048);
        var publicKeyPem = rsa2048.ExportRSAPublicKeyPem();
        var httpClient = new HttpClient();

        var nonce = RandomNumberGenerator.GetHexString(73);
        var state = RandomNumberGenerator.GetHexString(67);
        var clientId = "cid-fp-device";
        //client_id = cid_235saw4r4
        //& grant_type = fp_register
        //& public_key =< public_key >
        //&state =< state >
        //&nonce =< nonce >
        var formData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:fp_register"),
            new KeyValuePair<string, string>("public_key", publicKeyPem),
            new KeyValuePair<string, string>("state", state),
            new KeyValuePair<string, string>("nonce", nonce)
        };

        // Encodes the key-value pairs for the ContentType 'application/x-www-form-urlencoded'
        HttpContent content = new FormUrlEncodedContent(formData);
        var response = await httpClient.PostAsync("https://localhost:5101/api/DeviceRegistration", content);

        if (response.IsSuccessStatusCode)
        {
            var signingCredentials = new SigningCredentials(rsaCertificateKey, "RS256");
            var responseResult = await response.Content.ReadAsStringAsync();
            var deviceRegistrationResponse = JsonSerializer.Deserialize<DeviceRegistrationResponse>(responseResult);

            if (deviceRegistrationResponse == null)
            {
                throw new Exception("no response");
            }
            // TODO
            // Validate state
            // Validate JWT signing credential
            // Validate nbf, exp, iat
            // Validate nonce
            // Validate aud (clientId)
            // Validate iss
            // Validate  "typ": "fp+jwt"

            var (Valid, Reason, Error) = ValidateTokenResponsePayload
            .IsValid(deviceRegistrationResponse, _authFlowConfiguration);

            if (!Valid)
            {
                Console.WriteLine("UnauthorizedValidationParametersFailed {Reason} {Error}", Reason, Error);
                throw new ArgumentNullException("auth_session", "UnauthorizedValidationParametersFailed");
            }

            // get well known endpoints and validate access token sent in the assertion
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                _authFlowConfiguration.TokenMetadataAddress,
                new OpenIdConnectConfigurationRetriever());

            var wellKnownEndpoints = await configurationManager.GetConfigurationAsync();

            var deviceTokenValidationResult = await ValidateTokenResponsePayload.ValidateTokenAndSignature(
                deviceRegistrationResponse.FpToken, _authFlowConfiguration, wellKnownEndpoints.SigningKeys);

            if (!deviceTokenValidationResult.Valid)
            {
                Console.WriteLine("UnauthorizedValidationTokenAndSignatureFailed {Reason} {Error}", Reason, Error);
                throw new ArgumentNullException("auth_session", "UnauthorizedValidationTokenAndSignatureFailed");
            }

            var nonceInResponse = ValidateTokenResponsePayload.GetNonce(deviceTokenValidationResult.ClaimsIdentity!);
            if (nonceInResponse != nonce)
            {
                Console.WriteLine("Nonce validation failed");
                throw new ArgumentNullException("auth_session", "Nonce validation failed");
            }

            var authSession = ValidateTokenResponsePayload.GetAuthSession(deviceTokenValidationResult.ClaimsIdentity!);
            _inMemoryCache = (authSession, signingCredentials);

            // TODO persist key in TPM and re-use

            return _inMemoryCache;
        }

        throw new ArgumentNullException("auth_session", "something went wrong");
    }
}
