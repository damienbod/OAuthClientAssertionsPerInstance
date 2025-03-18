using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Controllers;

[Authorize(Policy = "onboardinguserpolicy", AuthenticationSchemes = "onboardinguser")]
[Route("api/[controller]")]
public class OnboardingUserController : Controller
{
    private readonly OnboardingUserService _onboardingUserService;

    public OnboardingUserController(OnboardingUserService onboardingUserService)
    {
        _onboardingUserService = onboardingUserService;
    }

    /// <summary>
    /// Session exists in cache, get from cache and create
    /// </summary>
    [HttpPost("StartEmailVerification")]
    public IActionResult StartEmailVerification(string email)
    {
        string sessionId = GetSessionId();

        _onboardingUserService.ProcessSessionAndEmail(sessionId, email);

        // TODO
        // Send email verification email
        // User MUST click link in email on phone
        // This connects the email to the mobile session

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("StartSmsVerification")]
    public IActionResult StartSmsVerification(string phoneNumber)
    {
        var sessionId = GetSessionId();
        _onboardingUserService.ProcessSessionAndPhoneNumber(sessionId, phoneNumber);

        // TODO
        // Send SMS verification

        return NoContent();
    }

    /// <summary>
    /// Session exists in DB, email is verified, now verify phone number
    /// </summary>
    [HttpPost("VerifyPhoneNumberWithSmsCode")]
    public IActionResult VerifyPhoneNumberWithSmsCode(string code)
    {
        var sessionId = GetSessionId();
        _onboardingUserService.VerifyPhoneNumberWithSmsCode(sessionId, code);

        return NoContent();
    }

    private string GetSessionId()
    {
        var sessionIdClaim = User.Claims.FirstOrDefault(c => c.Type == "scope" && c.Value.StartsWith("sessionId"));
        var sessionId = sessionIdClaim.Value.Replace("sessionId:", "");
        return sessionId;
    }
}
