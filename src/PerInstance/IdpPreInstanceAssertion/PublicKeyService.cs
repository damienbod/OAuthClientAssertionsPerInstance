using System.Security.Cryptography;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdpPreInstanceAssertion;

public class PublicKeyService
{
    private static readonly Dictionary<string, string> _inMemoryCache = [];

    public string CreateSession(string publicKey)
    {
        // TODO validate publicKey

        var sessionId =  RandomNumberGenerator.GetHexString(32);

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
        if(data != null)
        {
            return data;
        }

        throw new ArgumentNullException(nameof(sessionId), "something went wrong");
    }

    /// <summary>
    /// Get public key from cache
    /// </summary>
    public Secret GetPublicKeySecret(string sessionId)
    {
        var keyData = _inMemoryCache.GetValueOrDefault(sessionId);
        if (keyData != null)
        {
            return new Secret
            {
                // X509 cert base64-encoded
                Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                Value = keyData
            };
        }

        throw new ArgumentNullException(nameof(sessionId), "something went wrong");
    }
}
