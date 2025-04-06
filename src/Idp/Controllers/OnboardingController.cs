using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class OnboardingController : Controller
{
    private readonly PublicKeyService _publicKeyService;

    public OnboardingController(PublicKeyService publicKeyService)
    {
        _publicKeyService = publicKeyService;
    }

    /// <summary>
    /// Unsecure API which creates a session
    /// DDOS protection required
    /// Add nonce to prevent reply attacks
    /// </summary>
    /// <param name="publicKey">Public key which is used by the session creator</param>
    [HttpPost]
    public string CreateSession(string publicKey)
    {
        // validation and DDoS protection required...

        // Encypt sessionId using publickey
        // Use PKCE
        // Add nonce and state parameters as in code flow
        // send request in body
        return _publicKeyService.CreateSession(publicKey);
    }
}
