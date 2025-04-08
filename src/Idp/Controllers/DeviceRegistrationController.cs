using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class DeviceRegistrationController : Controller
{
    private readonly PublicKeyService _publicKeyService;

    public DeviceRegistrationController(PublicKeyService publicKeyService)
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
    public string CreateAuthSession(string publicKey)
    {
        // TODO
        // validation and DDoS protection required...
        // Maybe as secret to authenticate, prevent simple bots

        // TODO
        // Encypt auth_session using publickey
        // Use PKCE with email as second step
        // Add nonce and state parameters as in code flow
        // send request in body
        // return signed JWT with payload
        return _publicKeyService.CreateSession(publicKey);
    }
}
