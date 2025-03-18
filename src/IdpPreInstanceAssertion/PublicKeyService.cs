using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace IdpPreInstanceAssertion;

public class PublicKeyService
{
    private static readonly Dictionary<string, string> _inMemoryCache = [];

    public string CreateSession(string publicKey)
    {
        var sessionId = RandomNumberGenerator.GetHexString(32);

        // Add to cache with 10 min lifespan
        // DDoS protection required
        _inMemoryCache.Add(sessionId, publicKey);

        return sessionId;
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public string GetPublicKey(string sessionId)
    {
        var data = _inMemoryCache.GetValueOrDefault(sessionId);
        if (data != null)
        {
            return data;
        }

        throw new ArgumentNullException(nameof(sessionId), "something went wrong");
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public SecurityKey GetPublicSecurityKey(string sessionId)
    {
        var publicKeyPem = _inMemoryCache.GetValueOrDefault(sessionId);
        if (publicKeyPem != null)
        {
            RsaSecurityKey securityKey;
            var key = RSA.Create();
            key.ImportFromPem(publicKeyPem);
            securityKey = new RsaSecurityKey(key);

            return securityKey;
        }

        throw new ArgumentNullException(nameof(sessionId), "something went wrong");
    }
}
