using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Idp;

public class PublicKeyService
{
    private static readonly Dictionary<string, string> _inMemoryCache = [];

    public string CreateSession(string publicKey)
    {
        var authSession = RandomNumberGenerator.GetHexString(32);

        // Add to cache with 10 min lifespan
        // DDoS protection required
        _inMemoryCache.Add(authSession, publicKey);

        return authSession;
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public string GetPublicKey(string authSession)
    {
        var data = _inMemoryCache.GetValueOrDefault(authSession);
        if (data != null)
        {
            return data;
        }

        throw new ArgumentNullException(nameof(authSession), "something went wrong");
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public SecurityKey GetPublicSecurityKey(string authSession)
    {
        var publicKeyPem = _inMemoryCache.GetValueOrDefault(authSession);
        if (publicKeyPem != null)
        {
            RsaSecurityKey securityKey;
            var key = RSA.Create();
            key.ImportFromPem(publicKeyPem);
            securityKey = new RsaSecurityKey(key);

            return securityKey;
        }

        throw new ArgumentNullException(nameof(authSession), "something went wrong");
    }
}
