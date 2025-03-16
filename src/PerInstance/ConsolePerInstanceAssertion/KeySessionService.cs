using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdpPreInstanceAssertion;

public class KeySessionService
{
    private static (string? SessionId, SigningCredentials? SigningCredentials) _inMemoryCache = (null, null);

    public async Task<(string? SessionId, SigningCredentials? SigningCredentials)> CreateGetSessionAsync()
    {
        if(_inMemoryCache.SessionId != null)
        {
            return _inMemoryCache;
        }

        var cert = RSA.Create(2048);
        var rsaCertificateKey = new RsaSecurityKey(cert);
        var publicKey = cert.ExportRSAPublicKey();
        var pKeyBase64 = Convert.ToBase64String(publicKey);

        var httpClient = new HttpClient();
        var response = await httpClient.PostAsync("https://localhost:5001/api/Onboarding", new StringContent(pKeyBase64));
        if(response.IsSuccessStatusCode)
        {
            var signingCredentials = new SigningCredentials(rsaCertificateKey, "RS256");
            var sessionId = await response.Content.ReadAsStringAsync();

            _inMemoryCache = (sessionId, signingCredentials);
            return _inMemoryCache;
        }

        throw new ArgumentNullException("sessionId", "something went wrong");
    }
}
