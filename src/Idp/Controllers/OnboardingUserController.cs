using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Controllers;

[Authorize(Policy = "onboardinguserpolicy", AuthenticationSchemes = "onboardinguser")]
[Route("api/[controller]")]
public class OnboardingUserController : Controller
{
    private readonly PublicKeyService _publicKeyService;

    public OnboardingUserController(PublicKeyService publicKeyService)
    {
        _publicKeyService = publicKeyService;
    }

    /// <summary>
    /// Session exists in cache, get from cache and create
    /// </summary>
    [HttpPost("StartEmailVerification")]
    public IActionResult StartEmailVerification(string sessionId)
    {
        var publicKey = _publicKeyService.GetPublicKey(sessionId);

        // TODO
        // Add publicKey, sessionId to DB
        // Send email verification email

        return NoContent();
    }
}
