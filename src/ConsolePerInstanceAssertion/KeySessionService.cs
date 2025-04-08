using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

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

        var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("publicKey", publicKeyPem)
            };

        // Encodes the key-value pairs for the ContentType 'application/x-www-form-urlencoded'
        HttpContent content = new FormUrlEncodedContent(formData);
        var response = await httpClient.PostAsync("https://localhost:5101/api/Onboarding", content);

        if (response.IsSuccessStatusCode)
        {
            var signingCredentials = new SigningCredentials(rsaCertificateKey, "RS256");
            var auth_session = await response.Content.ReadAsStringAsync();

            _inMemoryCache = (auth_session, signingCredentials);

            // TODO persist key in TPM and re-use

            return _inMemoryCache;
        }

        throw new ArgumentNullException("auth_session", "something went wrong");
    }
}
