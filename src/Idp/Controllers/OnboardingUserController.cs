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
    public IActionResult StartEmailVerification(string sessionId, string email)
    {
        var publicKey = _publicKeyService.GetPublicKey(sessionId);

        // TODO
        // Add publicKey, sessionId to DB
        // Send email verification email

        // User MUST click link in email on phone
        // This connects the email to the mobile session

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("StartSmsVerification")]
    public IActionResult StartSmsVerification(string sessionId, string phoneNumber)
    {
        var publicKey = _publicKeyService.GetPublicKey(sessionId);

        // TODO
        // Update DB
        // Send SMS verification

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("VerifyPhoneNumberWithSmsCode")]
    public IActionResult VerifyPhoneNumberWithSmsCode(string sessionId, string code)
    {
        var publicKey = _publicKeyService.GetPublicKey(sessionId);

        // TODO
        // Update DB
        // Send SMS verification

        return NoContent();
    }
}
