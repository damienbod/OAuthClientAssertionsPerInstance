using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Idp;

public class PublicKeyService
{
    private static readonly Dictionary<string, (string PublicKey, string Alg)> _inMemoryCache = [];

    public string CreateSession(string publicKey, string alg)
    {
        var authSession = RandomNumberGenerator.GetHexString(32);

        // Add to cache with 10 min lifespan
        // DDoS protection required
        _inMemoryCache.Add(authSession, (publicKey, alg));

        return authSession;
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public string GetPublicKey(string authSession)
    {
        var data = _inMemoryCache.GetValueOrDefault(authSession);
        if (data.PublicKey != null)
        {
            return data.PublicKey;
        }

        throw new ArgumentNullException(nameof(authSession), "something went wrong");
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public SecurityKey GetPublicSecurityKey(string authSession)
    {
        var publicKeyPem = _inMemoryCache.GetValueOrDefault(authSession);

        // TODO we should support different alg types
        if (publicKeyPem.PublicKey != null)
        {
            RsaSecurityKey securityKey;
            var key = RSA.Create();
            key.ImportFromPem(publicKeyPem.PublicKey);
            securityKey = new RsaSecurityKey(key);

            return securityKey;
        }

        throw new ArgumentNullException(nameof(authSession), "something went wrong");
    }
}
