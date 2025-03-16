using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdpPreInstanceAssertion.Controllers;

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
    /// Convert.ToBase64String(rsaCertificate.GetRawCertData())
    /// </summary>
    /// <param name="publicKey">Public key which is used by the session creator</param>
    /// <returns></returns>
    [HttpPost]
    public string CreateSession(string publicKey)
    {
        // validation and DDoS protection required...

        return _publicKeyService.CreateSession(publicKey);
    }
}
