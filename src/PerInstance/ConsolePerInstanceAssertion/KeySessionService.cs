using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ConsoleDPoPClientAssertions;

public class KeySessionService
{
    private static (string? SessionId, SigningCredentials? SigningCredentials) _inMemoryCache = (null, null);

    public async Task<(string? SessionId, SigningCredentials? SigningCredentials)> CreateGetSessionAsync()
    {
        if (_inMemoryCache.SessionId != null)
        {
            return _inMemoryCache;
        }

        var cert = RSA.Create(2048);
        var rsaCertificateKey = new RsaSecurityKey(cert);
        var publicKeyPem = cert.ExportRSAPublicKeyPem();

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
            var sessionId = await response.Content.ReadAsStringAsync();

            _inMemoryCache = (sessionId, signingCredentials);

            // TODO persist key in TPM and re-use

            return _inMemoryCache;
        }

        throw new ArgumentNullException("sessionId", "something went wrong");
    }
}
